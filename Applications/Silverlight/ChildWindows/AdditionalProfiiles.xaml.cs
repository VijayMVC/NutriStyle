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
    public partial class AdditionalProfiles : ChildWindow
    {

        public event EventHandler SavedClicked;

        public AdditionalProfiles()
        {
            InitializeComponent();
        }
        public AdditionalProfiles(Guid contactId)
        {
            InitializeComponent();
            
        }
        
        void Close_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}

