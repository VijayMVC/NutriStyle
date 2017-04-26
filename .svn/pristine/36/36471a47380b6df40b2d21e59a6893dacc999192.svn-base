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
using System.Windows.Controls.DataVisualization;
using System.Windows.Controls.DataVisualization.Charting.Primitives;

namespace DynamicConnections.NutriStyle.MenuGenerator.Pages
{
    public partial class MenuOptions : Page
    {

        private RowIndexConverter _rowIndexConverter = new RowIndexConverter();
        bool generateMenu;
        
        String presetFilter;
        SortableCollectionView data = new SortableCollectionView();
        public MenuOptions()
        {
            presetFilter = "none";
            InitializeComponent();
            //Setup birthday values
            //RetrieveMenuOptions();

        }

        private void RetrieveContactProfile()
        {
            App ap = (App)App.Current;
            Contact c = ap.contact;
            name.Content = c.Email;

            dailyTarget.Text = String.Format("{0:0,0}", c.DEE);

            //kcalTarget.Text = String.Format("{0:0,0}", c.KcalTarget);
            if (c.KcalTarget != null)
            {
                ((App)App.Current).kcalCalculatedTarget = c.KcalTarget.Value;
            }
            //SpecifyCaloricTarget.IsChecked = c.UserSpecifiedKcalTarget;
            
            morningSnack.IsChecked      = c.MorningSnack;
            afternoonSnack.IsChecked    = c.AfternoonSnack;
            eveningSnack.IsChecked      = c.EveningSnack;

            int count = 0;
            SortableCollectionView data = (SortableCollectionView)dataGrid1.ItemsSource;
            foreach (Row r in data)
            {
                if (new Guid((String)r["dc_presetsid"]) == c.PresetId)
                {
                    dataGrid1.SelectedIndex = count;
                    dataGrid1_MouseLeftButtonDown(null, null);
                    break;
                }
                count++;
            }
            LayoutRoot.DataContext = c;
            customKcalTarget_Click(null, null);
        }

        void dataGrid1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //Populate the textbox with the description
                Row r = (Row)dataGrid1.SelectedItem;
                String description = (String)r["dc_description"];

                decimal cho = Convert.ToDecimal(r["dc_cho_pct"]);
                decimal fat = Convert.ToDecimal(r["dc_fat_pct"]);
                decimal pro = Convert.ToDecimal(r["dc_pro_pct"]);

                List<KeyValuePair<String, decimal>> list = new List<KeyValuePair<string, decimal>>();
                list.Add(new KeyValuePair<string, decimal>("Protien", pro));
                list.Add(new KeyValuePair<string, decimal>("Fat", fat));
                list.Add(new KeyValuePair<string, decimal>("Carbs", cho));
                /* DO NOT DELETE
                //deal with snacks
                if (Convert.ToBoolean(r["dc_incl_morningsnack"]))
                {
                    morningSnack.IsEnabled = true;
                }
                else
                {
                    morningSnack.IsEnabled = false;
                }

                if (Convert.ToBoolean(r["dc_incl_afternoonsnack"]))
                {
                    afternoonSnack.IsEnabled = true;
                }
                else
                {
                    afternoonSnack.IsEnabled = false;
                }
                if (Convert.ToBoolean(r["dc_incl_eveningsnack"]))
                {
                    eveningSnack.IsEnabled = true;
                }
                else
                {
                    eveningSnack.IsEnabled = false;
                }
                */

                ((PieSeries)chart.Series[0]).ItemsSource = list;

                Rect pieBounds = new Rect();
                var center = new Point();
                var radius = 0d;
                ResourceDictionaryCollection palette = new ResourceDictionaryCollection();
                try
                {
                    EdgePanel plotArea = (EdgePanel)((PieSeries)chart.Series[0]).Parent;

                    plotArea.Width = plotArea.ActualHeight;//makes it a sqaure
                    Style style = new Style(typeof(LegendItem));//((PieSeries)chart.Series[0]).LegendItemStyle;

                    style.Setters.Add(new Setter(WidthProperty, (244) - plotArea.Width - 95d));

                    //((PieSeries)chart.Series[0]).LegendItemStyle = style;

                    if (null != plotArea)
                    {
                        // Calculate the diameter of the pie (0.95 multiplier is from PieSeries implementation)
                        var diameter = Math.Min(plotArea.Width, plotArea.ActualHeight) * 0.95;
                        // Calculate the bounding rectangle of the pie
                        var leftTop = new Point((plotArea.Width - diameter) / 2, (plotArea.ActualHeight - diameter) / 2);
                        var rightBottom = new Point(leftTop.X + diameter, leftTop.Y + diameter);
                        pieBounds = new Rect(leftTop, rightBottom);
                        // Call the provided updater action for each PieDataPoint
                    }
                    double x = pieBounds.Left + ((pieBounds.Right - pieBounds.Left) / 2);
                    double y = pieBounds.Top + ((pieBounds.Bottom - (pieBounds.Top)) / 2);

                    x += 8;
                    //y -= 8;

                    center = new Point(x, y);

                    radius = (pieBounds.Right - pieBounds.Left) / 2;
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                    MessageBox.Show(err.StackTrace);
                }

