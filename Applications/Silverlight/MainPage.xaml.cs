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
using DynamicConnections.NutriStyle.MenuGenerator.Pages;
using DynamicConnections.NutriStyle.MenuGenerator.Engine;
using System.Reflection;
using DynamicConnections.NutriStyle.MenuGenerator.Engine.Helpers;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;
using DynamicConnections.NutriStyle.MenuGenerator.ChildWindows;
using DynamicConnections.NutriStyle.MenuGenerator.Controls;

namespace DynamicConnections.NutriStyle.MenuGenerator
{
    public partial class MainPage : Page
    {
        String currentPage;

        public MainPage()
        {
            currentPage = String.Empty;

            InitializeComponent();
            //htmlHost.SourceHtml = "<iframe src='//www.facebook.com/plugins/like.php?href=http%3A%2F%2Fnutrityle.com&amp;send=false&amp;layout=standard&amp;width=300px&amp;show_faces=true&amp;action=like&amp;colorscheme=light&amp;font&amp;height=80&amp;appId=124442095193' scrolling='no' frameborder='0' style='border:none; text-align:right;overflow:hidden; width:300px; height:25px;' allowTransparency='true'></iframe>";

            Assembly assembly = Assembly.GetExecutingAssembly();
            if (assembly.FullName != null)
            {
                string versionPart = assembly.FullName.Split(',')[1];
                string strVersion = versionPart.Split('=')[1];

                Version.Text = "Version: " + strVersion;
            }
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

        }

        private void Account_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void Profile_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ContentFrame.Source = new Uri("Profile", UriKind.Relative);
        }

