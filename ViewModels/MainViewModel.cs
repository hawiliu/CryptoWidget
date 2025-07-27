using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CryptoWidget.Services;
using System.Threading.Tasks;
using System.Timers;

namespace CryptoWidget.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        private readonly SettingsService _settingsService;

        public MainViewModel(SettingsService settingsService)
        {
            _settingsService = settingsService;

            UpdatePrices();
            _timer.Elapsed += async (s, e) => await UpdatePrices();
            _timer.Start();
        }

        public SettingsService Settings { get { return _settingsService; } }   // 供 XAML 綁定

        [ObservableProperty]
        private string btcPrice = "BTC: Loading...";

        [ObservableProperty]
        private string ethPrice = "ETH: Loading...";

        private readonly Timer _timer = new Timer(5000); // 每 5 秒更新



        private async Task UpdatePrices()
        {
            try
            {
                var prices = await PriceService.GetCryptoPricesAsync();
                BtcPrice = $"BTC: {prices["BTC/USDT"]:F2} USD";
                EthPrice = $"ETH: {prices["ETH/USDT"]:F2} USD";
            }
            catch
            {
                BtcPrice = "BTC: Error";
                EthPrice = "ETH: Error";
            }
        }

        [RelayCommand]
        private void OpenSettings()
        {
            var win = new SettingsWindow(_settingsService)
            {
                WindowStartupLocation = Avalonia.Controls.WindowStartupLocation.CenterOwner
            };
            win.Show();
        }
    }
}
