using Avalonia.Controls;
using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace CryptoWidget.Services.Converter
{
    public class BoolToGridLengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isVisible)
            {
                if (parameter is string lengthStr && double.TryParse(lengthStr, out double length))
                {
                    return isVisible ? new GridLength(length, GridUnitType.Pixel) : new GridLength(0);
                }
                // Default to star if no parameter
                return isVisible ? new GridLength(1, GridUnitType.Star) : new GridLength(0);
            }
            return new GridLength(0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
