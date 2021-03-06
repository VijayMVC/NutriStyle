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
using System.Globalization;
using System.Text.RegularExpressions;
using DynamicConnections.NutriStyle.MenuGenerator.ChildWindows;
using DynamicConnections.NutriStyle.MenuGenerator.Pages;

namespace DynamicConnections.NutriStyle.MenuGenerator.Controls
{
    public partial class FitnessLog : UserControl
    {
        int rowCount;
        SortableCollectionView data;
        private RowIndexConverter _rowIndexConverter = new RowIndexConverter();
        bool isLoaded = false;
        Row selectedRow;
        DateTime day;
        Dictionary<Guid, PhysicalActivity> activityList;
        Guid fitnessLogDayId;

        decimal kcalBaseLevel = 0m;
        public Guid ContactId {get; set;}


        public FitnessLog()
        {
            InitializeComponent();
            //get to sunday
            /*
            DateTime currentDate = DateTime.Now;
            int dayOfWeek = (int)currentDate.DayOfWeek;
            var dtf = CultureInfo.CurrentCulture.DateTimeFormat;
            currentDate = currentDate.AddDays(-dayOfWeek);

            fitnessChartComplete.ContactId = ContactId;
            fitnessChartComplete.RetrieveFitnessLog(currentDate, ContactId);
            fitnessChartComplete.RetrieveFoodLog(currentDate, ContactId);
            */
            kcalBaseLevel = ((App)App.Current).kcalCalculatedTarget;
            dataGrid1.LoadingRow += new EventHandler<DataGridRowEventArgs>(dataGrid1_LoadingRow);
            activityList = new Dictionary<Guid, PhysicalActivity>();
            fitnessLogDayId = Guid.Empty;

            MainPage m = (MainPage)Application.Current.RootVisual;
            m.CollapseTips();

            
        }
        public FitnessLog(DateTime day) : base()
        {
            InitializeComponent();
            
            /*
            kcalBaseLevel = ((App)App.Current).kcalCalculatedTarget;
            rowCount = 0;
            this.day = day;
            activityList = new Dictionary<Guid, PhysicalActivity>();
            dataGrid1.LoadingRow += new EventHandler<DataGridRowEventArgs>(dataGrid1_LoadingRow);
           
            fitnessLogDayId = Guid.Empty;

            MainPage m = (MainPage)Application.Current.RootVisual;
            m.CollapseTips();
            */
            this.day = day;
        }

