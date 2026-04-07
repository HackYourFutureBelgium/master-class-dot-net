Log.Logger = new LoggerConfiguration().MinimumLevel.Override( "Microsoft", LogEventLevel.Warning )
                                      .Enrich.FromLogContext()
                                      .WriteTo.Console()
                                      .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder( args );
    builder.Host.UseSerilog(
        ( context, _, configuration ) =>
            configuration.ReadFrom.Configuration( context.Configuration )
    );

    // Options
    builder.Services.Configure< RouteOptions >( o => o.LowercaseUrls = true );

    // Services
    // builder.Services.AddLogContext();
    builder.Services.AddHealthChecks();
    builder.Services.AddCors( o => o.AddDefaultPolicy( b => b.WithOrigins( "http://localhost:4040" )
                                                             .WithHeaders( "Authorization" ) ) );
    builder.Services
           .AddControllers( o =>
            {
                // This is required to support JSON PATCH requests
                o.InputFormatters.Insert( 0, NewtonsoftJsonPatchInputFormatterHelper.GetJsonPatchInputFormatter() );
            } )
           .AddJsonOptions( o => o.JsonSerializerOptions.Converters.Add( new JsonStringEnumConverter() ) );
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen( o =>
    {
        o.SwaggerDoc( "v1", new OpenApiInfo
        {
            Title = "Blogs API",
            Description = "",
            Version = "v0.0.0"
        } );
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine( AppContext.BaseDirectory, xmlFile );
        o.IncludeXmlComments( xmlPath );
        o.OperationFilter< JsonPatchDocumentFilter >();
        o.SchemaFilter< IdentitySchemaFilter >();
        o.UseAllOfForInheritance();
        o.UseOneOfForPolymorphism();
        o.CustomOperationIds( d =>
        {
            if ( !d.TryGetMethodInfo( out var methodInfo ) )
                throw new Exception( "Unable to get method info to generate operation ID." );

            var letters = methodInfo.Name.ToCharArray();
            letters[ 0 ] = letters[ 0 ].ToString().ToLowerInvariant()[ 0 ];
            return string.Join( "", letters );
        } );
    } );
    builder.Services.AddResponseCompression( o => o.Providers.Add< BrotliCompressionProvider >() );
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure( builder.Configuration.GetConnectionString( "DefaultConnection" )! );

    // Middleware
    var app = builder.Build();
    // app.UseLogsCorrelator();
    // app.UseGlobalExceptionHandling();
    // app.UseCorrelationHeaders();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHttpsRedirection();
    app.UseCors( p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod() );
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseResponseCompression();
    app.MapControllers();
    app.MapHealthChecks( "/health" );
    app.Run();
}
catch ( Exception e )
{
    Log.Fatal( e, "An unhandled exception occured during bootstrapping" );
}
finally
{
    Log.CloseAndFlush();
}
