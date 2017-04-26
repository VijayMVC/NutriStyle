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
using DynamicConnections.NutriStyle.MenuGenerator.Engine.Helpers;

namespace DynamicConnections.NutriStyle.MenuGenerator.Controls
{
    public partial class ComboBox : UserControl
    {
        private List<Pair> items;

        public List<Pair> Items
        {
            get
            {
                return (items);
            }
            set
            {
                items = value;
                MyAutoCompleteBox.ItemsSource = items;
            }
        }
        public String EntityName { get; set; }
        public String Attribute { get; set; }
        public String TagName { get; set; }
        public String OrgValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public String SearchText { get; set; }

        public bool IsBusy
        {
            set
            {
                if (value)
                {
                    progressBar.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    progressBar.Visibility = System.Windows.Visibility.Collapsed;
                }
            }

        }

        
        public ComboBox()
        {
            InitializeComponent();
            items = new List<Pair>();
            //IsBusy =true;
           // progressBar.Visibility = System.Windows.Visibility.Visible;
        }

  

        public Pair SelectedPair
        {
            get
            {
                return (Pair)GetValue(SelectedPairProperty);
            }
            set
            {
                SetValue(SelectedPairProperty, value);//set the value of the text box
            }
        }

        public static readonly DependencyProperty SelectedPairProperty =
           DependencyProperty.Register("SelectedPair",
           typeof(Pair),
           typeof(ComboBox),
           new PropertyMetadata(null, new PropertyChangedCallback(SelectedPairChanged)));

        private static void SelectedPairChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ComboBox myControl = sender as ComboBox;

            if (myControl != null)
            {
                if (e.NewValue != null)
                {
                    String text = ((Pair)e.NewValue).Name;
                    myControl.MyAutoCompleteBox.SelectedItem = (Pair)e.NewValue;
                }
            }
            else
            {
                myControl.MyAutoCompleteBox.SelectedItem = new Pair() { Name = "", Id = Guid.Empty.ToString() };
            }
        }

        private void MyAutoCompleteBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MyAutoCompleteBox.SelectedItem != null)
            {
                if (typeof(Pair) == MyAutoCompleteBox.SelectedItem.GetType())
                {
                    Pair pair = sender as Pair;
                    SelectedPair = (Pair)MyAutoCompleteBox.SelectedItem;//Have to set this value for binding to work correctly
                }
            }
        }

        private void MyAutoCompleteBox_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.MyAutoCompleteBox.Width = this.LayoutRoot.ActualWidth;
        }

        void ToggleButton_MouseEnter(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "ToggleButtonOver", true);
        }

        void ToggleButton_MouseLeave(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "ToggleButtonOut", true);
        }

        void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show(this.MyAutoCompleteBox.SearchText + ":" + this.MyAutoCompleteBox.Text);
            /*
            if (String.IsNullOrEmpty(this.MyAutoCompleteBox.SearchText))
            {
                this.MyAutoCompleteBox.Text = String.Empty;
            }*/
            this.MyAutoCompleteBox.Text = String.Empty;
            this.MyAutoCompleteBox.SelectedItem = new Pair() { Name = "", Id = Guid.Empty.ToString() };
            this.MyAutoCompleteBox.IsDropDownOpen = !this.MyAutoCompleteBox.IsDropDownOpen;
        }
        /// <summary>
        /// Open/close Dropdown
        /// </summary>
        /// <param name="open"></param>
        public void  OpenDropdown(bool open) {
            this.MyAutoCompleteBox.IsDropDownOpen = open;
        }
        /// <summary>
        /// deal with enter key
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyAutoCompleteBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (MyAutoCompleteBox.SelectedItem != null && MyAutoCompleteBox.SelectedItem.GetType() == typeof(Pair) && e.Key == Key.Enter)
            {
                SelectedPair = (Pair)MyAutoCompleteBox.SelectedItem;
            }
        }

        private void MyAutoCompleteBox_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!MyAutoCompleteBox.IsEnabled)
            {
                MyAutoCompleteBox.Style = Resources["EditableComboStyleDisabled"] as Style;
            }
            else
            {
                MyAutoCompleteBox.Style = Resources["EditableComboStyle"] as Style;
               
            }
            if (MyAutoCompleteBox.DataContext != null)
            {
                if (MyAutoCompleteBox.DataContext.GetType() == typeof(Row))
                {
                    ((Row)MyAutoCompleteBox.DataContext).RowChanged = false;//don't want this causing an update
                }
            }
        }
    }
}