        void dataGrid1_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            DataGrid dataGrid1 = (DataGrid)sender;
            rowCount++;
            if (sender != null)
            {
                DataGrid dg = (DataGrid)sender;
                //deal with getting the data bound to the autocompletebox
                try
                {
                    foreach (DataGridColumn c in dg.Columns)
                    {
                        //MessageBox.Show(""+c.GetCellContent(e.Row).GetType());
                        if (c.GetCellContent(e.Row).GetType() == typeof(Button))
                        {
                            Button ccb = c.GetCellContent(e.Row) as Button;
                            //ToolTipService.SetToolTip(ccb, "Remove From Fitness log");

                            ccb.Click -= ccb_MouseLeftButtonUp;
                            ccb.Click += new RoutedEventHandler(ccb_MouseLeftButtonUp);//new MouseButtonEventHandler(ccb_MouseLeftButtonUp);
                        }
                        else if (c.GetCellContent(e.Row).GetType() == typeof(Controls.CustomTextBox))
                        {
                            CustomTextBox ctb = c.GetCellContent(e.Row) as Controls.CustomTextBox;
                            if (ctb.TagName.Equals("dc_durationminutes", StringComparison.OrdinalIgnoreCase))
                            {
                                Controls.CustomTextBox tb = (Controls.CustomTextBox)c.GetCellContent(e.Row);
                                tb.TextBox.SelectionChanged -= tb_SelectionChanged;
                                tb.TextBox.SelectionChanged += new RoutedEventHandler(tb_SelectionChanged);
                            }
                        }
                        else if (c.GetCellContent(e.Row).GetType() == typeof(Controls.ComboBoxWithValidation))
                        {
                            Controls.ComboBoxWithValidation ccb = c.GetCellContent(e.Row) as Controls.ComboBoxWithValidation;

                            if (ccb.TagName.Equals("dc_physicalactivitycategoryid", StringComparison.OrdinalIgnoreCase))
                            {
                                if (App.Current.GetType() == typeof(DynamicConnections.NutriStyle.MenuGenerator.App))
                                {
                                    App app = (App)App.Current;
                                    
                                    ccb.SelectionChanged -= Category_SelectionChanged;
                                    ccb.SelectionChanged += new EventHandler<SelectionChangedEventArgs>(ccb_SelectionChanged); //new SelectionChangedEventHandler(Category_SelectionChanged); //new RoutedEventHandler(Category_SelectionChanged); //new SelectionChangedEventHandler(Category_SelectionChanged);
                                }
                            }
                            if (ccb.TagName.Equals("dc_physicalactivityid", StringComparison.OrdinalIgnoreCase))
                            {
                                ccb.SelectionChanged -= PhysicalActivity_SelectionChanged;
                                ccb.SelectionChanged += new EventHandler<SelectionChangedEventArgs>(PhysicalActivity_SelectionChanged); //new SelectionChangedEventHandler(PhysicalActivity_SelectionChanged); //PhysicalActivity_SelectionChanged;
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
        /// <summary>
        /// Populate activites box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ccb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Make sure something is selected on the row
            PairWithList pair = (PairWithList)((AutoCompleteBox)sender).SelectedItem;
            String rowId = String.Empty;

            FrameworkElement parent = (FrameworkElement)sender;
            if (pair != null && new Guid(pair.Id) != Guid.Empty)
            {
                String Id = pair.Id;
                Row r = null;
                //TODO: Move away.  Get row from data context
                while (true)
                {
                    parent = (FrameworkElement)VisualTreeHelper.GetParent(parent);
                    if (parent == null)
                    {
                        break;
                    }
                    else if (parent is ComboBoxWithValidation)
                    {
                        break;
                    }
                }

                r = (Row)((ComboBoxWithValidation)parent).DataContext;

                if (r != null)
                {
                    rowId = (String)r["dc_fitnesslogid"];
                    if (!String.IsNullOrEmpty(rowId))
                    {
                        //Retreive 
                        String fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='dc_physicalactivity'>
                                <attribute name='dc_name' />
                                <attribute name='dc_mets' />
                                <attribute name='dc_physicalactivityid' />
                                <order attribute='dc_name' descending='false' />
                                <filter type='and'>
                                    <condition attribute='dc_physicalactivitycategoryid' value='@ID' operator='eq'/>
                                </filter>     
                              </entity>
                            </fetch>";

                        fetchXml = fetchXml.Replace("@ID", Id);

                        XElement orderXml = new XElement("ColumnOrder");
                        orderXml.Add(new XElement("Column", "dc_name"));
                        orderXml.Add(new XElement("Column", "dc_mets"));
                        orderXml.Add(new XElement("Column", "dc_physicalactivityid"));
                        orderXml.Add(new XElement("Column", "dc_foodsid"));

                        try
                        {
                            CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
                            cms.RetrieveFetchXmlRowIdCompleted += new EventHandler<CrmSdk.RetrieveFetchXmlRowIdCompletedEventArgs>(cms_RetrievePhysicalActivities);
                            cms.RetrieveFetchXmlRowIdAsync(fetchXml, orderXml.ToString(), rowId);
                        }
                        catch (Exception err)
                        {
                            MessageBox.Show(err.Message);
                            MessageBox.Show(err.StackTrace);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Fired when the category changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Category_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Make sure something is selected on the row
            PairWithList pair = (PairWithList)((AutoCompleteBox)sender).SelectedItem;
            String rowId = String.Empty;

            FrameworkElement parent = (FrameworkElement)sender;
            if (pair != null && new Guid(pair.Id) != Guid.Empty)
            {
                String Id = pair.Id;
                Row r = null;
                //TODO: Move away.  Get row from data context
                while (true)
                {
                    parent = (FrameworkElement)VisualTreeHelper.GetParent(parent);
                    if (parent == null)
                    {
                        break;
                    }
                    else if (parent is ComboBoxWithList)
                    {
                        break;
                    }
                }

                r = (Row)((ComboBoxWithList)parent).DataContext;

                if (r != null)
                {
                    rowId = (String)r["dc_fitnesslogid"];
                    if (!String.IsNullOrEmpty(rowId))
                    {
                        //Retreive 
                        String fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='dc_physicalactivity'>
                                <attribute name='dc_name' />
                                <attribute name='dc_mets' />
                                <attribute name='dc_physicalactivityid' />
                                <order attribute='dc_name' descending='false' />
                                <filter type='and'>
                                    <condition attribute='dc_physicalactivitycategoryid' value='@ID' operator='eq'/>
                                </filter>     
                              </entity>
                            </fetch>";

                        fetchXml = fetchXml.Replace("@ID", Id);

                        XElement orderXml = new XElement("ColumnOrder");
                        orderXml.Add(new XElement("Column", "dc_name"));
                        orderXml.Add(new XElement("Column", "dc_mets"));
                        orderXml.Add(new XElement("Column", "dc_physicalactivityid"));
                        orderXml.Add(new XElement("Column", "dc_foodsid"));

                        try
                        {
                            CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
                            cms.RetrieveFetchXmlRowIdCompleted += new EventHandler<CrmSdk.RetrieveFetchXmlRowIdCompletedEventArgs>(cms_RetrievePhysicalActivities);
                            cms.RetrieveFetchXmlRowIdAsync(fetchXml, orderXml.ToString(), rowId);
                        }
                        catch (Exception err)
                        {
                            MessageBox.Show(err.Message);
                            MessageBox.Show(err.StackTrace);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Duration change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tb_SelectionChanged(object sender, RoutedEventArgs e)
        {
            App app = (App)App.Current;
            TextBox tb = (TextBox)sender;
            //Get the underlying data
            Row r = (Row)tb.DataContext;
            
            CalculateFooter();
        }
        /// <summary>
        /// Remove item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ccb_MouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
            try
            {
                SortableCollectionView data = (SortableCollectionView)dataGrid1.ItemsSource;

                Button img = sender as Button;
                String Id = (String)((Row)dataGrid1.SelectedItem)["dc_fitnesslogid"];
                Row r = (Row)dataGrid1.SelectedItem;
                if (!String.IsNullOrEmpty(Id))
                {
                    XElement deleteXml = new XElement("dc_fitnesslog", new XElement("id", Id));

                    CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
                    cms.DeleteCompleted += new EventHandler<CrmSdk.DeleteCompletedEventArgs>(cms_Delete);
                    cms.DeleteAsync(deleteXml);
                }
                data.Remove(r);
                dataGrid1.ItemsSource = data;
                CalculateFooter();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
            }
        }
        private void cms_Delete(object sender, CrmSdk.DeleteCompletedEventArgs e)
        {
            //CrmIndicator.IsBusy = false;
            //place holder
        }

        private void cms_RetrievePhysicalActivities(object sender, CrmSdk.RetrieveFetchXmlRowIdCompletedEventArgs e)
        {

            String entityName = "dc_physicalactivity";
            XElement element = e.Result;

            List<PairWithList> dataCollection = new List<PairWithList>();
            try
            {
                if (element.Descendants(entityName).Count() > 0)
                {
                    //Bind data
                    var rows = element.Descendants(entityName);
                    foreach (var row in rows)
                    {
                        Row rowData = new Row();

                        String name = String.Empty;
                        String Id = String.Empty;
                        String mets = String.Empty;
                        foreach (XElement xe in row.Elements())
                        {
                            if (xe.Name.LocalName.Equals("dc_name", StringComparison.OrdinalIgnoreCase))
                            {
                                name = xe.Value;
                            }
                            else if (xe.Name.LocalName.Equals(entityName + "id", StringComparison.OrdinalIgnoreCase))
                            {
                                Id = xe.Value;
                            }
                            else if (xe.Name.LocalName.Equals("dc_mets", StringComparison.OrdinalIgnoreCase))
                            {
                                mets = xe.Value;
                            }
                        }
                        dataCollection.Add(new PairWithList(name, Id));
                        PhysicalActivity ph = new PhysicalActivity()
                        {
                            Name = name,
                            Id = new Guid(Id),
                            Mets = Convert.ToDecimal(mets)
                        };
                        if (!activityList.ContainsKey(new Guid(Id)))
                        {
                            activityList.Add(new Guid(Id), ph);
                        }
                    }
                    String rowId = element.Attribute("rowId").Value;
                    //Find the ComboBox with the correct row
                    SortableCollectionView data = (SortableCollectionView)dataGrid1.ItemsSource;
                    foreach (Row r in data)
                    {
                        if (new Guid((String)r["dc_fitnesslogid"]) == new Guid(rowId))
                        {
                            PairWithList p = (PairWithList)r["dc_physicalactivityid"];
                            r["dc_physicalactivityid"] = new PairWithList(p.Name, p.Id, p.EntityType, dataCollection);
                            break;
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
        /// <summary>
        /// Calculate when physical activity changes.  Needs duration
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PhysicalActivity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            AutoCompleteBox cb = (AutoCompleteBox)sender;
            FrameworkElement parent = (FrameworkElement)sender;
            while (true)
            {
                parent = (FrameworkElement)VisualTreeHelper.GetParent(parent);
                if (parent == null)
                {
                    break;
                }
                else if (parent is ComboBoxWithValidation)
                {

                    break;
                }
            }
            Row r = (Row)((ComboBoxWithValidation)parent).DataContext;

            PairWithList p = (PairWithList)r["dc_physicalactivityid"];

            decimal mets = 0m;// Convert.ToDecimal(p.EntityType);//crap code.  Need to use a better container

            if (activityList.ContainsKey(new Guid(p.Id)))
            {
                mets = activityList[new Guid(p.Id)].Mets;
            }

            r["dc_physicalactivity.dc_mets"] = mets.ToString();
            r.RowChanged = true;
            App app = (App)App.Current;
            String strHours = (String)r["dc_durationminutes"];
            decimal hours = 0m;
            if (!String.IsNullOrEmpty(strHours))
            {
                hours = Convert.ToDecimal(strHours) / 60m;
            }

            //now find calories
            decimal kcals = app.contact.WeightKG * mets * hours;
            r["dc_calories"] = Math.Round(kcals, 0);

            CalculateFooter();
        }
        /// <summary>
        /// Calculate the footer values.  Also update the right hand grid
        /// </summary>
        private decimal CalculateFooter()
        {
            App app = (App)App.Current;
            Contact c = app.contact;
            decimal totalStart = 0m;
            decimal totalEnd = 0m;

            var dtf = CultureInfo.CurrentCulture.DateTimeFormat;
            SortableCollectionView col = (SortableCollectionView)dataGrid1.ItemsSource;
            //decimal total         = kcalBaseLevel;
            decimal total = 0m;
            decimal totalMinutes = 0m;
            decimal highMinutes = 0m;
            decimal lowMinutes = 10000m;

            //calculate starting point
            bool overweight = Fitness.IsOverWeight(c.BMI) ? true : false;

            decimal percentRange = 0m;
            var activityLevel = Fitness.IncrementalActivityLevel.Sedentary;

            if (totalMinutes > 0 && totalMinutes < 60)
            {
                percentRange = (totalMinutes - 1) / (59 - 1);
                activityLevel = Fitness.IncrementalActivityLevel.LightlyActive;
            }
            else if (totalMinutes > 59 && totalMinutes <= 180)
            {
                percentRange = (totalMinutes - 60) / (180 - 60);
                activityLevel = Fitness.IncrementalActivityLevel.ModeratelyToHighlyActive;
            }
            else if (totalMinutes > 180)//move to plugin
            {
                percentRange = (totalMinutes - 60) / (180 - 60);
                activityLevel = Fitness.IncrementalActivityLevel.ModeratelyToHighlyActive;
            }
            decimal incrementalPA = Fitness.CalculateIncrementalPA(Convert.ToInt32(c.Gender.Id), c.Age, overweight, activityLevel);
            decimal lowRange = Fitness.RetrieveLowValuePA(Convert.ToInt32(c.Gender.Id), c.Age, overweight, activityLevel);

            decimal PA = (percentRange * incrementalPA) + lowRange;
            totalStart = Fitness.CalculateKcals(Convert.ToInt32(c.Gender.Id), c.Age, PA, c.WeightKG, c.HeightM, overweight);

            if (col != null)
            {
                foreach (Row r in col)
                {
                    //total += Convert.ToDecimal(r["dc_calories"]);
                    decimal mets = Convert.ToDecimal(r["dc_physicalactivity.dc_mets"]);
                    decimal minutes = 0m;
                    if (!decimal.TryParse((String)r["dc_durationminutes"], out minutes))
                    {
                        minutes = 0m;
                    }
                    //decimal minutes = String.IsNullOrEmpty((String)r["dc_durationminutes"]) ? 0m : Convert.ToDecimal(r["dc_durationminutes"]);
                    decimal multipler = 1;
                    if (mets >= 1.6m && mets < 3)
                    {
                        multipler = .5m;
                    }
                    else if (mets >= 3m && mets < 6)
                    {
                        multipler = 1m;
                    }
                    else if (mets >= 6m)
                    {
                        multipler = 2m;
                    }
                    minutes = minutes * multipler;
                    totalMinutes += minutes;
                    if (lowMinutes > minutes)
                    {
                        lowMinutes = minutes;
                    }
                    if (highMinutes < minutes)
                    {
                        highMinutes = minutes;
                    }

                    //Calculate as we go
                    percentRange = 0m;

                    if (totalMinutes > 0 && totalMinutes < 60)
                    {
                        percentRange = (totalMinutes - 1) / (59 - 1);
                        activityLevel = Fitness.IncrementalActivityLevel.LightlyActive;
                    }
                    else if (totalMinutes > 59 && totalMinutes <= 180)
                    {
                        percentRange = (totalMinutes - 60) / (180 - 60);
                        activityLevel = Fitness.IncrementalActivityLevel.ModeratelyToHighlyActive;
                    }
                    else if (totalMinutes > 180)//move to plugin
                    {
                        percentRange = (totalMinutes - 60) / (180 - 60);
                        activityLevel = Fitness.IncrementalActivityLevel.ModeratelyToHighlyActive;
                    }
                    incrementalPA = Fitness.CalculateIncrementalPA(Convert.ToInt32(c.Gender.Id), c.Age, overweight, activityLevel);
                    lowRange = Fitness.RetrieveLowValuePA(Convert.ToInt32(c.Gender.Id), c.Age, overweight, activityLevel);

                    PA = (percentRange * incrementalPA) + lowRange;
                    total = Fitness.CalculateKcals(Convert.ToInt32(c.Gender.Id), c.Age, PA, c.WeightKG, c.HeightM, overweight);

                    kcalTotals.Text = "Kcals Burned: " + Math.Round(total, 0).ToString("#,##");// total.ToString();
                    decimal rowKcals = total - totalStart;
                    totalStart = total;
                    r["dc_calories"] = Math.Round(rowKcals, 0);
                }


                //Only calculate/update if there is a value in the collection
                String dayName = dtf.GetAbbreviatedDayName(this.day.DayOfWeek);
                foreach (BarChartData bcd in app.listBurned)
                {
                    if (bcd.Name.Equals(dayName, StringComparison.OrdinalIgnoreCase))
                    {
                        bcd.Value = total;
                        app.activityLog.SetBurned(dayName, total);
                        break;
                    }
                }
            }
            return (totalEnd - totalStart);
        }
        /// <summary>
        /// Retreive the fitness detail from the day
        /// </summary>
        /// <param name="day"></param>
        public void LoadDay(DateTime day, Guid contactId)
        {
            this.day = day;
            //Load the fitness log day first
            ContactId = contactId;
            LoadFitnessLogDay(contactId);
            //Set up fitness chart
            DateTime currentDate = DateTime.Now;
            int dayOfWeek = (int)currentDate.DayOfWeek;
            var dtf = CultureInfo.CurrentCulture.DateTimeFormat;
            currentDate = currentDate.AddDays(-dayOfWeek);

            fitnessChartComplete.ContactId = ContactId;
            fitnessChartComplete.RetrieveFitnessLog(currentDate, ContactId);
            fitnessChartComplete.RetrieveFoodLog(currentDate, ContactId);
        }
        public void LoadFitnessLogDay(Guid contactId)
        {

            if (App.Current.GetType() == typeof(DynamicConnections.NutriStyle.MenuGenerator.App) && !isLoaded)
            {
                CalculateFooter();
                App app = (App)App.Current;
                isLoaded = true;
                String fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                  <entity name='dc_fitnesslogday'>
                    <attribute name='dc_fitnesslogdayid' />
                    <attribute name='dc_detailedlogging' />
                    <attribute name='dc_activitylevel' />
                    <attribute name='dc_menuid' />
                    <attribute name='dc_kcals' />
                    <filter type='and'>
                      <condition attribute='statecode' operator='eq' value='0' />
                      <condition attribute='dc_contactid' operator='eq' value='@CONTACTID' />
                      <condition attribute='dc_date' operator='on' value='@DATETIME' />
                    </filter>
                    <link-entity name='dc_menu' alias='aa' to='dc_menuid' from='dc_menuid'>
                        <filter type='and'> 
                            <condition attribute='dc_primarymenu' value='1' operator='eq'/> 
                        </filter> 
                    </link-entity>
                  </entity>
                </fetch>";
                //<condition attribute='dc_menuid' operator='eq' value='@MENUID' />
                fetchXml = fetchXml.Replace("@CONTACTID", contactId.ToString());
                fetchXml = fetchXml.Replace("@DATETIME", day.ToString("MM/dd/yyyy"));
                //fetchXml = fetchXml.Replace("@MENUID", app.contact.MenuId.ToString());

                XElement orderXml = new XElement("ColumnOrder");
                orderXml.Add(new XElement("Column", "dc_detailedlogging"));
                orderXml.Add(new XElement("Column", "dc_activitylevel"));
                orderXml.Add(new XElement("Column", "dc_fitnesslogdayid"));
                orderXml.Add(new XElement("Column", "dc_menuid"));

                try
                {
                    busyIndicator.IsBusy = true;
                    busyIndicator.Visibility = System.Windows.Visibility.Visible;

                    CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
                    cms.RetrieveFetchXmlCompleted += new EventHandler<CrmSdk.RetrieveFetchXmlCompletedEventArgs>(cms_RetrieveFitnessLogDay);
                    cms.RetrieveFetchXmlAsync(fetchXml, orderXml.ToString());
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                    MessageBox.Show(err.StackTrace);
                }
            }
        }
        private void cms_RetrieveFitnessLogDay(object sender, CrmSdk.RetrieveFetchXmlCompletedEventArgs e)
        {
            var element = e.Result;
            String menuName = String.Empty;
            String entityName = "dc_fitnesslogday";

            App app = (App)App.Current;
            var dtf = CultureInfo.CurrentCulture.DateTimeFormat;


            var rows = element.Descendants(entityName);
            if (rows.Count() == 0)
            {
                //pull from contact
                Contact c = ((App)App.Current).contact;
                ActivityLevel.SelectedPair = new PairWithList(c.ActivityLevel.Name, c.ActivityLevel.Id, String.Empty, null);

            } else {

                foreach (var row in rows)
                {
                    fitnessLogDayId             = new Guid(row.Descendants("dc_fitnesslogdayid").First().Value);
                    if (row.Descendants("dc_activitylevel") != null && row.Descendants("dc_activitylevel").Count() > 0)
                    {
                        ActivityLevel.SelectedPair = new PairWithList(row.Descendants("dc_activitylevel").First().Value,
                            row.Descendants("dc_activitylevel").First().Attribute("Id").Value, String.Empty, null);
                    }
                    if (row.Descendants("dc_detailedlogging") != null && row.Descendants("dc_detailedlogging").Count() > 0)
                    {
                        if (Convert.ToBoolean(row.Descendants("dc_detailedlogging").First().Value))
                        {
                            detailedFitness.IsChecked = true;
                            generalFitnessStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                            detailedFitnessStackPanel.Visibility = System.Windows.Visibility.Visible;
                        }
                    }


                    if (row.Descendants("dc_kcals") != null && row.Descendants("dc_kcals").Count() > 0)
                    {
                        decimal kcals = Convert.ToDecimal(row.Descendants("dc_kcals").First().Value);
                        kcalTotals.Text = "Kcals Burned: " + Math.Round(kcals, 0).ToString("#,##");// total.ToString();

                        String dayName = dtf.GetAbbreviatedDayName(this.day.DayOfWeek);

                        foreach (BarChartData bcd in app.listBurned)
                        {
                            if (bcd.Name.Equals(dayName, StringComparison.OrdinalIgnoreCase))
                            {
                                bcd.Value = kcals;
                                app.activityLog.SetBurned(dayName, kcals);
                                break;
                            }
                        }
                    }
                    if (row.Descendants("dc_menuid") != null && row.Descendants("dc_menuid").Count() > 0)
                    {
                        menuName = row.Descendants("dc_menuid").First().Value;
                    }
                }
            }
            var dailyFitnessLog = General.FindParrentUserControl(this, typeof(DailyFitnessLog));
            if (dailyFitnessLog != null)
            {
                ((DailyFitnessLog)dailyFitnessLog).MenuName(menuName);
            }
            LoadDayPostContact(ContactId);
        }
        
        public void LoadDayPostContact(Guid contactId)
        {

            if (App.Current.GetType() == typeof(DynamicConnections.NutriStyle.MenuGenerator.App))
            {
                if (fitnessLogDayId == Guid.Empty)
                {
                    CalculateFooter();
                }
                App app = (App)App.Current;
                isLoaded = true;
                String fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                  <entity name='dc_fitnesslog'>
                    <attribute name='dc_fitnesslogid' />
                    <attribute name='dc_physicalactivitycategoryid' />
                    <attribute name='dc_physicalactivityid' />
                    <attribute name='dc_durationminutes' />
                    <attribute name='dc_calories' />
                    <order attribute='dc_physicalactivityid' descending='false' />
                    <filter type='and'>
                      <condition attribute='statecode' operator='eq' value='0' />
                      <condition attribute='dc_contactid' operator='eq' value='@CONTACTID' />
                      <condition attribute='dc_date' operator='on' value='@DATETIME' />
                    </filter>
                    <link-entity name='dc_physicalactivity' alias='dc_physicalactivity' to='dc_physicalactivityid' from='dc_physicalactivityid'>
                        <attribute name='dc_mets' />
                    </link-entity>
                    <link-entity name='dc_fitnesslogday' from='dc_fitnesslogdayid' to='dc_fitnesslogdayid' alias='aa'>
                      <filter type='and'>
                        <condition attribute='dc_contactid' operator='eq' value='@CONTACTID' />
                        <condition attribute='dc_date' operator='on' value='@DATETIME' />
                      </filter>
                      <link-entity name='dc_menu' alias='ab' to='dc_menuid' from='dc_menuid'>
                        <filter type='and'> 
                            <condition attribute='dc_primarymenu' value='1' operator='eq'/> 
                        </filter> 
                      </link-entity>
                    </link-entity>
                  </entity>
                </fetch>";

                fetchXml = fetchXml.Replace("@CONTACTID", contactId.ToString());
                fetchXml = fetchXml.Replace("@DATETIME", day.ToString("MM/dd/yyyy"));
                //fetchXml = fetchXml.Replace("@MENUID", app.contact.MenuId.ToString());

                XElement orderXml = new XElement("ColumnOrder");
                orderXml.Add(new XElement("Column", "dc_fitnesslogid"));
                orderXml.Add(new XElement("Column", "dc_physicalactivitycategoryid"));
                orderXml.Add(new XElement("Column", "dc_physicalactivityid"));
                orderXml.Add(new XElement("Column", "dc_durationminutes"));
                orderXml.Add(new XElement("Column", "dc_calories"));
                orderXml.Add(new XElement("Column", "dc_physicalactivity.dc_mets"));

                try
                {
                    busyIndicator.IsBusy = true;
                    busyIndicator.Visibility = System.Windows.Visibility.Visible;

                    CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
                    cms.RetrieveFetchXmlCompleted += new EventHandler<CrmSdk.RetrieveFetchXmlCompletedEventArgs>(cms_RetrieveFitnessLog);
                    cms.RetrieveFetchXmlAsync(fetchXml, orderXml.ToString());
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                    MessageBox.Show(err.StackTrace);
                }
            }
        }
        private void cms_RetrieveFitnessLog(object sender, CrmSdk.RetrieveFetchXmlCompletedEventArgs e)
        {

            buildGrid("dc_fitnesslog", dataGrid1, busyIndicator, e.Result, 1);

        }
        
        private void buildGrid(String entityName, DataGrid dataGrid, BusyIndicator bi, XElement element, int columns)
        {

            data = new SortableCollectionView();
            try
            {

                XElement xEl = element.Descendants("columns").ToList()[0];//get first one
                //MessageBox.Show(xEl.ToString());
                foreach (XElement xe in xEl.Elements())
                {
                    data.ColumnTypes.Add(xe.Name.LocalName, xe.Attribute("Type").Value);

                    if (!data.EntityTypes.ContainsKey((xe.Name.LocalName)))
                    {
                        data.EntityTypes.Add(xe.Name.LocalName, xe.Attribute("Entity").Value);
                    }
                    if (xe.Name.LocalName.Equals("dc_physicalactivity.dc_mets", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                    if (xe.Attribute("Type").Value.Equals("String", StringComparison.OrdinalIgnoreCase) ||
                        xe.Attribute("Type").Value.Equals("UniqueIdentifier", StringComparison.OrdinalIgnoreCase) ||
                        xe.Name.LocalName.Equals("dc_calories", StringComparison.OrdinalIgnoreCase))
                    {
                        DataGridTextColumn dg = new DataGridTextColumn();
                        dg.Header = xe.Attribute("LabelName").Value;
                        dg.CanUserSort = true;

                        dg.SortMemberPath = xe.Name.LocalName;
                        dg.Binding = new Binding("Data")
                        {
                            Converter = _rowIndexConverter,
                            ConverterParameter = xe.Name.LocalName
                        };

                        //visable?
                        dg.Visibility = System.Windows.Visibility.Visible;
                        if (xe.Name.LocalName.Equals(entityName + "id", StringComparison.OrdinalIgnoreCase))
                        {
                            dg.Header = xe.Name.LocalName;
                            dg.Visibility = System.Windows.Visibility.Collapsed;
                            //dg.Width = new DataGridLength
                        }
                        dg.IsReadOnly = true;
                        dataGrid.Columns.Add(dg);
                        dg.Width = new DataGridLength(65);
                    }

                    else if (xe.Attribute("Type").Value.Equals("Decimal", StringComparison.OrdinalIgnoreCase))
                    {
                        DataGridTemplateColumn dg = new DataGridTemplateColumn();
                        dg.Header = xe.Attribute("LabelName").Value;

                        dg.SortMemberPath = xe.Name.LocalName;
                        dg.CanUserSort = true;
                        StringBuilder CellETempPickList = new StringBuilder();
                        CellETempPickList.Append("<DataTemplate ");
                        CellETempPickList.Append("xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' ");
                        CellETempPickList.Append("xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' ");
                        CellETempPickList.Append("xmlns:basics2='clr-namespace:DynamicConnections.NutriStyle.MenuGenerator.Controls;");
                        CellETempPickList.Append("assembly=DynamicConnections.NutriStyle.MenuGenerator' >");

                        CellETempPickList.Append("<basics2:CustomTextBox Watermark='Type' TagName='" + xe.Name.LocalName + "' Foreground='#4C97CC' SelectedText='{Binding Path=Data, Converter={StaticResource rowConvertor}, ConverterParameter=" + xe.Name.LocalName + ", Mode=TwoWay}' />");
                        CellETempPickList.Append("</DataTemplate>");
                        dg.CellTemplate = (DataTemplate)XamlReader.Load(CellETempPickList.ToString());

                        dg.Width = new DataGridLength(100);
                        dataGrid.Columns.Add(dg);
                    }
                    else if (xe.Attribute("Type").Value.Equals("Lookup", StringComparison.OrdinalIgnoreCase))
                    {
                        DataGridTemplateColumn dg = new DataGridTemplateColumn();
                        dg.Header = xe.Attribute("LabelName").Value;

                        dg.SortMemberPath = xe.Name.LocalName;
                        dg.CanUserSort = true;

                        StringBuilder CellETempPickList = new StringBuilder();
                        CellETempPickList.Append("<DataTemplate ");
                        CellETempPickList.Append("xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' ");
                        CellETempPickList.Append("xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' ");
                        CellETempPickList.Append("xmlns:basics2='clr-namespace:DynamicConnections.NutriStyle.MenuGenerator.Controls;");
                        CellETempPickList.Append("assembly=DynamicConnections.NutriStyle.MenuGenerator' >");

                        CellETempPickList.Append("<basics2:ComboBoxWithValidation Watermark='Select' TagName='" + xe.Name.LocalName + "' Foreground='#4C97CC' SelectedPair='{Binding Path=Data, Converter={StaticResource rowConvertor}, ConverterParameter=" + xe.Name.LocalName + ", Mode=TwoWay}' />");
                        CellETempPickList.Append("</DataTemplate>");
                        dg.CellEditingTemplate = (DataTemplate)XamlReader.Load(CellETempPickList.ToString());

                        if (xe.Name.LocalName.Equals("dc_physicalactivityid", StringComparison.OrdinalIgnoreCase))
                        {
                            dg.Width = new DataGridLength(196);
                        }
                        else
                        {
                            dg.Width = new DataGridLength(120);
                        }
                        dataGrid.Columns.Add(dg);
                    }
                }

                //add image for delete
                DataGridTemplateColumn dgImage = new DataGridTemplateColumn();
                dgImage.Header = "Del";
                //dgImage.SortMemberPath = xe.Name.LocalName;
                dgImage.CanUserSort = false;

                StringBuilder CellETemp = new StringBuilder();
                CellETemp.Append("<DataTemplate ");
                CellETemp.Append("xmlns='http://schemas.microsoft.com/winfx/");
                CellETemp.Append("2006/xaml/presentation' ");
                CellETemp.Append("xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' >");
                CellETemp.Append("<Button HorizontalAlignment='Left' Cursor='Hand' ToolTipService.ToolTip='Remove From Shopping List' Width='26' Height='26' Visibility='{Binding Path=Data, Converter={StaticResource rowConvertor}, ConverterParameter=isdeletevisible, Mode=OneWay}'>");
                CellETemp.Append("<Image Stretch='Fill' Name='Delete' Source='/DynamicConnections.NutriStyle.MenuGenerator;component/images/delete.png'/>");// MouseLeftButtonDown='ImageDelete_MouseLeftButtonDown'
                CellETemp.Append("</Button>");
                CellETemp.Append("</DataTemplate>");
                dgImage.CellEditingTemplate = (DataTemplate)XamlReader.Load(CellETemp.ToString());

                dgImage.IsReadOnly = false;
                dgImage.Width = new DataGridLength(34);
                //visable?
                dgImage.Visibility = System.Windows.Visibility.Visible;
                //dataGrid1.Columns.Add(dgImage);

                dataGrid.Columns.Add(dgImage);

                //Bind data
                var rows = element.Descendants(entityName);
                foreach (var row in rows)
                {
                    Row rowData = new Row();
                    //MessageBox.Show(row.ToString());
                    foreach (KeyValuePair<String, String> type in data.ColumnTypes)
                    {

                        if (type.Value.Equals("Lookup", StringComparison.OrdinalIgnoreCase))
                        {
                            rowData[type.Key] = new PairWithList(String.Empty, Guid.Empty.ToString(), String.Empty, new List<PairWithList>());
                        }
                        if (type.Value.Equals("Picklist", StringComparison.OrdinalIgnoreCase))
                        {
                            rowData[type.Key] = "1";//new Pair(String.Empty, "-1");
                        }
                        if (type.Value.Equals("DateTime", StringComparison.OrdinalIgnoreCase))
                        {
                            rowData[type.Key] = null;
                        }
                        else if (type.Value.Equals("Decimal", StringComparison.OrdinalIgnoreCase))
                        {
                            rowData[type.Key] = null;
                        }
                        if (type.Value.Equals("Money", StringComparison.OrdinalIgnoreCase))
                        {
                            rowData[type.Key] = null;
                        }
                        else
                        {
                            rowData[type.Key] = String.Empty;
                        }
                    }

                    foreach (XElement xe in row.Elements())
                    {
                        if (data.ColumnTypes.ContainsKey(xe.Name.LocalName) && data.ColumnTypes[xe.Name.LocalName].Equals("money", StringComparison.OrdinalIgnoreCase))
                        {
                            rowData[xe.Name.LocalName] = String.Format("{0:C2}", Convert.ToDecimal(xe.Value));
                        }
                        else if (data.ColumnTypes.ContainsKey(xe.Name.LocalName) && data.ColumnTypes[xe.Name.LocalName].Equals("lookup", StringComparison.OrdinalIgnoreCase))
                        {
                            if (xe.Name.LocalName.Equals("dc_physicalactivitycategoryid", StringComparison.OrdinalIgnoreCase))
                            {
                                App app = (App)App.Current;
                                rowData[xe.Name.LocalName] = new PairWithList(xe.Value, xe.Attribute("Id").Value, data.EntityTypes[xe.Name.LocalName], app.physicalActivityCategory);
                            }
                            else
                            {
                                rowData[xe.Name.LocalName] = new PairWithList(xe.Value, xe.Attribute("Id").Value, data.EntityTypes[xe.Name.LocalName], new List<PairWithList>());
                            }
                        }
                        else if (data.ColumnTypes.ContainsKey(xe.Name.LocalName) && data.ColumnTypes[xe.Name.LocalName].Equals("picklist", StringComparison.OrdinalIgnoreCase))
                        {
                            rowData[xe.Name.LocalName] = new Pair(xe.Value, xe.Attribute("Id").Value);
                        }
                        else
                        {
                            rowData[xe.Name.LocalName] = xe.Value;
                        }
                    }
                    rowData.CreateRow = false;
                    data.Add(rowData);
                }

                dataGrid.ItemsSource = data;
                bi.IsBusy = false;
                dataGrid.Visibility = System.Windows.Visibility.Visible;


                dataGrid.BorderThickness = new Thickness(1);



            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
            }
        }

        /// <summary>
        /// For adding fitness row
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, EventArgs e)
        {

            //SortableCollectionView dgCollection = (SortableCollectionView)dataGrid1.ItemsSource;
            //Now add row to correct datagrid
            Row r = new Row();

            //Populate columnTypes  --  rowData.ColumnTypes.Add(xe.Name.LocalName, xe.Attribute("Type").Value);
            foreach (KeyValuePair<String, String> type in data.ColumnTypes)
            {
                if (type.Value.Equals("Lookup", StringComparison.OrdinalIgnoreCase))
                {
                    if (type.Key.Equals("dc_physicalactivitycategoryid", StringComparison.OrdinalIgnoreCase))
                    {
                        App app = (App)App.Current;
                        r[type.Key] = new PairWithList(String.Empty, Guid.Empty.ToString(), data.EntityTypes[type.Key], app.physicalActivityCategory);
                    }
                    else
                    {
                        r[type.Key] = new PairWithList(String.Empty, Guid.Empty.ToString(), data.EntityTypes[type.Key], new List<PairWithList>());
                    }
                }
                else if (type.Value.Equals("Picklist", StringComparison.OrdinalIgnoreCase))
                {
                    r[type.Key] = new Pair(String.Empty, String.Empty);
                }
                else if (type.Value.Equals("DateTime", StringComparison.OrdinalIgnoreCase))
                {
                    r[type.Key] = null;
                }
                else if (type.Value.Equals("Money", StringComparison.OrdinalIgnoreCase))
                {
                    r[type.Key] = null;
                }
                else if (type.Value.Equals("Decimal", StringComparison.OrdinalIgnoreCase))
                {
                    r[type.Key] = null;
                }
                else if (type.Value.Equals("UniqueIdentifier", StringComparison.OrdinalIgnoreCase))
                {
                    r[type.Key] = Guid.NewGuid().ToString();
                }
                else
                {
                    r[type.Key] = String.Empty;
                }
            }
            r.RowChanged = true;
            r.CreateRow = true;
            try
            {
                //dataFoods.Add(r);
                //dgCollection.Insert(0, r);
                data.Add(r);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
            }
        }
        /// <summary>
        /// Save the row or general data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            App app = null;
            Row r = null;

            if (App.Current.GetType() == typeof(DynamicConnections.NutriStyle.MenuGenerator.App))
            {
                XElement eventXml = null;

                if (detailedFitness.IsChecked.Value)
                {
                    app = (App)App.Current;
                    //get row
                    foreach (Row row in data)
                    {
                        if (row.RowChanged)
                        {
                            r = row;
                        }
                    }
                    selectedRow = r;
                    if (r == null)
                    {
                        busyIndicator.IsBusy = false;
                        return;//nothing to do
                    }
                    if (true)//if (ValidateRow(r))
                    {
                        busyIndicator.IsBusy = true;
                        //Have row
                        //create xml document and send to server to save
                        String entityName = "dc_fitnesslog";
                        eventXml = new XElement("dc_fitnesslog");
                        try
                        {
                            foreach (String columnName in data.ColumnTypes.Keys)
                            {
                                //MessageBox.Show("ColumnName: " + columnName + ": type: " + dataFoods.ColumnTypes[columnName]);
                                if (data.ColumnTypes[columnName].Equals("DateTime", StringComparison.OrdinalIgnoreCase))
                                {
                                    if (r[columnName] != null)
                                    {
                                        eventXml.Add(new XElement(columnName, ((DateTime)r[columnName])));
                                    }
                                }
                                else if (data.ColumnTypes[columnName].Equals("Lookup", StringComparison.OrdinalIgnoreCase))
                                {
                                    if (r[columnName] != null && r[columnName].GetType() == typeof(PairWithList))
                                    {
                                        if (((PairWithList)r[columnName]).Id != "" && ((PairWithList)r[columnName]).Id != String.Empty)
                                        {
                                            XElement lookupNode = new XElement(new XElement(columnName, ((PairWithList)r[columnName]).Id));
                                            eventXml.Add(lookupNode);

                                            XAttribute at = new XAttribute("entityname", data.EntityTypes[columnName]);
                                            lookupNode.Add(at);
                                        }
                                    }
                                }
                                else if (data.ColumnTypes[columnName].Equals("Picklist", StringComparison.OrdinalIgnoreCase))
                                {
                                    eventXml.Add(new XElement(columnName, ((Pair)r[columnName]).Id));
                                }
                                else if (data.ColumnTypes[columnName].Equals("UniqueIdentifier", StringComparison.OrdinalIgnoreCase))
                                {
                                    if (columnName.Equals(entityName + "id", StringComparison.OrdinalIgnoreCase) &&
                                        !String.IsNullOrEmpty((String)r[columnName]) && r.CreateRow)
                                    {
                                        eventXml.Add(new XElement(columnName, Guid.Empty.ToString()));
                                    }
                                    else if (!String.IsNullOrEmpty((String)r[columnName]))
                                    {
                                        eventXml.Add(new XElement(columnName, ((String)r[columnName])));
                                    }
                                }
                                else if (data.ColumnTypes[columnName].Equals("String", StringComparison.OrdinalIgnoreCase))
                                {
                                    if (!String.IsNullOrEmpty((String)r[columnName]))
                                    {
                                        eventXml.Add(new XElement(columnName, (String)r[columnName]));
                                    }
                                }
                                else if (data.ColumnTypes[columnName].Equals("Decimal", StringComparison.OrdinalIgnoreCase))
                                {

                                    //Check type
                                    if (r[columnName].GetType() == typeof(decimal))
                                    {
                                        eventXml.Add(new XElement(columnName, Convert.ToString(r[columnName])));
                                    }
                                    else
                                    {
                                        if (!String.IsNullOrEmpty((String)r[columnName]))
                                        {
                                            eventXml.Add(new XElement(columnName, (String)r[columnName]));
                                        }
                                    }
                                }
                                else if (data.ColumnTypes[columnName].Equals("Memo", StringComparison.OrdinalIgnoreCase))
                                {
                                    if (!String.IsNullOrEmpty((String)r[columnName]))
                                    {
                                        eventXml.Add(new XElement(columnName, (String)r[columnName]));
                                    }
                                }
                            }
                            //eventXml.Add(new XElement("contactid", ap.contactId));
                            XElement lookupNodeContact = new XElement(new XElement("dc_contactid", ContactId));
                            eventXml.Add(lookupNodeContact);

                            XAttribute atContact = new XAttribute("entityname", "contact");
                            lookupNodeContact.Add(atContact);

                            eventXml.Add(new XElement("dc_date", this.day));

                        }
                        catch (Exception err)
                        {
                            MessageBox.Show(err.Message);
                            MessageBox.Show(err.StackTrace);
                        }
                        r.RowChanged = false;
                    }
                }
                else
                {
                    
                }
                CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
                cms.CreateUpdateCompleted += new EventHandler<CrmSdk.CreateUpdateCompletedEventArgs>(cms_CreateUpdateGrid);
                cms.CreateUpdateAsync(eventXml);
            }
        }
        private void cms_CreateUpdateGrid(object sender, CrmSdk.CreateUpdateCompletedEventArgs e)
        {
            var results = from x in e.Result.Descendants("Success") select x;
            Guid Id = new Guid(e.Result.Value.ToString());
            selectedRow.RowChanged = false;
            selectedRow.CreateRow = false;
            //set id
            selectedRow["dc_fitnesslogid"] = Id.ToString();
            //call back
            Save_Click(sender, null);
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;

            if (rb.Name.Equals("generalfitness", StringComparison.OrdinalIgnoreCase) && rb.IsChecked.Value)
            {
                generalFitnessStackPanel.Visibility = System.Windows.Visibility.Visible;
                detailedFitnessStackPanel.Visibility = System.Windows.Visibility.Collapsed;
            }
            else if (rb.Name.Equals("detailedfitness", StringComparison.OrdinalIgnoreCase) && rb.IsChecked.Value)
            {
                generalFitnessStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                detailedFitnessStackPanel.Visibility = System.Windows.Visibility.Visible;
            }
        }
        private void ActivityLabel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ChildWindows.ActivityLevelsPopup alp = new ActivityLevelsPopup("Activity Levels");
            alp.Show();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ActivityLevel.SelectedPair = new PairWithList(String.Empty, String.Empty, String.Empty, ((App)App.Current).activityLevel);
            ActivityLevel.IsBusy = false;
        }
        //Save general fitness
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            //make sure tomething is selected
            if (ActivityLevel.SelectedPair == null || String.IsNullOrEmpty(ActivityLevel.SelectedPair.Name) )
            {
                Status s = new Status("Please Select activity level", false);
                s.Show();
            }
            else 
            {
                busyIndicator.IsBusy = true;
                XElement eventXml = new XElement("dc_fitnesslogday");
                //activity level
                if (ActivityLevel.SelectedPair != null)
                {
                    eventXml.Add(new XElement("dc_activitylevel", ((PairWithList)ActivityLevel.SelectedPair).Id));
                }
                //set to general
                eventXml.Add(new XElement("dc_detailedlogging", "false"));
                //Date
                eventXml.Add(new XElement("dc_date", General.ZeroOutDateHHMMSS(this.day).ToString()));
                //Contact Id
                XElement lookupNodeContact = new XElement(new XElement("dc_contactid", ContactId.ToString()));
                eventXml.Add(lookupNodeContact);

                XAttribute atContact = new XAttribute("entityname", "contact");
                lookupNodeContact.Add(atContact);
                if (fitnessLogDayId != Guid.Empty)
                {
                    eventXml.Add(new XElement("dc_fitnesslogdayid", fitnessLogDayId.ToString()));
                }

                CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
                cms.CreateUpdateReturnEntityCompleted += new EventHandler<CrmSdk.CreateUpdateReturnEntityCompletedEventArgs>(cms_CreateUpdateGridDay);
                cms.CreateUpdateReturnEntityAsync(eventXml, true);
            }
        }
        private void cms_CreateUpdateGridDay(object sender, CrmSdk.CreateUpdateReturnEntityCompletedEventArgs e)
        {
            //var results = from x in e.Result.Descendants("Success") select x;
            Guid Id = new Guid(e.Result.Descendants("dc_fitnesslogdayid").First().Value);
            fitnessLogDayId = Id;
            busyIndicator.IsBusy = false;
            decimal total = Convert.ToDecimal(e.Result.Descendants("dc_kcals").First().Value);
            kcalTotals.Text = "Kcals Burned: " + Math.Round(total, 0).ToString("#,##");// total.ToString();

            App app = (App)App.Current;
            var dtf = CultureInfo.CurrentCulture.DateTimeFormat;


            String dayName = dtf.GetAbbreviatedDayName(this.day.DayOfWeek);

            foreach (BarChartData bcd in app.listBurned)
            {
                if (bcd.Name.Equals(dayName, StringComparison.OrdinalIgnoreCase))
                {
                    bcd.Value = total;
                    app.activityLog.SetBurned(dayName, total);
                    break;
                }
            }
        }
    }
}

