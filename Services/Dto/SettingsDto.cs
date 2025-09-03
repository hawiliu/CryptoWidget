using System.Collections.Generic;

namespace CryptoWidget.Services.Dto
{
    public class SettingsDto
    {
        public double OpacityLevel { get; set; }

        public bool KeepOnTop { get; set; }

        public bool CloseOnExit { get; set; }

        public string SelectedExchange { get; set; } = "binance";

        public string SelectedLanguage { get; set; } = "en";

        public List<string> CryptoList { get; set; } = new List<string>();
    }
}
