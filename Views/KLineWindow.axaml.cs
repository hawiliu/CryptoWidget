using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia;
using Avalonia.Threading;
using Avalonia.Input;
using CryptoWidget.ViewModels;
using System;
using System.Linq;

namespace CryptoWidget
{
    public partial class KLineWindow : Window
    {
        private KLineViewModel? _viewModel;

        public KLineWindow(KLineViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            _viewModel = viewModel;

            this.PointerPressed += (_, e) =>
            {
                if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
                {
                    // 檢查點擊位置是否在ComboBox區域
                    var point = e.GetCurrentPoint(this);
                    
                    // 如果點擊的不是ComboBox區域，則允許拖拽
                    if (!IsPointInComboBox(point.Position))
                    {
                        BeginMoveDrag(e);
                    }
                }
            };
            
            // 監聽K線資料變化
            _viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(KLineViewModel.KLineData))
                {
                    System.Diagnostics.Debug.WriteLine($"KLineData changed, count: {_viewModel.KLineData?.Count ?? 0}");
                    // 確保在UI線程上執行圖表繪製
                    Dispatcher.UIThread.Post(() => DrawKLineChart());
                }
            };
        }

        private void CloseButton_Click(object? sender, RoutedEventArgs e)
        {
            Close();
        }

        private bool IsPointInComboBox(Point point)
        {
            // 檢查點擊位置是否在ComboBox的邊界範圍內
            if (TimeframeComboBox != null)
            {
                // 獲取ComboBox相對於視窗的實際位置
                var bounds = TimeframeComboBox.Bounds;
                
                // 將ComboBox的邊界轉換為相對於視窗的絕對位置
                var absoluteBounds = new Rect(
                    bounds.X + 10,
                    bounds.Y + 44,
                    bounds.Width,
                    bounds.Height
                );
                
                return absoluteBounds.Contains(point);
            }
            
            return false;
        }


        private void DrawKLineChart()
        {
            System.Diagnostics.Debug.WriteLine($"DrawKLineChart called, data count: {_viewModel?.KLineData?.Count ?? 0}");
            
            if (_viewModel?.KLineData == null || !_viewModel.KLineData.Any())
            {
                System.Diagnostics.Debug.WriteLine("No KLine data available");
                return;
            }

            KLineCanvas.Children.Clear();

            var data = _viewModel.KLineData.ToList();
            if (data.Count == 0) return;

            // 計算圖表尺寸
            double canvasWidth = 300;
            double canvasHeight = 80;
            double margin = 5;
            double chartWidth = canvasWidth - 2 * margin;
            double chartHeight = canvasHeight - 2 * margin;

            // 計算價格範圍
            decimal minPrice = data.Min(k => k.Low);
            decimal maxPrice = data.Max(k => k.High);
            decimal priceRange = maxPrice - minPrice;
            
            if (priceRange == 0) priceRange = 1; // 避免除零

            // 計算每根K棒的寬度
            double candleWidth = chartWidth / data.Count;
            double candleSpacing = candleWidth * 0.1; // 10%間距
            double actualCandleWidth = candleWidth - candleSpacing;

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
                var strokeColor = candle.IsGreen ? Brushes.LimeGreen : Brushes.Red;

                // 繪製影線（高低價線）
                var shadowLine = new Avalonia.Controls.Shapes.Line
                {
                    StartPoint = new Point(x + actualCandleWidth / 2, highY),
                    EndPoint = new Point(x + actualCandleWidth / 2, lowY),
                    Stroke = strokeColor,
                    StrokeThickness = 1
                };
                KLineCanvas.Children.Add(shadowLine);

                // 繪製實體（開收價）
                double bodyTop = Math.Min(openY, closeY);
                double bodyHeight = Math.Abs(closeY - openY);
                if (bodyHeight < 1) bodyHeight = 1; // 最小高度

                var body = new Avalonia.Controls.Shapes.Rectangle
                {
                    Width = actualCandleWidth,
                    Height = bodyHeight,
                    Fill = candle.IsGreen ? Brushes.LimeGreen : Brushes.Red,
                    Stroke = strokeColor,
                    StrokeThickness = 1
                };
                
                Canvas.SetLeft(body, x);
                Canvas.SetTop(body, bodyTop);
                KLineCanvas.Children.Add(body);
            }
        }
    }
}
