﻿using System;
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
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using DynamicConnections.NutriStyle.MenuGenerator.Controls;
using System.Windows.Controls.Primitives;

namespace DynamicConnections.NutriStyle.MenuGenerator.Pages
{
    public partial class Profile : Page
    {


        public Profile()
        {
            InitializeComponent();
            //Setup birthday values
            profileControl.ContactId = Guid.Empty;
            profileControl.ParentContactId = Guid.Empty;
            profileControl.IsPrimary = true;
            profileControl.SavedClicked += new EventHandler(Profile_SavedClicked);
        }

        void Profile_SavedClicked(object sender, EventArgs e)
        {
            XElement element = (XElement)sender;
            //Navigate
            NavigationService.Navigate(new Uri("MenuOptions", UriKind.Relative));
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //Get name value pairs
            if (this.NavigationContext.QueryString.ContainsKey("contactId"))
            {
                profileControl.ContactId = new Guid(this.NavigationContext.QueryString["contactId"]);
            }
            if (this.NavigationContext.QueryString.ContainsKey("contactParentId"))
            {
                profileControl.ParentContactId = new Guid(this.NavigationContext.QueryString["contactParentId"]);
            }
        }
    }
}