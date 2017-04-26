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
    public partial class AddARecipe : ChildWindow
    {

        Guid foodId;
        
        public event EventHandler SavedClicked;

        public AddARecipe()
        {
            InitializeComponent();
            AddARecipeForm.SavedClicked += new EventHandler(AddARecipeForm_SavedClicked);

        }
        public AddARecipe(Guid foodId)
        {
            InitializeComponent();
            this.foodId = foodId;
            AddARecipeForm.SavedClicked += new EventHandler(AddARecipeForm_SavedClicked);
            AddARecipeForm.RetrieveFood(foodId);
        }

        void AddARecipeForm_SavedClicked(object sender, EventArgs e)
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

