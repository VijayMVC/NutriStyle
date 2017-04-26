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
using System.Globalization;
using System.Text.RegularExpressions;
using DynamicConnections.NutriStyle.MenuGenerator.ChildWindows;
using DynamicConnections.NutriStyle.MenuGenerator.Pages;
using System.Windows.Controls.Primitives;

namespace DynamicConnections.NutriStyle.MenuGenerator.Controls
{
    public partial class AddARecipe : UserControl
    {
        Engine.FormData.Recipe recipe = new Engine.FormData.Recipe();
        List<String> requiredFields;
        List<String> watermarkFields;
        private RowIndexConverter _rowIndexConverter = new RowIndexConverter();
        Controls.ComboBoxWithValidation comboBox;
        Dictionary<Guid, Food> foods;

        public event EventHandler SavedClicked;


        public AddARecipe()
        {
            InitializeComponent();
            recipe = new Engine.FormData.Recipe();
            foods = new Dictionary<Guid, Food>();
            recipe.AvailableToAllUsers = Engine.FormData.Recipe.AvailableToAllUsersOptions.False;
            
            requiredFields = new List<string>();
            requiredFields.Add("dc_name");
            requiredFields.Add("dc_sourceofinformationdateobtained");
            requiredFields.Add("dc_preparationtime");
            requiredFields.Add("dc_portion_amount");
            requiredFields.Add("dc_portiontypeid");
            requiredFields.Add("dc_numberofservings");
            requiredFields.Add("dc_directions");
            //requiredFields.Add("dc_unit_gram_weight");

            watermarkFields = new List<string>();
            watermarkFields.Add("dc_name");
            watermarkFields.Add("dc_sourceofinformationdateobtained");
            watermarkFields.Add("dc_preparationtime");
            watermarkFields.Add("dc_portion_amount");
            watermarkFields.Add("dc_portiontypeid");
            watermarkFields.Add("dc_numberofservings");
            watermarkFields.Add("dc_directions");
            //watermarkFields.Add("dc_unit_gram_weight");

            recipe.IngredientData = new SortableCollectionView();

            //Add one row to grid;

            recipe.IngredientData.ColumnTypes.Add("dc_foodingredientid", "lookup");
            recipe.IngredientData.ColumnTypes.Add("dc_portiontypeid", "lookup");
            recipe.IngredientData.ColumnTypes.Add("dc_portionsize", "decimal");

            recipe.IngredientData.EntityTypes.Add("dc_foodingredientid", "dc_foods");
            recipe.IngredientData.EntityTypes.Add("dc_portiontypeid", "dc_portion_types");
            //setup event handlers
            DataGrid.LoadingRow += new EventHandler<DataGridRowEventArgs>(dataGrid_LoadingRow);

        }
        void dataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            DataGrid dataGrid1 = (DataGrid)sender;
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
                            //ToolTipService.SetToolTip(ccb, "Remove From Menu");

