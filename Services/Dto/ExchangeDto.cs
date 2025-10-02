using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace CryptoWidget.Services.Dto
{
    public partial class PriceItem : ObservableObject
    {
        public PriceItem()
        {
            History.CollectionChanged += History_CollectionChanged;
        }

        partial void OnHistoryChanged(ObservableCollection<double> oldValue, ObservableCollection<double> newValue)
        {
            if (oldValue != null) oldValue.CollectionChanged -= History_CollectionChanged;
            if (newValue != null) newValue.CollectionChanged += History_CollectionChanged;
            OnPropertyChanged(nameof(History));
        }

        private void History_CollectionChanged(object? s, NotifyCollectionChangedEventArgs e)
            => OnPropertyChanged(nameof(History));


        [ObservableProperty]
        private string symbol = string.Empty;

        [ObservableProperty]
        private string price = string.Empty;

        [ObservableProperty]
        private string inputValue = string.Empty;

        [ObservableProperty]
        private ObservableCollection<double> history = new ObservableCollection<double>();

        public void Push(double newPrice, int maxPoints = 100)
        {
            History.Add(newPrice);
            while (History.Count > maxPoints)
                History.RemoveAt(0);
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