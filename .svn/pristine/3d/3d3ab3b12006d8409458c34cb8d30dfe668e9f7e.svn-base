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
using DynamicConnections.NutriStyle.MenuGenerator.Engine.Helpers;
using DynamicConnections.NutriStyle.MenuGenerator.Controls;

namespace DynamicConnections.NutriStyle.MenuGenerator.Pages
{
    public partial class Login : Page
    {
       
        Engine.Helpers.Login l;

        public Login()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void login_keyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter))
            {
            }
        }
        private void login_Click(object sender, RoutedEventArgs e)
        {
            String email = emailAddress.Text;
            String passwordStr = password.TrueText;
            String message = String.Empty;

            if (String.IsNullOrWhiteSpace(email))
            {
                message = "Please enter email address";
            }
            else if (String.IsNullOrWhiteSpace(passwordStr))
            {
                message = "Please enter password";
            }

            if (message.Length > 0)
            {
                Status s = new Status(message);
                s.Show();
                return;
            }
            loginBusyIndicator.IsBusy = true;
            CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
            cms.LoginCompleted += new EventHandler<CrmSdk.LoginCompletedEventArgs>(cms_Login);
            cms.LoginAsync(email, passwordStr);
        }
        private void cms_Login(object sender, CrmSdk.LoginCompletedEventArgs e)
        {
            try
            {
                Guid userId = Guid.Empty;
                var results = from x in e.Result.Descendants("valid") select x;
                if (results.Count() > 0)
                {
                    foreach (var result in results)
                    {
                        String str = result.Value;
                        if (str.Equals("false", StringComparison.OrdinalIgnoreCase))
                        {
                            loginBusyIndicator.IsBusy = false;
                            Status s = new Status("Your login failed.  Please check your email, password and try again");
                            s.Show();
                        }
                    }
                }
                else
                {

                    var results2 = from x in e.Result.Descendants("contact") select x;

                    foreach (var result2 in results2)
                    {
                        Contact c = null;

                        c = Contact.BuildContact(result2);

                        if (c != null && c.ContactId != null && c.ContactId != Guid.Empty)
                        {
                            
                            App ap = (App)App.Current;
                            ap.contactId = c.ContactId.ToString();
                            ap.contact = c;
                        }

                    }
                    NavigationService.Navigate(new Uri("Profile", UriKind.Relative));
                    //loginBusyIndicator.IsBusy = false;
                }

            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
            }
        }

        private void newUser_Click(object sender, RoutedEventArgs e)
        {
            newUserPanel.Visibility = System.Windows.Visibility.Visible;
            ValidationSummary.Visibility = System.Windows.Visibility.Visible;
            loginPanel.Visibility = System.Windows.Visibility.Collapsed;
        }
        /// <summary>
        /// Creates new user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void createUser_Click(object sender, RoutedEventArgs e)
        {

            //Make sure email is populated and passwords are the same
            FirstName.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            LastName.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            PostalCode.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            VerificationCodeBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            EmailAddress.GetBindingExpression(TextBox.TextProperty).UpdateSource();


            Password.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            VerifyPassword.GetBindingExpression(TextBox.TextProperty).UpdateSource();

            GrocerPrimary.GetBindingExpression(ComboBoxWithValidation.SelectedPairProperty).UpdateSource();
            GrocerSecondary.GetBindingExpression(ComboBoxWithValidation.SelectedPairProperty).UpdateSource();
            GrocerTertiary.GetBindingExpression(ComboBoxWithValidation.SelectedPairProperty).UpdateSource();
            Country.GetBindingExpression(ComboBoxWithValidation.SelectedPairProperty).UpdateSource();

            grocerOther.GetBindingExpression(TextBox.TextProperty).UpdateSource();

            String email = l.Email;//newEmailAddress.Text;
            String password = Password.TrueText;// newPassword.Password;
            String password2 = l.PasswordVerify;// newPasswordValid.Password;
            String code = l.VerificationCode;//verificationCode.Text;
            String zipCode = l.ZipCode;//newZipCode.Text;
            String message = String.Empty;

            if (!Validation.GetHasError(EmailAddress) && !Validation.GetHasError(Password) &&
                !Validation.GetHasError(VerifyPassword) && !Validation.GetHasError(VerificationCodeBox) &&
                !Validation.GetHasError(PostalCode)
            )
            {
                String fetchXml = @"<fetch distinct='false' mapping='logical' output-format='xml-platform' version='1.0'> 
                    <entity name='dc_verifycustomer'> 
                        <attribute name='dc_verifycustomerid'/> 
                        <attribute name='dc_name'/> 
                        <filter type='and'> 
                            <condition attribute='dc_name' value='@VERIFYCUSTOMERID' operator='eq'/> 
                        </filter> 
                     </entity> 
                </fetch>";

                fetchXml = fetchXml.Replace("@VERIFYCUSTOMERID", code);

                XElement orderXml = new XElement("ColumnOrder");
                orderXml.Add(new XElement("Column", "dc_name"));
                orderXml.Add(new XElement("Column", "dc_verifycustomerid"));

                //call the server
                CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
                cms.RetrieveFetchXmlCompleted += new EventHandler<CrmSdk.RetrieveFetchXmlCompletedEventArgs>(cms_RetrieveVerification);
                cms.RetrieveFetchXmlAsync(fetchXml, orderXml.ToString());

            }
            else
            {
                Status s = new Status("Please fill out all required fields", false);
                s.Show();
            }
        }
        private void cms_RetrieveVerification(object sender, CrmSdk.RetrieveFetchXmlCompletedEventArgs e)
        {
            String message = String.Empty;
            String entityName = "dc_verifycustomer";
            Guid verificationCodeId = Guid.Empty;
            if (e.Result.Descendants(entityName).Count() == 1)
            {
                var rows = e.Result.Descendants(entityName);
                var row = rows.First();
                foreach (XElement xe in row.Elements())
                {
                    if (xe.Name.LocalName.Equals("dc_verifycustomerid", StringComparison.OrdinalIgnoreCase))
                    {

                        verificationCodeId = new Guid(xe.Value);
                        //CreateContact();
                    }
                    else
                    {
                        message = "The code does not exist, please try again.";
                    }
                }

                if (verificationCodeId != Guid.Empty)
                {

                    CreateContact(verificationCodeId);
                }
            }
            else
            {
                //validation not found
                Status s = new Status("Validation code was not valid", false);
                s.Show();
            }
        }
        private void CreateContact(Guid verificationCodeId)
        {

            //String email = emailAddress.Text;
            //Create xmldocument and submit
            loginBusyIndicator.IsBusy = true;
            XElement entityXml = new XElement("contact");

            entityXml.Add(new XElement("emailaddress1", EmailAddress.Text));
            entityXml.Add(new XElement("dc_password", Password.TrueText));
            entityXml.Add(new XElement("address1_postalcode", PostalCode.Text));

            CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
            cms.CreateUserCompleted += new EventHandler<CrmSdk.CreateUserCompletedEventArgs>(cms_CreateUser);
            cms.CreateUserAsync(l.Email, Password.TrueText, l.ZipCode, l.FirstName, 
                l.LastName, l.GrocerPrimary.Id, l.GrocerSecondary.Id,
                l.GrocerTertiary.Id, l.Country.Id, l.GrocerOther, verificationCodeId.ToString());
        }
        private void cms_CreateUser(object sender, CrmSdk.CreateUserCompletedEventArgs e)
        {
            try
            {
                var errors = from x in e.Result.Descendants("error") select x;
                if (errors.Count() > 0)
                {
                    loginBusyIndicator.IsBusy = false;
                    String message = e.Result.Value.ToString();
                    Status s = new Status(message);
                    s.Show();
                }
                else
                {
                    App ap = (App)App.Current;
                    Contact c = new Contact();
                    c.FirstName = l.FirstName;
                    c.LastName = l.LastName;
                    c.ContactId = new Guid(e.Result.Value);
                    c.Email = l.Email;

                    ap.contactId = c.ContactId.ToString();
                    ap.contact = c;
                    //loginBusyIndicator.IsBusy = false;
                    NavigationService.Navigate(new Uri("Profile", UriKind.Relative));

                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
            }

        }
        private void returnToLogin_Click(object sender, RoutedEventArgs e)
        {
            newUserPanel.Visibility = System.Windows.Visibility.Collapsed;
            ValidationSummary.Visibility = System.Windows.Visibility.Collapsed;
            loginPanel.Visibility = System.Windows.Visibility.Visible;
        }

        private void newUser_KeyDown(object sender, KeyEventArgs e)
        {

        }
        private void Password_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                login_Click(null, null);
            }
        }

        private void CreatePasswordKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                createUser_Click(null, null);
            }
        }

        private void forgotPassword_Click(object sender, RoutedEventArgs e)
        {

            CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
            cms.ResendPasswordCompleted += new EventHandler<CrmSdk.ResendPasswordCompletedEventArgs>(cms_SendPassword);
            cms.ResendPasswordAsync(emailAddress.Text);
        }
        private void cms_SendPassword(object sender, CrmSdk.ResendPasswordCompletedEventArgs e)
        {
            try
            {
                var errors = from x in e.Result.Descendants("error") select x;
                if (errors.Count() > 0)
                {
                    loginBusyIndicator.IsBusy = false;
                    String message = e.Result.Value.ToString();
                    Status s = new Status(message, false);
                    s.Show();
                }
                else
                {
                    Status s = new Status("Please check your email.  Your password has been set to your email address", true);
                    s.Show();
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("Page_Loaded");
            l = new Engine.Helpers.Login();
            //Setting watermarks here.  Not ideal
            
            LayoutRoot.DataContext = l;
            EmailAddress.Watermark = "Email Address";
            FirstName.Watermark = "First Name";
            LastName.Watermark = "Last Name";
            PostalCode.Watermark = "Postal Code";
            Password.Watermark = "Password";
            VerifyPassword.Watermark = "Password";
            VerificationCodeBox.Watermark = "Verification Code";
            grocerOther.Watermark = "Grocer (Other)";
            //Find usa
            PairWithList pwl = new PairWithList();

            foreach(PairWithList pair in ((App)App.Current).countries) {
                if (pair.Name.Equals("USA", StringComparison.OrdinalIgnoreCase))
                {
                    pwl = pair;
                }
            }
            Country.SelectedPair = new PairWithList(pwl.Name, pwl.Id, String.Empty, ((App)App.Current).countries);
            l.Country = pwl;

            GrocerPrimary.SelectedPair = new PairWithList(String.Empty, Guid.Empty.ToString(), String.Empty, ((App)App.Current).groceries);
            GrocerSecondary.SelectedPair = new PairWithList(String.Empty, Guid.Empty.ToString(), String.Empty, ((App)App.Current).groceries);
            GrocerTertiary.SelectedPair = new PairWithList(String.Empty, Guid.Empty.ToString(), String.Empty, ((App)App.Current).groceries);

            Retrieve("1. Login/Register");
        }

        private void Password_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void Password_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void VerifyPassword_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void VerifyPassword_GotFocus(object sender, RoutedEventArgs e)
        {

        }



        public void Retrieve(String name)
        {
            String fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                  <entity name='dc_help'>
                    <attribute name='dc_comments' />
                    <attribute name='dc_helpid' />
                          <filter type='and'>
                            <condition attribute='dc_name' operator='eq' value='@TEXT' />
                          </filter>
                  </entity>
                </fetch>";

            fetchXml = fetchXml.Replace("@TEXT", name);

            XElement orderXml = new XElement("ColumnOrder");
            orderXml.Add(new XElement("Column", "dc_comments"));
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
                   
                    foreach (XElement xe in row.Elements())
                    {
                        if (xe.Name.LocalName.Equals("dc_comments", StringComparison.OrdinalIgnoreCase))
                        {
                            TextBlock.Text = xe.Value;
                        }
                    }
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
            }
        }
    }
}
