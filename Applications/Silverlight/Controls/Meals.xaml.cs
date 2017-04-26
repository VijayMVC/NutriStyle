﻿using DynamicConnections.NutriStyle.MenuGenerator.ChildWindows;
using DynamicConnections.NutriStyle.MenuGenerator.Engine.Helpers;
using DynamicConnections.NutriStyle.MenuGenerator.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml.Linq;

namespace DynamicConnections.NutriStyle.MenuGenerator.Controls
{
    public partial class Meals : UserControl
    {
        private RowIndexConverter _rowIndexConverter = new RowIndexConverter();

        public String ContactId { get; set; }

        public String DayNumber { get; set; }

        private Dictionary<Guid, Food> foods;
        private bool loaded = false;
        private Dictionary<String, SortableCollectionView> daysList = new Dictionary<String, SortableCollectionView>();
        private Dictionary<String, int> dayList = new Dictionary<string, int>();
        private List<DataGrid> dataGrids = new List<DataGrid>();
        private DataGrid activeDataGrid;
        private Guid menuId = Guid.Empty;
        private Controls.ComboBoxWithValidation comboBox;
        private SortableCollectionView dataFoods;
        private Row selectedRow;

        //public static int dayNumber = 948170000;

        private MainPage m = null;

        private MacroChart macroChart;

        public MacroChart MacroChart
        {
            get { return (macroChart); }
            set
            {
                macroChart = value;
            }
        }

        public Meals()
        {
            InitializeComponent();
            Setup();
            /*
            m = (MainPage)Application.Current.RootVisual;
            m.CollapseTips();
            m.SetMacroChart(macroChart);*/
        }

        public Meals(MacroChart mc)
        {
            InitializeComponent();
            Setup();
            macroChart = mc;
        }

