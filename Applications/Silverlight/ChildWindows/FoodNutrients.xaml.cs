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
    public partial class FoodNutrients : ChildWindow
    {

        private Guid foodId;

        Dictionary<String, String> names = new Dictionary<string, string>();
        private RowIndexConverter _rowIndexConverter = new RowIndexConverter();
        SortableCollectionView grams = new SortableCollectionView();
        SortableCollectionView percents = new SortableCollectionView();
        SortableCollectionView multipleRecommendations = new SortableCollectionView();

        public FoodNutrients()
        {
            InitializeComponent();
        }

        public FoodNutrients(Guid foodId)
        {
            InitializeComponent();
            this.foodId = foodId;
            RetrieveDataForEachFood();
        }
        public FoodNutrients(Guid foodId, String foodName)
        {
            InitializeComponent();
            this.foodId = foodId;
            RetrieveDataForEachFood();
            FoodName.Text = foodName;
        }
        private void RetrieveDataForEachFood()
        {
            try
            {
                if (foodId != null && foodId != Guid.Empty)
                {
                    CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
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
                data.ColumnNames = new Dictionary<string, string>();
                if (element.Descendants("columns").Count() > 0)
                {
                    XElement xEl = element.Descendants("columns").ToList()[0];//get first one
                    //MessageBox.Show(xEl.ToString());

                    foreach (XElement xe in xEl.Elements())
                    {

                        data.ColumnTypes.Add(xe.Name.LocalName, xe.Attribute("Type").Value);
                        data.ColumnNames.Add(xe.Name.LocalName, xe.Attribute("LabelName").Value);

                        Row rowData = new Row();
                        rowData["name"] = (String)data.ColumnNames[xe.Name.LocalName];
                        rowData["key"] = xe.Name.LocalName;
                        data.Add(rowData);

                    }

                    DataGridTextColumn dg = new DataGridTextColumn();
                    dg.Header = "Nutrient";
                    dg.CanUserSort = false;
                    dg.SortMemberPath = "Nutrient Name";
                    dg.Binding = new Binding("Data")
                    {
                        Converter = _rowIndexConverter,
                        ConverterParameter = "name"
                    };

                    dataGrid.Columns.Add(dg);

                    dg = new DataGridTextColumn();
                    dg.Header = "Value";
                    dg.CanUserSort = false;
                    dg.SortMemberPath = "Nutrient Value";
                    dg.Binding = new Binding("Data")
                    {
                        Converter = _rowIndexConverter,
                        ConverterParameter = "value"
                    };

                    dataGrid.Columns.Add(dg);


                    //Bind data

                    var rows = element.Descendants("dc_foods");
                    foreach (var row in rows)
                    {

                        //MessageBox.Show(row.ToString());
                        /*
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
                        }*/
                        foreach (XElement xe in row.Elements())
                        {
                            var node = from x in data where (string)x["key"] == xe.Name.LocalName select x;
                            Row r = ((Row)node.FirstOrDefault());
                            if (xe.Name.LocalName.Equals("dc_foodsid"))
                            {
                                data.Remove(r);
                            }
                            else
                            {
                                r["value"] = xe.Value;
                            }
                        }

                        /*
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
                        }*/
                        //data.Add(rowData);
                    }
                    //set sort
                    //data.SortDescriptions.Add(new SortDescription("dc_name", ListSortDirection.Ascending));

                    dataGrid.ItemsSource = data;

                    dataGrid.Visibility = System.Windows.Visibility.Visible;
                    dataGrid.SelectedIndex = 0;

                    dataGrid.BorderThickness = new Thickness(1);

                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
            }
            busyIndicator.IsBusy = false;

        }


        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}

