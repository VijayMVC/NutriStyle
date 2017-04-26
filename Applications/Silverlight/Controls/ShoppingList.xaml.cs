﻿using System;
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
using DynamicConnections.NutriStyle.MenuGenerator.Engine.Helpers;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Controls.DataVisualization;
using System.Windows.Controls.DataVisualization.Charting.Primitives;
using System.Xml.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows.Data;
using System.Windows.Printing;
using System.Windows.Media.Imaging;
using DynamicConnections.NutriStyle.MenuGenerator.ChildWindows;
using System.ComponentModel;
using System.Windows.Browser;

namespace DynamicConnections.NutriStyle.MenuGenerator.Controls
{
    public partial class ShoppingList : UserControl
    {
        SortableCollectionView dataFoods;
        private RowIndexConverter _rowIndexConverter = new RowIndexConverter();
        Guid shoppingListId = Guid.Empty;

        private double lineHeight = 20;
        int rowIndex = 0;
        private double spacetoPrint { get; set; }
        private double CanvasTop { get; set; }
        private Canvas PrintBody { get; set; }
        
        Controls.ComboBoxWithValidation comboBox;

        SyncRun SR;
        CrmSdk.WebServicesSoapClient cms;

        public Guid ContactId {get;set;}
        

        public ShoppingList()
        {
            SR = new SyncRun();
            InitializeComponent();
            dataGrid1.LoadingRow += new EventHandler<DataGridRowEventArgs>(dataGrid1_LoadingRow);
            cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
            RetrieveMenuName();
        }


        public ShoppingList(Guid contactId, Guid shoppingListId)
        {
            SR = new SyncRun();
            InitializeComponent();
            dataGrid1.LoadingRow += new EventHandler<DataGridRowEventArgs>(dataGrid1_LoadingRow);
            cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
            this.ContactId = contactId;
            this.shoppingListId = shoppingListId;
            RetrieveMenuName(contactId);
        }

        public ShoppingList(Guid shoppingListId)
        {
            SR = new SyncRun();
            InitializeComponent();
            this.shoppingListId = shoppingListId;
            dataGrid1.LoadingRow += new EventHandler<DataGridRowEventArgs>(dataGrid1_LoadingRow);
            cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
            RetrieveMenuName();
        }