                //set colors
                for (int x = 0; x < 3; x++)
                {
                    Style style2 = new Style(typeof(Control));
                    GradientStopCollection gsc = new GradientStopCollection();

                    GradientStop gs = new GradientStop();
                    GradientStop gs2 = new GradientStop();
                    GradientStop gs3 = new GradientStop();
                    if (x == 0)
                    {//red
                        gs.Color = Color.FromArgb(255, 255, 0, 0);
                        gs.Offset = 0.0;

                        gs2 = new GradientStop();
                        gs2.Color = Color.FromArgb(255, 200, 0, 0);
                        gs2.Offset = 0.8;

                        gs3 = new GradientStop();
                        gs3.Color = Color.FromArgb(255, 150, 0, 0);
                        gs3.Offset = 1.0;
                    }
                    else if (x == 1)
                    {//blue
                        gs.Color = Color.FromArgb(255, 0, 0, 255);
                        gs.Offset = 0.0;

                        gs2 = new GradientStop();
                        gs2.Color = Color.FromArgb(255, 0, 0, 200);
                        gs2.Offset = 0.8;

                        gs3 = new GradientStop();
                        gs3.Color = Color.FromArgb(255, 0, 0, 150);
                        gs3.Offset = 1.0;
                    }
                    else if (x == 2)
                    {//green
                        gs.Color = Color.FromArgb(255, 0, 255, 0);
                        gs.Offset = 0.0;

                        gs2 = new GradientStop();
                        gs2.Color = Color.FromArgb(255, 0, 200, 0);
                        gs2.Offset = 0.8;

                        gs3 = new GradientStop();
                        gs3.Color = Color.FromArgb(255, 0, 150, 0);
                        gs3.Offset = 1.0;
                    }
                    gsc.Add(gs);
                    gsc.Add(gs2);
                    gsc.Add(gs3);

                    RadialGradientBrush rgb = new RadialGradientBrush(gsc);
                    rgb.MappingMode         = BrushMappingMode.Absolute;
                    rgb.SpreadMethod        = GradientSpreadMethod.Reflect;

                    rgb.Center = center;
                    rgb.GradientOrigin = center;
                    rgb.RadiusX = radius;
                    rgb.RadiusY = radius;

                    style2.Setters.Add(new Setter(BackgroundProperty, rgb));
                    style2.Setters.Add(new Setter(TemplateProperty, this.Resources["pi"]));

                    ResourceDictionary dictionary = new ResourceDictionary();
                    dictionary.Add("DataPointStyle", style2);
                    palette.Add(dictionary);

                }
                chart.Palette = palette;
                busyIndicator.IsBusy = false;

