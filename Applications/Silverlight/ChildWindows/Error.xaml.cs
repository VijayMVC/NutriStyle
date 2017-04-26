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
    public partial class Error : ChildWindow
    {

        private String message;

        public Error()
        {
            InitializeComponent();
        }
        public Error(String message)
        {
            InitializeComponent();
            this.message = message;
            statusBlock.Text = message;
        }
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}

