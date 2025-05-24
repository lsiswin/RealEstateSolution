using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace RealEstateSolution.WpfClient.Converters
{
    public class TrendToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double trend)
            {
                return trend > 0 ? new SolidColorBrush(Color.FromRgb(76, 175, 80)) : // Green
                       trend < 0 ? new SolidColorBrush(Color.FromRgb(244, 67, 54)) : // Red
                       new SolidColorBrush(Color.FromRgb(158, 158, 158)); // Gray
            }
            return new SolidColorBrush(Color.FromRgb(158, 158, 158));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 