                menuDescription.Text = description;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
            }
        }

        private void RetrieveMenuOptions()
        {
            //only active
            String fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
              <entity name='dc_presets'>
                <attribute name='dc_name' />
                <attribute name='dc_description' />
                <attribute name='dc_cho_pct' />
                <attribute name='dc_fat_pct' />
                <attribute name='dc_pro_pct' />

                <attribute name='dc_incl_morningsnack' />
                <attribute name='dc_incl_afternoonsnack' />
                <attribute name='dc_incl_eveningsnack' />
                
                <attribute name='dc_vegan' />
                <attribute name='dc_dairyfree' />
                <attribute name='dc_glutenfree' />
                <attribute name='dc_vegetarian' />
                <attribute name='dc_parent' />

                <attribute name='dc_presetsid' />
                <order attribute='dc_orderonmenuprefscreen' descending='false' />
                <filter type='and'> 
                    <condition attribute='statecode' value='0' operator='eq'/> 
                </filter>
              </entity>
            </fetch>";

            XElement orderXml = new XElement("ColumnOrder");
            orderXml.Add(new XElement("Column", "dc_name"));
            orderXml.Add(new XElement("Column", "dc_description"));
            orderXml.Add(new XElement("Column", "dc_cho_pct"));
            orderXml.Add(new XElement("Column", "dc_pro_pct"));
            orderXml.Add(new XElement("Column", "dc_fat_pct"));

            orderXml.Add(new XElement("Column", "dc_incl_morningsnack"));
            orderXml.Add(new XElement("Column", "dc_incl_afternoonsnack"));
            orderXml.Add(new XElement("Column", "dc_incl_eveningsnack"));

            orderXml.Add(new XElement("Column", "dc_vegan"));
            orderXml.Add(new XElement("Column", "dc_dairyfree"));
            orderXml.Add(new XElement("Column", "dc_glutenfree"));
            orderXml.Add(new XElement("Column", "dc_vegetarian"));

            orderXml.Add(new XElement("Column", "dc_parent"));


            orderXml.Add(new XElement("Column", "dc_presetsid"));

            try
            {
                busyIndicator.IsBusy = true;
                busyIndicator.Visibility = System.Windows.Visibility.Visible;

                CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
                cms.RetrieveFetchXmlCompleted += new EventHandler<CrmSdk.RetrieveFetchXmlCompletedEventArgs>(cms_RetrieveMenuOptions);
                cms.RetrieveFetchXmlAsync(fetchXml, orderXml.ToString());
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
            }
        }
        private void cms_RetrieveMenuOptions(object sender, CrmSdk.RetrieveFetchXmlCompletedEventArgs e)
        {

            buildGrid("dc_presets", dataGrid1, busyIndicator, e.Result, 1);
            //MessageBox.Show("error: "+e.Error);
            //RetrieveContactProfile();
        }


        private void buildGrid(String entityName, DataGrid dataGrid, BusyIndicator bi, XElement element, int columns)
        {
            dataGrid.ItemsSource = null;
            dataGrid.Columns.Clear();
            data = new SortableCollectionView();
            
            try
            {
                

                if (element.Descendants(entityName).Count() > 0)
                {
                    XElement xEl = element.Descendants("columns").ToList()[0];//get first one
                    //MessageBox.Show(xEl.ToString());

                    foreach (XElement xe in xEl.Elements())
                    {
                        if (xe.Name.LocalName.Equals("dc_name", StringComparison.OrdinalIgnoreCase))
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
                            dg.Width = new DataGridLength(215);
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
                        }
                        data.Add(rowData);
                    }
                    SortableCollectionView filteredView = new SortableCollectionView();
                    foreach (Row r in data)
                    {
                        if (r["dc_parent"] != null && Boolean.Parse((String)r["dc_parent"]))
                        {
                            filteredView.Add(r);
                        }
                    }

                    dataGrid.ItemsSource = filteredView;
                    bi.IsBusy = false;
                    dataGrid.Visibility = System.Windows.Visibility.Visible;
                    dataGrid.SelectedIndex = 0;
                    dataGrid1_MouseLeftButtonDown(null, null);

                    dataGrid1.MouseLeftButtonUp += new MouseButtonEventHandler(dataGrid1_MouseLeftButtonDown);

                    dataGrid.BorderThickness = new Thickness(1);

                }
                RetrieveContactProfile();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
            }


        }
        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }
        /// <summary>
        /// Save data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Make sure that a grid element is selected
            if (dataGrid1.SelectedItem == null)
            {
                Status s = new Status("Please select a menu option", false);
            }
            else
            {
                //validate first

                SpecifyCaloricTarget.GetBindingExpression(CheckBox.IsCheckedProperty).UpdateSource();

                if (SpecifyCaloricTarget.IsChecked.Value)
                {
                    if (!String.IsNullOrEmpty(kcalTarget.Text))
                    {
                        kcalTarget.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                        //PoundsPerWeek.GetBindingExpression(ComboBoxWithValidation.SelectedPairProperty).UpdateSource();
                    }
                    else //value is blank
                    {
                        kcalTarget.Text = decimal.MinValue.ToString();
                        kcalTarget.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                        kcalTarget.Text = String.Empty;
                    }
                    
                }

                if (!Validation.GetHasError(kcalTarget))
                {

                    Row r = (Row)dataGrid1.SelectedItem;
                    String menuPresetId = (String)r["dc_presetsid"];

                    App ap = (App)App.Current;
                    XElement contactXml = new XElement("contact");

                    contactXml.Add(new XElement("contactid", ap.contactId));

                    //kcal target

                    if (SpecifyCaloricTarget.IsChecked.Value)
                    {
                        if (kcalTarget.Text != null)
                        {//TODO:  apply mask
                            contactXml.Add(new XElement("dc_kcaltarget", kcalTarget.Text));
                        }
                    }
                    else
                    {
                        contactXml.Add(new XElement("dc_kcaltarget", dailyTarget.Text));
                    }

                    //contactXml.Add(new XElement("dc_kcaltarget", dailyTarget.Text));

                    XElement element = new XElement("dc_menupresetid", menuPresetId);

                    contactXml.Add(new XElement("dc_userspecifiedkcaltarget", SpecifyCaloricTarget.IsChecked.Value));

                    XAttribute attribute = new XAttribute("entityname", "dc_presets");
                    element.Add(attribute);

                    contactXml.Add(element);
                    //Snacks
                    //maintain weight

                    contactXml.Add(new XElement("dc_morningsnack", morningSnack.IsChecked.Value.ToString()));
                    contactXml.Add(new XElement("dc_afternoonsnack", afternoonSnack.IsChecked.Value.ToString()));
                    contactXml.Add(new XElement("dc_eveningsnack", eveningSnack.IsChecked.Value.ToString()));

                    CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
                    cms.CreateUpdateReturnEntityCompleted += new EventHandler<CrmSdk.CreateUpdateReturnEntityCompletedEventArgs>(cms_CreateUpdateContact);
                    cms.CreateUpdateReturnEntityAsync(contactXml, true);
                }
                else
                {
                    Status s = new Status("Please fill out required fields", false);
                    s.Show();
                }
            }
        }
        private void cms_CreateUpdateContact(object sender, CrmSdk.CreateUpdateReturnEntityCompletedEventArgs e)
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
                var contacts = from x in e.Result.Descendants("contact") select x;
                XElement contactNode = e.Result.Descendants("contact").First();
                Contact c = Contact.BuildContact(contactNode);
                ((App)App.Current).contact = c;
                if (generateMenu)
                {

                    GenerateMenuAfterSave();
                    //NavigationService.Navigate(new Uri("DailyMenu", UriKind.Relative));
                }
                else
                {
                    NavigationService.Navigate(new Uri("FoodLikes", UriKind.Relative));
                }
            }
        }
        private void customKcalTarget_Click(object sender, RoutedEventArgs e)
        {

            if (SpecifyCaloricTarget.IsChecked.Value)
            {
                kcalTargetStackPanel.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                kcalTargetStackPanel.Visibility = System.Windows.Visibility.Collapsed;

                if (Validation.GetHasError(kcalTarget) && ValidationSummary.Errors.Count > 0)
                {
                    int x = 0;
                    while (true)
                    {

                        if (ValidationSummary.Errors[x].MessageHeader.Equals("KcalTarget", StringComparison.OrdinalIgnoreCase))
                        {
                            ValidationSummary.Errors.Remove(ValidationSummary.Errors[x]);
                            //targetWeight.Text = String.Empty;
                            kcalTarget.Text = decimal.MinValue.ToString();
                            kcalTarget.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                            kcalTarget.Text = String.Empty;
                            break;
                        }
                        x++;
                        if (x > ValidationSummary.Errors.Count())
                        {
                            break;
                        }
                    }
                }

            }
        }
        /// <summary>
        /// Open generic popup
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Label_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ChildWindows.GenericPopup gp = new GenericPopup("Determining Calorie Targets");
            gp.Show();
        }
        private void YourProfile_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ChildWindows.GenericPopup gp = new GenericPopup("your profile information");
            gp.Show();
        }
        /// <summary>
        /// Fired by button click.  Hads off to save and then generates the menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GenerateMenu_Click(object sender, RoutedEventArgs e)
        {
            Button_Click(null, null);
            generateMenu = true;
        }
        private void GenerateMenuAfterSave()
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


        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (dataGrid1 != null)//make sure the form is loaded
            {
                RadioButton rb = (RadioButton)sender;
                presetFilter = (String)rb.Tag;
                //RetrieveMenuOptions();
                //Clear out description
                menuDescription.Text = String.Empty;
                SortableCollectionView filteredView = new SortableCollectionView();
                //filter now
                if (presetFilter.Equals("none", StringComparison.OrdinalIgnoreCase))
                {
                    //filteredView = data;
                    foreach (Row r in data)
                    {
                        if (r["dc_parent"] != null && Boolean.Parse((String)r["dc_parent"]))
                        {
                            filteredView.Add(r);
                        }
                    }
                }
                else
                {
                    foreach (Row r in data)
                    {
                        if (r[presetFilter] != null && Boolean.Parse((String)r[presetFilter]))
                        {
                            filteredView.Add(r);
                        }
                    }
                }
                dataGrid1.ItemsSource = filteredView;
                if (filteredView.Count > 0)
                {
                    //select the first row
                    dataGrid1.SelectedIndex = 0;
                    dataGrid1_MouseLeftButtonDown(null, null);
                }
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //RetrieveContactProfile();
            RetrieveMenuOptions();
        }
    }
}
