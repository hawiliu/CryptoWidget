using Avalonia;
using Avalonia.Collections;
using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace CryptoWidget.Services.Converter
{
    public class HistoryToPointsConverter : IValueConverter
    {
        // 目標畫布是 50x40（Viewbox 會自動縮放）
        const double W = 50.0;
        const double H = 26.0;
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is IEnumerable<double> seq)
            {
                var vals = seq.ToList();
                if (vals.Count == 0) return new AvaloniaList<Point>();
                // 正常化到 0..1
                var min = vals.Min();
                var max = vals.Max();
                var range = Math.Max(max - min, 1e-9); // 避免除以 0

                // X 等距分佈
                var n = vals.Count;
                var points = new AvaloniaList<Point>(n);
                for (int i = 0; i < n; i++)
                {
                    var x = (n == 1) ? 0 : (i / (double)(n - 1)) * W;
                    var yNorm = (vals[i] - min) / range;    // 0..1（越大越高）
                    var y = (1 - yNorm) * H;                // 0 在下方
                    points.Add(new Point(x, y));
                }
                return points;
            }
            return new AvaloniaList<Point>();
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
