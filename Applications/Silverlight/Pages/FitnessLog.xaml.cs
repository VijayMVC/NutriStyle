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
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Controls.DataVisualization.Charting;
using System.Text;
using System.Windows.Markup;
using System.Windows.Controls.Primitives;
using System.Reflection;
using System.Collections;

namespace DynamicConnections.NutriStyle.MenuGenerator.Pages
{
    public partial class FitnessLog : Page
    {
        private List<CalendarDayButton> calendarButtons = new List<CalendarDayButton>();

        private RowIndexConverter _rowIndexConverter = new RowIndexConverter();


        public event EventHandler<CalendarEventArgs> EventClick;
        

        public FitnessLog()
        {
            InitializeComponent();
            _itemsSourceDictionary = new Dictionary<DateTime, List<object>>();
           
            
            //Build list of calendar events
            RetrieveDays();

        }
        private void RetrieveDays()
        {
            
                XElement orderXml = null;
                App app = (App)App.Current;
                DateTime currentMonth = new DateTime(calendar.DisplayDate.Year, calendar.DisplayDate.Month, 1);
                
                String fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' aggregate='true'>
                  <entity name='dc_fitnesslog'>
                    <attribute name='dc_calories' alias='dc_calories' aggregate='sum'/> 
                    <attribute name='dc_date' groupby='true' alias='dc_date' dategrouping='day' />
                    <filter type='and'>
                        <condition attribute='dc_contactid' value='@CONTACTID' operator='ge'/>
                        <condition attribute='dc_date' value='@STARTDATE' operator='ge'/>
                        <condition attribute='dc_date' value='@ENDDATE' operator='le'/>
                    </filter>
                  </entity>
                </fetch>";

               
                fetchXml = fetchXml.Replace("@CONTACTID", app.contactId);
                fetchXml = fetchXml.Replace("@STARTDATE", currentMonth.AddDays(-1).ToString("MM/dd/yyyy"));
                fetchXml = fetchXml.Replace("@ENDDATE", currentMonth.AddMonths(1).AddDays(1).ToString("MM/dd/yyyy"));

                orderXml = new XElement("ColumnOrder");
                orderXml.Add(new XElement("Column", "dc_calories"));
                orderXml.Add(new XElement("Column", "dc_date"));
                

                try
                {
                    CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
                    cms.RetrieveFetchXmlCompleted += new EventHandler<CrmSdk.RetrieveFetchXmlCompletedEventArgs>(cms_RetrieveDays);
                    cms.RetrieveFetchXmlAsync(fetchXml, orderXml.ToString());
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                    MessageBox.Show(err.StackTrace);
                }
            
        }
        private void cms_RetrieveDays(object sender, CrmSdk.RetrieveFetchXmlCompletedEventArgs e)
        {
           
            
            try
            {
                String entityName = "dc_fitnesslog";
                
                var element = e.Result;

                List<Pair> dataCollection = new List<Pair>();
                try
                {
                    if (element.Descendants(entityName).Count() > 0)
                    {
                        //Bind data
                        var rows = element.Descendants(entityName);
                        foreach (var row in rows)
                        {
                            List<object> list = new List<object>();
                            Row rowData = new Row();

                            String kcals = String.Empty;
                            int dayNumber = 1;

                            foreach (XElement xe in row.Elements())
                            {
                                if (xe.Name.LocalName.Equals("dc_calories", StringComparison.OrdinalIgnoreCase))
                                {
                                    kcals = xe.Value;
                                }else  if (xe.Name.LocalName.Equals("dc_date", StringComparison.OrdinalIgnoreCase))
                                {
                                    dayNumber = Convert.ToInt32(xe.Value);
                                }
                            }

                            //border container 
                            Border b = new Border();
                            b.CornerRadius = new CornerRadius(4d);
                            b.Height = 30d;
                            //Stack panel for content
                            StackPanel sp = new StackPanel();
                            sp.Height = 30d;
                            b.Child = sp;

                            TextBlock tb = new TextBlock();//for content
                            tb.Height = 30d;
                            tb.Text = "Kcals: "+kcals;
                            sp.Children.Add(tb);//add to stack panel

                            
                            list.Add(b);
                            ItemsSourceDictionary.Add(new DateTime(calendar.DisplayDate.Year, calendar.DisplayDate.Month, dayNumber), list);
                        }
                        FillCalendar();
                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                    MessageBox.Show(err.StackTrace);
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
            }
        }

        /// <summary>
        /// Calendar day clicked on
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CalendarDayButton_Click(object sender, EventArgs e)
        {
            CalendarDayButton cdb = (CalendarDayButton)sender;

            //Pop window for data
            DateTime day = (DateTime)cdb.DataContext;
            EditFitnessLog efl = new EditFitnessLog(day);
            efl.Show();
        }



        //---------------------------------------------
        //Calendar stuff
        //---------------------------------------------
        /// <summary>
        /// Clear all the items in the Calendar
        /// </summary>
        public void ClearCallendar()
        {
            foreach (CalendarDayButton button in calendarButtons)
            {
                var panel = button.Parent as StackPanel;
                int nbControls = panel.Children.Count;

                for (int i = nbControls - 1; i > 0; i--)
                {
                    panel.Children.RemoveAt(i);
                }
            }
        }

        public static void ItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var owner = d as FitnessLog;
            //if the property was set to null we have to clear all the events from calendar
            if (e.NewValue == null)
            {
                owner.ClearCallendar();
                return;
            }

            IEnumerable rawItems = (IEnumerable)e.NewValue;
            PropertyInfo property = null;

            //to determine the if the Count>0
            var enumerator = rawItems.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                owner.ClearCallendar();
                return;
            }

            Object o = enumerator.Current;
            Type type = o.GetType();

            //get the type of the properties inside of the IEnumerable
            property = type.GetProperty(owner.DatePropertyName);

            if (property != null)
            {
                IEnumerable<Object> items = Enumerable.Cast<Object>((IEnumerable)e.NewValue);
                //group the items and store in a dictionary
                if (items != null)
                {
                    var parDate = items
                                .GroupBy(x => GetDateValue(x, property))
                                .ToDictionary(x => x.Key, x => x.ToList());
                    owner.ItemsSourceDictionary = parDate;
                    owner.FillCalendar();
                }
            }
        }

