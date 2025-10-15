using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CryptoWidget.Services;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using System.Collections.ObjectModel;
using System.Collections.Generic;
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

        public KLineWindow? _kLineWindow;

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
                // 建立現有項目的字典，以 Symbol 為鍵
                var existingItems = PriceItems.ToDictionary(item => item.Symbol, item => item);
                var currentSymbols = new HashSet<string>();

                // 處理每個交易對
                foreach (var symbol in Settings.CryptoList)
                {
                    try
                    {
                        // 獲取K棒資料（10根K棒）
                        var kLineData = await ExchangeService.GetKLineDataAsync(symbol, Settings.SelectedTimeframe, 10, Settings.SelectedExchange);
                        
                        if (kLineData != null && kLineData.Any())
                        {
                            // 從最新K棒獲取當前價格（Close價格）
                            var latestKLine = kLineData.Last();
                            var currentPrice = latestKLine.Close.ToString();

                            if (existingItems.TryGetValue(symbol, out var existingItem))
                            {
                                // 存在：更新價格和K棒資料
                                existingItem.Price = currentPrice;
                                existingItem.UpdateKLineData(new ObservableCollection<KLineData>(kLineData));
                            }
                            else
                            {
                                // 不存在：新增項目
                                var newItem = new PriceItem
                                {
                                    Symbol = symbol,
                                    Price = currentPrice,
                                    InputValue = ""
                                };
                                newItem.UpdateKLineData(new ObservableCollection<KLineData>(kLineData));
                                PriceItems.Add(newItem);
                            }
                            
                            currentSymbols.Add(symbol);
                        }
                    }
                    catch
                    {
                        // 單個交易對獲取失敗時，標記為錯誤但繼續處理其他交易對
                        if (existingItems.TryGetValue(symbol, out var existingItem))
                        {
                            existingItem.Price = "Error";
                        }
                        else
                        {
                            var newItem = new PriceItem
                            {
                                Symbol = symbol,
                                Price = "Error",
                                InputValue = ""
                            };
                            PriceItems.Add(newItem);
                        }
                        currentSymbols.Add(symbol);
                    }
                }

                // 移除不再存在的項目
                var itemsToRemove = PriceItems.Where(item => !currentSymbols.Contains(item.Symbol)).ToList();
                foreach (var item in itemsToRemove)
                {
                    PriceItems.Remove(item);
                }

                // 根據設定檔中的順序重新排序
                var orderedSymbols = Settings.CryptoList.ToList();
                for (int i = 0; i < orderedSymbols.Count; i++)
                {
                    var symbol = orderedSymbols[i];
                    var item = PriceItems.FirstOrDefault(p => p.Symbol == symbol);
                    if (item != null)
                    {
                        var currentIndex = PriceItems.IndexOf(item);
                        if (currentIndex != i)
                        {
                            PriceItems.Move(currentIndex, i);
                        }
                    }
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

                if (mainWindow is not null && mainWindow.IsVisible)
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

        [RelayCommand]
        private void OpenKLine()
        {
            if (_kLineWindow is null || !_kLineWindow.IsVisible)
            {
                var mainWindow = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;

                _kLineWindow = new KLineWindow(new KLineViewModel(_settingViewModel));

                if (mainWindow is not null && mainWindow.IsVisible)
                {
                    _kLineWindow.WindowStartupLocation = WindowStartupLocation.Manual;

                    var rightX = mainWindow.Position.X + (int)mainWindow.Bounds.Width;
                    var sameY = mainWindow.Position.Y;
                    _kLineWindow.Position = new PixelPoint(rightX, sameY);

                    _kLineWindow.Closed += (_, __) => _kLineWindow = null;
                    _kLineWindow.Show(mainWindow);
                }
                else
                {
                    _kLineWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    _kLineWindow.Closed += (_, __) => _kLineWindow = null;
                    _kLineWindow.Show();
                }
            }
            else
            {
                _kLineWindow.Activate();
            }
        }
    }


}
