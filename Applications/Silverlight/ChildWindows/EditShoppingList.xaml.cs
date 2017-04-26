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

namespace DynamicConnections.NutriStyle.MenuGenerator.ChildWindows
{
    public partial class EditShoppingList : ChildWindow
    {

        SortableCollectionView dataFoods;
        private RowIndexConverter _rowIndexConverter = new RowIndexConverter();
        Guid shoppingListId = Guid.Empty;

        private double lineHeight = 20;
        int rowIndex = 0;
        private double spacetoPrint { get; set; }
        private double CanvasTop { get; set; }
        private Canvas PrintBody { get; set; }
        Row selectedRow;
        Controls.ComboBox comboBox;

        public EditShoppingList()
        {
            InitializeComponent();
        }
        public EditShoppingList(Guid shoppingListId)
        {
            InitializeComponent();
            this.shoppingListId = shoppingListId;
            dataGrid1.LoadingRow += new EventHandler<DataGridRowEventArgs>(dataGrid1_LoadingRow);
            RetrieveShoppingItems();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
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
                        //MessageBox.Show(""+c.GetCellContent(e.Row).GetType());
                        if (c.GetCellContent(e.Row).GetType() == typeof(Image))
                        {
                            Image ccb = c.GetCellContent(e.Row) as Image;
                            //ToolTip tt = ToolTipService.GetToolTip(ccb) as ToolTip;
                            ToolTipService.SetToolTip(ccb, "Remove From Shopping List");

                            ccb.MouseLeftButtonUp -= ccb_MouseLeftButtonUp;
                            ccb.MouseLeftButtonUp += new MouseButtonEventHandler(ccb_MouseLeftButtonUp);
                        }
                        else if (c.GetCellContent(e.Row).GetType() == typeof(Controls.ComboBox))
                        {
                            Controls.ComboBox ccb = c.GetCellContent(e.Row) as Controls.ComboBox;

                            if (ccb.TagName.Equals("dc_name", StringComparison.OrdinalIgnoreCase))
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

            if (e != null && e.Result != null)
            {
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
                    else //nothing found.
                    {
                        data.Add(new Pair(comboBox.MyAutoCompleteBox.Text, Guid.Empty.ToString()));
                        comboBox.MyAutoCompleteBox.ItemsSource = data;
                        comboBox.OpenDropdown(false);
                        //remove portion type
                        if (dataGrid1.SelectedItem != null)
                        {
                            Row r = (Row)dataGrid1.SelectedItem;
                            r.RowChanged = true;
                            r["dc_portiontypeid"] = new Pair(String.Empty, Guid.Empty.ToString());
                            r["dc_foodid"] = new Pair(comboBox.MyAutoCompleteBox.Text, Guid.Empty.ToString());
                            r["dc_name"] = new Pair(comboBox.MyAutoCompleteBox.Text, Guid.Empty.ToString());
                            //CreateUpdate(r);
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
            if (dataGrid1.SelectedItem != null)
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

                    Row r = (Row)dataGrid1.SelectedItem;
                    r.RowChanged = true;//mark as dirty
                    r["dc_portiontypeid"] = p;
                    r.RowChanged = true;
                    comboBox.KeyDown -= food_KeyDown;
                    comboBox.KeyDown += new KeyEventHandler(food_KeyDown);

                    //Save item back to database
                    //CreateUpdate(r);
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
            }
        }
        private void Save(object sender, RoutedEventArgs e)
        {
            //loop through data and find dirty rows
            SortableCollectionView list = (SortableCollectionView)dataGrid1.ItemsSource;
            busyIndicator.IsBusy = true;
            foreach (Row r in list)
            {
                if (r.RowChanged)
                {
                    CreateUpdate(r);
                }
            }
            busyIndicator.IsBusy = false;
        }
        private void CreateUpdate(Row r)
        {
            selectedRow = r;
            //make sure something is selected
            if ( !String.IsNullOrEmpty(((Pair)r["dc_name"]).Name))
            {
                XElement contactXml = new XElement("dc_shoppinglistitem");

                XElement contact = new XElement("dc_shoppinglistid", shoppingListId.ToString());
                contactXml.Add(contact);

                XAttribute at = new XAttribute("entityname", "dc_shoppinglist");
                contact.Add(at);

                if (!String.IsNullOrEmpty((String)r["dc_portionsize"]))
                {
                    contactXml.Add(new XElement("dc_portionsize", (String)r["dc_portionsize"]));
                }
                //if (((Pair)r["dc_foodid"]).Id.Equals(Guid.Empty.ToString())) {
                contactXml.Add(new XElement("dc_name", ((Pair)r["dc_name"]).Name));
                //}

                //if(!((Pair)r["dc_portiontypeid"]).Id.Equals(Guid.Empty.ToString())) {//make sure a real value is selected
                XElement portiontTypeId = new XElement("dc_portiontypeid", ((Pair)r["dc_portiontypeid"]).Id);
                XAttribute attribute = new XAttribute("entityname", "dc_portion_types");
                portiontTypeId.Add(attribute);
                contactXml.Add(portiontTypeId);
                //}
                //if (!((Pair)r["dc_foodid"]).Id.Equals(Guid.Empty.ToString())) {
                XElement foodId = new XElement("dc_foodid", ((Pair)r["dc_name"]).Id);
                XAttribute attribute2 = new XAttribute("entityname", "dc_foods");
                foodId.Add(attribute2);
                contactXml.Add(foodId);

                //contactXml.Add(new XElement("dc_name", ((Pair)r["dc_name"]).Name));
                //}

                //if (r["dc_shoppinglistitemid"] != null && !((String)r["dc_shoppinglistitemid"]).Equals(Guid.Empty.ToString())){
                contactXml.Add(new XElement("dc_shoppinglistitemid", ((String)r["dc_shoppinglistitemid"])));
                //}
                r.RowChanged = false;
                CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
                cms.CreateUpdateCompleted += new EventHandler<CrmSdk.CreateUpdateCompletedEventArgs>(cms_CreateUpdateFoodLike);
                cms.CreateUpdateAsync(contactXml);
            }
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

                var results = e.Result;

                //MessageBox.Show("Success");
                //NavigationService.Navigate(new Uri("MenuOptions", UriKind.Relative));
                String Id = e.Result.Value.ToString();
                SortableCollectionView data = (SortableCollectionView)dataGrid1.ItemsSource;
                selectedRow["dc_shoppinglistitem"] = Id;
                
            }
        }

        void ccb_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            App ap = (App)App.Current;
            //get the foodId
            if (dataGrid1.SelectedItem != null)
            {
                Row r = (Row)dataGrid1.SelectedItem;
                String foodId = (String)r["dc_shoppinglistitemid"];
                if (!String.IsNullOrEmpty(foodId) && new Guid(foodId) != Guid.Empty)
                {
                    //now remove food from menu shopping list
                    XElement deleteXml = new XElement("dc_shoppinglistitem", new XElement("id", foodId));

                    CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
                    cms.DeleteCompleted += new EventHandler<CrmSdk.DeleteCompletedEventArgs>(cms_Delete);
                    cms.DeleteAsync(deleteXml);
                }
                
                dataFoods.Remove(r);
                dataGrid1.ItemsSource = dataFoods;
            }
        }

        private void cms_Delete(object sender, CrmSdk.DeleteCompletedEventArgs e)
        {
            //CrmIndicator.IsBusy = false;
            //place holder
        }

        private void RetrieveShoppingItems()
        {
            App ap = (App)App.Current;

            String fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical'>
              <entity name='dc_shoppinglistitem'>
                <attribute name='dc_name' />
                <attribute name='dc_portionsize' />
                <attribute name='dc_portiontypeid'/>
                <attribute name='dc_shoppinglistitemid'/>
                <order attribute='dc_name' descending='false' />
                <filter type='and'>
                    <condition attribute='dc_shoppinglistid' operator='eq' value='@SHOPPINGLISTID' />
                </filter>
              </entity>
            </fetch>";

            fetchXml = fetchXml.Replace("@SHOPPINGLISTID", shoppingListId.ToString());

            XElement orderXml = new XElement("ColumnOrder");
            orderXml.Add(new XElement("Column", "dc_name"));
            orderXml.Add(new XElement("Column", "dc_portionsize"));
            orderXml.Add(new XElement("Column", "dc_portiontypeid"));
            orderXml.Add(new XElement("Column", "dc_shoppinglistitemid"));

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
            buildGrid("dc_shoppinglistitem", dataGrid1, busyIndicator, e.Result, 1);
        }

        private void buildGrid(String entityName, DataGrid dataGrid, BusyIndicator bi, XElement element, int columns)
        {
            dataFoods = new SortableCollectionView();
            try
            {
                if (element.Descendants(entityName).Count() > 0)
                {
                    XElement xEl = element.Descendants("columns").ToList()[0];//get first one

                    foreach (XElement xe in xEl.Elements())
                    {
                        dataFoods.ColumnTypes.Add(xe.Name.LocalName, xe.Attribute("Type").Value);

                        if ( (xe.Attribute("Type").Value.Equals("Lookup", StringComparison.OrdinalIgnoreCase) || xe.Name.LocalName.Equals("dc_name")) &&
                            !xe.Name.LocalName.Equals("dc_portiontypeid")  )
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
                                dg.Width = new DataGridLength(100);
                            }

                            dataGrid.Columns.Add(dg);
                        }
                        else if (xe.Name.LocalName.Equals("dc_portionsize", StringComparison.OrdinalIgnoreCase))
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
                        else
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
                                dg.Visibility = System.Windows.Visibility.Collapsed;
                            }
                            dg.IsReadOnly = false;
                            dataGrid.Columns.Add(dg);
                            dg.Width = new DataGridLength(100);
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
                    dataGrid1.Columns.Add(dgImage);


                    //Bind data
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

                        foreach (XElement xe in row.Elements())
                        {
                            if (xe.Name.LocalName.Equals("dc_name"))
                            {
                                rowData["dc_name"] = new Pair(xe.Value, Guid.Empty.ToString());
                            }
                            else if (dataFoods.ColumnTypes.ContainsKey(xe.Name.LocalName) && dataFoods.ColumnTypes[xe.Name.LocalName].Equals("money", StringComparison.OrdinalIgnoreCase))
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

                        }
                        dataFoods.Add(rowData);
                    }
                    dataGrid.ItemsSource = dataFoods;
                    dataGrid.BorderThickness = new Thickness(1);

                    // Space available to fill ( -18 Standard vScrollbar)
                    double space_available = (LayoutRoot.ActualWidth - 18 - 50 - 35 - 100 - 35-25); //18 is width of scroll bar, 150 is width of menu 
                    //figure out column types

                    int count = 4;
                    if (space_available > 0)
                    {
                        foreach (DataGridColumn dg_c in dataGrid1.Columns)
                        {
                            if (dg_c.Width.Value != 50 && dg_c.Width.Value != 35 && dg_c.Width.Value != 100)
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
        private void PrintDirections(object sender, RoutedEventArgs e)
        {
            rowIndex = 0;
            var doc = new PrintDocument();

            WriteableBitmap bitmap = new WriteableBitmap(dataGrid1, null);
            var bitmapImage = new BitmapImage();

            var imageBrush = new ImageBrush();
            imageBrush.ImageSource = bitmap;
            var rectangle = new System.Windows.Shapes.Rectangle
            {
                Width = (Application.Current.RootVisual as FrameworkElement).ActualWidth,
                Height = 450,
                Fill = imageBrush
            };
            doc.PrintPage += (s, e2) =>
            {
                PrintBody = new Canvas();
                //PrintBody.Children.Add(rectangle);
                e2.PageVisual = PrintBody;
                //CanvasTop = 650;
                double heightgap = 30;
                spacetoPrint = (e2.PrintableArea.Height - (heightgap * 2));
                CanvasTop = heightgap;
                //CanvasTop += heightgap;
                //CanvasTop = heightgap;
                IterateRow(e2);
            };

            doc.Print("Shopping List");
        }
        private void IterateRow(PrintPageEventArgs p)
        {

            if (rowIndex < dataFoods.Count)
            {
                var tb = new TextBlock { Text = ((Pair)dataFoods[rowIndex]["dc_name"]).Name };
                var tb2 = new TextBlock { Text = (String)dataFoods[rowIndex]["dc_portionsize"] + " " + ((Pair)dataFoods[rowIndex]["dc_portiontypeid"]).Name };
                if (lineHeight > spacetoPrint)
                {
                    p.HasMorePages = true;
                    return;
                }
                tb.SetValue(Canvas.TopProperty, CanvasTop);

                tb.SetValue(Canvas.LeftProperty, 30.00);
                PrintBody.Children.Add(tb);

                tb2.SetValue(Canvas.TopProperty, CanvasTop);
                tb2.SetValue(Canvas.LeftProperty, 650.00);
                PrintBody.Children.Add(tb2);

                CanvasTop += lineHeight;
                spacetoPrint -= lineHeight;
                rowIndex += 1;
                IterateRow(p);
            }
        }

        private void newItem_Click(object sender, RoutedEventArgs e)
        {
            Row r = new Row();
            dataFoods = (SortableCollectionView)dataGrid1.ItemsSource;
            //Populate columnTypes  --  rowData.ColumnTypes.Add(xe.Name.LocalName, xe.Attribute("Type").Value);
            foreach (KeyValuePair<String, String> type in dataFoods.ColumnTypes)
            {
                if (type.Key.Equals("dc_name"))
                {
                    r[type.Key] = new Pair(String.Empty, Guid.Empty.ToString());
                }
                else if (type.Value.Equals("Lookup", StringComparison.OrdinalIgnoreCase))
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
            try
            {
                dataFoods.Insert(0, r);//insert at top of list
                //dataFoods.Add(r);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
            }
        }
    }
}

