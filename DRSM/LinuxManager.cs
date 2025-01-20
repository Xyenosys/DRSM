using System.Diagnostics;

namespace DRSM
{
    internal class LinuxManager
    {


        public static void CreateSteamUser(string username)
        {
            // 1. Check if the user already exists
            var checkUserResult = RunShellCommand("id", username);
            if (checkUserResult.Item1 == 0)
            {
                Console.WriteLine($"User '{username}' already exists.");
                return;
            }

            // 2. Create the user and set a password
            Console.WriteLine($"Creating user '{username}'...");
            var createUserResult = RunShellCommand("useradd", $"-m {username}");
            if (createUserResult.Item1 != 0)
            {
                throw new Exception($"Failed to create user '{username}': {createUserResult.Item2}");
            }

            Console.WriteLine($"User '{username}' created successfully.");
        }

        public static void switchToSteamUser(string username)
        {
            // 1. Check if the user already exists
            var checkUserResult = RunShellCommand("id", username);
            if (checkUserResult.Item1 == 0)
            {
                Console.WriteLine($"User '{username}' exists.");
                return;
            }

            // 2. Create the user and set a password
            Console.WriteLine($"Creating user '{username}'...");
            var createUserResult = RunShellCommand("sudo", $"-u {username} -s && cd /home/steam");
            if (createUserResult.Item1 != 0)
            {
                throw new Exception($"Failed to create user '{username}': {createUserResult.Item2}");
            }

            Console.WriteLine($"User '{username}' created successfully.");
        }

        public static void ConfigureSudoNoPassword(string username)
        {
            Console.WriteLine($"Configuring sudo with no password for user '{username}'...");

            // Add an entry in the sudoers file for the steam user
            string sudoersEntry = $"{username} ALL=(ALL) NOPASSWD:ALL";
            string command = $"echo \"{sudoersEntry}\" | sudo tee /etc/sudoers.d/{username}";

            var result = RunShellCommand("bash", $"-c \"{command}\"");
            if (result.Item1 != 0)
            {
                throw new Exception($"Failed to configure sudo for '{username}': {result.Item2}");
            }

            Console.WriteLine($"Sudo configured for '{username}' with no password.");
        }




        // Helper method to run shell commands
        static Tuple<int, string> RunShellCommand(string command, string arguments)
        {
            using (var process = new Process())
            {
                process.StartInfo.FileName = command;
                process.StartInfo.Arguments = arguments;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.WorkingDirectory = "/tmp"; // Ensure a valid directory


                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                int exitCode = process.ExitCode;
                if (exitCode != 0)
                {
                    return Tuple.Create(exitCode, error);
                }
                else
                {
                    return Tuple.Create(exitCode, output);
                }
            }
        }
    }
}
