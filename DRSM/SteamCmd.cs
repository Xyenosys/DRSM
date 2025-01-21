using DRSM;
using System.Collections.Concurrent;
using System.Diagnostics;

public class SteamCmd
{

    public static void SetupSteamCmd()
    {
        Console.WriteLine("Setting up SteamCMD...");

        // Create SteamCMD directory as the steam user
        var createDirResult = RunShellCommand("sudo", $"-u {Globals.SteamUser} bash -c \"mkdir -p {Globals.SteamCmdDir} && cd {Globals.SteamCmdDir}\"");
        if (createDirResult.Item1 != 0)
        {
            throw new Exception($"Failed to create SteamCMD directory: {createDirResult.Item2}");
        }

        // Download and extract SteamCMD
        var downloadResult = RunShellCommand("sudo", $"-u {Globals.SteamUser} bash -c \"curl -sqL https://steamcdn-a.akamaihd.net/client/installer/steamcmd_linux.tar.gz | tar zxvf - -C {Globals.SteamCmdDir}\"");
        if (downloadResult.Item1 != 0)
        {
            throw new Exception($"Failed to download and extract SteamCMD: {downloadResult.Item2}");
        }

        Console.WriteLine("SteamCMD setup completed successfully.");
    }

    public static void InstallRustServer(string serverFolder)
    {
        Console.WriteLine("Installing Rust server...");

        try
        {
            if (!Directory.Exists(serverFolder))
            {
                // Ensure the server folder exists
                Console.WriteLine($"Creating server folder: {serverFolder}");
                var createServerDirResult = RunShellCommand("sudo", $"-u {Globals.SteamUser} bash -c \"mkdir -p {serverFolder}\"");
                if (createServerDirResult.Item1 != 0)
                {
                    string error = $"Failed to create server directory: {createServerDirResult.Item2}";
                    Console.WriteLine(error);
                    throw new Exception(error);
                }
            }

            string steamCmdExecutable = Path.Combine(Globals.SteamCmdDir, "steamcmd.sh");
            string installCommand = $"+force_install_dir {serverFolder} +login anonymous +app_update 258550 +quit";

            Console.WriteLine("Starting Rust server installation...");
            var installResult = RunShellCommand("sudo", $"-u {Globals.SteamUser} bash -c \"{steamCmdExecutable} {installCommand}\"");

            if (installResult.Item1 != 0)
            {
                string error = $"Failed to install Rust server: {installResult.Item2}";
                Console.WriteLine(error);
                throw new Exception(error);
            }

            Console.WriteLine("Rust server installed successfully.");
            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during installation: {ex.Message}");
        }
    }

    #region Shell
    private static Tuple<int, string> RunShellCommand(string command, string arguments)
    {
        using (var process = new Process())
        {
            process.StartInfo.FileName = command;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;

            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            return Tuple.Create(process.ExitCode, output + error);
        }
    }
    #endregion

}
