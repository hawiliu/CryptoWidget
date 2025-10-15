using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace CryptoWidget.Services.Dto
{
    public partial class PriceItem : ObservableObject
    {
        public PriceItem()
        {
            KLineData.CollectionChanged += KLineData_CollectionChanged;
        }

        partial void OnKLineDataChanged(ObservableCollection<KLineData> oldValue, ObservableCollection<KLineData> newValue)
        {
            if (oldValue != null) oldValue.CollectionChanged -= KLineData_CollectionChanged;
            if (newValue != null) newValue.CollectionChanged += KLineData_CollectionChanged;
            OnPropertyChanged(nameof(KLineData));
        }

        private void KLineData_CollectionChanged(object? s, NotifyCollectionChangedEventArgs e)
            => OnPropertyChanged(nameof(KLineData));

        [ObservableProperty]
        private string symbol = string.Empty;

        [ObservableProperty]
        private string price = string.Empty;

        [ObservableProperty]
        private string inputValue = string.Empty;

        [ObservableProperty]
        private ObservableCollection<KLineData> kLineData = new ObservableCollection<KLineData>();

        public void UpdateKLineData(ObservableCollection<KLineData> newKLineData)
        {
            // 先清除舊的事件監聽器
            if (KLineData != null)
            {
                KLineData.CollectionChanged -= KLineData_CollectionChanged;
            }
            
            // 設置新的KLineData
            KLineData = newKLineData;
            
            // 添加新的事件監聽器
            if (KLineData != null)
            {
                KLineData.CollectionChanged += KLineData_CollectionChanged;
            }
            
            // 觸發屬性變更通知
            OnPropertyChanged(nameof(KLineData));
        }
    }

    public sealed partial class PositionItem : ObservableObject
    {
        [ObservableProperty]
        private string symbol = string.Empty;

        [ObservableProperty]
        private string? side;

        [ObservableProperty]
        private double? contracts;

        [ObservableProperty]
        private double? entryPrice;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsInProfit))]
        [NotifyPropertyChangedFor(nameof(IsInLoss))]
        private double? unrealizedPnl;

        public bool IsInProfit => UnrealizedPnl.HasValue && UnrealizedPnl.Value > 0;
        public bool IsInLoss => UnrealizedPnl.HasValue && UnrealizedPnl.Value < 0;
    }
}