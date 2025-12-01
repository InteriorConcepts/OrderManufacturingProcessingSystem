using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace OMPS.Converters
{
    public class NullToBooleanConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Return true if value is not null, false if null
            return value != null;
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }

    public class NullToVisibilityConverter : MarkupExtension, IValueConverter
    {
        object? original = null;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            original = value;
            // Return true if value is not null, false if null
            return value is null ? Visibility.Hidden : Visibility.Visible;
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is Visibility.Hidden ? null: original;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
