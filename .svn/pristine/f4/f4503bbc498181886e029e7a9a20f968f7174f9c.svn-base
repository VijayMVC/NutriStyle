using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace DynamicConnections.NutriStyle.MenuGenerator.Controls
{
    public partial class MealHeader : UserControl
    {
        public delegate void ClickHandler(object sender, EventArgs e);
        public event ClickHandler LeftClick;

       

        public MealHeader()
        {
            InitializeComponent();
            
        }
        void RaiseLeftClick(RoutedEventArgs e)
        {
            if (LeftClick != null)
            {
                LeftClick(this, e);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            RaiseLeftClick(e);
        }

        public String SetText
        {
            get { return (string)GetValue(SetTextProperty); }
            set { SetValue(SetTextProperty, value); }
        }
        public static readonly DependencyProperty SetTextProperty =
        DependencyProperty.Register("SetText", typeof(string), typeof(MealHeader), new PropertyMetadata("", SetText_PropertyChangedCallback));

        private static void SetText_PropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            MealHeader myControl = sender as MealHeader;

            if (e.NewValue != null)
            {
                //Label.text = (String)e.NewValue;
                myControl.Label.Text = (String)e.NewValue;
            }
        }


    }
}
