using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CryptoWidget.Services;
using System.Threading.Tasks;
using System.Timers;

namespace CryptoWidget.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        public SettingsService Settings => SettingsService.Instance;

        [ObservableProperty]
        private string btcPrice = "BTC: Loading...";

        [ObservableProperty]
        private string ethPrice = "ETH: Loading...";

        private readonly Timer _timer = new Timer(5000); // 每 5 秒更新

        public MainViewModel()
        {
            UpdatePrices();
            _timer.Elapsed += async (s, e) => await UpdatePrices();
            _timer.Start();
        }

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
            var win = new SettingsWindow
            {
                DataContext = SettingsService.Instance,
                WindowStartupLocation = Avalonia.Controls.WindowStartupLocation.CenterOwner
            };
            win.Show();
        }
    }
}
