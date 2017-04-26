using System;
using System.Collections.Generic;
using System.Linq;

using System.Windows;
using System.Windows.Controls;

using System.Windows.Input;
using System.Windows.Media;

using DynamicConnections.NutriStyle.MenuGenerator.Engine.Helpers;
using System.Windows.Controls.DataVisualization.Charting;

using System.Xml.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows.Data;
using System.Text.RegularExpressions;
using System.Windows.Browser;
using DynamicConnections.NutriStyle.MenuGenerator.ChildWindows;
using DynamicConnections.NutriStyle.MenuGenerator.Pages;

namespace DynamicConnections.NutriStyle.MenuGenerator.Controls
{
    public partial class FoodLog : UserControl
    {
        private RowIndexConverter _rowIndexConverter = new RowIndexConverter();

        public Guid ContactId { get; set; }
        public String DayNumber { get; set; }
        Dictionary<Guid, Food> foods;
        bool loaded = false;
        Dictionary<String, SortableCollectionView> daysList = new Dictionary<String, SortableCollectionView>();
        Dictionary<String, int> dayList = new Dictionary<string, int>();
        List<DataGrid> dataGrids = new List<DataGrid>();
        DataGrid activeDataGrid;
        Guid menuId = Guid.Empty;
        Controls.ComboBoxWithValidation comboBox;
        SortableCollectionView dataFoods;
        Row selectedRow;
        DateTime day;
        //public static int dayNumber = 948170000;

        Dictionary<String, SortableCollectionView> mealList = new Dictionary<String, SortableCollectionView>();

        private MacroChart macroChart;

        public MacroChart MacroChart
        {
            get { return (macroChart); }
            set
            {
                macroChart = value;
            }
        }


        public FoodLog()
        {
            InitializeComponent();
            Setup();
            /*
            MainPage m = (MainPage)Application.Current.RootVisual;
            m.CollapseTips();
            m.SetMacroChart(macroChart);
            */
        }

