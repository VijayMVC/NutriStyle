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
using System.Windows.Printing;
using System.Windows.Media.Imaging;

namespace DynamicConnections.NutriStyle.MenuGenerator.Pages
{
    public partial class ShoppingList : Page
    {
        
        SortableCollectionView dataFoods;

        private double lineHeight = 20;
        int rowIndex = 0;
        private double spacetoPrint { get; set; }
        private double CanvasTop { get; set; }

        private Canvas PrintBody { get; set; }


        private RowIndexConverter _rowIndexConverter = new RowIndexConverter();

        public ShoppingList()
        {
            dataFoods = new SortableCollectionView();

            InitializeComponent();
            //Setup birthday values

            RetrieveShoppingLists();
            PopulateHelp hp = new PopulateHelp();
            dataGrid1.LoadingRow += new EventHandler<DataGridRowEventArgs>(dataGrid1_LoadingRow);
            RetrieveMenuName();
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
                            ToolTipService.SetToolTip(ccb, "Delete");

                            ccb.MouseLeftButtonUp -= ccb_MouseLeftButtonUp;
                            ccb.MouseLeftButtonUp += new MouseButtonEventHandler(ccb_MouseLeftButtonUp);
                        }
                        if (c.GetCellContent(e.Row).GetType() == typeof(Button))
                        {
                            Button ccb = c.GetCellContent(e.Row) as Button;
                            //ToolTip tt = ToolTipService.GetToolTip(ccb) as ToolTip;
                            ToolTipService.SetToolTip(ccb, "Edit");

                            ccb.Click -= edit_MouseLeftButtonUp;
                            ccb.Click += new RoutedEventHandler(edit_MouseLeftButtonUp);
                            //ccb.Click += new MouseButtonEventHandler(edit_MouseLeftButtonUp);
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
        private void RetrieveMenuName() {
            String fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical'>
              <entity name='dc_menu'>
                <attribute name='dc_name'/>
                <attribute name='dc_menuid'/>
                <order attribute='dc_name' descending='false' />
                <filter type='and'>
                    <condition attribute='dc_primarymenu' operator='eq' value='1' />
                    <condition attribute='dc_contactid' operator='eq' value='@CONTACTID' />
                </filter>
              </entity>
            </fetch>";

            fetchXml = fetchXml.Replace("@CONTACTID", ((App)App.Current).contact.ContactId.ToString());

            XElement orderXml = new XElement("ColumnOrder");
            orderXml.Add(new XElement("Column", "dc_name"));
            orderXml.Add(new XElement("Column", "dc_menuid"));

            try
            {
                busyIndicator.IsBusy = true;
                busyIndicator.Visibility = System.Windows.Visibility.Visible;

                CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
                cms.RetrieveFetchXmlCompleted += new EventHandler<CrmSdk.RetrieveFetchXmlCompletedEventArgs>(cms_RetrievePrimaryMenu);
                cms.RetrieveFetchXmlAsync(fetchXml, orderXml.ToString());
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
            }
        }
        private void cms_RetrievePrimaryMenu(object sender, CrmSdk.RetrieveFetchXmlCompletedEventArgs e)
        {

            XElement element = e.Result;
            String entityName = "dc_menu";

            dataFoods = new SortableCollectionView();
            try
            {
                if (element.Descendants(entityName).Count() > 0)
                {
                    //Bind data
                    var rows = element.Descendants(entityName);
                    var row = rows.First();

                    foreach (XElement xe in row.Elements())
                    {
                        if (xe.Name.LocalName.Equals("dc_name", StringComparison.OrdinalIgnoreCase))
                        {
                            menuName.Content = xe.Value;
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
        /// <summary>
        /// Remove shopping list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ccb_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            App ap = (App)App.Current;
            //get the foodId
            if (dataGrid1.SelectedItem != null)
            {
                SortableCollectionView data = (SortableCollectionView)dataGrid1.ItemsSource;

                Image img = sender as Image;
                String Id = (String)((Row)dataGrid1.SelectedItem)["dc_shoppinglistid"];
                Row r = (Row)dataGrid1.SelectedItem;
                if (!String.IsNullOrEmpty(Id))
                {
                    XElement deleteXml = new XElement("dc_shoppinglist", new XElement("id", Id));

                    CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
                    cms.DeleteCompleted += new EventHandler<CrmSdk.DeleteCompletedEventArgs>(cms_Delete);
                    cms.DeleteAsync(deleteXml);
                }
                data.Remove(r);
                dataGrid1.ItemsSource = data;
            }
        }
        private void cms_Delete(object sender, CrmSdk.DeleteCompletedEventArgs e)
        {
            //CrmIndicator.IsBusy = false;
            //place holder
        }

        void edit_MouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
            App ap = (App)App.Current;
            //get the shoppinglistid

            SortableCollectionView data = (SortableCollectionView)dataGrid1.ItemsSource;
            if (dataGrid1.SelectedItem != null)
            {
                String Id = (String)((Row)dataGrid1.SelectedItem)["dc_shoppinglistid"];
                //need to open child window to edit shopping list
                ChildWindows.EditShoppingList esl = new ChildWindows.EditShoppingList(new Guid(Id));
                esl.Show();
            }
        }
        private void cms_RemoveFromShoppingList(object sender, CrmSdk.RemoveFromShippingListCompletedEventArgs e)
        {
            Row r = (Row)dataGrid1.SelectedItem;
            dataFoods.Remove(r);
            dataGrid1.ItemsSource = dataFoods;
        }
        
        private void RetrieveShoppingLists()
        {
            App ap = (App)App.Current;

            String fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical'>
              <entity name='dc_shoppinglist'>
                <attribute name='dc_name'/>
                <attribute name='dc_shoppinglistid'/>
                <order attribute='dc_name' descending='false' />
                <filter type='and'>
                    <condition attribute='dc_contactid' operator='eq' value='@CONTACTID' />
                </filter>
              </entity>
            </fetch>";

            fetchXml = fetchXml.Replace("@CONTACTID", ap.contactId);

            XElement orderXml = new XElement("ColumnOrder");
            orderXml.Add(new XElement("Column", "dc_name"));
            orderXml.Add(new XElement("Column", "dc_shoppinglistid"));
            
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

            buildGrid("dc_shoppinglist", dataGrid1, busyIndicator, e.Result, 1);
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

                    //Add button for edit
                    DataGridTemplateColumn dgButton = new DataGridTemplateColumn();
                    dgButton.Header = String.Empty;
                    //dgImage.SortMemberPath = xe.Name.LocalName;
                    dgButton.CanUserSort = false;

                    StringBuilder CellETemp = new StringBuilder();
                    CellETemp.Append("<DataTemplate ");
                    CellETemp.Append("xmlns='http://schemas.microsoft.com/winfx/");
                    CellETemp.Append("2006/xaml/presentation' ");
                    CellETemp.Append("xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' >");
                    CellETemp.Append("<Button Width='30' Height='22' Content='Edit'/>");// MouseLeftButtonDown='ImageDelete_MouseLeftButtonDown'
                    CellETemp.Append("</DataTemplate>");
                    dgButton.CellEditingTemplate = (DataTemplate)XamlReader.Load(CellETemp.ToString());

                    dgButton.IsReadOnly = false;
                    dgButton.Width = new DataGridLength(50);
                    //visable?
                    dgButton.Visibility = System.Windows.Visibility.Visible;
                    dataGrid1.Columns.Add(dgButton);

                    //add image for delete
                    DataGridTemplateColumn dgImage = new DataGridTemplateColumn();
                    dgImage.Header = "Del";
                    dgImage.CanUserSort = false;

                    CellETemp = new StringBuilder();
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

                        }
                        dataFoods.Add(rowData);
                    }
                    dataGrid.ItemsSource = dataFoods;
                    dataGrid.BorderThickness = new Thickness(1);

                    // Space available to fill ( -18 Standard vScrollbar)
                    double space_available = (LayoutRoot.ActualWidth - 18 - 100); //18 is width of scroll bar, 150 is width of menu 
                    //figure out column types

                    int count = 3;
                    if (space_available > 0)
                    {
                        foreach (DataGridColumn dg_c in dataGrid1.Columns)
                        {
                            if (dg_c.Width.Value != 50)
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
                var tb = new TextBlock { Text = ((Pair)dataFoods[rowIndex]["alias_dc_foodid"]).Name };
                var tb2 = new TextBlock { Text = (String)dataFoods[rowIndex]["alias_dc_portionsize"] + " " + ((Pair)dataFoods[rowIndex]["alias_dc_portiontypeid"]).Name };
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            App ap = (App)App.Current;
            try
            {
                busyIndicator.IsBusy = true;
                busyIndicator.Visibility = System.Windows.Visibility.Visible;

                CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
                cms.ResetShoppingListCompleted += new EventHandler<CrmSdk.ResetShoppingListCompletedEventArgs>(cms_ResetShoppingList);
                cms.ResetShoppingListAsync(ap.contactId);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
            }
        }
        private void cms_ResetShoppingList(object sender, CrmSdk.ResetShoppingListCompletedEventArgs e)
        {
            dataGrid1.ItemsSource = null;
            dataGrid1.Columns.Clear();
            dataFoods.Clear();
            RetrieveShoppingLists();
        }
    }
}
