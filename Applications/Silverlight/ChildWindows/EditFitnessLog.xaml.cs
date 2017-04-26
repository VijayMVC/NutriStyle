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

namespace DynamicConnections.NutriStyle.MenuGenerator.ChildWindows
{
    public partial class EditFitnessLog : ChildWindow
    {

        Controls.ComboBox comboBoxCategory;
        Controls.ComboBox comboBoxPhysicalActivity;
        int rowCount;
        SortableCollectionView data;
        private RowIndexConverter _rowIndexConverter = new RowIndexConverter();
        App app = (App)App.Current;
        Row selectedRow;
        DateTime day;


        public EditFitnessLog()
        {
            InitializeComponent();
        }
        public EditFitnessLog(DateTime day)
        {
            InitializeComponent();
            rowCount = 0;
            this.day = day;
            Label.Text = "Fitness Log: " + day.ToString("MM/dd/yyyy");
            RetrieveFitnessLog(day);
            dataGrid1.LoadingRow += new EventHandler<DataGridRowEventArgs>(dataGrid1_LoadingRow);
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
                            ToolTipService.SetToolTip(ccb, "Remove From Fitness log");

                            ccb.MouseLeftButtonUp -= ccb_MouseLeftButtonUp;
                            ccb.MouseLeftButtonUp += new MouseButtonEventHandler(ccb_MouseLeftButtonUp);

                        }/*
                        else if (c.GetCellContent(e.Row).GetType() == typeof(TextBlock))
                        {
                            TextBox tb = (TextBox)c.GetCellContent(e.Row);
                            tb.SelectionChanged -= tb_SelectionChanged;
                            tb.SelectionChanged += new RoutedEventHandler(tb_SelectionChanged);
                        }*/
                        else if (c.GetCellContent(e.Row).GetType() == typeof(Controls.CustomTextBox))
                        {
                            CustomTextBox ctb = c.GetCellContent(e.Row) as Controls.CustomTextBox;
                            if (ctb.TagName.Equals("dc_durationminutes", StringComparison.OrdinalIgnoreCase))
                            {
                                Controls.CustomTextBox tb = (Controls.CustomTextBox)c.GetCellContent(e.Row);
                                tb.TextBox.SelectionChanged -= tb_SelectionChanged;
                                tb.TextBox.SelectionChanged += new RoutedEventHandler(tb_SelectionChanged);
                            }
                        }
                        else if (c.GetCellContent(e.Row).GetType() == typeof(Controls.ComboBox))
                        {
                            Controls.ComboBox ccb = c.GetCellContent(e.Row) as Controls.ComboBox;

                            if (ccb.TagName.Equals("dc_physicalactivitycategoryid", StringComparison.OrdinalIgnoreCase))
                            {

                                ccb.KeyDown -= category_KeyDown;
                                ccb.KeyDown += new KeyEventHandler(category_KeyDown);
                                /*
                                ccb.LostFocus -= ccb_LostFocus;
                                ccb.LostFocus += new RoutedEventHandler(ccb_LostFocus);
                                */
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

        void tb_SelectionChanged(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (!String.IsNullOrEmpty(tb.Text))
            {
                decimal hours = Convert.ToDecimal(tb.Text) / 60m;

                //find datagridrow
                FrameworkElement dgr = (FrameworkElement)sender;
                while (true)
                {
                    dgr = (FrameworkElement)VisualTreeHelper.GetParent(dgr) == null ? null : (FrameworkElement)VisualTreeHelper.GetParent(dgr);
                    if (dgr == null)
                    {
                        break;
                    }
                    else if (dgr is DataGridRow)
                    {
                        break;
                    }
                }
                //Found row.  Now find dc_phyicalactivityid
                Controls.ComboBox cb2 = null;
                DataGridRow row = (DataGridRow)dgr;
                foreach (DataGridColumn c in dataGrid1.Columns)
                {
                    //MessageBox.Show(""+c.GetCellContent(e.Row).GetType());
                    if (c.GetCellContent(row).GetType() == typeof(Controls.ComboBox))
                    {
                        Controls.ComboBox ctb = c.GetCellContent(row) as Controls.ComboBox;
                        if (ctb.TagName.Equals("dc_physicalactivityid", StringComparison.OrdinalIgnoreCase))
                        {
                            cb2 = ctb;
                        }
                    }
                }
                Pair p = cb2.SelectedPair;
                Row r = (Row)dataGrid1.SelectedItem;
                r.RowChanged = true;//validate this
                decimal mets = Convert.ToDecimal(r["dc_mets"]);
                //now find calories
                decimal kcals = app.weightInKilos * mets * hours;
                foreach (DataGridColumn c in dataGrid1.Columns)
                {
                    //MessageBox.Show(""+c.GetCellContent(e.Row).GetType());
                    if (row != null && c.SortMemberPath != null && c.SortMemberPath.Equals("dc_calories", StringComparison.OrdinalIgnoreCase))
                    {
                        TextBlock tb2 = (TextBlock)c.GetCellContent(row);
                        tb2.Text = Math.Round(kcals, 2).ToString();
                        r["dc_calories"] = tb2.Text;
                    }
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
                SortableCollectionView data = (SortableCollectionView)dataGrid1.ItemsSource;

                Image img = sender as Image;
                String Id = (String)((Row)dataGrid1.SelectedItem)["dc_fitnesslogid"];
                Row r = (Row)dataGrid1.SelectedItem;
                if (!String.IsNullOrEmpty(Id))
                {
                    XElement deleteXml = new XElement("dc_fitnesslog", new XElement("id", Id));

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

        private void category_KeyDown(object sender, KeyEventArgs e)
        {

            comboBoxCategory = (Controls.ComboBox)sender;

            String text = comboBoxCategory.MyAutoCompleteBox.Text;

            if (text.Length > 1)
            {

                comboBoxCategory.MyAutoCompleteBox.LostFocus -= Category_SelectionChanged;
                comboBoxCategory.MyAutoCompleteBox.LostFocus += new RoutedEventHandler(Category_SelectionChanged); //new SelectionChangedEventHandler(Category_SelectionChanged);


                XElement orderXml = null;

                String fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
                  <entity name='dc_physicalactivitycategory'>
                    <attribute name='dc_name' />
                    <attribute name='dc_physicalactivitycategoryid' />
                    <order attribute='dc_name' descending='false' />
                    <filter type='and'>
                        <condition attribute='dc_name' value='%@TEXT%' operator='like'/>
                    </filter>
                  </entity>
                </fetch>";

                fetchXml = fetchXml.Replace("@TEXT", text);

                orderXml = new XElement("ColumnOrder");
                orderXml.Add(new XElement("Column", "dc_name"));
                orderXml.Add(new XElement("Column", "dc_physicalactivitycategoryid"));

                try
                {
                    CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
                    cms.RetrieveFetchXmlCompleted += new EventHandler<CrmSdk.RetrieveFetchXmlCompletedEventArgs>(cms_RetrieveCategory);
                    cms.RetrieveFetchXmlAsync(fetchXml, orderXml.ToString());
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                    MessageBox.Show(err.StackTrace);
                }
            }
        }
        private void cms_RetrieveCategory(object sender, CrmSdk.RetrieveFetchXmlCompletedEventArgs e)
        {
            String entityName = "dc_physicalactivitycategory";

            var element = e.Result;

            List<Pair> dataCollection = new List<Pair>();
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
                        dataCollection.Add(new Pair(name, Id));
                    }
                    comboBoxCategory.MyAutoCompleteBox.ItemsSource = dataCollection;
                    comboBoxCategory.OpenDropdown(true);


                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
            }
        }
        //void Category_SelectionChanged(object sender, SelectionChangedEventArgs e)
        void Category_SelectionChanged(object sender, RoutedEventArgs e)
        {

            //Make sure something is selected on the row

            if (dataGrid1.SelectedItem != null)
            {
                //get the dc_physicalactivitycategoryid
                //Make sure something is selected
                if (comboBoxCategory.SelectedPair != null)
                {
                    String Id = comboBoxCategory.SelectedPair.Id;
                    AutoCompleteBox cb = (AutoCompleteBox)sender;
                    //find datagridrow
                    FrameworkElement dgr = (FrameworkElement)cb;
                    while (true)
                    {
                        dgr = (FrameworkElement)VisualTreeHelper.GetParent(dgr) == null ? null : (FrameworkElement)VisualTreeHelper.GetParent(dgr);
                        if (dgr == null)
                        {
                            break;
                        }
                        else if (dgr is DataGridRow)
                        {
                            break;
                        }
                    }
                    //Found row.  Now find dc_phyicalactivityid
                    Controls.ComboBox cb2 = null;
                    DataGridRow row = (DataGridRow)dgr;
                    foreach (DataGridColumn c in dataGrid1.Columns)
                    {
                        //MessageBox.Show(""+c.GetCellContent(e.Row).GetType());
                        if (c.GetCellContent(row).GetType() == typeof(Controls.ComboBox))
                        {
                            Controls.ComboBox ctb = c.GetCellContent(row) as Controls.ComboBox;
                            if (ctb.TagName.Equals("dc_physicalactivityid", StringComparison.OrdinalIgnoreCase))
                            {
                                cb2 = ctb;
                            }
                        }
                    }
                    //set to null
                    comboBoxPhysicalActivity = cb2;
                    if (comboBoxPhysicalActivity.SelectedPair != null)
                    {
                        comboBoxPhysicalActivity.SelectedPair = new Pair(String.Empty, Guid.Empty.ToString());
                    }
                    comboBoxPhysicalActivity.MyAutoCompleteBox.ItemsSource = null;


                    cb2.IsEnabled = true;
                    if (cb.SelectedItem != null && !String.IsNullOrEmpty(cb.Text))
                    {
                        Pair p = (Pair)cb.SelectedItem;
                        //Retreive 
                        String fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                      <entity name='dc_physicalactivity'>
                        <attribute name='dc_name' />
                        <attribute name='dc_mets' />
                        <attribute name='dc_physicalactivityid' />
                        <order attribute='dc_name' descending='false' />
                        <filter type='and'>
                            <condition attribute='dc_physicalactivitycategoryid' value='@ID' operator='eq'/>
                        </filter>     
                      </entity>
                    </fetch>";

                        fetchXml = fetchXml.Replace("@ID", Id);

                        XElement orderXml = new XElement("ColumnOrder");
                        orderXml.Add(new XElement("Column", "dc_name"));
                        orderXml.Add(new XElement("Column", "dc_mets"));
                        orderXml.Add(new XElement("Column", "dc_physicalactivityid"));
                        orderXml.Add(new XElement("Column", "dc_foodsid"));

                        try
                        {
                            CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
                            cms.RetrieveFetchXmlCompleted += new EventHandler<CrmSdk.RetrieveFetchXmlCompletedEventArgs>(cms_RetrievePhysicalActivities);
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
        private void cms_RetrievePhysicalActivities(object sender, CrmSdk.RetrieveFetchXmlCompletedEventArgs e)
        {

            String entityName = "dc_physicalactivity";
            XElement element = e.Result;

            List<Pair> dataCollection = new List<Pair>();
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
                        String Id   = String.Empty;
                        String mets = String.Empty;
                        foreach (XElement xe in row.Elements())
                        {
                            if (xe.Name.LocalName.Equals("dc_name", StringComparison.OrdinalIgnoreCase))
                            {
                                name = xe.Value;
                            }
                            else if (xe.Name.LocalName.Equals(entityName + "id", StringComparison.OrdinalIgnoreCase))
                            {
                                Id = xe.Value;
                            }else if (xe.Name.LocalName.Equals("dc_mets", StringComparison.OrdinalIgnoreCase))
                            {
                                mets = xe.Value;
                            }
                        }
                        dataCollection.Add(new Pair(name, Id, mets));
                    }
                    comboBoxPhysicalActivity.MyAutoCompleteBox.ItemsSource = dataCollection;
                    //comboBoxPhysicalActivity.OpenDropdown(true);
                    comboBoxPhysicalActivity.MyAutoCompleteBox.LostFocus -= PhysicalActivity_SelectionChanged;
                    comboBoxPhysicalActivity.MyAutoCompleteBox.LostFocus += new RoutedEventHandler(PhysicalActivity_SelectionChanged); //new SelectionChangedEventHandler(Category_SelectionChanged);


                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
            }
        }
        void PhysicalActivity_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var x = (FrameworkElement)sender;
            Controls.ComboBox cb = null;
            while (true)
            {
                x = (FrameworkElement)VisualTreeHelper.GetParent(x);
                if (x is Controls.ComboBox)
                {
                    cb = ( Controls.ComboBox)x;
                    break;
                }
            }
            
            if (cb.SelectedPair != null)
            {
                Pair p = cb.SelectedPair;
                
                decimal mets = Convert.ToDecimal(p.EntityType);//crap code.  Need to use a better container

                Row r = (Row)dataGrid1.SelectedItem;
                r["dc_mets"] = mets.ToString();
                r.RowChanged = true;
                App app = (App)App.Current;
                decimal hours = 1;
                //find datagridrow
                FrameworkElement dgr = (FrameworkElement)cb;
                while (true)
                {
                    dgr = (FrameworkElement)VisualTreeHelper.GetParent(dgr) == null ? null : (FrameworkElement)VisualTreeHelper.GetParent(dgr);
                    if (dgr == null)
                    {
                        break;
                    }
                    else if (dgr is DataGridRow)
                    {
                        break;
                    }
                }
                //Found row.  Now find duration
                DataGridRow row = (DataGridRow)dgr;
                foreach (DataGridColumn c in dataGrid1.Columns)
                {
                    //MessageBox.Show(""+c.GetCellContent(e.Row).GetType());
                    if (row != null && c.GetCellContent(row).GetType() == typeof(Controls.CustomTextBox))
                    {
                        Controls.CustomTextBox ctb = c.GetCellContent(row) as Controls.CustomTextBox;
                        if (!String.IsNullOrEmpty(ctb.TextBox.Text))
                        {
                            hours = Convert.ToDecimal(ctb.TextBox.Text) / 60m;
                        }
                        else
                        {
                            hours = 0;
                        }
                    }
                }
                //now find calories
                decimal kcals = app.weightInKilos * mets * hours;
                foreach (DataGridColumn c in dataGrid1.Columns)
                {
                    //MessageBox.Show(""+c.GetCellContent(e.Row).GetType());
                    if (row != null && c.SortMemberPath != null && c.SortMemberPath.Equals("dc_calories", StringComparison.OrdinalIgnoreCase))
                    {
                        TextBlock tb = (TextBlock)c.GetCellContent(row);
                        tb.Text = Math.Round(kcals, 2).ToString();
                        r["dc_calories"] = tb.Text;
                    }
                }
            }
        }
        private void RetrieveFitnessLog(DateTime day)
        {
            String fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
              <entity name='dc_fitnesslog'>
                <attribute name='dc_fitnesslogid' />
                <attribute name='dc_physicalactivitycategoryid' />
                <attribute name='dc_physicalactivityid' />
                <attribute name='dc_durationminutes' />
                <attribute name='dc_calories' />
                <order attribute='dc_physicalactivityid' descending='false' />
                <filter type='and'>
                  <condition attribute='statecode' operator='eq' value='0' />
                  <condition attribute='dc_contactid' operator='eq' value='@CONTACTID' />
                  <condition attribute='dc_date' operator='on' value='@DATETIME' />
                </filter>
                <link-entity name='dc_physicalactivity' alias='dc_physicalactivity' to='dc_physicalactivityid' from='dc_physicalactivityid'>
                    <attribute name='dc_mets' />
                </link-entity>
              </entity>
            </fetch>";

            fetchXml = fetchXml.Replace("@CONTACTID", app.contactId);
            fetchXml = fetchXml.Replace("@DATETIME", day.ToString("MM/dd/yyyy"));

            XElement orderXml = new XElement("ColumnOrder");
            orderXml.Add(new XElement("Column", "dc_physicalactivitycategoryid"));
            orderXml.Add(new XElement("Column", "dc_physicalactivityid"));
            orderXml.Add(new XElement("Column", "dc_durationminutes"));
            orderXml.Add(new XElement("Column", "dc_calories"));
            orderXml.Add(new XElement("Column", "dc_physicalactivity.dc_mets"));
            orderXml.Add(new XElement("Column", "dc_fitnesslogid"));

            try
            {
                busyIndicator.IsBusy = true;
                busyIndicator.Visibility = System.Windows.Visibility.Visible;

                CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
                cms.RetrieveFetchXmlCompleted += new EventHandler<CrmSdk.RetrieveFetchXmlCompletedEventArgs>(cms_RetrieveFitnessLog);
                cms.RetrieveFetchXmlAsync(fetchXml, orderXml.ToString());
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
            }
        }
        private void cms_RetrieveFitnessLog(object sender, CrmSdk.RetrieveFetchXmlCompletedEventArgs e)
        {

            buildGrid("dc_fitnesslog", dataGrid1, busyIndicator, e.Result, 1);

        }

        private void buildGrid(String entityName, DataGrid dataGrid, BusyIndicator bi, XElement element, int columns)
        {

            data = new SortableCollectionView();
            try
            {

                XElement xEl = element.Descendants("columns").ToList()[0];//get first one
                //MessageBox.Show(xEl.ToString());
                foreach (XElement xe in xEl.Elements())
                {
                    data.ColumnTypes.Add(xe.Name.LocalName, xe.Attribute("Type").Value);

                    if (!data.EntityTypes.ContainsKey((xe.Name.LocalName)))
                    {
                        data.EntityTypes.Add(xe.Name.LocalName, xe.Attribute("Entity").Value);
                    }
                    if (xe.Name.LocalName.Equals("dc_mets", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                    if (xe.Attribute("Type").Value.Equals("String", StringComparison.OrdinalIgnoreCase) ||
                        xe.Attribute("Type").Value.Equals("UniqueIdentifier", StringComparison.OrdinalIgnoreCase) ||
                        xe.Name.LocalName.Equals("dc_calories", StringComparison.OrdinalIgnoreCase))
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
                        dg.IsReadOnly = true;
                        dataGrid.Columns.Add(dg);
                        dg.Width = new DataGridLength(65);
                    }

                    else if (xe.Attribute("Type").Value.Equals("Decimal", StringComparison.OrdinalIgnoreCase))
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

                        CellETempPickList.Append("<basics2:CustomTextBox TagName='" + xe.Name.LocalName + "' Foreground='#4C97CC' SelectedText='{Binding Path=Data, Converter={StaticResource rowConvertor}, ConverterParameter=" + xe.Name.LocalName + ", Mode=TwoWay}' />");
                        CellETempPickList.Append("</DataTemplate>");
                        dg.CellTemplate = (DataTemplate)XamlReader.Load(CellETempPickList.ToString());

                        dg.Width = new DataGridLength(140);
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

                        CellETempPickList.Append("<basics2:ComboBox   TagName='" + xe.Name.LocalName + "' Foreground='#4C97CC' SelectedPair='{Binding Path=Data, Converter={StaticResource rowConvertor}, ConverterParameter=" + xe.Name.LocalName + ", Mode=TwoWay}' />");
                        CellETempPickList.Append("</DataTemplate>");
                        dg.CellEditingTemplate = (DataTemplate)XamlReader.Load(CellETempPickList.ToString());

                        if (xe.Name.LocalName.Equals("dc_physicalactivityid", StringComparison.OrdinalIgnoreCase))
                        {
                            dg.Width = new DataGridLength(265);
                        }
                        else
                        {
                            dg.Width = new DataGridLength(150);
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
                dataGrid1.Columns.Add(dgImage);

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
                    }
                    data.Add(rowData);
                }

                dataGrid.ItemsSource = data;
                bi.IsBusy = false;
                dataGrid.Visibility = System.Windows.Visibility.Visible;


                dataGrid.BorderThickness = new Thickness(1);

                // Space available to fill ( -18 Standard vScrollbar)
                double space_available = (LayoutRoot.ActualWidth - 18 - 90 - 35); //18 is width of scroll bar, 150 is width of menu 
                //figure out column types

                

            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
            }


        }


        /// <summary>
        /// For adding fitness row
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, EventArgs e)
        {

            //SortableCollectionView dgCollection = (SortableCollectionView)dataGrid1.ItemsSource;
            //Now add row to correct datagrid
            Row r = new Row();

            //Populate columnTypes  --  rowData.ColumnTypes.Add(xe.Name.LocalName, xe.Attribute("Type").Value);
            foreach (KeyValuePair<String, String> type in data.ColumnTypes)
            {
                if (type.Value.Equals("Lookup", StringComparison.OrdinalIgnoreCase))
                {
                    r[type.Key] = new Pair(String.Empty, Guid.Empty.ToString(), data.EntityTypes[type.Key]);
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
                //dataFoods.Add(r);
                //dgCollection.Insert(0, r);
                data.Add(r);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            App ap = (App)App.Current;
            Row r = null;

            //get row
            foreach (Row row in data)
            {
                if (row.RowChanged)
                {
                    r = row;
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

                XElement eventXml = new XElement("dc_fitnesslog");
                try
                {
                    foreach (String columnName in data.ColumnTypes.Keys)
                    {
                        //MessageBox.Show("ColumnName: " + columnName + ": type: " + dataFoods.ColumnTypes[columnName]);
                        if (data.ColumnTypes[columnName].Equals("DateTime", StringComparison.OrdinalIgnoreCase))
                        {
                            if (r[columnName] != null)
                            {
                                eventXml.Add(new XElement(columnName, ((DateTime)r[columnName])));
                            }
                        }
                        else if (data.ColumnTypes[columnName].Equals("Lookup", StringComparison.OrdinalIgnoreCase))
                        {
                            if (r[columnName] != null && r[columnName].GetType() == typeof(Pair))
                            {
                                if (((Pair)r[columnName]).Id != "" && ((Pair)r[columnName]).Id != String.Empty)
                                {
                                    XElement lookupNode = new XElement(new XElement(columnName, ((Pair)r[columnName]).Id));
                                    eventXml.Add(lookupNode);

                                    XAttribute at = new XAttribute("entityname", data.EntityTypes[columnName]);
                                    lookupNode.Add(at);
                                }
                            }
                        }
                        else if (data.ColumnTypes[columnName].Equals("Picklist", StringComparison.OrdinalIgnoreCase))
                        {
                            eventXml.Add(new XElement(columnName, ((Pair)r[columnName]).Id));
                        }
                        else if (data.ColumnTypes[columnName].Equals("UniqueIdentifier", StringComparison.OrdinalIgnoreCase))
                        {
                            if (!String.IsNullOrEmpty((String)r[columnName]))
                            {
                                eventXml.Add(new XElement(columnName, ((String)r[columnName])));
                            }
                        }
                        else if (data.ColumnTypes[columnName].Equals("String", StringComparison.OrdinalIgnoreCase))
                        {
                            if (!String.IsNullOrEmpty((String)r[columnName]))
                            {
                                eventXml.Add(new XElement(columnName, (String)r[columnName]));
                            }
                        }
                        else if (data.ColumnTypes[columnName].Equals("Decimal", StringComparison.OrdinalIgnoreCase))
                        {
                            if (!String.IsNullOrEmpty((String)r[columnName]))
                            {
                                eventXml.Add(new XElement(columnName, (String)r[columnName]));
                            }
                        }
                        else if (data.ColumnTypes[columnName].Equals("Memo", StringComparison.OrdinalIgnoreCase))
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
                cms.CreateUpdateCompleted += new EventHandler<CrmSdk.CreateUpdateCompletedEventArgs>(cms_CreateUpdateGrid);
                cms.CreateUpdateAsync(eventXml);

            }
        }
        private void cms_CreateUpdateGrid(object sender, CrmSdk.CreateUpdateCompletedEventArgs e)
        {
            var results = from x in e.Result.Descendants("Success") select x;
            Guid Id = new Guid(e.Result.Value.ToString());
            selectedRow.RowChanged = false;
            //set id
            selectedRow["dc_fitnesslogid"] = Id.ToString();
            //call back
            Save_Click(sender, null);
        }

    }
}

