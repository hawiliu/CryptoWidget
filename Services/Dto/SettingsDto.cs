using System.Collections.Generic;

namespace CryptoWidget.Services.Dto
{
    public class SettingsDto
    {
        public double OpacityLevel { get; set; }

        public bool KeepOnTop { get; set; }

        public bool CloseOnExit { get; set; }

        public bool ShowChart { get; set; }

        public bool ShowTemporaryInput { get; set; }

        public double SymbolFontSize { get; set; } = 16;

        public double PriceFontSize { get; set; } = 14;

        public string SelectedExchange { get; set; } = "binance";

        public string ExchangeApiKey { get; set; } = string.Empty;

        public string ExchangeApiSecret { get; set; } = string.Empty;

        public string SelectedLanguage { get; set; } = "en";

        public string SelectedTimeframe { get; set; } = "15m";

        public List<string> CryptoList { get; set; } = new List<string>();
    }
}
