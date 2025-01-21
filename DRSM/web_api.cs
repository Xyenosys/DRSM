using DRSM;
using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;
using System.Runtime.InteropServices;
//using System.Web.Mvc;

[ApiController]
[Route("api/[controller]")]
public class RustServerController : ControllerBase
{

   

    // Start the server by sending a POST request with server arguments
    [HttpPost("start")]
    public IActionResult StartServer([FromBody] string arguments)
    {
        try
        {
            ServerManagement.StartServer();
            return Ok("Server started successfully.");
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
