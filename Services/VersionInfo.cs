using System.Reflection;

namespace CryptoWidget.Services
{
    public static class VersionInfo
    {
        public static string Version
        {
            get
            {
                var assembly = Assembly.GetExecutingAssembly();
                var version = assembly.GetName().Version;
                return version?.ToString() ?? "1.0.0.0";
            }
        }

        public static string Author
        {
            get
            {
                var assembly = Assembly.GetExecutingAssembly();
                var companyAttribute = assembly.GetCustomAttribute<AssemblyCompanyAttribute>();
                return companyAttribute?.Company ?? "Hawiliu";
            }
        }

        public static string Description
        {
            get
            {
                var assembly = Assembly.GetExecutingAssembly();
                var descriptionAttribute = assembly.GetCustomAttribute<AssemblyDescriptionAttribute>();
                return descriptionAttribute?.Description ?? "加密貨幣價格監控小工具";
            }
        }

        public static string Copyright
        {
            get
            {
                var assembly = Assembly.GetExecutingAssembly();
                var copyrightAttribute = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>();
                return copyrightAttribute?.Copyright ?? "Copyright © 2025 Hawiliu All Rights Reserved";
            }
        }

        public static string ProductName
        {
            get
            {
                var assembly = Assembly.GetExecutingAssembly();
                var productAttribute = assembly.GetCustomAttribute<AssemblyProductAttribute>();
                return productAttribute?.Product ?? "CryptoWidget";
            }
        }

        public static string GetFullVersionInfo()
        {
            return $"{ProductName} v{Version}";
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