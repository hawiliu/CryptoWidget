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
        private readonly SettingViewModel _settingViewModel;

        public SettingsWindow? _settingWindow;

        public AboutWindow? _aboutWindow;

        public MainViewModel(SettingViewModel settingViewModel)
        {
            _settingViewModel = settingViewModel;

            _timer.Elapsed += async (s, e) => await UpdatePrices();
            _timer.Start();
        }

        public SettingViewModel Settings { get { return _settingViewModel; } }   // 供 XAML 綁定

        [ObservableProperty]
        private string priceLines = "Loading...";

        private readonly Timer _timer = new Timer(5000); // 每 5 秒更新

        private async Task UpdatePrices()
        {
            try
            {
                var prices = await PriceService.GetCryptoPricesAsync(Settings.CryptoList.ToList(), Settings.SelectedExchange);
                if (prices.Count == 0)
                    PriceLines = "Empty";
                else
                    PriceLines = string.Join('\n', prices.Select(p =>
                    {
                        if (p.Value.HasValue)
                        {
                            return $"{p.Key}:\t{p.Value.Value}";
                        }
                        else
                        {
                            return $"{p.Key}:\tError";
                        }
                    }));
            }
            catch
            {
                PriceLines = "Error";
            }
        }

        private string FormatPrice(double price)
        {
            if (price >= 1.0)
            {
                return price.ToString("F2");
            }
            else if (price >= 0.01)
            {
                return price.ToString("F4");
            }
            else if (price >= 0.0001)
            {
                return price.ToString("F6");
            }
            else if (price >= 0.000001)
            {
                return price.ToString("F8");
            }
            else
            {
                return price.ToString("F10");
            }
        }

        [RelayCommand]
        private void OpenSettings()
        {
            _settingWindow = new SettingsWindow(_settingViewModel)
            {
                WindowStartupLocation = Avalonia.Controls.WindowStartupLocation.CenterOwner
            };
            _settingWindow.Show();
        }

        [RelayCommand]
        private void OpenAbout()
        {
            _aboutWindow = new AboutWindow(_settingViewModel)
            {
                WindowStartupLocation = Avalonia.Controls.WindowStartupLocation.CenterOwner
            };
            _aboutWindow.Show();
        }
    }
}