        void dataGrid1_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (sender != null)
            {
                DataGrid dg = (DataGrid)sender;
                //deal with getting the data bound to the autocompletebox
                Row r = (Row)e.Row.DataContext;
                try
                {
                    foreach (DataGridColumn c in dg.Columns)
                    {
                        //MessageBox.Show(""+c.GetCellContent(e.Row).GetType());
                        if (c.GetCellContent(e.Row).GetType() == typeof(Button))
                        {
                            if (r["controlenabled"] != null && !(bool)r["controlenabled"])
                            {
                               
                            }
                            else
                            {
                                Button ccb = c.GetCellContent(e.Row) as Button;
                                
                                ccb.Click -= edit_MouseLeftButtonUp;
                                ccb.Click += new RoutedEventHandler(edit_MouseLeftButtonUp);
                            }
                        }
                        else if (c.GetCellContent(e.Row).GetType() == typeof(Controls.ComboBoxWithValidation))
                        {
                            Controls.ComboBoxWithValidation ccb = c.GetCellContent(e.Row) as Controls.ComboBoxWithValidation;

                            if (ccb.TagName.Equals("dc_name", StringComparison.OrdinalIgnoreCase))
                            {
                                ccb.KeyUp -= food_KeyDown;
                                ccb.KeyUp += new KeyEventHandler(food_KeyDown);
                            }
                            else if (ccb.TagName.Equals("dc_portiontypeid", StringComparison.OrdinalIgnoreCase))
                            {
                                ccb.IsEnabled = false;
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
        /// For default user
        /// </summary>
        private void RetrieveMenuName()
        {
            RetrieveMenuName(new Guid(General.ContactId()));
        }

        private void RetrieveMenuName(Guid contactId)
        {
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

            fetchXml = fetchXml.Replace("@CONTACTID", contactId.ToString());

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
                RetrieveShoppingItems(ContactId);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
            }

        }
        private void food_KeyDown(object sender, KeyEventArgs e)
        {
            comboBox = (Controls.ComboBoxWithValidation)sender;
            String text = comboBox.Text;

            Row r = (Row)comboBox.DataContext;
            //r.RowChanged = true;
            r["dc_portiontypeid"] = new PairWithList(String.Empty, Guid.Empty.ToString());
            r["dc_foodid"] = new PairWithList(text, Guid.Empty.ToString());
            //r["dc_name"] = new Pair(text, Guid.Empty.ToString());

            if (String.IsNullOrEmpty(comboBox.SearchText) || comboBox.SearchText.Length >= text.Length)
            {
                if (text.Length > 1 && (e.Key != Key.Down && e.Key != Key.Up && e.Key != Key.Delete && e.Key != Key.Back))
                {
                    //The text has to be different than what has already been searched on.  'cof' should not trigger a new search if the org search was 'co'
                    if (String.IsNullOrEmpty(comboBox.SearchText) || (text.Length != comboBox.SearchText.Length) || !text.Equals(comboBox.SearchText, StringComparison.OrdinalIgnoreCase))
                    {
                        comboBox.SearchText = text;

                        comboBox.IsBusy = true;
                        comboBox.MyAutoCompleteBox.SelectionChanged -= MyAutoCompleteBox_SelectionChanged;
                        comboBox.MyAutoCompleteBox.SelectionChanged += new SelectionChangedEventHandler(MyAutoCompleteBox_SelectionChanged);
                        comboBox.IsBusy = true;

                        XElement orderXml = null;

                        String fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
                          <entity name='dc_foods'>
                            <attribute name='dc_name' />
                            <attribute name='dc_foodsid' />
                            <order attribute='dc_name' descending='false' />
                            <filter type='and'>
                                <condition attribute='dc_name' value='%@TEXT%' operator='like'/>
                                <condition attribute='dc_food_id' operator='null'/>
                                <condition attribute='dc_reviewed' operator='eq' value='1'/>
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
            }
        }
        private void cms_RetrieveFoods(object sender, CrmSdk.RetrieveFetchXmlCompletedEventArgs e)
        {
            String entityName = "dc_foods";

            if (e != null && e.Result != null)
            {
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
                        comboBox.SelectedPair = new PairWithList(String.Empty, String.Empty, String.Empty, data);
                        comboBox.OpenDropdown(true);
                        comboBox.IsBusy = false;
                    }
                    else //nothing found.
                    {
                        data.Add(new PairWithList(comboBox.Text, Guid.Empty.ToString()));
                        comboBox.SelectedPair = new PairWithList(String.Empty, String.Empty, String.Empty, data);
                        comboBox.OpenDropdown(false);
                        comboBox.IsBusy = false;
                        //unhook the portion size handler
                        //comboBox.MyAutoCompleteBox.SelectionChanged -= MyAutoCompleteBox_SelectionChanged;

                        //remove portion type
                        if (dataGrid1.SelectedItem != null)
                        {
                            Row r = (Row)dataGrid1.SelectedItem;
                            r.RowChanged = true;
                            r["dc_portiontypeid"] = new PairWithList(String.Empty, Guid.Empty.ToString());
                            r["dc_foodid"] = new PairWithList(comboBox.Text, Guid.Empty.ToString());
                            r["dc_name"] = new PairWithList(comboBox.Text, Guid.Empty.ToString());
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
                //Row r = (Row)cb.DataContext;

                if (cb.SelectedItem != null && ((PairWithList)cb.SelectedItem) != null && ((PairWithList)cb.SelectedItem).Id != null && new Guid(((PairWithList)cb.SelectedItem).Id) != Guid.Empty)
                {
                    //Retreive the portion size
                    String fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                      <entity name='dc_foods'>
                        <attribute name='dc_portiontypeid' />
                        <order attribute='dc_foodsid' descending='false' />
                        <link-entity name='dc_portion_types' alias='dc_portion_types' to='dc_portiontypeid' from='dc_portion_typesid'>
                          <attribute name='dc_name' />
                          <attribute name='dc_abbreviation' />
                        </link-entity>
                              <filter type='and'>
                                <condition attribute='dc_foodsid' operator='eq' value='@FOODID' />
                              </filter>
                      </entity>
                    </fetch>";

                    fetchXml = fetchXml.Replace("@FOODID", ((PairWithList)cb.SelectedItem).Id);

                    XElement orderXml = new XElement("ColumnOrder");
                    orderXml.Add(new XElement("Column", "dc_portiontypeid"));
                    orderXml.Add(new XElement("Column", "dc_portion_types.dc_name"));
                    orderXml.Add(new XElement("Column", "dc_portion_types.dc_abbreviation"));
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
                Row r = (Row)dataGrid1.SelectedItem;
                if (element.Descendants(entityName).Count() > 0)
                {
                    //Bind data
                    var rows = element.Descendants(entityName);

                    var row = rows.First();
                    PairWithList p = new PairWithList();

                    foreach (XElement xe in row.Elements())
                    {
                        if (xe.Name.LocalName.Equals("dc_portion_types.dc_name", StringComparison.OrdinalIgnoreCase))
                        {
                            r["dc_portion_types.dc_name"] = xe.Value;
                        }
                        else if (xe.Name.LocalName.Equals("dc_portion_types.dc_abbreviation", StringComparison.OrdinalIgnoreCase))
                        {
                            r["dc_portion_types.dc_abbreviation"] = xe.Value;
                        }
                        else if (xe.Name.LocalName.Equals("dc_portiontypeid", StringComparison.OrdinalIgnoreCase))
                        {
                            r["dc_portiontypeid"] = new PairWithList(xe.Value, xe.Attribute("Id").Value);
                        }
                    }
                    comboBox.KeyUp -= food_KeyDown;
                    comboBox.KeyUp += new KeyEventHandler(food_KeyDown);
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
            CreateUpdate();
           
        }
        private void CheckSave()
        {
            
        }

        private void CreateUpdate()
        {

            SortableCollectionView list = (SortableCollectionView)dataGrid1.ItemsSource;
            
            XElement shoppingListXml = new XElement("ShoppingLists");

            foreach (Row r in list)
            {
                //make sure something is selected
                if (r.RowChanged)
                {
                    busyIndicator.IsBusy = true;


                    XElement contactXml = new XElement("dc_shoppinglistitem");

                    XElement contact = new XElement("dc_shoppinglistid", shoppingListId.ToString());
                    contactXml.Add(contact);

                    XAttribute at = new XAttribute("entityname", "dc_shoppinglist");
                    contact.Add(at);

                    if (!String.IsNullOrEmpty((String)r["dc_portionsize"]))
                    {
                        contactXml.Add(new XElement("dc_portionsize", (String)r["dc_portionsize"]));
                    }
                    if (r["dc_name"].GetType() == typeof(PairWithList) && r["dc_name"] != null && !String.IsNullOrEmpty(((PairWithList)r["dc_name"]).Name))
                    {
                        contactXml.Add(new XElement("dc_name", ((PairWithList)r["dc_name"]).Name));
                    }
                    else
                    {
                        contactXml.Add(new XElement("dc_name", ((PairWithList)r["dc_foodid"]).Name));
                    }
                    if (new Guid(((PairWithList)r["dc_portiontypeid"]).Id) != Guid.Empty)
                    {
                        XElement portiontTypeId = new XElement("dc_portiontypeid", ((PairWithList)r["dc_portiontypeid"]).Id);
                        XAttribute attribute = new XAttribute("entityname", "dc_portion_types");
                        portiontTypeId.Add(attribute);
                        contactXml.Add(portiontTypeId);
                    }

                    if (new Guid(((PairWithList)r["dc_foodid"]).Id) != Guid.Empty)
                    {
                        XElement foodId = new XElement("dc_foodid", ((PairWithList)r["dc_foodid"]).Id);
                        XAttribute attribute2 = new XAttribute("entityname", "dc_foods");
                        foodId.Add(attribute2);
                        contactXml.Add(foodId);
                    }
                    else
                    {
                        XElement foodId = new XElement("dc_foodid", ((PairWithList)r["dc_name"]).Id);
                        XAttribute attribute2 = new XAttribute("entityname", "dc_foods");
                        foodId.Add(attribute2);
                        contactXml.Add(foodId);
                    }

                    contactXml.Add(new XElement("dc_shoppinglistitemid", ((String)r["dc_shoppinglistitemid"])));
                    if (r["new_record"] != null && (bool)r["new_record"])
                    {
                        XAttribute attributeCreate = new XAttribute("create", "true");
                        contactXml.Add(attributeCreate);
                    }
                    else
                    {
                        XAttribute attributeCreate = new XAttribute("create", "false");
                        contactXml.Add(attributeCreate);
                    }


                    XElement contactId = new XElement("dc_contactid", ((App)App.Current).contactId);
                    XAttribute attribute3 = new XAttribute("entityname", "contact");
                    contactId.Add(attribute3);
                    contactXml.Add(contactId);

                    shoppingListXml.Add(contactXml);

                }
            }
            if (shoppingListXml.Descendants().Count() > 0)
            {
                cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
                cms.CreateUpdateReturnEntitiesCompleted += new EventHandler<CrmSdk.CreateUpdateReturnEntitiesCompletedEventArgs>(cms_CreateUpdate);
                cms.CreateUpdateReturnEntitiesAsync(shoppingListXml, true);
            }

        }
        private void cms_CreateUpdate(object sender, CrmSdk.CreateUpdateReturnEntitiesCompletedEventArgs e)
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
                var nodes = e.Result.Descendants("resultset");

                SortableCollectionView data = (SortableCollectionView)dataGrid1.ItemsSource;

                foreach (var node in nodes)
                {
                    var shoppingListItem = node.Descendants("dc_shoppinglistitem");

                }
            }
            busyIndicator.IsBusy = false;
        }

        void edit_MouseLeftButtonUp(object sender, RoutedEventArgs e)
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

        }
        private void RetrieveShoppingItemsNoCategory(Guid contactId)
        {
            App ap = null;
            if (App.Current.GetType() == typeof(DynamicConnections.NutriStyle.MenuGenerator.App))
            {
                ap = (App)App.Current;
            }
            String fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical'>
              <entity name='dc_shoppinglistitem'>
                <attribute name='dc_name' />
                <attribute name='dc_portionsize' />
                <attribute name='dc_portiontypeid'/>
                
                <attribute name='dc_shoppinglistid'/>
                <attribute name='dc_shoppinglistitemid'/>
                <filter type='and'> 
                    <condition attribute='dc_foodid' operator='null'/> 
                </filter>
                    <link-entity name='dc_shoppinglist' from='dc_shoppinglistid' to='dc_shoppinglistid' alias='aa'>
                      <filter type='and'>
                        <condition attribute='statecode' operator='eq' value='0' />
                        <condition attribute='dc_contactid' operator='eq' value='@CONTACTID' />
                      </filter>
                      <link-entity name='dc_menu' alias='ab' to='dc_menuid' from='dc_menuid'>
                        <filter type='and'> 
                            <condition attribute='dc_primarymenu' value='1' operator='eq'/> 
                        </filter> 
                      </link-entity>
                    </link-entity>
              </entity>
            </fetch>";

            if (ap != null)
            {
                fetchXml = fetchXml.Replace("@CONTACTID", contactId.ToString());
                //fetchXml = fetchXml.Replace("@SHOPPINGLISTID", shoppinglistId.ToString());
            }
            XElement orderXml = new XElement("ColumnOrder");
            orderXml.Add(new XElement("Column", "dc_name"));
            orderXml.Add(new XElement("Column", "dc_portionsize"));
            orderXml.Add(new XElement("Column", "dc_portiontypeid"));

            orderXml.Add(new XElement("Column", "dc_shoppinglistid"));
            //orderXml.Add(new XElement("Column", "dc_component_category.dc_name"));
            orderXml.Add(new XElement("Column", "dc_shoppinglistitemid"));

            try
            {
                if (ap != null)
                {
                    busyIndicator.IsBusy = true;
                    busyIndicator.Visibility = System.Windows.Visibility.Visible;

                    CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
                    cms.RetrieveFetchXmlCompleted += new EventHandler<CrmSdk.RetrieveFetchXmlCompletedEventArgs>(cms_RetrieveFoodLikes2);
                    cms.RetrieveFetchXmlAsync(fetchXml, orderXml.ToString());
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
            }
        }
        private void cms_RetrieveFoodLikes2(object sender, CrmSdk.RetrieveFetchXmlCompletedEventArgs e)
        {
            buildGrid2("dc_shoppinglistitem", dataGrid1, busyIndicator, e.Result, 1);
        }

        private void buildGrid2(String entityName, DataGrid dataGrid, BusyIndicator bi, XElement element, int columns)
        {
            dataFoods = (SortableCollectionView)dataGrid.ItemsSource;
            try
            {
                if (element.Descendants(entityName).Count() > 0)
                {
                    Row r = new Row();
                    r["dc_name"] = new PairWithList("MISC", Guid.Empty.ToString());
                    r["controlenabled"] = false;
                    r["isdeletevisible"] = System.Windows.Visibility.Collapsed;
                    dataFoods.Add(r);

                    //Bind data
                    var rows = element.Descendants(entityName);

                    String groupName = String.Empty;
                    foreach (var row in rows)
                    {
                        Row rowData = new Row();


                        foreach (KeyValuePair<String, String> type in dataFoods.ColumnTypes)
                        {
                            if (type.Value.Equals("Lookup", StringComparison.OrdinalIgnoreCase))
                            {
                                rowData[type.Key] = new PairWithList(String.Empty, Guid.Empty.ToString());
                            }
                            else if (type.Value.Equals("Picklist", StringComparison.OrdinalIgnoreCase))
                            {
                                rowData[type.Key] = new PairWithList(String.Empty, String.Empty);//new Pair(String.Empty, "-1");
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
                                rowData["dc_name"] = new PairWithList(xe.Value, Guid.Empty.ToString());
                            }
                            else if (dataFoods.ColumnTypes.ContainsKey(xe.Name.LocalName) && dataFoods.ColumnTypes[xe.Name.LocalName].Equals("money", StringComparison.OrdinalIgnoreCase))
                            {
                                rowData[xe.Name.LocalName] = String.Format("{0:C2}", Convert.ToDecimal(xe.Value));
                            }
                            else if (dataFoods.ColumnTypes.ContainsKey(xe.Name.LocalName) && dataFoods.ColumnTypes[xe.Name.LocalName].Equals("lookup", StringComparison.OrdinalIgnoreCase))
                            {
                                //MessageBox.Show(xe.Name.LocalName + ":" + xe.Value + ":" + xe.Attribute("Id").Value);
                                rowData[xe.Name.LocalName] = new PairWithList(xe.Value, xe.Attribute("Id").Value);
                            }
                            else if (dataFoods.ColumnTypes.ContainsKey(xe.Name.LocalName) && dataFoods.ColumnTypes[xe.Name.LocalName].Equals("picklist", StringComparison.OrdinalIgnoreCase))
                            {
                                //MessageBox.Show(xe.Value+":"+xe.Attribute("Id").Value);
                                rowData[xe.Name.LocalName] = new PairWithList(xe.Value, xe.Attribute("Id").Value);
                            }
                            else
                            {
                                rowData[xe.Name.LocalName] = xe.Value;
                            }
                        }

                        rowData["controlenabled"] = true;
                        rowData["isdeletevisible"] = System.Windows.Visibility.Visible;
                        rowData["dc_portionsize"] = String.IsNullOrEmpty((String)rowData["dc_portionsize"]) ? String.Empty : String.Format("{0:0.0}", Convert.ToDecimal((String)rowData["dc_portionsize"]));
                        dataFoods.Add(rowData);
                        rowData.RowChanged = false;
                    }
                    dataGrid.ItemsSource = dataFoods;
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


        private void RetrieveShoppingItems(Guid contactId)
        {
            App ap = null;
            if (App.Current.GetType() == typeof(DynamicConnections.NutriStyle.MenuGenerator.App))
            {
                ap = (App)App.Current;
            }
            String fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
              <entity name='dc_shoppinglistitem'>
                <attribute name='dc_name' />
                <attribute name='dc_portionsize' />
                <attribute name='dc_foodid'/>
                <attribute name='dc_shoppinglistid'/>
                <attribute name='dc_shoppinglistitemid'/>
                    <link-entity name='dc_portion_types' alias='dc_portion_types' to='dc_portiontypeid' from='dc_portion_typesid'>
                        <attribute name='dc_name' />
                        <attribute name='dc_abbreviation' />
                    </link-entity>
        
                    <link-entity name='dc_shoppinglist' from='dc_shoppinglistid' to='dc_shoppinglistid' alias='aa'>
                      <filter type='and'>
                        <condition attribute='statecode' operator='eq' value='0' />
                        <condition attribute='dc_contactid' operator='eq' value='@CONTACTID' />
                      </filter>
                      <link-entity name='dc_menu' alias='ab' to='dc_menuid' from='dc_menuid'>
                        <filter type='and'> 
                            <condition attribute='dc_primarymenu' value='1' operator='eq'/> 
                        </filter> 
                      </link-entity>
                    </link-entity>
                    <link-entity name='dc_foods' alias='dc_foods' to='dc_foodid' from='dc_foodsid'>
                        <attribute name='dc_foodgroup'/>
                        <order attribute='dc_foodgroup' descending='false' />
                    </link-entity>
              </entity>
            </fetch>";

            if (ap != null)
            {
                fetchXml = fetchXml.Replace("@CONTACTID", contactId.ToString());
                //fetchXml = fetchXml.Replace("@MENUID", shoppinglistId.ToString());
            }
            XElement orderXml = new XElement("ColumnOrder");
            orderXml.Add(new XElement("Column", "dc_name"));
            orderXml.Add(new XElement("Column", "dc_portionsize"));
            //orderXml.Add(new XElement("Column", "dc_portiontypeid"));
            orderXml.Add(new XElement("Column", "dc_foodid"));
            orderXml.Add(new XElement("Column", "dc_shoppinglistid"));
            //orderXml.Add(new XElement("Column", "dc_component_category.dc_name"));
            orderXml.Add(new XElement("Column", "dc_foods.dc_foodgroup"));
            orderXml.Add(new XElement("Column", "dc_shoppinglistitemid"));

            orderXml.Add(new XElement("Column", "dc_portion_types.dc_name"));
            orderXml.Add(new XElement("Column", "dc_portion_types.dc_abbreviation"));


            try
            {
                if (ap != null)
                {
                    busyIndicator.IsBusy = true;
                    busyIndicator.Visibility = System.Windows.Visibility.Visible;

                    CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
                    cms.RetrieveFetchXmlCompleted += new EventHandler<CrmSdk.RetrieveFetchXmlCompletedEventArgs>(cms_RetrieveFoodLikes);
                    cms.RetrieveFetchXmlAsync(fetchXml, orderXml.ToString());
                }
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
                        if (xe.Name.LocalName.Equals("dc_shoppinglistid") || xe.Name.LocalName.Equals("dc_component_category.dc_name")
                            || xe.Name.LocalName.Equals("dc_foodid") || xe.Name.LocalName.Equals("dc_foods.dc_foodgroup")  ||
                            xe.Name.LocalName.Equals("dc_portion_types.dc_name", StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }
                        else if ((xe.Attribute("Type").Value.Equals("Lookup", StringComparison.OrdinalIgnoreCase) || xe.Name.LocalName.Equals("dc_name")) &&
                        !xe.Name.LocalName.Equals("dc_portion_types.dc_abbreviation"))
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

                            //CellETempPickList.Append("<basics2:ComboBoxWithValidation  TagName='" + xe.Name.LocalName + "'  SelectedPair='{Binding Path=Data, Converter={StaticResource rowConvertor}, ConverterParameter=" + xe.Name.LocalName + ", Mode=TwoWay}' IsEnabled='{Binding Path=Data, Converter={StaticResource rowConvertor}, ConverterParameter=controlenabled, Mode=TwoWay}' />");
                            CellETempPickList.Append("<basics2:ComboBoxWithValidation TagName='" + xe.Name.LocalName + "' SelectedPair='{Binding Path=Data, Converter={StaticResource rowConvertor}, ConverterParameter=" + xe.Name.LocalName + ", Mode=TwoWay}' IsEnabled='{Binding Path=Data, Converter={StaticResource rowConvertor}, ConverterParameter=controlenabled, Mode=TwoWay}' />");
                            CellETempPickList.Append("</DataTemplate>");
                            dg.CellEditingTemplate = (DataTemplate)XamlReader.Load(CellETempPickList.ToString());

                            if (xe.Name.LocalName.Equals("dc_portiontypeid", StringComparison.OrdinalIgnoreCase))
                            {
                                dg.IsReadOnly = true;
                                dg.Width = new DataGridLength(100);
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

                            CellETempPickList.Append("<basics2:CustomTextBox TagName='" + xe.Name.LocalName + "' Visibility='{Binding Path=Data, Converter={StaticResource rowConvertor}, ConverterParameter=isdeletevisible, Mode=OneWay}' TextAlign='Right' Format='0:0.0'  SelectedText='{Binding Path=Data, Converter={StaticResource rowConvertor}, ConverterParameter=" + xe.Name.LocalName + ", Mode=TwoWay}'  />");
                            CellETempPickList.Append("</DataTemplate>");
                            dg.CellTemplate = (DataTemplate)XamlReader.Load(CellETempPickList.ToString());

                            dg.Width = new DataGridLength(40);
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
                            //dg.Width = new DataGridLength(100);
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
                    CellETemp.Append("<Button HorizontalAlignment='Left' Cursor='Hand' ToolTipService.ToolTip='Remove From Shopping List' Width='26' Height='26' Visibility='{Binding Path=Data, Converter={StaticResource rowConvertor}, ConverterParameter=isdeletevisible, Mode=OneWay}'>");
                    CellETemp.Append("<Image Stretch='Fill' Name='Delete' Source='/DynamicConnections.NutriStyle.MenuGenerator;component/images/delete.png'/>");// MouseLeftButtonDown='ImageDelete_MouseLeftButtonDown'
                    CellETemp.Append("</Button>");
                    CellETemp.Append("</DataTemplate>");
                    dgImage.CellEditingTemplate = (DataTemplate)XamlReader.Load(CellETemp.ToString());

                    dgImage.IsReadOnly = false;
                    dgImage.Width = new DataGridLength(34);
                    //visable?
                    dgImage.Visibility = System.Windows.Visibility.Visible;
                    //dataGrid1.Columns.Add(dgImage);

                    dataGrid.Columns.Add(dgImage);

                    //Bind data
                    var rows = element.Descendants(entityName);

                    String groupName = String.Empty;
                    foreach (var row in rows)
                    {
                        Row rowData = new Row();

                        foreach (KeyValuePair<String, String> type in dataFoods.ColumnTypes)
                        {
                            if (type.Value.Equals("Lookup", StringComparison.OrdinalIgnoreCase))
                            {
                                rowData[type.Key] = new PairWithList(String.Empty, Guid.Empty.ToString());
                            }
                            else if (type.Value.Equals("Picklist", StringComparison.OrdinalIgnoreCase))
                            {
                                rowData[type.Key] = new PairWithList(String.Empty, String.Empty);// String.Empty;//new Pair(String.Empty, "-1");
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
                                rowData["dc_name"] = new PairWithList(xe.Value, Guid.Empty.ToString());
                            }
                            else if (dataFoods.ColumnTypes.ContainsKey(xe.Name.LocalName) && dataFoods.ColumnTypes[xe.Name.LocalName].Equals("money", StringComparison.OrdinalIgnoreCase))
                            {
                                rowData[xe.Name.LocalName] = String.Format("{0:C2}", Convert.ToDecimal(xe.Value));
                            }
                            else if (dataFoods.ColumnTypes.ContainsKey(xe.Name.LocalName) && dataFoods.ColumnTypes[xe.Name.LocalName].Equals("lookup", StringComparison.OrdinalIgnoreCase))
                            {
                                //MessageBox.Show(xe.Name.LocalName + ":" + xe.Value + ":" + xe.Attribute("Id").Value);
                                rowData[xe.Name.LocalName] = new PairWithList(xe.Value, xe.Attribute("Id").Value);
                            }
                            else if (dataFoods.ColumnTypes.ContainsKey(xe.Name.LocalName) && dataFoods.ColumnTypes[xe.Name.LocalName].Equals("picklist", StringComparison.OrdinalIgnoreCase))
                            {
                                //MessageBox.Show(xe.Value+":"+xe.Attribute("Id").Value);
                                rowData[xe.Name.LocalName] = new PairWithList(xe.Value, xe.Attribute("Id").Value);
                            }
                            else
                            {
                                rowData[xe.Name.LocalName] = xe.Value;
                            }
                        }
                        //String str = ((String)rowData["dc_component_category.dc_name"]);
                        //dc_foods.dc_foodgroup
                        String str = ((PairWithList)rowData["dc_foods.dc_foodgroup"]).Name;
                        if ((String.IsNullOrEmpty(groupName) || (!str.Equals(groupName))) && !String.IsNullOrEmpty(str))
                        {
                            Row r = new Row();

                            r["dc_name"] = new PairWithList(((PairWithList)rowData["dc_foods.dc_foodgroup"]).Name, Guid.Empty.ToString());
                            r["controlenabled"] = false;
                            r["isdeletevisible"] = System.Windows.Visibility.Collapsed;
                            groupName = ((PairWithList)rowData["dc_foods.dc_foodgroup"]).Name;

                            dataFoods.Add(r);
                        }
                        if (rowData["dc_shoppinglistid"].GetType() == typeof(string))
                        {
                            shoppingListId = new Guid((String)rowData["dc_shoppinglistid"]);
                        }
                        else if (rowData["dc_shoppinglistid"].GetType() == typeof(PairWithList))
                        {
                            shoppingListId = new Guid(((PairWithList)rowData["dc_shoppinglistid"]).Id);
                        }
                        rowData["controlenabled"] = true;
                        rowData["isdeletevisible"] = System.Windows.Visibility.Visible;
                        rowData.RowChanged = false;
                        //make sure that dc_portionsize is rounded to a 10th
                        rowData["dc_portionsize"] = String.IsNullOrEmpty((String)rowData["dc_portionsize"]) ? String.Empty : String.Format("{0:0.0}", Convert.ToDecimal((String)rowData["dc_portionsize"]));
                        
                        
                        dataFoods.Add(rowData);

                    }

                    dataGrid.ItemsSource = dataFoods;
                    dataGrid.BorderThickness = new Thickness(1);

                    // Space available to fill ( -18 Standard vScrollbar)
                    double space_available = (Grid.ActualWidth - 18 - 40 - 34 - 46); //18 is width of scroll bar, 150 is width of menu 
                    //figure out column types

                    if (space_available > 0)
                    {
                        foreach (DataGridColumn dg_c in dataGrid.Columns)
                        {
                            if (dg_c.SortMemberPath != null && dg_c.SortMemberPath.Equals("dc_name", StringComparison.OrdinalIgnoreCase))
                            {
                                dg_c.Width = new DataGridLength(space_available);
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
            dataGrid.Visibility = System.Windows.Visibility.Visible;
            RetrieveShoppingItemsNoCategory(ContactId);
        }
        private void PrintDirections(object sender, RoutedEventArgs e)
        {
            //no day
            HtmlPage.Window.Navigate(new Uri(General.ReportServerUrl(((App)App.Current).webServicesName) + "/ShoppingList&rs:Command=Render&ShoppingListId="+shoppingListId.ToString()), "_blank");
        }
        private void IterateRow(PrintPageEventArgs p)
        {

            if (rowIndex < dataFoods.Count)
            {
                var tb = new TextBlock { Text = ((PairWithList)dataFoods[rowIndex]["dc_name"]).Name };
                var tb2 = new TextBlock();
                if ((bool)dataFoods[rowIndex]["controlenabled"])
                {
                    tb2 = new TextBlock { Text = (String)dataFoods[rowIndex]["dc_portionsize"] + " " + ((PairWithList)dataFoods[rowIndex]["dc_portiontypeid"]).Name };
                }
                else
                {
                    tb2 = new TextBlock { Text = "-------------" };
                    tb.Text = tb.Text + "-----------";
                }
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
                    r[type.Key] = new PairWithList(String.Empty, Guid.Empty.ToString());
                }
                else if (type.Key.Equals("dc_shoppinglistitemid"))
                {
                    r[type.Key] = Guid.NewGuid().ToString();
                }
                else if (type.Value.Equals("Lookup", StringComparison.OrdinalIgnoreCase))
                {
                    r[type.Key] = new PairWithList(String.Empty, Guid.Empty.ToString());
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
            r.RowChanged = true;
            r["controlenabled"] = true;
            r["new_record"] = true;
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

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //RetrieveShoppingItems();
        }
    }
}









































