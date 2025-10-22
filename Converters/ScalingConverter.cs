using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace OMPS.Converters
{
    public class ScalingConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double originalValue && parameter is string scaleFactorString)
            {
                if (double.TryParse(scaleFactorString, out double scaleFactor))
                {
                    return originalValue * scaleFactor;
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double originalValue && parameter is string scaleFactorString)
            {
                if (double.TryParse(scaleFactorString, out double scaleFactor))
                {
                    return originalValue / scaleFactor;
                }
            }
            return value;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
