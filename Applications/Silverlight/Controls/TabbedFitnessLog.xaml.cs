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
    public partial class TabbedFitnessLog : UserControl
    {
        private RowIndexConverter _rowIndexConverter = new RowIndexConverter();

        public Guid ContactId { get; set; }
        public String DayNumber { get; set; }
        public Dictionary<String, int> DayList { get; set; }
        Dictionary<Guid, Food> foods;


        DateTime date;

        public TabbedFitnessLog()
        {
            date = DateTime.Now;
            int dayOfWeek = (int)date.DayOfWeek;
            date = date.AddDays(-dayOfWeek);

            InitializeComponent();
            foods = new Dictionary<Guid, Food>();
            if (ContactId == Guid.Empty)
            {
                ContactId = new Guid(General.ContactId());
            }
            SundayMenu.ContactId    = ContactId;
            MondayMenu.ContactId    = ContactId;
            TuesdayMenu.ContactId   = ContactId;
            WednesdayMenu.ContactId = ContactId;
            ThursdayMenu.ContactId  = ContactId;
            FridayMenu.ContactId    = ContactId;
            SaturdayMenu.ContactId  = ContactId;
        }

        public TabbedFitnessLog(Guid contactId) : base()
        {
            date = DateTime.Now;
            int dayOfWeek = (int)date.DayOfWeek;
            date = date.AddDays(-dayOfWeek);

            InitializeComponent();
            foods = new Dictionary<Guid, Food>();
            ContactId = contactId;
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
                    SundayMenu.LoadDay(date, ContactId);
                }
            }
            else if (day.Equals("Monday", StringComparison.OrdinalIgnoreCase))
            {
                MondayMenu.LoadDay(date.AddDays(1), ContactId);
            }
            else if (day.Equals("Tuesday", StringComparison.OrdinalIgnoreCase))
            {
                TuesdayMenu.LoadDay(date.AddDays(2), ContactId);
            }
            else if (day.Equals("Wednesday", StringComparison.OrdinalIgnoreCase))
            {
                WednesdayMenu.LoadDay(date.AddDays(3), ContactId);
            }
            else if (day.Equals("Thursday", StringComparison.OrdinalIgnoreCase))
            {
                ThursdayMenu.LoadDay(date.AddDays(4), ContactId);
            }
            else if (day.Equals("Friday", StringComparison.OrdinalIgnoreCase))
            {
                FridayMenu.LoadDay(date.AddDays(5), ContactId);
            }
            else if (day.Equals("Saturday", StringComparison.OrdinalIgnoreCase))
            {
                SaturdayMenu.LoadDay(date.AddDays(6), ContactId);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //figure out what day it is and load up that tab
            DateTime currentDate = DateTime.Now;
            if (currentDate.DayOfWeek == DayOfWeek.Sunday)
            {
                Sunday.IsSelected = true;
                SundayMenu.LoadDay(date.AddDays(0), ContactId);
            }
            else if (currentDate.DayOfWeek == DayOfWeek.Monday)
            {
                Monday.IsSelected = true;
                MondayMenu.LoadDay(date.AddDays(1), ContactId);
            }
            else if (currentDate.DayOfWeek == DayOfWeek.Tuesday)
            {
                Tuesday.IsSelected = true;
                TuesdayMenu.LoadDay(date.AddDays(2), ContactId);
            }
            else if (currentDate.DayOfWeek == DayOfWeek.Wednesday)
            {
                Wednesday.IsSelected = true;
                WednesdayMenu.LoadDay(date.AddDays(3), ContactId);
            }
            else if (currentDate.DayOfWeek == DayOfWeek.Thursday)
            {
                Thursday.IsSelected = true;
                ThursdayMenu.LoadDay(date.AddDays(4), ContactId);
            }
            else if (currentDate.DayOfWeek == DayOfWeek.Friday)
            {
                Friday.IsSelected = true;
                FridayMenu.LoadDay(date.AddDays(5), ContactId);
            }
            else if (currentDate.DayOfWeek == DayOfWeek.Saturday)
            {
                Saturday.IsSelected = true;
                SaturdayMenu.LoadDay(date.AddDays(6), ContactId);
            }

            TabControl.SelectionChanged += new SelectionChangedEventHandler(TabControl_SelectionChanged);
        }

        
    }
}

