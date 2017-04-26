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

namespace DynamicConnections.NutriStyle.MenuGenerator.Pages
{
    public partial class FoodDislikes : Page
    {

        private RowIndexConverter _rowIndexConverter = new RowIndexConverter();
        String searchText;
        List<PairWithList> foodGroup = null;
        public FoodDislikes()
        {
            foodGroup = new List<PairWithList>();

            InitializeComponent();
            //Setup birthday values
            searchText = String.Empty;

            RetrieveFoodDislikes();
            //dataGrid1.MouseLeftButtonUp += new MouseButtonEventHandler(dataGrid1_MouseLeftButtonUp);
            dataGrid1.LoadingRow += new EventHandler<DataGridRowEventArgs>(dataGrid1_LoadingRow);
            name.Content = ((App)App.Current).contact.Email;
            PopulateFoodGroups();
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
                            Button ccb = c.GetCellContent(e.Row) as Button;
                            ccb.Click -= ccb_MouseLeftButtonUp;
                            ccb.Click += new RoutedEventHandler(ccb_MouseLeftButtonUp);
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

        void ccb_MouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
            try
            {
                SortableCollectionView data = (SortableCollectionView)dataGrid1.ItemsSource;

                Button img = sender as Button;
                String Id = (String)((Row)dataGrid1.SelectedItem)["dc_fooddislikeid"];
                Row r = (Row)dataGrid1.SelectedItem;

                XElement deleteXml = new XElement("dc_fooddislike", new XElement("id", Id));

                CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
                cms.DeleteCompleted += new EventHandler<CrmSdk.DeleteCompletedEventArgs>(cms_Delete);
                cms.DeleteAsync(deleteXml);

                data.Remove(r);
                dataGrid1.ItemsSource = data;

            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
            }
        }

        void dataGrid1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }
        private void cms_Delete(object sender, CrmSdk.DeleteCompletedEventArgs e)
        {
            //CrmIndicator.IsBusy = false;
            //place holder
        }


