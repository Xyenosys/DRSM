using DRSM;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.IO.Compression;
using System.Runtime.InteropServices;
//using System.Web.Mvc;

[ApiController]
[Route("api/[controller]")]
public class RustServerController : ControllerBase
{



    [HttpPost("start")]
    public IActionResult StartServer([FromBody] string arguments)
    {
        try
        {
            // Start the server
            ServerManagement.StartServer();

            // Find the server process by name (adjust the name as needed)
            var serverProcess = Process.GetProcessesByName("your-server-process-name").FirstOrDefault();
            if (serverProcess == null)
            {
                return StatusCode(500, "Server process not found after starting.");
            }

            // Start resource monitoring
            var cancellationTokenSource = new CancellationTokenSource();
            HttpContext.Items["MonitoringToken"] = cancellationTokenSource; // Store the token for future stop calls
            ProcessResourceMonitor.StartMonitoring(
                serverProcess,
                resourceData => Console.WriteLine(resourceData), // For now, logs to console
                cancellationTokenSource.Token
            );

            return Ok(new { message = "Server started and monitoring initiated successfully." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error starting server: {ex.Message}");
        }
    }


    // Stop the server by sending a POST request
    [HttpPost("stop")]
    public IActionResult StopServer()
    {
        try
        {
            ServerManagement.StopServer();
            return Ok("Server stopped successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error stopping server: {ex.Message}");
        }
    }

    // restart the server by sending a POST request
    [HttpPost("restart")]
    public IActionResult restart()
    {
        try
        {
            ServerManagement.RestartServer();
            return Ok("Server restarted successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error restarting server: {ex.Message}");
        }
    }

    [HttpPost("install")]
    public IActionResult install()
    {
        try
        {
            SteamCmd.InstallRustServer(Globals.ServerFolder);
            return Ok("Server restarted successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error restarting server: {ex.Message}");
        }
    }

    [HttpPost("download-umod")]
    public async Task<IActionResult> DownloadUMod()
    {
        try
        {
            DownloadManager.DownloadUmod();
            return Ok("Server restarted successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error restarting server: {ex.Message}");
        }
    }

    [HttpPost("download-rustedit")]
    public async Task<IActionResult> DownloadRustEditExtension()
    {
        try
        {
            DownloadManager.DownloadRustedit();
            return Ok("Server restarted successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error restarting server: {ex.Message}");
        }
    }

}
