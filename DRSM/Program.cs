//using DRSM;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.Extensions.Hosting;
//using System;
//using System.Net.WebSockets;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

//class Program
//{
//    static void Main()
//    {

//        try
//        {
//            SteamCmd.SetupSteamCmd();
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine("Error: " + ex.Message);
//        }
//        var builder = WebApplication.CreateBuilder();
//        builder.Services.AddControllers();
//        builder.Services.AddEndpointsApiExplorer();

//        var app = builder.Build();


//        // Enable serving static files from the wwwroot folder
//        app.UseWebSockets();

//        app.Map("/ws", async context =>
//        {
//            if (context.WebSockets.IsWebSocketRequest)
//            {
//                using var webSocket = await context.WebSockets.AcceptWebSocketAsync();

//                // Send resource usage data every second
//                await SendResourceUsage(webSocket);
//            }
//            else
//            {
//                context.Response.StatusCode = 400; // Bad Request
//            }
//        });

//        // Map a fallback to serve index.html for the frontend
//        app.MapFallbackToFile("index.html");

//        // Configure HTTP request pipeline
//        app.MapControllers();

//        // Configure HTTP request pipeline
//        // Map actions for start, stop, restart, and install
//        app.MapPost("/api/start", async () =>
//        {
//            // Call your StartServer method
//            await Task.Run(() => ServerManagement.StartServer());
//            return Results.Ok("Server started successfully.");
//        });

//        app.MapPost("/api/stop", async () =>
//        {
//            // Call your StopServer method
//            await Task.Run(() => ServerManagement.StopServer());
//            return Results.Ok("Server stopped successfully.");
//        });

//        app.MapPost("/api/restart", async () =>
//        {
//            // Call your RestartServer method
//            await Task.Run(() => ServerManagement.RestartServer());
//            return Results.Ok("Server restarted successfully.");
//        });

//        app.MapPost("/api/install", async () =>
//        {
//            // Call your Install method
//            await Task.Run(() => SteamCmd.InstallRustServer(Globals.ServerFolder));
//            return Results.Ok("Server installed successfully.");
//        });

//        app.MapPost("/api/download-umod", async () =>
//        {
//            // Call your Install method
//            await Task.Run(() => DownloadManager.DownloadUmod());
//            return Results.Ok("Umod downloaded and installed successfully.");
//        });

//        app.MapPost("/api/download-rustedit", async () =>
//        {
//            // Call your Install method
//            await Task.Run(() => DownloadManager.DownloadRustedit());
//            return Results.Ok("Umod downloaded and installed successfully.");
//        });

//        // Run the web server
//        app.Run("http://0.0.0.0:5000");




//        Console.ReadLine();
//    }
//    private static async Task SendResourceUsage(WebSocket webSocket)
//    {
//        var buffer = new byte[1024 * 4];

//        while (webSocket.State == WebSocketState.Open)
//        {
//            try
//            {
//                // Simulating resource usage data
//                var cpuUsage = "CPU Usage: 10%";
//                var ramUsage = "RAM Usage: 512MB";

//                var message = $"{cpuUsage}\n{ramUsage}";
//                var byteArray = Encoding.UTF8.GetBytes(message);
//                var segment = new ArraySegment<byte>(byteArray);

//                await webSocket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);

//                // Wait 1 second before sending new data
//                await Task.Delay(1000);
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error sending data: {ex.Message}");
//                break;
//            }
//        }

//        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
//    }
//}
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using DRSM;
using System.Diagnostics;

public class Program
{

    private static CancellationTokenSource _monitoringCancellationTokenSource;


    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.WebHost.ConfigureKestrel(serverOptions =>
        {
            serverOptions.ListenAnyIP(5000); // Port 5000
        });

