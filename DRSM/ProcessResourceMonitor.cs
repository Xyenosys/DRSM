using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public static class ProcessResourceMonitor
{
    private static Process _serverProcess;

    // Constructor takes the process you want to monitor
    public static void StartMonitoring(Process serverProcess)
    {
        _serverProcess = serverProcess;

        // Start a background task to monitor the process every second
        Task.Run(() => MonitorProcessUsage());
    }

    // Monitors the resource usage of the server process
    private static void MonitorProcessUsage()
    {
        while (!_serverProcess.HasExited)
        {
            try
            {
                // Get CPU, RAM, and Disk usage
                var cpuUsage = GetCpuUsage();
                var ramUsage = GetRamUsage();
                var diskUsage = GetDiskUsage();
                var networkUsage = GetNetworkUsage();

                // Print to the console (you can change this to log to a file or elsewhere)
                Console.WriteLine($"CPU Usage: {cpuUsage}%");
                Console.WriteLine($"RAM Usage: {ramUsage}MB");
                Console.WriteLine($"Disk Usage: {diskUsage}%");
                Console.WriteLine($"Network Usage: {networkUsage}");

                // Sleep for a second before the next check
                Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error monitoring process usage: {ex.Message}");
            }
        }
    }

    // Fetches the CPU usage of the process (Linux method)
    private static double GetCpuUsage()
    {
        try
        {
            var process = Process.GetProcessById(_serverProcess.Id);

            // Use 'ps' to get CPU usage (in percentage)
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "ps",
                Arguments = $"-p {_serverProcess.Id} -o %cpu",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var processInfo = Process.Start(processStartInfo);
            var output = processInfo.StandardOutput.ReadToEnd();
            processInfo.WaitForExit();

            // The first line contains the column name, the second line contains the CPU usage
            var lines = output.Split("\n");
            if (lines.Length > 1)
            {
                return double.Parse(lines[1].Trim()); // CPU percentage
            }
            return 0;  // Default if no valid output
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to get CPU usage: {ex.Message}");
            return 0;
        }
    }

    // Fetches the RAM usage of the process in MB (Linux method)
    private static double GetRamUsage()
    {
        try
        {
            var process = Process.GetProcessById(_serverProcess.Id);

            // Use 'ps' to get memory usage in KB and convert to MB
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "ps",
                Arguments = $"-p {_serverProcess.Id} -o rss",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var processInfo = Process.Start(processStartInfo);
            var output = processInfo.StandardOutput.ReadToEnd();
            processInfo.WaitForExit();

            // The first line contains the column name, the second line contains the RAM usage
            var lines = output.Split("\n");
            if (lines.Length > 1)
            {
                return double.Parse(lines[1].Trim()) / 1024;  // Convert from KB to MB
            }
            return 0;  // Default if no valid output
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to get RAM usage: {ex.Message}");
            return 0;
        }
    }

    // Fetches the disk usage of the process (Linux method)
    private static double GetDiskUsage()
    {
        try
        {
            // Use 'df' to get disk usage (for Linux systems)
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "df",
                Arguments = "-h /",  // Get disk usage for root directory (adjust if necessary)
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var process = Process.Start(processStartInfo);
            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            // Parse the output (you can fine-tune this for your specific needs)
            var lines = output.Split("\n");
            foreach (var line in lines)
            {
                if (line.Contains("/"))
                {
                    var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length > 4)
                    {
                        return double.Parse(parts[4].TrimEnd('%'));  // Disk usage percentage
                    }
                }
            }

            return 0;  // Default if no valid output found
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to get disk usage: {ex.Message}");
            return 0;
        }
    }

    // Fetches the network usage of the system (Linux method)
    private static string GetNetworkUsage()
    {
        try
        {
            // Use 'netstat' to get network stats (for Linux systems)
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "netstat",
                Arguments = "-i",  // Get network interface stats
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var process = Process.Start(processStartInfo);
            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            // Parse output to check for network usage related to the server process
            var lines = output.Split("\n");
            string result = "No network usage detected";

            foreach (var line in lines)
            {
                if (line.Contains("eth0") || line.Contains("wlan0"))  // Assuming typical interface names
                {
                    var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length > 6)
                    {
                        result = $"RX: {parts[3]} bytes, TX: {parts[7]} bytes";  // Incoming and outgoing traffic
                    }
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to get network usage: {ex.Message}");
            return "Error";
        }
    }
}
