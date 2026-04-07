namespace Blogs.Application.Extensions;

[ ExcludeFromCodeCoverage ]
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication( this IServiceCollection services )
        => services.AddMediatR( cfg => cfg.RegisterServicesFromAssemblies( AppDomain.CurrentDomain.GetAssemblies() ) );
}
