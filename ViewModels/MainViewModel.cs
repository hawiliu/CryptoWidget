using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CryptoWidget.Services;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace CryptoWidget.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        private readonly SettingsService _settingsService;

        public SettingsWindow? _settingWindow;

        public MainViewModel(SettingsService settingsService)
        {
            _settingsService = settingsService;

            UpdatePrices();
            _timer.Elapsed += async (s, e) => await UpdatePrices();
            _timer.Start();
        }

        public SettingsService Settings { get { return _settingsService; } }   // 供 XAML 綁定

        [ObservableProperty]
        private string priceLines = "Loading...";

        private readonly Timer _timer = new Timer(5000); // 每 5 秒更新

        private async Task UpdatePrices()
        {
            try
            {
                var prices = await PriceService.GetCryptoPricesAsync(Settings.CryptoList.ToList());
                if (prices.Count == 0)
                    PriceLines = "Empty";
                else
                    PriceLines = string.Join('\n', prices.Select(p => $"{p.Key}: {p.Value:F2} USD"));
            }
            catch
            {
                PriceLines = "Error";
            }
        }

        [RelayCommand]
        private void OpenSettings()
        {
            _settingWindow = new SettingsWindow(_settingsService)
            {
                WindowStartupLocation = Avalonia.Controls.WindowStartupLocation.CenterOwner
            };
            _settingWindow.Show();
        }
    }
}
