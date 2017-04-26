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
using DynamicConnections.NutriStyle.MenuGenerator.Engine.Helpers;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Controls.DataVisualization;
using System.Windows.Controls.DataVisualization.Charting.Primitives;
using System.Xml.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows.Data;

namespace DynamicConnections.NutriStyle.MenuGenerator.Controls
{
    public partial class TabbedMenu : UserControl
    {
        private RowIndexConverter _rowIndexConverter = new RowIndexConverter();

        public Guid contactId { get; set; }
        public String DayNumber { get; set; }
        public Dictionary<String, int> DayList { get; set; }
        Dictionary<Guid, Food> foods;
        
        
        //public static int dayNumber = 948170000;

        public TabbedMenu()
        {
            InitializeComponent();
            foods       = new Dictionary<Guid, Food>();
            contactId   = new Guid(General.ContactId());
            
            MacroChart mc = new MacroChart();

            SundayMenu.MacroChart = mc;
            MondayMenu.MacroChart = mc;
            TuesdayMenu.MacroChart = mc;
            WednesdayMenu.MacroChart = mc;
            ThursdayMenu.MacroChart = mc;
            FridayMenu.MacroChart = mc;
            SaturdayMenu.MacroChart = mc;

            MainPage m = (MainPage)Application.Current.RootVisual;
            m.CollapseTips();
            m.SetMacroChart(mc);

        }
        public TabbedMenu(Guid contactId, MacroChart mc)
        {
            InitializeComponent();
            mc                          = new MacroChart();
            foods                       = new Dictionary<Guid, Food>();
            this.contactId              = contactId;
            SundayMenu.MacroChart       = mc;
            MondayMenu.MacroChart       = mc;
            TuesdayMenu.MacroChart      = mc;
            WednesdayMenu.MacroChart    = mc;
            ThursdayMenu.MacroChart     = mc;
            FridayMenu.MacroChart       = mc;
            SaturdayMenu.MacroChart     = mc;

            MainPage m = (MainPage)Application.Current.RootVisual;
            m.CollapseTips();
            m.SetMacroChart(mc);

        }
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabControl tc = (TabControl)sender;

            TabItem ti = (TabItem)tc.SelectedItem;
            String day = (String)ti.Tag;
            if (day.Equals("Sunday", StringComparison.OrdinalIgnoreCase))
            {
                if (SundayMenu != null)
                {
                    SundayMenu.LoadDay(contactId);
                }
            }
            else if (day.Equals("Monday", StringComparison.OrdinalIgnoreCase))
            {
                MondayMenu.LoadDay(contactId);
            }
            else if (day.Equals("Tuesday", StringComparison.OrdinalIgnoreCase))
            {
                TuesdayMenu.LoadDay(contactId);
            }
            else if (day.Equals("Wednesday", StringComparison.OrdinalIgnoreCase))
            {
                WednesdayMenu.LoadDay(contactId);
            }
            else if (day.Equals("Thursday", StringComparison.OrdinalIgnoreCase))
            {
                ThursdayMenu.LoadDay(contactId);
            }
            else if (day.Equals("Friday", StringComparison.OrdinalIgnoreCase))
            {
                FridayMenu.LoadDay(contactId);
            }
            else if (day.Equals("Saturday", StringComparison.OrdinalIgnoreCase))
            {
                SaturdayMenu.LoadDay(contactId);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //figure out what day it is and load up that tab
            DateTime currentDate = DateTime.Now;
            if (currentDate.DayOfWeek == DayOfWeek.Sunday)
            {
                Sunday.IsSelected = true;
                SundayMenu.LoadDay(contactId);
            }
            else if (currentDate.DayOfWeek == DayOfWeek.Monday)
            {
                Monday.IsSelected = true;
                MondayMenu.LoadDay(contactId);
            }
            else if (currentDate.DayOfWeek == DayOfWeek.Tuesday)
            {
                Tuesday.IsSelected = true;
                TuesdayMenu.LoadDay(contactId);
            }
            else if (currentDate.DayOfWeek == DayOfWeek.Wednesday)
            {
                Wednesday.IsSelected = true;
                WednesdayMenu.LoadDay(contactId);
            }
            else if (currentDate.DayOfWeek == DayOfWeek.Thursday)
            {
                Thursday.IsSelected = true;
                ThursdayMenu.LoadDay(contactId);
            }
            else if (currentDate.DayOfWeek == DayOfWeek.Friday)
            {
                Friday.IsSelected = true;
                FridayMenu.LoadDay(contactId);
            }
            else if (currentDate.DayOfWeek == DayOfWeek.Saturday)
            {
                Saturday.IsSelected = true;
                SaturdayMenu.LoadDay(contactId);
            }
        }

        
    }
}

