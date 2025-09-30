using ccxt;
using CommunityToolkit.Mvvm.ComponentModel;
using CryptoWidget.Services;
using CryptoWidget.Services.Dto;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace CryptoWidget.ViewModels
{
    public partial class ExchangePositionsViewModel : ViewModelBase
    {
        private readonly SettingViewModel _settingViewModel;

        public SettingsWindow? _settingWindow;

        public ExchangePositionsViewModel(SettingViewModel settingViewModel)
        {
            _settingViewModel = settingViewModel;
            if (settingViewModel != null && string.IsNullOrEmpty(settingViewModel.ExchangeApiKey))
            {
                StatusStr = "API Key is empty.";
            }
            else
            {
                _timer.Elapsed += async (s, e) => await UpdatePositions();
                _timer.Start();
            }
        }

        public SettingViewModel Settings { get { return _settingViewModel; } }   // 供 XAML 綁定

        private readonly Timer _timer = new Timer(5000); // 每 5 秒更新

        [ObservableProperty]
        private bool hasApiKey = false;

        [ObservableProperty]
        private bool hasPosition = false;

        [ObservableProperty]
        private string statusStr = "Loading...";

        public bool ShowStatus => !HasApiKey && !HasPosition;

        [ObservableProperty]
        private ObservableCollection<PositionItem> positions = new ObservableCollection<PositionItem>();

        partial void OnHasApiKeyChanged(bool value)
        {
            OnPropertyChanged(nameof(ShowStatus));
        }

        partial void OnHasPositionChanged(bool value)
        {
            OnPropertyChanged(nameof(ShowStatus));
        }

        private async Task UpdatePositions()
        {
            try
            {
                var apiKey = Settings.ExchangeApiKey;
                var apiSecret = Settings.ExchangeApiSecret;
                var exchange = Settings.SelectedExchange;

                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    HasApiKey = false;
                    HasPosition = false;
                    StatusStr = "API Key is empty.";
                    return;
                }

                HasApiKey = true;

                var positions = await ExchangeService.GetOpenPositionsAsync(exchange, apiKey, apiSecret);

                // 建立現有項目的字典，以 Symbol 為鍵
                var existingItems = Positions.ToDictionary(item => item.Symbol, item => item);

                // 處理每個價格項目
                foreach (var position in positions)
                {
                    if (existingItems.TryGetValue(position.Symbol, out var existingItem))
                    {
                        // 存在：更新價格
                        existingItem.UnrealizedPnl = position.UnrealizedPnl.HasValue ? position.UnrealizedPnl.Value : 0;
                    }
                    else
                    {
                        // 不存在：新增項目
                        Positions.Add(position);
                    }
                }

                // 移除不再存在的項目
                var currentSymbols = positions.Select(p => p.Symbol).ToHashSet();
                var itemsToRemove = Positions.Where(item => !currentSymbols.Contains(item.Symbol)).ToList();
                foreach (var item in itemsToRemove)
                {
                    Positions.Remove(item);
                }

                HasPosition = positions != null && positions.Count > 0;
                StatusStr = HasPosition ? string.Empty : "No positions";
            }
            catch
            {
                Positions.Clear();
                HasPosition = false;
                StatusStr = "Error";
            }
        }
    }
}
