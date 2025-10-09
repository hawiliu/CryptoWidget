using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CryptoWidget.Services;
using CryptoWidget.Services.Dto;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia;
using System;

namespace CryptoWidget.ViewModels
{
    public partial class KLineViewModel : ViewModelBase
    {
        private readonly SettingViewModel _settingViewModel;

        public KLineViewModel(SettingViewModel settingViewModel)
        {
            _settingViewModel = settingViewModel;
            
            // 初始化時區選項
            Timeframes = new ObservableCollection<string>
            {
                "1m", "3m", "5m", "15m", "30m", "1h", "2h", "4h", "6h", "8h", "12h", "1d", "3d", "1w", "1M"
            };
            SelectedTimeframe = "15m"; // 預設選擇15分鐘
            
            SymbolInput = ""; // 預設交易對
        }

        public SettingViewModel Settings { get { return _settingViewModel; } }

        [ObservableProperty]
        private string symbolInput = "";

        [ObservableProperty]
        private string selectedTimeframe = "1h";

        [ObservableProperty]
        private ObservableCollection<string> timeframes = new();

        [ObservableProperty]
        private ObservableCollection<KLineData> kLineData = new();

        [ObservableProperty]
        private bool hasKLineData = false;

        [ObservableProperty]
        private bool showStatus = true;

        [ObservableProperty]
        private string statusText = "請輸入交易對和選擇時區";

        [RelayCommand]
        private async Task QueryKLine()
        {
            if (string.IsNullOrWhiteSpace(SymbolInput) || string.IsNullOrWhiteSpace(SelectedTimeframe))
            {
                StatusText = "請輸入交易對和選擇時區";
                ShowStatus = true;
                HasKLineData = false;
                return;
            }

            try
            {
                StatusText = "查詢中...";
                ShowStatus = true;
                HasKLineData = false;

                var data = await ExchangeService.GetKLineDataAsync(SymbolInput, SelectedTimeframe, 20, Settings.SelectedExchange);
                
                if (data != null && data.Any())
                {
                    // 重新賦值整個集合以觸發PropertyChanged事件
                    KLineData = new ObservableCollection<KLineData>(data);
                    
                    HasKLineData = true;
                    ShowStatus = false;
                }
                else
                {
                    StatusText = "查詢失敗或無資料";
                    ShowStatus = true;
                    HasKLineData = false;
                }
            }
            catch (Exception ex)
            {
                StatusText = $"查詢錯誤: {ex.Message}";
                ShowStatus = true;
                HasKLineData = false;
            }
        }
    }

}
