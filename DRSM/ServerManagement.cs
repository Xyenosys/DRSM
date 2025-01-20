using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRSM
{
    internal class ServerManagement
    {
        private static Process _serverProcess;

        // Path to the Rust server executable
        private static readonly string ServerExecutable = "/home/steam/rust_server/RustDedicated";

        // Arguments for the server (from Globals.ServerArguments)
        public static string ServerArguments = Globals.ServerArguments;

        // Starts the Rust server
        public static void StartServer()
        {
            if (_serverProcess != null && !_serverProcess.HasExited)
            {
                Console.WriteLine("Server is already running.");
                return;
            }

            if (!File.Exists(ServerExecutable))
            {
                Console.WriteLine($"Server executable not found at {ServerExecutable}");
                return;
            }

            try
            {
                Console.WriteLine("Starting Rust server...");

                _serverProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = ServerExecutable,
                        Arguments = ServerArguments,
                        UseShellExecute = false,
                        WorkingDirectory = Globals.ServerFolder,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        RedirectStandardInput = true, // For future interaction
                        CreateNoWindow = true
                    }
                };

                _serverProcess.OutputDataReceived += (sender, args) =>
                {
                    if (!string.IsNullOrEmpty(args.Data))
                    {
                        Console.WriteLine(args.Data); // Stream server output to the console
                    }
                };

                _serverProcess.ErrorDataReceived += (sender, args) =>
                {
                    if (!string.IsNullOrEmpty(args.Data))
                    {
                        Console.WriteLine($"ERROR: {args.Data}"); // Stream server errors to the console
                    }
                };

                _serverProcess.Start();
                _serverProcess.BeginOutputReadLine();
                _serverProcess.BeginErrorReadLine();
                ProcessResourceMonitor.StartMonitoring(_serverProcess);

                Console.WriteLine($"Rust server started successfully with PID: {_serverProcess.Id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to start Rust server: {ex.Message}");
            }
        }

        // Stops the Rust server
        public static void StopServer()
        {
            if (_serverProcess == null || _serverProcess.HasExited)
            {
                Console.WriteLine("Server is not running.");
                return;
            }

            try
            {
                Console.WriteLine("Stopping Rust server...");

                _serverProcess.Kill();
                _serverProcess.WaitForExit();

                Console.WriteLine("Rust server stopped successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to stop Rust server: {ex.Message}");
            }
            finally
            {
                _serverProcess = null;
            }
        }

        // Restarts the Rust server
        public static void RestartServer()
        {
            Console.WriteLine("Restarting Rust server...");

            // Stop the server if it's running
            StopServer();

            // Wait briefly before restarting
            Thread.Sleep(2000);

            // Start the server again
            StartServer();
        }
    }
}
