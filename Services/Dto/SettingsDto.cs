using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoWidget.Services.Dto
{
    public class SettingsDto
    {
        public double OpacityLevel { get; set; }

        public bool KeepOnTop { get; set; }

        public List<string> CryptoList { get; set; }
    }
}
