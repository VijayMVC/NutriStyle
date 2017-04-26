using System;

using System.Windows.Controls;

using DynamicConnections.NutriStyle.MenuGenerator.Engine.Helpers;


namespace DynamicConnections.NutriStyle.MenuGenerator.Pages
{
    public partial class DailyFoodLog : Page
    {



        private RowIndexConverter _rowIndexConverter = new RowIndexConverter();

        public DailyFoodLog()
        {
            InitializeComponent();
            //Setup birthday values
            name.Content = ((App)App.Current).contact.Email;
        }
        public void MenuName(String value)
        {
            menuName.Content = value;
        }
    }
}
