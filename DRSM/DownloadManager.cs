using DRSM;
using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;
using System.Runtime.InteropServices;

namespace DRSM

{
    public class DownloadManager
    {
        public static async void DownloadUmod()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    // Download the uMod ZIP file
                    var response = await client.GetAsync(Globals.umodUrl);
                    response.EnsureSuccessStatusCode();

                    var fileData = await response.Content.ReadAsByteArrayAsync();
                    await System.IO.File.WriteAllBytesAsync(Globals.umodDownloadPath, fileData);
                }

                // Extract the ZIP into the Rust server directory
                ZipFile.ExtractToDirectory(Globals.umodDownloadPath, Globals.ServerFolder, true);

                // Clean up the ZIP file after extraction
                System.IO.File.Delete(Globals.umodDownloadPath);

                Console.WriteLine("uMod successfully downloaded and extracted.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to download or extract uMod: {ex.Message}");
            }

        }
        public static async void DownloadRustedit()
        {
            try
            {
                var filePath = Path.Combine(Globals.ServerDir, "Oxide.Ext.RustEdit.dll");

                using (var client = new HttpClient())
                {
                    // Download the RustEdit DLL file
                    var response = await client.GetAsync(Globals.rustEditUrl);
                    response.EnsureSuccessStatusCode();

                    var fileData = await response.Content.ReadAsByteArrayAsync();
                    await System.IO.File.WriteAllBytesAsync(filePath, fileData);
                }

                Console.WriteLine("RustEdit extension successfully downloaded.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to download RustEdit extension: {ex.Message}");
            }
        }
    }
}
