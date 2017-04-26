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
    public partial class ActivityLevelsPopup : ChildWindow
    {
        private String popupName;

        public ActivityLevelsPopup()
        {
            InitializeComponent();
        }

        public ActivityLevelsPopup(String popupName)
        {
            InitializeComponent();
            this.popupName = popupName;
            RetrievePopupData(popupName);
        }

        private void RetrievePopupData(String popupName)
        {
           String fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
              <entity name='dc_activitylevelspopup'> 
                    <attribute name='dc_activitylevelspopupid'/> 
                    <attribute name='dc_name'/> 
                    <attribute name='dc_header' />
                    <attribute name='dc_sedentarysubheader' />
                    <attribute name='dc_sedentarycontent' />
                    <attribute name='dc_lightactivitysubheader' />
                    <attribute name='dc_lightactivitycontent' />
                    <attribute name='dc_moderateactivitysubheader' />
                    <attribute name='dc_moderateactivitycontent' />
                    <attribute name='dc_veryactivesubheader' />
                    <attribute name='dc_veryactivecontent' />
                    <filter type='and'> 
                        <condition attribute='dc_name' value='@POPUPNAME' operator='eq'/> 
                    </filter> 
                </entity> 
            </fetch>";

            fetchXml = fetchXml.Replace("@POPUPNAME", popupName);

            XElement orderXml = new XElement("ColumnOrder");

            orderXml.Add(new XElement("Column", "dc_popupid"));
            orderXml.Add(new XElement("Column", "dc_name"));
            orderXml.Add(new XElement("Column", "dc_header"));
            orderXml.Add(new XElement("Column", "dc_sedentarysubheader"));
            orderXml.Add(new XElement("Column", "dc_sedentarycontent"));
            orderXml.Add(new XElement("Column", "dc_lightactivitysubheader"));
            orderXml.Add(new XElement("Column", "dc_lightactivitycontent"));
            orderXml.Add(new XElement("Column", "dc_moderateactivitysubheader"));
            orderXml.Add(new XElement("Column", "dc_moderateactivitycontent"));
            orderXml.Add(new XElement("Column", "dc_veryactivesubheader"));
            orderXml.Add(new XElement("Column", "dc_veryactivecontent"));

            CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
            cms.RetrieveFetchXmlCompleted += new EventHandler<CrmSdk.RetrieveFetchXmlCompletedEventArgs>(cms_RetrievePopupData);
            cms.RetrieveFetchXmlAsync(fetchXml, orderXml.ToString());

        }

        private void cms_RetrievePopupData(object sender, CrmSdk.RetrieveFetchXmlCompletedEventArgs e)
        {
            String entityName = "dc_activitylevelspopup";
            if (e.Result.Descendants(entityName).Count() == 1)
            {
                var rows = e.Result.Descendants(entityName);
                var row = rows.First();
                foreach (XElement xe in row.Elements())
                {
                    if (xe.Name.LocalName.Equals("dc_header", StringComparison.OrdinalIgnoreCase))
                    {
                        header.Content = xe.Value;
                    }
                    else if (xe.Name.LocalName.Equals("dc_sedentarysubheader", StringComparison.OrdinalIgnoreCase))
                    {
                        sedentarySubHeader.Content = xe.Value;
                    }
                    else if (xe.Name.LocalName.Equals("dc_sedentarycontent", StringComparison.OrdinalIgnoreCase))
                    {
                        sedentaryContent.Text = xe.Value;
                    }
                    else if (xe.Name.LocalName.Equals("dc_lightactivitysubheader", StringComparison.OrdinalIgnoreCase))
                    {
                        lightActivitySubHeader.Content = xe.Value;
                    }
                    else if (xe.Name.LocalName.Equals("dc_lightactivitycontent", StringComparison.OrdinalIgnoreCase))
                    {
                        lightActivityContent.Text = xe.Value;
                    }
                    else if (xe.Name.LocalName.Equals("dc_moderateactivitysubheader", StringComparison.OrdinalIgnoreCase))
                    {
                        moderateActivitySubHeader.Content = xe.Value;
                    }
                    else if (xe.Name.LocalName.Equals("dc_moderateactivitycontent", StringComparison.OrdinalIgnoreCase))
                    {
                        moderateActivityContent.Text = xe.Value;
                    }
                    else if (xe.Name.LocalName.Equals("dc_veryactivesubheader", StringComparison.OrdinalIgnoreCase))
                    {
                        veryActiveSubHeader.Content = xe.Value;
                    }
                    else if (xe.Name.LocalName.Equals("dc_veryactivecontent", StringComparison.OrdinalIgnoreCase))
                    {
                        veryActiveContent.Text = xe.Value;
                    }
                }
            }

        }
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

    }
}

