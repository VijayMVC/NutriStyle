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
    public partial class Menus : Page
    {
       
        SortableCollectionView dataFoods;

       
        private double spacetoPrint { get; set; }
        private double CanvasTop { get; set; }

        private Canvas PrintBody { get; set; }


        private RowIndexConverter _rowIndexConverter = new RowIndexConverter();

        public Menus()
        {
            dataFoods = new SortableCollectionView();

            InitializeComponent();
            //Setup birthday values

            RetrieveMenus();
            RetrieveMenusPrimary();
            PopulateHelp hp = new PopulateHelp();
            dataGrid1.LoadingRow += new EventHandler<DataGridRowEventArgs>(dataGrid1_LoadingRow);
            dataGrid2.LoadingRow += new EventHandler<DataGridRowEventArgs>(dataGrid1_LoadingRow);
            name.Content = ((App)App.Current).contact.Email;

        }
        /// <summary>
        /// Fired when new row is added to the grid.  Assigns button and delete event handlers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                        if (c.GetCellContent(e.Row).GetType() == typeof(Button))
                        {
                            Button ccb = c.GetCellContent(e.Row) as Button;
                            //ToolTip tt = ToolTipService.GetToolTip(ccb) as ToolTip;
                            //ToolTipService.SetToolTip(ccb, "Delete");

                            ccb.Click -= ccb_MouseLeftButtonUp;
                            ccb.Click += new RoutedEventHandler(ccb_MouseLeftButtonUp);//new MouseButtonEventHandler(ccb_MouseLeftButtonUp);
                        }
                        if (c.GetCellContent(e.Row).GetType() == typeof(Button))
                        {

                            Button ccb = c.GetCellContent(e.Row) as Button;
                            if (ccb.Name.Equals("EditMenu", StringComparison.OrdinalIgnoreCase))
                            {
                                //ToolTip tt = ToolTipService.GetToolTip(ccb) as ToolTip;
                                ToolTipService.SetToolTip(ccb, "Edit Menu");

                                ccb.Click -= edit_MouseLeftButtonUp;
                                ccb.Click += new RoutedEventHandler(edit_MouseLeftButtonUp);
                                //ccb.Click += new MouseButtonEventHandler(edit_MouseLeftButtonUp);
                            }
                            else if (ccb.Name.Equals("CreateShoppingList", StringComparison.OrdinalIgnoreCase))
                            {
                                //ToolTip tt = ToolTipService.GetToolTip(ccb) as ToolTip;
                                ToolTipService.SetToolTip(ccb, "Create Shopping List");
                                ccb.Click -= GenerateShoppingList;
                                ccb.Click += new RoutedEventHandler(GenerateShoppingList);

                            }
                            else if (ccb.Name.Equals("CreateFoodLog", StringComparison.OrdinalIgnoreCase))
                            {
                                //ToolTip tt = ToolTipService.GetToolTip(ccb) as ToolTip;
                                ToolTipService.SetToolTip(ccb, "Create FoodLog");
                                ccb.Click -= CreateFoodLog;
                                ccb.Click += new RoutedEventHandler(CreateFoodLog);

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

        void CreateFoodLog(object sender, RoutedEventArgs e)
        {
            Guid menuId = Guid.Empty;
            App app = (App)App.Current;

            Row r = (Row)((Button)sender).DataContext;
            menuId = new Guid((String)r["dc_menuid"]);

            CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
            cms.CreateFoodLogFromMenuCompleted += new EventHandler<CrmSdk.CreateFoodLogFromMenuCompletedEventArgs>(cms_CreateFoodLog);
            cms.CreateFoodLogFromMenuAsync(menuId.ToString(), app.contactId);
            busyIndicator.IsBusy = true;
            busyIndicator2.IsBusy = true;
        }
        private void cms_CreateFoodLog(object sender, CrmSdk.CreateFoodLogFromMenuCompletedEventArgs e)
        {
            busyIndicator.IsBusy = false;
            busyIndicator2.IsBusy = false;
        }
        /// <summary>
        /// Generate Shopping list from Menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void GenerateShoppingList(object sender, RoutedEventArgs e)
        {
            Row r = (Row)dataGrid1.SelectedItem;
            String Id = (String)r["dc_menuid"];

            CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
            cms.GenerateShoppingListFromMenuCompleted += new EventHandler<CrmSdk.GenerateShoppingListFromMenuCompletedEventArgs>(cms_GenerateShoppingList);
            cms.GenerateShoppingListFromMenuAsync(Id);
            busyIndicator.IsBusy = true;
            busyIndicator2.IsBusy = true;
        }
        private void cms_GenerateShoppingList(object sender, CrmSdk.GenerateShoppingListFromMenuCompletedEventArgs e)
        {
            busyIndicator.IsBusy = false;
            busyIndicator2.IsBusy = false;
        }
        /// <summary>
        /// Remove menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ccb_MouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
            App ap = (App)App.Current;
            //get the foodId
            if (dataGrid1.SelectedItem != null)
            {
                SortableCollectionView data = (SortableCollectionView)dataGrid1.ItemsSource;

                Button img = sender as Button;
                String Id = (String)((Row)dataGrid1.SelectedItem)["dc_menuid"];
                Row r = (Row)dataGrid1.SelectedItem;
                if (!String.IsNullOrEmpty(Id))
                {
                    XElement deleteXml = new XElement("dc_menu", new XElement("id", Id));

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
        //Open menu editor
        void edit_MouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
            App ap = (App)App.Current;
            //get the shoppinglistid

            SortableCollectionView data = (SortableCollectionView)dataGrid1.ItemsSource;
            if (dataGrid1.SelectedItem != null)
            {
                String Id = (String)((Row)dataGrid1.SelectedItem)["dc_menuid"];
                //need to open child window to edit shopping list
                ChildWindows.MenuEditor esl = new ChildWindows.MenuEditor(new Guid(Id));
                esl.Show();
            }
        }

        private void RetrieveMenus()
        {
            dataGrid1.ItemsSource = new SortableCollectionView();
            dataGrid1.Columns.Clear();

            App ap = (App)App.Current;

            String fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical'>
              <entity name='dc_menu'>
                <attribute name='dc_name'/>
                <attribute name='dc_menuid'/>
                <order attribute='dc_name' descending='false' />
                <filter type='and'>
                    <condition attribute='dc_primarymenu' operator='eq' value='0' />
                    <condition attribute='dc_contactid' operator='eq' value='@CONTACTID' />
                </filter>
              </entity>
            </fetch>";

            fetchXml = fetchXml.Replace("@CONTACTID", ap.contactId);

            XElement orderXml = new XElement("ColumnOrder");
            orderXml.Add(new XElement("Column", "dc_name"));
            orderXml.Add(new XElement("Column", "dc_menuid"));

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

            buildGrid("dc_menu", dataGrid1, busyIndicator, e.Result, 1);
        }
        private void RetrieveMenusPrimary()
        {
            dataGrid2.ItemsSource = new SortableCollectionView();
            dataGrid2.Columns.Clear();

            App ap = (App)App.Current;

            String fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical'>
              <entity name='dc_menu'>
                <attribute name='dc_name'/>
               
                <order attribute='dc_name' descending='false' />
                <filter type='and'>
                    <condition attribute='dc_primarymenu' operator='eq' value='1' />
                    <condition attribute='dc_contactid' operator='eq' value='@CONTACTID' />
                </filter>
              </entity>
            </fetch>";

            fetchXml = fetchXml.Replace("@CONTACTID", ap.contactId);

            XElement orderXml = new XElement("ColumnOrder");
            orderXml.Add(new XElement("Column", "dc_name"));


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

            buildGrid("dc_menu", dataGrid2, busyIndicator2, e.Result, 1);
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

                    if (element.Descendants(entityName).Count() > 1)
                    {
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
                        CellETemp.Append("<Button Cursor='Hand' ToolTipService.ToolTip='Remove Saved Menu' Width='26' Height='26' HorizontalAlignment='Left'>");
                        CellETemp.Append("<Image  Stretch='Fill' Name='Delete' Source='/DynamicConnections.NutriStyle.MenuGenerator;component/images/delete.png'/>");// MouseLeftButtonDown='ImageDelete_MouseLeftButtonDown'
                        CellETemp.Append("</Button>");
                        CellETemp.Append("</DataTemplate>");
                        dgImage.CellEditingTemplate = (DataTemplate)XamlReader.Load(CellETemp.ToString());

                        dgImage.IsReadOnly = false;
                        dgImage.Width = new DataGridLength(32);
                        //visable?
                        dgImage.Visibility = System.Windows.Visibility.Visible;

                        dataGrid.Columns.Add(dgImage);
                    }
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
                    //18 is width of scroll bar, 150 is width of menu 
                    //figure out column types

                    if (element.Descendants(entityName).Count() > 1)
                    {
                        double space_available = (LayoutRoot.ActualWidth - 18 - 50);

                        if (space_available > 0)
                        {
                            foreach (DataGridColumn dg_c in dataGrid.Columns)
                            {
                                if (dg_c.Width.Value != 32)
                                {
                                    dg_c.Width = new DataGridLength((space_available / (dataGrid.Columns.Count - 2)));
                                }
                            }
                        }
                    }
                    else
                    {
                        double space_available = (LayoutRoot.ActualWidth - 18);

                        foreach (DataGridColumn dg_c in dataGrid.Columns)
                        {
                            if (dg_c.Width.Value != 50)
                            {
                                dg_c.Width = new DataGridLength((space_available / (dataGrid.Columns.Count)));
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

        private void cms_ResetShoppingList(object sender, CrmSdk.ResetShoppingListCompletedEventArgs e)
        {
            dataGrid1.ItemsSource = null;
            dataGrid1.Columns.Clear();
            dataFoods.Clear();
            RetrieveMenus();
        }
        /// <summary>
        /// Generates a new menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GenerateMenu_Click(object sender, RoutedEventArgs e)
        {
            App app = (App)App.Current;
            busyIndicator.IsBusy = true;
            busyIndicator2.IsBusy = true;

            Menu m = new Menu();
            m.Generate(new Guid(app.contactId), postMenuGenerate);
        }
        private void postMenuGenerate()
        {
            busyIndicator.IsBusy = false;
            busyIndicator2.IsBusy = false;
            RetrieveMenus();//fetch menu
            RetrieveMenusPrimary();//fetch menu
        }

        private void MakePrimary_Click(object sender, RoutedEventArgs e)
        {
            //make sure a row is selected
            if (dataGrid1.SelectedItem == null)
            {
                Status s = new Status("Please select a menu first", false);
            }
            else
            {
                Row r = (Row)dataGrid1.SelectedItem;

                XElement eventXml = new XElement("dc_menu");
                eventXml.Add(new XElement("dc_menuid", (String)r["dc_menuid"]));
                eventXml.Add(new XElement("dc_primarymenu", "1"));

                busyIndicator.IsBusy = true;
                busyIndicator2.IsBusy = true;
                CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
                cms.SetPrimaryCompleted += new EventHandler<CrmSdk.SetPrimaryCompletedEventArgs>(cms_CreateUpdateGrid);
                cms.SetPrimaryAsync((String)r["dc_menuid"], ((App)App.Current).contactId);
            }
        }
        private void cms_CreateUpdateGrid(object sender, CrmSdk.SetPrimaryCompletedEventArgs e)
        {
            busyIndicator.IsBusy = false;
            busyIndicator2.IsBusy = false;

            var errors = from x in e.Result.Descendants("error") select x;
            if (errors.Count() > 0)
            {
                /*
                String message = e.Result.Value.ToString();
                Status s = new Status(message, false);
                s.Show();*/
            }
            else
            {
                var results = e.Result;
                Guid menuId = new Guid(e.Result.Value);
                ((App)App.Current).contact.MenuId = menuId;
            }

            RetrieveMenus();//fetch menu
            RetrieveMenusPrimary();//fetch menu





        }

        private void EditMenu_Click(object sender, RoutedEventArgs e)
        {
            Guid menuId = Guid.Empty;
            if (dataGrid1.SelectedItem == null)
            {
                Status s = new Status("Please select a menu to edit", false);
                s.Show();
            }
            else
            {
                Row r = (Row)dataGrid1.SelectedItem;
                menuId = new Guid((String)r["dc_menuid"]);
                EditMenu em = new EditMenu(menuId);
                em.SubmitClicked += new EventHandler(em_SubmitClicked);
                em.Show();
            }


        }

        void em_SubmitClicked(object sender, EventArgs e)
        {
            EditMenu em = sender as EditMenu;
            Row r = (Row)dataGrid1.SelectedItem;
            r["dc_name"] = em.Name;

        }
    }
}
