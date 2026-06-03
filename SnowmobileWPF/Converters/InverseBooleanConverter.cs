using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SnowmobileWPF.Converters
{
    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
            {
                if (targetType == typeof(Visibility))
                    return b ? Visibility.Collapsed : Visibility.Visible;
                return !b;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => value is bool b ? !b : value;
    }
}