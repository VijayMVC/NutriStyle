using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using DynamicConnections.NutriStyle.MenuGenerator.Engine.Helpers;

namespace DynamicConnections.NutriStyle.MenuGenerator.Controls
{

    public class WatermarkTextBox : TextBox
    {
        private bool _displayWatermark = true;
        private bool _hasFocus;
        

        public WatermarkTextBox()
        {
            
            GotFocus += WatermarkTextBoxGotFocus;
            LostFocus += WatermarkTextBoxLostFocus;
            TextChanged += WatermarkTextBoxTextChanged;
            Unloaded += WatermarkTextBoxUnloaded;
            
        }
        
        

        private void WatermarkTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (String.IsNullOrEmpty(Text) && !String.IsNullOrEmpty(base.Text) &&
                !base.Text.Equals(Watermark))
            {
                Text = base.Text;
            }
            OnTextChanged();
        }

        private void WatermarkTextBoxUnloaded(object sender, RoutedEventArgs e)
        {
            GotFocus -= WatermarkTextBoxGotFocus;
            LostFocus -= WatermarkTextBoxLostFocus;
            Unloaded -= WatermarkTextBoxUnloaded;
            TextChanged -= WatermarkTextBoxTextChanged;
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

        private void OnTextChanged()
        {
            if (!_displayWatermark && !_hasFocus)
            {
                bool isEmpty = string.IsNullOrEmpty(Text);
                SetMode(isEmpty);
                _displayWatermark = isEmpty;

                if (isEmpty)
                {
                    Text = Watermark;
                }
                
                if (base.Text.Equals(Watermark) && !String.IsNullOrEmpty(Text))
                {
                    SetMode(true);
                }
            }
        }
        /// <summary>
        /// Show watermark?
        /// </summary>
        /// <param name="showWatermark"></param>
        private void SetMode(bool showWatermark)
        {
            FontStyle   = showWatermark ? FontStyles.Italic : FontStyles.Normal;
            Foreground  = showWatermark ? new SolidColorBrush(Colors.Gray) : (SolidColorBrush)App.Current.Resources["defaultFontColor"];
        }

        public new string Watermark
        {
            get { return GetValue(WatermarkProperty) as string; }
            set { SetValue(WatermarkProperty, value); }
        }

        public static new readonly DependencyProperty WatermarkProperty =
            DependencyProperty.Register("Watermark", typeof(string), typeof(WatermarkTextBox), new PropertyMetadata(OnWatermarkPropertyChanged));

        private static void OnWatermarkPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            WatermarkTextBox watermarkTextBox = sender as WatermarkTextBox;
            if (watermarkTextBox != null && watermarkTextBox._displayWatermark)
            {
                watermarkTextBox.Text = e.NewValue.ToString();
                watermarkTextBox.SetMode(true);
            }
        }
        public string TrueText
        {
            get { return GetValue(TrueTextProperty) as string; }
            set { SetValue(TrueTextProperty, value); }
        }

        public static readonly DependencyProperty TrueTextProperty =
            DependencyProperty.Register("TrueText", typeof(string), typeof(WatermarkTextBox), new PropertyMetadata(OnTrueTextPropertyChanged));

        private static void OnTrueTextPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            WatermarkTextBox watermarkTextBox = sender as WatermarkTextBox;
            if (watermarkTextBox != null && watermarkTextBox._displayWatermark)
            {
                watermarkTextBox.SetMode(true);
            }
        }
        public new string Text
        {
            get { 
                return _displayWatermark ? string.Empty : base.Text;
            }
            set {
                //SetMode(false);
                base.Text = value ?? string.Empty;
                if (base.Text.Equals(Watermark))
                {
                    _displayWatermark = true;
                    SetMode(true);
                }
                else if (String.IsNullOrEmpty(base.Text))
                {
                    _displayWatermark = true;
                    SetMode(true);
                }
                else
                {
                    _displayWatermark = false;
                    SetMode(false);
                }
            }

        }
    }
}
