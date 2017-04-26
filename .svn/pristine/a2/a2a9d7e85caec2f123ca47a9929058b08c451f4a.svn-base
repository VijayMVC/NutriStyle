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
using System.Xml.Linq;

namespace DynamicConnections.NutriStyle.MenuGenerator.ChildWindows
{
    public partial class AddAFood : ChildWindow
    {

        public event EventHandler SavedClicked;

       
        Guid foodId;

        public AddAFood()
        {
            InitializeComponent();
            AddAFoodForm.SavedClicked += new EventHandler(AddAFoodForm_SavedClicked);
        }
        public AddAFood(Guid foodId)
        {
            InitializeComponent();
            AddAFoodForm.SavedClicked += new EventHandler(AddAFoodForm_SavedClicked);
            this.foodId = foodId;
            AddAFoodForm.FoodId = this.foodId;
        }

        void AddAFoodForm_SavedClicked(object sender, EventArgs e)
        {
            XElement element = (XElement)sender;
            SavedClicked(element, new EventArgs());
            this.DialogResult = true;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}

