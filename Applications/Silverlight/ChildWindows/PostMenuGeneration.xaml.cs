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

namespace DynamicConnections.NutriStyle.MenuGenerator.ChildWindows
{
    public partial class PostMenuGeneration : ChildWindow
    {
        SyncRun SR;
        private Guid menuId;
        private Guid contactId;

        public PostMenuGeneration(Guid menuId, Guid contactId)
        {

            this.contactId  = contactId;
            this.menuId     = menuId;

            InitializeComponent();
            SR = new SyncRun();
            SR.FinalAsync(Complete);
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            //Make sure something is selected;
            if (shoppingList.IsChecked.Value || foodLog.IsChecked.Value)
            {

                busyIndicator.IsBusy = true;
                //figure out what's selected
                bool generateShoppingList = shoppingList.IsChecked.Value;
                bool generateFoodLog = foodLog.IsChecked.Value;

                if (generateShoppingList)
                {
                    SR.MultipleAsyncRun(GenerateShoppingList);
                }
                if (generateFoodLog)
                {
                    SR.MultipleAsyncRun(GenerateFoodLog);
                }
            }
            else
            {
                Status s = new Status("Please check a checkbox before proceeding", false);
                s.Show();
            }
        }
        private void GenerateShoppingList()
        {

            CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
            cms.GenerateShoppingListFromMenuCompleted += new EventHandler<CrmSdk.GenerateShoppingListFromMenuCompletedEventArgs>(cms_GenerateShoppingList);
            cms.GenerateShoppingListFromMenuAsync(menuId.ToString());
            cms.GenerateShoppingListFromMenuCompleted += (s, e) => { SR.AssignResultCheckforAllAsyncsDone(e, e.Result, "Acquisition failure for GenerateShoppingList"); };
            
        }
        private void cms_GenerateShoppingList(object sender, CrmSdk.GenerateShoppingListFromMenuCompletedEventArgs e)
        {
            
        }

        void GenerateFoodLog()
        {
            CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
            cms.CreateFoodLogFromMenuCompleted += new EventHandler<CrmSdk.CreateFoodLogFromMenuCompletedEventArgs>(cms_CreateFoodLog);
            cms.CreateFoodLogFromMenuAsync(menuId.ToString(), contactId.ToString());
            cms.CreateFoodLogFromMenuCompleted += (s, e) => { SR.AssignResultCheckforAllAsyncsDone(e, e.Result, "Acquisition failure for GenerateFoodLog"); };
        }
        private void cms_CreateFoodLog(object sender, CrmSdk.CreateFoodLogFromMenuCompletedEventArgs e)
        {
           
        }

        private void Complete()
        {
            busyIndicator.IsBusy = false;
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}

