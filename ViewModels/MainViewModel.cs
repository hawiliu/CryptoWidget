using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CryptoWidget.Services;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using CryptoWidget.Services.Dto;

namespace CryptoWidget.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        private readonly SettingViewModel _settingViewModel;

        public SettingsWindow? _settingWindow;

        public AboutWindow? _aboutWindow;

        public ExchangePositionsWindow? _exchangePositionsWindow;

        public MainViewModel(SettingViewModel settingViewModel)
        {
            _settingViewModel = settingViewModel;

            _timer.Elapsed += async (s, e) => await UpdatePrices();
            _timer.Start();
        }

        public SettingViewModel Settings { get { return _settingViewModel; } }   // 供 XAML 綁定

        [ObservableProperty]
        private ObservableCollection<PriceItem> priceItems = new() { new PriceItem() { Symbol = "Loading..." } };

        [ObservableProperty]
        private bool hasData = false;

        [ObservableProperty]
        private string statusStr = "Loading...";

        private readonly Timer _timer = new Timer(5000); // 每 5 秒更新

        private async Task UpdatePrices()
        {
            try
            {
                var prices = await ExchangeService.GetCryptoPricesAsync(Settings.CryptoList.ToList(), Settings.SelectedExchange);
                
                // 建立現有項目的字典，以 Symbol 為鍵
                var existingItems = PriceItems.ToDictionary(item => item.Symbol, item => item);
                
                // 處理每個價格項目
                foreach (var price in prices)
                {
                    if (existingItems.TryGetValue(price.Key, out var existingItem))
                    {
                        // 存在：更新價格
                        existingItem.Price = price.Value.HasValue ? price.Value.Value.ToString() : "Error";
                        existingItem.Push((double)(price.Value.HasValue ? price.Value.Value : 0));
                    }
                    else
                    {
                        // 不存在：新增項目
                        var newItem = new PriceItem
                        {
                            Symbol = price.Key,
                            Price = price.Value.HasValue ? price.Value.Value.ToString() : "Error",
                            InputValue = ""
                        };
                        newItem.Push((double)(price.Value.HasValue ? price.Value.Value : 0));
                        PriceItems.Add(newItem);
                    }
                }
                
                // 移除不再存在的項目
                var currentSymbols = prices.Keys.ToHashSet();
                var itemsToRemove = PriceItems.Where(item => !currentSymbols.Contains(item.Symbol)).ToList();
                foreach (var item in itemsToRemove)
                {
                    PriceItems.Remove(item);
                }
                
                // 更新資料狀態
                HasData = PriceItems.Count > 0;
                StatusStr = HasData ? "" : "Empty";
            }
            catch
            {
                // 發生錯誤時清空集合
                PriceItems.Clear();
                HasData = false;
                StatusStr = "Error";
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
            if (_settingWindow is null || !_settingWindow.IsVisible)
            {
                _settingWindow = new SettingsWindow(_settingViewModel)
                {
                    WindowStartupLocation = Avalonia.Controls.WindowStartupLocation.CenterOwner
                };
                _settingWindow.Closed += (_, __) => _settingWindow = null;
                _settingWindow.Show();
            }
            else
            {
                _settingWindow.Activate();
            }
        }

        [RelayCommand]
        private void OpenAbout()
        {
            if (_aboutWindow is null || !_aboutWindow.IsVisible)
            {
                _aboutWindow = new AboutWindow(_settingViewModel)
                {
                    WindowStartupLocation = Avalonia.Controls.WindowStartupLocation.CenterOwner
                };
                _aboutWindow.Closed += (_, __) => _aboutWindow = null;
                _aboutWindow.Show();
            }
            else
            {
                _aboutWindow.Activate();
            }
        }

        [RelayCommand]
        private void OpenExchangePositions()
        {
            if (_exchangePositionsWindow is null || !_exchangePositionsWindow.IsVisible)
            {
                var mainWindow = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;

                _exchangePositionsWindow = new ExchangePositionsWindow(_settingViewModel);

                if (mainWindow is not null)
                {
                    _exchangePositionsWindow.WindowStartupLocation = WindowStartupLocation.Manual;

                    var leftX = mainWindow.Position.X;
                    var belowY = mainWindow.Position.Y + (int)mainWindow.Bounds.Height;
                    _exchangePositionsWindow.Position = new PixelPoint(leftX, belowY);

                    _exchangePositionsWindow.Closed += (_, __) => _exchangePositionsWindow = null;
                    _exchangePositionsWindow.Show(mainWindow);
                }
                else
                {
                    _exchangePositionsWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    _exchangePositionsWindow.Closed += (_, __) => _exchangePositionsWindow = null;
                    _exchangePositionsWindow.Show();
                }
            }
            else
            {
                _exchangePositionsWindow.Activate();
            }
        }
    }


}
