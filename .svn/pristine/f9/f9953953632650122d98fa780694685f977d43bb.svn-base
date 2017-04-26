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
using System.Xml.Linq;
using DynamicConnections.NutriStyle.MenuGenerator.Engine.Helpers;
using System.Windows.Data;
using System.Windows.Controls.DataVisualization.Charting;
using System.Text;
using System.Windows.Controls.Primitives;

using DynamicConnections.NutriStyle.MenuGenerator.Controls.Scroll;
using System.ComponentModel;


namespace DynamicConnections.NutriStyle.MenuGenerator.ChildWindows
{
    public partial class Nutrients : ChildWindow
    {

        private Guid menuId;
        private int dayNumber;
        private Guid contactId;
        private String entityName;
        private DateTime date;
        private Guid foodId;

        Dictionary<String, String> names = new Dictionary<string, string>();
        private RowIndexConverter _rowIndexConverter = new RowIndexConverter();
        SortableCollectionView grams = new SortableCollectionView();
        SortableCollectionView percents = new SortableCollectionView();
        SortableCollectionView multipleRecommendations = new SortableCollectionView();

        public Nutrients()
        {
            InitializeComponent();
        }
        public Nutrients(Guid menuId, int dayNumber, Guid contactId, String entityName, DateTime date)
        {
            InitializeComponent();
            this.menuId = menuId;
            this.dayNumber = dayNumber;
            this.contactId = contactId;
            this.entityName = entityName;
            this.date = new DateTime(date.Year, date.Month, date.Day);
            RetrieveData();
            RetrieveDataForEachFood();
            //dataGridForEachFood.HorizontalScroll
            PleaseChoose.SelectedIndex = 0;
            
        }
        public Nutrients(Guid foodId)
        {
            InitializeComponent();
            this.foodId = foodId;
            RetrieveDataForEachFood();
        }
        private void RetrieveDataForEachFood()
        {
            try
            {
                CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
                cms.FoodNutrientsForEachFoodCompleted += new EventHandler<CrmSdk.FoodNutrientsForEachFoodCompletedEventArgs>(cms_RetrieveDataForEachFood);
                //cms.FoodNutrientsForEachFoodAsync(menuId.ToString(), Convert.ToString(dayNumber), contactId.ToString(), "dc_mealfood", null);

                if (!String.IsNullOrEmpty(entityName) && entityName.Equals("dc_mealfood", StringComparison.OrdinalIgnoreCase))
                {
                    cms.FoodNutrientsForEachFoodAsync(menuId.ToString(), Convert.ToString(dayNumber), contactId.ToString(), entityName, null);
                }
                else if (!String.IsNullOrEmpty(entityName) && entityName.Equals("dc_foodlog", StringComparison.OrdinalIgnoreCase))
                {
                    cms.FoodNutrientsForEachFoodAsync(menuId.ToString(), Convert.ToString(-1), contactId.ToString(), entityName, date.ToString());
                }
                else if (foodId != null && foodId != Guid.Empty)
                {
                    cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
                    cms.FoodNutrientsFoodIdCompleted += new EventHandler<CrmSdk.FoodNutrientsFoodIdCompletedEventArgs>(cms_RetrieveDataFoodId);
                    cms.FoodNutrientsFoodIdAsync(foodId.ToString());
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
            }
        }
        private void cms_RetrieveDataFoodId(object sender, CrmSdk.FoodNutrientsFoodIdCompletedEventArgs e)
        {
              
            SortableCollectionView data = new SortableCollectionView();
            try
            {
                XElement element = e.Result;

                if (element.Descendants("columns").Count() > 0)
                {
                    XElement xEl = element.Descendants("columns").ToList()[0];//get first one
                    //MessageBox.Show(xEl.ToString());
                    foreach (XElement xe in xEl.Elements())
                    {
                        if (!xe.Name.LocalName.Equals("dc_portionsize") &&
                            !xe.Name.LocalName.Equals("dc_portiontypeid") &&
                            !xe.Name.LocalName.Equals("dc_foodid") &&
                            (!xe.Name.LocalName.Equals("dc_meal") &&
                            !xe.Name.LocalName.Equals("dc_meal.dc_meal"))

                            )
                        {

                            data.ColumnTypes.Add(xe.Name.LocalName, xe.Attribute("Type").Value);

                            DataGridTextColumn dg = new DataGridTextColumn();
                            dg.Header = xe.Attribute("LabelName").Value;
                            dg.CanUserSort = true;
                            dg.SortMemberPath = xe.Name.LocalName;
                            dg.Binding = new Binding("Data")
                            {
                                Converter = _rowIndexConverter,
                                ConverterParameter = xe.Name.LocalName
                            };
                            dg.Width = new DataGridLength(45d);
                            dataGridForEachFood.Columns.Add(dg);
                        }
                    }
                    foreach (XElement xe in xEl.Elements())
                    {
                        if (xe.Name.LocalName.Equals("dc_portionsize") ||
                            xe.Name.LocalName.Equals("dc_portiontypeid") ||
                            xe.Name.LocalName.Equals("dc_foodid") ||
                            xe.Name.LocalName.Equals("dc_meal") ||
                            xe.Name.LocalName.Equals("dc_meal.dc_meal")

                            )
                        {

                            data.ColumnTypes.Add(xe.Name.LocalName, xe.Attribute("Type").Value);

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
                                dg.Visibility = System.Windows.Visibility.Collapsed;
                            }
                            dg.IsReadOnly = true;
                            if (xe.Name.LocalName.Equals("dc_foodid"))
                            {
                                dg.Width = new DataGridLength(280d);
                            }
                            else if (xe.Name.LocalName.Equals("dc_portionsize"))
                            {
                                dg.Width = new DataGridLength(65d);
                            }
                            else if (xe.Name.LocalName.Equals("dc_portiontypeid"))
                            {
                                dg.Width = new DataGridLength(100d);
                            }
                            else if (xe.Name.LocalName.Equals("dc_meal.dc_meal") || xe.Name.LocalName.Equals("dc_meal"))
                            {
                                dg.Width = new DataGridLength(110d);
                                dg.IsReadOnly = true;
                                dg.CellStyle = (Style)StyleController.FindResource("disabledCellStyle");
                            }
                            else
                            {
                                dg.Width = new DataGridLength(45d);
                            }
                            dataGridForEachFoodNoScroll.Columns.Add(dg);
                        }
                    }

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
                                rowData[type.Key] = new Pair(String.Empty, Guid.Empty.ToString());
                            }
                            if (type.Value.Equals("Picklist", StringComparison.OrdinalIgnoreCase))
                            {
                                rowData[type.Key] = "1";//new Pair(String.Empty, "-1");
                            }
                            if (type.Value.Equals("DateTime", StringComparison.OrdinalIgnoreCase))
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
                                rowData[xe.Name.LocalName] = new Pair(xe.Value, xe.Attribute("Id").Value);
                            }
                            else if (data.ColumnTypes.ContainsKey(xe.Name.LocalName) && data.ColumnTypes[xe.Name.LocalName].Equals("picklist", StringComparison.OrdinalIgnoreCase))
                            {
                                rowData[xe.Name.LocalName] = new Pair(xe.Value, xe.Attribute("Id").Value);
                            }
                            else
                            {
                                rowData[xe.Name.LocalName] = xe.Value;
                            }
                            if (data.ColumnTypes.ContainsKey("dc_meal.dc_meal") && xe.Name.LocalName.Equals("dc_meal.dc_meal", StringComparison.OrdinalIgnoreCase))
                            {
                                rowData["dc_meal.dc_mealid"] = xe.Attribute("Id").Value;
                            }
                        }
                        data.Add(rowData);
                    }
                    //set sort
                    data.SortDescriptions.Add(new SortDescription("dc_meal.dc_mealid", ListSortDirection.Ascending));