        //Returns the DateTime value of a property specified by its information
        public static DateTime GetDateValue(Object x, PropertyInfo property)
        {
            return ((DateTime)property.GetValue(x, null)).Date;
        }


        public static readonly DependencyProperty SelectedEventProperty = DependencyProperty.Register("SelectedEvent", typeof(Object), typeof(FitnessLog), null);
        public Object SelectedEvent
        {
            get { return (Object)GetValue(SelectedEventProperty); }
            set { SetValue(SelectedEventProperty, value); }
        }

        public static readonly DependencyProperty CalendarEventButtonStyleProperty = DependencyProperty.Register("CalendarEventButtonStyle", typeof(Style), typeof(FitnessLog), null);
        public Style CalendarEventButtonStyle
        {
            get { return (Style)GetValue(CalendarEventButtonStyleProperty); }
            set { SetValue(CalendarEventButtonStyleProperty, value); }
        }

        public static readonly DependencyProperty DatePropertyNameProperty = DependencyProperty.Register("DatePropertyName", typeof(String), typeof(FitnessLog), null);
        public String DatePropertyName
        {
            get { return (String)GetValue(DatePropertyNameProperty); }
            set { SetValue(DatePropertyNameProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(FitnessLog),
            new PropertyMetadata(ItemsSourcePropertyChanged));

        private void CalendarDayButton_Loaded(object sender, RoutedEventArgs e)
        {
            var button = sender as CalendarDayButton;
            calendarButtons.Add(button);

            //Resizing the buttons is the only way to change the dimensions of the calendar
            button.Width = this.ActualWidth / 8.5;
            button.Height = this.ActualHeight / 9;

            if (calendarButtons.Count == 42)
            {
                FillCalendar();
            }
        }
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        private Dictionary<DateTime, List<Object>> _itemsSourceDictionary;
        public Dictionary<DateTime, List<Object>> ItemsSourceDictionary
        {
            get
            {
                return _itemsSourceDictionary;
            }

            set
            {
                _itemsSourceDictionary = value;
            }
        }

        private DateTime GetFirstCalendarDate()
        {
            return new DateTime(calendar.DisplayDate.Year, calendar.DisplayDate.Month, 1);
        }

        private void FillCalendar()
        {
            FillCalendar(GetFirstCalendarDate());
        }

        private void FillCalendar(DateTime firstDate)
        {
            if (ItemsSourceDictionary != null && ItemsSourceDictionary.Count > 0)
            {
                DateTime currentDay;

                int weekDay = (int)firstDate.DayOfWeek;
                if (weekDay == 0) weekDay = 7;
                if (weekDay == 1) weekDay = 8;

                for (int counter = 0; counter < calendarButtons.Count; counter++)
                {
                    var button = calendarButtons[counter];
                    var panel = button.Parent as StackPanel;


                    int nbControls = panel.Children.Count;
                    for (int i = nbControls - 1; i > 0; i--)
                    {
                        panel.Children.RemoveAt(i);
                    }

                    currentDay = firstDate.AddDays(counter).AddDays(-weekDay);

                    if (ItemsSourceDictionary.ContainsKey(currentDay))
                    {
                        var events = ItemsSourceDictionary[currentDay];
                        foreach (Object calendarEvent in events)
                        {
                            Button btn = new Button();
                            btn.Height = 30d;
                            btn.Content = calendarEvent;
                            btn.Style = CalendarEventButtonStyle;
                            panel.Children.Add(btn);
                            btn.Click += new RoutedEventHandler(EventButton_Click);
                        }
                    }
                }
            }
        }
        void EventButton_Click(object sender, RoutedEventArgs e)
        {
            object eventClicked = (sender as Button).DataContext as object;

            //set the selected event
            SelectedEvent = eventClicked;

            //just pass the click event to the hosting envirenment of the component
            if (EventClick != null)
            {
                EventClick(sender, new CalendarEventArgs(eventClicked));
            }
        }
    }
}
