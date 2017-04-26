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
using System.ComponentModel;
using System.Windows.Data;
using System.Text.RegularExpressions;

namespace DynamicConnections.NutriStyle.MenuGenerator.Controls
{
    public partial class CustomTextBox : UserControl
    {
        private bool _displayWatermark = true;
        private bool _hasFocus;

        public CustomTextBox()
        {
            InitializeComponent();
            IsTabStop = false;
            GotFocus += WatermarkTextBoxGotFocus;
            LostFocus += WatermarkTextBoxLostFocus;
            //TextChanged += WatermarkTextBoxTextChanged;
            Unloaded += WatermarkTextBoxUnloaded;
        }

        public String EntityName { get; set; }
        public String Attribute { get; set; }
        public String TagName { get; set; }
        public String OrgValue { get; set; }
        public static String Format { get; set; }
        public bool IsNumeric { get; set; }

        public String TextAlign
        {
            get
            {
                return (this.TextBox.TextAlignment.ToString());
            }
            set
            {
                if (value.Equals("Right", StringComparison.OrdinalIgnoreCase))
                {
                    this.TextBox.TextAlignment = TextAlignment.Right;
                }
                else if (value.Equals("Left", StringComparison.OrdinalIgnoreCase))
                {
                    this.TextBox.TextAlignment = TextAlignment.Left;
                }
            }
        }
        public bool IsVisable
        {
            set
            {
                if (value)
                {
                    LayoutRoot.Visibility = Visibility.Visible;
                }
                else
                {
                    LayoutRoot.Visibility = Visibility.Collapsed;
                }

            }
        }

        public String SelectedText
        {
            get
            {
                return (String)GetValue(SelectedTextProperty);
            }
            set
            {
                SetValue(SelectedTextProperty, value);//set the value of the text box
            }
        }

        public static readonly DependencyProperty SelectedTextProperty =
           DependencyProperty.Register("SelectedText",
           typeof(String),
           typeof(CustomTextBox),
           new PropertyMetadata(null, new PropertyChangedCallback(SelectedTextChanged)));

        private static void SelectedTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            //MessageBox.Show("" + sender.GetType());
            CustomTextBox myControl = sender as CustomTextBox;
            
            if (myControl != null)
            {
                if (e.NewValue != null)
                {
                    String text = (String)e.NewValue;
                    /*
                    double d = 0d;
                    bool isNumber = Double.TryParse(text, out d);
                    */


                    //myControl.TextBox.Text = text;
                    if (myControl != null && myControl._displayWatermark)
                    {
                        myControl.TextBox.Text = text;
                        myControl.SetMode(false);
                        if (myControl.IsNumeric)
                        {
                            //parse out non numeric
                            Regex regex = new Regex(@"^\d\.,");
                            text = regex.Replace(text, "");
                            myControl.TextBox.Text = text;
                        }

                    }
                    //myControl.SetMode(false);
                }
            }
            else
            {
                myControl.TextBox.Text = String.Empty;
            }
        }


        private void MyAutoCompleteBox_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.TextBox.Width = this.LayoutRoot.ActualWidth;
        }



        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            double d = 0d;
            String text = this.TextBox.Text;
            bool isNumber = Double.TryParse(text, out d);
            if (!String.IsNullOrEmpty(Format) && isNumber)
            {
                //this.TextBox.Text = String.Format("{" + Format + "}", Convert.ToDouble(text));//Convert.ToDecimal(text).ToString("{" + Format + "}");
            }

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb != null)
            {
                //SelectedText = tb.Text;
                if (this.IsNumeric)
                {
                    //parse out non numeric
                    Regex regex = new Regex(@"[^0-9.,]");
                    tb.Text = regex.Replace(tb.Text, "");
                    SelectedText = tb.Text;
                }
                else
                {
                    SelectedText = tb.Text;
                }

            }
            
        }



        private void WatermarkTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            OnTextChanged();
        }

        private void WatermarkTextBoxUnloaded(object sender, RoutedEventArgs e)
        {
            GotFocus -= WatermarkTextBoxGotFocus;
            LostFocus -= WatermarkTextBoxLostFocus;
            Unloaded -= WatermarkTextBoxUnloaded;
            //TextChanged -= WatermarkTextBoxTextChanged;
        }

        private void WatermarkTextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            _hasFocus = true;
            if (_displayWatermark)
            {
                _displayWatermark = false;
                SetMode(false);
                Text = string.Empty;
            }
        }

        private void WatermarkTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            _hasFocus = false;
            OnTextChanged();
        }

        private String Text
        {
            get { return (TextBox.Text); }
            set
            {
                if (value != null)
                {
                    TextBox.Text = value;
                    
                }
            }
        }


        private void OnTextChanged()
        {
            if (!_displayWatermark && !_hasFocus)
            {
                bool isEmpty = string.IsNullOrEmpty(Text);
                SetMode(isEmpty);
                _displayWatermark = isEmpty;

                if (isEmpty) Text = Watermark;
            }
        }

        private void SetMode(bool showWatermark)
        {
            TextBox.FontStyle = showWatermark ? FontStyles.Italic : FontStyles.Normal;
            TextBox.Foreground = showWatermark ? new SolidColorBrush(Colors.Gray) : (SolidColorBrush)App.Current.Resources["defaultFontColor"];
        }

        public string Watermark
        {
            get { return GetValue(WatermarkProperty) as string; }
            set { SetValue(WatermarkProperty, value); }
        }

        public static readonly DependencyProperty WatermarkProperty =
            DependencyProperty.Register("Watermark", typeof(string), typeof(CustomTextBox), new PropertyMetadata(OnWatermarkPropertyChanged));

        private static void OnWatermarkPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            CustomTextBox watermarkTextBox = sender as CustomTextBox;
            if (watermarkTextBox != null && watermarkTextBox._displayWatermark)
            {
                watermarkTextBox.Text = e.NewValue.ToString();
                watermarkTextBox.SetMode(true);
            }
        }
    }
}

