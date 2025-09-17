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
    /// Interaction logic for LabelComboPair.xaml
    /// </summary>
    public partial class LabelComboPair : UserControl
    {
        public LabelComboPair()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty LabelTextProperty =
            DependencyProperty.Register("LabelText", typeof(string), typeof(LabelComboPair),
                new PropertyMetadata("Label:"));

        public static readonly DependencyProperty LabelWidthProperty =
            DependencyProperty.Register("LabelWidth", typeof(int), typeof(LabelComboPair),
                new PropertyMetadata(65));

        public static readonly DependencyProperty InputTextProperty =
            DependencyProperty.Register("InputText", typeof(string), typeof(LabelComboPair),
                new PropertyMetadata("Value"));

        public static readonly DependencyProperty InputFormatProperty =
            DependencyProperty.Register("InputFormat", typeof(string), typeof(LabelComboPair),
                new PropertyMetadata("{}"));

        public static readonly DependencyProperty InputReadOnlyProperty =
            DependencyProperty.Register("InputReadOnly", typeof(bool), typeof(LabelComboPair),
                new PropertyMetadata(false));

        public static readonly DependencyProperty ItemSourceProperty =
            DependencyProperty.Register("ItemSource", typeof(object[]), typeof(LabelComboPair),
                new PropertyMetadata(Array.Empty<object>()));

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

        public string InputText
        {
            get { return (string)GetValue(InputTextProperty); }
            set { SetValue(InputTextProperty, value); }
        }

        public object[] ItemSource
        {
            get { return (object[])GetValue(ItemSourceProperty); }
            set { SetValue(ItemSourceProperty, value); }
        }

        public bool InputReadOnly
        {
            get { return (bool)GetValue(InputReadOnlyProperty); }
            set { SetValue(InputReadOnlyProperty, value); }
        }
    }
}
