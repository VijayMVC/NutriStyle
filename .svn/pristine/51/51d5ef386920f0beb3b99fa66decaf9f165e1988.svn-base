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
using System.Windows.Media.Imaging;
using DynamicConnections.NutriStyle.MenuGenerator.Engine.Helpers;
using System.Text;
using System.Windows.Markup;
using System.Xml.Linq;
using System.Windows.Data;
using System.Windows.Printing;
using DynamicConnections.NutriStyle.MenuGenerator.Controls;
using System.Collections;
using System.Reflection;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Controls.DataVisualization;
using System.Windows.Controls.DataVisualization.Charting.Primitives;

namespace DynamicConnections.NutriStyle.MenuGenerator.ChildWindows
{

    public partial class EditFoodLog : ChildWindow
    {

        Controls.ComboBox comboBoxFood;

        int rowCount;
        App app;
        private RowIndexConverter _rowIndexConverter = new RowIndexConverter();

        Row selectedRow;
        DataGrid activeDataGrid;

        DateTime day;

        List<DataGrid> dataGrids = new List<DataGrid>();
        Dictionary<Guid, Food> foods;
        SortableCollectionView dataFoods;
        Dictionary<String, SortableCollectionView> mealList = new Dictionary<String, SortableCollectionView>();

        public EditFoodLog()
        {
            InitializeComponent();
            dataFoods = new SortableCollectionView();
        }
        public EditFoodLog(DateTime day)
        {
            foods = new Dictionary<Guid, Food>();
            dataFoods = new SortableCollectionView();
            app = (App)App.Current;
            InitializeComponent();
            rowCount = 0;
            this.day = day;
            Label.Text = "Food Log: " + day.ToString("MM/dd/yyyy");
            RetrieveFoodLog(day);

            dataGrids.Add(dataGridBreakfast);
            dataGrids.Add(dataGridLunch);
            dataGrids.Add(dataGridDinner);

            foreach (DataGrid dg in dataGrids)
            {
                dg.LoadingRow += new EventHandler<DataGridRowEventArgs>(dataGrid1_LoadingRow);
            }

            //dataGrid1.LoadingRow += new EventHandler<DataGridRowEventArgs>(dataGrid1_LoadingRow);
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
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
                        if (c.GetCellContent(e.Row).GetType() == typeof(Image))
                        {
                            Image ccb = c.GetCellContent(e.Row) as Image;
                            ToolTipService.SetToolTip(ccb, "Remove From Food log");

                            ccb.MouseLeftButtonUp -= ccb_MouseLeftButtonUp;
                            ccb.MouseLeftButtonUp += new MouseButtonEventHandler(ccb_MouseLeftButtonUp);

                        }
                        else if (c.GetCellContent(e.Row).GetType() == typeof(Controls.CustomTextBox))
                        {
                            CustomTextBox ctb = c.GetCellContent(e.Row) as Controls.CustomTextBox;
                            if (ctb.TagName.Equals("dc_portionsize", StringComparison.OrdinalIgnoreCase))
                            {
                                ctb.LostFocus -= ccb_LostFocus;
                                ctb.LostFocus += new RoutedEventHandler(ccb_LostFocus);
                                /*
                                ctb.TextBox.SelectionChanged -= TextBox_SelectionChanged;
                                ctb.TextBox.SelectionChanged += new RoutedEventHandler(TextBox_SelectionChanged);
                                 * */
                            }
                        }
                        else if (c.GetCellContent(e.Row).GetType() == typeof(Controls.ComboBox))
                        {
                            Controls.ComboBox ccb = c.GetCellContent(e.Row) as Controls.ComboBox;

                            if (ccb.TagName.Equals("dc_foodid", StringComparison.OrdinalIgnoreCase))
                            {

                                ccb.KeyDown -= food_KeyDown;
                                ccb.KeyDown += new KeyEventHandler(food_KeyDown);
                                ccb.LostFocus -= ccb_LostFocus;
                                ccb.LostFocus += new RoutedEventHandler(ccb_LostFocus);

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

        void TextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            DataGrid dg = GetRoot((FrameworkElement)sender);
            Row r = (Row)dg.SelectedItem;
            r.RowChanged = true;
        }
        private void food_KeyDown(object sender, KeyEventArgs e)
        {

            comboBoxFood = (Controls.ComboBox)sender;

            String text = comboBoxFood.MyAutoCompleteBox.Text;

            if (text.Length > 1)
            {
                comboBoxFood.MyAutoCompleteBox.SelectionChanged -= MyAutoCompleteBox_SelectionChanged;
                comboBoxFood.MyAutoCompleteBox.SelectionChanged += new SelectionChangedEventHandler(MyAutoCompleteBox_SelectionChanged);

                XElement orderXml = null;

                String fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
                  <entity name='dc_foods'>
                    <attribute name='dc_name' />
                    <attribute name='dc_portion_amount' />
                    <attribute name='dc_portiontypeid' />

                    <attribute name='dc_foodsid' />
                    <order attribute='dc_name' descending='false' />
                    <filter type='and'>
                        <condition attribute='dc_name' value='%@TEXT%' operator='like'/>
                        <condition attribute='dc_canusefoodinmenu' value='1' operator='eq'/>
                    </filter>
                        <link-entity name='dc_food_nutrients' from='dc_food_nutrientsid' to='dc_foodnutrientid' alias='dc_food_nutrients'>
                            <attribute name='dc_fat' />
                            <attribute name='dc_protein' />
                            <attribute name='dc_carbohydrate' />
                        </link-entity>
                  </entity>
                </fetch>";

                fetchXml = fetchXml.Replace("@TEXT", text);

                orderXml = new XElement("ColumnOrder");
                orderXml.Add(new XElement("Column", "dc_name"));
                orderXml.Add(new XElement("Column", "dc_portion_amount"));
                orderXml.Add(new XElement("Column", "dc_portiontypeid"));
                orderXml.Add(new XElement("Column", "dc_food_nutrients.dc_fat"));
                orderXml.Add(new XElement("Column", "dc_food_nutrients.dc_protein"));
                orderXml.Add(new XElement("Column", "dc_food_nutrients.dc_carbohydrate"));
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
                        decimal fat = 0m;
                        decimal protein = 0m;
                        decimal carbs = 0m;
                        decimal orgPortionSize = 0m;
                        PairWithList portionType = new PairWithList();

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
                            else if (xe.Name.LocalName.Equals("dc_fat", StringComparison.OrdinalIgnoreCase))
                            {
                                fat = Convert.ToDecimal(xe.Value);
                            }
                            else if (xe.Name.LocalName.Equals("dc_protein", StringComparison.OrdinalIgnoreCase))
                            {
                                protein = Convert.ToDecimal(xe.Value);
                            }
                            else if (xe.Name.LocalName.Equals("dc_carbohydrate", StringComparison.OrdinalIgnoreCase))
                            {
                                carbs = Convert.ToDecimal(xe.Value);
                            }
                            else if (xe.Name.LocalName.Equals("dc_portion_amount", StringComparison.OrdinalIgnoreCase))
                            {
                                orgPortionSize = Convert.ToDecimal(xe.Value);
                            }
                            else if (xe.Name.LocalName.Equals("dc_portiontypeid", StringComparison.OrdinalIgnoreCase))
                            {
                                portionType = new PairWithList(xe.Value, xe.Attribute("Id").Value);
                            }
                        }
                        data.Add(new Pair(name, Id));
                        if (!foods.ContainsKey(new Guid(Id)))
                        {
                            Food f = new Food();
                            f.Fat = fat;
                            f.Protein = protein;
                            f.Carbohydrate = carbs;
                            f.PortionSize = orgPortionSize;
                            f.PortionType = portionType;
                            foods.Add(new Guid(Id), f);
                        }
                    }
                    comboBoxFood.MyAutoCompleteBox.ItemsSource = data;
                    comboBoxFood.OpenDropdown(true);
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
            }
        }
        void MyAutoCompleteBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            //Make sure something is selected on the row
            activeDataGrid = GetRoot((FrameworkElement)sender);
            Row r = (Row)activeDataGrid.SelectedItem;
            r.RowChanged = true;
            if (activeDataGrid.SelectedItem != null)
            {

                AutoCompleteBox cb = (AutoCompleteBox)sender;
                Border b = (Border)cb.Parent;
                Grid g = (Grid)b.Parent;
                Controls.ComboBox ccb = (Controls.ComboBox)g.Parent;
                //Get parent datagrid

                if (cb.SelectedItem != null && !String.IsNullOrEmpty(cb.Text))
                {
                    Pair p = (Pair)cb.SelectedItem;
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
                    r["dc_portionsize"] = foods[new Guid(p.Id)].PortionSize.ToString();
                    r["dc_portiontypeid"] = foods[new Guid(p.Id)].PortionType;

                    XElement orderXml = new XElement("ColumnOrder");
                    orderXml.Add(new XElement("Column", "dc_portiontype"));
                    orderXml.Add(new XElement("Column", "dc_portion_amount"));
                    orderXml.Add(new XElement("Column", "dc_foodsid"));
                    
                }
            }
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
                SetFooterValues(dataGrid, data);
                CalculatePieChart();
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