        public FoodLog(Guid contactId) 
        {
            InitializeComponent();
            Setup();
            ContactId = contactId;
        }
        private void Setup()
        {
            day         = DateTime.Now;
            foods       = new Dictionary<Guid, Food>();
            ContactId   = Guid.Empty;
            dayList     = new Dictionary<string, int>();
            dataFoods   = new SortableCollectionView();

            dataGrids.Add(dataGridBreakfast);
            dataGrids.Add(dataGridMorningSnack);
            dataGrids.Add(dataGridLunch);
            dataGrids.Add(dataGridAfternoonSnack);
            dataGrids.Add(dataGridDinner);
            dataGrids.Add(dataGridEveningSnack);

            foreach (DataGrid dg in dataGrids)
            {
                dg.LoadingRow += new EventHandler<DataGridRowEventArgs>(dataGrid1_LoadingRow);
            }
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
                       
                        if (c.GetCellContent(e.Row).GetType() == typeof(Button))
                        {

                            Button ccb = c.GetCellContent(e.Row) as Button;
                            if (ccb.Name.Equals("delete", StringComparison.OrdinalIgnoreCase))
                            {
                                ccb.Click -= ccb_MouseLeftButtonUp;
                                ccb.Click += new RoutedEventHandler(ccb_MouseLeftButtonUp);
                            }
                            else if (ccb.Name.Equals("compare", StringComparison.OrdinalIgnoreCase))
                            {
                                ccb.Click -= CompareFoods;
                                ccb.Click += new RoutedEventHandler(CompareFoods);
                            }
                        }
                        else if (c.GetCellContent(e.Row).GetType() == typeof(Controls.CustomTextBox))
                        {
                            CustomTextBox ctb = c.GetCellContent(e.Row) as Controls.CustomTextBox;
                            if (ctb.TagName.Equals("dc_portionsize", StringComparison.OrdinalIgnoreCase))
                            {
                                ctb.TextBox.SelectionChanged -= ccb_LostFocus;
                                ctb.TextBox.SelectionChanged += new RoutedEventHandler(ccb_LostFocus); ;
                            }
                        }
                        else if (c.GetCellContent(e.Row).GetType() == typeof(Controls.ComboBoxWithValidation))
                        {
                            Controls.ComboBoxWithValidation ccb = c.GetCellContent(e.Row) as Controls.ComboBoxWithValidation;

                            if (ccb.TagName.Equals("dc_foodid", StringComparison.OrdinalIgnoreCase))
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
        /// 
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        private bool ValidateFood(Row r)
        {
            if (!String.IsNullOrEmpty((String)r["dc_portionsize"]) &&
                r["dc_foodid"] != null && !String.IsNullOrEmpty(((PairWithList)r["dc_foodid"]).Id))
            {
                return (true);
            }
            return (false);
        }

        /// <summary>
        /// calculates fats, protein and carbs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ccb_LostFocus(object sender, RoutedEventArgs e)
        {

            DataGrid dg = GetRoot((FrameworkElement)sender);
            if (dg == null)
            {
                dg = GetRoot((FrameworkElement)sender);
            }
            activeDataGrid = dg;
            Row selectedRow = null;
            if (sender.GetType() == typeof(TextBox))
            {
                TextBox tb = (TextBox)sender;
                selectedRow = (Row)tb.DataContext;
            }
            else if (sender.GetType() == typeof(ComboBoxWithValidation))
            {
                ComboBoxWithValidation cb = (ComboBoxWithValidation)sender;
                selectedRow = (Row)cb.DataContext;
            }

            //Row selectedRow = (Row);
            //get food
            if (selectedRow != null && foods.ContainsKey(new Guid(((PairWithList)selectedRow["dc_foodid"]).Id)))
            {
                Food f = foods[new Guid(((PairWithList)selectedRow["dc_foodid"]).Id)];
                //calculate 
                String strPortionSize = (String)selectedRow["dc_portionsize"];
                Regex digitsOnly = new Regex(@"/[^0-9-.]/g");
                strPortionSize = digitsOnly.Replace(strPortionSize, "");

                if (!String.IsNullOrEmpty(strPortionSize) && !strPortionSize.Equals("."))
                {
                    decimal portionSize = Convert.ToDecimal(strPortionSize);
                    decimal kcalMultiplier = portionSize / f.PortionSize;

                    selectedRow["dc_fat"]           = (f.Fat * kcalMultiplier).ToString();
                    selectedRow["dc_protein"]       = (f.Protein * kcalMultiplier).ToString();
                    selectedRow["dc_carbohydrate"]  = (f.Carbohydrate * kcalMultiplier).ToString();
                    selectedRow["dc_alcohol"]       = (f.Alcohol * kcalMultiplier).ToString();
                    selectedRow.RowChanged          = true;
                    SetFooterValues(activeDataGrid, (SortableCollectionView)activeDataGrid.ItemsSource);

                    CalculatePieChart();
                }
            }
        }

        /// <summary>
        /// Save the meal foods
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            App ap = (App)App.Current;
            Row r = null;

            List<DataGrid> dataGrids = new List<DataGrid>();

            dataGrids.Add(dataGridBreakfast);
            dataGrids.Add(dataGridMorningSnack);
            dataGrids.Add(dataGridLunch);
            dataGrids.Add(dataGridAfternoonSnack);
            dataGrids.Add(dataGridDinner);
            dataGrids.Add(dataGridEveningSnack);

            String entityName = "dc_foodlog";

            foreach (DataGrid dg in dataGrids)
            {
                SortableCollectionView data = (SortableCollectionView)dg.ItemsSource;
                //get row

                foreach (Row row in data)
                {
                    if (row.RowChanged)
                    {
                        r = row;
                        break;
                    }
                }
                if (r != null)
                {
                    break;
                }
            }
            selectedRow = r;
            if (r == null)
            {
                busyIndicator.IsBusy = false;
                return;//nothing to do
            }
            if (true)
            {//if (ValidateRow(r))
                busyIndicator.IsBusy = true;
                //Have row
                //create xml document and send to server to save

                XElement eventXml = new XElement("dc_foodlog");

                foreach (String columnName in dataFoods.ColumnTypes.Keys)
                {
                    if (dataFoods.ColumnTypes[columnName].Equals("DateTime", StringComparison.OrdinalIgnoreCase))
                    {
                        if (r[columnName] != null)
                        {
                            eventXml.Add(new XElement(columnName, ((DateTime)r[columnName])));
                        }
                    }
                    else if (dataFoods.ColumnTypes[columnName].Equals("Lookup", StringComparison.OrdinalIgnoreCase))
                    {
                        if (r[columnName] != null && r[columnName].GetType() == typeof(PairWithList))
                        {
                            if (((PairWithList)r[columnName]).Id != "" && ((PairWithList)r[columnName]).Id != String.Empty)
                            {
                                XElement lookupNode = new XElement(new XElement(columnName, ((PairWithList)r[columnName]).Id));
                                eventXml.Add(lookupNode);

                                XAttribute at = new XAttribute("entityname", dataFoods.EntityTypes[columnName]);
                                lookupNode.Add(at);
                            }
                        }
                    }
                    else if (dataFoods.ColumnTypes[columnName].Equals("Picklist", StringComparison.OrdinalIgnoreCase))
                    {
                        eventXml.Add(new XElement(columnName, ((PairWithList)r[columnName]).Id));
                    }
                    else if (dataFoods.ColumnTypes[columnName].Equals("UniqueIdentifier", StringComparison.OrdinalIgnoreCase))
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
                    else if (dataFoods.ColumnTypes[columnName].Equals("String", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!String.IsNullOrEmpty((String)r[columnName]))
                        {
                            eventXml.Add(new XElement(columnName, (String)r[columnName]));
                        }
                    }
                    else if (dataFoods.ColumnTypes[columnName].Equals("Decimal", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!String.IsNullOrEmpty((String)r[columnName]))
                        {
                            eventXml.Add(new XElement(columnName, (String)r[columnName]));
                        }
                    }
                    else if (dataFoods.ColumnTypes[columnName].Equals("Memo", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!String.IsNullOrEmpty((String)r[columnName]))
                        {
                            eventXml.Add(new XElement(columnName, (String)r[columnName]));
                        }
                    }
                }
                XElement lookupNodeContact = new XElement(new XElement("dc_contactid", this.ContactId));
                eventXml.Add(lookupNodeContact);

                XAttribute atContact = new XAttribute("entityname", "contact");
                lookupNodeContact.Add(atContact);

                eventXml.Add(new XElement("dc_date", new DateTime(this.day.Year, this.day.Month, this.day.Day)));

                if (r["dc_foodid"] != null)
                {
                    eventXml.Add(new XElement("dc_name", ((PairWithList)r["dc_foodid"]).Name));
                }

                r.RowChanged = false;

                CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
                cms.CreateUpdateCompleted += new EventHandler<CrmSdk.CreateUpdateCompletedEventArgs>(cms_CreateUpdateGrid);
                cms.CreateUpdateAsync(eventXml, true);
                busyIndicator.IsBusy = false;
            }
        }
        private void cms_CreateUpdateGrid(object sender, CrmSdk.CreateUpdateCompletedEventArgs e)
        {

            //call back
            var results = from x in e.Result.Descendants("Success") select x;
            Guid Id = new Guid(e.Result.Value.ToString());
            selectedRow.RowChanged = false;
            //set id
            selectedRow["dc_foodlogid"] = Id.ToString();
            //call back
            Save_Click(sender, null);
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
            if (activeDataGrid == null)
            {
                activeDataGrid = GetRoot((FrameworkElement)sender);
            }
            if (activeDataGrid.SelectedItem != null)
            {
                Row r = (Row)activeDataGrid.SelectedItem;
                r.RowChanged = true;

                AutoCompleteBox cb = (AutoCompleteBox)sender;
                //Get parent datagrid

                if (cb.SelectedItem != null && !String.IsNullOrEmpty(cb.Text))
                {
                    PairWithList p = (PairWithList)cb.SelectedItem;
                    //Retreive the portion size
                    String fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                      <entity name='dc_foods'>
                        <attribute name='dc_portiontypeid' />
                        <attribute name='dc_portion_amount' />
                        <order attribute='dc_foodsid' descending='false' />
                              <filter type='and'>
                                <condition attribute='dc_foodsid' operator='eq' value='@FOODID' />
                              </filter>
                      </entity>
                    </fetch>";

                    fetchXml = fetchXml.Replace("@FOODID", p.Id);

                    XElement orderXml = new XElement("ColumnOrder");
                    orderXml.Add(new XElement("Column", "dc_portiontype"));
                    orderXml.Add(new XElement("Column", "dc_portion_amount"));
                    orderXml.Add(new XElement("Column", "dc_foodsid"));

                    r["dc_portionsize"] = foods[new Guid(p.Id)].PortionSize.ToString();
                    r["dc_portiontypeid"] = foods[new Guid(p.Id)].PortionType;
                    r["dc_foods.dc_recipefood"] = foods[new Guid(p.Id)].IsRecipe;
                    r["dc_portion_types.dc_name"] = foods[new Guid(p.Id)].PortionTypeName;
                    r["dc_portion_types.dc_abbreviation"] = foods[new Guid(p.Id)].PortionTypeAbbreviation;
                }
            }
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
                DataGrid dataGrid = GetRoot((FrameworkElement)sender);
                if (dataGrid == null)
                {
                    dataGrid = GetRoot((FrameworkElement)sender);
                }
                SortableCollectionView data = (SortableCollectionView)dataGrid.ItemsSource;

                Image img = sender as Image;
                String Id = (String)((Row)dataGrid.SelectedItem)["dc_foodlogid"];
                Row r = (Row)dataGrid.SelectedItem;
                if (!String.IsNullOrEmpty(Id))
                {
                    XElement deleteXml = new XElement("dc_foodlog", new XElement("id", Id));

                    CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
                    cms.DeleteCompleted += new EventHandler<CrmSdk.DeleteCompletedEventArgs>(cms_Delete);
                    cms.DeleteAsync(deleteXml);
                }
                data.Remove(r);
                dataGrid.ItemsSource = data;
                CalculatePieChart();
                SetFooterValues(dataGrid, (SortableCollectionView)dataGrid.ItemsSource);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
            }
        }
        /// <summary>
        /// Open compare foods child window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CompareFoods(object sender, RoutedEventArgs e)
        {
            try
            {
                DataGrid dataGrid = GetRoot((FrameworkElement)sender);
                if (dataGrid == null)
                {
                    dataGrid = GetRoot((FrameworkElement)sender);
                }
                SortableCollectionView data = (SortableCollectionView)dataGrid.ItemsSource;

                Image img = sender as Image;
                String foodId = ((PairWithList)((Row)dataGrid.SelectedItem)["dc_foodid"]).Id;
                String foodName = ((PairWithList)((Row)dataGrid.SelectedItem)["dc_foodid"]).Name;
                String portionSize = (String)((Row)dataGrid.SelectedItem)["dc_portionsize"];

                Row r = (Row)dataGrid.SelectedItem;
                if (!String.IsNullOrEmpty(foodId) && !String.IsNullOrEmpty(portionSize))
                {
                    CompareFoods cf = new CompareFoods(new Guid(foodId), Convert.ToDecimal(portionSize), foodName);
                    cf.Show();
                    activeDataGrid = dataGrid;
                    cf.SwapClicked += new EventHandler(cf_SwapClicked);
                }
                
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
            }
        }

