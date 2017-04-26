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
using DynamicConnections.NutriStyle.MenuGenerator.Engine;
using System.Windows.Controls.DataVisualization.Charting.Primitives;
using System.Windows.Controls.DataVisualization;

namespace DynamicConnections.NutriStyle.MenuGenerator.Pages
{
    public partial class MenuEditor : Page
    {
        List<Pair> meals;
        List<Pair> days;
        Controls.ComboBox comboBox;
        
        private int AsyncErrorResource = 0;
        private int AsyncFinal = 0;
        private List<Exception> AsyncErrors;    // If not null errors have been encountered.
        private int PendingAsyncOperations;     // Holds the counted total of ongoing async operations. Zero means do users final operation.
        private Action FinalOperation;          // The user's final operation.
        SortableCollectionView dataFoods;
        
        private RowIndexConverter _rowIndexConverter = new RowIndexConverter();
        Dictionary<String, Dictionary<String, SortableCollectionView>> daysList = new Dictionary<String, Dictionary<String, SortableCollectionView>>();
        Dictionary<String, int> dayList = new Dictionary<string, int>();
        List<DataGrid> dataGrids = new List<DataGrid>();
        DataGrid activeDataGrid;

        public MenuEditor()
        {
            dataFoods = new SortableCollectionView();
            comboBox = new Controls.ComboBox();
            meals = new List<Pair>();
            days = new List<Pair>();
            InitializeComponent();
            //Setup birthday values

            MultipleAsyncRun(RetrieveDays);
            MultipleAsyncRun(RetrieveMeals);

            FinalAsync(CombineDataAndDisplay);
            PopulateHelp hp = new PopulateHelp();
            //hp.Retrieve("menu editor", helpText);
            dataGrids.Add(dataGridSatBreakfast);
            dataGrids.Add(dataGridSatLunch);
            dataGrids.Add(dataGridSatDinner);

            foreach (DataGrid dg in dataGrids)
            {
                dg.LoadingRow += new EventHandler<DataGridRowEventArgs>(dataGrid1_LoadingRow);
            }
            //day.MyAutoCompleteBox.SelectionChanged += new SelectionChangedEventHandler(Day_SelectionChanged);
        }
        void Day_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!String.IsNullOrEmpty(day.MyAutoCompleteBox.Text))
            {
                RetrieveMealFoods();
            }
        }
        public void CombineDataAndDisplay()
        {
            RetrieveMealFoods();
        }

        void dataGrid1_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            DataGrid dataGrid1 = (DataGrid)sender;
            if (sender != null)
            {
                DataGrid dg = (DataGrid)sender;
                //deal with getting the data bound to the autocompletebox
                try
                {
                    foreach (DataGridColumn c in dg.Columns)
                    {
                        //MessageBox.Show(""+c.GetCellContent(e.Row).GetType());
                        if (c.GetCellContent(e.Row).GetType() == typeof(Image))
                        {
                            Image ccb = c.GetCellContent(e.Row) as Image;
                            ToolTipService.SetToolTip(ccb, "Remove From Menu");
                            ccb.MouseLeftButtonUp -= ccb_MouseLeftButtonUp;
                            ccb.MouseLeftButtonUp += new MouseButtonEventHandler(ccb_MouseLeftButtonUp);
                        }
                        else if (c.GetCellContent(e.Row).GetType() == typeof(Controls.ComboBox))
                        {
                            Controls.ComboBox ccb = c.GetCellContent(e.Row) as Controls.ComboBox;

                            if (ccb.TagName.Equals("dc_day", StringComparison.OrdinalIgnoreCase))
                            {
                                ccb.MyAutoCompleteBox.ItemsSource = days;
                            }
                            else if (ccb.TagName.Equals("dc_meal", StringComparison.OrdinalIgnoreCase))
                            {
                                ccb.MyAutoCompleteBox.ItemsSource = meals;
                            }
                            else if (ccb.TagName.Equals("dc_foodid", StringComparison.OrdinalIgnoreCase))
                            {

                                ccb.KeyDown -= food_KeyDown;
                                ccb.KeyDown += new KeyEventHandler(food_KeyDown);
                            }
                            else if (ccb.TagName.Equals("dc_portiontypeid", StringComparison.OrdinalIgnoreCase))
                            {
                                ccb.MyAutoCompleteBox.IsEnabled = false;
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
        /// Retrieve the portion type for the food
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MyAutoCompleteBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            //Make sure something is selected on the row
            activeDataGrid = GetRoot((FrameworkElement)sender);
            if (activeDataGrid.SelectedItem != null)
            {

                AutoCompleteBox cb      = (AutoCompleteBox)sender;
                Border b                = (Border)cb.Parent;
                Grid g                  = (Grid)b.Parent;
                Controls.ComboBox ccb   = (Controls.ComboBox)g.Parent;
                //Get parent datagrid
                
                
                if (cb.SelectedItem != null && !String.IsNullOrEmpty(cb.Text))
                {
                    Pair p = (Pair)cb.SelectedItem;
                    //Retreive the portion size
                    String fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                      <entity name='dc_foods'>
                        <attribute name='dc_portiontypeid' />
                        <order attribute='dc_foodsid' descending='false' />
                              <filter type='and'>
                                <condition attribute='dc_foodsid' operator='eq' value='@FOODID' />
                              </filter>
                      </entity>
                    </fetch>";

                    fetchXml = fetchXml.Replace("@FOODID", p.Id);

                    XElement orderXml = new XElement("ColumnOrder");
                    orderXml.Add(new XElement("Column", "dc_portiontype"));
                    orderXml.Add(new XElement("Column", "dc_foodsid"));

                    try
                    {
                        CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
                        cms.RetrieveFetchXmlCompleted += new EventHandler<CrmSdk.RetrieveFetchXmlCompletedEventArgs>(cms_RetrievePortionType);
                        cms.RetrieveFetchXmlAsync(fetchXml, orderXml.ToString());
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show(err.Message);
                        MessageBox.Show(err.StackTrace);
                    }
                }
            }
            
        }
        private void cms_RetrievePortionType(object sender, CrmSdk.RetrieveFetchXmlCompletedEventArgs e)
        {
            
            String entityName = "dc_foods";
            XElement element = e.Result;

            try
            {
                if (element.Descendants(entityName).Count() > 0)
                {
                    //Bind data
                    var rows = element.Descendants(entityName);

                    var row = rows.First();
                    Pair p = new Pair();

                    foreach (XElement xe in row.Elements())
                    {
                        if (xe.Name.LocalName.Equals("dc_portiontypeid", StringComparison.OrdinalIgnoreCase))
                        {
                            p = new Pair(xe.Value, xe.Attribute("Id").Value);
                        }
                    }

                    Row r = (Row)activeDataGrid.SelectedItem;
                    r.RowChanged = true;//mark as dirty
                    r["dc_portiontypeid"] = p;
                    
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
            }
            
        }

        void ccb_KeyDown(object sender, KeyEventArgs e)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Remove item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ccb_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

            try
            {
                DataGrid dataGrid = GetRoot((FrameworkElement)sender);

                SortableCollectionView data = (SortableCollectionView)dataGrid.ItemsSource;

                Image img = sender as Image;
                String Id = (String)((Row)dataGrid.SelectedItem)["dc_mealfoodid"];
                Row r = (Row)dataGrid.SelectedItem;
                if (!String.IsNullOrEmpty(Id))
                {
                    XElement deleteXml = new XElement("dc_mealfood", new XElement("id", Id));

                    CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
                    cms.DeleteCompleted += new EventHandler<CrmSdk.DeleteCompletedEventArgs>(cms_Delete);
                    cms.DeleteAsync(deleteXml);
                }
                data.Remove(r);
                dataGrid.ItemsSource = data;

            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
            }
        }
        private DataGrid GetRoot(FrameworkElement child)
        {
            //var parent = child.Parent as FrameworkElement;

            FrameworkElement parent = VisualTreeHelper.GetParent(child) as FrameworkElement;

            if (parent is DataGrid)
            {
                return (DataGrid)parent;
            }
            else
            {
                return (GetRoot(parent));
            }
        }

        private void cms_Delete(object sender, CrmSdk.DeleteCompletedEventArgs e)
        {
            //CrmIndicator.IsBusy = false;
            //place holder
        }
        private void RetrieveDays()
        {
            CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
            cms.RetrieveOptionSetValuesCompleted += new EventHandler<CrmSdk.RetrieveOptionSetValuesCompletedEventArgs>(cms_RetrieveDays);
            cms.RetrieveOptionSetValuesAsync("dc_foodlike", "dc_day");

            cms.RetrieveOptionSetValuesCompleted += (s, e) => { AssignResultCheckforAllAsyncsDone(e, e.Result, "Acquisition failure for days"); };
        }
        private void cms_RetrieveDays(object sender, CrmSdk.RetrieveOptionSetValuesCompletedEventArgs e)
        {
            try
            {
                ComboBoxItem itemSelected = new ComboBoxItem();

                var results = from x in e.Result.Descendants("pair") select x;

                foreach (var pair in results)
                {
                    Pair p = new Pair();
                    p.Name = pair.Descendants("name").First().Value;
                    p.Id = pair.Descendants("value").First().Value;

                    days.Add(p);
                    dayList.Add(pair.Descendants("name").First().Value, Convert.ToInt32(pair.Descendants("value").First().Value));
                }
                day.Items = days;
                day.SelectedPair = days[0];
                TabControl.SelectedIndex = 0;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
            }
        }
        private void RetrieveMeals()
        {

            CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
            cms.RetrieveOptionSetValuesCompleted += new EventHandler<CrmSdk.RetrieveOptionSetValuesCompletedEventArgs>(cms_RetrieveMeals);
            cms.RetrieveOptionSetValuesAsync("dc_foodlike", "dc_meal");

            cms.RetrieveOptionSetValuesCompleted += (s, e) => { AssignResultCheckforAllAsyncsDone(e, e.Result, "Acquisition failure for meals"); };
        }
        private void cms_RetrieveMeals(object sender, CrmSdk.RetrieveOptionSetValuesCompletedEventArgs e)
        {
            try
            {
                ComboBoxItem itemSelected = new ComboBoxItem();

                var results = from x in e.Result.Descendants("pair") select x;

                foreach (var pair in results)
                {
                    Pair p = new Pair();
                    p.Name = pair.Descendants("name").First().Value;
                    p.Id = pair.Descendants("value").First().Value;

                    meals.Add(p);
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
            }
        }

        private void RetrieveMealFoods()
        {

            App ap = (App)App.Current;
            //Get Selected day
            int dayValue = Convert.ToInt32(day.SelectedPair.Id);

            String fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
              <entity name='dc_mealfood'>
                <attribute name='dc_foodid' />
                <attribute name='dc_portionsize' />
                <attribute name='dc_portiontypeid' />
                <attribute name='dc_mealfoodid' />
                <order attribute='dc_mealfoodid' descending='false' />
                <link-entity name='dc_meal' from='dc_mealid' to='dc_mealid' alias='dc_meal'>
                  <attribute name='dc_meal'/>
                  <link-entity name='dc_day' from='dc_dayid' to='dc_dayid' alias='dc_day'>
                    <attribute name='dc_day'/>  
                        <filter type='and'>
                            <condition attribute='dc_day' operator='eq' value='@DAY' />
                        </filter>
                    <link-entity name='dc_menu' from='dc_menuid' to='dc_menuid' alias='dc_menu'>
                    <attribute name='dc_menuid'/>  
                      <filter type='and'>
                        <condition attribute='dc_contactid' operator='eq' value='@CONTACTID' />
                        <condition attribute='statecode' operator='eq' value='0' />
                      </filter>
                    </link-entity>
                  </link-entity>
                </link-entity>
              </entity>
            </fetch>";

            fetchXml = fetchXml.Replace("@CONTACTID", ap.contactId);
            fetchXml = fetchXml.Replace("@DAY", dayValue.ToString());

            XElement orderXml = new XElement("ColumnOrder");
            orderXml.Add(new XElement("Column", "dc_foodid"));
            orderXml.Add(new XElement("Column", "dc_portiontypeid"));
            orderXml.Add(new XElement("Column", "dc_portionsize"));
            orderXml.Add(new XElement("Column", "dc_mealfoodid"));

            orderXml.Add(new XElement("Column", "dc_meal.dc_meal"));
            orderXml.Add(new XElement("Column", "dc_day.dc_day"));
            orderXml.Add(new XElement("Column", "dc_menu.dc_menuid"));


            try
            {
                /*
                busyIndicator.IsBusy = true;
                busyIndicator.Visibility = System.Windows.Visibility.Visible;
                */
                CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
                cms.RetrieveFetchXmlCompleted += new EventHandler<CrmSdk.RetrieveFetchXmlCompletedEventArgs>(cms_RetrieveFoodLikes);
                cms.RetrieveFetchXmlAsync(fetchXml, orderXml.ToString());
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
            }
        }
        private void cms_RetrieveFoodLikes(object sender, CrmSdk.RetrieveFetchXmlCompletedEventArgs e)
        {
            //buildGrid("dc_mealfood", dataGrid1, busyIndicator, e.Result, 1);
        }

        
        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

        }


        private void AddDislike_Click(object sender, RoutedEventArgs e)
        {
            App ap = (App)App.Current;
            XElement contactXml = new XElement("dc_fooddislike");

            XElement contact = new XElement("dc_contactid", ap.contactId);
            contactXml.Add(contact);

            XAttribute at = new XAttribute("entityname", "contact");
            contact.Add(at);

            CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
            cms.CreateUpdateCompleted += new EventHandler<CrmSdk.CreateUpdateCompletedEventArgs>(cms_CreateUpdateFoodLike);
            cms.CreateUpdateAsync(contactXml);

        }
        private void cms_CreateUpdateFoodLike(object sender, CrmSdk.CreateUpdateCompletedEventArgs e)
        {
            
        }

        private void food_KeyDown(object sender, KeyEventArgs e)
        {

            comboBox = (Controls.ComboBox)sender;

            String text = comboBox.MyAutoCompleteBox.Text;

            if (text.Length > 1)
            {
                comboBox.MyAutoCompleteBox.SelectionChanged -= MyAutoCompleteBox_SelectionChanged;
                comboBox.MyAutoCompleteBox.SelectionChanged += new SelectionChangedEventHandler(MyAutoCompleteBox_SelectionChanged);

                XElement orderXml = null;

                String fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
                  <entity name='dc_foods'>
                    <attribute name='dc_name' />
                    <attribute name='dc_foodsid' />
                    <order attribute='dc_name' descending='false' />
                    <filter type='and'>
                        <condition attribute='dc_name' value='%@TEXT%' operator='like'/>
                        <condition attribute='dc_canusefoodinmenu' value='1' operator='eq'/>
                    </filter>
                  </entity>
                </fetch>";

                fetchXml = fetchXml.Replace("@TEXT", text);

                orderXml = new XElement("ColumnOrder");
                orderXml.Add(new XElement("Column", "dc_name"));
                orderXml.Add(new XElement("Column", "dc_meal_componentid"));

                try
                {
                    CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
                    cms.RetrieveFetchXmlCompleted += new EventHandler<CrmSdk.RetrieveFetchXmlCompletedEventArgs>(cms_RetrieveFoods);
                    cms.RetrieveFetchXmlAsync(fetchXml, orderXml.ToString());
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                    MessageBox.Show(err.StackTrace);
                }
            }
        }
        private void cms_RetrieveFoods(object sender, CrmSdk.RetrieveFetchXmlCompletedEventArgs e)
        {
            String entityName = "dc_foods";

            var element = e.Result;

            List<Pair> data = new List<Pair>();
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
                        }
                        data.Add(new Pair(name, Id));
                    }
                    comboBox.MyAutoCompleteBox.ItemsSource = data;
                    comboBox.OpenDropdown(true);
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
            }

        }


        //For getting the contact retrieve to run after all the other call are completed
        public void MultipleAsyncRun(Action Operation)
        {
            Interlocked.Increment(ref PendingAsyncOperations);
            Operation();
        }
        public void FinalAsync(Action method)
        {
            FinalOperation = method;
        }

        public void AssignResultCheckforAllAsyncsDone<T>(AsyncCompletedEventArgs ea, T receivedData, string ErrorMessage)
            where T : class
        {
            bool valid = !((ea.Error != null) || (receivedData == null));

            if (valid == false)
            {
                if (0 == Interlocked.Exchange(ref AsyncErrorResource, 1))
                {
                    if (AsyncErrors == null)
                        AsyncErrors = new List<Exception>();

                    AsyncErrors.Add(ea.Error);

                    //Release the lock
                    Interlocked.Exchange(ref AsyncErrorResource, 0);
                }
            }
            else
            {
                //assignTo = receivedData;
            }

            Interlocked.Decrement(ref PendingAsyncOperations);
            if (PendingAsyncOperations == 0)
            {
                if (0 == Interlocked.Exchange(ref AsyncFinal, 1))
                {
                    FinalOperation();
                }

                Interlocked.Exchange(ref AsyncFinal, 0);
                Interlocked.Decrement(ref PendingAsyncOperations); // Move to -1
            }
        }

        private void newFood_Click(object sender, RoutedEventArgs e)
        {
            Row r = new Row();

            //Populate columnTypes  --  rowData.ColumnTypes.Add(xe.Name.LocalName, xe.Attribute("Type").Value);
            foreach (KeyValuePair<String, String> type in dataFoods.ColumnTypes)
            {
                if (type.Value.Equals("Lookup", StringComparison.OrdinalIgnoreCase))
                {
                    r[type.Key] = new Pair(String.Empty, Guid.Empty.ToString());
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
                else
                {
                    r[type.Key] = String.Empty;
                }
            }
            r.RowChanged = true;
            //data.Insert(0, r);
            try
            {
                dataFoods.Add(r);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
            }
        }


        private void saveMenu_Click(object sender, RoutedEventArgs e)
        {
            /*
            busyIndicator.IsBusy = true;
            App ap = (App)App.Current;
            Row r = null;
            //Find a row that needs saved
            foreach (Row row in dataFoods)
            {
                if (row.RowChanged)
                {
                    r = row;
                    break;
                }
            }
            if (r == null)
            {
                busyIndicator.IsBusy = false;
                return;//nothing to do
            }
            //Have row
            //create xml document and send to server to save

            XElement eventXml = new XElement("dc_mealfood");
            try
            {
                foreach (String columnName in dataFoods.ColumnTypes.Keys)
                {
                    //MessageBox.Show("ColumnName: " + columnName + ": type: " + dataFoods.ColumnTypes[columnName]);
                    if (dataFoods.ColumnTypes[columnName].Equals("DateTime", StringComparison.OrdinalIgnoreCase))
                    {
                        if (r[columnName] != null)
                        {
                            eventXml.Add(new XElement(columnName, ((DateTime)r[columnName])));
                        }
                    }
                    else if (dataFoods.ColumnTypes[columnName].Equals("Lookup", StringComparison.OrdinalIgnoreCase))
                    {
                        if (r[columnName].GetType() == typeof(Pair))
                        {
                            if (((Pair)r[columnName]).Id != "" && ((Pair)r[columnName]).Id != String.Empty)
                            {
                                eventXml.Add(new XElement(columnName, ((Pair)r[columnName]).Id));
                            }
                        }
                    }
                    else if (dataFoods.ColumnTypes[columnName].Equals("Picklist", StringComparison.OrdinalIgnoreCase))
                    {
                        eventXml.Add(new XElement(columnName, ((Pair)r[columnName]).Id));
                    }
                    else if (dataFoods.ColumnTypes[columnName].Equals("UniqueIdentifier", StringComparison.OrdinalIgnoreCase))
                    {
                        if ((String)r[columnName] != "" && (String)r[columnName] != String.Empty)
                        {
                            eventXml.Add(new XElement(columnName, ((String)r[columnName])));
                        }
                    }
                    else if (dataFoods.ColumnTypes[columnName].Equals("String", StringComparison.OrdinalIgnoreCase))
                    {
                        eventXml.Add(new XElement(columnName, (String)r[columnName]));
                    }
                    else if (dataFoods.ColumnTypes[columnName].Equals("Decimal", StringComparison.OrdinalIgnoreCase))
                    {
                        eventXml.Add(new XElement(columnName, (String)r[columnName]));
                    }
                    else if (dataFoods.ColumnTypes[columnName].Equals("Memo", StringComparison.OrdinalIgnoreCase))
                    {
                        eventXml.Add(new XElement(columnName, (String)r[columnName]));
                    }
                }
                eventXml.Add(new XElement("contactid", ap.contactId));

            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
            }
            r.RowChanged = false;

            //MessageBox.Show(eventXml.ToString());

            CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
            cms.CreateUpdateMealFoodCompleted += new EventHandler<CrmSdk.CreateUpdateMealFoodCompletedEventArgs>(cms_CreateUpdateGrid);
            cms.CreateUpdateMealFoodAsync(eventXml);
            */
        }
        private void cms_CreateUpdateGrid(object sender, CrmSdk.CreateUpdateMealFoodCompletedEventArgs e)
        {
            var results = from x in e.Result.Descendants("Success") select x;
            Guid Id = new Guid(e.Result.Value.ToString());

            //call back
            saveMenu_Click(sender, null);
        }

        private void Saturday_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            TabItem ti = (TabItem)sender;

        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            App ap = (App)App.Current;
            //Get Selected day
            //int dayValue = Convert.ToInt32(day.SelectedPair.Id);
            int dayValue = 0;
            TabControl tc = (TabControl)sender;

            TabItem ti = (TabItem)tc.SelectedItem;
            if (dayList != null && dayList.Count > 0)
            {
                dayValue = dayList[(String)ti.Tag];
            }
            String fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
              <entity name='dc_mealfood'>
                <attribute name='dc_foodid' />
                <attribute name='dc_portionsize' />
                <attribute name='dc_portiontypeid' />

                <attribute name='dc_fat' />
                <attribute name='dc_protein' />
                <attribute name='dc_carbohydrate' />
                <attribute name='dc_kcals' />

                <attribute name='dc_mealfoodid' />
                <order attribute='dc_mealfoodid' descending='false' />
                <link-entity name='dc_meal' from='dc_mealid' to='dc_mealid' alias='dc_meal'>
                  <attribute name='dc_meal'/>
                  <link-entity name='dc_day' from='dc_dayid' to='dc_dayid' alias='dc_day'>
                    <attribute name='dc_day'/>  
                        <filter type='and'>
                            <condition attribute='dc_day' operator='eq' value='@DAY' />
                        </filter>
                    <link-entity name='dc_menu' from='dc_menuid' to='dc_menuid' alias='dc_menu'>
                    <attribute name='dc_menuid'/>  
                      <filter type='and'>
                        <condition attribute='dc_contactid' operator='eq' value='@CONTACTID' />
                        <condition attribute='statecode' operator='eq' value='0' />
                      </filter>
                    </link-entity>
                  </link-entity>
                </link-entity>
              </entity>
            </fetch>";

            fetchXml = fetchXml.Replace("@CONTACTID", ap.contactId);
            fetchXml = fetchXml.Replace("@DAY", dayValue.ToString());

            XElement orderXml = new XElement("ColumnOrder");
            orderXml.Add(new XElement("Column", "dc_foodid"));
            orderXml.Add(new XElement("Column", "dc_portionsize"));

            orderXml.Add(new XElement("Column", "dc_portiontypeid"));
            
            orderXml.Add(new XElement("Column", "dc_fat"));
            orderXml.Add(new XElement("Column", "dc_protein"));
            orderXml.Add(new XElement("Column", "dc_carbohydrate"));
            orderXml.Add(new XElement("Column", "dc_kcals"));


            orderXml.Add(new XElement("Column", "dc_mealfoodid"));

            orderXml.Add(new XElement("Column", "dc_meal.dc_meal"));
            orderXml.Add(new XElement("Column", "dc_day.dc_day"));
            orderXml.Add(new XElement("Column", "dc_menu.dc_menuid"));


            try
            {
                /*
                busyIndicator.IsBusy = true;
                busyIndicator.Visibility = System.Windows.Visibility.Visible;
                */
                CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
                cms.RetrieveFetchXmlCompleted += new EventHandler<CrmSdk.RetrieveFetchXmlCompletedEventArgs>(cms_RetrieveFoodsForDay);
                cms.RetrieveFetchXmlAsync(fetchXml, orderXml.ToString());
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
            }
        }
        private void buildOutDataGrids(XElement element, DataGrid dataGrid)
        {
            String entityName = "dc_mealfood";

            try
            {
                if (element.Descendants(entityName).Count() > 0)
                {
                    XElement xEl = element.Descendants("columns").ToList()[0];//get first one

                    foreach (XElement xe in xEl.Elements())
                    {
                        if (!dataFoods.ColumnTypes.ContainsKey((xe.Name.LocalName)))
                        {
                            dataFoods.ColumnTypes.Add(xe.Name.LocalName, xe.Attribute("Type").Value);
                        }
                        if (xe.Name.LocalName.Equals("dc_menuid", StringComparison.OrdinalIgnoreCase) ||
                            xe.Name.LocalName.Equals("dc_meal", StringComparison.OrdinalIgnoreCase) ||
                            xe.Name.LocalName.Equals("dc_fat", StringComparison.OrdinalIgnoreCase) ||
                            xe.Name.LocalName.Equals("dc_protein", StringComparison.OrdinalIgnoreCase) ||
                            xe.Name.LocalName.Equals("dc_carbohydrate", StringComparison.OrdinalIgnoreCase) ||
                            xe.Name.LocalName.Equals("dc_kcals", StringComparison.OrdinalIgnoreCase) ||
                            xe.Name.LocalName.Equals("dc_day", StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }
                        if(xe.Name.LocalName.Equals("dc_portionsize", StringComparison.OrdinalIgnoreCase)) {

                            DataGridTemplateColumn dg = new DataGridTemplateColumn();
                            dg.Header = String.Empty;
                            dg.SortMemberPath = xe.Name.LocalName;
                            dg.CanUserSort = true;
                            StringBuilder CellETempPickList = new StringBuilder();
                            CellETempPickList.Append("<DataTemplate ");
                            CellETempPickList.Append("xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' ");
                            CellETempPickList.Append("xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' ");
                            CellETempPickList.Append("xmlns:basics2='clr-namespace:DynamicConnections.NutriStyle.MenuGenerator.Controls;");
                            CellETempPickList.Append("assembly=DynamicConnections.NutriStyle.MenuGenerator' >");

                            CellETempPickList.Append("<basics2:CustomTextBox TagName='" + xe.Name.LocalName + "' Foreground='#4C97CC' SelectedText='{Binding Path=Data, Converter={StaticResource rowConvertor}, ConverterParameter=" + xe.Name.LocalName + ", Mode=TwoWay}' />");
                            CellETempPickList.Append("</DataTemplate>");
                            dg.CellTemplate = (DataTemplate)XamlReader.Load(CellETempPickList.ToString());

                            dg.Width = new DataGridLength(35);
                            dataGrid.Columns.Add(dg);
                        }
                        else if (xe.Attribute("Type").Value.Equals("String", StringComparison.OrdinalIgnoreCase) ||
                            xe.Attribute("Type").Value.Equals("UniqueIdentifier", StringComparison.OrdinalIgnoreCase) ||
                            xe.Attribute("Type").Value.Equals("Decimal", StringComparison.OrdinalIgnoreCase) || (
                            xe.Attribute("Type").Value.Equals("Lookup", StringComparison.OrdinalIgnoreCase) &&
                            xe.Name.LocalName.Equals("dc_portiontypeid", StringComparison.OrdinalIgnoreCase)
                            ))
                        {
                            
                            DataGridTextColumn dg = new DataGridTextColumn();
                            if (xe.Name.LocalName.Equals("dc_portionsize", StringComparison.OrdinalIgnoreCase))
                            {
                                dg.Header = String.Empty;
                                dg.Width = new DataGridLength(35);
                            }
                            else
                            {
                                dg.Header = xe.Attribute("LabelName").Value;
                                dg.Width = new DataGridLength(100);
                            }

                            dg.CanUserSort = true;
                            dg.SortMemberPath = xe.Name.LocalName;
                            dg.Binding = new Binding("Data")
                            {
                                Converter = _rowIndexConverter,
                                ConverterParameter = xe.Name.LocalName
                            };
                            dg.Foreground = new SolidColorBrush(Color.FromArgb(255, 76, 151, 204));
                            //visable?
                            dg.Visibility = System.Windows.Visibility.Visible;
                            if (xe.Name.LocalName.Equals(entityName + "id", StringComparison.OrdinalIgnoreCase))
                            {
                                dg.Visibility = System.Windows.Visibility.Collapsed;
                            }
                            dg.IsReadOnly = false;

                            if (xe.Name.LocalName.Equals("dc_portiontypeid", StringComparison.OrdinalIgnoreCase))
                            {
                                dg.IsReadOnly = true;
                            }
                            dataGrid.Columns.Add(dg);

                            

                        }
                        else if (xe.Attribute("Type").Value.Equals("picklist", StringComparison.OrdinalIgnoreCase))
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

                            CellETempPickList.Append("<basics2:ComboBox TagName='" + xe.Name.LocalName + "' SelectedPair='{Binding Path=Data, Converter={StaticResource rowConvertor}, ConverterParameter=" + xe.Name.LocalName + ", Mode=TwoWay}' />");
                            CellETempPickList.Append("</DataTemplate>");
                            dg.CellEditingTemplate = (DataTemplate)XamlReader.Load(CellETempPickList.ToString());

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

                            CellETempPickList.Append("<basics2:ComboBox  TagName='" + xe.Name.LocalName + "' Foreground='#4C97CC' SelectedPair='{Binding Path=Data, Converter={StaticResource rowConvertor}, ConverterParameter=" + xe.Name.LocalName + ", Mode=TwoWay}' />");
                            CellETempPickList.Append("</DataTemplate>");
                            dg.CellEditingTemplate = (DataTemplate)XamlReader.Load(CellETempPickList.ToString());

                            if (xe.Name.LocalName.Equals("dc_portiontypeid", StringComparison.OrdinalIgnoreCase))
                            {
                                dg.IsReadOnly = true;
                            }
                            dataGrid.Columns.Add(dg);
                        }
                    }

                    //add image for delete
                    DataGridTemplateColumn dgImage = new DataGridTemplateColumn();
                    dgImage.Header = String.Empty;
                    //dgImage.SortMemberPath = xe.Name.LocalName;
                    dgImage.CanUserSort = false;

                    StringBuilder CellETemp = new StringBuilder();
                    CellETemp.Append("<DataTemplate ");
                    CellETemp.Append("xmlns='http://schemas.microsoft.com/winfx/");
                    CellETemp.Append("2006/xaml/presentation' ");
                    CellETemp.Append("xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' >");
                    CellETemp.Append("<Image Width='22' Height='22' Stretch='Fill' Name='Delete' Source='/DynamicConnections.NutriStyle.MenuGenerator;component/images/delete.png'/>");// MouseLeftButtonDown='ImageDelete_MouseLeftButtonDown'
                    CellETemp.Append("</DataTemplate>");
                    dgImage.CellEditingTemplate = (DataTemplate)XamlReader.Load(CellETemp.ToString());

                    dgImage.IsReadOnly = false;
                    dgImage.Width = new DataGridLength(50);
                    //visable?
                    dgImage.Visibility = System.Windows.Visibility.Visible;
                    //dataGrid1.Columns.Add(dgImage);

                    dataGrid.Columns.Add(dgImage);

                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
            }
        }
        private void cms_RetrieveFoodsForDay(object sender, CrmSdk.RetrieveFetchXmlCompletedEventArgs e)
        {
            String entityName = "dc_mealfood";
            XElement element = e.Result;

            TabItem ti = (TabItem)TabControl.SelectedItem;
            String day = (String)ti.Tag;

            Dictionary<String, DataGrid> dataGrids = new Dictionary<String, DataGrid>();
            if (!daysList.ContainsKey(day))
            {

                if (day.Equals("Saturday", StringComparison.OrdinalIgnoreCase))
                {
                    dataGrids.Add("Breakfast", dataGridSatBreakfast);
                    dataGrids.Add("Lunch", dataGridSatLunch);
                    dataGrids.Add("Dinner", dataGridSatDinner);
                }

                if (dataGrids.Count > 0)
                {
                    foreach (KeyValuePair<String, DataGrid> dg in dataGrids)
                    {
                        buildOutDataGrids(element, dg.Value);
                    }
                }
                daysList.Add(day, new Dictionary<String, SortableCollectionView>());
            }
            //Build out datagrids

            //collection is in place.  Add rows
            var rows = element.Descendants(entityName);
            foreach (var row in rows)
            {
                Row rowData = new Row();

                foreach (KeyValuePair<String, String> type in dataFoods.ColumnTypes)
                {
                    if (type.Value.Equals("Lookup", StringComparison.OrdinalIgnoreCase))
                    {
                        rowData[type.Key] = new Pair(String.Empty, Guid.Empty.ToString());
                    }
                    else if (type.Value.Equals("Picklist", StringComparison.OrdinalIgnoreCase))
                    {
                        rowData[type.Key] = "1";//new Pair(String.Empty, "-1");
                    }
                    else if (type.Value.Equals("DateTime", StringComparison.OrdinalIgnoreCase))
                    {
                        rowData[type.Key] = null;
                    }
                    else if (type.Value.Equals("Money", StringComparison.OrdinalIgnoreCase))
                    {
                        rowData[type.Key] = null;
                    }
                    else
                    {
                        rowData[type.Key] = String.Empty;
                    }
                }
                String mealName = String.Empty;//key for collection
                foreach (XElement xe in row.Elements())
                {

                    if (dataFoods.ColumnTypes.ContainsKey(xe.Name.LocalName) && dataFoods.ColumnTypes[xe.Name.LocalName].Equals("money", StringComparison.OrdinalIgnoreCase))
                    {
                        rowData[xe.Name.LocalName] = String.Format("{0:C2}", Convert.ToDecimal(xe.Value));
                    }
                    else if (dataFoods.ColumnTypes.ContainsKey(xe.Name.LocalName) && dataFoods.ColumnTypes[xe.Name.LocalName].Equals("lookup", StringComparison.OrdinalIgnoreCase))
                    {
                        //MessageBox.Show(xe.Name.LocalName + ":" + xe.Value + ":" + xe.Attribute("Id").Value);
                        rowData[xe.Name.LocalName] = new Pair(xe.Value, xe.Attribute("Id").Value);
                    }
                    else if (dataFoods.ColumnTypes.ContainsKey(xe.Name.LocalName) && dataFoods.ColumnTypes[xe.Name.LocalName].Equals("picklist", StringComparison.OrdinalIgnoreCase))
                    {
                        //MessageBox.Show(xe.Value+":"+xe.Attribute("Id").Value);
                        rowData[xe.Name.LocalName] = new Pair(xe.Value, xe.Attribute("Id").Value);
                    }
                    else
                    {
                        rowData[xe.Name.LocalName] = xe.Value;
                    }

                    //dc_mael
                    if (xe.Name.LocalName.Equals("dc_meal"))
                    {
                        mealName = xe.Value;
                    }
                }
                if (daysList[day].ContainsKey(mealName))
                {
                    daysList[day][mealName].Add(rowData);
                }
                else//need to add collection
                {
                    daysList[day].Add(mealName, new SortableCollectionView());
                    daysList[day][mealName].Add(rowData);
                }
            }
            if (dataGrids.Count > 0)
            {
                decimal kcals   = 0m;
                decimal protein = 0m;
                decimal fat     = 0m;
                decimal carbs   = 0m;

                foreach (KeyValuePair<String, DataGrid> dg in dataGrids)
                {
                    DataGrid dataGrid = (DataGrid)dg.Value;
                    String gridName = dg.Key;
                    dataGrid.ItemsSource = daysList[day][gridName];
                    SizeGrid(dataGrid);

                    //Set footer values
                    SetFooterValues(dataGrid, daysList[day][gridName]);

                    foreach (Row row in daysList[day][gridName])
                    {
                        kcals   += Convert.ToDecimal(row["dc_kcals"]);
                        protein += Convert.ToDecimal(row["dc_protein"]);
                        fat     += Convert.ToDecimal(row["dc_fat"]);
                        carbs   += Convert.ToDecimal(row["dc_carbohydrate"]);
                    }
                }
                //Setup the pie chart for this
                //Using pie chart name.  This is lame.  Use the Visual framework to find
                //Calculate up the kcals of protein, fat and carbs

                List<KeyValuePair<String, decimal>> list = new List<KeyValuePair<string, decimal>>();

                protein     = protein * (decimal)App.GramToKcalMultipler.Protein;
                fat         = fat * (decimal)App.GramToKcalMultipler.Fat;
                carbs       = carbs * (decimal)App.GramToKcalMultipler.Carbohydrate;

                list.Add(new KeyValuePair<string, decimal>("Protein: " + " - " + protein.ToString("#.##"), protein));
                list.Add(new KeyValuePair<string, decimal>("Fat " + " - " + fat.ToString("#.##"), fat));
                list.Add(new KeyValuePair<string, decimal>("Carbs" + " - " + carbs.ToString("#.##"), carbs));
                
                ((PieSeries)saturdayChart.Series[0]).ItemsSource = list;

                Rect pieBounds = new Rect();
                var center = new Point();
                var radius = 0d;
                ResourceDictionaryCollection palette = new ResourceDictionaryCollection();
                try
                {
                    EdgePanel plotArea = (EdgePanel)((PieSeries)saturdayChart.Series[0]).Parent;
                    
                    plotArea.Width = plotArea.ActualHeight;//makes it a sqaure
                    Style style = new Style(typeof(LegendItem));//((PieSeries)chart.Series[0]).LegendItemStyle;

                    style.Setters.Add(new Setter(WidthProperty, (544) - plotArea.Width - 95d));

                    ((PieSeries)saturdayChart.Series[0]).LegendItemStyle = style;

                    if (null != plotArea)
                    {
                        // Calculate the diameter of the pie (0.95 multiplier is from PieSeries implementation)
                        var diameter = Math.Min(plotArea.Width, plotArea.ActualHeight) * 0.95;
                        // Calculate the bounding rectangle of the pie
                        var leftTop = new Point((plotArea.Width - diameter) / 2, (plotArea.ActualHeight - diameter) / 2);
                        var rightBottom = new Point(leftTop.X + diameter, leftTop.Y + diameter);
                        pieBounds = new Rect(leftTop, rightBottom);
                        // Call the provided updater action for each PieDataPoint
                    }
                    double x = pieBounds.Left + ((pieBounds.Right - pieBounds.Left) / 2);
                    double y = pieBounds.Top + ((pieBounds.Bottom - (pieBounds.Top)) / 2);

                    x += 8;
                    //y -= 8;

                    center = new Point(x, y);

                    radius = (pieBounds.Right - pieBounds.Left) / 2;
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                    MessageBox.Show(err.StackTrace);
                }

                //set colors
                for (int x = 0; x < 3; x++)
                {
                    Style style2 = new Style(typeof(Control));
                    GradientStopCollection gsc = new GradientStopCollection();

                    GradientStop gs = new GradientStop();
                    GradientStop gs2 = new GradientStop();
                    GradientStop gs3 = new GradientStop();
                    if (x == 0)
                    {
                        gs.Color = Color.FromArgb(255, 242, 175, 122);
                        gs.Offset = 0.0;

                        gs2 = new GradientStop();
                        gs2.Color = Color.FromArgb(255, 200, 125, 75);
                        gs2.Offset = 0.8;

                        gs3 = new GradientStop();
                        gs3.Color = Color.FromArgb(255, 150, 75, 25);
                        gs3.Offset = 1.0;
                    }
                    else if (x == 1)
                    {
                        gs.Color = Color.FromArgb(255, 169, 207, 85);
                        gs.Offset = 0.0;

                        gs2 = new GradientStop();
                        gs2.Color = Color.FromArgb(255, 125, 150, 50);
                        gs2.Offset = 0.8;

                        gs3 = new GradientStop();
                        gs3.Color = Color.FromArgb(255, 75, 50, 5);
                        gs3.Offset = 1.0;
                    } 
                    else if (x == 2)
                    {
                        gs.Color = Color.FromArgb(255, 239, 235, 41);
                        gs.Offset = 0.0;

                        gs2 = new GradientStop();
                        gs2.Color = Color.FromArgb(255, 175, 175, 20);
                        gs2.Offset = 0.8;

                        gs3 = new GradientStop();
                        gs3.Color = Color.FromArgb(255, 125, 125, 5);
                        gs3.Offset = 1.0;
                    }
                    gsc.Add(gs);
                    gsc.Add(gs2);
                    gsc.Add(gs3);

                    RadialGradientBrush rgb = new RadialGradientBrush(gsc);
                    rgb.MappingMode = BrushMappingMode.Absolute;

                    rgb.Center = center;
                    rgb.GradientOrigin = center;
                    rgb.RadiusX = radius;
                    rgb.RadiusY = radius;

                    style2.Setters.Add(new Setter(BackgroundProperty, rgb));
                    style2.Setters.Add(new Setter(TemplateProperty, this.Resources["pi"]));

                    ResourceDictionary dictionary = new ResourceDictionary();
                    dictionary.Add("DataPointStyle", style2);
                    palette.Add(dictionary);

                }
                this.saturdayChart.Palette = palette;
                busyIndicatorSaturday.IsBusy = false;
            }
        }//end of method

        private void SetFooterValues(DataGrid dataGrid, SortableCollectionView rows)
        {

            StackPanel sp       = GetFooter(dataGrid);
            TextBlock kcalsTB   = (TextBlock)VisualTreeHelper.GetChild(sp, 0);
            TextBlock proteinTB = (TextBlock)VisualTreeHelper.GetChild(sp, 1);
            TextBlock fatTB     = (TextBlock)VisualTreeHelper.GetChild(sp, 2);
            TextBlock carbsTB   = (TextBlock)VisualTreeHelper.GetChild(sp, 3);
            
            double kcals    = 0d;
            double protein  = 0d;
            double fat      = 0d;
            double carbs    = 0d;
            
            foreach (Row row in rows)
            {
                kcals   += Convert.ToDouble(row["dc_kcals"]);
                protein += Convert.ToDouble(row["dc_protein"]);
                fat     += Convert.ToDouble(row["dc_fat"]);
                carbs   += Convert.ToDouble(row["dc_carbohydrate"]); 
            }
           
            kcalsTB.Text    = "Kcals: "+Math.Round(kcals, 2);
            proteinTB.Text = "Protein: " + Math.Round(protein, 2) + "g";
            fatTB.Text = "Fat: " + Math.Round(fat, 2) + "g";
            carbsTB.Text = "Carbs: " + Math.Round(carbs, 2) + "g";

            
        }
        private void SizeGrid(DataGrid dataGrid)
        {
            dataGrid.Visibility = System.Windows.Visibility.Visible;

            dataGrid.BorderThickness = new Thickness(1);

            // Space available to fill ( -18 Standard vScrollbar)
            double space_available = (LayoutRoot.ActualWidth - 18 - 80-35); //18 is width of scroll bar, 150 is width of menu 
            //figure out column types

            int count = 3;
            if (space_available > 0)
            {
                foreach (DataGridColumn dg_c in dataGrid.Columns)
                {
                    if (dg_c.Width.Value != 50 && dg_c.Width.Value != 35)
                    {
                        dg_c.Width = new DataGridLength((space_available / (dataGrid.Columns.Count - count)));
                    }
                }
            }
        }
        /// <summary>
        /// retruns the stack panel that will function as the datagrid footer
        /// </summary>
        /// <param name="dataGrid"></param>
        /// <returns></returns>
        private StackPanel GetFooter(DataGrid dataGrid)
        {
            //var parent = child.Parent as FrameworkElement;

            FrameworkElement parent = VisualTreeHelper.GetParent(dataGrid) as FrameworkElement;

            for (int x = 0; x < VisualTreeHelper.GetChildrenCount(parent); x++)
            {
                var child = VisualTreeHelper.GetChild(parent, x);
                if (child is DataGrid && child == dataGrid)
                {
                    return ((StackPanel)VisualTreeHelper.GetChild(parent, x + 1));
                }
            }

            return (null);
        }
        /// <summary>
        /// For adding food to a meal
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Find datagrid  -  This is needed to know what to add the food to.
            //stack panel -> parent -> child -> find matching stackpanel; find next sibling
            Grid sp                             = null;
            DataGrid dataGrid                   = null;
            SortableCollectionView dataFoods    = null;
            FrameworkElement child              = (FrameworkElement)sender;
            while(true) {
                FrameworkElement parent = VisualTreeHelper.GetParent(child) as FrameworkElement;

                if (parent is Grid)
                {
                    sp = (Grid)parent;
                    break;
                }
            }
            //found parent panel.  Now find grid
            FrameworkElement spParent = (FrameworkElement)VisualTreeHelper.GetParent(sp);
            for (int x = 0; x < VisualTreeHelper.GetChildrenCount(spParent); x++)
            {
                var child2 = VisualTreeHelper.GetChild(spParent, x);
                if (child2 is Grid && child2 == sp)
                {
                    dataGrid = ((DataGrid)VisualTreeHelper.GetChild(spParent, x + 1));
                }
            }
            dataFoods = (SortableCollectionView)dataGrid.ItemsSource;
            //Now add row to correct datagrid
            Row r = new Row();

            //Populate columnTypes  --  rowData.ColumnTypes.Add(xe.Name.LocalName, xe.Attribute("Type").Value);
            foreach (KeyValuePair<String, String> type in dataFoods.ColumnTypes)
            {
                if (type.Value.Equals("Lookup", StringComparison.OrdinalIgnoreCase))
                {
                    r[type.Key] = new Pair(String.Empty, Guid.Empty.ToString());
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
                else
                {
                    r[type.Key] = String.Empty;
                }
            }
            r.RowChanged = true;
            //data.Insert(0, r);
            try
            {
                dataFoods.Add(r);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
            }


        }
    }
}