                    dataGridForEachFood.ItemsSource = data;
                    dataGridForEachFoodNoScroll.ItemsSource = data;

                    dataGridForEachFood.Visibility = System.Windows.Visibility.Visible;
                    dataGridForEachFood.SelectedIndex = 0;

                    dataGridForEachFood.BorderThickness = new Thickness(1);

                    dataGridForEachFoodNoScroll.Visibility = System.Windows.Visibility.Visible;
                    dataGridForEachFoodNoScroll.SelectedIndex = 0;

                    dataGridForEachFoodNoScroll.BorderThickness = new Thickness(1);
                    /*
                    var scrollBar = dataGridForEachFood.Descendents()
                      .OfType<ScrollBar>()
                      .FirstOrDefault(sb => sb.Name == "HorizontalScrollbar");
                    scrollBar.Visibility = System.Windows.Visibility.Visible;
                     * */

                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
            }
            busyIndicatorForEachFood.IsBusy = false;


        }
        private void cms_RetrieveDataForEachFood(object sender, CrmSdk.FoodNutrientsForEachFoodCompletedEventArgs e)
        {
            SortableCollectionView data = new SortableCollectionView();
            try
            {
                XElement element = e.Result;

                if (element.Descendants("columns").Count() > 0)
                {
                    XElement xEl = element.Descendants("columns").ToList()[0];//get first one
                    //MessageBox.Show(xEl.ToString());
                    foreach (XElement xe in xEl.Elements())
                    {
                        if (!xe.Name.LocalName.Equals("dc_portionsize") && 
                            !xe.Name.LocalName.Equals("dc_portiontypeid") && 
                            !xe.Name.LocalName.Equals("dc_foodid") &&
                            (!xe.Name.LocalName.Equals("dc_meal") &&
                            !xe.Name.LocalName.Equals("dc_meal.dc_meal"))
                            
                            )
                        {

                            data.ColumnTypes.Add(xe.Name.LocalName, xe.Attribute("Type").Value);

                            DataGridTextColumn dg = new DataGridTextColumn();
                            dg.Header = xe.Attribute("LabelName").Value;
                            dg.CanUserSort = true;
                            dg.SortMemberPath = xe.Name.LocalName;
                            dg.Binding = new Binding("Data")
                            {
                                Converter = _rowIndexConverter,
                                ConverterParameter = xe.Name.LocalName
                            };
                            dg.Width = new DataGridLength(45d);
                            dataGridForEachFood.Columns.Add(dg);
                        }
                    }
                    foreach (XElement xe in xEl.Elements())
                    {
                        if (xe.Name.LocalName.Equals("dc_portionsize") ||
                            xe.Name.LocalName.Equals("dc_portiontypeid") ||
                            xe.Name.LocalName.Equals("dc_foodid") ||
                            xe.Name.LocalName.Equals("dc_meal") ||
                            xe.Name.LocalName.Equals("dc_meal.dc_meal")

                            )
                        {

                            data.ColumnTypes.Add(xe.Name.LocalName, xe.Attribute("Type").Value);

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
                                dg.Visibility = System.Windows.Visibility.Collapsed;
                            }
                            dg.IsReadOnly = true;
                            if (xe.Name.LocalName.Equals("dc_foodid"))
                            {
                                dg.Width = new DataGridLength(280d);
                            }
                            else if (xe.Name.LocalName.Equals("dc_portionsize"))
                            {
                                dg.Width = new DataGridLength(65d);
                            }
                            else if (xe.Name.LocalName.Equals("dc_portiontypeid"))
                            {
                                dg.Width = new DataGridLength(100d);
                            }
                            else if (xe.Name.LocalName.Equals("dc_meal.dc_meal") || xe.Name.LocalName.Equals("dc_meal"))
                            {
                                dg.Width = new DataGridLength(110d);
                                dg.IsReadOnly = true;
                                dg.CellStyle = (Style)StyleController.FindResource("disabledCellStyle");
                            }
                            else
                            {
                                dg.Width = new DataGridLength(45d);
                            }
                            dataGridForEachFoodNoScroll.Columns.Add(dg);
                        }
                    }

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
                                rowData[type.Key] = new Pair(String.Empty, Guid.Empty.ToString());
                            }
                            if (type.Value.Equals("Picklist", StringComparison.OrdinalIgnoreCase))
                            {
                                rowData[type.Key] = "1";//new Pair(String.Empty, "-1");
                            }
                            if (type.Value.Equals("DateTime", StringComparison.OrdinalIgnoreCase))
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
                                rowData[xe.Name.LocalName] = new Pair(xe.Value, xe.Attribute("Id").Value);
                            }
                            else if (data.ColumnTypes.ContainsKey(xe.Name.LocalName) && data.ColumnTypes[xe.Name.LocalName].Equals("picklist", StringComparison.OrdinalIgnoreCase))
                            {
                                rowData[xe.Name.LocalName] = new Pair(xe.Value, xe.Attribute("Id").Value);
                            }
                            else
                            {
                                rowData[xe.Name.LocalName] = xe.Value;
                            }
                            if (data.ColumnTypes.ContainsKey("dc_meal.dc_meal") && xe.Name.LocalName.Equals("dc_meal.dc_meal", StringComparison.OrdinalIgnoreCase))
                            {
                                rowData["dc_meal.dc_mealid"] = xe.Attribute("Id").Value;
                            }
                        }
                        data.Add(rowData);
                    }
                    //set sort
                    data.SortDescriptions.Add(new SortDescription("dc_meal.dc_mealid", ListSortDirection.Ascending));

                    dataGridForEachFood.ItemsSource = data;
                    dataGridForEachFoodNoScroll.ItemsSource = data;

                    dataGridForEachFood.Visibility = System.Windows.Visibility.Visible;
                    dataGridForEachFood.SelectedIndex = 0;

                    dataGridForEachFood.BorderThickness = new Thickness(1);

                    dataGridForEachFoodNoScroll.Visibility = System.Windows.Visibility.Visible;
                    dataGridForEachFoodNoScroll.SelectedIndex = 0;

                    dataGridForEachFoodNoScroll.BorderThickness = new Thickness(1);
                    /*
                    var scrollBar = dataGridForEachFood.Descendents()
                      .OfType<ScrollBar>()
                      .FirstOrDefault(sb => sb.Name == "HorizontalScrollbar");
                    scrollBar.Visibility = System.Windows.Visibility.Visible;
                     * */
                    
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
            }
            busyIndicatorForEachFood.IsBusy = false;


        }


        private void RetrieveData()
        {
            try
            {
                CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
                cms.FoodNutrientsCompleted += new EventHandler<CrmSdk.FoodNutrientsCompletedEventArgs>(cms_RetrieveData);
                if (entityName.Equals("dc_mealfood", StringComparison.OrdinalIgnoreCase))
                {
                    cms.FoodNutrientsAsync(menuId.ToString(), Convert.ToString(dayNumber), contactId.ToString(), entityName, null);
                }
                else if (entityName.Equals("dc_foodlog", StringComparison.OrdinalIgnoreCase))
                {
                    cms.FoodNutrientsAsync(menuId.ToString(), Convert.ToString(-1), contactId.ToString(), entityName, date.ToString());
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
            }
        }
        private void cms_RetrieveData(object sender, CrmSdk.FoodNutrientsCompletedEventArgs e)
        {
            var element = e.Result;
            //
            if (element.Descendants("columns").Count() > 0)
            {   
                XElement xEl = element.Descendants("columns").ToList()[0];

                foreach (XElement xe in xEl.Elements())
                {
                    names.Add(xe.Name.LocalName, xe.Attribute("LabelName").Value);
                }
            }
            XElement xEl2 = element.Descendants("grams").ToList()[0];

            IEnumerable<XElement> list1 = from el in xEl2.Descendants() where (string)el.Attribute("RDI").Value != "Multiple Recommendations" select el;

            IEnumerable<XElement> list2 = from el in xEl2.Descendants() where (string)el.Attribute("RDI").Value == "Multiple Recommendations" select el;


            DataGridTextColumn dgName = new DataGridTextColumn();
            dgName.Header = "Nutrients";
            dgName.CanUserSort = true;
            dgName.SortMemberPath = "Name";
            dgName.Binding = new Binding("Data")
            {
                Converter = _rowIndexConverter,
                ConverterParameter = "name"
            };
            dgName.Foreground = new SolidColorBrush(Color.FromArgb(255, 76, 151, 204));
            dgName.Visibility = System.Windows.Visibility.Visible;
            dgName.IsReadOnly = true;
            dgName.Width = new DataGridLength(200);
            dataGridDRI.Columns.Add(dgName);


            DataGridTextColumn dgValue = new DataGridTextColumn();
            dgValue.Header = "Your Daily Intake";
            dgValue.CanUserSort = true;
            dgValue.SortMemberPath = "Value";
            dgValue.Binding = new Binding("Data")
            {
                Converter = _rowIndexConverter,
                ConverterParameter = "value"
            };
            dgValue.Foreground = new SolidColorBrush(Color.FromArgb(255, 76, 151, 204));
            dgValue.Visibility = System.Windows.Visibility.Visible;
            dgValue.IsReadOnly = true;
            dgValue.Width = new DataGridLength(710);
            dataGridDRI.Columns.Add(dgValue);

            //Populate first grid
            if (list2.Count() > 0)
            {
                foreach (XElement xe in list2)
                {
                    Row r = new Row();
                    r["name"] = names[xe.Name.LocalName];
                    bool missing = xe.Attribute("missing") != null ? Convert.ToBoolean(xe.Attribute("missing").Value) : false;
                    if (missing)
                    {
                        if (!String.IsNullOrEmpty(xe.Value))
                        {
                            r["value"] = xe.Value + "*";
                        }
                        else
                        {
                            r["value"] = "0*";
                        }
                    }
                    else
                    {
                        r["value"] = xe.Value;
                    }
                   
                        multipleRecommendations.Add(r);
                    
                }
            }
            dataGridDRI.ItemsSource = multipleRecommendations;
            busyIndicatorDRI.IsBusy = false;


            if (list1.Count() > 0)
            {
                //XElement xEl = element.Descendants("grams").ToList()[0];

                foreach (XElement xe in list1)
                {
                    Row r = new Row();

                    r["name"] = names[xe.Name.LocalName];
                    bool missing = xe.Attribute("missing") != null ? Convert.ToBoolean(xe.Attribute("missing").Value) : false;
                    if (missing)
                    {
                        if (!String.IsNullOrEmpty(xe.Value))
                        {
                            r["value"] = xe.Value + "*";
                        }
                        else
                        {
                            r["value"] ="0*";
                        }
                    }
                    else
                    {
                        r["value"] = xe.Value;
                    }
                    r["dri"] = xe.Attribute("RDI").Value;
                    r["di"] = xe.Attribute("DI").Value;
                    grams.Add(r);
                }
            }

            List<BarChartData> barData = new List<BarChartData>();
            if (list1.Count() > 0)
            {
                XElement xEl = element.Descendants("grams").ToList()[0];

                foreach (XElement xe in list1)
                {
                    Row r = new Row();
                    //if (!xe.Attribute("RDI").Value.Equals("Multiple Recommendations", StringComparison.OrdinalIgnoreCase))
                    //{
                        r["name"]   = names[xe.Name.LocalName];
                        if (String.IsNullOrEmpty(xe.Attribute("DI").Value))
                        {
                            r["value"] = "0";//String.Empty; //Math.Round(Convert.ToDecimal(xe.Value), 2);
                        }
                        else
                        {
                            r["value"] = Math.Round(Convert.ToDecimal(xe.Attribute("DI").Value.Replace("%", "")), 2).ToString();
                        }
                        percents.Add(r);
                        String name = names[xe.Name.LocalName].Length > 16 ? names[xe.Name.LocalName].Substring(0, 16) : names[xe.Name.LocalName];
                        barData.Add(new BarChartData() { Name = name, Value = Math.Round(Convert.ToDecimal((String)r["value"]), 2) });
                    //}
                }
            }
            //reverse the order
            barData.Reverse();

            DataGridTextColumn dg = new DataGridTextColumn();
            dg.Header = "Nutrients";
            dg.CanUserSort = true;
            dg.SortMemberPath = "Name";
            dg.Binding = new Binding("Data")
            {
                Converter = _rowIndexConverter,
                ConverterParameter = "name"
            };
            dg.Foreground = new SolidColorBrush(Color.FromArgb(255, 76, 151, 204));
            dg.Visibility = System.Windows.Visibility.Visible;
            dg.IsReadOnly = true;
            dg.Width = new DataGridLength(200);
            dataGrid1.Columns.Add(dg);

            DataGridTextColumn dg2 = new DataGridTextColumn();
            dg2.CanUserSort = true;
            dg2.Header = "Your Daily Intake";
            dg2.SortMemberPath = "value";
            dg2.Binding = new Binding("Data")
            {
                Converter = _rowIndexConverter,
                ConverterParameter = "value"
            };
            dg2.Foreground = new SolidColorBrush(Color.FromArgb(255, 76, 151, 204));
            dg2.Visibility = System.Windows.Visibility.Visible;
            dg2.IsReadOnly = true;
            dg2.Width = new DataGridLength(150);

            dataGrid1.Columns.Add(dg2);

            DataGridTextColumn dg3 = new DataGridTextColumn();
            dg3.CanUserSort = true;
            dg3.Header = "Your Recommended Daily Value";
            dg3.SortMemberPath = "RDI";
            dg3.Binding = new Binding("Data")
            {
                Converter = _rowIndexConverter,
                ConverterParameter = "dri"
            };
            dg3.Foreground = new SolidColorBrush(Color.FromArgb(255, 76, 151, 204));
            dg3.Visibility = System.Windows.Visibility.Visible;
            dg3.IsReadOnly = true;
            dg3.Width = new DataGridLength(225);

            dataGrid1.Columns.Add(dg3);

            DataGridTextColumn dg4 = new DataGridTextColumn();
            dg4.CanUserSort = true;
            dg4.Header = "% of Recommended Daily Intake";
            dg4.SortMemberPath = "di";
            dg4.Binding = new Binding("Data")
            {
                Converter = _rowIndexConverter,
                ConverterParameter = "di"
            };
            dg4.Foreground = new SolidColorBrush(Color.FromArgb(255, 76, 151, 204));
            dg4.Visibility = System.Windows.Visibility.Visible;
            dg4.IsReadOnly = true;
            dg4.Width = new DataGridLength(225);

            dataGrid1.Columns.Add(dg4);
            
            dataGrid1.ItemsSource = grams;
            busyIndicator.IsBusy = false;

            //Grid is complete.  Build out percents

            ((BarSeries)barChart.Series[0]).ItemsSource = barData;



        }



        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        

        private void PleaseChoose_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            if (cb.SelectedIndex == 0)
            {
                busyIndicatorForEachFood.Visibility = System.Windows.Visibility.Collapsed;
                ScrollViewer.Visibility = System.Windows.Visibility.Visible;
            }
            else if (cb.SelectedIndex == 1)
            {
                busyIndicatorForEachFood.Visibility = System.Windows.Visibility.Visible;
                ScrollViewer.Visibility = System.Windows.Visibility.Collapsed;
                
            }
        }
        private void VertScroll1(object sender, ScrollEventArgs e)
        {
            dataGridForEachFood.Scroll(ScrollMode.Vertical,
                dataGridForEachFoodNoScroll.GetScrollPosition(ScrollMode.Vertical));
        }

        private void VertScroll2(object sender, ScrollEventArgs e)
        {
            dataGridForEachFoodNoScroll.Scroll(ScrollMode.Vertical,
                dataGridForEachFood.GetScrollPosition(ScrollMode.Vertical));
        }

        private void ChildWindow_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
    }
}