        // Add CORS policy
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins("http://localhost:8091") // Allow the frontend at port 8091
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials();
            });
        });

        // Ensure the application listens on port 5000


        var app = builder.Build();
        app.UseCors("AllowAllOrigins"); 
        app.UseWebSockets();
        app.UseRouting();
        app.UseCors();

        // WebSocket route to send resource usage (CPU and RAM)
        app.Map("/ws", async context =>
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                await HandleWebSocketConnection(webSocket);
            }
            else
            {
                context.Response.StatusCode = 400; // Bad Request
            }
        });

        // API route for server control (start, stop, restart, install, etc.)
        app.MapPost("/api/start", async () =>
        {
            await Task.Run(() => ServerManagement.StartServer());
            return Results.Ok("Server started successfully.");
        });

        app.MapPost("/api/stop", async () =>
        {
            await Task.Run(() => ServerManagement.StopServer());
            return Results.Ok("Server stopped successfully.");
        });

        app.MapPost("/api/restart", async () =>
        {
            await Task.Run(() => ServerManagement.RestartServer());
            return Results.Ok("Server restarted successfully.");
        });

        // Polling route to get resource usage
        app.MapGet("/api/resourceusage", async context =>
        {
            // Simulate fetching resource usage data
            var cpuUsage = "CPU Usage: 10%";
            var ramUsage = "RAM Usage: 512MB";

            var resourceData = new { CpuUsage = cpuUsage, RamUsage = ramUsage };
            await context.Response.WriteAsJsonAsync(resourceData);
        });

        // Map the fallback route to serve the index.html (front-end page)
        app.MapFallbackToFile("index.html");

        // Run the app on port 5000
        app.Run("0.0.0.0:5000");
    }

    private static async Task HandleWebSocketConnection(WebSocket webSocket)
    {
        var buffer = new byte[1024];
        while (webSocket.State == WebSocketState.Open)
        {
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Text)
            {
                var command = Encoding.UTF8.GetString(buffer, 0, result.Count).Trim();
                if (command == "start")
                {
                    if (ProcessResourceMonitor._serverProcess == null || ProcessResourceMonitor._serverProcess.HasExited)
                    {
                        ProcessResourceMonitor._serverProcess = Process.GetProcessesByName("your-server-process-name").FirstOrDefault();
                    }

                    if (ProcessResourceMonitor._serverProcess != null)
                    {
                        // Start monitoring
                        _monitoringCancellationTokenSource = new CancellationTokenSource();
                        ProcessResourceMonitor.StartMonitoring(
                            ProcessResourceMonitor._serverProcess,
                            async resourceData =>
                            {
                                if (webSocket.State == WebSocketState.Open)
                                {
                                    var byteArray = Encoding.UTF8.GetBytes(resourceData);
                                    await webSocket.SendAsync(new ArraySegment<byte>(byteArray), WebSocketMessageType.Text, true, CancellationToken.None);
                                }
                            },
                            _monitoringCancellationTokenSource.Token
                        );

                        // Notify frontend
                        await webSocket.SendAsync(
                            Encoding.UTF8.GetBytes("Monitoring started."),
                            WebSocketMessageType.Text,
                            true,
                            CancellationToken.None
                        );
                    }
                    else
                    {
                        await webSocket.SendAsync(
                            Encoding.UTF8.GetBytes("Server process not found."),
                            WebSocketMessageType.Text,
                            true,
                            CancellationToken.None
                        );
                    }
                }
                else if (command == "stop")
                {
                    if (_monitoringCancellationTokenSource != null)
                    {
                        _monitoringCancellationTokenSource.Cancel();
                        _monitoringCancellationTokenSource.Dispose();
                        _monitoringCancellationTokenSource = null;
                        await webSocket.SendAsync(
                            Encoding.UTF8.GetBytes("Monitoring stopped."),
                            WebSocketMessageType.Text,
                            true,
                            CancellationToken.None
                        );
                    }
                }
            }
            else if (result.MessageType == WebSocketMessageType.Close)
            {
                if (_monitoringCancellationTokenSource != null)
                {
                    _monitoringCancellationTokenSource.Cancel();
                    _monitoringCancellationTokenSource.Dispose();
                }
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connection closed", CancellationToken.None);
            }
        }
    }
}
