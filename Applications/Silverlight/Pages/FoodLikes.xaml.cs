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
    public partial class FoodLikes : Page
    {

        private RowIndexConverter _rowIndexConverter = new RowIndexConverter();
        private List<String> selectedDays = new List<string>();
        private List<String> selectedMeals = new List<string>();

        private Dictionary<String, int> days = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        private Dictionary<String, int> meals = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        List<KeyValuePair<Guid, bool>> recipeList = new List<KeyValuePair<Guid,bool>>();
        String searchText;
        
        public FoodLikes()
        {
            searchText = String.Empty;
           
            InitializeComponent();

            dataGrid1.LoadingRow += new EventHandler<DataGridRowEventArgs>(dataGrid1_LoadingRow);
            
            RetrieveFoodLikes();
            RetrieveDays();
            RetrieveMeals();
            name.Content = ((App)App.Current).contact.Email;
            food.SelectionChanged += new EventHandler<SelectionChangedEventArgs>(food_SelectionChanged);
        }

        void food_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AutoCompleteBox acb = sender as AutoCompleteBox;
            if (acb.SelectedItem != null && !String.IsNullOrEmpty( ((PairWithList)acb.SelectedItem).Id))
            {
                if (recipeList.Contains(new KeyValuePair<Guid, bool>(new Guid(((PairWithList)acb.SelectedItem).Id), true)))
                {
                    food.IsRecipe = true;
                }
                else
                {
                    food.IsRecipe = false;
                }
            }
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
                            else
                            {
                                Button ccb = c.GetCellContent(e.Row) as Button;
                                ccb.Click -= ccb_Click;
                                ccb.Click += new RoutedEventHandler(ccb_Click); 
                            }
                        }
                        else if (c.GetCellContent(e.Row).GetType() == typeof(Button))
                        {
                            Button ccb = c.GetCellContent(e.Row) as Button;
                            ccb.Click -= ccb_Click;
                            ccb.Click += new RoutedEventHandler(ccb_Click);
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

        void ccb_Click(object sender, RoutedEventArgs e)
        {
            //open recipe food
            Button b = sender as Button;
            Row row = (Row)b.DataContext;
            if (row["dc_foodid"] != null && row["dc_foodid"].GetType() == typeof(Pair))
            {
                Pair pwl = row["dc_foodid"] as Pair;
                Recipe r = new Recipe(new Guid(pwl.Id));
                r.Show();

            }
            else if (row["dc_foodid"] != null && row["dc_foodid"].GetType() == typeof(PairWithList))//Should normalize this
            {
                PairWithList pwl = row["dc_foodid"] as PairWithList;
                Recipe r = new Recipe(new Guid(pwl.Id));
                r.Show();
            }
        }
        /// <summary>
        /// Delete food like
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ccb_MouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
            try
            {
                SortableCollectionView data = (SortableCollectionView)dataGrid1.ItemsSource;

                Button img = sender as Button;
                String Id = (String)((Row)dataGrid1.SelectedItem)["dc_foodlikeid"];
                Row r = (Row)dataGrid1.SelectedItem;
                if (!String.IsNullOrEmpty(Id))
                {
                    XElement deleteXml = new XElement("dc_foodlike", new XElement("id", Id));

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

        private void RetrieveDays()
        {

            CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
            cms.RetrieveOptionSetValuesCompleted += new EventHandler<CrmSdk.RetrieveOptionSetValuesCompletedEventArgs>(cms_RetrieveDays);
            cms.RetrieveOptionSetValuesAsync("dc_foodlike", "dc_day");
        }
        private void cms_RetrieveDays(object sender, CrmSdk.RetrieveOptionSetValuesCompletedEventArgs e)
        {
            try
            {
                ComboBoxItem itemSelected = new ComboBoxItem();
                List<Pair> list = new List<Pair>();
                var results = from x in e.Result.Descendants("pair") select x;

                foreach (var pair in results)
                {
                    Pair p = new Pair();
                    p.Name = pair.Descendants("name").First().Value;
                    p.Id = pair.Descendants("value").First().Value;

                    list.Add(p);
                    days.Add(p.Name, Convert.ToInt32(p.Id));
                }
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
        }
        private void cms_RetrieveMeals(object sender, CrmSdk.RetrieveOptionSetValuesCompletedEventArgs e)
        {
            try
            {
                ComboBoxItem itemSelected = new ComboBoxItem();
                List<Pair> list = new List<Pair>();
                var results = from x in e.Result.Descendants("pair") select x;

                foreach (var pair in results)
                {
                    Pair p = new Pair();
                    p.Name = pair.Descendants("name").First().Value.Replace(" ", "");
                    p.Id = pair.Descendants("value").First().Value;


                    list.Add(p);
                    meals.Add(p.Name, Convert.ToInt32(p.Id));
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
            }
        }

        private void RetrieveFoodLikes()
        {
            App ap = (App)App.Current;

            String fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
              <entity name='dc_foodlike'>
                <attribute name='dc_day' />
                <attribute name='dc_meal' />
                <attribute name='dc_foodid' />
                <attribute name='dc_foodlikeid' />
                <order attribute='dc_foodid' descending='false' />
                <filter type='and'>
                    <condition attribute='dc_contactid' value='@CONTACTID' operator='eq'/>
                </filter>
                <link-entity name='dc_foods' from='dc_foodsid' to='dc_foodid' alias='dc_foods'>
                    <attribute name='dc_recipefood' />
                </link-entity>
              </entity>
            </fetch>";

            fetchXml = fetchXml.Replace("@CONTACTID", ap.contactId);

            XElement orderXml = new XElement("ColumnOrder");
            orderXml.Add(new XElement("Column", "dc_day"));
            orderXml.Add(new XElement("Column", "dc_meal"));
            orderXml.Add(new XElement("Column", "dc_foodid"));
            orderXml.Add(new XElement("Column", "dc_foods.dc_recipefood"));
            orderXml.Add(new XElement("Column", "dc_foodlikeid"));
            
            //MessageBox.Show(fetchXml);

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
            buildGrid("dc_foodlike", dataGrid1, busyIndicator, e.Result, 1);
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
                        if (!xe.Name.LocalName.Equals("dc_foods.dc_recipefood"))
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
                            if (xe.Name.LocalName.Equals("dc_day", StringComparison.OrdinalIgnoreCase))
                            {
                                dg.Width = new DataGridLength(85);
                            }
                            else if (xe.Name.LocalName.Equals("dc_meal", StringComparison.OrdinalIgnoreCase))
                            {
                                dg.Width = new DataGridLength(105);
                            }
                            else if (xe.Name.LocalName.Equals("dc_foodid", StringComparison.OrdinalIgnoreCase))
                            {
                                dg.Width = new DataGridLength(284);
                            }
                            else
                            {
                                dg.Width = new DataGridLength(100);
                            }
                        }
                        else if (xe.Name.LocalName.Equals("dc_foods.dc_recipefood") )
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
                            CellETemp2.Append("<Button BorderThickness='0' ToolTipService.ToolTip='View Recipe' Cursor='Hand' Width='21' Height='21' Visibility='{Binding Path=Data, Converter={StaticResource rowConvertor}, ConverterParameter=visibility, Mode=OneWay}'>");
                            CellETemp2.Append("<Image  HorizontalAlignment='Center'  Source='/DynamicConnections.NutriStyle.MenuGenerator;component/images/recipe-icon.png'/>");
                            CellETemp2.Append("</Button>");
                            CellETemp2.Append("</DataTemplate>");
                            dgRecipe.CellEditingTemplate = (DataTemplate)XamlReader.Load(CellETemp2.ToString());

                            dgRecipe.IsReadOnly = false;
                            dgRecipe.Width = new DataGridLength(30);
                            
                            dgRecipe.Visibility = System.Windows.Visibility.Visible;
                            dataGrid1.Columns.Add(dgRecipe);
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
                    CellETemp.Append("<Button Cursor='Hand' ToolTipService.ToolTip='Remove From Food Like' Width='26' Height='26' HorizontalAlignment='Left'>");
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

                            if (xe.Name.LocalName.Equals("dc_foods.dc_recipefood"))
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
                    double space_available = (LayoutRoot.ActualWidth - 25 - 60); //18 is width of scroll bar, 150 is width of menu 
                    //figure out column types
                    /*
                    int count = 2;
                    if (space_available > 0)
                    {
                        foreach (DataGridColumn dg_c in dataGrid1.Columns)
                        {
                            if (dg_c.Width.Value != 50)
                            {
                                dg_c.Width = new DataGridLength((space_available / (dataGrid1.Columns.Count - count)));
                            }
                        }
                    }*/
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
            }
            bi.IsBusy = false;

        }

        /// <summary>
        /// Save to database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddFavorite_Click(object sender, RoutedEventArgs e)
        {
            App ap = (App)App.Current;
            //Get day

            //Get meal

            XElement foodLikes = new XElement("entities");
            foreach (String day in selectedDays)
            {

                foreach (String meal in selectedMeals)
                {
                    busyIndicator.IsBusy = true;
                    XElement contactXml = new XElement("dc_foodlike");

                    XElement contact = new XElement("dc_contactid", ap.contactId);
                    contactXml.Add(contact);

                    XAttribute at = new XAttribute("entityname", "contact");
                    contact.Add(at);

                    contactXml.Add(new XElement("dc_day", days[day]));
                    contactXml.Add(new XElement("dc_meal", meals[meal]));

                    XElement foodId = new XElement("dc_foodid", ((PairWithList)food.SelectedPair).Id);
                    XAttribute attribute = new XAttribute("entityname", "dc_foods");
                    foodId.Add(attribute);
                    contactXml.Add(foodId);

                    foodLikes.Add(contactXml);

                }

            }

            CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
            cms.CreateUpdateReturnEntitiesCompleted += new EventHandler<CrmSdk.CreateUpdateReturnEntitiesCompletedEventArgs>(cms_CreateUpdateFoodLike);
            cms.CreateUpdateReturnEntitiesAsync(foodLikes, true);

        }
        private void cms_CreateUpdateFoodLike(object sender, CrmSdk.CreateUpdateReturnEntitiesCompletedEventArgs e)
        {
            var errors = from x in e.Result.Descendants("error") select x;
            if (errors.Count() > 0)
            {
                busyIndicator.IsBusy = false;
                String message = e.Result.Value.ToString();
                Status s = new Status(message, false);
                s.Show();
            }
            else
            {
                var nodes = e.Result.Descendants("resultset");

                SortableCollectionView data = (SortableCollectionView)dataGrid1.ItemsSource;
                
                foreach (var node in nodes)
                {
                    var foodLike        = node.Descendants("dc_foodlike");
                    Pair dayPair        = new Pair();
                    dayPair.Id          = foodLike.Descendants("dc_day").Attributes("Id").First().Value;
                    dayPair.Name        = foodLike.Descendants("dc_day").First().Value;//days.FindKeyByValue(Convert.ToInt32(dayPair.Id));

                    Pair mealPair       = new Pair();
                    mealPair.Id         = foodLike.Descendants("dc_meal").Attributes("Id").First().Value;//e.Result.Descendants("dc_day").First().Value;
                    mealPair.Name       = foodLike.Descendants("dc_meal").First().Value;//meals.FindKeyByValue(Convert.ToInt32(mealPair.Id));
                    
                    Row r               = new Row();

                    r["dc_day"]         = dayPair;
                    r["dc_meal"]        = mealPair;
                    r["dc_foodid"]      = food.SelectedPair;
                    r["dc_foodlikeid"]  = e.Result.Descendants("dc_foodlikeid").First().Value;//results.Attribute("Id").Value;

                    //look at selected pair to see if recipe
                    if (food.IsRecipe)
                    {
                        r["visibility"] = Visibility.Visible.ToString();
                    }
                    else
                    {
                        r["visibility"] = Visibility.Collapsed.ToString();
                    }
                    
                    data.Add(r);
                }
                busyIndicator.IsBusy = false;
                ResetForm();
            }
        }
        private void ResetForm()
        {
            selectedDays = new List<string>();
            selectedMeals = new List<string>();
            var allImages = this.Descendents().OfType<Image>();
            foreach (var image in allImages)
            {
                BitmapImage imageSource = (BitmapImage)image.Source;

                String source = imageSource.UriSource.OriginalString;
                if (source.Contains("_on"))
                {
                    source = source.Replace("_on", "_off");
                    image.Source = new BitmapImage(new Uri(source, UriKind.Relative));
                }
            }
        }
        private void food_KeyDown(object sender, KeyEventArgs e)
        {
            String text = food.Text;

            if (String.IsNullOrEmpty(searchText) || searchText.Length >= text.Length)
            {
                if (text.Length > 1 && (e.Key != Key.Down && e.Key != Key.Up && e.Key != Key.Delete && e.Key != Key.Back))
                {
                    if (String.IsNullOrEmpty(searchText) || (text.Length == searchText.Length))
                    {
                        searchText = text;
                    }

                    food.IsBusy = true;

                    try
                    {
                        CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
                        cms.RetrieveFoodLikesCompleted += new EventHandler<CrmSdk.RetrieveFoodLikesCompletedEventArgs>(cms_RetrieveFoods);
                        cms.RetrieveFoodLikesAsync(General.ContactId(), text);
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show(err.Message);
                        MessageBox.Show(err.StackTrace);
                    }
                }
            }
        }
        private void cms_RetrieveFoods(object sender, CrmSdk.RetrieveFoodLikesCompletedEventArgs e)
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

                        String name = String.Empty;
                        String Id = String.Empty;
                        bool isRecipe = false;
                        foreach (XElement xe in row.Elements())
                        {
                            if (xe.Name.LocalName.Equals("dc_name", StringComparison.OrdinalIgnoreCase))
                            {
                                name = xe.Value;
                            }
                            else if (xe.Name.LocalName.Equals("dc_foodsid", StringComparison.OrdinalIgnoreCase))
                            {
                                Id = xe.Value;
                            }
                            else if (xe.Name.LocalName.Equals("dc_recipefood", StringComparison.OrdinalIgnoreCase))
                            {
                                if (Convert.ToBoolean(xe.Value))
                                {
                                    isRecipe = true;
                                }
                            }
                        }
                        data.Add(new PairWithList(name, Id));
                        if (!recipeList.Contains(new KeyValuePair<Guid, bool>(new Guid(Id), isRecipe)))
                        {
                            recipeList.Add(new KeyValuePair<Guid, bool>(new Guid(Id), isRecipe));
                        }
                    }
                    food.SelectedPair = new PairWithList(String.Empty, String.Empty, String.Empty, data);
                    
                    food.OpenDropdown(true);
                    food.IsBusy = false;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("FoodDislikes", UriKind.Relative));
        }

        private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }

        private void meal_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            var allImages = b.Descendents().OfType<Image>();


            BitmapImage imageSource = (BitmapImage)allImages.First().Source;

            String source = imageSource.UriSource.OriginalString;


            if (source.Contains("off"))
            {
                //turn button on
                source = source.Replace("_off", "_on");
                selectedMeals.Add(b.Name);
            }
            else
            {
                source = source.Replace("_on", "_off");
                selectedMeals.Remove(b.Name);
            }
            allImages.First().Source = new BitmapImage(new Uri(source, UriKind.Relative));
        }
        private void day_Click(object sender, RoutedEventArgs e)
        {

            Button b = (Button)sender;
            var allImages = b.Descendents().OfType<Image>();

            BitmapImage imageSource = (BitmapImage)allImages.First().Source;

            String source = imageSource.UriSource.OriginalString;


            if (source.Contains("off"))
            {
                //turn button on
                source = source.Replace("_off", "_on");
                selectedDays.Add(b.Name);
            }
            else
            {
                source = source.Replace("_on", "_off");
                selectedDays.Add(b.Name);
            }
            allImages.First().Source = new BitmapImage(new Uri(source, UriKind.Relative));
        }
        private void Label_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ChildWindows.GenericPopup gp = new GenericPopup("Determining Calorie Targets");
            gp.Show();
        }
        private void GenerateMenu_Click(object sender, RoutedEventArgs e)
        {
            busyIndicator.IsBusy = true;
            App app = (App)App.Current;
            Menu m = new Menu();
            m.Generate(new Guid(app.contactId), postMenuGenerate);

        }
        private void postMenuGenerate()
        {
            busyIndicator.IsBusy = false;
            NavigationService.Navigate(new Uri("DailyMenu", UriKind.Relative));
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            food.IsBusy = false;
            food.Text   = String.Empty;
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            food.Text = String.Empty;
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            var allImages = dayGrid.Descendents().OfType<Image>();
            if (cb.IsChecked.Value)
            {
                //checked all
                foreach (var image in allImages)
                {
                    BitmapImage imageSource = (BitmapImage)image.Source;

                    String source = imageSource.UriSource.OriginalString;
                    if (source.Contains("_off"))
                    {
                        source = source.Replace("_off", "_on");
                        image.Source = new BitmapImage(new Uri(source, UriKind.Relative));
                    }
                }
                selectedDays = new List<string>();
                selectedDays.Add("sunday");
                selectedDays.Add("monday");
                selectedDays.Add("tuesday");
                selectedDays.Add("wednesday");
                selectedDays.Add("thursday");
                selectedDays.Add("friday");
                selectedDays.Add("saturday");
            }
            else
            {
                foreach (var image in allImages)
                {
                    BitmapImage imageSource = (BitmapImage)image.Source;

                    String source = imageSource.UriSource.OriginalString;
                    if (source.Contains("_on"))
                    {
                        source = source.Replace("_on", "_off");
                        image.Source = new BitmapImage(new Uri(source, UriKind.Relative));
                    }
                }
                selectedDays = new List<string>();
            }
        }

    }
}
