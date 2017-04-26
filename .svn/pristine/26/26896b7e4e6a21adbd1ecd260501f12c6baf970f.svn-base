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
    public partial class EditMenu : ChildWindow
    {

        public event EventHandler SubmitClicked;
        private String nameField;
        private String descriptionField;

        private Guid menuId;

        public EditMenu()
        {
            InitializeComponent();
        }
        public EditMenu(Guid menuId)
        {
            InitializeComponent();
            this.menuId = menuId;
            
        }

        public String Name
        {
            get
            {
                return (nameField);
            }
            set
            {
                nameField = value;
            }
        }
        public String Description
        {
            get
            {
                return (descriptionField);
            }
            set
            {
                descriptionField = value;
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void ChildWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //Load up recipe
            RetrieveRecipe(menuId);
        }
        private void RetrieveRecipe(Guid foodId)
        {

            App ap = (App)App.Current;
            //Get Selected day
            

            String fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
              <entity name='dc_menu'>
                <attribute name='dc_name' />
                <attribute name='dc_description' />
               
                <filter type='and'>
                    <condition attribute='dc_menuid' operator='eq' value='@MENUID' />
                </filter>
                
              </entity>
            </fetch>";

            fetchXml = fetchXml.Replace("@MENUID", menuId.ToString());
           

            XElement orderXml = new XElement("ColumnOrder");
            orderXml.Add(new XElement("Column", "dc_name"));
            orderXml.Add(new XElement("Column", "dc_description"));
            

            try
            {
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
            String entityName = "dc_menu";

            if (e.Result.Descendants(entityName).Count() == 1)
            {
                var rows = e.Result.Descendants(entityName);
                var row = rows.First();
                foreach (XElement xe in row.Elements())
                {
                    if (xe.Name.LocalName.Equals("dc_name", StringComparison.OrdinalIgnoreCase))
                    {
                        name.Text = xe.Value;
                    }
                    else if (xe.Name.LocalName.Equals("dc_description", StringComparison.OrdinalIgnoreCase))
                    {
                        description.Text = xe.Value;
                    }
                   
                }

            }
            busyIndicator.IsBusy = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            busyIndicator.IsBusy = true;
            XElement menuXml = new XElement("dc_menu");
            menuXml.Add(new XElement("dc_menuid", menuId.ToString()));
            menuXml.Add(new XElement("dc_name", name.Text));
            menuXml.Add(new XElement("dc_description", description.Text));


            CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
            cms.CreateUpdateCompleted += new EventHandler<CrmSdk.CreateUpdateCompletedEventArgs>(cms_CreateUpdateMenu);
            cms.CreateUpdateAsync(menuXml);
        }

        private void cms_CreateUpdateMenu(object sender, CrmSdk.CreateUpdateCompletedEventArgs e)
        {
            var errors = from x in e.Result.Descendants("error") select x;
            if (errors.Count() > 0)
            {
                busyIndicator.IsBusy = false;
                String message = e.Result.Value.ToString();
                Status s = new Status(message, false);
                s.Show();

            }
            else
            {
                if (SubmitClicked != null)
                {
                    nameField = name.Text;
                    descriptionField = description.Text;
                    SubmitClicked(this, new EventArgs());
                }
                this.DialogResult = true;
            }
        }
    }
}

