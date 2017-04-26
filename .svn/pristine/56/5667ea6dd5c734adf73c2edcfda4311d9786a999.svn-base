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
using System.Windows.Controls.DataVisualization.Charting;
using System.Globalization;
using System.Xml.Linq;
using DynamicConnections.NutriStyle.MenuGenerator.Engine.Helpers;
using System.Windows.Controls.DataVisualization.Charting.Primitives;
using System.Windows.Controls.DataVisualization;

namespace DynamicConnections.NutriStyle.MenuGenerator.Controls
{

    public partial class MacroChart : UserControl
    {
        
        private decimal calories = 0;
        private decimal protein = 0;
        private decimal fat = 0;
        private decimal carbohydrate = 0;

        private decimal proteinPct = 0m;
        private decimal fatPct = 0m;
        private decimal carbohydratePct = 0m;
        
        private RadialGradientBrush alcoholRGB;

        private Guid contactId;

        App app;
        public MacroChart()
        {
            InitializeComponent();

            if (App.Current.GetType() == typeof(DynamicConnections.NutriStyle.MenuGenerator.App))
            {
                app = (App)App.Current;
            }
            ChartData = new List<BarChartData>();
            ChartData.Add(new BarChartData());
            ChartData.Add(new BarChartData());
            ChartData.Add(new BarChartData());
        }
        public MacroChart(Guid contactId) 
        {
            InitializeComponent();

            if (App.Current.GetType() == typeof(DynamicConnections.NutriStyle.MenuGenerator.App))
            {
                app = (App)App.Current;
            }
            ChartData = new List<BarChartData>();
            ChartData.Add(new BarChartData());
            ChartData.Add(new BarChartData());
            ChartData.Add(new BarChartData());

            this.contactId = contactId;
        }
        public decimal ProteinPct {
            get { return (proteinPct); }
            set
            {
                proteinPct = value;
            }
        }
        public decimal FatPct {
            get { return (fatPct); }
            set
            {
                fatPct = value;
            }
        }
        public decimal CarbohydratePct { 
            get { return (carbohydratePct); }
            set
            {
                carbohydratePct = value;
            }
        }

        public bool IsBusy
        {
            get
            {
                return (busyIndicator.IsBusy);
            }
            set
            {
                if (busyIndicator != null)
                {
                    busyIndicator.IsBusy = value;
                }
            }
        }
        public decimal Calories
        {
            get { return (calories); }
            set
            {
                calories = value;
                kcals.DataContext = new BarChartData() { Name = "Kcals: " + Math.Round(calories, 0).ToString("#,##") }; 
            }
        }
        public decimal Fat
        {
            get { return (fat); }

            set
            {
                fat = value;
            }
        }
        public decimal Protein
        {
            get { return (protein); }
            set
            {
                protein = value;
            }
        }
        public decimal Carbohydrate {
            get { return (carbohydrate); }
            set
            {
                carbohydrate = value;
            }
        }
        public List<BarChartData> ChartData { get; set; }
        public void SetChartData(List<BarChartData> list)
        {
            ChartData = list;
            ((PieSeries)pieChart.Series[0]).ItemsSource = list;
            busyIndicator.IsBusy = false;
        }


