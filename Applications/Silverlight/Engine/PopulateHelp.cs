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
using System.Xml.Linq;
using System.Linq;
using DynamicConnections.NutriStyle.MenuGenerator.Engine.Helpers;
using System.Windows.Media.Imaging;
using System.Windows.Browser;

namespace DynamicConnections.NutriStyle.MenuGenerator.Engine
{
    public class PopulateHelp
    {
        TextBlock tb;
        public PopulateHelp() { }
        Image topLeft;
        Image bottomLeft;
        Image topRight;
        Image bottomRight;

        String topLeftStrURL;
        String bottomLeftStrURL;
        String topRightStrURL;
        String bottomRightStrURL;

        public void Retrieve(String name, TextBlock control, Image topLeft, Image bottomLeft, Image topRight, Image bottomRight)
        {
            tb              = control;
            this.topLeft    = topLeft;
            this.bottomLeft = bottomLeft;
            this.topRight   = topRight;
            this.bottomRight = bottomRight;

            topLeftStrURL = String.Empty;
            bottomLeftStrURL = String.Empty;
            topRightStrURL = String.Empty;
            bottomRightStrURL = String.Empty;

            
            topRight.MouseLeftButtonUp -= topRight_MouseLeftButtonUp;
            topRight.MouseLeftButtonUp += new MouseButtonEventHandler(topRight_MouseLeftButtonUp);


            String fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                  <entity name='dc_help'>
                    <attribute name='dc_comments' />
                    <attribute name='dc_topleft' />
                    <attribute name='dc_bottomleft' />
                    <attribute name='dc_topright' />
                    <attribute name='dc_bottomright' />

                    <attribute name='dc_toplefturl' />
                    <attribute name='dc_bottomlefturl' />
                    <attribute name='dc_toprighturl' />
                    <attribute name='dc_bottomrighturl' />

                    <attribute name='dc_helpid' />
                          <filter type='and'>
                            <condition attribute='dc_name' operator='eq' value='@TEXT' />
                          </filter>
                  </entity>
                </fetch>";

            fetchXml = fetchXml.Replace("@TEXT", name);

            XElement orderXml = new XElement("ColumnOrder");
            orderXml.Add(new XElement("Column", "dc_comments"));
            orderXml.Add(new XElement("Column", "dc_topleft"));
            orderXml.Add(new XElement("Column", "dc_bottomleft"));
            orderXml.Add(new XElement("Column", "dc_topright"));
            orderXml.Add(new XElement("Column", "dc_bottomright"));

            orderXml.Add(new XElement("Column", "dc_toplefturl"));
            orderXml.Add(new XElement("Column", "dc_bottomlefturl"));
            orderXml.Add(new XElement("Column", "dc_toprighturl"));
            orderXml.Add(new XElement("Column", "dc_bottomrighturl"));

            orderXml.Add(new XElement("Column", "dc_helpid"));
           
            try
            {
                CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
                cms.RetrieveFetchXmlCompleted += new EventHandler<CrmSdk.RetrieveFetchXmlCompletedEventArgs>(cms_RetrieveHelp);
                cms.RetrieveFetchXmlAsync(fetchXml, orderXml.ToString());
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
            }
        }
        private void cms_RetrieveHelp(object sender, CrmSdk.RetrieveFetchXmlCompletedEventArgs e)
        {
            
            try
            {
                //MessageBox.Show(e.Result.ToString());
                String entityName = "dc_help";
                XElement element = e.Result;
               
           
                if (element.Descendants(entityName).Count() > 0)
                {
                    //Bind data
                    var rows = element.Descendants(entityName);

                    var row = rows.First();
                    Pair p = new Pair();
                    String topLeftStr = String.Empty;
                    String bottomLeftStr = String.Empty;
                    String topRightStr = String.Empty;
                    String bottomRightStr = String.Empty;

                   


                    foreach (XElement xe in row.Elements())
                    {
                        if (xe.Name.LocalName.Equals("dc_comments", StringComparison.OrdinalIgnoreCase))
                        {
                            tb.Text = xe.Value;
                        }
                        else if (xe.Name.LocalName.Equals("dc_topleft", StringComparison.OrdinalIgnoreCase))
                        {
                            topLeftStr = xe.Value;
                        }
                        else if (xe.Name.LocalName.Equals("dc_bottomleft", StringComparison.OrdinalIgnoreCase))
                        {
                            bottomLeftStr = xe.Value;
                        }
                        else if (xe.Name.LocalName.Equals("dc_topright", StringComparison.OrdinalIgnoreCase))
                        {
                            topRightStr = xe.Value;
                        }
                        else if (xe.Name.LocalName.Equals("dc_bottomright", StringComparison.OrdinalIgnoreCase))
                        {
                            bottomRightStr = xe.Value;
                        }

                        
                        else if (xe.Name.LocalName.Equals("dc_toplefturl", StringComparison.OrdinalIgnoreCase))
                        {
                            topLeftStrURL = xe.Value;
                        }
                        else if (xe.Name.LocalName.Equals("dc_bottomlefturl", StringComparison.OrdinalIgnoreCase))
                        {
                            bottomLeftStrURL = xe.Value;
                        }
                        else if (xe.Name.LocalName.Equals("dc_toprighturl", StringComparison.OrdinalIgnoreCase))
                        {
                            topRightStrURL = xe.Value;
                        }
                        else if (xe.Name.LocalName.Equals("dc_bottomrighturl", StringComparison.OrdinalIgnoreCase))
                        {
                            bottomRightStrURL = xe.Value;
                        }
                    }

                    if (!String.IsNullOrEmpty(topLeftStr))
                    {
                        Uri uri = new Uri(topLeftStr);
                        topLeft.Source = new BitmapImage(uri);
                    }
                    if (!String.IsNullOrEmpty(bottomLeftStr))
                    {
                        Uri uri = new Uri(bottomLeftStr);
                        bottomLeft.Source = new BitmapImage(uri);
                    }
                    if (!String.IsNullOrEmpty(topRightStr))
                    {
                        Uri uri = new Uri(topRightStr);
                        topRight.Source = new BitmapImage(uri);
                        
                        
                    }
                    if (!String.IsNullOrEmpty(bottomRightStr))
                    {
                        Uri uri = new Uri(bottomRightStr);
                        bottomRight.Source = new BitmapImage(uri);
                    }
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
            }
        }

        void topRight_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

            HtmlPage.Window.Navigate(new Uri(topRightStrURL), "popup", "directories=no,fullscreen=no,menubar=no,resizable=yes,scrollbars=yes,status=no,titlebar=no,toolbar=no," + App.Height + ", " + App.Width);
        }


       
    }

}
