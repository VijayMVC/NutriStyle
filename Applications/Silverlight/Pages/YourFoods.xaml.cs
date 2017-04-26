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
using System.Windows.Media.Imaging;
using DynamicConnections.NutriStyle.MenuGenerator.Controls;

namespace DynamicConnections.NutriStyle.MenuGenerator.Pages
{
    public partial class YourFoods : Page
    {

        private RowIndexConverter _rowIndexConverter = new RowIndexConverter();
        private List<String> selectedDays = new List<string>();
        private List<String> selectedMeals = new List<string>();

        List<KeyValuePair<Guid, bool>> recipeList = new List<KeyValuePair<Guid,bool>>();
        
        public YourFoods()
        {
            InitializeComponent();
            dataGrid1.LoadingRow    += new EventHandler<DataGridRowEventArgs>(dataGrid1_LoadingRow);
            RetrieveUserFoods(String.Empty);
            name.Content            = ((App)App.Current).contact.Email;
        }

        void dataGrid1_LoadingRow(object sender, DataGridRowEventArgs e)
        {
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
                            if (((String)c.Header).Equals("Del", StringComparison.OrdinalIgnoreCase))
                            {
                                Button ccb = c.GetCellContent(e.Row) as Button;
                                ccb.Click -= ccb_MouseLeftButtonUp;
                                ccb.Click += new RoutedEventHandler(ccb_MouseLeftButtonUp);
                            }
                            else if (((String)c.Header).Equals(String.Empty, StringComparison.OrdinalIgnoreCase))
                            {
                                //default 
                                Button ccb = c.GetCellContent(e.Row) as Button;
                                if (ccb.Name.ToString().Equals("edit", StringComparison.OrdinalIgnoreCase))
                                {
                                    ccb.Click -= ccb_ClickEdit;
                                    ccb.Click += new RoutedEventHandler(ccb_ClickEdit);
                                }
                                //nutrients
                                else if (ccb.Name.ToString().Equals("Nutrients", StringComparison.OrdinalIgnoreCase))
                                {

                                    ccb.Click -= ShowFoodNutrients;
                                    ccb.Click += new RoutedEventHandler(ShowFoodNutrients);
                                }
                                //recipe
                                else if (ccb.Name.ToString().Equals("recipe", StringComparison.OrdinalIgnoreCase))
                                {

                                    ccb.Click -= ShowRecipe;
                                    ccb.Click += new RoutedEventHandler(ShowRecipe);
                                }

                            }
                            else if (((String)c.Header).Equals("edit", StringComparison.OrdinalIgnoreCase))
                            {
                                Button ccb = c.GetCellContent(e.Row) as Button;
                                ccb.Click -= ccb_ClickEdit;
                                ccb.Click += new RoutedEventHandler(ccb_ClickEdit);
                            }
                                /*
                            else if (((String)c.Header).Equals("nutrients", StringComparison.OrdinalIgnoreCase))
                            {
                                Button ccb = c.GetCellContent(e.Row) as Button;
                                
                            }*/
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
        /// Show food nutrients
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ShowFoodNutrients(object sender, RoutedEventArgs e)
        {
            
            Button b = sender as Button;
            Row row = (Row)b.DataContext;
            if (row["dc_foodsid"] != null && row["dc_foodsid"].GetType() == typeof(String))//Should normalize this
            {
                String Id = row["dc_foodsid"] as String;
                String name = row["dc_name"] as String;
                FoodNutrients n = new FoodNutrients(new Guid(Id), name);
                n.Show();
            }
        }
        /// <summary>
        /// Show recipe
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ShowRecipe(object sender, RoutedEventArgs e)
        {
            //open recipe food
            Button b = sender as Button;
            Row row = (Row)b.DataContext;
            if (row["dc_foodsid"] != null && row["dc_foodsid"].GetType() == typeof(String))//Should normalize this
            {
                String Id = row["dc_foodsid"] as String;
               
                Recipe r = new Recipe(new Guid(Id));
                r.Show();
            }
        }

        /// <summary>
        /// Edit Food
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ccb_ClickEdit(object sender, RoutedEventArgs e)
        {
            //open recipe food
            Button b = sender as Button;
            Row row = (Row)b.DataContext;
            //Recipe?
            String recipe = row["dc_recipefood"] as String;
            if (row["dc_foodsid"] != null && row["dc_foodsid"].GetType() == typeof(String))//Should normalize this
            {
                String Id = row["dc_foodsid"] as String;

                if (recipe.Equals("true", StringComparison.OrdinalIgnoreCase))
                {
                    ChildWindows.AddARecipe r = new ChildWindows.AddARecipe(new Guid(Id));
                    r.Show();
                    r.SavedClicked -= recipe_SavedClicked;
                    r.SavedClicked += new EventHandler(recipe_SavedClicked);
                }
                else
                {
                    ChildWindows.AddAFood f = new ChildWindows.AddAFood(new Guid(Id));
                    f.Show();
                    f.SavedClicked -= f_SavedClicked;
                    f.SavedClicked += new EventHandler(f_SavedClicked);
                }
            }
        }

        void f_SavedClicked(object sender, EventArgs e)
        {
            XElement element = (XElement)sender;
            var name = element.Descendants("dc_foods").First().Descendants("dc_name").First().Value;
            var foodId = element.Descendants("dc_foods").First().Descendants("dc_foodsid").First().Value;

            SortableCollectionView data = (SortableCollectionView)dataGrid1.ItemsSource;

            var node    = from x in data where (string)x["dc_foodsid"] == foodId select x;
            Row row     = ((Row)node.FirstOrDefault());
            if (row == null)
            {//new record
                row = new Row();
                //add needed fields
                row["dc_name"] = name;
                row["dc_foodsid"] = foodId;
                row["dc_recipefood"] = "false";
                row["visibility"] = Visibility.Collapsed.ToString();
                data.Add(row);
            }
            else
            {
                row["dc_name"] = name;
            }
        }

        void recipe_SavedClicked(object sender, EventArgs e)
        {
            XElement element = (XElement)sender;
            var name = element.Descendants("dc_foods").First().Descendants("dc_name").First().Value;
            var foodId = element.Descendants("dc_foods").First().Descendants("dc_foodsid").First().Value;

            SortableCollectionView data = (SortableCollectionView)dataGrid1.ItemsSource;

            var node = from x in data where (string)x["dc_foodsid"] == foodId select x;
            Row row = ((Row)node.FirstOrDefault());
            if (row == null)
            {//new record
                row = new Row();
                //add needed fields
                row["dc_name"] = name;
                row["dc_foodsid"] = foodId;
                row["dc_recipefood"] = "true";
                row["visibility"] = Visibility.Visible.ToString();
                data.Add(row);
            }
            else
            {
                row["dc_name"] = name;
            }
        }

        /// <summary>
        /// Delete food
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ccb_MouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
            try
            {
                SortableCollectionView data = (SortableCollectionView)dataGrid1.ItemsSource;

                Button img = sender as Button;
                String Id = (String)((Row)dataGrid1.SelectedItem)["dc_foodsid"];
                Row r = (Row)dataGrid1.SelectedItem;
                //Is recipe?
                //bool isRecipe = Boolean.Parse((String)((Row)dataGrid1.SelectedItem)["dc_recipefood"]);

                if (!String.IsNullOrEmpty(Id))
                {
                    XElement deleteXml = new XElement("dc_foods", new XElement("id", Id));

                    CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
                    cms.DeleteCompleted += new EventHandler<CrmSdk.DeleteCompletedEventArgs>(cms_Delete);
                    cms.DeleteAsync(deleteXml);
                }
                data.Remove(r);
                dataGrid1.ItemsSource = data;

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


        private void RetrieveUserFoods(String searchFor)
        {
            App ap = (App)App.Current;

            String fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true' page='1' count='25'>
              <entity name='dc_foods'>
                <attribute name='dc_name' />
                <attribute name='dc_recipefood' />
                <attribute name='dc_foodsid' />
                <order attribute='dc_name' descending='false' />
                <filter type='and'>
                    <condition attribute='dc_contactid' value='@CONTACTID' operator='eq'/>
                </filter>
                @FILTER
              </entity>
            </fetch>";

            String searchFilter = String.Empty;

            fetchXml = fetchXml.Replace("@CONTACTID", ap.contactId);

            if (!String.IsNullOrEmpty(searchFor))
            {
                searchFilter = @"<filter type='and'>
                    <condition attribute='dc_name' value='@SEARCHFOR' operator='@OPERATOR'/>
                </filter>";
                
                //eq or contains
                if (searchFor.Contains("*"))
                {
                    searchFilter = searchFilter.Replace("@OPERATOR", "like");
                }
                else
                {
                    searchFilter = searchFilter.Replace("@OPERATOR", "eq");
                }
                //replace * with %
                searchFor = searchFor.Replace("*", "%");
                
                searchFilter = searchFilter.Replace("@SEARCHFOR", searchFor);
            }
            fetchXml = fetchXml.Replace("@FILTER", searchFilter);

            XElement orderXml = new XElement("ColumnOrder");
            orderXml.Add(new XElement("Column", "dc_name"));
            orderXml.Add(new XElement("Column", "dc_recipefood"));
            orderXml.Add(new XElement("Column", "dc_foodsid"));
            
            try
            {
                busyIndicator.IsBusy = true;
                busyIndicator.Visibility = System.Windows.Visibility.Visible;

                CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
                cms.RetrieveFetchXmlCompleted += new EventHandler<CrmSdk.RetrieveFetchXmlCompletedEventArgs>(cms_RetrieveUserFoods);
                cms.RetrieveFetchXmlAsync(fetchXml, orderXml.ToString());
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
            }
        }
        private void cms_RetrieveUserFoods(object sender, CrmSdk.RetrieveFetchXmlCompletedEventArgs e)
        {
            buildGrid("dc_foods", dataGrid1, busyIndicator, e.Result, 1);
        }

        private void buildGrid(String entityName, DataGrid dataGrid, BusyIndicator bi, XElement element, int columns)
        {

            SortableCollectionView data = new SortableCollectionView();
            try
            {
                if (element.Descendants("columns").Count() > 0)
                {
                    XElement xEl = element.Descendants("columns").ToList()[0];//get first one

                    if (dataGrid.Columns.Count == 0)//only build out the first time.  Successive calls should not add more columns to the grid
                    {
                        foreach (XElement xe in xEl.Elements())
                        {
                            if (!xe.Name.LocalName.Equals("dc_recipefood"))
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
                                dataGrid.Columns.Add(dg);
                            }
                            else if (xe.Name.LocalName.Equals("dc_recipefood"))
                            {
                                DataGridTemplateColumn dgRecipe = new DataGridTemplateColumn();
                                dgRecipe.Header = String.Empty;
                                //dgImage.SortMemberPath = xe.Name.LocalName;
                                dgRecipe.CanUserSort = false;

                                StringBuilder CellETemp2 = new StringBuilder();
                                CellETemp2.Append("<DataTemplate ");
                                CellETemp2.Append("xmlns='http://schemas.microsoft.com/winfx/");
                                CellETemp2.Append("2006/xaml/presentation' ");
                                CellETemp2.Append("xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' >");
                                CellETemp2.Append("<Button Name='Recipe' BorderThickness='0' ToolTipService.ToolTip='View Recipe' Cursor='Hand' Width='21' Height='21' Visibility='{Binding Path=Data, Converter={StaticResource rowConvertor}, ConverterParameter=visibility, Mode=OneWay}'>");
                                CellETemp2.Append("<Image  HorizontalAlignment='Center'  Source='/DynamicConnections.NutriStyle.MenuGenerator;component/images/recipe-icon.png'/>");
                                CellETemp2.Append("</Button>");
                                CellETemp2.Append("</DataTemplate>");
                                dgRecipe.CellEditingTemplate = (DataTemplate)XamlReader.Load(CellETemp2.ToString());

                                dgRecipe.IsReadOnly = false;
                                dgRecipe.Width = new DataGridLength(30);

                                dgRecipe.Visibility = System.Windows.Visibility.Visible;
                                dataGrid.Columns.Add(dgRecipe);
                            }
                        }
                        //add image for edit
                        DataGridTemplateColumn dgImage = new DataGridTemplateColumn();
                        dgImage.Header = String.Empty;
                        //dgImage.SortMemberPath = xe.Name.LocalName;
                        dgImage.CanUserSort = false;

                        StringBuilder CellETemp = new StringBuilder();
                        CellETemp.Append("<DataTemplate ");
                        CellETemp.Append("xmlns='http://schemas.microsoft.com/winfx/");
                        CellETemp.Append("2006/xaml/presentation' ");
                        CellETemp.Append("xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' >");
                        CellETemp.Append("<Button Cursor='Hand' ToolTipService.ToolTip='View Nutrient Information' Width='23' Height='23' BorderThickness='0' HorizontalAlignment='Left' Name='Nutrients'>");
                        CellETemp.Append("<Image  HorizontalAlignment='Center'  Source='/DynamicConnections.NutriStyle.MenuGenerator;component/images/nutrition-icon.png'/>");
                        CellETemp.Append("</Button>");
                        CellETemp.Append("</DataTemplate>");
                        dgImage.CellEditingTemplate = (DataTemplate)XamlReader.Load(CellETemp.ToString());

                        dgImage.IsReadOnly = false;
                        dgImage.Width = new DataGridLength(30);
                        //visable?
                        dgImage.Visibility = System.Windows.Visibility.Visible;

                        dataGrid.Columns.Add(dgImage);

                        //add image for edit
                        dgImage = new DataGridTemplateColumn();
                        dgImage.Header = "Edit";
                        //dgImage.SortMemberPath = xe.Name.LocalName;
                        dgImage.CanUserSort = false;

                        CellETemp = new StringBuilder();
                        CellETemp.Append("<DataTemplate ");
                        CellETemp.Append("xmlns='http://schemas.microsoft.com/winfx/");
                        CellETemp.Append("2006/xaml/presentation' ");
                        CellETemp.Append("xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' >");
                        CellETemp.Append("<Button Cursor='Hand' ToolTipService.ToolTip='Edit' Width='45' Height='23' HorizontalAlignment='Left' Name='Edit' Content='Edit'>");
                        CellETemp.Append("</Button>");
                        CellETemp.Append("</DataTemplate>");
                        dgImage.CellEditingTemplate = (DataTemplate)XamlReader.Load(CellETemp.ToString());

                        dgImage.IsReadOnly = false;
                        dgImage.Width = new DataGridLength(50);
                        //visable?
                        dgImage.Visibility = System.Windows.Visibility.Visible;

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
                        CellETemp.Append("<Button Cursor='Hand' ToolTipService.ToolTip='Remove' Width='26' Height='26' HorizontalAlignment='Left'>");
                        CellETemp.Append("<Image  Stretch='Fill' Name='Delete' Source='/DynamicConnections.NutriStyle.MenuGenerator;component/images/delete.png'/>");// MouseLeftButtonDown='ImageDelete_MouseLeftButtonDown'
                        CellETemp.Append("</Button>");
                        CellETemp.Append("</DataTemplate>");
                        dgImage.CellEditingTemplate = (DataTemplate)XamlReader.Load(CellETemp.ToString());

                        dgImage.IsReadOnly = false;
                        dgImage.Width = new DataGridLength(34);
                        //visable?
                        dgImage.Visibility = System.Windows.Visibility.Visible;

                        dataGrid.Columns.Add(dgImage);
                    }
                    //Bind data
                    var rows = element.Descendants(entityName);
                    foreach (var row in rows)
                    {
                        Row rowData = new Row();
                        rowData["visibility"] = Visibility.Collapsed.ToString();
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

                            if (xe.Name.LocalName.Equals("dc_recipefood"))
                            {
                                if (xe.Value.Equals("true", StringComparison.OrdinalIgnoreCase))
                                {
                                    rowData["visibility"] = Visibility.Visible.ToString();
                                }
                            }
                        }
                        data.Add(rowData);
                    }

                    dataGrid.ItemsSource = data;

                    dataGrid.Visibility = System.Windows.Visibility.Visible;
                    dataGrid.SelectedIndex = 0;

                    dataGrid.BorderThickness = new Thickness(1);

                    // Space available to fill ( -18 Standard vScrollbar)
                    double space_available = (LayoutRoot.ActualWidth - 25-34 - 50 - 30 - 30); //18 is width of scroll bar, 150 is width of menu 
                    //figure out column types
                    
                    int count = 5;//id, edit, recipe and delete, nutrients column,
                    if (space_available > 0)
                    {
                        foreach (DataGridColumn dg_c in dataGrid1.Columns)
                        {
                            if (dg_c.Width.Value != 34 && dg_c.Width.Value != 50 && dg_c.Width.Value != 30 && dg_c.Width.Value != 75)
                            {
                                dg_c.Width = new DataGridLength((space_available / (dataGrid1.Columns.Count - count)));
                            }
                        }
                    }
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
            }
            bi.IsBusy = false;
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
        private void AddAFood_Click(object sender, RoutedEventArgs e)
        {
            ChildWindows.AddAFood aaf = new ChildWindows.AddAFood();
            aaf.Show();
            aaf.SavedClicked -= f_SavedClicked;
            aaf.SavedClicked += new EventHandler(f_SavedClicked);
        }
        private void AddARecipe_Click(object sender, RoutedEventArgs e)
        {
            ChildWindows.AddARecipe aar = new ChildWindows.AddARecipe();
            aar.Show();
            aar.SavedClicked -= recipe_SavedClicked;
            aar.SavedClicked += new EventHandler(recipe_SavedClicked);
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            RetrieveUserFoods(food.SelectedText);
        }
    }
}
