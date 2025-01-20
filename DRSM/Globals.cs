using System.Diagnostics;

namespace DRSM
{
   public class Globals
    {
        public static string SteamUser = "steam";
        public static string SteamHomeDir = $"/home/{SteamUser}";
        public static string SteamCmdDir = Path.Combine(SteamHomeDir, "Steam");
        public static string ServerFolder = Path.Combine(SteamHomeDir, "rust_server");
        public static string ServerDir = Path.Combine(ServerFolder, @"RustDedicated_Data/Managed");
        public static string ServerArguments = " + server.port 28015 +server.worldsize 1000 +server.seed 12345 +server.maxplayers 50";
        public static string umodUrl = "https://umod.org/games/rust/download/develop"; // Official uMod download URL
        public static string umodDownloadPath = "/home/steam/rust_server/umod.zip";
        public static string rustEditUrl = "https://github.com/k1lly0u/Oxide.Ext.RustEdit/raw/master/Oxide.Ext.RustEdit.dll"; // Direct link to the .dll file
    }
}
