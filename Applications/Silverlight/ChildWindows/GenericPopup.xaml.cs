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
    public partial class GenericPopup : ChildWindow
    {
        private String popupName;
        
        public GenericPopup()
        {
            InitializeComponent();
        }
        public GenericPopup(String popupName)
        {
            InitializeComponent();
            this.popupName = popupName;
            RetrievePopupData(popupName);
        }

        private void RetrievePopupData(String popupName)
        {
            
        String fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
            <entity name='dc_genericpopup'> 
                <attribute name='dc_genericpopupid'/> 
                <attribute name='dc_name'/> 
                <attribute name='dc_header'/>
                <attribute name='dc_subheader'/>
                <attribute name='dc_content'/>
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
            orderXml.Add(new XElement("Column", "dc_subheader"));
            orderXml.Add(new XElement("Column", "dc_content"));

            CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
            cms.RetrieveFetchXmlCompleted += new EventHandler<CrmSdk.RetrieveFetchXmlCompletedEventArgs>(cms_RetrievePopupData);
            cms.RetrieveFetchXmlAsync(fetchXml, orderXml.ToString());
        }

        private void cms_RetrievePopupData(object sender, CrmSdk.RetrieveFetchXmlCompletedEventArgs e)
        {
            String entityName = "dc_genericpopup";
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
                    else if (xe.Name.LocalName.Equals("dc_subheader", StringComparison.OrdinalIgnoreCase))
                    {
                        subHeader.Content = xe.Value;
                    }
                    else if (xe.Name.LocalName.Equals("dc_content", StringComparison.OrdinalIgnoreCase))
                    {
                        content.Text = xe.Value;
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

