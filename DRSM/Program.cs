using DRSM;
using System.Diagnostics;

class Program
{
    static void Main()
    {

        try
        {
            SteamCmd.SetupSteamCmd();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();

        var app = builder.Build();

        // Enable serving static files from the wwwroot folder
        app.UseStaticFiles();

        // Map a fallback to serve index.html for the frontend
        app.MapFallbackToFile("index.html");

        // Configure HTTP request pipeline
        app.MapControllers();

        // Configure HTTP request pipeline
        // Map actions for start, stop, restart, and install
        app.MapPost("/api/start", async () =>
        {
            // Call your StartServer method
            await Task.Run(() => ServerManagement.StartServer());
            return Results.Ok("Server started successfully.");
        });

        app.MapPost("/api/stop", async () =>
        {
            // Call your StopServer method
            await Task.Run(() => ServerManagement.StopServer());
            return Results.Ok("Server stopped successfully.");
        });

        app.MapPost("/api/restart", async () =>
        {
            // Call your RestartServer method
            await Task.Run(() => ServerManagement.RestartServer());
            return Results.Ok("Server restarted successfully.");
        });

        app.MapPost("/api/install", async () =>
        {
            // Call your Install method
            await Task.Run(() => SteamCmd.InstallRustServer(Globals.ServerFolder));
            return Results.Ok("Server installed successfully.");
        });

        app.MapPost("/api/download-umod", async () =>
        {
            // Call your Install method
            await Task.Run(() => DownloadManager.DownloadUmod());
            return Results.Ok("Umod downloaded and installed successfully.");
        });

        app.MapPost("/api/download-rustedit", async () =>
        {
            // Call your Install method
            await Task.Run(() => DownloadManager.DownloadRustedit());
            return Results.Ok("Umod downloaded and installed successfully.");
        });

        // Run the web server
        app.Run("http://0.0.0.0:5000");




        Console.ReadLine();
    }