        private void RetrieveFoodLog(DateTime day)
        {
            String fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
              <entity name='dc_foodlog'>
                <attribute name='dc_foodlogid' />
                <attribute name='dc_portiontypeid' />
                <attribute name='dc_foodid' />
                <attribute name='dc_portionsize' />
                
                <attribute name='dc_fat' />
                <attribute name='dc_protein' />
                <attribute name='dc_carbohydrate' />

                <attribute name='dc_meal' />
                <order attribute='dc_foodid' descending='false' />
                <filter type='and'>
                  <condition attribute='statecode' operator='eq' value='0' />
                  <condition attribute='dc_contactid' operator='eq' value='@CONTACTID' />
                  <condition attribute='dc_date' operator='on' value='@DATETIME' />
                </filter>
              </entity>
            </fetch>";

            fetchXml = fetchXml.Replace("@CONTACTID", app.contactId);
            fetchXml = fetchXml.Replace("@DATETIME", day.ToString("MM/dd/yyyy"));

            XElement orderXml = new XElement("ColumnOrder");
            orderXml.Add(new XElement("Column", "dc_foodid"));
            orderXml.Add(new XElement("Column", "dc_portionsize"));
            orderXml.Add(new XElement("Column", "dc_portiontypeid"));


            orderXml.Add(new XElement("Column", "dc_fat"));
            orderXml.Add(new XElement("Column", "dc_protein"));
            orderXml.Add(new XElement("Column", "dc_carbohydrate"));

            orderXml.Add(new XElement("Column", "dc_meal"));

            orderXml.Add(new XElement("Column", "dc_foodlogid"));

            try
            {
                busyIndicator.IsBusy = true;
                busyIndicator.Visibility = System.Windows.Visibility.Visible;

                CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
                cms.RetrieveFetchXmlCompleted += new EventHandler<CrmSdk.RetrieveFetchXmlCompletedEventArgs>(cms_RetrieveFoodLog);
                cms.RetrieveFetchXmlAsync(fetchXml, orderXml.ToString());
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
            }
        }
        private void cms_RetrieveFoodLog(object sender, CrmSdk.RetrieveFetchXmlCompletedEventArgs e)
        {
            String entityName = "dc_foodlog";
            //buildGrid("dc_foodlog", dataGrid1, busyIndicator, e.Result, 1);
            XElement element = e.Result;

            Dictionary<String, DataGrid> dataGrids = new Dictionary<String, DataGrid>();

            dataGrids.Add("Breakfast", dataGridBreakfast);
            dataGrids.Add("Lunch", dataGridLunch);
            dataGrids.Add("Dinner", dataGridDinner);

            if (dataGrids.Count > 0)
            {
                foreach (KeyValuePair<String, DataGrid> dg in dataGrids)
                {
                    buildOutDataGrids(element, dg.Value);
                }
            }


            //collection is in place.  Add rows
            var rows = element.Descendants(entityName);
            foreach (var row in rows)
            {
                Row rowData = new Row();

                foreach (KeyValuePair<String, String> type in dataFoods.ColumnTypes)
                {
                    if (type.Value.Equals("Lookup", StringComparison.OrdinalIgnoreCase))
                    {
                        rowData[type.Key] = new Pair(String.Empty, Guid.Empty.ToString(), dataFoods.EntityTypes[type.Key]);
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
                //String mealName = String.Empty;//key for collection
                Pair mealName = new Pair(String.Empty, String.Empty);
                foreach (XElement xe in row.Elements())
                {

                    if (dataFoods.ColumnTypes.ContainsKey(xe.Name.LocalName) && dataFoods.ColumnTypes[xe.Name.LocalName].Equals("money", StringComparison.OrdinalIgnoreCase))
                    {
                        rowData[xe.Name.LocalName] = String.Format("{0:C2}", Convert.ToDecimal(xe.Value));
                    }
                    else if (dataFoods.ColumnTypes.ContainsKey(xe.Name.LocalName) && dataFoods.ColumnTypes[xe.Name.LocalName].Equals("lookup", StringComparison.OrdinalIgnoreCase))
                    {
                        //MessageBox.Show(xe.Name.LocalName + ":" + xe.Value + ":" + xe.Attribute("Id").Value);
                        rowData[xe.Name.LocalName] = new Pair(xe.Value, xe.Attribute("Id").Value, dataFoods.EntityTypes[xe.Name.LocalName]);
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
                        mealName = new Pair(xe.Value, xe.Attribute("Id").Value);
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
            }
            CalculatePieChart();
        }
        private void CalculatePieChart()
        {

            Chart chart = pieChart;

            Dictionary<String, DataGrid> dataGrids = new Dictionary<String, DataGrid>();

            dataGrids.Add("Breakfast", dataGridBreakfast);
            dataGrids.Add("Lunch", dataGridLunch);
            dataGrids.Add("Dinner", dataGridDinner);

            if (dataGrids.Count > 0)
            {
                decimal protein = 0m;
                decimal fat = 0m;
                decimal carbs = 0m;

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


                            //kcals += String.IsNullOrEmpty((String)row["dc_kcals"]) ? 0m : Convert.ToDecimal(row["dc_kcals"]);
                            protein += String.IsNullOrEmpty((String)row["dc_protein"]) ? 0m : Convert.ToDecimal(row["dc_protein"]);
                            fat += String.IsNullOrEmpty((String)row["dc_fat"]) ? 0m : Convert.ToDecimal(row["dc_fat"]);
                            carbs += String.IsNullOrEmpty((String)row["dc_carbohydrate"]) ? 0m : Convert.ToDecimal(row["dc_carbohydrate"]);
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
                //Setup the pie chart for this
                //Using pie chart name.  This is lame.  Use the Visual framework to find
                //Calculate up the kcals of protein, fat and carbs

                List<KeyValuePair<String, decimal>> list = new List<KeyValuePair<string, decimal>>();

                protein = protein * (decimal)App.GramToKcalMultipler.Protein;
                fat = fat * (decimal)App.GramToKcalMultipler.Fat;
                carbs = carbs * (decimal)App.GramToKcalMultipler.Carbohydrate;

                decimal proteinPct = 0m;
                decimal fatPct = 0m;
                decimal carbsPct = 0m;

                if (protein > 0 && (protein + fat + carbs) > 0)
                {
                    proteinPct = protein / (protein + fat + carbs);
                }
                if (fat > 0 && (protein + fat + carbs) > 0)
                {
                    fatPct = fat / (protein + fat + carbs);
                }
                if (carbs > 0 && (protein + fat + carbs) > 0)
                {
                    carbsPct = carbs / (protein + fat + carbs);
                }

                list.Add(new KeyValuePair<string, decimal>("Protein: " + " - " + string.Format("{0:0.0%}", proteinPct), protein));
                list.Add(new KeyValuePair<string, decimal>("Fat " + " - " + string.Format("{0:0.0%}", fatPct), fat));
                list.Add(new KeyValuePair<string, decimal>("Carbs" + " - " + string.Format("{0:0.0%}", carbsPct), carbs));

                //((PieSeries)saturdayChart.Series[0]).ItemsSource = list;
                ((PieSeries)chart.Series[0]).ItemsSource = list;

                Rect pieBounds = new Rect();
                var center = new Point();
                var radius = 0d;
                ResourceDictionaryCollection palette = new ResourceDictionaryCollection();
                try
                {
                    EdgePanel plotArea = (EdgePanel)((PieSeries)chart.Series[0]).Parent;

                    plotArea.Width = plotArea.ActualHeight;//makes it a sqaure
                    Style style = new Style(typeof(LegendItem));//((PieSeries)chart.Series[0]).LegendItemStyle;

                    style.Setters.Add(new Setter(WidthProperty, (544) - plotArea.Width - 95d));

                    ((PieSeries)chart.Series[0]).LegendItemStyle = style;

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
                chart.Palette = palette;
                busyIndicator.IsBusy = false;
            }



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
            if (dgCollection == null)
            {
                dgCollection = new SortableCollectionView();
                dataGrid.ItemsSource = dgCollection;



                mealList.Add((String)dataGrid.Tag, dgCollection);

            }
            //Now add row to correct datagrid
            Row r = new Row();

            //Populate columnTypes  --  rowData.ColumnTypes.Add(xe.Name.LocalName, xe.Attribute("Type").Value);
            foreach (KeyValuePair<String, String> type in dataFoods.ColumnTypes)
            {
                if (type.Value.Equals("Lookup", StringComparison.OrdinalIgnoreCase))
                {
                    r[type.Key] = new Pair(String.Empty, Guid.Empty.ToString(), dataFoods.EntityTypes[type.Key]);
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

            r["dc_meal"] = new Pair("dc_meal", app.mealList[(String)dataGrid.Tag].ToString());

            r.RowChanged = true;
            try
            {
                //dataFoods.Add(r);
                dgCollection.Insert(0, r);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
            }
        }

        /// <summary>
        /// Makes sure all fields are accounted for
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        private bool ValidateRow(Row r)
        {
            if (!String.IsNullOrEmpty((String)r["dc_portionsize"]) &&
                r["dc_foodid"] != null && !String.IsNullOrEmpty(((Pair)r["dc_foodid"]).Id))
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

            selectedRow = (Row)dg.SelectedItem;
            //get food
            if (foods.ContainsKey(new Guid(((Pair)selectedRow["dc_foodid"]).Id)))
            {
                Food f = foods[new Guid(((Pair)selectedRow["dc_foodid"]).Id)];
                //calculate 

                decimal portionSize = (String)selectedRow["dc_portionsize"] != String.Empty ? Convert.ToDecimal((String)selectedRow["dc_portionsize"]) : 0m;
                decimal kcalMultiplier = portionSize / f.PortionSize;

                selectedRow["dc_fat"] = (f.Fat * kcalMultiplier).ToString();
                selectedRow["dc_protein"] = (f.Fat * kcalMultiplier).ToString();
                selectedRow["dc_carbohydrate"] = (f.Carbohydrate * kcalMultiplier).ToString();

                SetFooterValues(activeDataGrid, (SortableCollectionView)activeDataGrid.ItemsSource);

                CalculatePieChart();
            }
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {

            App ap = (App)App.Current;
            Row r = null;

            List<DataGrid> dataGrids = new List<DataGrid>();

            dataGrids.Add(dataGridBreakfast);
            dataGrids.Add(dataGridLunch);
            dataGrids.Add(dataGridDinner);


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
            if (true)//if (ValidateRow(r))
            {
                busyIndicator.IsBusy = true;
                //Have row
                //create xml document and send to server to save

                XElement eventXml = new XElement("dc_foodlog");
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
                            if (r[columnName] != null && r[columnName].GetType() == typeof(Pair))
                            {
                                if (((Pair)r[columnName]).Id != "" && ((Pair)r[columnName]).Id != String.Empty)
                                {
                                    XElement lookupNode = new XElement(new XElement(columnName, ((Pair)r[columnName]).Id));
                                    eventXml.Add(lookupNode);

                                    XAttribute at = new XAttribute("entityname", dataFoods.EntityTypes[columnName]);
                                    lookupNode.Add(at);
                                }
                            }
                        }
                        else if (dataFoods.ColumnTypes[columnName].Equals("Picklist", StringComparison.OrdinalIgnoreCase))
                        {
                            eventXml.Add(new XElement(columnName, ((Pair)r[columnName]).Id));
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
                            if (!String.IsNullOrEmpty((String)r[columnName]) && !columnName.Equals("dc_fat", StringComparison.OrdinalIgnoreCase))
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
                    //eventXml.Add(new XElement("contactid", ap.contactId));
                    XElement lookupNodeContact = new XElement(new XElement("dc_contactid", app.contactId));
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

                //MessageBox.Show(eventXml.ToString());
                CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
                /*
                CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
                cms.CreateUpdateCompleted += new EventHandler<CrmSdk.CreateUpdateCompletedEventArgs>(cms_CreateUpdateGrid);
                cms.CreateUpdateAsync(eventXml);
                */
                cms.CreateUpdateCompleted += new EventHandler<CrmSdk.CreateUpdateCompletedEventArgs>(cms_CreateUpdateGrid);
                cms.CreateUpdateAsync(eventXml, true);
            }


        }
        private void cms_CreateUpdateGrid(object sender, CrmSdk.CreateUpdateCompletedEventArgs e)
        {

            

            var results = from x in e.Result.Descendants("Success") select x;
            Guid Id = new Guid(e.Result.Value.ToString());
            selectedRow.RowChanged = false;
            //set id
            selectedRow["dc_foodlogid"] = Id.ToString();
            //call back
            Save_Click(sender, null);
        }


        private void buildOutDataGrids(XElement element, DataGrid dataGrid)
        {
            String entityName = "dc_foodlog";

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
                            xe.Name.LocalName.Equals("dc_carbohydrate", StringComparison.OrdinalIgnoreCase) ||
                            xe.Name.LocalName.Equals("dc_kcals", StringComparison.OrdinalIgnoreCase) ||
                            xe.Name.LocalName.Equals("dc_day", StringComparison.OrdinalIgnoreCase) ||
                            xe.Name.LocalName.Equals("dc_mealid", StringComparison.OrdinalIgnoreCase))
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

        private DataGrid GetRoot(FrameworkElement child)
        {
            //var parent = child.Parent as FrameworkElement;
            if (child != null)
            {
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
            else
            {
                return (null);
            }
        }

        private void SetFooterValues(DataGrid dataGrid, SortableCollectionView rows)
        {

            StackPanel sp = GetFooter(dataGrid);
            TextBlock kcalsTB = (TextBlock)VisualTreeHelper.GetChild(sp, 0);
            TextBlock proteinTB = (TextBlock)VisualTreeHelper.GetChild(sp, 1);
            TextBlock fatTB = (TextBlock)VisualTreeHelper.GetChild(sp, 2);
            TextBlock carbsTB = (TextBlock)VisualTreeHelper.GetChild(sp, 3);

            decimal kcals = 0m;
            decimal protein = 0m;
            decimal fat = 0m;
            decimal carbs = 0m;

            foreach (Row row in rows)
            {
                //kcals += String.IsNullOrEmpty((String)row["dc_kcals"]) ? 0d : Convert.ToDouble(row["dc_kcals"]);
                protein += String.IsNullOrEmpty((String)row["dc_protein"]) ? 0m : Convert.ToDecimal(row["dc_protein"]);
                fat += String.IsNullOrEmpty((String)row["dc_fat"]) ? 0m : Convert.ToDecimal(row["dc_fat"]);
                carbs += String.IsNullOrEmpty((String)row["dc_carbohydrate"]) ? 0m : Convert.ToDecimal(row["dc_carbohydrate"]);
            }
            kcals += (fat * Convert.ToDecimal(App.GramToKcalMultipler.Fat) + protein * Convert.ToDecimal(App.GramToKcalMultipler.Protein) + carbs * Convert.ToDecimal(App.GramToKcalMultipler.Carbohydrate));

            kcalsTB.Text = "Kcals: " + Math.Round(kcals, 2);
            proteinTB.Text = "Protein: " + Math.Round(protein, 2) + "g";
            fatTB.Text = "Fat: " + Math.Round(fat, 2) + "g";
            carbsTB.Text = "Carbs: " + Math.Round(carbs, 2) + "g";


        }
        private void SizeGrid(DataGrid dataGrid)
        {
            dataGrid.Visibility = System.Windows.Visibility.Visible;

            dataGrid.BorderThickness = new Thickness(1);

            // Space available to fill ( -18 Standard vScrollbar)
            double space_available = (LayoutRoot.ActualWidth - 18 - 80 - 35); //18 is width of scroll bar, 150 is width of menu 
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
                    var footer = (MealFooter)VisualTreeHelper.GetChild(parent, x + 1);
                    var grid = (Grid)VisualTreeHelper.GetChild(footer, 0);
                    return ((StackPanel)VisualTreeHelper.GetChild(grid, 0));
                }
            }

            return (null);
        }
    }
}