        public void PopulateChartFoodLog()
        {
            DateTime date = DateTime.Now;

            String fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' aggregate='true'>
                  <entity name='dc_foodlog'>
                    <attribute name='dc_date' groupby='true' dategrouping='day' alias='dc_date'/>

                    <attribute alias='dc_fat' name='dc_fat' aggregate='sum'/>
                    <attribute alias='dc_protein' name='dc_protein' aggregate='sum'/>
                    <attribute alias='dc_carbohydrate' name='dc_carbohydrate' aggregate='sum' />

                    <filter type='and'>
                      <condition attribute='statecode' operator='eq' value='0' />
                    </filter>
                    <link-entity name='dc_foodlogday' from='dc_foodlogdayid' to='dc_foodlogdayid' alias='aa'>
                      <filter type='and'>
                        <condition attribute='dc_menuid' operator='eq' value='@MENUID' />
                        <condition attribute='dc_contactid' operator='eq' value='@CONTACTID' />
                        <condition attribute='dc_date' operator='on' value='@DATETIME' />
                      </filter>
                    </link-entity>


                  </entity>
                </fetch>";

            fetchXml = fetchXml.Replace("@CONTACTID", ((App)App.Current).contact.ContactId.ToString());
            fetchXml = fetchXml.Replace("@DATETIME", date.ToString("MM/dd/yyyy"));
            fetchXml = fetchXml.Replace("@MENUID", ((App)App.Current).contact.MenuId.ToString());

            XElement orderXml = new XElement("ColumnOrder");

            orderXml.Add(new XElement("Column", "dc_fat"));
            orderXml.Add(new XElement("Column", "dc_protein"));
            orderXml.Add(new XElement("Column", "dc_carbohydrate"));


            CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
            cms.RetrieveFetchXmlCompleted += new EventHandler<CrmSdk.RetrieveFetchXmlCompletedEventArgs>(cms_RetrieveFoodLog);
            cms.RetrieveFetchXmlAsync(fetchXml, orderXml.ToString());

        }
        private void cms_RetrieveFoodLog(object sender, CrmSdk.RetrieveFetchXmlCompletedEventArgs e)
        {
            String entityName = "dc_foodlog";
            var element = e.Result;
            var dtf = CultureInfo.CurrentCulture.DateTimeFormat;

            //List<BarChartData> listMarco = app.listMacro;
            try
            {
                if (element.Descendants(entityName).Count() > 0)
                {
                    //Bind data
                    var rows = element.Descendants(entityName);

                    foreach (var row in rows)
                    {
                        Row rowData = new Row();
                        foreach (XElement xe in row.Elements())
                        {
                            if (xe.Name.LocalName.Equals("dc_fat", StringComparison.OrdinalIgnoreCase))
                            {
                                fat = Convert.ToDecimal(xe.Value);
                            }
                            else if (xe.Name.LocalName.Equals("dc_protein", StringComparison.OrdinalIgnoreCase))
                            {
                                protein = Convert.ToDecimal(xe.Value);
                            }
                            else if (xe.Name.LocalName.Equals("dc_carbohydrate", StringComparison.OrdinalIgnoreCase))
                            {
                                carbohydrate = Convert.ToDecimal(xe.Value);
                            }
                        }
                    }

                    if (protein > 0 && (protein + fat + carbohydrate) > 0)
                    {
                        proteinPct = protein / (protein + fat + carbohydrate);
                    }
                    if (fat > 0 && (protein + fat + carbohydrate) > 0)
                    {
                        fatPct = fat / (protein + fat + carbohydrate);
                    }
                    if (carbohydrate > 0 && (protein + fat + carbohydrate) > 0)
                    {
                        carbohydratePct = carbohydrate / (protein + fat + carbohydrate);
                    }

                    proteinPct = Math.Round(proteinPct, 2);
                    fatPct = Math.Round(fatPct, 2);
                    carbohydratePct = Math.Round(carbohydratePct, 2);

                    calories = (fat * Convert.ToDecimal(App.GramToKcalMultipler.Fat) + protein * Convert.ToDecimal(App.GramToKcalMultipler.Protein) + carbohydrate * Convert.ToDecimal(App.GramToKcalMultipler.Carbohydrate));

                    ChartData[0] = new BarChartData("Protein: " + " - " + string.Format("{0:0%}", proteinPct), protein);
                    ChartData[1] = new BarChartData("Fat: " + " - " + string.Format("{0:0%}", fatPct), fat);
                    ChartData[2] = new BarChartData("Carbs: " + " - " + string.Format("{0:0%}", carbohydratePct), carbohydrate);

                    //app.listKcal[0].Name = //"Kcals: " + Math.Round(kcalTotal, 0);

                    //kcals.Content = "Kcals: " + Math.Round(kcalTotal, 2).ToString();
                    kcals.DataContext = new BarChartData() { Name = "Kcals: " + Math.Round(calories, 0) }; 

                    //((PieSeries)pieChart.Series[0]).ItemsSource = app.listMacro;
                    ((PieSeries)pieChart.Series[0]).ItemsSource = ChartData;
                    
                }
                else
                {
                    ChartData[0] = new BarChartData("Protein: " + " - " + string.Format("{0:0%}", proteinPct), 0m);
                    ChartData[1] = new BarChartData("Fat: " + " - " + string.Format("{0:0%}", fatPct), 0m);
                    ChartData[2] = new BarChartData("Carbs: " + " - " + string.Format("{0:0%}", carbohydratePct), 0m);

                    //app.listKcal[0].Name = "Kcals: " + Math.Round(0m, 0);

                    ((PieSeries)pieChart.Series[0]).ItemsSource = ChartData;
                   
                    
                }
                //pieChart_Loaded(null, null);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
            }
            busyIndicator.IsBusy = false;
        }
        public void PopulateChartDailyMenu(Guid menuId)
        {

            DateTime date = DateTime.Now;
            Dictionary<String, int> dayList = new Dictionary<string, int>();
            dayList = ((App)App.Current).dayList;

            var dtf = CultureInfo.CurrentCulture.DateTimeFormat;
            DateTime current = DateTime.Now;
            String day = current.DayOfWeek.ToString();

            String fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                      <entity name='dc_mealfood'>
                      
                        <attribute name='dc_fat' />
                        <attribute name='dc_protein' />
                        <attribute name='dc_carbohydrate' />
                       
                        <link-entity name='dc_meal' from='dc_mealid' to='dc_mealid' alias='dc_meal'>
                          <attribute name='dc_meal'/>
                          <link-entity name='dc_day' from='dc_dayid' to='dc_dayid' alias='dc_day'>
                            <attribute name='dc_day'/>  
                                <filter type='and'>
                                    <condition attribute='dc_day' operator='eq' value='@DAY' />
                                </filter>
                            <link-entity name='dc_menu' from='dc_menuid' to='dc_menuid' alias='dc_menu'>
                            <attribute name='dc_menuid'/>  
                              <filter type='and'>
                                <condition attribute='dc_contactid' operator='eq' value='@CONTACTID' />
                                <condition attribute='statecode' operator='eq' value='0' />
                                <condition attribute='dc_menuid' operator='eq' value='@MENUID' />
                              </filter>
                            </link-entity>
                          </link-entity>
                        </link-entity>
                      </entity>
                    </fetch>";

            fetchXml = fetchXml.Replace("@CONTACTID", app.contactId);
            fetchXml = fetchXml.Replace("@MENUID", menuId.ToString());
            if (dayList.Count > 0 && dayList.ContainsKey(day))
            {
                fetchXml = fetchXml.Replace("@DAY", dayList[day].ToString());
            }
            XElement orderXml = new XElement("ColumnOrder");
            orderXml.Add(new XElement("Column", "dc_foodid"));
            orderXml.Add(new XElement("Column", "dc_portionsize"));

            orderXml.Add(new XElement("Column", "dc_portiontypeid"));

            orderXml.Add(new XElement("Column", "dc_fat"));
            orderXml.Add(new XElement("Column", "dc_protein"));
            orderXml.Add(new XElement("Column", "dc_carbohydrate"));
            orderXml.Add(new XElement("Column", "dc_kcals"));
            orderXml.Add(new XElement("Column", "dc_mealid"));

            orderXml.Add(new XElement("Column", "dc_mealfoodid"));

            orderXml.Add(new XElement("Column", "dc_meal.dc_meal"));
            orderXml.Add(new XElement("Column", "dc_day.dc_day"));
            orderXml.Add(new XElement("Column", "dc_menu.dc_menuid"));


            CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
            cms.RetrieveFetchXmlCompleted += new EventHandler<CrmSdk.RetrieveFetchXmlCompletedEventArgs>(cms_RetrieveDailyMenu);
            cms.RetrieveFetchXmlAsync(fetchXml, orderXml.ToString());

        }


