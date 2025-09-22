using System;
using System.Collections.Generic;
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
    /// Interaction logic for LabelCheckPair.xaml
    /// </summary>
    public partial class LabelCheckPair : UserControl
    {
        public LabelCheckPair()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty LabelTextProperty =
            DependencyProperty.Register("LabelText", typeof(string), typeof(LabelCheckPair),
                new PropertyMetadata("Label"));

        public static readonly DependencyProperty InputTextProperty =
            DependencyProperty.Register("InputText", typeof(string), typeof(LabelCheckPair),
                new PropertyMetadata("False"));

        public static readonly DependencyProperty InputReadOnlyProperty =
            DependencyProperty.Register("InputReadOnly", typeof(bool), typeof(LabelCheckPair),
                new PropertyMetadata(false));

        public string LabelText
        {
            get { return (string)GetValue(LabelTextProperty); }
            set { SetValue(LabelTextProperty, value); }
        }

        public string InputText
        {
            get { return (string)GetValue(InputTextProperty); }
            set { SetValue(InputTextProperty, value); }
        }

        public bool CheckValue
        {
            get { return bool.TryParse((string)GetValue(InputTextProperty), out bool val) && val; }
            set { SetValue(InputTextProperty, value.ToString()); }
        }

        public bool InputReadOnly
        {
            get { return (bool)GetValue(InputReadOnlyProperty); }
            set { SetValue(InputReadOnlyProperty, value); }
        }
    }
}