                            ccb.Click -= ccb_MouseLeftButtonUp;
                            ccb.Click += new RoutedEventHandler(ccb_MouseLeftButtonUp);
                        }
                        else if (c.GetCellContent(e.Row).GetType() == typeof(Controls.CustomTextBox))
                        {
                            CustomTextBox ctb = c.GetCellContent(e.Row) as Controls.CustomTextBox;
                            if (ctb.TagName.Equals("dc_portionsize", StringComparison.OrdinalIgnoreCase))
                            {

                            }
                        }
                        else if (c.GetCellContent(e.Row).GetType() == typeof(Controls.ComboBoxWithValidation))
                        {
                            Controls.ComboBoxWithValidation ccb = c.GetCellContent(e.Row) as Controls.ComboBoxWithValidation;

                            if (ccb.TagName.Equals("dc_foodingredientid", StringComparison.OrdinalIgnoreCase))
                            {
                                ccb.KeyDown -= food_KeyDown;
                                ccb.KeyDown += new KeyEventHandler(food_KeyDown);
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

        public void RetrieveFood(Guid foodId)
        {
            String fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
              <entity name='dc_foods'>
                <attribute name='dc_name' />
                <attribute name='dc_sourceofinformationdateobtained' />
                <attribute name='dc_foodmanufacturerweb' />
                <attribute name='dc_portion_amount' />
                <attribute name='dc_portiontypeid' />
                <attribute name='dc_availabletoallusers' />
                <attribute name='dc_preparationtime' />
                <attribute name='dc_numberofservings' />
                <attribute name='dc_directions' />
                <attribute name='dc_foodsid' />
                <order attribute='dc_name' descending='false' />
                <filter type='and'>
                    <condition attribute='dc_foodsid' value='@FOODID' operator='eq'/>
                </filter>
                <link-entity name='dc_food_nutrients' from='dc_food_nutrientsid' to='dc_foodnutrientid' alias='dc_food_nutrients'>
                     <attribute name='dc_food_nutrientsid' />
                </link-entity>

              </entity>
            </fetch>";

            fetchXml = fetchXml.Replace("@FOODID", foodId.ToString());

            XElement element = XElement.Parse(fetchXml);

            //IEnumerable<XElement> nodes = element.Descendants().First().Value.Where(e => e.Name.LocalName == "attribute");

            XElement orderXml = new XElement("ColumnOrder");

            foreach (XElement xe in element.Descendants("attribute"))
            {
                if (xe.Parent.Attributes("alias").Count() == 0)
                {
                    orderXml.Add(new XElement("Column", xe.Attributes("name").First().Value));
                }
            }

            try
            {
                busyIndicator.IsBusy = true;
                busyIndicator.Visibility = System.Windows.Visibility.Visible;

                CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
                cms.RetrieveFetchXmlCompleted += new EventHandler<CrmSdk.RetrieveFetchXmlCompletedEventArgs>(cms_RetrieveFood);
                cms.RetrieveFetchXmlAsync(fetchXml, orderXml.ToString());
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
            }
            #region Fetch the ingredients
            fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
              <entity name='dc_ingredient'>
                <attribute name='dc_name' />
                <attribute name='dc_ingredientid' />
                <attribute name='dc_foodingredientid' />
                <attribute name='dc_portionsize' />
                <attribute name='dc_portiontypeid' />
                <order attribute='dc_name' descending='false' />
                <filter type='and'>
                  <condition attribute='dc_foodid' operator='eq' value='@FOODID' />
                </filter>
              </entity>
            </fetch>";

            fetchXml = fetchXml.Replace("@FOODID", foodId.ToString());

            element = XElement.Parse(fetchXml);

            orderXml = new XElement("ColumnOrder");

            foreach (XElement xe in element.Descendants("attribute"))
            {
                if (xe.Parent.Attributes("alias").Count() == 0)
                {
                    orderXml.Add(new XElement("Column", xe.Attributes("name").First().Value));
                }
            }
            try
            {
                busyIndicator.IsBusy = true;
                busyIndicator.Visibility = System.Windows.Visibility.Visible;

                CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
                cms.RetrieveFetchXmlCompleted += new EventHandler<CrmSdk.RetrieveFetchXmlCompletedEventArgs>(cms_RetrieveIngredients);
                cms.RetrieveFetchXmlAsync(fetchXml, orderXml.ToString());
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
            }
            #endregion
        }

        private void cms_RetrieveFood(object sender, CrmSdk.RetrieveFetchXmlCompletedEventArgs e)
        {
            var element = e.Result;
            var rows = element.Descendants("dc_foods");

            foreach (var row in rows)
            {
                foreach (XElement xe in row.Elements())
                {
                    #region dc_foods

                    if (xe.Name.LocalName.Equals("dc_name", StringComparison.OrdinalIgnoreCase))
                    {
                        recipe.Name = xe.Value;
                    }
                    else if (xe.Name.LocalName.Equals("dc_sourceofinformationdateobtained", StringComparison.OrdinalIgnoreCase))
                    {
                        recipe.SourceOfInformation = xe.Value;
                    }
                    else if (xe.Name.LocalName.Equals("dc_directions", StringComparison.OrdinalIgnoreCase))
                    {
                        recipe.Directions = xe.Value;
                    }
                    else if (xe.Name.LocalName.Equals("dc_preparationtime", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!String.IsNullOrEmpty(xe.Value))
                        {
                            recipe.PreparationTime = Convert.ToInt32(xe.Value);
                        }
                    }
                    else if (xe.Name.LocalName.Equals("dc_numberofservings", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!String.IsNullOrEmpty(xe.Value))
                        {
                            recipe.NumberOfServings = Convert.ToDecimal(xe.Value);
                        }
                    }

                    else if (xe.Name.LocalName.Equals("dc_portion_amount", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!String.IsNullOrEmpty(xe.Value))
                        {
                            recipe.PortionAmount = Convert.ToDecimal(xe.Value);
                        }
                    }
                    else if (xe.Name.LocalName.Equals("dc_portiontypeid", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!String.IsNullOrEmpty(xe.Value))
                        {
                            recipe.PortionTypeId = new PairWithList(xe.Value, xe.Attributes("Id").First().Value, String.Empty, new List<PairWithList>());
                        }
                    }

                    else if (xe.Name.LocalName.Equals("dc_availabletoallusers", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!String.IsNullOrEmpty(xe.Value))
                        {
                            if (Boolean.Parse(xe.Value))
                            {
                                recipe.AvailableToAllUsers = Engine.FormData.Recipe.AvailableToAllUsersOptions.True;
                            }
                            else
                            {
                                recipe.AvailableToAllUsers = Engine.FormData.Recipe.AvailableToAllUsersOptions.False;
                            }
                        }
                    }

                    else if (xe.Name.LocalName.Equals("dc_foodsid", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!String.IsNullOrEmpty(xe.Value))
                        {
                            recipe.FoodId = new Guid(xe.Value);
                        }
                    }
                    #endregion

                    #region dc_food_nutrients

                    else if (xe.Name.LocalName.Equals("dc_food_nutrients.dc_food_nutrientsid", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!String.IsNullOrEmpty(xe.Value))
                        {
                            recipe.FoodNutrientId = new Guid(xe.Value);
                        }
                    }
                    #endregion
                }
            }
            recipe.RaisePropertyChanged();
            busyIndicator.IsBusy = false;
        }

        private void cms_RetrieveIngredients(object sender, CrmSdk.RetrieveFetchXmlCompletedEventArgs e)
        {
            var element = e.Result;
            var rows = element.Descendants("dc_ingredient");

            foreach (var row in rows)
            {
                Row dataRow = new Row();
                foreach (XElement xe in row.Elements())
                {
                    #region dc_ingredients

                    if (xe.Name.LocalName.Equals("dc_name", StringComparison.OrdinalIgnoreCase))
                    {
                        dataRow[xe.Name.LocalName] = xe.Value;
                    }
                    else if (xe.Name.LocalName.Equals("dc_ingredientid", StringComparison.OrdinalIgnoreCase))
                    {
                        Guid g = Guid.Empty;
                        if (Guid.TryParse(xe.Value, out g))
                        {
                            dataRow[xe.Name.LocalName] = new Guid(xe.Value);
                        }
                    }
                    else if (xe.Name.LocalName.Equals("dc_foodingredientid", StringComparison.OrdinalIgnoreCase))
                    {
                        dataRow[xe.Name.LocalName] = new PairWithList(xe.Value, xe.Attribute("Id").Value, "dc_foods", new List<PairWithList>());
                    }
                    else if (xe.Name.LocalName.Equals("dc_portionsize", StringComparison.OrdinalIgnoreCase))
                    {
                        decimal d = 0;
                        if (Decimal.TryParse(xe.Value, out d))
                        {
                            dataRow[xe.Name.LocalName] = d;
                        }
                    }
                    else if (xe.Name.LocalName.Equals("dc_portiontypeid", StringComparison.OrdinalIgnoreCase))
                    {
                        dataRow[xe.Name.LocalName] = new PairWithList(xe.Value, xe.Attribute("Id").Value, "dc_foods", new List<PairWithList>());
                    }
                    #endregion
                }
                recipe.IngredientData.Add(dataRow);
            }
        }

        /// <summary>
        /// Search for matching list of foods
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void food_KeyDown(object sender, KeyEventArgs e)
        {

            comboBox = (Controls.ComboBoxWithValidation)sender;

            String text = comboBox.MyAutoCompleteBox.Text;

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

                        XElement orderXml = null;
                        String fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
                          <entity name='dc_foods'>
                            <attribute name='dc_name' />
                            <attribute name='dc_portion_amount' />
                            <attribute name='dc_portiontypeid' />
                            <attribute name='dc_unit_gram_weight' />
                            <attribute name='dc_foodsid' />
                            <attribute name='dc_recipefood' />
                            <order attribute='dc_name' descending='false' />
                            <filter type='and'>
                                <condition attribute='dc_name' value='%@TEXT%' operator='like'/>
                                <condition attribute='dc_food_id' operator='null'/>
                                <condition attribute='dc_reviewed' operator='eq' value='1'/>
                                <condition attribute='statecode' operator='eq' value='0'/>
                            </filter>
                            <link-entity name='dc_portion_types' from='dc_portion_typesid' to='dc_portiontypeid' alias='dc_portion_types'>
                              <attribute name='dc_name' />
                              <attribute name='dc_abbreviation' />
                            </link-entity>
    
                                <link-entity name='dc_food_nutrients' from='dc_food_nutrientsid' to='dc_foodnutrientid' alias='dc_food_nutrients'>
                                    <attribute name='dc_fat' />
                                    <attribute name='dc_protein' />
                                    <attribute name='dc_carbohydrate' />
                                </link-entity>
                          </entity>
                        </fetch>";

                        fetchXml = fetchXml.Replace("@TEXT", text);

                        orderXml = new XElement("ColumnOrder");
                        orderXml.Add(new XElement("Column", "dc_name"));
                        orderXml.Add(new XElement("Column", "dc_portion_amount"));
                        orderXml.Add(new XElement("Column", "dc_portiontypeid"));
                        orderXml.Add(new XElement("Column", "dc_food_nutrients.dc_fat"));
                        orderXml.Add(new XElement("Column", "dc_food_nutrients.dc_protein"));
                        orderXml.Add(new XElement("Column", "dc_food_nutrients.dc_carbohydrate"));
                        orderXml.Add(new XElement("Column", "dc_meal_componentid"));
                        orderXml.Add(new XElement("Column", "dc_recipefood"));

                        orderXml.Add(new XElement("Column", "dc_portion_types.dc_name"));
                        orderXml.Add(new XElement("Column", "dc_portion_types.dc_abbreviation"));
                        orderXml.Add(new XElement("Column", "dc_unit_gram_weight"));
                        try
                        {
                            CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
                            cms.RetrieveFoodIngredientsCompleted += new EventHandler<CrmSdk.RetrieveFoodIngredientsCompletedEventArgs>(cms_RetrieveFoods);
                            cms.RetrieveFoodIngredientsAsync(General.ContactId(), text);
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
        private void cms_RetrieveFoods(object sender, CrmSdk.RetrieveFoodIngredientsCompletedEventArgs e)
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
                        decimal fat = 0m;
                        decimal protein = 0m;
                        decimal carbs = 0m;
                        decimal orgPortionSize = 0m;
                        decimal unitGramWeight = 0m;
                        bool recipe = false;
                        PairWithList portionType = new PairWithList();
                        String portionTypeName = String.Empty;
                        String portionTypeAbbreviation = String.Empty;

                        foreach (XElement xe in row.Elements())
                        {
                            if (xe.Name.LocalName.Equals("dc_name", StringComparison.OrdinalIgnoreCase))
                            {
                                name = xe.Value;
                            }
                            else if (xe.Name.LocalName.Equals("dc_recipefood", StringComparison.OrdinalIgnoreCase))
                            {
                                recipe = Convert.ToBoolean(xe.Value);
                            }
                            else if (xe.Name.LocalName.Equals(entityName + "id", StringComparison.OrdinalIgnoreCase))
                            {
                                Id = xe.Value;
                            }
                            else if (xe.Name.LocalName.Equals("dc_food_nutrients.dc_fat", StringComparison.OrdinalIgnoreCase))
                            {
                                fat = Convert.ToDecimal(xe.Value);
                            }
                            else if (xe.Name.LocalName.Equals("dc_food_nutrients.dc_protein", StringComparison.OrdinalIgnoreCase))
                            {
                                protein = Convert.ToDecimal(xe.Value);
                            }
                            else if (xe.Name.LocalName.Equals("dc_food_nutrients.dc_carbohydrate", StringComparison.OrdinalIgnoreCase))
                            {
                                carbs = Convert.ToDecimal(xe.Value);
                            }
                            else if (xe.Name.LocalName.Equals("dc_portion_amount", StringComparison.OrdinalIgnoreCase))
                            {
                                orgPortionSize = Convert.ToDecimal(xe.Value);
                            }
                            else if (xe.Name.LocalName.Equals("dc_unit_gram_weight", StringComparison.OrdinalIgnoreCase))
                            {
                                unitGramWeight = Convert.ToDecimal(xe.Value);
                            }
                            else if (xe.Name.LocalName.Equals("dc_portiontypeid", StringComparison.OrdinalIgnoreCase))
                            {
                                portionType = new PairWithList(xe.Value, xe.Attribute("Id").Value);
                            }
                            else if (xe.Name.LocalName.Equals("dc_portion_types.dc_name", StringComparison.OrdinalIgnoreCase))
                            {
                                portionTypeName = xe.Value;
                            }
                            else if (xe.Name.LocalName.Equals("dc_portion_types.dc_abbreviation", StringComparison.OrdinalIgnoreCase))
                            {
                                portionTypeAbbreviation = xe.Value;
                            }
                        }
                        data.Add(new PairWithList(name, Id));
                        if (!foods.ContainsKey(new Guid(Id)))
                        {
                            Food f = new Food();
                            f.Fat = fat;
                            f.Protein = protein;
                            f.Carbohydrate = carbs;
                            f.PortionSize = orgPortionSize;
                            f.PortionType = portionType;
                            f.IsRecipe = recipe;
                            f.PortionTypeName = portionTypeName;
                            f.PortionTypeAbbreviation = portionTypeAbbreviation;
                            f.UnitGramWeight = unitGramWeight;
                            foods.Add(new Guid(Id), f);
                        }
                    }
                    //comboBox.MyAutoCompleteBox.ItemsSource = data;
                    comboBox.SelectedPair = new PairWithList(string.Empty, String.Empty, String.Empty, data);
                    comboBox.IsBusy = false;
                    comboBox.OpenDropdown(true);
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
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

            Row r = (Row)DataGrid.SelectedItem;
            if (r != null)
            {
                r.RowChanged = true;

                AutoCompleteBox cb = (AutoCompleteBox)sender;
                //Get parent datagrid

                if (cb.SelectedItem != null && !String.IsNullOrEmpty(cb.Text))
                {
                    PairWithList p = (PairWithList)cb.SelectedItem;
                    //Retreive the portion size
                    String fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                      <entity name='dc_foods'>
                        <attribute name='dc_portiontypeid' />
                        <attribute name='dc_portion_amount' />
                        <order attribute='dc_foodsid' descending='false' />
                              <filter type='and'>
                                <condition attribute='dc_foodsid' operator='eq' value='@FOODID' />
                              </filter>
                      </entity>
                    </fetch>";

                    fetchXml = fetchXml.Replace("@FOODID", p.Id);
                    XElement orderXml = new XElement("ColumnOrder");
                    orderXml.Add(new XElement("Column", "dc_portiontype"));
                    orderXml.Add(new XElement("Column", "dc_portion_amount"));
                    orderXml.Add(new XElement("Column", "dc_foodsid"));

                    r["dc_portionsize"] = foods[new Guid(p.Id)].PortionSize.ToString();
                    r["dc_portiontypeid"] = foods[new Guid(p.Id)].PortionType;
                    r["dc_foods.dc_recipefood"] = foods[new Guid(p.Id)].IsRecipe;
                    r["dc_portion_types.dc_name"] = foods[new Guid(p.Id)].PortionTypeName;
                    r["dc_portion_types.dc_abbreviation"] = foods[new Guid(p.Id)].PortionTypeAbbreviation;
                    r["dc_unit_gram_weight"] = foods[new Guid(p.Id)].UnitGramWeight;

                }
            }
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LayoutRoot.DataContext = recipe;
            //build out the grid after the control has loaded.  This will help with sizing issues on child windows
            buildGrid(DataGrid);
            foreach (String field in watermarkFields)
            {
                //find child and set watermark
                object element = FormData.FindName(field) as object;
                String label = String.Empty;
                if (field.Contains("dc_food_nutrients"))
                {
                    if (App.Current.GetType() == typeof(DynamicConnections.NutriStyle.MenuGenerator.App))
                    {
                        label = General.RetrieveAttributeLabelName(field.Replace("dc_food_nutrients_", ""), (XElement)((App)App.Current).metadataList["dc_food_nutrients"]);
                    }
                }
                else
                {
                    if (App.Current.GetType() == typeof(DynamicConnections.NutriStyle.MenuGenerator.App))
                    {
                        label = General.RetrieveAttributeLabelName(field, (XElement)((App)App.Current).metadataList["dc_foods"]);
                    }
                }
                if (element.GetType() == typeof(WatermarkTextBox))
                {
                    //Get label
                    ((WatermarkTextBox)element).Watermark = label;
                }
                else if (element.GetType() == typeof(ComboBoxWithValidation))
                {
                    //Get label
                    ((ComboBoxWithValidation)element).Watermark = label;
                }
                else if (element.GetType() == typeof(WatermarkRichTextBox))
                {
                    //Get label
                    ((WatermarkRichTextBox)element).Watermark = label;
                }
            }
            //load up dropdowns
            if (App.Current.GetType() == typeof(DynamicConnections.NutriStyle.MenuGenerator.App))
            {
                dc_portiontypeid.SelectedPair = new PairWithList(String.Empty, String.Empty, String.Empty, ((App)App.Current).PortionTypes);
            }

            //Add red '*' to required labels
            foreach (String str in requiredFields)
            {
                TextBlock tb = this.GetVisualDescendants().OfType<TextBlock>().Where(txb => txb.Name == str + "Label").FirstOrDefault();
                Run r = new Run();
                r.Text = "*";
                //{StaticResource defaultFontRequiredColor}
                r.Foreground = Application.Current.Resources["defaultFontRequiredColor"] as SolidColorBrush;
                tb.Inlines.Add(r);
            }
        }


        //Get all child control with type T
        public IEnumerable<T> GetChildren<T>(DependencyObject d) where T : DependencyObject
        {
            var count = VisualTreeHelper.GetChildrenCount(d);
            for (int i = 0; i < count; i++)
            {
                var c = VisualTreeHelper.GetChild(d, i);
                if (c is T)
                    yield return (T)c;
                foreach (var c1 in GetChildren<T>(c))
                    yield return c1;
            }
        }


        /// <summary>
        /// Save the recipe.  Build an XElement document and sumbit to the webserver.  Validates the data first.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveFood(object sender, RoutedEventArgs e)
        {
            bool error = false;
            //Need to validate first
            var textBoxList = GetChildren<WatermarkTextBox>(FormData);
            var comboBoxList = GetChildren<ComboBoxWithValidation>(FormData);
            var checkboxList = GetChildren<CheckBox>(FormData);
            var richTextBoxList = GetChildren<WatermarkRichTextBox>(FormData);
            
            foreach (var element in textBoxList)
            {
                if (element.GetBindingExpression(WatermarkTextBox.TextProperty) != null)
                {
                    //is required?
                    if (requiredFields.Contains(element.Name) || !String.IsNullOrEmpty(element.Text))
                    {
                        element.GetBindingExpression(WatermarkTextBox.TextProperty).UpdateSource();
                    }
                }
                if (requiredFields.Contains(element.Name))
                {
                    if (Validation.GetHasError(element))
                    {
                        error = true;
                    }
                }
            }

            foreach (var element in richTextBoxList)
            {
                if (element.GetBindingExpression(WatermarkRichTextBox.BindableXamlProperty) != null)
                {
                    //is required?
                    if (requiredFields.Contains(element.Name) || !String.IsNullOrEmpty(element.BindableXaml))
                    {
                        element.GetBindingExpression(WatermarkRichTextBox.BindableXamlProperty).UpdateSource();
                    }
                }
                if (requiredFields.Contains(element.Name))
                {
                    if (Validation.GetHasError(element))
                    {
                        error = true;
                    }
                }
            }
            foreach (var element in comboBoxList)
            {
                if (element.GetBindingExpression(ComboBoxWithValidation.SelectedPairProperty) != null)
                {
                    element.GetBindingExpression(ComboBoxWithValidation.SelectedPairProperty).UpdateSource();
                }
                if (requiredFields.Contains(element.Name))
                {
                    if (Validation.GetHasError(element))
                    {
                        error = true;
                    }
                }
            }
            foreach (var element in checkboxList)
            {
                if (element.GetBindingExpression(CheckBox.IsCheckedProperty) != null)
                {
                    element.GetBindingExpression(CheckBox.IsCheckedProperty).UpdateSource();

                }
                if (requiredFields.Contains(element.Name))
                {
                    if (Validation.GetHasError(element))
                    {
                        error = true;
                    }
                }
            }

            availableToAllUsersYes.GetBindingExpression(ToggleButton.IsCheckedProperty).UpdateSource();
            availableToAllUsersNo.GetBindingExpression(ToggleButton.IsCheckedProperty).UpdateSource();
            //validate radio button
            if (availableToAllUsersYes.IsChecked.Value)
            {
                availableToAllUsersYes.GetBindingExpression(ToggleButton.IsCheckedProperty).UpdateSource();
            }
            if (availableToAllUsersNo.IsChecked.Value)
            {
                availableToAllUsersNo.GetBindingExpression(ToggleButton.IsCheckedProperty).UpdateSource();
            }

            if (!availableToAllUsersYes.IsChecked.Value && !availableToAllUsersNo.IsChecked.Value)
            {
                availableToAllUsersNo.GetBindingExpression(ToggleButton.IsCheckedProperty).UpdateSource();
                availableToAllUsersYes.GetBindingExpression(ToggleButton.IsCheckedProperty).UpdateSource();
            }

            if (Validation.GetHasError(availableToAllUsersNo) && Validation.GetHasError(availableToAllUsersYes))
            {
                error = true;
            }

            //description.GetBindingExpression(TextBox.TextProperty).UpdateSource();

            if (!error)
            {
                busyIndicator.IsBusy = true;
                XElement entities = new XElement("entities");
                //Create dc_foods_nutrient first
                XElement foodNutrients = new XElement("dc_food_nutrients");

                //Create dc_foods
                XElement foodXml = new XElement("dc_foods");
                //Guid
                if (recipe.FoodId == Guid.Empty)
                {
                    recipe.FoodId = Guid.NewGuid();
                    XAttribute create = new XAttribute("create", "true");
                    foodXml.Add(create);
                }
                if (recipe.FoodNutrientId == Guid.Empty)
                {
                    recipe.FoodNutrientId = Guid.NewGuid();
                    //Add create attribute to the foodNutrients
                    XAttribute create = new XAttribute("create", "true");
                    foodNutrients.Add(create);
                }

                foodNutrients.Add(new XElement("dc_food_nutrientsid", recipe.FoodNutrientId.ToString()));


                //Relate to dc_food_nutrients
                XElement foodNutrientId = new XElement("dc_foodnutrientid", recipe.FoodNutrientId.ToString());
                XAttribute attribute = new XAttribute("entityname", "dc_food_nutrients");
                foodNutrientId.Add(attribute);
                foodXml.Add(foodNutrientId);
                //Add dc_name
                foodXml.Add(new XElement("dc_name", recipe.Name));
                foodXml.Add(new XElement("dc_foodsid", recipe.FoodId.ToString()));
                //mark as recipe
                foodXml.Add(new XElement("dc_recipefood", "true"));
                foodXml.Add(new XElement("dc_sourceofinformationdateobtained", recipe.SourceOfInformation));
                foodXml.Add(new XElement("dc_portion_amount", recipe.PortionAmount));

                foodXml.Add(new XElement("dc_preparationtime", recipe.PreparationTime));
                foodXml.Add(new XElement("dc_numberofservings", recipe.NumberOfServings));
                //foodXml.Add(new XElement("dc_unit_gram_weight", recipe.UnitGramWeight));

                foodXml.Add(new XElement("dc_directions", new XCData(recipe.Directions)));

                //Portion Type Id
                XElement portionType = new XElement("dc_portiontypeid", recipe.PortionTypeId.Id);
                XAttribute portionTypeEntityName = new XAttribute("entityname", "dc_portion_types");
                portionType.Add(portionTypeEntityName);
                foodXml.Add(portionType);

                //contact Id
                XElement contactId = new XElement("dc_contactid", ((App)App.Current).contactId.ToString());
                XAttribute contactIdEntityName = new XAttribute("entityname", "contact");
                contactId.Add(contactIdEntityName);
                foodXml.Add(contactId);
                foodXml.Add(new XElement("dc_availabletoallusers", recipe.AvailableToAllUsers));

                //Add to entities node
                entities.Add(foodNutrients);
                entities.Add(foodXml);

                //loop through ingredients
                foreach (Row r in recipe.IngredientData)
                {
                    XElement ingredient = new XElement("dc_ingredient");
                    //parent food id
                    XElement parentfoodId = new XElement("dc_foodid", recipe.FoodId.ToString());
                    XAttribute parentfoodIdEntityName = new XAttribute("entityname", "dc_foods");
                    parentfoodId.Add(parentfoodIdEntityName);
                    ingredient.Add(parentfoodId);

                    ingredient.Add(new XElement("dc_name", ((PairWithList)r["dc_foodingredientid"]).Name));

                    //dc_foodingredientid
                    XElement ingredientfoodId = new XElement("dc_foodingredientid", ((PairWithList)r["dc_foodingredientid"]).Id);
                    XAttribute ingredientfoodIdEntityName = new XAttribute("entityname", "dc_foods");
                    ingredientfoodId.Add(ingredientfoodIdEntityName);
                    ingredient.Add(ingredientfoodId);

                    //Portion Type Id
                    XElement ingredientPortionType = new XElement("dc_portiontypeid", ((PairWithList)r["dc_portiontypeid"]).Id);
                    XAttribute ingredientPortionTypeEntityName = new XAttribute("entityname", "dc_portion_types");
                    ingredientPortionType.Add(ingredientPortionTypeEntityName);
                    ingredient.Add(ingredientPortionType);

                    //Portion Size
                    ingredient.Add(new XElement("dc_portionsize", r["dc_portionsize"]));

                    //Org Portion Size
                    Guid Id = new Guid(((PairWithList)r["dc_foodingredientid"]).Id);
                    //org unit gram weight
                    ingredient.Add(new XElement("dc_originalgramweight", r["dc_unit_gram_weight"]));//need org size
                    //id
                    if (r["dc_ingredientid"] != null)
                    {
                        ingredient.Add(new XElement("dc_ingredientid", ((Guid)r["dc_ingredientid"]).ToString()));
                    }

                    entities.Add(ingredient);
                }



                //Save
                CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
                cms.CreateUpdateReturnEntitiesCompleted += new EventHandler<CrmSdk.CreateUpdateReturnEntitiesCompletedEventArgs>(cms_CreateUpdateFoodLike);
                cms.CreateUpdateReturnEntitiesAsync(entities, true);
            }
        }
        /// <summary>
        /// Delete/Remove item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ccb_MouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
            try
            {
                SortableCollectionView data = (SortableCollectionView)DataGrid.ItemsSource;

                Button img = sender as Button;

                Row r = (Row)DataGrid.SelectedItem;
                if (((Row)DataGrid.SelectedItem)["dc_ingredientid"] != null)
                {

                    String Id = ((Guid)((Row)DataGrid.SelectedItem)["dc_ingredientid"]).ToString();

                    if (!String.IsNullOrEmpty(Id))
                    {
                        XElement deleteXml = new XElement("dc_ingredient", new XElement("id", Id));

                        CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
                        cms.DeleteCompleted += new EventHandler<CrmSdk.DeleteCompletedEventArgs>(cms_Delete);
                        cms.DeleteAsync(deleteXml);
                    }
                }
                data.Remove(r);
                DataGrid.ItemsSource = data;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
            }
        }
        private void cms_Delete(object sender, CrmSdk.DeleteCompletedEventArgs e)
        {
        }

        /// <summary>
        /// For ingredient row
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddIngredient_Click(object sender, EventArgs e)
        {

            //SortableCollectionView dgCollection = (SortableCollectionView)dataGrid1.ItemsSource;
            //Now add row to correct datagrid
            Row r = new Row();

            //Populate columnTypes  --  rowData.ColumnTypes.Add(xe.Name.LocalName, xe.Attribute("Type").Value);
            foreach (KeyValuePair<String, String> type in recipe.IngredientData.ColumnTypes)
            {
                if (type.Value.Equals("Lookup", StringComparison.OrdinalIgnoreCase))
                {
                    r[type.Key] = new PairWithList(String.Empty, Guid.Empty.ToString(), recipe.IngredientData.EntityTypes[type.Key], new List<PairWithList>());
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
                else if (type.Value.Equals("Decimal", StringComparison.OrdinalIgnoreCase))
                {
                    r[type.Key] = null;
                }
                else if (type.Value.Equals("UniqueIdentifier", StringComparison.OrdinalIgnoreCase))
                {
                    r[type.Key] = Guid.NewGuid().ToString();
                }
                else
                {
                    r[type.Key] = String.Empty;
                }
            }
            r.RowChanged = true;
            r.CreateRow = true;
            try
            {
                //dataFoods.Add(r);
                //dgCollection.Insert(0, r);
                recipe.IngredientData.Add(r);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
            }
        }
        private void buildGrid(DataGrid dataGrid)
        {

            SortableCollectionView data = new SortableCollectionView();
            try
            {



                DataGridTemplateColumn dg = new DataGridTemplateColumn();
                String label = General.RetrieveAttributeLabelName("dc_foodingredientid", (XElement)((App)App.Current).metadataList["dc_ingredient"]);
                dg.Header = label;
                dg.SortMemberPath = "dc_foodingredientid";
                dg.CanUserSort = true;

                StringBuilder CellETempPickList = new StringBuilder();
                CellETempPickList.Append("<DataTemplate ");
                CellETempPickList.Append("xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' ");
                CellETempPickList.Append("xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' ");
                CellETempPickList.Append("xmlns:basics2='clr-namespace:DynamicConnections.NutriStyle.MenuGenerator.Controls;");
                CellETempPickList.Append("assembly=DynamicConnections.NutriStyle.MenuGenerator' >");

                //CellETempPickList.Append("<basics2:ComboBoxWithValidation  TagName='" + xe.Name.LocalName + "'  SelectedPair='{Binding Path=Data, Converter={StaticResource rowConvertor}, ConverterParameter=" + xe.Name.LocalName + ", Mode=TwoWay}' IsEnabled='{Binding Path=Data, Converter={StaticResource rowConvertor}, ConverterParameter=controlenabled, Mode=TwoWay}' />");
                CellETempPickList.Append("<basics2:ComboBoxWithValidation Watermark='" + label + "' TagName='dc_foodingredientid' SelectedPair='{Binding Path=Data, Converter={StaticResource rowConvertor}, ConverterParameter=dc_foodingredientid, Mode=TwoWay}' />");
                CellETempPickList.Append("</DataTemplate>");
                dg.CellEditingTemplate = (DataTemplate)XamlReader.Load(CellETempPickList.ToString());
                dg.Width = new DataGridLength(300);
                dataGrid.Columns.Add(dg);

                label = General.RetrieveAttributeLabelName("dc_portiontypeid", (XElement)((App)App.Current).metadataList["dc_ingredient"]);
                dg = new DataGridTemplateColumn();
                dg.Header = label;
                dg.SortMemberPath = "dc_portiontypeid";
                dg.CanUserSort = true;

                CellETempPickList = new StringBuilder();
                CellETempPickList.Append("<DataTemplate ");
                CellETempPickList.Append("xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' ");
                CellETempPickList.Append("xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' ");
                CellETempPickList.Append("xmlns:basics2='clr-namespace:DynamicConnections.NutriStyle.MenuGenerator.Controls;");
                CellETempPickList.Append("assembly=DynamicConnections.NutriStyle.MenuGenerator' >");

                //CellETempPickList.Append("<basics2:ComboBoxWithValidation  TagName='" + xe.Name.LocalName + "'  SelectedPair='{Binding Path=Data, Converter={StaticResource rowConvertor}, ConverterParameter=" + xe.Name.LocalName + ", Mode=TwoWay}' IsEnabled='{Binding Path=Data, Converter={StaticResource rowConvertor}, ConverterParameter=controlenabled, Mode=TwoWay}' />");
                CellETempPickList.Append("<basics2:ComboBoxWithValidation TagName='dc_portiontypeid' IsEnabled='false' SelectedPair='{Binding Path=Data, Converter={StaticResource rowConvertor}, ConverterParameter=dc_portiontypeid, Mode=TwoWay}'  />");
                CellETempPickList.Append("</DataTemplate>");
                dg.CellEditingTemplate = (DataTemplate)XamlReader.Load(CellETempPickList.ToString());
                dg.Width = new DataGridLength(100);
                dataGrid.Columns.Add(dg);

                label = General.RetrieveAttributeLabelName("dc_portionsize", (XElement)((App)App.Current).metadataList["dc_ingredient"]);
                dg = new DataGridTemplateColumn();
                dg.Header = label;

                dg.SortMemberPath = "dc_portionsize";
                dg.CanUserSort = true;
                CellETempPickList = new StringBuilder();
                CellETempPickList.Append("<DataTemplate ");
                CellETempPickList.Append("xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' ");
                CellETempPickList.Append("xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' ");
                CellETempPickList.Append("xmlns:basics2='clr-namespace:DynamicConnections.NutriStyle.MenuGenerator.Controls;");
                CellETempPickList.Append("assembly=DynamicConnections.NutriStyle.MenuGenerator' >");

                CellETempPickList.Append("<basics2:CustomTextBox Watermark='" + label + "' TagName='dc_portionsize' Foreground='#4C97CC' SelectedText='{Binding Path=Data, Converter={StaticResource rowConvertor}, ConverterParameter=dc_portionsize, Mode=TwoWay}' />");
                CellETempPickList.Append("</DataTemplate>");
                dg.CellTemplate = (DataTemplate)XamlReader.Load(CellETempPickList.ToString());

                dg.Width = new DataGridLength(90);
                dataGrid.Columns.Add(dg);



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
                CellETemp.Append("<Button Cursor='Hand' ToolTipService.ToolTip='Remove Ingredient' Width='26' Height='26' HorizontalAlignment='Left'>");
                CellETemp.Append("<Image  Stretch='Fill' Name='Delete' Source='/DynamicConnections.NutriStyle.MenuGenerator;component/images/delete.png'/>");// MouseLeftButtonDown='ImageDelete_MouseLeftButtonDown'
                CellETemp.Append("</Button>");
                CellETemp.Append("</DataTemplate>");
                dgImage.CellEditingTemplate = (DataTemplate)XamlReader.Load(CellETemp.ToString());

                dgImage.IsReadOnly = false;
                dgImage.Width = new DataGridLength(34);
                //visable?
                dgImage.Visibility = System.Windows.Visibility.Visible;

                dataGrid.Columns.Add(dgImage);


                dataGrid.ItemsSource = recipe.IngredientData;

                dataGrid.Visibility = System.Windows.Visibility.Visible;
                dataGrid.SelectedIndex = 0;

                dataGrid.BorderThickness = new Thickness(1);


                // Space available to fill ( -18 Standard vScrollbar)
                double space_available = (LayoutRoot.ActualWidth - 25 - 34 - 90 - 106); //18 is width of scroll bar, 150 is width of menu 
                //figure out column types

                int count = 3;//delete, portion size, portion type,
                if (space_available > 0)
                {
                    foreach (DataGridColumn dg_c in dataGrid.Columns)
                    {
                        if (dg_c.Width.Value != 34 && dg_c.Width.Value != 90 && dg_c.Width.Value != 100)
                        {
                            dg_c.Width = new DataGridLength((space_available / (dataGrid.Columns.Count - count)));
                        }
                    }
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                MessageBox.Show(err.StackTrace);
            }
            //bi.IsBusy = false;

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
                busyIndicator.IsBusy = false;
            }
            else
            {
                var nodes = e.Result.Descendants("resultset");
                SavedClicked(e.Result, new EventArgs());
                busyIndicator.IsBusy = false;
            }
        }
    }
}