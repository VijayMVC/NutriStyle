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
using System.Windows.Media.Imaging;
using DynamicConnections.NutriStyle.MenuGenerator.Engine.Helpers;
using System.Windows.Navigation;

namespace DynamicConnections.NutriStyle.MenuGenerator.ChildWindows
{
    public partial class EditProfile : ChildWindow
    {

        public event EventHandler SavedClicked;

        Guid contactId = Guid.Empty;
        Guid parentContactId = Guid.Empty;

        public EditProfile()
        {
            InitializeComponent();
        }
        public EditProfile(Guid contactId)
        {
            InitializeComponent();
        }
        public EditProfile(Guid contactId, Guid parentContactId)
        {
            InitializeComponent();
            
            this.contactId          = contactId;
            this.parentContactId    = parentContactId;

            Controls.Profile profile = new Controls.Profile();
            profile.ContactId = contactId;
            profile.ParentContactId = parentContactId;
            profile.SavedClicked += profile_SavedClicked;
            //contentBorder.Child.Clear();
            contentBorder.Child = profile;
            if (contactId != Guid.Empty)
            {
                Enable();
            }
        }
        /// <summary>
        /// Close the window on sucessful save
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void profile_SavedClicked(object sender, EventArgs e)
        {
            SavedClicked(sender, new EventArgs());
            Enable();
            //this.DialogResult = true;
        }

        void Close_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
        /// <summary>
        /// Save Contact
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            //profile.SaveContact();
        }

        private void Profile_Selected(object sender, RoutedEventArgs e)
        {
            TreeViewItem tvi    = (TreeViewItem)sender;
            tvi.FontWeight      = FontWeights.Bold;
            tvi.Foreground      = new SolidColorBrush(Colors.Black);
        }

        private void Profile_Unselected(object sender, RoutedEventArgs e)
        {
            TreeViewItem tvi    = (TreeViewItem)sender;
            tvi.Foreground      = General.GetColorFromHexa("#FF3395B9");
        }

        private void FitnessLog_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            CleanupNavigation();
            Controls.TabbedFitnessLog tfl = new Controls.TabbedFitnessLog(contactId);
            contentBorder.Child = tfl;
        }

        private void FoodLog_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Controls.MacroChart mc = new Controls.MacroChart(contactId);
            Controls.TabbedFoodLog foodLog = new Controls.TabbedFoodLog(contactId, mc);

            //clear
            CleanupNavigation();
            //populate
            contentBorder.Child = foodLog;
            //contentBorder.Margin = new Thickness(0, 30, 0, 0);
            leftPanel.Children.Add(mc);
        }

        private void DailyMenu_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //ContentFrame.Source = new Uri("DailyMenu", UriKind.Relative);

            Controls.MacroChart mc          = new Controls.MacroChart(contactId);
            Controls.TabbedMenu dailyMenu   = new Controls.TabbedMenu(contactId, mc);
            
            //clear
            CleanupNavigation();
            //populate
            contentBorder.Child = dailyMenu;
            leftPanel.Children.Add(mc);

        }
        private void  CleanupNavigation() 
        {
            //contentBorder.Child = .Clear();
            leftPanel.Children.Clear();
            contentBorder.Margin = new Thickness(0, 0, 0, 0);
        }
        private void ShoppingList_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Controls.ShoppingList sl = new Controls.ShoppingList(contactId, Guid.Empty);
            CleanupNavigation();
            contentBorder.Child = sl;

        }

        private void Profile_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //Not able to get urimapping to work with name/value pairs.  Moving to a contianer approach.
            //ContentFrame.Source = new Uri("Profile", UriKind.Relative);
            //ContentFrame.Navigate
            //Uri uri = new Uri("Profile", UriKind.Relative);
            //Uri uri = new Uri("Profile/{" + contactId.ToString() + "}/{" + parentContactId.ToString()+"}", UriKind.Relative);
            Uri uri = new Uri("Profile/" + contactId.ToString() + "/" + parentContactId.ToString(), UriKind.Relative);
            //Uri uri = new Uri("/Pages/Profile.xaml?contactId=" + contactId + "&parentContactId=" + parentContactId, UriKind.Absolute);
            //ContentFrame.Navigate(uri);
            //ContentFrame.Source = uri;

            Controls.Profile profile = new Controls.Profile();
            profile.ContactId = contactId;
            profile.ParentContactId = parentContactId;

            CleanupNavigation();
            contentBorder.Child = profile;
        }

        private void ContentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {

        }
        private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
        }
        /// <summary>
        /// Enable the additional menu items
        /// </summary>
        public void Enable()
        {
            DailyMenu.IsEnabled     = true;
            ShoppingList.IsEnabled  = true;
            FoodLog.IsEnabled       = true;
            FitnessLog.IsEnabled    = true;
        }
    }
}

