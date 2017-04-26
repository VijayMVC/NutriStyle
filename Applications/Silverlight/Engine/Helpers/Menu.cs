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
using DynamicConnections.NutriStyle.MenuGenerator.ChildWindows;
using System.Linq;


namespace DynamicConnections.NutriStyle.MenuGenerator.Engine.Helpers
{
    public class Menu
    {

        Action method;

        public void Generate(Guid contactId, Action method) {

            this.method = method;
            CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
            cms.InnerChannel.OperationTimeout = new TimeSpan(0, 5, 0);
            cms.GenerateMenuCompleted += new EventHandler<CrmSdk.GenerateMenuCompletedEventArgs>(cms_GenerateMenu_Click);
            cms.GenerateMenuAsync(contactId.ToString());

        }
        private void cms_GenerateMenu_Click(object sender, CrmSdk.GenerateMenuCompletedEventArgs e)
        {

            var errors = from x in e.Result.Descendants("error") select x;
            if (errors.Count() > 0)
            {
                String message = e.Result.Value.ToString();
                Status s = new Status(message, false);
                s.Show();
            }
            else
            {
                var results = e.Result;
                String Id = e.Result.Value.ToString();
                
                Guid menuId = new Guid(e.Result.Value);
                ((App)App.Current).contact.MenuId = menuId;
                /*
                PostMenuGeneration pmg = new PostMenuGeneration(new Guid(Id), new Guid(((App)App.Current).contactId));
                pmg.Show();
                */
                if (method != null)
                {
                    method();//execute passed in method
                }
            }
            //NavigationService.Navigate(new Uri("DailyMenu", UriKind.Relative));
        }
    }
}