        public void PopulateChartDailyMenu()
        {

            DateTime date = DateTime.Now;
            Dictionary<String, int> dayList = new Dictionary<string, int>();
            dayList = ((App)App.Current).dayList;

            var dtf = CultureInfo.CurrentCulture.DateTimeFormat;
            DateTime current = DateTime.Now;
            String day = current.DayOfWeek.ToString();

            String fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                      <entity name='dc_mealfood'>
                      
                        <attribute name='dc_fat' />
                        <attribute name='dc_protein' />
                        <attribute name='dc_carbohydrate' />
                       
                        <link-entity name='dc_meal' from='dc_mealid' to='dc_mealid' alias='dc_meal'>
                          <attribute name='dc_meal'/>
                          <link-entity name='dc_day' from='dc_dayid' to='dc_dayid' alias='dc_day'>
                            <attribute name='dc_day'/>  
                                <filter type='and'>
                                    <condition attribute='dc_day' operator='eq' value='@DAY' />
                                </filter>
                            <link-entity name='dc_menu' from='dc_menuid' to='dc_menuid' alias='dc_menu'>
                            <attribute name='dc_menuid'/>  
                              <filter type='and'>
                                <condition attribute='dc_contactid' operator='eq' value='@CONTACTID' />
                                <condition attribute='statecode' operator='eq' value='0' />
                                <condition attribute='dc_primarymenu' operator='eq' value='1' />
                              </filter>
                            </link-entity>
                          </link-entity>
                        </link-entity>
                      </entity>
                    </fetch>";

            fetchXml = fetchXml.Replace("@CONTACTID", app.contactId);
            if (dayList.Count > 0 && dayList.ContainsKey(day))
            {
                fetchXml = fetchXml.Replace("@DAY", dayList[day].ToString());
            }
            XElement orderXml = new XElement("ColumnOrder");
            orderXml.Add(new XElement("Column", "dc_foodid"));
            orderXml.Add(new XElement("Column", "dc_portionsize"));

            orderXml.Add(new XElement("Column", "dc_portiontypeid"));

            orderXml.Add(new XElement("Column", "dc_fat"));
            orderXml.Add(new XElement("Column", "dc_protein"));
            orderXml.Add(new XElement("Column", "dc_carbohydrate"));
            orderXml.Add(new XElement("Column", "dc_kcals"));
            orderXml.Add(new XElement("Column", "dc_mealid"));

            orderXml.Add(new XElement("Column", "dc_mealfoodid"));

            orderXml.Add(new XElement("Column", "dc_meal.dc_meal"));
            orderXml.Add(new XElement("Column", "dc_day.dc_day"));
            orderXml.Add(new XElement("Column", "dc_menu.dc_menuid"));


            CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
            cms.RetrieveFetchXmlCompleted += new EventHandler<CrmSdk.RetrieveFetchXmlCompletedEventArgs>(cms_RetrieveDailyMenu);
            cms.RetrieveFetchXmlAsync(fetchXml, orderXml.ToString());

        }
        private void cms_RetrieveDailyMenu(object sender, CrmSdk.RetrieveFetchXmlCompletedEventArgs e)
        {
            String entityName = "dc_mealfood";
            var element = e.Result;
            var dtf = CultureInfo.CurrentCulture.DateTimeFormat;

            //List<BarChartData> listMarco = app.listMacro;
            try
            {
                if (element.Descendants(entityName).Count() > 0)
                {
                    //Bind data
                    var rows = element.Descendants(entityName);
                    fat = 0;
                    protein = 0;
                    carbohydrate = 0;
                    foreach (var row in rows)
                    {
                        Row rowData = new Row();
                        foreach (XElement xe in row.Elements())
                        {
                            if (xe.Name.LocalName.Equals("dc_fat", StringComparison.OrdinalIgnoreCase))
                            {
                                fat += Convert.ToDecimal(xe.Value);
                            }
                            else if (xe.Name.LocalName.Equals("dc_protein", StringComparison.OrdinalIgnoreCase))
                            {
                                protein += Convert.ToDecimal(xe.Value);
                            }
                            else if (xe.Name.LocalName.Equals("dc_carbohydrate", StringComparison.OrdinalIgnoreCase))
                            {
                                carbohydrate += Convert.ToDecimal(xe.Value);
                            }
                        }
                    }

                    if (protein > 0 && (protein + fat + carbohydrate) > 0)
                    {
                        proteinPct = protein / (protein + fat + carbohydrate);
                    }
                    if (fat > 0 && (protein + fat + carbohydrate) > 0)
                    {
                        fatPct = fat / (protein + fat + carbohydrate);
                    }
                    if (carbohydrate > 0 && (protein + fat + carbohydrate) > 0)
                    {
                        carbohydratePct = carbohydrate / (protein + fat + carbohydrate);
                    }

                    proteinPct = Math.Round(proteinPct, 2);
                    fatPct = Math.Round(fatPct, 2);
                    carbohydratePct = Math.Round(carbohydratePct, 2);

                    calories = (fat * Convert.ToDecimal(App.GramToKcalMultipler.Fat) + protein * Convert.ToDecimal(App.GramToKcalMultipler.Protein) + carbohydrate * Convert.ToDecimal(App.GramToKcalMultipler.Carbohydrate));

                    ChartData[0] = new BarChartData("Protein: " + " - " + string.Format("{0:0%}", proteinPct), protein);
                    ChartData[1] = new BarChartData("Fat: " + " - " + string.Format("{0:0%}", fatPct), fat);
                    ChartData[2] = new BarChartData("Carbs: " + " - " + string.Format("{0:0%}", carbohydratePct), carbohydrate);

                    //kcals.Content = "Kcals: " + Math.Round(kcalTotal, 2).ToString();
                    kcals.DataContext = new BarChartData() { Name = "Kcals: " + Math.Round(calories, 0) };

                    ((PieSeries)pieChart.Series[0]).ItemsSource = ChartData;
                    busyIndicator.IsBusy = false;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
            }
        }
        private void PopulateChart()
        {
            
            foreach (BarChartData bcd in ChartData)
            {
                if (bcd.Name.StartsWith("Protein", StringComparison.OrdinalIgnoreCase))
                {
                    protein = bcd.Value;
                }
                if (bcd.Name.StartsWith("fat", StringComparison.OrdinalIgnoreCase))
                {
                    fat = bcd.Value;
                }
                else if (bcd.Name.StartsWith("Carbs", StringComparison.OrdinalIgnoreCase))
                {
                    carbohydrate = bcd.Value;
                }
            }

            if (protein > 0 && (protein + fat + carbohydrate) > 0)
            {
                proteinPct = protein / (protein + fat + carbohydrate);
            }
            if (fat > 0 && (protein + fat + carbohydrate) > 0)
            {
                fatPct = fat / (protein + fat + carbohydrate);
            }
            if (carbohydrate > 0 && (protein + fat + carbohydrate) > 0)
            {
                carbohydratePct = carbohydrate / (protein + fat + carbohydrate);
            }
            proteinPct = Math.Round(proteinPct, 2);
            fatPct = Math.Round(fatPct, 2);
            carbohydratePct = Math.Round(carbohydratePct, 2);

            foreach (BarChartData bcd in ChartData)
            {
                if (bcd.Name.StartsWith("Protein", StringComparison.OrdinalIgnoreCase))
                {
                    bcd.Name = "Protein: " + " - " + string.Format("{0:0.0%}", proteinPct);
                }
                if (bcd.Name.StartsWith("fat", StringComparison.OrdinalIgnoreCase))
                {
                    bcd.Name = "Fat: " + " - " + string.Format("{0:0.0%}", fatPct);
                }
                else if (bcd.Name.StartsWith("Carbs", StringComparison.OrdinalIgnoreCase))
                {
                    bcd.Name = "Carbs: " + " - " + string.Format("{0:0.0%}", carbohydratePct);
                }
            }
            calories = protein + fat + carbohydrate;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void pieChart_Loaded(object sender, RoutedEventArgs e)
        {
            Rect pieBounds = new Rect();
            var center = new Point();
            var radius = 0d;
            double diameter = 0d;
            ResourceDictionaryCollection palette = new ResourceDictionaryCollection();
            try
            {
                EdgePanel plotArea = (EdgePanel)((PieSeries)pieChart.Series[0]).Parent;

                //plotArea.Width = plotArea.ActualHeight;//makes it a sqaure
                Style style = new Style(typeof(LegendItem));//((PieSeries)chart.Series[0]).LegendItemStyle;

                //style.Setters.Add(new Setter(WidthProperty, (544) - plotArea.Width - 95d));

                //((PieSeries)pieChart.Series[0]).LegendItemStyle = style;

                if (null != plotArea)
                {
                    //MessageBox.Show("plotArea.ActualWidth: " + plotArea.ActualWidth); //295
                    //MessageBox.Show("plotArea.ActualHeight: " + plotArea.ActualHeight); //245

                    // Calculate the diameter of the pie (0.95 multiplier is from PieSeries implementation)
                    //diameter = Math.Min(plotArea.ActualWidth, plotArea.ActualHeight) * 0.75;

                    diameter = Math.Min(295, 245) * 0.75;
                    // Calculate the bounding rectangle of the pie

                    //var leftTop = new Point((plotArea.ActualWidth - diameter) / 2, (plotArea.ActualHeight - diameter) / 2);
                    var leftTop = new Point((295 - diameter) / 2, (245 - diameter) / 2);

                    var rightBottom = new Point(leftTop.X + diameter, leftTop.Y + diameter);
                    pieBounds = new Rect(leftTop, rightBottom);
                    //MessageBox.Show("diameter: " + diameter);
                    // Call the provided updater action for each PieDataPoint
                }

                double x = pieBounds.Left + ((pieBounds.Right - pieBounds.Left) / 2);
                double y = pieBounds.Top + ((pieBounds.Bottom - (pieBounds.Top)) / 2);

                x += 4 - 20;
                y -= 30;

                center = new Point(x, y);

                radius = (pieBounds.Right - pieBounds.Left) / 2;

                kcals.Margin = new Thickness(x - 25, y - 30, 0, 0);
                kcals.Visibility = System.Windows.Visibility.Visible;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
            }
            if (diameter > 150)
            {
                //set colors
                for (int x = 0; x < 4; x++)
                {
                    GradientStopCollection gsc = new GradientStopCollection();
                    Style style2 = new Style(typeof(Control));
                    GradientStop gs = new GradientStop();
                    GradientStop gs2 = new GradientStop();
                    GradientStop gs3 = new GradientStop();
                    if (x == 0) //Protien
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
                    else if (x == 1) //Fat
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
                    else if (x == 2) //Carbs
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
                    else if (x == 3) //Alcohol
                    {//yellow
                        gs.Color = Color.FromArgb(255, 255, 202, 0);
                        gs.Offset = 0.0;

                        gs2 = new GradientStop();
                        gs2.Color = Color.FromArgb(255, 255, 150, 0);
                        gs2.Offset = 0.8;

                        gs3 = new GradientStop();
                        gs3.Color = Color.FromArgb(255, 255, 100, 0);
                        gs3.Offset = 1.0;
                    }
                    if (x < 3)
                    {
                        gsc.Add(gs);
                        gsc.Add(gs2);
                        gsc.Add(gs3);

                        RadialGradientBrush rgb = new RadialGradientBrush(gsc);
                        rgb.MappingMode = BrushMappingMode.Absolute;
                        rgb.SpreadMethod = GradientSpreadMethod.Reflect;

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
                    else
                    {
                        gsc.Add(gs);
                        gsc.Add(gs2);
                        gsc.Add(gs3);

                        alcoholRGB = new RadialGradientBrush(gsc);
                        alcoholRGB.MappingMode = BrushMappingMode.Absolute;
                        alcoholRGB.SpreadMethod = GradientSpreadMethod.Reflect;

                        alcoholRGB.Center = center;
                        alcoholRGB.GradientOrigin = center;
                        alcoholRGB.RadiusX = radius;
                        alcoholRGB.RadiusY = radius;
                    }
                }
                pieChart.Palette = palette;
                //busyIndicator.IsBusy = false;
            }
            else
            {
                //use dispatcher
                Dispatcher.BeginInvoke(() => Pause());
            }
        }
        private void Pause()
        {
            System.Threading.Thread.Sleep(500);
            pieChart_Loaded(null, null);
        }
        public void AddAlcoholPalette()
        {
            Style style2 = new Style(typeof(Control));
            var palette = pieChart.Palette;
            if (palette.Count < 4)
            {
                style2.Setters.Add(new Setter(BackgroundProperty, alcoholRGB));
                style2.Setters.Add(new Setter(TemplateProperty, this.Resources["pi"]));

                var dictionary = new ResourceDictionary();
                dictionary.Add("DataPointStyle", style2);
                palette.Insert(3, dictionary);
            }
        }
        public void RemoveAlcoholPalette()
        {
            Style style2 = new Style(typeof(Control));
            var palette = pieChart.Palette;
            if (palette.Count == 4)
            {
                //remove last one.
                palette.RemoveAt(3);
            }

        }
    }
}