        private void Setup()
        {
            if (App.Current.GetType() == typeof(DynamicConnections.NutriStyle.MenuGenerator.App))
            {
                MacroChart = ((App)App.Current).mc;
                if (MacroChart == null)
                {
                    MacroChart = new MacroChart();
                    ((App)App.Current).mc = MacroChart;
                }
            }

            foods = new Dictionary<Guid, Food>();
            ContactId = String.Empty;
            dayList = new Dictionary<string, int>();
            dataFoods = new SortableCollectionView();

            dataGrids.Add(dataGridBreakfast);
            dataGrids.Add(dataGridMorningSnack);
            dataGrids.Add(dataGridLunch);
            dataGrids.Add(dataGridAfternoonSnack);
            dataGrids.Add(dataGridDinner);
            dataGrids.Add(dataGridEveningSnack);

            foreach (DataGrid dg in dataGrids)
            {
                dg.LoadingRow -= dataGrid1_LoadingRow;
                dg.UnloadingRow -= dataGrid1_UnloadingRowRow;
                dg.LoadingRow += new EventHandler<DataGridRowEventArgs>(dataGrid1_LoadingRow);
                dg.UnloadingRow += new EventHandler<DataGridRowEventArgs>(dataGrid1_UnloadingRowRow);
                
                //dg.UnloadingRow -= new EventHandler<DataGridRowEventArgs>(dataGrid1_LoadingRow);
            }
        }
        private void dataGrid1_UnloadingRowRow(object sender, DataGridRowEventArgs e)
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
                        if (c.GetCellContent(e.Row).GetType() == typeof(Controls.ComboBoxWithValidation))
                        {
                            Controls.ComboBoxWithValidation ccb = c.GetCellContent(e.Row) as Controls.ComboBoxWithValidation;

                            if (ccb.TagName.Equals("dc_foodid", StringComparison.OrdinalIgnoreCase))
                            {
                                 ccb.MyAutoCompleteBox.Text = String.Empty;
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
        private void dataGrid1_LoadingRow(object sender, DataGridRowEventArgs e)
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
                                //ToolTipService.SetToolTip(ccb, "Remove From Menu");
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
                                ctb.TextBox.SelectionChanged += new RoutedEventHandler(ccb_LostFocus);
                            }
                        }
                        else if (c.GetCellContent(e.Row).GetType() == typeof(Controls.ComboBoxWithValidation))
                        {
                            Controls.ComboBoxWithValidation ccb = c.GetCellContent(e.Row) as Controls.ComboBoxWithValidation;

                            if (ccb.TagName.Equals("dc_foodid", StringComparison.OrdinalIgnoreCase))
                            {
                                //ccb.KeyDown -= food_KeyDown;
                                //ccb.KeyDown += new KeyEventHandler(food_KeyDown);

                                ccb.KeyUp -= food_KeyDown;
                                ccb.KeyUp += new KeyEventHandler(food_KeyDown);

                                Row r = (Row)e.Row.DataContext;
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
        /// Open compare foods child window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CompareFoods(object sender, RoutedEventArgs e)
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

        private void cf_SwapClicked(object sender, EventArgs e)
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

            ((Row)dataGrid.SelectedItem)["dc_foodid"] = new PairWithList(name, Id, String.Empty, new List<PairWithList>());
            ((Row)dataGrid.SelectedItem)["dc_portionsize"] = portionSize;
            ((Row)dataGrid.SelectedItem)["dc_fat"] = fat;
            ((Row)dataGrid.SelectedItem)["dc_carbohydrate"] = carbs;
            ((Row)dataGrid.SelectedItem)["dc_protein"] = protein;
            Row r = (Row)dataGrid.SelectedItem;
            r.RowChanged = true;//for saving data
            CalculatePieChart();
            SetFooterValues(dataGrid, (SortableCollectionView)dataGrid.ItemsSource);
        }


        private void Row_GotFocus(object sender, RoutedEventArgs e)
        {
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
        private void ccb_LostFocus(object sender, RoutedEventArgs e)
        {
            DataGrid dg = GetRoot((FrameworkElement)sender);

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
            selectedRow.RowChanged = true;

            //get food
            if (foods.ContainsKey(new Guid(((PairWithList)selectedRow["dc_foodid"]).Id)))
            {
                Food f = foods[new Guid(((PairWithList)selectedRow["dc_foodid"]).Id)];

                //calculate
                String strPortionSize = (String)selectedRow["dc_portionsize"];
                Regex digitsOnly = new Regex(@"/[^0-9.]/g");
                strPortionSize = digitsOnly.Replace(strPortionSize, "");

                if (!String.IsNullOrEmpty(strPortionSize) && !strPortionSize.Equals("."))
                {
                    decimal portionSize = Convert.ToDecimal(strPortionSize);
                    decimal kcalMultiplier = portionSize / f.PortionSize;

                    selectedRow["dc_fat"] = (f.Fat * kcalMultiplier).ToString();
                    selectedRow["dc_protein"] = (f.Protein * kcalMultiplier).ToString();
                    selectedRow["dc_carbohydrate"] = (f.Carbohydrate * kcalMultiplier).ToString();
                    selectedRow["dc_alcohol"] = (f.Alcohol * kcalMultiplier).ToString();

                    //SetFooterValues(activeDataGrid, (SortableCollectionView)activeDataGrid.ItemsSource);

                    SetFooterValues(dg, (SortableCollectionView)dg.ItemsSource);

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

            String dataGridName = String.Empty;

            foreach (DataGrid dg in dataGrids)
            {
                SortableCollectionView data = (SortableCollectionView)dg.ItemsSource;

                //get row

                foreach (Row row in data)
                {
                    if (row.RowChanged)
                    {
                        r = row;
                        dataGridName = dg.Name;
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

                XElement eventXml = new XElement("dc_mealfood");

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
                        if (!String.IsNullOrEmpty((String)r[columnName]))
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
                if (r["dc_foodid"] != null)
                {
                    eventXml.Add(new XElement("dc_name", ((PairWithList)r["dc_foodid"]).Name));
                }

                //eventXml.Add(new XElement("dc_contactid", ap.contactId));

                XElement contactNode = new XElement(new XElement("dc_contactid", ap.contactId));
                eventXml.Add(contactNode);

                XAttribute atContact = new XAttribute("entityname", "contact");
                contactNode.Add(atContact);

                App app = (App)App.Current;
                int mealId = 0;

                //in case the meal doesn't have any foods in it.
                if (dataGridName.Contains("Morning"))
                {
                    mealId = app.mealList["Morning Snack"];
                }
                else if (dataGridName.Contains("Afternoon"))
                {
                    mealId = app.mealList["Afternoon Snack"];
                }
                else if (dataGridName.Contains("Evening"))
                {
                    mealId = app.mealList["Evening Snack"];
                }
                else if (dataGridName.Contains("Breakfast"))
                {
                    mealId = app.mealList["Breakfast"];
                }
                else if (dataGridName.Contains("Lunch"))
                {
                    mealId = app.mealList["Lunch"];
                }
                else if (dataGridName.Contains("Dinner"))
                {
                    mealId = app.mealList["Dinner"];
                }

                eventXml.Add(new XElement("dc_menuid", dataFoods[0]["dc_menu.dc_menuid"]));
                eventXml.Add(new XElement("dc_dayid", dataFoods[0]["dc_day.dc_dayid"]));
                eventXml.Add(new XElement("dc_mealvalue", mealId.ToString()));
                eventXml.Add(new XElement("dc_addedbyuser", "true"));

                r.RowChanged = false;

                //MessageBox.Show(eventXml.ToString());

                CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
                cms.CreateUpdateCompleted += new EventHandler<CrmSdk.CreateUpdateCompletedEventArgs>(cms_CreateUpdateGrid);
                cms.CreateUpdateAsync(eventXml, true);
                busyIndicator.IsBusy = false;
            }
        }

        private void cms_CreateUpdateGrid(object sender, CrmSdk.CreateUpdateCompletedEventArgs e)
        {
            //call back
            //saveMenu_Click(sender, null);
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
        private void MyAutoCompleteBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Make sure something is selected on the row
            activeDataGrid = GetRoot((FrameworkElement)sender);

            if (activeDataGrid.SelectedItem != null)
            {
                Row r = (Row)activeDataGrid.SelectedItem;
                r.RowChanged = true;
                AutoCompleteBox cb = sender as AutoCompleteBox;

                if (cb.SelectedItem != null && !String.IsNullOrEmpty(cb.Text))
                {
                    //Add search intelligence

                    PairWithList p = (PairWithList)cb.SelectedItem;

                    //Retreive the portion size
                    String fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                      <entity name='dc_foods'>
                        <attribute name='dc_portiontypeid' />
                        <attribute name='dc_portion_amount' />
                        <attribute name='dc_recipefood' />
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
                    orderXml.Add(new XElement("Column", "dc_recipefood"));
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
        /// Remove/Delete Row
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ccb_MouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
            try
            {
                DataGrid dataGrid = GetRoot((FrameworkElement)sender);

                SortableCollectionView data = (SortableCollectionView)dataGrid.ItemsSource;

                Button img = sender as Button;
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
                dataGrid.SelectedItem = null;
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

        private DataGrid GetRoot(FrameworkElement child)
        {
            //var parent = child.Parent as FrameworkElement;

            FrameworkElement parent = VisualTreeHelper.GetParent(child) as FrameworkElement;
            if (parent == null)
            {
                return null;
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
            if (e.Key != Key.Down && e.Key != Key.Up && e.Key != Key.Delete && e.Key != Key.Back && e.Key != Key.Enter)
            {
                comboBox = (Controls.ComboBoxWithValidation) sender;

                String text = comboBox.MyAutoCompleteBox.Text;

                if (String.IsNullOrEmpty(text))
                {
                    comboBox.SearchText = text;
                }
                //if (text != null && (String.IsNullOrEmpty(comboBox.SearchText) || comboBox.SearchText.Length > text.Length))
                if (comboBox.SearchText == null || (comboBox.SearchText != null && !comboBox.SearchText.Contains(text)) )
                {
                    if (text.Length > 2 &&
                        (e.Key != Key.Down && e.Key != Key.Up && e.Key != Key.Delete && e.Key != Key.Back))
                    {
                        //The text has to be different than what has already been searched on.  'cof' should not trigger a new search if the org search was 'co'
                        if (String.IsNullOrEmpty(comboBox.SearchText) || (text.Length != comboBox.SearchText.Length) ||
                            !text.Equals(comboBox.SearchText, StringComparison.OrdinalIgnoreCase))
                        {
                            comboBox.SearchText = text;

                            comboBox.IsBusy = true;
                            comboBox.MyAutoCompleteBox.SelectionChanged -= MyAutoCompleteBox_SelectionChanged;
                            comboBox.MyAutoCompleteBox.SelectionChanged +=
                                new SelectionChangedEventHandler(MyAutoCompleteBox_SelectionChanged);
                            comboBox.IsBusy = true;

                            try
                            {
                                CrmSdk.WebServicesSoapClient cms =
                                    new CrmSdk.WebServicesSoapClient(((App) App.Current).webServicesName);
                                cms.RetrieveFoodsCompleted +=
                                    new EventHandler<CrmSdk.RetrieveFoodsCompletedEventArgs>(cms_RetrieveFoods);
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
                        bool recipe = false;
                        String portionTypeName = String.Empty;
                        String portionTypeAbbreviation = String.Empty;

                        PairWithList portionType = new PairWithList();

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
                            Food f = new Food();
                            f.Fat = fat;
                            f.Protein = protein;
                            f.Carbohydrate = carbs;
                            f.Alcohol = alcohol;
                            f.PortionSize = orgPortionSize;
                            f.PortionType = portionType;
                            f.PortionTypeName = portionTypeName;
                            f.PortionTypeAbbreviation = portionTypeAbbreviation;
                            f.IsRecipe = recipe;
                            foods.Add(new Guid(Id), f);
                        }
                    }

                    //recipe?

                    //comboBox.MyAutoCompleteBox.ItemsSource = data;
                    comboBox.SelectedPair = new PairWithList(String.Empty, String.Empty, String.Empty, data);
                    //comboBox.OpenDropdown(true);
                    comboBox.IsBusy = false;
                }
                else if (element.Descendants(entityName).Count() == 0)//nothing found
                {
                    comboBox.IsBusy = false;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
            }
        }

        public void LoadDay(Guid contactId)
        {
            this.ContactId = contactId.ToString();
            if (!loaded)
            {
                macroChart.IsBusy = true;

                busyIndicator.IsBusy = true;
                loaded = true;

                //Make sure day has not been loaded

                var parent = this.Parent;
                TabItem ti = (TabItem)parent;// null;//(TabItem)TabControl.SelectedItem;

                String day = (String)ti.Tag;
                if (App.Current.GetType() == typeof(DynamicConnections.NutriStyle.MenuGenerator.App))
                {
                    this.dayList = ((App)App.Current).dayList;

                    //this.ContactId = ((App)App.Current).contactId;
                }

                String fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                      <entity name='dc_mealfood'>
                        <attribute name='dc_foodid' />
                        <attribute name='dc_portionsize' />

                        <attribute name='dc_portiontypeid' />

                        <attribute name='dc_fat' />
                        <attribute name='dc_protein' />
                        <attribute name='dc_carbohydrate' />
                        <attribute name='dc_alcohol' />
                        <attribute name='dc_kcals' />
                        <attribute name='dc_mealid' />

                        <attribute name='dc_mealfoodid' />

                        <order attribute='dc_mealfoodid' descending='false' />
                        <link-entity name='dc_portion_types' from='dc_portion_typesid' to='dc_portiontypeid' alias='dc_portion_types'>
                          <attribute name='dc_name' />
                          <attribute name='dc_abbreviation' />
                        </link-entity>
                        <link-entity name='dc_foods' alias='dc_foods' to='dc_foodid' from='dc_foodsid'>
                            <attribute name='dc_recipefood' />
                        </link-entity>
                        <link-entity name='dc_meal' from='dc_mealid' to='dc_mealid' alias='dc_meal'>
                          <attribute name='dc_meal'/>
                          <link-entity name='dc_day' from='dc_dayid' to='dc_dayid' alias='dc_day'>
                            <attribute name='dc_day'/>
                            <attribute name='dc_dayid'/>
                                <filter type='and'>
                                    <condition attribute='dc_day' operator='eq' value='@DAY' />
                                </filter>
                            <link-entity name='dc_menu' from='dc_menuid' to='dc_menuid' alias='dc_menu'>
                            <attribute name='dc_menuid'/>
                              <attribute name='dc_name' />
                              <filter type='and'>
                                <condition attribute='dc_contactid' operator='eq' value='@CONTACTID' />
                                <condition attribute='statecode' operator='eq' value='0' />
                                <condition attribute='dc_primarymenu' operator='eq' value='1' />
                              </filter>
                            </link-entity>
                          </link-entity>
                        </link-entity>
                      </entity>
                    </fetch>";

                fetchXml = fetchXml.Replace("@CONTACTID", contactId.ToString());
                if (dayList.Count > 0 && dayList.ContainsKey(day))
                {
                    fetchXml = fetchXml.Replace("@DAY", dayList[day].ToString());
                }
                XElement orderXml = new XElement("ColumnOrder");
                orderXml.Add(new XElement("Column", "dc_foodid"));
                orderXml.Add(new XElement("Column", "dc_portionsize"));

                orderXml.Add(new XElement("Column", "dc_portiontypeid"));

                orderXml.Add(new XElement("Column", "dc_fat"));
                orderXml.Add(new XElement("Column", "dc_protein"));
                orderXml.Add(new XElement("Column", "dc_carbohydrate"));
                orderXml.Add(new XElement("Column", "dc_alcohol"));
                orderXml.Add(new XElement("Column", "dc_kcals"));
                orderXml.Add(new XElement("Column", "dc_mealid"));

                orderXml.Add(new XElement("Column", "dc_mealfoodid"));

                orderXml.Add(new XElement("Column", "dc_meal.dc_meal"));
                orderXml.Add(new XElement("Column", "dc_day.dc_day"));
                orderXml.Add(new XElement("Column", "dc_day.dc_dayid"));
                orderXml.Add(new XElement("Column", "dc_menu.dc_menuid"));
                orderXml.Add(new XElement("Column", "dc_foods.dc_recipefood"));

                orderXml.Add(new XElement("Column", "dc_portion_types.dc_name"));
                orderXml.Add(new XElement("Column", "dc_portion_types.dc_abbreviation"));

                orderXml.Add(new XElement("Column", "dc_menu.dc_name"));

                try
                {
                    if (dayList.Count > 0)
                    {
                        /*
                        busyIndicator.IsBusy = true;
                        busyIndicator.Visibility = System.Windows.Visibility.Visible;
                        */
                        CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
                        cms.RetrieveFetchXmlCompleted += new EventHandler<CrmSdk.RetrieveFetchXmlCompletedEventArgs>(cms_RetrieveFoodsForDay);
                        cms.RetrieveFetchXmlAsync(fetchXml, orderXml.ToString());
                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                    MessageBox.Show(err.StackTrace);
                }
            }
            else
            {
                CalculatePieChart();
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

                        if (!dataFoods.EntityTypes.ContainsKey((xe.Name.LocalName)))
                        {
                            dataFoods.EntityTypes.Add(xe.Name.LocalName, xe.Attribute("Entity").Value);
                        }

                        if (xe.Name.LocalName.Equals("dc_menu.dc_menuid", StringComparison.OrdinalIgnoreCase) ||
                            xe.Name.LocalName.Equals("dc_meal.dc_meal", StringComparison.OrdinalIgnoreCase) ||
                            xe.Name.LocalName.Equals("dc_fat", StringComparison.OrdinalIgnoreCase) ||
                            xe.Name.LocalName.Equals("dc_alcohol", StringComparison.OrdinalIgnoreCase) ||
                            xe.Name.LocalName.Equals("dc_protein", StringComparison.OrdinalIgnoreCase) ||
                            xe.Name.LocalName.Equals("dc_carbohydrate", StringComparison.OrdinalIgnoreCase) ||
                            xe.Name.LocalName.Equals("dc_kcals", StringComparison.OrdinalIgnoreCase) ||
                            xe.Name.LocalName.Equals("dc_day.dc_day", StringComparison.OrdinalIgnoreCase) ||
                            xe.Name.LocalName.Equals("dc_day.dc_dayid", StringComparison.OrdinalIgnoreCase) ||
                            xe.Name.LocalName.Equals("dc_foods.dc_recipefood", StringComparison.OrdinalIgnoreCase) ||
                            xe.Name.LocalName.Equals("dc_mealid", StringComparison.OrdinalIgnoreCase) ||
                            xe.Name.LocalName.Equals("dc_portion_types.dc_name", StringComparison.OrdinalIgnoreCase) ||
                            xe.Name.LocalName.Equals("dc_menu.dc_name", StringComparison.OrdinalIgnoreCase) ||
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

                            CellETempPickList.Append("<basics2:CustomTextBox TagName='" + xe.Name.LocalName + "' SelectedText='{Binding Path=Data, Converter={StaticResource rowConvertor}, ConverterParameter=" + xe.Name.LocalName + ", Mode=TwoWay}' />");
                            CellETempPickList.Append("</DataTemplate>");
                            dg.CellTemplate = (DataTemplate)XamlReader.Load(CellETempPickList.ToString());

                            dg.Width = new DataGridLength(40);
                            dataGrid.Columns.Add(dg);
                        }
                        else if ((xe.Attribute("Type").Value.Equals("String", StringComparison.OrdinalIgnoreCase) ||
                            xe.Attribute("Type").Value.Equals("UniqueIdentifier", StringComparison.OrdinalIgnoreCase) ||
                            xe.Attribute("Type").Value.Equals("Decimal", StringComparison.OrdinalIgnoreCase)) &&
                            !(xe.Name.LocalName.Equals("dc_portion_types.dc_abbreviation", StringComparison.OrdinalIgnoreCase))

                            )
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

                            CellETempPickList.Append("<basics2:ComboBoxWithValidation TagName='" + xe.Name.LocalName + "' SelectedPair='{Binding Path=Data, Converter={StaticResource rowConvertor}, ConverterParameter=" + xe.Name.LocalName + ", Mode=TwoWay}' />");
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

                            CellETempPickList.Append("<basics2:ComboBoxWithValidation TagName='" + xe.Name.LocalName + "' Foreground='#4C97CC' IsRecipe='{Binding Path=Data, Converter={StaticResource rowConvertor}, ConverterParameter=dc_foods.dc_recipefood, Mode=TwoWay}' SelectedPair='{Binding Path=Data, Converter={StaticResource rowConvertor}, ConverterParameter=" + xe.Name.LocalName + ", Mode=TwoWay}' />");
                            CellETempPickList.Append("</DataTemplate>");
                            dg.CellEditingTemplate = (DataTemplate)XamlReader.Load(CellETempPickList.ToString());

                            if (xe.Name.LocalName.Equals("dc_portiontypeid", StringComparison.OrdinalIgnoreCase))
                            {
                                dg.IsReadOnly = true;
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
                    dgImage.Header = String.Empty;

                    //dgImage.SortMemberPath = xe.Name.LocalName;
                    dgImage.CanUserSort = false;

                    CellETemp = new StringBuilder();
                    CellETemp.Append("<DataTemplate ");
                    CellETemp.Append("xmlns='http://schemas.microsoft.com/winfx/");
                    CellETemp.Append("2006/xaml/presentation' ");
                    CellETemp.Append("xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' >");
                    CellETemp.Append("<Button Cursor='Hand' ToolTipService.ToolTip='Remove From Menu' Width='26' Height='26' Name='delete'>");
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

        private void cms_RetrieveFoodsForDay(object sender, CrmSdk.RetrieveFetchXmlCompletedEventArgs e)
        {
            String entityName = "dc_mealfood";
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

                    //dc_mael
                    if (xe.Name.LocalName.Equals("dc_meal.dc_meal"))
                    {
                        mealName = xe.Value;
                    }
                    else if (xe.Name.LocalName.Equals("dc_menu.dc_menuid"))
                    {
                        menuId = new Guid(xe.Value);
                    }
                    else if (xe.Name.LocalName.Equals("dc_menu.dc_name") && String.IsNullOrEmpty(menuName))
                    {
                        menuName = xe.Value;
                    }
                }

                Guid Id = new Guid(((PairWithList)rowData["dc_foodid"]).Id);
                decimal fat = Convert.ToDecimal(rowData["dc_fat"]);
                decimal protein = Convert.ToDecimal(rowData["dc_protein"]);
                decimal carbs = Convert.ToDecimal(rowData["dc_carbohydrate"]);
                decimal alcohol = Convert.ToDecimal(rowData["dc_alcohol"]);
                decimal orgPortionSize = Convert.ToDecimal(rowData["dc_portionsize"]);
                PairWithList portionType = ((PairWithList)rowData["dc_portiontypeid"]);

                String portionTypeName = (String)rowData["dc_portion_types.dc_name"];
                String portionTypeAbbreviation = (String)rowData["dc_portion_types.dc_abbreviation"];

                if (!foods.ContainsKey(Id))
                {
                    Food f = new Food();
                    f.Fat = fat;
                    f.Protein = protein;
                    f.Carbohydrate = carbs;
                    f.Alcohol = alcohol;
                    f.PortionSize = orgPortionSize;
                    f.PortionType = portionType;
                    f.PortionTypeAbbreviation = portionTypeAbbreviation;
                    f.PortionTypeName = portionTypeName;
                    foods.Add(Id, f);
                }

                if (daysList.ContainsKey(mealName))
                {
                    daysList[mealName].Add(rowData);
                }
                else//need to add collection
                {
                    daysList.Add(mealName, new SortableCollectionView());
                    daysList[mealName].Add(rowData);
                }
                dataFoods.Add(rowData);
            }

            //Make sure each grid is assigned a collection
            foreach (KeyValuePair<String, DataGrid> dg in dataGrids)
            {
                if (!daysList.ContainsKey(dg.Key))
                {
                    daysList.Add(dg.Key, new SortableCollectionView());
                }
            }
            var dailyMenu = General.FindParrentUserControl(this, typeof(DailyMenu));
            if (dailyMenu != null)
            {
                ((DailyMenu)dailyMenu).MenuName(menuName);
            }
            CalculatePieChart();
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

            if (dataGrids.Count > 0 && daysList.Count == 6)
            {
                decimal protein = 0m;
                decimal fat = 0m;
                decimal carbs = 0m;
                decimal alcohol = 0m;

                foreach (KeyValuePair<String, DataGrid> dg in dataGrids)
                {
                    DataGrid dataGrid = (DataGrid) dg.Value;
                    String gridName = dg.Key;
                    dataGrid.ItemsSource = daysList[gridName];

                    if (daysList[gridName].Count > 0)
                    {
                        dataGrid.Tag = ((PairWithList) ((SortableCollectionView) daysList[gridName])[0]["dc_mealid"]).Id;
                    }
                    SizeGrid(dataGrid);

                    //Set footer values
                    SetFooterValues(dataGrid, daysList[gridName]);

                    foreach (Row row in daysList[gridName])
                    {
                        protein += String.IsNullOrEmpty((String) row["dc_protein"])
                                       ? 0m
                                       : Convert.ToDecimal(row["dc_protein"]);
                        fat += String.IsNullOrEmpty((String) row["dc_fat"]) ? 0m : Convert.ToDecimal(row["dc_fat"]);
                        carbs += String.IsNullOrEmpty((String) row["dc_carbohydrate"])
                                     ? 0m
                                     : Convert.ToDecimal(row["dc_carbohydrate"]);
                        alcohol += String.IsNullOrEmpty((String) row["dc_alcohol"])
                                       ? 0m
                                       : Convert.ToDecimal(row["dc_alcohol"]);

                    }
                }

                //Setup the pie chart for this
                //Using pie chart name.  This is lame.  Use the Visual framework to find
                //Calculate up the kcals of protein, fat and carbs

                //calculate and populate collection
                decimal proteinPct = 0m;
                decimal fatPct = 0m;
                decimal carbsPct = 0m;
                decimal alcoholPct = 0m;

                //Update app list
                App app = (App) App.Current;

                protein = protein*(decimal) App.GramToKcalMultipler.Protein;
                fat = fat*(decimal) App.GramToKcalMultipler.Fat;
                carbs = carbs*(decimal) App.GramToKcalMultipler.Carbohydrate;
                alcohol = alcohol*(decimal) App.GramToKcalMultipler.Alcohol;

                if (protein > 0 && (protein + fat + carbs + alcohol) > 0)
                {
                    proteinPct = protein/(protein + fat + carbs + alcohol);
                }
                if (fat > 0 && (protein + fat + carbs + alcohol) > 0)
                {
                    fatPct = fat/(protein + fat + carbs + alcohol);
                }
                if (carbs > 0 && (protein + fat + carbs + alcohol) > 0)
                {
                    carbsPct = carbs/(protein + fat + carbs + alcohol);
                }
                if (alcohol > 0 && (protein + fat + carbs + alcohol) > 0)
                {
                    alcoholPct = alcohol/(protein + fat + carbs + alcohol);
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
                    BarChartData c3 = new BarChartData("Alcohol: " + " - " + string.Format("{0:0%}", alcoholPct),alcohol);
                    list[3].Name = c3.Name;
                    list[3].Value = c3.Value;
                }
                

                macroChart.SetChartData(list);
                macroChart.Calories             = Math.Round((protein + fat + carbs + alcohol), 0);
                macroChart.IsBusy               = false;
                macroChart.busyIndicator.IsBusy = false;
            }
            macroChart.IsBusy = false;

            //m.SetMacroChart(MacroChart);
        }

        private void SetFooterValues(DataGrid dataGrid, SortableCollectionView rows)
        {
            StackPanel sp       = GetFooter(dataGrid);
            TextBlock kcalsTB   = (TextBlock)VisualTreeHelper.GetChild(sp, 0);
            TextBlock proteinTB = (TextBlock)VisualTreeHelper.GetChild(sp, 1);
            TextBlock fatTB     = (TextBlock)VisualTreeHelper.GetChild(sp, 2);
            TextBlock carbsTB   = (TextBlock)VisualTreeHelper.GetChild(sp, 3);
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
            if (alcohol > 0)
            {
                var x = 0;
            }
            kcals += (fat * Convert.ToDecimal(App.GramToKcalMultipler.Fat) + protein * Convert.ToDecimal(App.GramToKcalMultipler.Protein) + carbs * Convert.ToDecimal(App.GramToKcalMultipler.Carbohydrate) + alcohol * Convert.ToDecimal(App.GramToKcalMultipler.Alcohol));

            kcalsTB.Text    = "Kcals: " + Math.Round(kcals, 0);
            proteinTB.Text  = "Protein: " + Math.Round(protein, 0) + "g";
            fatTB.Text      = "Fat: " + Math.Round(fat, 0) + "g";
            carbsTB.Text    = "Carbs: " + Math.Round(carbs, 0) + "g";
            alcoholTB.Text  = "Alcohol: " + Math.Round(alcohol, 0) + "g";
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
            double space_available = (LayoutRoot.ActualWidth - 22 - 35 - 40 - 40 - 24); //22 is width of scroll bar

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
        /// For adding row to a meal
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
            activeDataGrid = dataGrid;

            //Now add row to correct datagrid
            Row r = new Row();

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
            r["dc_mealid"] = new PairWithList("dc_mealid", (String)dataGrid.Tag, "dc_meal", new List<PairWithList>());

            r.RowChanged = true;
            try
            {
                dgCollection.Insert(0, r);
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
            //get busy indicator
            //FrameworkElement bi = (FrameworkElement)VisualTreeHelper.GetChild((FrameworkElement)ti.Content, 0);
            //get stack panel
            //FrameworkElement sp = (FrameworkElement)VisualTreeHelper.GetChild(bi, 0);

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

        private void NutrientDetails_Click(object sender, RoutedEventArgs e)
        {
            var parent = this.Parent;
            TabItem ti = (TabItem)parent;// null;//(TabItem)TabControl.SelectedItem;

            String day = (String)ti.Tag;

            Nutrients n = new Nutrients(menuId, Convert.ToInt32(dayList[day].ToString()), new Guid(ContactId), "dc_mealfood", DateTime.MinValue);
            n.Show();
        }

        private void PrintEntireWeek_Click(object sender, RoutedEventArgs e)
        {
            //no day
            HtmlPage.Window.Navigate(new Uri(General.ReportServerUrl(((App)App.Current).webServicesName) + "/Menu&rs:Command=Render&day:isnull=true&MenuId=" + menuId.ToString()), "_blank");
        }

        private void PrintDay_Click(object sender, RoutedEventArgs e)
        {
            //no day
            var parent = this.Parent;
            TabItem ti = (TabItem)parent;// null;//(TabItem)TabControl.SelectedItem;
            String day = (String)ti.Tag;
            HtmlPage.Window.Navigate(new Uri(General.ReportServerUrl(((App)App.Current).webServicesName) + "/Menu&rs:Command=Render&day=" + dayList[day].ToString() + "&MenuId=" + menuId.ToString()), "_blank");
        }
    }
}