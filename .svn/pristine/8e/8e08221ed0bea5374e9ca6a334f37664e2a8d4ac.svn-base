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

    public class WatermarkRichTextBox : RichTextBox
    {
        private bool _displayWatermark = true;
        private bool _hasFocus;
        private String Empty;
        public String watermarkString;

        public WatermarkRichTextBox()
        {
            watermarkString = String.Empty;
            Empty = "<Section xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'/>";
            GotFocus += WatermarkRichTextBoxGotFocus;
            LostFocus += WatermarkRichTextBoxLostFocus;
            //TextChanged += WatermarkRichTextBoxTextChanged;
            Unloaded += WatermarkRichTextBoxUnloaded;
            ContentChanged += ContentChangedXaml;
        }
  
        private void WatermarkRichTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
           
            if (String.IsNullOrEmpty(Xaml) && base.Blocks != null &&
                !base.Blocks.Equals(Watermark))
            {
                Xaml = base.Xaml;
            }
            OnTextChanged();
        }

        private void WatermarkRichTextBoxUnloaded(object sender, RoutedEventArgs e)
        {
            GotFocus        -= WatermarkRichTextBoxGotFocus;
            LostFocus       -= WatermarkRichTextBoxLostFocus;
            Unloaded        -= WatermarkRichTextBoxUnloaded;
            ContentChanged  -= ContentChangedXaml;
            //XamlTextChanged -= WatermarkRichTextBoxTextChanged;
            
        }

        private void WatermarkRichTextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            _hasFocus = true;
            
            if (_displayWatermark)
            {
                _displayWatermark = false;
                SetMode(false);
                Xaml = this.Empty;
            }
        }

        private void WatermarkRichTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            _hasFocus = false;
            OnTextChanged();
        }

        private void OnTextChanged()
        {
            if (!_displayWatermark && !_hasFocus)
            {
                
                bool isEmpty = string.IsNullOrEmpty(Xaml);
                
                SetMode(isEmpty);
                _displayWatermark = isEmpty;

                if (isEmpty)
                {
                    Xaml = watermarkString;
                }

                if (base.Xaml.Equals(watermarkString) && !String.IsNullOrEmpty(Xaml))
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

        public string Watermark
        {
            get { return GetValue(WatermarkProperty) as string; }
            set { SetValue(WatermarkProperty, value); }
        }

        public static readonly DependencyProperty WatermarkProperty =
            DependencyProperty.Register("Watermark", typeof(string), typeof(WatermarkRichTextBox), new PropertyMetadata(OnWatermarkPropertyChanged));

        private static void OnWatermarkPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            WatermarkRichTextBox WatermarkRichTextBox = sender as WatermarkRichTextBox;
            if (WatermarkRichTextBox != null && WatermarkRichTextBox._displayWatermark)
            {
                // create a paragraph
                Paragraph prgParagraph = new Paragraph();

                // create some text, and add it to the paragraph
                Run r = new Run();
                r.Text = e.NewValue.ToString();
                //italic
                r.FontStyle = FontStyles.Italic;
                r.Foreground = new SolidColorBrush(Colors.Gray);

                prgParagraph.Inlines.Add(r);
                WatermarkRichTextBox.Blocks.Add(prgParagraph);
                WatermarkRichTextBox.watermarkString = WatermarkRichTextBox.Xaml;
                //WatermarkRichTextBox.Xaml = e.NewValue.ToString();
                //WatermarkRichTextBox.SetMode(true);
            }
        }

        public String BindableXaml
        {
            get { return GetValue(BindableXamlProperty) as string; }
            set { SetValue(BindableXamlProperty, value); }
        }

        public static readonly DependencyProperty BindableXamlProperty =
            DependencyProperty.Register("BindableXaml", typeof(string), typeof(WatermarkRichTextBox), new PropertyMetadata(OnBindableXamlPropertyChanged));

        private static void OnBindableXamlPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            WatermarkRichTextBox WatermarkRichTextBox = sender as WatermarkRichTextBox;
            if (WatermarkRichTextBox != null && WatermarkRichTextBox._displayWatermark)
            {
                WatermarkRichTextBox.Xaml = "<Section xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'/>"; //e.NewValue.ToString();
                WatermarkRichTextBox.SetMode(true);
            }
        }
        private void ContentChangedXaml(object sender, System.Windows.Controls.ContentChangedEventArgs e)
        {
            if (!Xaml.Equals(watermarkString) && !String.IsNullOrEmpty(Xaml))
            {
                BindableXaml = Xaml;
            }
        }

        /*
        public new String Xaml
        {
            get
            {
                
                return _displayWatermark ? string.Empty : base.Xaml;
            }
            set
            {
                //SetMode(false);
                base.Xaml = value ?? string.Empty;
                if (base.Xaml.Equals(Watermark))
                {
                    _displayWatermark = true;
                    SetMode(true);
                }
                else if (String.IsNullOrEmpty(base.Xaml))
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
        }*/
    }
}
