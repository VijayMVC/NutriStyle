using System;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using System.Linq;
using DynamicConnections.NutriStyle.MenuGenerator.Engine.Helpers;

namespace DynamicConnections.NutriStyle.MenuGenerator.Engine
{
    public class PopulateTip
    {
        ScrollViewer tb;
        public PopulateTip() { }

        /// <summary>
        /// Retrieve the tip from CRM.  The passed in parameter is the control that is the parent
        /// </summary>
        /// <param name="control"></param>
        public void Retrieve(ref ScrollViewer control)
        {
            tb = control;

            try
            {
                CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
                cms.RetrieveDailyTipCompleted += new EventHandler<CrmSdk.RetrieveDailyTipCompletedEventArgs>(cms_RetrieveTip);
                cms.RetrieveDailyTipAsync();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
            }
        }
        public void Retrieve()
        {
            try
            {
                CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
                cms.RetrieveDailyTipCompleted += new EventHandler<CrmSdk.RetrieveDailyTipCompletedEventArgs>(cms_RetrieveTip);
                cms.RetrieveDailyTipAsync();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
            }
        }
        private void cms_RetrieveTip(object sender, CrmSdk.RetrieveDailyTipCompletedEventArgs e)
        {

            //Build out structure for tip
            StackPanel sp = new StackPanel();

            TextBlock title = new TextBlock();
            TextBlock comments = new TextBlock();
            TextBlock source = new TextBlock();
            
            sp.HorizontalAlignment = HorizontalAlignment.Stretch;
            sp.VerticalAlignment = VerticalAlignment.Stretch;

            sp.Children.Add(title);
            sp.Children.Add(comments);
            sp.Children.Add(source);

            comments.VerticalAlignment = VerticalAlignment.Stretch;

            title.TextWrapping = TextWrapping.Wrap;
            comments.TextWrapping = TextWrapping.Wrap;
            source.TextWrapping = TextWrapping.Wrap;

            title.FontSize = 12d;
            title.FontWeight = FontWeights.Bold;

            comments.FontSize = 10d;
            comments.Margin = new Thickness(0d, 10d, 0d, 10d);
            comments.FontWeight = FontWeights.Normal;
            source.FontSize = 8d;
            source.FontWeight = FontWeights.Normal;
            /*
            title.BorderThickness = new Thickness(0d);
            comments.BorderThickness = new Thickness(0d);
            source.BorderThickness = new Thickness(0d);
            */
            title.HorizontalAlignment = HorizontalAlignment.Left;
            comments.HorizontalAlignment = HorizontalAlignment.Left;
            source.HorizontalAlignment = HorizontalAlignment.Left;
            tb.Content = sp;

            //sp.Orientation = Orientation.Vertical;

            try
            {
                String entityName = "dc_tipoftheday";
                XElement element = e.Result;
                
           
                if (element.Descendants(entityName).Count() > 0)
                {
                    //Bind data
                    var rows = element.Descendants(entityName);

                    var row = rows.First();
                    Pair p = new Pair();

                    foreach (XElement xe in row.Elements())
                    {
                        if (xe.Name.LocalName.Equals("dc_comments", StringComparison.OrdinalIgnoreCase))
                        {
                            //tb.Text = xe.Value;
                            comments.Text = xe.Value;
                        }
                        else if (xe.Name.LocalName.Equals("dc_titletopic", StringComparison.OrdinalIgnoreCase))
                        {
                            title.Text = xe.Value;
                        }
                        else if (xe.Name.LocalName.Equals("dc_sourcedate", StringComparison.OrdinalIgnoreCase))
                        {
                            source.Text = xe.Value;
                        }
                    }
                }
            }
            catch (Exception err)
            {
                //MessageBox.Show(err.Message);
                //MessageBox.Show(err.StackTrace);
                //ChildWindows.Error error = new Error(err.Message +"\n"+err.StackTrace);
                //error.Show();
                //Sleep and make the request again
                Random r = new Random(100);
                System.Threading.Thread.Sleep(new TimeSpan(5*Convert.ToInt32(r.NextDouble())));
                Retrieve();

            }

        }

    }

}
