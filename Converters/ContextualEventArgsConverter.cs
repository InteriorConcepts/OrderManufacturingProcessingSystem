using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Markup;

namespace OMPS.Converters
{
    public class ContextualEventArgs : EventArgs
    {
        public object? Context { get; }

        public EventArgs OriginalEventArgs { get; }

        public ContextualEventArgs(EventArgs original, object? context) : base()
        {
            Context = context;
            OriginalEventArgs = original;
        }
    }

    public class ContextualEventArgsConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is EventArgs args)
            {
                return new ContextualEventArgs(args, parameter);
            }

            //return Binding.DoNothing;
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
