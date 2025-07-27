using System.Reflection;

namespace CryptoWidget.Services
{
    public static class VersionInfo
    {
        public static string Version => "1.0.0";
        public static string Author => "Hawiliu";
        public static string Description => "加密貨幣價格監控小工具";
        public static string Copyright => "Copyright © 2025 Hawiliu All Rights Reserved";
        
        public static string GetFullVersionInfo()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var version = assembly.GetName().Version;
            return $"CryptoWidget v{Version} ({version?.ToString() ?? "Unknown"})";
        }
        
        public static string AboutInfo
        {
            get
            {
                return $"{GetFullVersionInfo()}\n{Description}\n{Author}\n{Copyright}";
            }
        }
        
    }
} 