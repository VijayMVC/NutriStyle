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
using System.Windows.Navigation;
using DynamicConnections.NutriStyle.MenuGenerator.ChildWindows;
using System.Xml.Linq;

namespace DynamicConnections.NutriStyle.MenuGenerator.Pages
{
    public partial class GenerateMenu : Page
    {
        

        public GenerateMenu()
        {
            InitializeComponent();
            BuildMenu();
        }
        private void BuildMenu()
        {
            App ap = (App)App.Current;

            busyIndicator.IsBusy = true;
            CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
            cms.GenerateMenuCompleted += new EventHandler<CrmSdk.GenerateMenuCompletedEventArgs>(cms_MenuBuilt);
            cms.GenerateMenuAsync(ap.contactId);
        }
        private void cms_MenuBuilt(object sender, CrmSdk.GenerateMenuCompletedEventArgs e)
        {
            //Move to the menu editor
            var errors = from x in e.Result.Descendants("error") select x;
            if (errors.Count() > 0)
            {

                String message = e.Result.Value.ToString();
                Status s = new Status(message, false);
                s.Show();
            }
            else
            {
                //var rows = xml.Descendants(entityName);
                var results = e.Result.Descendants("Success");

                Guid menuId = new Guid(e.Result.Value);
                ((App)App.Current).contact.MenuId = menuId;
            }
            NavigationService.Navigate(new Uri("MenuEditor", UriKind.Relative));
        }
       
    }
}
