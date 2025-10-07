using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OMPS.Components
{

    /// <summary>
    /// Interaction logic for LabelInputPair.xaml
    /// </summary>
    public partial class LabelInputPair : UserControl
    {
        public const string StringFormat_Currency = "C2";
        public const string StringFormat_Text = "{}";
        public static string StringFormat_Current = StringFormat_Text;
        public LabelInputPair()
        {
            InitializeComponent();
        }
        /*
        public static readonly DependencyProperty RelSourceProperty =
            DependencyProperty.Register("RelSource", typeof(Type), typeof(LabelInputPair),
                new PropertyMetadata(null));
        */

        public enum InputType
        {
            Text = 0,
            Currency = 1
        }

        public static readonly DependencyProperty LabelTextProperty =
            DependencyProperty.Register("LabelText", typeof(string), typeof(LabelInputPair),
                new PropertyMetadata("Label:"));

        public static readonly DependencyProperty LabelWidthProperty =
            DependencyProperty.Register("LabelWidth", typeof(int), typeof(LabelInputPair),
                new PropertyMetadata(65));

        public static readonly DependencyProperty InputTypeProperty =
            DependencyProperty.Register("InputType", typeof(InputType), typeof(LabelInputPair),
                new PropertyMetadata(InputType.Text));

        public static readonly DependencyProperty InputTextProperty =
            DependencyProperty.Register("InputText", typeof(object), typeof(LabelInputPair),
                new PropertyMetadata("DefaultValue"));

        public static readonly DependencyProperty InputFormatProperty =
            DependencyProperty.Register("InputFormat", typeof(string), typeof(LabelInputPair),
                new PropertyMetadata("{}"));

        public static readonly DependencyProperty InputReadOnlyProperty =
            DependencyProperty.Register("InputReadOnly", typeof(bool), typeof(LabelInputPair),
                new PropertyMetadata(false));

        
        /*
        public Type RelSource
        {
            get { return (Type)GetValue(RelSourceProperty); }
            set { SetValue(RelSourceProperty, value); }
        }

        private static void OnAncestorTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Debug.WriteLine("1");
            if (d is FrameworkElement element && e.NewValue is Type ancestorType)
            {
                Debug.WriteLine("2");
                var ancestor = FindVisualAncestor(element, ancestorType);
                Debug.WriteLine(ancestorType.FullName);
                if (ancestor != null)
                {
                    Debug.WriteLine("3");
                    var binding = new Binding("Foreground") { Source = ancestor };
                    element.SetBinding(Control.ForegroundProperty, binding);
                }
            }
        }

        private static DependencyObject FindVisualAncestor(DependencyObject current, Type ancestorType)
        {
            while (current != null && current.GetType() != ancestorType)
            {
                Debug.WriteLine("-");
                current = VisualTreeHelper.GetParent(current);
                Debug.WriteLine(current?.DependencyObjectType.Name);
            }
            return current;
        }
        */


        public string LabelText
        {
            get { return (string)GetValue(LabelTextProperty); }
            set { SetValue(LabelTextProperty, value); }
        }

        public int LabelWidth
        {
            get { return (int)GetValue(LabelWidthProperty); }
            set { SetValue(LabelWidthProperty, value); }
        }

        public object InputText
        {
            get { return (object)GetValue(InputTextProperty); }
            set { SetValue(InputTextProperty, value); }
        }

        public IValueConverter? InputConverter
        {
            get {
                return (InputType)GetValue(InputTypeProperty) switch
                {
                    InputType.Text => null,
                    InputType.Currency => null,
                    _ => null,
                };
            }
        }

        public string InputFormat
        {
            get { return (string)GetValue(InputFormatProperty); }
            set { SetValue(InputFormatProperty, value); }
        }

        public bool InputReadOnly
        {
            get { return (bool)GetValue(InputReadOnlyProperty); }
            set { SetValue(InputReadOnlyProperty, value); }
        }
    }
}
