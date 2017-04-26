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
using System.Windows.Media.Imaging;

namespace DynamicConnections.NutriStyle.MenuGenerator.ChildWindows
{
    public partial class Status : ChildWindow
    {

        private String message;

        public Status()
        {
            InitializeComponent();
        }
        public Status(String message)
        {
            InitializeComponent();
            this.message = message;
            statusBlock.Text = message;

        }
        public Status(String message, bool success)
        {
            InitializeComponent();
            this.message = message;
            statusBlock.Text = message;
            if (success)
            {
                image.Source = new BitmapImage(new Uri("/DynamicConnections.NutriStyle.MenuGenerator;component/images/success.png", UriKind.Relative));

            }
        }
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        
    }
}

