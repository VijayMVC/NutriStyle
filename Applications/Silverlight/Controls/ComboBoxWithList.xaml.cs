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
    public partial class ComboBoxWithList : UserControl
    {
        
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
        

        public ComboBoxWithList()
        {
            InitializeComponent();
            
            //IsBusy =true;
           // progressBar.Visibility = System.Windows.Visibility.Visible;
        }

        public PairWithList SelectedPairWithList
        {
            get
            {
                return (PairWithList)GetValue(SelectedPairWithListProperty);
            }
            set
            {
                SetValue(SelectedPairWithListProperty, value);//set the value of the text box
            }
        }

        public static readonly DependencyProperty SelectedPairWithListProperty =
           DependencyProperty.Register("SelectedPairWithList",
           typeof(PairWithList),
           typeof(ComboBoxWithList),
           new PropertyMetadata(null, new PropertyChangedCallback(SelectedPairWithListChanged)));

        private static void SelectedPairWithListChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ComboBoxWithList myControl = sender as ComboBoxWithList;

            if (myControl != null)
            {
                if (e.NewValue != null)
                {
                    String text = ((PairWithList)e.NewValue).Name;
                    myControl.MyAutoCompleteBox.SelectedItem = (PairWithList)e.NewValue;
                    if( ((PairWithList)e.NewValue).List != null && ((PairWithList)e.NewValue).List.Count() > 0 )
                    {
                        //myControl.MyAutoCompleteBox.ItemsSource = ((PairWithList)e.NewValue).List;
                        myControl.MyAutoCompleteBox.DataContext = ((PairWithList)e.NewValue).List;
                    }
                    myControl.IsBusy = false;
                }
            }
            else
            {
                PairWithList pwl = new PairWithList();
                pwl.List = new List<PairWithList>();
                pwl.Name = String.Empty;
                pwl.Id = Guid.Empty.ToString();
                myControl.MyAutoCompleteBox.SelectedItem = pwl;
            }
        }

        private void MyAutoCompleteBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MyAutoCompleteBox.SelectedItem != null)
            {
                if (typeof(PairWithList) == MyAutoCompleteBox.SelectedItem.GetType())
                {
                    
                    SelectedPairWithList = (PairWithList)MyAutoCompleteBox.SelectedItem;//Have to set this value for binding to work correctly
                }
            }
        }

        private void MyAutoCompleteBox_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.MyAutoCompleteBox.Width = this.LayoutRoot.ActualWidth -20d;
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
            this.MyAutoCompleteBox.Text = String.Empty;
            this.MyAutoCompleteBox.IsDropDownOpen = !this.MyAutoCompleteBox.IsDropDownOpen;
        }
        /// <summary>
        /// Open/close Dropdown
        /// </summary>
        /// <param name="open"></param>
        public void  OpenDropdown(bool open) {
            this.MyAutoCompleteBox.IsDropDownOpen = open;
        }
        public Row Row()
        {
            if (this.DataContext.GetType() == typeof(Row))
            {
                return ((Row)this.DataContext);
            }
            return (null);
        }
        /// <summary>
        /// deal with enter key
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyAutoCompleteBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (MyAutoCompleteBox.SelectedItem != null && MyAutoCompleteBox.SelectedItem.GetType() == typeof(PairWithList) && e.Key == Key.Enter)
            {
                SelectedPairWithList = (PairWithList)MyAutoCompleteBox.SelectedItem;
            }
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.MyAutoCompleteBox.Text = String.Empty;
            this.MyAutoCompleteBox.IsDropDownOpen = !this.MyAutoCompleteBox.IsDropDownOpen;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.MyAutoCompleteBox.Text = String.Empty;
            this.MyAutoCompleteBox.IsDropDownOpen = !this.MyAutoCompleteBox.IsDropDownOpen;
        }
    }
}