        private void RetrieveFoodDislikes()
        {
            App ap = (App)App.Current;

            String fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
              <entity name='dc_fooddislike'>
                <attribute name='dc_foodid' />
                <attribute name='dc_mealcomponentid' />
                <attribute name='dc_fooddislikeid' />
                <order attribute='dc_foodid' descending='false' />
                <filter type='and'>
                    <condition attribute='dc_contactid' value='@CONTACTID' operator='eq'/>
                </filter>
              </entity>
            </fetch>";

            fetchXml = fetchXml.Replace("@CONTACTID", ap.contactId);

            XElement orderXml = new XElement("ColumnOrder");
            orderXml.Add(new XElement("Column", "dc_foodid"));
            orderXml.Add(new XElement("Column", "dc_mealcomponentid"));
            orderXml.Add(new XElement("Column", "dc_fooddislikeid"));

            try
            {
                busyIndicator.IsBusy = true;
                busyIndicator.Visibility = System.Windows.Visibility.Visible;

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
            buildGrid("dc_fooddislike", dataGrid1, busyIndicator, e.Result, 1);
        }


        private void buildGrid(String entityName, DataGrid dataGrid, BusyIndicator bi, XElement element, int columns)
        {

            SortableCollectionView data = new SortableCollectionView();
            try
            {
                if (element.Descendants("columns").Count() > 0)
                {
                    XElement xEl = element.Descendants("columns").ToList()[0];//get first one
                    //MessageBox.Show(xEl.ToString());
                    foreach (XElement xe in xEl.Elements())
                    {
                        data.ColumnTypes.Add(xe.Name.LocalName, xe.Attribute("Type").Value);

                        DataGridTextColumn dg = new DataGridTextColumn();
                        if (xe.Name.LocalName.Equals("dc_foodid", StringComparison.OrdinalIgnoreCase))
                        {
                            dg.Header = "Unique Food";
                        }
                        else if (xe.Name.LocalName.Equals("dc_mealcomponentid", StringComparison.OrdinalIgnoreCase))
                        {
                            dg.Header = "Food Group";
                        }else {
                            dg.Header = xe.Attribute("LabelName").Value;
                        }
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
                        dg.Width = new DataGridLength(100);

                    }
                    //add image for delete


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
                    CellETemp.Append("<Button Cursor='Hand' ToolTipService.ToolTip='Remove From Food Dislike' Width='26' Height='26' HorizontalAlignment='Left'>");
                    CellETemp.Append("<Image  Stretch='Fill' Name='Delete' Source='/DynamicConnections.NutriStyle.MenuGenerator;component/images/delete.png'/>");// MouseLeftButtonDown='ImageDelete_MouseLeftButtonDown'
                    CellETemp.Append("</Button>");
                    CellETemp.Append("</DataTemplate>");
                    dgImage.CellEditingTemplate = (DataTemplate)XamlReader.Load(CellETemp.ToString());

                    dgImage.IsReadOnly = false;
                    dgImage.Width = new DataGridLength(34);
                    //visable?
                    dgImage.Visibility = System.Windows.Visibility.Visible;

                    dataGrid.Columns.Add(dgImage);

                    //Bind data
                    var rows = element.Descendants(entityName);
                    foreach (var row in rows)
                    {
                        Row rowData = new Row();

                        foreach (KeyValuePair<String, String> type in data.ColumnTypes)
                        {
                            //MessageBox.Show(type.Key + ":" + type.Value);
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

                        foreach (XElement xe in row.Elements())
                        {

                            if (data.ColumnTypes.ContainsKey(xe.Name.LocalName) && data.ColumnTypes[xe.Name.LocalName].Equals("money", StringComparison.OrdinalIgnoreCase))
                            {
                                rowData[xe.Name.LocalName] = String.Format("{0:C2}", Convert.ToDecimal(xe.Value));
                            }
                            else if (data.ColumnTypes.ContainsKey(xe.Name.LocalName) && data.ColumnTypes[xe.Name.LocalName].Equals("lookup", StringComparison.OrdinalIgnoreCase))
                            {
                                //MessageBox.Show(xe.Name.LocalName + ":" + xe.Value + ":" + xe.Attribute("Id").Value);
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
                        }
                        data.Add(rowData);
                    }

                    dataGrid.ItemsSource = data;

                    dataGrid.BorderThickness = new Thickness(1);

                    // Space available to fill ( -18 Standard vScrollbar)
                    double space_available = (LayoutRoot.ActualWidth - 18 - 30); //18 is width of scroll bar, 150 is width of menu 
                    //figure out column types

                    int count = 2;
                    if (space_available > 0)
                    {
                        foreach (DataGridColumn dg_c in dataGrid1.Columns)
                        {
                            if (dg_c.Width.Value != 34)
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
            dataGrid.Visibility = System.Windows.Visibility.Visible;

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

            if (foodUniqueRadio.IsChecked.Value)
            {
                XElement foodId = new XElement("dc_foodid", ((PairWithList)food.SelectedPair).Id);
                XAttribute attribute = new XAttribute("entityname", "dc_foods");
                foodId.Add(attribute);
                contactXml.Add(foodId);
            }
            else
            {
                XElement foodId = new XElement("dc_mealcomponentid", ((PairWithList)food.SelectedPair).Id);
                XAttribute attribute = new XAttribute("entityname", "dc_meal_component");
                foodId.Add(attribute);
                contactXml.Add(foodId);
            }

            CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
            cms.CreateUpdateCompleted += new EventHandler<CrmSdk.CreateUpdateCompletedEventArgs>(cms_CreateUpdateFoodLike);
            cms.CreateUpdateAsync(contactXml);

        }
        private void cms_CreateUpdateFoodLike(object sender, CrmSdk.CreateUpdateCompletedEventArgs e)
        {
            var errors = from x in e.Result.Descendants("error") select x;
            if (errors.Count() > 0)
            {

                String message = e.Result.Value.ToString();
                Status s = new Status(message, false);
                s.Show();

            }
            else
            {
                //var rows = xml.Descendants(entityName);

                var results = e.Result.Descendants("Success");

                //MessageBox.Show("Success");
                //NavigationService.Navigate(new Uri("MenuOptions", UriKind.Relative));
                SortableCollectionView data = (SortableCollectionView)dataGrid1.ItemsSource;
                Row r = new Row();
                if (foodUniqueRadio.IsChecked.Value)
                {
                    r["dc_foodid"] = food.SelectedPair;
                    r["dc_mealcomponentid"] = new Pair(String.Empty, Guid.Empty.ToString());
                }
                else
                {
                    r["dc_mealcomponentid"] = food.SelectedPair;
                    r["dc_foodid"] = new Pair(String.Empty, Guid.Empty.ToString());
                }

                r["dc_fooddislikeid"] = e.Result.Value;
                data.Add(r);
            }
        }

        /// <summary>
        /// Retrieve list of matching foods
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void food_KeyDown(object sender, KeyEventArgs e)
        {
            String text = food.MyAutoCompleteBox.Text;

            if (String.IsNullOrEmpty(searchText) || searchText.Length >= text.Length)
            {
                if (text.Length > 1 && (e.Key != Key.Down && e.Key != Key.Up && e.Key != Key.Delete && e.Key != Key.Back))
                {
                    if (String.IsNullOrEmpty(searchText) || (text.Length == searchText.Length))
                    {
                        searchText = text;
                    }
                    String fetchXml = String.Empty;
                   

                    if (foodUniqueRadio.IsChecked.Value)
                    {
                        try
                        {
                            CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
                            cms.RetrieveFoodDislikesCompleted += new EventHandler<CrmSdk.RetrieveFoodDislikesCompletedEventArgs>(cms_RetrieveFoods);
                            cms.RetrieveFoodDislikesAsync(General.ContactId(), text);
                        }
                        catch (Exception err)
                        {
                            MessageBox.Show(err.Message);
                            MessageBox.Show(err.StackTrace);
                        }
                    }
                    else
                    {
                       //do nothing.
                    }
                    
                }
            }
        }
        private void cms_RetrieveFoods(object sender, CrmSdk.RetrieveFoodDislikesCompletedEventArgs e)
        {
            String entityName = "dc_foods";
            if (foodGroupRadio.IsChecked.Value)
            {
                entityName = "dc_meal_component";
            }
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
                        data.Add(new PairWithList(name, Id));
                    }
                    food.SelectedPair = new PairWithList(String.Empty, String.Empty, String.Empty, data);
                    food.OpenDropdown(true);
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
            }

        }
        private void PopulateFoodGroups()
        {
            String fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
                <entity name='dc_meal_component'>
                <attribute name='dc_name' />
                <attribute name='dc_meal_componentid' />
                <order attribute='dc_name' descending='false' />
                </entity>
            </fetch>";

            XElement orderXml = new XElement("ColumnOrder");
            orderXml.Add(new XElement("Column", "dc_name"));
            orderXml.Add(new XElement("Column", "dc_meal_componentid"));

            try
            {
                CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
                cms.RetrieveFetchXmlCompleted += new EventHandler<CrmSdk.RetrieveFetchXmlCompletedEventArgs>(cms_RetrieveFoodGroups);
                cms.RetrieveFetchXmlAsync(fetchXml, orderXml.ToString());
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
            }
        }
        private void cms_RetrieveFoodGroups(object sender, CrmSdk.RetrieveFetchXmlCompletedEventArgs e)
        {
            String entityName = "dc_meal_component";
            
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
                        data.Add(new PairWithList(name, Id));
                    }
                    foodGroup = data;
                    food.SelectedPair = new PairWithList(String.Empty, String.Empty, String.Empty, foodGroup);
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
            }

        }

        private void foodUniqueRadio_Click(object sender, RoutedEventArgs e)
        {
            if (foodUniqueRadio.IsChecked.Value)
            {
                
                food.KeyDown += food_KeyDown;
                
                //AddFavorite.Content = "Add Food";
                food.SelectedPair = new PairWithList(String.Empty, String.Empty, String.Empty, new List<PairWithList>());
                //food.MyAutoCompleteBox.Text = String.Empty;
                food.MyAutoCompleteBox.SelectedItem = null;
                food.Watermark = "Type part of Food Name and Select from list";
                food.SetMode(true);
            }
        }

        private void foodGroupRadio_Click(object sender, RoutedEventArgs e)
        {
            if (foodGroupRadio.IsChecked.Value)
            {
                
                food.KeyDown -= food_KeyDown;
                
                //AddFavorite.Content = "Add Food Group";
                food.SelectedPair = new PairWithList(String.Empty, String.Empty, String.Empty, foodGroup);
                food.MyAutoCompleteBox.SelectedItem = null;
                //food.MyAutoCompleteBox.Text = String.Empty;
                food.OpenDropdown(false);
                food.Watermark = "Click Drop-down Arrow and Select from list";
                food.SetMode(true);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            App app = (App)App.Current;
            busyIndicator.IsBusy = true;
            Menu m = new Menu();
            m.Generate(new Guid(app.contactId), postMenuGenerate);

        }
        private void postMenuGenerate()
        {
            busyIndicator.IsBusy = false;
            NavigationService.Navigate(new Uri("DailyMenu", UriKind.Relative));
        }

    }
}
