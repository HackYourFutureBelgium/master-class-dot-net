using Microsoft.AspNetCore.ResponseCompression;
using TicTacToe.Core;
using TicTacToe.Web;
using TicTacToe.Web.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<GameStatsService>();
builder.Services.AddSingleton<GameEngine>();
builder.Services.AddSingleton<GameRoomManager>();
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddSignalR();
builder.Services.AddResponseCompression(o =>
{
    o.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat([ "application/octet-stream" ]);
});

var app = builder.Build();

app.UseResponseCompression();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapHub<GameHub>("/gamehub");

app.Run();