using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CryptoWidget.ViewModels;
using CryptoWidget.Services.Dto;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace CryptoWidget
{
    public partial class MainWindow : Window
    {
        private MainViewModel? _viewModel;
        
        // 迷你K線圖的常數
        private const double MINI_CANVAS_WIDTH = 100.0;
        private const double MINI_CANVAS_HEIGHT = 26.0;
        private const double MINI_MARGIN = 2.0;
        private const double MINI_CANDLE_SPACING_RATIO = 0.1; // 10%間距

        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(MainViewModel mainViewModel)
        {
            InitializeComponent();
            DataContext = mainViewModel;
            _viewModel = mainViewModel;

            this.PointerPressed += (_, e) =>
            {
                if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
                    BeginMoveDrag(e);
            };

            // 監聽價格項目變化
            if (_viewModel != null)
            {
                _viewModel.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(MainViewModel.PriceItems))
                    {
                        Dispatcher.UIThread.Post(() => UpdateMiniKLineCharts());
                    }
                };

                // 監聽PriceItems集合變化
                _viewModel.PriceItems.CollectionChanged += (s, e) =>
                {
                    Dispatcher.UIThread.Post(() => UpdateMiniKLineCharts());
                    
                    // 監聽新添加的PriceItem的KLineData變化
                    if (e.NewItems != null)
                    {
                        foreach (PriceItem item in e.NewItems)
                        {
                            item.PropertyChanged += (sender, args) =>
                            {
                                if (args.PropertyName == nameof(PriceItem.KLineData))
                                {
                                    Dispatcher.UIThread.Post(() => UpdateMiniKLineCharts());
                                }
                            };
                        }
                    }
                };

                // 監聽現有PriceItem的KLineData變化
                foreach (var item in _viewModel.PriceItems)
                {
                    item.PropertyChanged += (sender, e) =>
                    {
                        if (e.PropertyName == nameof(PriceItem.KLineData))
                        {
                            Dispatcher.UIThread.Post(() => UpdateMiniKLineCharts());
                        }
                    };
                }
            }
        }

        private void CloseButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                if (vm._settingWindow != null)
                    vm._settingWindow.Close();

                if (vm._aboutWindow != null)
                    vm._aboutWindow.Close();

                if (vm.Settings.CloseOnExit)
                {
                    Close(); // 完全關閉應用程式
                }
                else
                {
                    Hide(); // 隱藏到系統匣
                }
            }
        }

        private void UpdateMiniKLineCharts()
        {
            if (_viewModel?.PriceItems == null) return;

            // 找到所有的迷你K線Canvas並更新
            var itemsControl = this.FindControl<ItemsControl>("PriceItemsControl");
            if (itemsControl == null) return;

            // 遍歷每個價格項目並更新其K線圖
            for (int i = 0; i < _viewModel.PriceItems.Count; i++)
            {
                var priceItem = _viewModel.PriceItems[i];
                if (priceItem.KLineData != null && priceItem.KLineData.Any())
                {
                    // 這裡我們需要找到對應的Canvas並更新
                    // 由於ItemsControl的結構，我們需要通過視覺樹來找到對應的Canvas
                    UpdateSingleMiniKLineChart(priceItem, i);
                }
            }
        }

        private void UpdateSingleMiniKLineChart(PriceItem priceItem, int index)
        {
            // 找到對應的Canvas並繪製K線
            var canvas = FindMiniKLineCanvas(index);
            if (canvas != null)
            {
                DrawMiniKLineChart(canvas, priceItem.KLineData);
            }
        }

        private Canvas? FindMiniKLineCanvas(int index)
        {
            // 通過視覺樹找到對應的Canvas
            // 這是一個簡化的實現，實際可能需要更複雜的邏輯
            var itemsControl = this.FindControl<ItemsControl>("PriceItemsControl");
            if (itemsControl == null) return null;

            // 獲取ItemsControl的容器
            var container = itemsControl.ContainerFromIndex(index);
            if (container is ContentPresenter presenter)
            {
                return FindCanvasInVisualTree(presenter);
            }

            return null;
        }

        private Canvas? FindCanvasInVisualTree(Control parent)
        {
            if (parent is Canvas canvas && parent.Name == "MiniKLineCanvas")
                return canvas;

            foreach (var child in parent.GetVisualChildren().OfType<Control>())
            {
                var result = FindCanvasInVisualTree(child);
                if (result != null) return result;
            }

            return null;
        }

        private void DrawMiniKLineChart(Canvas canvas, ObservableCollection<KLineData> kLineData)
        {
            // 清除現有的K線圖形（保留X軸線）
            var childrenToRemove = canvas.Children.OfType<Control>()
                .Where(c => !(c is Rectangle && c.Name == "XAxisLine"))
                .ToList();
            
            foreach (var child in childrenToRemove)
            {
                canvas.Children.Remove(child);
            }

            if (kLineData == null || !kLineData.Any()) return;

            var data = kLineData.ToList();
            if (data.Count == 0) return;

            // 計算圖表尺寸
            double canvasWidth = MINI_CANVAS_WIDTH;
            double canvasHeight = MINI_CANVAS_HEIGHT;
            double margin = MINI_MARGIN;
            double chartWidth = canvasWidth - 2 * margin;
            double chartHeight = canvasHeight - 2 * margin;

            // 計算價格範圍
            decimal minPrice = data.Min(k => k.Low);
            decimal maxPrice = data.Max(k => k.High);
            decimal priceRange = maxPrice - minPrice;
            
            if (priceRange == 0) priceRange = 1; // 避免除零

            // 計算每根K棒的寬度
            double candleWidth = chartWidth / data.Count;
            double candleSpacing = candleWidth * MINI_CANDLE_SPACING_RATIO;
            double actualCandleWidth = Math.Max(candleWidth - candleSpacing, 1);

            // 繪製K棒
            for (int i = 0; i < data.Count; i++)
            {
                var candle = data[i];
                double x = margin + i * candleWidth + candleSpacing / 2;
                
                // 計算Y座標（價格轉換為畫布座標）
                double openY = margin + (double)((maxPrice - candle.Open) / priceRange) * chartHeight;
                double closeY = margin + (double)((maxPrice - candle.Close) / priceRange) * chartHeight;
                double highY = margin + (double)((maxPrice - candle.High) / priceRange) * chartHeight;
                double lowY = margin + (double)((maxPrice - candle.Low) / priceRange) * chartHeight;

                // 選擇顏色
                var color = candle.IsGreen ? Brushes.LimeGreen : Brushes.Red;

                // 繪製影線（高低價線）
                var shadowLine = new Line
                {
                    StartPoint = new Point(x + actualCandleWidth / 2, highY),
                    EndPoint = new Point(x + actualCandleWidth / 2, lowY),
                    Stroke = color,
                    StrokeThickness = 1
                };
                canvas.Children.Add(shadowLine);

                // 繪製實體（開收價）
                double bodyTop = Math.Min(openY, closeY);
                double bodyHeight = Math.Abs(closeY - openY);
                if (bodyHeight < 1) bodyHeight = 1; // 最小高度

                var body = new Rectangle
                {
                    Width = actualCandleWidth,
                    Height = bodyHeight,
                    Fill = color,
                    Stroke = color,
                    StrokeThickness = 0.5
                };
                
                Canvas.SetLeft(body, x);
                Canvas.SetTop(body, bodyTop);
                canvas.Children.Add(body);
            }
        }
    }
}