        private void MenuPreferences_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //NavigationService.Navigate(new Uri("MenuOptions", UriKind.Relative));
            ContentFrame.Source = new Uri("MenuOptions", UriKind.Relative);
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double space_available = (LayoutRoot.ActualWidth - 18 - 150); //18 is width of scroll bar, 150 is width of menu 
            double heightAvailable = (LayoutRoot.ActualHeight - 74);
            ContentFrame.Height = heightAvailable;

        }

        private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            PopulateHelp ph = new PopulateHelp();
            NavigationService ns = (NavigationService)sender;

            if (!currentPage.Equals(ns.Source.ToString()))
            {

                Image im = new Image();
                im.Height = 205;
                im.Width = 285;
                im.Name = "bottomRight";
                BottomRightSP.Children.RemoveAt(0);
                BottomRightSP.Children.Add(im);
                BottomRightSP.Height = 205;

                if (ns.Source.ToString().Equals("/pages/login.xaml", StringComparison.OrdinalIgnoreCase) && !ns.Source.ToString().Equals(currentPage, StringComparison.OrdinalIgnoreCase))
                {
                    ph.Retrieve("1. Login/Register", helpText, topLeft, bottomLeft, topRight, bottomRight);

                }
                else if (ns.Source.ToString().Equals("profile", StringComparison.OrdinalIgnoreCase) && !ns.Source.ToString().Equals(currentPage, StringComparison.OrdinalIgnoreCase))
                {
                    StackPanalDock.Visibility = System.Windows.Visibility.Visible;
                    ph.Retrieve("2. Profile", helpText, topLeft, bottomLeft, topRight, bottomRight);
                    TreeViewItem item = TreeView.ItemContainerGenerator.ContainerFromItem(TreeView.Items[0]) as TreeViewItem;
                    if (item != null)
                    {
                        item.IsSelected = true;
                    }
                    SwapTextOnTop();
                }
                else if (ns.Source.ToString().Equals("foodlikes", StringComparison.OrdinalIgnoreCase) && !ns.Source.ToString().Equals(currentPage, StringComparison.OrdinalIgnoreCase))
                {
                    ph.Retrieve("4. Food Likes", helpText, topLeft, bottomLeft, topRight, bottomRight);
                    TreeViewItem item = TreeView.ItemContainerGenerator.ContainerFromItem(TreeView.Items[4]) as TreeViewItem;
                    if (item != null)
                    {
                        item.IsSelected = true;
                    }
                    SwapTextOnTop();
                }
                else if (ns.Source.ToString().Equals("fooddislikes", StringComparison.OrdinalIgnoreCase) && !ns.Source.ToString().Equals(currentPage, StringComparison.OrdinalIgnoreCase))
                {
                    ph.Retrieve("5. Food Dislikes", helpText, topLeft, bottomLeft, topRight, bottomRight);
                    TreeViewItem item = TreeView.ItemContainerGenerator.ContainerFromItem(TreeView.Items[6]) as TreeViewItem;
                    if (item != null)
                    {
                        item.IsSelected = true;
                    }
                    SwapTextOnTop();
                }
                else if (ns.Source.ToString().Equals("MenuOptions", StringComparison.OrdinalIgnoreCase) && !ns.Source.ToString().Equals(currentPage, StringComparison.OrdinalIgnoreCase))
                {
                    ph.Retrieve("3. Menu Preferences", helpText, topLeft, bottomLeft, topRight, bottomRight);
                    TreeViewItem item = TreeView.ItemContainerGenerator.ContainerFromItem(TreeView.Items[2]) as TreeViewItem;
                    if (item != null)
                    {
                        item.IsSelected = true;
                    }
                    SwapTextOnTop();
                }
                else if (ns.Source.ToString().Equals("MenuEditor", StringComparison.OrdinalIgnoreCase) && !ns.Source.ToString().Equals(currentPage, StringComparison.OrdinalIgnoreCase))
                {
                    ph.Retrieve("Menu Editor", helpText, topLeft, bottomLeft, topRight, bottomRight);
                    SwapTextOnTop();
                }
                else if (ns.Source.ToString().Equals("ShoppingList", StringComparison.OrdinalIgnoreCase) && !ns.Source.ToString().Equals(currentPage, StringComparison.OrdinalIgnoreCase))
                {
                    ph.Retrieve("8. Shopping List", helpText, topLeft, bottomLeft, topRight, bottomRight);
                    SwapTextOnTop();
                }

                else if (ns.Source.ToString().Equals("GenerateMenu", StringComparison.OrdinalIgnoreCase) && !ns.Source.ToString().Equals(currentPage, StringComparison.OrdinalIgnoreCase))
                {
                    ph.Retrieve("Menu being generated...Please wait...", helpText, topLeft, bottomLeft, topRight, bottomRight);
                }
                else if (ns.Source.ToString().Equals("Menus", StringComparison.OrdinalIgnoreCase) && !ns.Source.ToString().Equals(currentPage, StringComparison.OrdinalIgnoreCase))
                {
                    ph.Retrieve("7. Saved Menus", helpText, topLeft, bottomLeft, topRight, bottomRight);

                    SwapTextOnTop();

                }
                else if (ns.Source.ToString().Equals("fitnesslog", StringComparison.OrdinalIgnoreCase) && !ns.Source.ToString().Equals(currentPage, StringComparison.OrdinalIgnoreCase))
                {
                    
                    ph.Retrieve("9. Fitness Log", helpText, topLeft, bottomLeft, topRight, bottomRight);
                    SwapTextOnTop();
                }
                else if (ns.Source.ToString().Equals("DailyFoodLog", StringComparison.OrdinalIgnoreCase) && !ns.Source.ToString().Equals(currentPage, StringComparison.OrdinalIgnoreCase))
                {
                    ph.Retrieve("9+1 i.e., 10. Food Log", helpText, topLeft, bottomLeft, topRight, bottomRight);

                    //Controls.MacroChart fl = new Controls.MacroChart();
                    
                    //Add Tips and news to BottomRightSP
                    
                    while (BottomRightSP.Children.Count() > 0)
                    {
                        BottomRightSP.Children.RemoveAt(0);
                    }

                    //remove all children
                    /*
                    while (SPTipsAndNews.Children.Count() > 0)
                    {
                        SPTipsAndNews.Children.RemoveAt(0);
                    }*/
                    //Add
                    BottomRightSP.Children.Add(SPTipsAndNewsContainer);
                    BottomRightSP.Margin = new Thickness(0, 10, 0, 0);

                    //Add chart to SPTipsAndNews
                    //SPTipsAndNews.Children.Add(fl);

                    //((App)App.Current).mc = fl;
                }
                else if (ns.Source.ToString().Equals("DailyMenu", StringComparison.OrdinalIgnoreCase) && !ns.Source.ToString().Equals(currentPage, StringComparison.OrdinalIgnoreCase))
                {
                    ph.Retrieve("6. Daily Menu", helpText, topLeft, bottomLeft, topRight, bottomRight);

                    TreeViewItem item = TreeView.ItemContainerGenerator.ContainerFromItem(TreeView.Items[8]) as TreeViewItem;
                    if (item != null)
                    {
                        item.IsSelected = true;
                    }
                    //Controls.MacroChart fl = new Controls.MacroChart();
                    
                    //Add Tips and news to BottomRightSP
                    while (BottomRightSP.Children.Count() > 0)
                    {
                        BottomRightSP.Children.RemoveAt(0);
                    }
                    /*
                    //remove all children
                    while (SPTipsAndNews.Children.Count() > 0)
                    {
                        SPTipsAndNews.Children.RemoveAt(0);
                    }
                    */
                    //Add
                    BottomRightSP.Children.Add(SPTipsAndNewsContainer);
                    BottomRightSP.Margin = new Thickness(0, 10, 0, 0);

                    //Add chart to SPTipsAndNews
                    //SPTipsAndNews.Children.Add(fl);

                    //((App)App.Current).mc = fl;
                }
                else if (ns.Source.ToString().Equals("YourFoods", StringComparison.OrdinalIgnoreCase) && !ns.Source.ToString().Equals(currentPage, StringComparison.OrdinalIgnoreCase))
                {
                    ph.Retrieve("My Foods screen", helpText, topLeft, bottomLeft, topRight, bottomRight);
                    SwapTextOnTop();
                }
                //change images here
                PopulateTip pt = new PopulateTip();
                pt.Retrieve(ref tipScrollViewer);
                //tipText
                currentPage = ns.Source.ToString();
            }
        }
        /// <summary>
        /// Set MacroChart
        /// </summary>
        /// <param name="mc"></param>
        public void SetMacroChart(MacroChart mc) {
            SPTipsAndNews.Children.Clear();
            SPTipsAndNews.Children.Add(mc);
        }
        private void SwapTextOnTop()
        {
            //Add Tips and news to BottomRightSP
            while (BottomRightSP.Children.Count() > 0)
            {
                BottomRightSP.Children.RemoveAt(0);
            }

            //remove all children
            while (SPTipsAndNews.Children.Count() > 0)
            {
                SPTipsAndNews.Children.RemoveAt(0);
            }
            //Add
            SPTipsAndNews.Children.Add(SPTipsAndNewsContainer);
            BottomRightSP.Margin = new Thickness(0, 100, 0, 0);
            Image image = new Image();
            //Width="285" Height="205"
            image.Width = 285;
            image.Height = 205;

            BottomRightSP.Children.Add(image);
            //Expand Tips 
            ExpandTipsFrame();

        }
        private void ContentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {

        }

        private void foodDisLikes_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ContentFrame.Source = new Uri("FoodDislikes", UriKind.Relative);
        }

        private void foodLikes_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ContentFrame.Source = new Uri("FoodLikes", UriKind.Relative);
        }

        private void menuEditor_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ContentFrame.Source = new Uri("MenuEditor", UriKind.Relative);
        }

        private void ShoppingList_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ContentFrame.Source = new Uri("ShoppingList", UriKind.Relative);
        }
        private void GenerateMenu_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ContentFrame.Source = new Uri("GenerateMenu", UriKind.Relative);
        }

        private void WeeklyMenu_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ContentFrame.Source = new Uri("Menus", UriKind.Relative);
        }

        private void FitnessLog_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ContentFrame.Source = new Uri("FitnessLog", UriKind.Relative);
        }

        private void FoodLog_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ContentFrame.Source = new Uri("DailyFoodLog", UriKind.Relative);
        }

        private void DailyMenu_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ContentFrame.Source = new Uri("DailyMenu", UriKind.Relative);
        }

        private void Account_Selected(object sender, RoutedEventArgs e)
        {

        }
        private void Additional_Profiles_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ContentFrame.Source = new Uri("AdditionalProfiles", UriKind.Relative);
        }

        private void Profile_Selected(object sender, RoutedEventArgs e)
        {
            TreeViewItem tvi = (TreeViewItem)sender;
            tvi.FontWeight = FontWeights.Bold;
            tvi.Foreground = new SolidColorBrush(Colors.Black);
        }
        
        private void Profile_Unselected(object sender, RoutedEventArgs e)
        {
            TreeViewItem tvi = (TreeViewItem)sender;
            tvi.Foreground = General.GetColorFromHexa("#FF3395B9");
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (((ToggleButton)sender).IsChecked.Value)
            {
                ExpandAbout.Source = new BitmapImage(new Uri("/DynamicConnections.NutriStyle.MenuGenerator;component/images/minus.png", UriKind.Relative));
                //AboutBorder.Height = 200d;
                da.To = 300d;
                borderStoryBoard.Begin();
            }
            else
            {
                ExpandAbout.Source = new BitmapImage(new Uri("/DynamicConnections.NutriStyle.MenuGenerator;component/images/plus.png", UriKind.Relative));
                //AboutBorder.Height = 100d;
                da.To = 90d;
                borderStoryBoard.Begin();
            }
        }

        private void ToggleButton_Click2(object sender, RoutedEventArgs e)
        {
            if (((ToggleButton)sender).IsChecked.Value)
            {
                ExpandTips.Source = new BitmapImage(new Uri("/DynamicConnections.NutriStyle.MenuGenerator;component/images/minus.png", UriKind.Relative));
                //AboutBorder.Height = 200d;
                tipsda.To = 300d;
                tipsStoryBoard.Begin();
            }
            else
            {
                ExpandTips.Source = new BitmapImage(new Uri("/DynamicConnections.NutriStyle.MenuGenerator;component/images/plus.png", UriKind.Relative));
                //AboutBorder.Height = 100d;
                tipsda.To = 95;
                tipsStoryBoard.Begin();
            }
        }
        public void CollapseTips()
        {
            
            ExpandTips.Source = new BitmapImage(new Uri("/DynamicConnections.NutriStyle.MenuGenerator;component/images/plus.png", UriKind.Relative));
            //AboutBorder.Height = 200d;
            tipsda.To = 95d;

            try
            {
                tipsStoryBoard.Begin();
            }
            catch (Exception)//Getting occasional error about inability to find TipsBorder
            {
                TipsBorder.Height = 95d;
            }

        }
        public void ExpandTipsFrame()
        {
            ExpandTips.Source = new BitmapImage(new Uri("/DynamicConnections.NutriStyle.MenuGenerator;component/images/minus.png", UriKind.Relative));
            //AboutBorder.Height = 200d;
            tipsda.To = 300d;
            try
            {
                tipsStoryBoard.Begin();
            }
            catch (Exception)//Getting occasional error about inability to find TipsBorder
            {
                TipsBorder.Height = 300d;
            }
        }

        private void Feedback_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DynamicConnections.NutriStyle.MenuGenerator.ChildWindows.FeedBack fb = new DynamicConnections.NutriStyle.MenuGenerator.ChildWindows.FeedBack();
            fb.Show();
        }

        

        private void AddAFood_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ContentFrame.Source = new Uri("AddAFood", UriKind.Relative);
        }

        private void AddARecipe_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ContentFrame.Source = new Uri("AddARecipe", UriKind.Relative);
        }

        private void YourFoods_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ContentFrame.Source = new Uri("YourFoods", UriKind.Relative);
        }
    }
}