        void cf_SwapClicked(object sender, EventArgs e)
        {
            //Swap food
            String[] array = (String[])sender;
            String Id = array[0];
            String name = array[1];
            String portionSize = array[2];
            String fat = array[3];
            String carbs = array[4];
            String protein = array[5];

            DataGrid dataGrid = activeDataGrid;
            
            SortableCollectionView data = (SortableCollectionView)dataGrid.ItemsSource;

            ((Row)dataGrid.SelectedItem)["dc_foodid"]       = new PairWithList(name, Id, String.Empty, new List<PairWithList>());
            ((Row)dataGrid.SelectedItem)["dc_portionsize"]  = portionSize;
            ((Row)dataGrid.SelectedItem)["dc_fat"]          = fat;
            ((Row)dataGrid.SelectedItem)["dc_carbohydrate"] = carbs;
            ((Row)dataGrid.SelectedItem)["dc_protein"]      = protein;
            Row r = (Row)dataGrid.SelectedItem;
            r.RowChanged = true;//for saving data
            CalculatePieChart();
            SetFooterValues(dataGrid, (SortableCollectionView)dataGrid.ItemsSource);

        }
        private DataGrid GetRoot(FrameworkElement child)
        {

            //var parent = child.Parent as FrameworkElement;

            FrameworkElement parent = VisualTreeHelper.GetParent(child) as FrameworkElement;

            if (parent == null)
            {
                return (null);
            }
            else if (parent is DataGrid)
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

        /// <summary>
        /// Search for matching list of foods
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void food_KeyDown(object sender, KeyEventArgs e)
        {

            comboBox = (Controls.ComboBoxWithValidation)sender;

            String text = comboBox.MyAutoCompleteBox.Text;

            if (String.IsNullOrEmpty(comboBox.SearchText) || comboBox.SearchText.Length >= text.Length)
            {
                if (text.Length > 1 && (e.Key != Key.Down && e.Key != Key.Up && e.Key != Key.Delete && e.Key != Key.Back))
                {
                    //The text has to be different than what has already been searched on.  'cof' should not trigger a new search if the org search was 'co'
                    if (String.IsNullOrEmpty(comboBox.SearchText) || (text.Length != comboBox.SearchText.Length) || !text.Equals(comboBox.SearchText, StringComparison.OrdinalIgnoreCase))
                    {
                        comboBox.SearchText = text;


                        comboBox.IsBusy = true;

                        comboBox.MyAutoCompleteBox.SelectionChanged -= MyAutoCompleteBox_SelectionChanged;
                        comboBox.MyAutoCompleteBox.SelectionChanged += new SelectionChangedEventHandler(MyAutoCompleteBox_SelectionChanged);
                        
                        try
                        {
                            CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
                            cms.RetrieveFoodsCompleted += new EventHandler<CrmSdk.RetrieveFoodsCompletedEventArgs>(cms_RetrieveFoods);
                            cms.RetrieveFoodsAsync(General.ContactId(), text);
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
        private void cms_RetrieveFoods(object sender, CrmSdk.RetrieveFoodsCompletedEventArgs e)
        {
            String entityName = "dc_foods";

            var element = e.Result;

            List<PairWithList> data = new List<PairWithList>();
            try
            {
                if (element.Descendants(entityName).Count() > 0)
                {
                    //Bind data
                    var rows = element.Descendants(entityName);
                    foreach (var row in rows)
                    {
                        Row rowData = new Row();

                        String name     = String.Empty;
                        String Id       = String.Empty;
                        decimal fat     = 0m;
                        decimal alcohol = 0m;
                        decimal protein = 0m;
                        decimal carbs   = 0m;
                        decimal orgPortionSize = 0m;
                        bool recipe     = false;
                        PairWithList portionType = new PairWithList();
                        String portionTypeName = String.Empty;
                        String portionTypeAbbreviation = String.Empty;

                        foreach (XElement xe in row.Elements())
                        {
                            if (xe.Name.LocalName.Equals("dc_name", StringComparison.OrdinalIgnoreCase))
                            {
                                name = xe.Value;
                            }
                            else if (xe.Name.LocalName.Equals("dc_recipefood", StringComparison.OrdinalIgnoreCase))
                            {
                                recipe = Convert.ToBoolean(xe.Value);
                            }
                            else if (xe.Name.LocalName.Equals(entityName + "id", StringComparison.OrdinalIgnoreCase))
                            {
                                Id = xe.Value;
                            }
                            else if (xe.Name.LocalName.Equals("dc_food_nutrients.dc_fat", StringComparison.OrdinalIgnoreCase))
                            {
                                fat = Convert.ToDecimal(xe.Value);
                            }
                            else if (xe.Name.LocalName.Equals("dc_food_nutrients.dc_protein", StringComparison.OrdinalIgnoreCase))
                            {
                                protein = Convert.ToDecimal(xe.Value);
                            }
                            else if (xe.Name.LocalName.Equals("dc_food_nutrients.dc_carbohydrate", StringComparison.OrdinalIgnoreCase))
                            {
                                carbs = Convert.ToDecimal(xe.Value);
                            }
                            else if (xe.Name.LocalName.Equals("dc_food_nutrients.dc_alcohol", StringComparison.OrdinalIgnoreCase))
                            {
                                alcohol = Convert.ToDecimal(xe.Value);
                            }
                            else if (xe.Name.LocalName.Equals("dc_portion_amount", StringComparison.OrdinalIgnoreCase))
                            {
                                orgPortionSize = Convert.ToDecimal(xe.Value);
                            }
                            else if (xe.Name.LocalName.Equals("dc_portiontypeid", StringComparison.OrdinalIgnoreCase))
                            {
                                portionType = new PairWithList(xe.Value, xe.Attribute("Id").Value);
                            }
                            else if (xe.Name.LocalName.Equals("dc_portion_types.dc_name", StringComparison.OrdinalIgnoreCase))
                            {
                                portionTypeName = xe.Value;
                            }
                            else if (xe.Name.LocalName.Equals("dc_portion_types.dc_abbreviation", StringComparison.OrdinalIgnoreCase))
                            {
                                portionTypeAbbreviation = xe.Value;
                            }

                        }
                        data.Add(new PairWithList(name, Id));
                        if (!foods.ContainsKey(new Guid(Id)))
                        {
                            Food f          = new Food();
                            f.Fat           = fat;
                            f.Protein       = protein;
                            f.Carbohydrate  = carbs;
                            f.Alcohol       = alcohol;
                            f.PortionSize   = orgPortionSize;
                            f.PortionType   = portionType;
                            f.IsRecipe      = recipe;
                            f.PortionTypeName = portionTypeName;
                            f.PortionTypeAbbreviation = portionTypeAbbreviation;
                            foods.Add(new Guid(Id), f);
                        }
                    }
                    //comboBox.MyAutoCompleteBox.ItemsSource = data;
                    comboBox.SelectedPair = new PairWithList(string.Empty, String.Empty, String.Empty, data);
                    comboBox.IsBusy = false;
                    comboBox.OpenDropdown(true);
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
            }
        }

        /// <summary>
        /// Load the Food Log for the passed in day
        /// </summary>
        /// <param name="date"></param>
        public void LoadDay(DateTime date, Guid contactId)
        {
            if (!loaded)
            {
                busyIndicator.IsBusy = true;
                
                macroChart.IsBusy = true;
                loaded = true;
                //Make sure day has not been loaded

                var parent = this.Parent;
                TabItem ti = (TabItem)parent;// null;//(TabItem)TabControl.SelectedItem;

                String day = (String)ti.Tag;
                if (App.Current.GetType() == typeof(DynamicConnections.NutriStyle.MenuGenerator.App))
                {
                    this.day = date;
                    this.ContactId = contactId;
                }

                String fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                  <entity name='dc_foodlog'>
                    <attribute name='dc_foodlogid' />
                    <attribute name='dc_portiontypeid' />
                    <attribute name='dc_foodid' />
                    <attribute name='dc_portionsize' />
                
                    <attribute name='dc_fat' />
                    <attribute name='dc_protein' />
                    <attribute name='dc_carbohydrate' />
                    <attribute name='dc_alcohol' />
                    <attribute name='dc_meal' />
                    
                    <filter type='and'>
                      <condition attribute='statecode' operator='eq' value='0' />
                      <condition attribute='dc_contactid' operator='eq' value='@CONTACTID' />
                      <condition attribute='dc_date' operator='on' value='@DATETIME' />
                    </filter>
                        <link-entity name='dc_portion_types' from='dc_portion_typesid' to='dc_portiontypeid' alias='dc_portion_types'>
                          <attribute name='dc_name' />
                          <attribute name='dc_abbreviation' />
                        </link-entity>
                    <link-entity name='dc_foodlogday' from='dc_foodlogdayid' to='dc_foodlogdayid' alias='dc_foodlogday'>
                        <attribute name='dc_menuid' />
                      <filter type='and'>
                        <condition attribute='dc_contactid' operator='eq' value='@CONTACTID' />
                        <condition attribute='dc_date' operator='on' value='@DATETIME' />
                      </filter>

                      <link-entity name='dc_menu' alias='dc_menu' to='dc_menuid' from='dc_menuid'>  
                          <filter type='and'> 
                            <condition attribute='dc_primarymenu' value='1' operator='eq'/> 
                          </filter>  
                      </link-entity>
                    </link-entity>
                    <link-entity name='dc_foods' from='dc_foodsid' to='dc_foodid' alias='dc_foods'>
                        <attribute name='dc_recipefood' />
                    </link-entity>
                  </entity>
                </fetch>";

                fetchXml = fetchXml.Replace("@CONTACTID", contactId.ToString());
                fetchXml = fetchXml.Replace("@DATETIME", date.ToString("MM/dd/yyyy"));
                
                XElement orderXml = new XElement("ColumnOrder");
                orderXml.Add(new XElement("Column", "dc_foodid"));
                orderXml.Add(new XElement("Column", "dc_portionsize"));
                orderXml.Add(new XElement("Column", "dc_portiontypeid"));

                orderXml.Add(new XElement("Column", "dc_fat"));
                orderXml.Add(new XElement("Column", "dc_protein"));
                orderXml.Add(new XElement("Column", "dc_carbohydrate"));
                orderXml.Add(new XElement("Column", "dc_alcohol"));

                orderXml.Add(new XElement("Column", "dc_meal"));
                orderXml.Add(new XElement("Column", "dc_foods.dc_recipefood"));

                orderXml.Add(new XElement("Column", "dc_foodlogid"));

                orderXml.Add(new XElement("Column", "dc_portion_types.dc_name"));
                orderXml.Add(new XElement("Column", "dc_portion_types.dc_abbreviation"));

                orderXml.Add(new XElement("Column", "dc_foodlogday.dc_menuid"));

                try
                {
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
            else
            {
                CalculatePieChart();//this will force the correct population of the graph
            }
        }
        private void buildOutDataGrids(XElement element, DataGrid dataGrid)
        {
            String entityName = "dc_foodlog";
            if (element != null)
            {
                try
                {
                    if (element.Descendants("columns").Count() > 0)
                    {
                        XElement xEl = element.Descendants("columns").ToList()[0];//get first one

                        foreach (XElement xe in xEl.Elements())
                        {
                            if (!dataFoods.ColumnTypes.ContainsKey((xe.Name.LocalName)))
                            {
                                dataFoods.ColumnTypes.Add(xe.Name.LocalName, xe.Attribute("Type").Value);
                            }

                            if (!dataFoods.EntityTypes.ContainsKey((xe.Name.LocalName)))
                            {
                                dataFoods.EntityTypes.Add(xe.Name.LocalName, xe.Attribute("Entity").Value);
                            }

                            if (xe.Name.LocalName.Equals("dc_menuid", StringComparison.OrdinalIgnoreCase) ||
                                xe.Name.LocalName.Equals("dc_meal", StringComparison.OrdinalIgnoreCase) ||
                                xe.Name.LocalName.Equals("dc_fat", StringComparison.OrdinalIgnoreCase) ||
                                xe.Name.LocalName.Equals("dc_protein", StringComparison.OrdinalIgnoreCase) ||
                                xe.Name.LocalName.Equals("dc_alcohol", StringComparison.OrdinalIgnoreCase) ||
                                xe.Name.LocalName.Equals("dc_carbohydrate", StringComparison.OrdinalIgnoreCase) ||
                                xe.Name.LocalName.Equals("dc_kcals", StringComparison.OrdinalIgnoreCase) ||
                                xe.Name.LocalName.Equals("dc_day", StringComparison.OrdinalIgnoreCase) ||
                                xe.Name.LocalName.Equals("dc_mealid", StringComparison.OrdinalIgnoreCase) ||
                                xe.Name.LocalName.Equals("dc_portion_types.dc_name", StringComparison.OrdinalIgnoreCase) ||
                                xe.Name.LocalName.Equals("dc_foodlogday.dc_menuid", StringComparison.OrdinalIgnoreCase) ||
                                xe.Name.LocalName.Equals("dc_portiontypeid", StringComparison.OrdinalIgnoreCase))
                            {
                                continue;
                            }
                            if (xe.Name.LocalName.Equals("dc_portionsize", StringComparison.OrdinalIgnoreCase))
                            {

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

                                dg.Width = new DataGridLength(40);
                                dataGrid.Columns.Add(dg);
                            }
                            else if ((xe.Attribute("Type").Value.Equals("String", StringComparison.OrdinalIgnoreCase) ||
                                xe.Attribute("Type").Value.Equals("UniqueIdentifier", StringComparison.OrdinalIgnoreCase) ||
                                xe.Attribute("Type").Value.Equals("Decimal", StringComparison.OrdinalIgnoreCase)) &&
                                !(xe.Name.LocalName.Equals("dc_portion_types.dc_abbreviation", StringComparison.OrdinalIgnoreCase)))
                            {


                                DataGridTextColumn dg = new DataGridTextColumn();
                                if (xe.Name.LocalName.Equals("dc_portionsize", StringComparison.OrdinalIgnoreCase))
                                {
                                    dg.Header = String.Empty;
                                    dg.Width = new DataGridLength(40);
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
                                //dg.Foreground = new SolidColorBrush(Color.FromArgb(255, 76, 151, 204));
                                //visable?
                                dg.Visibility = System.Windows.Visibility.Visible;
                                if (xe.Name.LocalName.Equals(entityName + "id", StringComparison.OrdinalIgnoreCase))
                                {
                                    dg.Visibility = System.Windows.Visibility.Collapsed;
                                }
                                dg.IsReadOnly = false;

                                if (xe.Name.LocalName.Equals("dc_portion_types.dc_abbreviation", StringComparison.OrdinalIgnoreCase))
                                {
                                    dg.IsReadOnly = true;
                                    dg.Width = new DataGridLength(40);
                                }
                                dataGrid.Columns.Add(dg);

                            }
                            else if (xe.Attribute("Type").Value.Equals("String", StringComparison.OrdinalIgnoreCase) &&
                                xe.Name.LocalName.Equals("dc_portion_types.dc_abbreviation", StringComparison.OrdinalIgnoreCase))
                            {


                                DataGridTemplateColumn portionType = new DataGridTemplateColumn();
                                portionType.Header = String.Empty;
                                //dgImage.SortMemberPath = xe.Name.LocalName;
                                portionType.CanUserSort = false;

                                StringBuilder CellPortionType = new StringBuilder();
                                CellPortionType.Append("<DataTemplate ");
                                CellPortionType.Append("xmlns='http://schemas.microsoft.com/winfx/");
                                CellPortionType.Append("2006/xaml/presentation' ");
                                CellPortionType.Append("xmlns:sdk='http://schemas.microsoft.com/winfx/2006/xaml/presentation/sdk' ");
                                CellPortionType.Append("xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' >");
                                CellPortionType.Append("<sdk:Label Margin='2,0,0,0' ToolTipService.ToolTip='{Binding Path=Data, Converter={StaticResource rowConvertor}, ConverterParameter=dc_portion_types.dc_name, Mode=TwoWay}' Content='{Binding Path=Data, Converter={StaticResource rowConvertor}, ConverterParameter=" + xe.Name.LocalName + ", Mode=TwoWay}'/>");
                                CellPortionType.Append("</DataTemplate>");
                                portionType.CellEditingTemplate = (DataTemplate)XamlReader.Load(CellPortionType.ToString());

                                portionType.IsReadOnly = false;
                                portionType.Width = new DataGridLength(40);
                                //visable?
                                portionType.Visibility = System.Windows.Visibility.Visible;
                                dataGrid.Columns.Add(portionType);
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

                                CellETempPickList.Append("<basics2:ComboBoxWithValidataion TagName='" + xe.Name.LocalName + "' SelectedPair='{Binding Path=Data, Converter={StaticResource rowConvertor}, ConverterParameter=" + xe.Name.LocalName + ", Mode=TwoWay}' />");
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

                                CellETempPickList.Append("<basics2:ComboBoxWithValidation Watermark='Type in Name' TagName='" + xe.Name.LocalName + "' Foreground='#4C97CC' IsRecipe='{Binding Path=Data, Converter={StaticResource rowConvertor}, ConverterParameter=dc_foods.dc_recipefood, Mode=TwoWay}' SelectedPair='{Binding Path=Data, Converter={StaticResource rowConvertor}, ConverterParameter=" + xe.Name.LocalName + ", Mode=TwoWay}' />");
                                CellETempPickList.Append("</DataTemplate>");
                                dg.CellEditingTemplate = (DataTemplate)XamlReader.Load(CellETempPickList.ToString());

                                if (xe.Name.LocalName.Equals("dc_portiontypeid", StringComparison.OrdinalIgnoreCase))
                                {
                                    dg.IsReadOnly = true;
                                    dg.Width = new DataGridLength(100);
                                }
                                dataGrid.Columns.Add(dg);
                            }
                        }
                        //Add image for compare foods
                        DataGridTemplateColumn dgImage = new DataGridTemplateColumn();
                        dgImage.Header = String.Empty;
                        //dgImage.SortMemberPath = xe.Name.LocalName;
                        dgImage.CanUserSort = false;

                        StringBuilder CellETemp = new StringBuilder();
                        CellETemp.Append("<DataTemplate ");
                        CellETemp.Append("xmlns='http://schemas.microsoft.com/winfx/");
                        CellETemp.Append("2006/xaml/presentation' ");
                        CellETemp.Append("xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' >");
                        CellETemp.Append("<Button Cursor='Hand' ToolTipService.ToolTip='Compare Foods' Width='24' Height='24' Name='compare'>");
                        CellETemp.Append("<Image Stretch='Fill' Name='Compare Foods' Source='/DynamicConnections.NutriStyle.MenuGenerator;component/images/scales.png'/>");
                        CellETemp.Append("</Button>");
                        CellETemp.Append("</DataTemplate>");
                        dgImage.CellEditingTemplate = (DataTemplate)XamlReader.Load(CellETemp.ToString());

                        dgImage.IsReadOnly = false;
                        dgImage.Width = new DataGridLength(24);
                        //visable?
                        dgImage.Visibility = System.Windows.Visibility.Visible;
                        //dataGrid1.Columns.Add(dgImage);

                        dataGrid.Columns.Add(dgImage);

                        //add image for delete
                        dgImage = new DataGridTemplateColumn();
                        dgImage.Header = "Del";
                        //dgImage.SortMemberPath = xe.Name.LocalName;
                        dgImage.CanUserSort = false;

                        CellETemp = new StringBuilder();
                        CellETemp.Append("<DataTemplate ");
                        CellETemp.Append("xmlns='http://schemas.microsoft.com/winfx/");
                        CellETemp.Append("2006/xaml/presentation' ");
                        CellETemp.Append("xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' >");
                        CellETemp.Append("<Button Cursor='Hand' ToolTipService.ToolTip='Remove From Food Log' Width='26' Height='26' Name='delete'>");
                        CellETemp.Append("<Image  Stretch='Fill' Name='Delete' Source='/DynamicConnections.NutriStyle.MenuGenerator;component/images/delete.png'/>");// MouseLeftButtonDown='ImageDelete_MouseLeftButtonDown'
                        CellETemp.Append("</Button>");
                        CellETemp.Append("</DataTemplate>");
                        dgImage.CellEditingTemplate = (DataTemplate)XamlReader.Load(CellETemp.ToString());

                        dgImage.IsReadOnly = false;
                        dgImage.Width = new DataGridLength(30);
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
        }
        private void cms_RetrieveFoodsForDay(object sender, CrmSdk.RetrieveFetchXmlCompletedEventArgs e)
        {
            String entityName = "dc_foodlog";
            String menuName = String.Empty;
            XElement element = e.Result;

            var parent = this.Parent;
            TabItem ti = (TabItem)parent;// null;//(TabItem)TabControl.SelectedItem;

            String day = (String)ti.Tag;
            Chart chart = GetChart(ti);
            Dictionary<String, DataGrid> dataGrids = new Dictionary<String, DataGrid>();

            dataGrids.Add("Breakfast", dataGridBreakfast);
            dataGrids.Add("Morning Snack", dataGridMorningSnack);
            dataGrids.Add("Lunch", dataGridLunch);
            dataGrids.Add("Afternoon Snack", dataGridAfternoonSnack);
            dataGrids.Add("Dinner", dataGridDinner);
            dataGrids.Add("Evening Snack", dataGridEveningSnack);

            if (dataGrids.Count > 0)
            {
                foreach (KeyValuePair<String, DataGrid> dg in dataGrids)
                {
                    buildOutDataGrids(element, dg.Value);
                }
            }

            //Build out datagrids

            //collection is in place.  Add rows
            if (element != null)
            {
                var rows = element.Descendants(entityName);
                foreach (var row in rows)
                {
                    Row rowData = new Row();

                    foreach (KeyValuePair<String, String> type in dataFoods.ColumnTypes)
                    {
                        if (type.Value.Equals("Lookup", StringComparison.OrdinalIgnoreCase))
                        {
                            rowData[type.Key] = new PairWithList(String.Empty, Guid.Empty.ToString(), dataFoods.EntityTypes[type.Key], new List<PairWithList>());
                        }
                        else if (type.Value.Equals("Picklist", StringComparison.OrdinalIgnoreCase))
                        {
                            rowData[type.Key] = "-1";//new Pair(String.Empty, String.Empty, );
                        }
                        else if (type.Value.Equals("DateTime", StringComparison.OrdinalIgnoreCase))
                        {
                            rowData[type.Key] = null;
                        }
                        else if (type.Value.Equals("Money", StringComparison.OrdinalIgnoreCase))
                        {
                            rowData[type.Key] = null;
                        }
                        else if (type.Value.Equals("Decimal", StringComparison.OrdinalIgnoreCase))
                        {
                            rowData[type.Key] = "0";
                        }
                        else
                        {
                            rowData[type.Key] = String.Empty;
                        }
                    }
                    PairWithList mealName = new PairWithList(String.Empty, String.Empty);
                    foreach (XElement xe in row.Elements())
                    {

                        if (dataFoods.ColumnTypes.ContainsKey(xe.Name.LocalName) && dataFoods.ColumnTypes[xe.Name.LocalName].Equals("money", StringComparison.OrdinalIgnoreCase))
                        {
                            rowData[xe.Name.LocalName] = String.Format("{0:C2}", Convert.ToDecimal(xe.Value));
                        }
                        else if (dataFoods.ColumnTypes.ContainsKey(xe.Name.LocalName) && dataFoods.ColumnTypes[xe.Name.LocalName].Equals("lookup", StringComparison.OrdinalIgnoreCase))
                        {
                            //MessageBox.Show(xe.Name.LocalName + ":" + xe.Value + ":" + xe.Attribute("Id").Value);
                            rowData[xe.Name.LocalName] = new PairWithList(xe.Value, xe.Attribute("Id").Value, dataFoods.EntityTypes[xe.Name.LocalName], new List<PairWithList>());
                        }
                        else if (dataFoods.ColumnTypes.ContainsKey(xe.Name.LocalName) && dataFoods.ColumnTypes[xe.Name.LocalName].Equals("picklist", StringComparison.OrdinalIgnoreCase))
                        {
                            //MessageBox.Show(xe.Value+":"+xe.Attribute("Id").Value);
                            rowData[xe.Name.LocalName] = new PairWithList(xe.Value, xe.Attribute("Id").Value);
                        }
                        else
                        {
                            //If number round to tenths
                            decimal d = 0m;
                            if (decimal.TryParse(xe.Value, out d))
                            {
                                rowData[xe.Name.LocalName] = Math.Round(d, 1).ToString();
                            }
                            else
                            {
                                rowData[xe.Name.LocalName] = xe.Value;
                            }
                        }

                        //dc_meal
                        if (xe.Name.LocalName.Equals("dc_meal"))
                        {
                            mealName = new PairWithList(xe.Value, xe.Attribute("Id").Value);
                        }
                        if (xe.Name.LocalName.Equals("dc_foods.dc_recipefood"))
                        {
                            rowData["dc_recipefood"] = xe.Value;
                        }
                        if (xe.Name.LocalName.Equals("dc_foodlogday.dc_menuid") && String.IsNullOrEmpty(menuName))
                        {
                            menuName = xe.Value;
                        }

                    }
                    if (mealList.ContainsKey(mealName.Name))
                    {
                        mealList[mealName.Name].Add(rowData);
                    }
                    else//need to add collection
                    {
                        mealList.Add(mealName.Name, new SortableCollectionView());
                        mealList[mealName.Name].Add(rowData);
                    }
                    rowData.CreateRow = false;
                    Guid Id = new Guid(((PairWithList)rowData["dc_foodid"]).Id);
                    if (!foods.ContainsKey(Id))
                    {
                        Food f = new Food();
                        f.Fat = Convert.ToDecimal(rowData["dc_fat"]);
                        f.Protein = Convert.ToDecimal(rowData["dc_protein"]);
                        f.Carbohydrate = Convert.ToDecimal(rowData["dc_carbohydrate"]);
                        f.Alcohol = Convert.ToDecimal(rowData["dc_alcohol"]);
                        f.PortionSize = Convert.ToDecimal(rowData["dc_portionsize"]);
                        f.PortionType = (PairWithList)(rowData["dc_portiontypeid"]);
                        f.PortionTypeAbbreviation = (String)rowData["dc_portion_types.dc_abbreviation"];
                        f.PortionTypeName = (String)rowData["dc_portion_types.dc_name"];
                        foods.Add(Id, f);
                    }
                }
                CalculatePieChart();
            }
            var dailyFoodLog = General.FindParrentUserControl(this, typeof(DailyFoodLog));
            if (dailyFoodLog != null)
            {
                ((DailyFoodLog)dailyFoodLog).MenuName(menuName);
            }
            busyIndicator.IsBusy = false;
        }//end of method

        private void CalculatePieChart()
        {
            Dictionary<String, DataGrid> dataGrids = new Dictionary<String, DataGrid>();

            dataGrids.Add("Breakfast", dataGridBreakfast);
            dataGrids.Add("Morning Snack", dataGridMorningSnack);
            dataGrids.Add("Lunch", dataGridLunch);
            dataGrids.Add("Afternoon Snack", dataGridAfternoonSnack);
            dataGrids.Add("Dinner", dataGridDinner);
            dataGrids.Add("Evening Snack", dataGridEveningSnack);

            if (dataGrids.Count > 0)
            {
                decimal protein = 0m;
                decimal fat     = 0m;
                decimal carbs   = 0m;
                decimal alcohol = 0m;
               
                foreach (KeyValuePair<String, DataGrid> dg in dataGrids)
                {
                    DataGrid dataGrid = (DataGrid)dg.Value;
                    String gridName = dg.Key;
                    if (mealList.ContainsKey(gridName))
                    {
                        dataGrid.ItemsSource = mealList[gridName];
                        if (mealList[gridName].Count > 0)
                        {
                            //dataGrid.Tag = dg.Key;//set to meal value
                        }
                        //Set footer values
                        SetFooterValues(dataGrid, (SortableCollectionView)dataGrid.ItemsSource);

                        foreach (Row row in mealList[gridName])
                        {
                            protein += String.IsNullOrEmpty((String)row["dc_protein"]) ? 0m : Convert.ToDecimal(row["dc_protein"]);
                            fat     += String.IsNullOrEmpty((String)row["dc_fat"]) ? 0m : Convert.ToDecimal(row["dc_fat"]);
                            carbs   += String.IsNullOrEmpty((String)row["dc_carbohydrate"]) ? 0m : Convert.ToDecimal(row["dc_carbohydrate"]);
                            alcohol += String.IsNullOrEmpty((String)row["dc_alcohol"]) ? 0m : Convert.ToDecimal(row["dc_alcohol"]);
                        }
                    }
                    else
                    {
                        dataGrid.ItemsSource = new SortableCollectionView();
                        mealList.Add(gridName, (SortableCollectionView)dataGrid.ItemsSource);
                    }
                    dataGrid.Tag = dg.Key;
                    SizeGrid(dataGrid);
                }

                //calculate and populate collection
                decimal proteinPct  = 0m;
                decimal fatPct      = 0m;
                decimal carbsPct    = 0m;
                decimal alcoholPct  = 0m;

                //Update app list
                App app = (App)App.Current;


                protein = protein * (decimal)App.GramToKcalMultipler.Protein;
                fat     = fat * (decimal)App.GramToKcalMultipler.Fat;
                carbs   = carbs * (decimal)App.GramToKcalMultipler.Carbohydrate;
                alcohol = alcohol * (decimal)App.GramToKcalMultipler.Alcohol;

                if (protein > 0 && (protein + fat + carbs + alcohol) > 0)
                {
                    proteinPct = protein / (protein + fat + carbs + alcohol);
                }
                if (fat > 0 && (protein + fat + carbs + alcohol) > 0)
                {
                    fatPct = fat / (protein + fat + carbs + alcohol);
                }
                if (carbs > 0 && (protein + fat + carbs + alcohol) > 0)
                {
                    carbsPct = carbs / (protein + fat + carbs + alcohol);
                }
                if (alcohol > 0 && (protein + fat + carbs + alcohol) > 0)
                {
                    alcoholPct = alcohol / (protein + fat + carbs + alcohol);
                }

                proteinPct = Math.Round(proteinPct, 2);
                fatPct = Math.Round(fatPct, 2);
                carbsPct = Math.Round(carbsPct, 2);
                alcoholPct = Math.Round(alcoholPct, 2);

                List<BarChartData> list = new List<BarChartData>();
                list.Add(new BarChartData());
                list.Add(new BarChartData());
                list.Add(new BarChartData());

                BarChartData p = new BarChartData("Protein: " + " - " + string.Format("{0:0%}", proteinPct), protein);
                list[0].Name = p.Name;
                list[0].Value = p.Value;

                BarChartData f2 = new BarChartData("Fat: " + " - " + string.Format("{0:0%}", fatPct), fat);
                list[1].Name = f2.Name;
                list[1].Value = f2.Value;

                BarChartData c2 = new BarChartData("Carbs: " + " - " + string.Format("{0:0%}", carbsPct), carbs);
                list[2].Name = c2.Name;
                list[2].Value = c2.Value;

                macroChart.RemoveAlcoholPalette();
               
                if (alcohol > 0)
                {
                    macroChart.AddAlcoholPalette();
                    list.Add(new BarChartData());
                    BarChartData c3 = new BarChartData("Alcohol: " + " - " + string.Format("{0:0%}", alcoholPct), alcohol);
                    list[3].Name = c3.Name;
                    list[3].Value = c3.Value;
                }
                

                macroChart.SetChartData(list);
                macroChart.Calories             = Math.Round((protein + fat + carbs + alcohol), 0);
                macroChart.IsBusy               = false;
                macroChart.busyIndicator.IsBusy = false;

            }
            //busyIndicator.IsBusy = false;
        }

        private void SetFooterValues(DataGrid dataGrid, SortableCollectionView rows)
        {

            StackPanel sp = GetFooter(dataGrid);
            TextBlock kcalsTB = (TextBlock)VisualTreeHelper.GetChild(sp, 0);
            TextBlock proteinTB = (TextBlock)VisualTreeHelper.GetChild(sp, 1);
            TextBlock fatTB = (TextBlock)VisualTreeHelper.GetChild(sp, 2);
            TextBlock carbsTB = (TextBlock)VisualTreeHelper.GetChild(sp, 3);
            TextBlock alcoholTB = (TextBlock)VisualTreeHelper.GetChild(sp, 4);
            decimal kcals   = 0m;
            decimal protein = 0m;
            decimal fat     = 0m;
            decimal carbs   = 0m;
            decimal alcohol = 0m;

            foreach (Row row in rows)
            {
                //kcals   += String.IsNullOrEmpty((String)row["dc_kcals"]) ? 0d : Convert.ToDouble(row["dc_kcals"]);
                protein += String.IsNullOrEmpty((String)row["dc_protein"]) ? 0m : Convert.ToDecimal(row["dc_protein"]);
                fat     += String.IsNullOrEmpty((String)row["dc_fat"]) ? 0m : Convert.ToDecimal(row["dc_fat"]);
                carbs   += String.IsNullOrEmpty((String)row["dc_carbohydrate"]) ? 0m : Convert.ToDecimal(row["dc_carbohydrate"]);
                alcohol += String.IsNullOrEmpty((String)row["dc_alcohol"]) ? 0m : Convert.ToDecimal(row["dc_alcohol"]);
            }
            //kcals += (fat * Convert.ToDecimal(App.GramToKcalMultipler.Fat) + protein * Convert.ToDecimal(App.GramToKcalMultipler.Protein) + carbs * Convert.ToDecimal(App.GramToKcalMultipler.Carbohydrate));
            kcals += (fat * Convert.ToDecimal(App.GramToKcalMultipler.Fat) + protein * Convert.ToDecimal(App.GramToKcalMultipler.Protein) + carbs * Convert.ToDecimal(App.GramToKcalMultipler.Carbohydrate) + alcohol * Convert.ToDecimal(App.GramToKcalMultipler.Alcohol));

            kcalsTB.Text = "Kcals: " + Math.Round(kcals, 0);
            proteinTB.Text = "Protein: " + Math.Round(protein, 0) + "g";
            fatTB.Text = "Fat: " + Math.Round(fat, 0) + "g";
            carbsTB.Text = "Carbs: " + Math.Round(carbs, 0) + "g";
            alcoholTB.Text = "Alcohol: " + Math.Round(alcohol, 0) + "g";
            if (alcohol > 0)
            {
                alcoholTB.Visibility = Visibility.Visible;
            }
            else
            {
                alcoholTB.Visibility = Visibility.Collapsed;
            }
        }
        private void SizeGrid(DataGrid dataGrid)
        {
            dataGrid.Visibility = System.Windows.Visibility.Visible;

            dataGrid.BorderThickness = new Thickness(1);

            // Space available to fill ( -18 Standard vScrollbar)
            double space_available = (LayoutRoot.ActualWidth - 22 - 35 - 40 - 40- 24); //22 is width of scroll bar 
            //figure out column types

            if (space_available > 0)
            {
                foreach (DataGridColumn dg_c in dataGrid.Columns)
                {
                    if (dg_c.SortMemberPath != null && dg_c.SortMemberPath.Equals("dc_foodid", StringComparison.OrdinalIgnoreCase))
                    {
                        dg_c.Width = new DataGridLength(space_available);
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
                    var footer = (MealFooter)VisualTreeHelper.GetChild(parent, x + 1);
                    var grid = (Grid)VisualTreeHelper.GetChild(footer, 0);
                    return ((StackPanel)VisualTreeHelper.GetChild(grid, 0));
                }
            }

            return (null);
        }
        /// <summary>
        /// For adding food to a meal
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, EventArgs e)
        {
            //Find datagrid  -  This is needed to know what to add the food to.
            //stack panel -> parent -> child -> find matching stackpanel; find next sibling
            StackPanel sp = null;
            DataGrid dataGrid = null;

            FrameworkElement child = (FrameworkElement)sender;
            while (true)
            {
                FrameworkElement parent = VisualTreeHelper.GetParent(child) as FrameworkElement;

                if (parent is StackPanel)
                {
                    sp = (StackPanel)parent;
                    break;
                }
                else
                {
                    child = parent;
                }
            }
            //found parent panel.  Now find grid
            //FrameworkElement spParent = (FrameworkElement)VisualTreeHelper.GetParent(sp);
            for (int x = 0; x < VisualTreeHelper.GetChildrenCount(sp); x++)
            {
                var child2 = VisualTreeHelper.GetChild(sp, x);
                if (child2 is MealHeader && child2 == (MealHeader)sender)
                {
                    dataGrid = ((DataGrid)VisualTreeHelper.GetChild(sp, x + 1));
                    break;
                }
            }
            SortableCollectionView dgCollection = (SortableCollectionView)dataGrid.ItemsSource;
            //Now add row to correct datagrid
            Row r = new Row();
            r.CreateRow = true;
            //Populate columnTypes  --  rowData.ColumnTypes.Add(xe.Name.LocalName, xe.Attribute("Type").Value);
            foreach (KeyValuePair<String, String> type in dataFoods.ColumnTypes)
            {
                if (type.Value.Equals("Lookup", StringComparison.OrdinalIgnoreCase))
                {
                    r[type.Key] = new PairWithList(String.Empty, Guid.Empty.ToString(), dataFoods.EntityTypes[type.Key], new List<PairWithList>());
                }
                else if (type.Value.Equals("Picklist", StringComparison.OrdinalIgnoreCase))
                {
                    r[type.Key] = new PairWithList(String.Empty, String.Empty);
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
            App app = (App)App.Current;
            r["dc_meal"] = new PairWithList("dc_meal", app.mealList[(String)dataGrid.Tag].ToString());
            r.RowChanged = true;
            try
            {
                //dataFoods.Add(r);
                dgCollection.Insert(0, r);
                //dgCollection.Add(r);
                dataGrid.ItemsSource = dgCollection;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
            }
        }

        /// <summary>
        /// Returns the chart for the day
        /// </summary>
        /// <param name="dataGrid"></param>
        /// <returns></returns>
        private Chart GetChart(TabItem ti)
        {
            FrameworkElement sp = (FrameworkElement)this.FindName("wrapper");

            for (int x = 0; x < VisualTreeHelper.GetChildrenCount(sp); x++)
            {
                var child = VisualTreeHelper.GetChild(sp, x);
                if (child is StackPanel)
                {
                    //Check if first child is a chart
                    var firstChild = VisualTreeHelper.GetChild(child, 0);
                    if (firstChild is Chart)
                    {
                        return ((Chart)firstChild);
                    }
                }
            }
            return (null);
        }

        private void PrintEntireWeek_Click(object sender, RoutedEventArgs e)
        {
            int dayOfWeek = (int)this.day.DayOfWeek;

            DateTime sundayDate = this.day.AddDays(-dayOfWeek - 1);
            DateTime saturdayDate = sundayDate.AddDays(8);
            String url = General.ReportServerUrl(((App)App.Current).webServicesName) + "/Food Log&rs:Command=Render";
            url += "&menuId=" + ((App)App.Current).contact.MenuId.ToString();
            url += "&contactId=" + ContactId;
            url += "&startDate=" + sundayDate;
            url += "&endDate=" + saturdayDate;
            HtmlPage.Window.Navigate(new Uri(url), "_blank");


        }
        private void PrintDay_Click(object sender, RoutedEventArgs e)
        {
            String url = General.ReportServerUrl(((App)App.Current).webServicesName) + "/Food Log&rs:Command=Render";
            url += "&menuId=" + ((App)App.Current).contact.MenuId.ToString();
            url += "&contactId=" + ContactId;
            url += "&startDate=" + this.day.AddDays(-1);
            url += "&endDate=" + this.day;
            HtmlPage.Window.Navigate(new Uri(url), "_blank");
        }

        private void NutrientDetails_Click(object sender, RoutedEventArgs e)
        {
            var parent = this.Parent;
            TabItem ti = (TabItem)parent;// null;//(TabItem)TabControl.SelectedItem;

            String day = (String)ti.Tag;

            Nutrients n = new Nutrients(((App)App.Current).contact.MenuId, -1, ContactId, "dc_foodlog", this.day);
            n.Show();
        }
    }
}

