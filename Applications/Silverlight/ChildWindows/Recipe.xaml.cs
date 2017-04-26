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
using System.Windows.Browser;
using DynamicConnections.NutriStyle.MenuGenerator.Engine.Helpers;

namespace DynamicConnections.NutriStyle.MenuGenerator.ChildWindows
{
    public partial class Recipe : ChildWindow
    {

        
        private Guid foodId;

        public Recipe()
        {
            InitializeComponent();
        }
        public Recipe(Guid foodId) {
            InitializeComponent();
            this.foodId = foodId;
            
        }


        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void ChildWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //Load up recipe
            RetrieveRecipe(foodId);
        }
        private void RetrieveRecipe(Guid foodId)
        {

            App ap = (App)App.Current;
            //Get Selected day
            

            String fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
              <entity name='dc_foods'>
                <attribute name='dc_name' />
                <attribute name='dc_directions' />
                <attribute name='dc_preparationtime' />
                <attribute name='dc_numberofservings' />
                <attribute name='dc_portiontypeid' />
                <attribute name='dc_sourceofinformationdateobtained' />
                <attribute name='dc_recipeingredients' />
                <attribute name='dc_portion_amount' />
                <filter type='and'>
                  <condition attribute='statecode' operator='eq' value='0' />
                  <condition attribute='dc_recipefood' operator='eq' value='1' />
                    <condition attribute='dc_foodsid' operator='eq' value='@FOODID' />
                </filter>
                
              </entity>
            </fetch>";

            fetchXml = fetchXml.Replace("@FOODID", foodId.ToString());
           

            XElement orderXml = new XElement("ColumnOrder");
            orderXml.Add(new XElement("Column", "dc_name"));
            orderXml.Add(new XElement("Column", "dc_directions"));
            orderXml.Add(new XElement("Column", "dc_preparationtime"));
            orderXml.Add(new XElement("Column", "dc_numberofservings"));

            orderXml.Add(new XElement("Column", "dc_portiontypeid"));
            orderXml.Add(new XElement("Column", "dc_sourceofinformationdateobtained"));
            orderXml.Add(new XElement("Column", "dc_recipeingredients"));
            orderXml.Add(new XElement("Column", "dc_portion_amount"));

            try
            {
                /*
                busyIndicator.IsBusy = true;
                busyIndicator.Visibility = System.Windows.Visibility.Visible;
                */
                CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
                cms.RetrieveFetchXmlCompleted += new EventHandler<CrmSdk.RetrieveFetchXmlCompletedEventArgs>(cms_RetrieveRecipe);
                cms.RetrieveFetchXmlAsync(fetchXml, orderXml.ToString());
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
            }
        }
        private void cms_RetrieveRecipe(object sender, CrmSdk.RetrieveFetchXmlCompletedEventArgs e)
        {
            String entityName = "dc_foods";

            if (e.Result.Descendants(entityName).Count() == 1)
            {
                var rows = e.Result.Descendants(entityName);
                var row = rows.First();
                foreach (XElement xe in row.Elements())
                {
                    if (xe.Name.LocalName.Equals("dc_name", StringComparison.OrdinalIgnoreCase))
                    {
                        name.Content = xe.Value;
                    }
                    else if (xe.Name.LocalName.Equals("dc_preparationtime", StringComparison.OrdinalIgnoreCase))
                    {
                        prepTime.Text = xe.Value;
                    }
                    else if (xe.Name.LocalName.Equals("dc_numberofservings", StringComparison.OrdinalIgnoreCase))
                    {
                        numberOfServings.Text = xe.Value;
                    }
                    else if (xe.Name.LocalName.Equals("dc_portion_amount", StringComparison.OrdinalIgnoreCase))
                    {
                        amount.Text = xe.Value;
                    }
                    else if (xe.Name.LocalName.Equals("dc_portiontypeid", StringComparison.OrdinalIgnoreCase))
                    {
                        type.Text = xe.Value;
                    }

                    else if (xe.Name.LocalName.Equals("dc_sourceofinformationdateobtained", StringComparison.OrdinalIgnoreCase))
                    {
                        source.Text = xe.Value;
                    }

                    else if (xe.Name.LocalName.Equals("dc_directions", StringComparison.OrdinalIgnoreCase))
                    {
                        directions.Text = xe.Value.Replace("<br><br>", "\n").Replace("<br>", "\n");
                    }

                    else if (xe.Name.LocalName.Equals("dc_recipeingredients", StringComparison.OrdinalIgnoreCase))
                    {
                        ingredients.Text = xe.Value.Replace("<br><br>", "\n").Replace("<br>", "\n");
                    }
                }

            }
            busyIndicator.IsBusy = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //print
            HtmlPage.Window.Navigate(new Uri(General.ReportServerUrl(((App)App.Current).webServicesName) + "/Recipe&rs:Command=Render&recipeId=" + foodId.ToString()), "_blank");
        }
        
    }
}

