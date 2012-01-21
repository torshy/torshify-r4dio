using System;
using System.IO;

namespace Torshify.Radio.Framework
{
    public class AppConstants
    {
        #region Fields

        public static readonly string AppDataFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "r4dio");
        public static readonly string BackdropCacheFolder = Path.Combine(
            AppDataFolder, "Backdrops");
        public static readonly string CoverArtCacheFolder = Path.Combine(
            AppDataFolder, "CoverArts");
        public static readonly string LogFolder = Path.Combine(
            AppDataFolder, "Logs");

        #endregion Fields
    }
}