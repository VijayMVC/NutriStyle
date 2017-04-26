using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using System.Xml.Linq;

namespace DynamicConnections.NutriStyle.MenuGenerator.ChildWindows
{
    public partial class FeedBack : ChildWindow
    {
        DynamicConnections.NutriStyle.MenuGenerator.Engine.Helpers.FeedBack fb;

        public FeedBack()
        {
            InitializeComponent();
            fb = new DynamicConnections.NutriStyle.MenuGenerator.Engine.Helpers.FeedBack();
        }
        /// <summary>
        /// Closes the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CLoseForm(object sender, EventArgs e)
        {
            this.DialogResult = true;
        }

        /// <summary>
        /// Save data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            //Need to validate first
            title.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            description.GetBindingExpression(TextBox.TextProperty).UpdateSource();

            if (!Validation.GetHasError(title) && !Validation.GetHasError(description))
            {
                busyIndicator.IsBusy = true;
                //build out xml document
                App ap = (App)App.Current;

                XElement feedbackXml = new XElement("dc_feedback");

                XElement contact = new XElement("dc_contactid", ap.contactId);
                feedbackXml.Add(contact);

                XAttribute at = new XAttribute("entityname", "contact");
                contact.Add(at);

                feedbackXml.Add(new XElement("dc_name", title.Text));
                feedbackXml.Add(new XElement("dc_description", description.Text));

                 CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
                cms.CreateUpdateCompleted += new EventHandler<CrmSdk.CreateUpdateCompletedEventArgs>(cms_CreateUpdateFeedBack);
                cms.CreateUpdateAsync(feedbackXml);
            }
        }
        private void cms_CreateUpdateFeedBack(object sender, CrmSdk.CreateUpdateCompletedEventArgs e)
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
                this.DialogResult = true;
            }
        }

        private void ChildWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LayoutRoot.DataContext = fb;
            title.Watermark = "Title";
            description.Watermark = "Description";
        }
    }
}

