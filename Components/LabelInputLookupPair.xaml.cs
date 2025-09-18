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
    /// Interaction logic for LabelInputLookupPair.xaml
    /// </summary>
    public partial class LabelInputLookupPair : UserControl
    {
        public LabelInputLookupPair()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty LabelTextProperty =
            DependencyProperty.Register("LabelText", typeof(string), typeof(LabelInputLookupPair),
                new PropertyMetadata("Label:"));

        public static readonly DependencyProperty LabelWidthProperty =
            DependencyProperty.Register("LabelWidth", typeof(int), typeof(LabelInputLookupPair),
                new PropertyMetadata(65));

        public static readonly DependencyProperty InputTextProperty =
            DependencyProperty.Register("InputText", typeof(string), typeof(LabelInputLookupPair),
                new PropertyMetadata("Value"));

        public static readonly DependencyProperty InputFormatProperty =
            DependencyProperty.Register("InputFormat", typeof(string), typeof(LabelInputLookupPair),
                new PropertyMetadata("{}"));

        public static readonly DependencyProperty LookupEnabledProperty =
            DependencyProperty.Register("LookupEnabled", typeof(bool), typeof(LabelInputLookupPair),
                new PropertyMetadata(true));

        public string LabelText
        {
            get { return (string)GetValue(LabelTextProperty); }
            set { SetValue(LabelTextProperty, value); }
        }

        public int LabelFullWidth
        {
            get { return (int)GetValue(LabelWidthProperty); }
            set { SetValue(LabelWidthProperty, value - 16); }
        }

        public int LabelWidth
        {
            get { return (int)GetValue(LabelWidthProperty) - 16; }
            set { SetValue(LabelWidthProperty, value); }
        }

        public string InputText
        {
            get { return (string)GetValue(InputTextProperty); }
            set { SetValue(InputTextProperty, value); }
        }

        public string InputFormat
        {
            get { return (string)GetValue(InputFormatProperty); }
            set { SetValue(InputFormatProperty, value); }
        }

        public bool LookupEnabled
        {
            get { return (bool)GetValue(LookupEnabledProperty); }
            set { SetValue(LookupEnabledProperty, value); }
        }
    }
}
