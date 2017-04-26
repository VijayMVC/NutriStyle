﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using DynamicConnections.NutriStyle.MenuGenerator.Engine.Helpers;
using System.Xml.Linq;
using DynamicConnections.NutriStyle.MenuGenerator.ChildWindows;
using System.Windows.Controls.Primitives;
namespace DynamicConnections.NutriStyle.MenuGenerator.Controls

{
    public partial class AddAFood : UserControl
    {
        Engine.FormData.Food food;
        List<String> requiredFields;
        List<String> watermarkFields;

        public event EventHandler SavedClicked;

        public Guid FoodId {get;set;}

        public AddAFood()
        {
            InitializeComponent();
            //set available to all users to false
            
            food = new Engine.FormData.Food();
            food.AvailableToAllUsers = Engine.FormData.Food.AvailableToAllUsersOptions.False;


            requiredFields = new List<string>();
            requiredFields.Add("dc_name");
            requiredFields.Add("dc_sourceofinformationdateobtained");
            requiredFields.Add("dc_foodmanufacturerweb");
            requiredFields.Add("dc_unit_gram_weight");
            requiredFields.Add("dc_portion_amount");
            requiredFields.Add("dc_portiontypeid");
            requiredFields.Add("dc_food_nutrients_dc_kcals");
            requiredFields.Add("dc_food_nutrients_dc_fat");
            requiredFields.Add("dc_food_nutrients_dc_carbohydrate");
            requiredFields.Add("dc_food_nutrients_dc_protein");
            
            requiredFields.Add("dc_foodgroup");
            
            watermarkFields = new List<string>();
            watermarkFields.Add("dc_name");
            watermarkFields.Add("dc_foodgroup");
            watermarkFields.Add("dc_foodmanufacturerweb");
            watermarkFields.Add("dc_sourceofinformationdateobtained");
            watermarkFields.Add("dc_unit_gram_weight");
            watermarkFields.Add("dc_portion_amount");
            watermarkFields.Add("dc_portiontypeid");
            watermarkFields.Add("dc_food_nutrients_dc_kcals");
            watermarkFields.Add("dc_food_nutrients_dc_fat");
            watermarkFields.Add("dc_food_nutrients_dc_fa_sat");
            watermarkFields.Add("dc_food_nutrients_dc_fa_trans");
            watermarkFields.Add("dc_food_nutrients_dc_cholestrol");
            watermarkFields.Add("dc_food_nutrients_dc_sodium");
            watermarkFields.Add("dc_food_nutrients_dc_carbohydrate");
            watermarkFields.Add("dc_food_nutrients_dc_fiber");
            watermarkFields.Add("dc_food_nutrients_dc_sugar");
            watermarkFields.Add("dc_food_nutrients_dc_vit_a_iu");
            watermarkFields.Add("dc_food_nutrients_dc_vit_c");
            watermarkFields.Add("dc_food_nutrients_dc_calcium");
            watermarkFields.Add("dc_food_nutrients_dc_iron");
            watermarkFields.Add("dc_food_nutrients_dc_protein");
            watermarkFields.Add("dc_food_nutrients_dc_alcohol");

            //additional fields
            watermarkFields.Add("dc_food_nutrients_dc_vit_d");
            watermarkFields.Add("dc_food_nutrients_dc_vit_e");
            watermarkFields.Add("dc_food_nutrients_dc_vit_k");
            watermarkFields.Add("dc_food_nutrients_dc_thiamin");
            watermarkFields.Add("dc_food_nutrients_dc_riboflavin");
            watermarkFields.Add("dc_food_nutrients_dc_niacin");

            watermarkFields.Add("dc_food_nutrients_dc_vit_b6");
            watermarkFields.Add("dc_food_nutrients_dc_folate");
            watermarkFields.Add("dc_food_nutrients_dc_vit_b12");
            watermarkFields.Add("dc_food_nutrients_dc_biotin");
            watermarkFields.Add("dc_food_nutrients_dc_panto_acid");
            watermarkFields.Add("dc_food_nutrients_dc_potassium");
            watermarkFields.Add("dc_food_nutrients_dc_magnesium");
            watermarkFields.Add("dc_food_nutrients_dc_zinc");
            watermarkFields.Add("dc_food_nutrients_dc_selenium");
            watermarkFields.Add("dc_food_nutrients_dc_copper");
            watermarkFields.Add("dc_food_nutrients_dc_manganese");
            watermarkFields.Add("dc_food_nutrients_dc_choline");
            watermarkFields.Add("dc_food_nutrients_dc_chromium");
            watermarkFields.Add("dc_food_nutrients_dc_phosphorus");
            watermarkFields.Add("dc_ingredientlist");
        }
        public void RetrieveFood(Guid foodId)
        {
           

            String fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
              <entity name='dc_foods'>
                <attribute name='dc_name' />
                <attribute name='dc_foodgroup' />
                <attribute name='dc_sourceofinformationdateobtained' />
                <attribute name='dc_foodmanufacturerweb' />
                <attribute name='dc_unit_gram_weight' />
                <attribute name='dc_portion_amount' />
                <attribute name='dc_portiontypeid' />
                <attribute name='dc_canuseinmenu' />
                <attribute name='dc_availabletoallusers' />
                <attribute name='dc_corn' />
                <attribute name='dc_egg' />
                <attribute name='dc_dairy' />
                <attribute name='dc_artificialsweeteners' />
                <attribute name='dc_soy' />
                <attribute name='dc_peanut' />
                <attribute name='dc_caffeine' />
                <attribute name='dc_animalproduct' />
                <attribute name='dc_artificialcolorsadditives' />
                <attribute name='dc_foodsid' />
                <order attribute='dc_name' descending='false' />
                <filter type='and'>
                    <condition attribute='dc_foodsid' value='@FOODID' operator='eq'/>
                </filter>
                <link-entity name='dc_food_nutrients' from='dc_food_nutrientsid' to='dc_foodnutrientid' alias='dc_food_nutrients'>
                    <attribute name='dc_kcals' />
                    <attribute name='dc_fat' />
                    <attribute name='dc_protein' />
                    <attribute name='dc_alcohol' />
                    <attribute name='dc_fa_sat' />
                    <attribute name='dc_fa_trans' />
                    <attribute name='dc_cholestrol' />
                    <attribute name='dc_sodium' />
                    <attribute name='dc_carbohydrate' />
                    <attribute name='dc_fiber' />
                    <attribute name='dc_sugar' />
                    <attribute name='dc_vit_a_iu' />
                    <attribute name='dc_vit_c' />
                    <attribute name='dc_calcium' />
                    <attribute name='dc_iron' />
           
                    <attribute name='dc_vit_d' />
                    <attribute name='dc_vit_e' />
                    <attribute name='dc_vit_k' />
                    <attribute name='dc_thiamin' />
                    <attribute name='dc_riboflavin' />
                    <attribute name='dc_niacin' />

                    <attribute name='dc_vit_b6' />
                    <attribute name='dc_folate' />
                    <attribute name='dc_vit_b12' />
                    <attribute name='dc_biotin' />
                    <attribute name='dc_panto_acid' />
                    <attribute name='dc_potassium' />
                    <attribute name='dc_magnesium' />
                    <attribute name='dc_zinc' />
                    <attribute name='dc_selenium' />
                    <attribute name='dc_copper' />
                    <attribute name='dc_manganese' />
                    <attribute name='dc_choline' />
                    <attribute name='dc_chromium' />
                    <attribute name='dc_phosphorus' />
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
                else
                {
                    orderXml.Add(new XElement("Column", xe.Parent.Attributes("alias").First().Value+ "." + xe.Attributes("name").First().Value));
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
                        food.Name = xe.Value;
                    }
                    else if (xe.Name.LocalName.Equals("dc_sourceofinformationdateobtained", StringComparison.OrdinalIgnoreCase))
                    {
                        food.SourceOfInformation = xe.Value;
                    }
                    else if (xe.Name.LocalName.Equals("dc_foodmanufacturerweb", StringComparison.OrdinalIgnoreCase))
                    {
                        food.FoodManufacturerWeb = xe.Value;
                    }
                    else if (xe.Name.LocalName.Equals("dc_unit_gram_weight", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!String.IsNullOrEmpty(xe.Value))
                        {
                            food.UnitGramWeight = Convert.ToDecimal(xe.Value);
                        }
                    }
                    else if (xe.Name.LocalName.Equals("dc_portion_amount", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!String.IsNullOrEmpty(xe.Value))
                        {
                            food.PortionAmount = Convert.ToDecimal(xe.Value);
                        }
                    }
                    else if (xe.Name.LocalName.Equals("dc_portiontypeid", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!String.IsNullOrEmpty(xe.Value))
                        {
                            food.PortionTypeId = new PairWithList(xe.Value, xe.Attributes("Id").First().Value, String.Empty, new List<PairWithList>());
                        }
                    }
                    else if (xe.Name.LocalName.Equals("dc_foodgroup", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!String.IsNullOrEmpty(xe.Value))
                        {
                            food.FoodGroup = new PairWithList(xe.Value, xe.Attributes("Id").First().Value, String.Empty, new List<PairWithList>());
                        }
                    }
                    else if (xe.Name.LocalName.Equals("dc_canuseinmenu", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!String.IsNullOrEmpty(xe.Value))
                        {
                            if (Boolean.Parse(xe.Value))
                            {
                                food.MenuFood = Engine.FormData.Food.MenuFoodOptions.True;
                            }
                            else
                            {
                                food.MenuFood = Engine.FormData.Food.MenuFoodOptions.False;
                            }
                        }
                    }
                    else if (xe.Name.LocalName.Equals("dc_availabletoallusers", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!String.IsNullOrEmpty(xe.Value))
                        {
                            if (Boolean.Parse(xe.Value))
                            {
                                food.AvailableToAllUsers = Engine.FormData.Food.AvailableToAllUsersOptions.True;
                            }
                            else
                            {
                                food.AvailableToAllUsers = Engine.FormData.Food.AvailableToAllUsersOptions.False;
                            }
                        }
                    }
                    else if (xe.Name.LocalName.Equals("dc_corn", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!String.IsNullOrEmpty(xe.Value))
                        {
                            food.Corn = Boolean.Parse(xe.Value);
                        }
                    }
                    else if (xe.Name.LocalName.Equals("dc_egg", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!String.IsNullOrEmpty(xe.Value))
                        {
                            food.Eggs = Boolean.Parse(xe.Value);
                        }
                    }
                    else if (xe.Name.LocalName.Equals("dc_dairy", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!String.IsNullOrEmpty(xe.Value))
                        {
                            food.Dairy = Boolean.Parse(xe.Value);
                        }
                    }
                    else if (xe.Name.LocalName.Equals("dc_artificialsweeteners", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!String.IsNullOrEmpty(xe.Value))
                        {
                            food.ArtificialSweeteners = Boolean.Parse(xe.Value);
                        }
                    }
                    else if (xe.Name.LocalName.Equals("dc_soy", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!String.IsNullOrEmpty(xe.Value))
                        {
                            food.Soy = Boolean.Parse(xe.Value);
                        }
                    }
                    else if (xe.Name.LocalName.Equals("dc_peanut", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!String.IsNullOrEmpty(xe.Value))
                        {
                            food.Peanuts = Boolean.Parse(xe.Value);
                        }
                    }
                    else if (xe.Name.LocalName.Equals("dc_caffeine", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!String.IsNullOrEmpty(xe.Value))
                        {
                            food.Caffeine = Boolean.Parse(xe.Value);
                        }
                    }
                    else if (xe.Name.LocalName.Equals("dc_animalproduct", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!String.IsNullOrEmpty(xe.Value))
                        {
                            food.AnimalProduct = Boolean.Parse(xe.Value);
                        }
                    }
                    else if (xe.Name.LocalName.Equals("dc_artificialcolorsadditives", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!String.IsNullOrEmpty(xe.Value))
                        {
                            food.ArtificialColorsAdditives = Boolean.Parse(xe.Value);
                        }
                    }
                    else if (xe.Name.LocalName.Equals("dc_foodsid", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!String.IsNullOrEmpty(xe.Value))
                        {
                            food.FoodId = new Guid(xe.Value);
                        }
                    } 
                    #endregion

                    #region dc_food_nutrients
                    else if (xe.Name.LocalName.Equals("dc_food_nutrients.dc_kcals", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!String.IsNullOrEmpty(xe.Value))
                        {
                            food.Kcals = Convert.ToDecimal(xe.Value);
                        }
                    }
                    else if (xe.Name.LocalName.Equals("dc_food_nutrients.dc_fa_sat", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!String.IsNullOrEmpty(xe.Value))
                        {
                            food.SaturatedFat = Convert.ToDecimal(xe.Value);
                        }
                    }
                    else if (xe.Name.LocalName.Equals("dc_food_nutrients.dc_fat", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!String.IsNullOrEmpty(xe.Value))
                        {
                            food.Fat = Convert.ToDecimal(xe.Value);
                        }
                    }
                    else if (xe.Name.LocalName.Equals("dc_food_nutrients.dc_fa_trans", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!String.IsNullOrEmpty(xe.Value))
                        {
                            food.TransFat = Convert.ToDecimal(xe.Value);
                        }
                    }
                    else if (xe.Name.LocalName.Equals("dc_food_nutrients.dc_cholestrol", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!String.IsNullOrEmpty(xe.Value))
                        {
                            food.Cholesterol = Convert.ToDecimal(xe.Value);
                        }
                    }
                    else if (xe.Name.LocalName.Equals("dc_food_nutrients.dc_sodium", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!String.IsNullOrEmpty(xe.Value))
                        {
                            food.Sodium = Convert.ToDecimal(xe.Value);
                        }
                    }
                    else if (xe.Name.LocalName.Equals("dc_food_nutrients.dc_carbohydrate", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!String.IsNullOrEmpty(xe.Value))
                        {
                            food.Carbohydrate = Convert.ToDecimal(xe.Value);
                        }
                    }
                    else if (xe.Name.LocalName.Equals("dc_food_nutrients.dc_fiber", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!String.IsNullOrEmpty(xe.Value))
                        {
                            food.Fiber = Convert.ToDecimal(xe.Value);
                        }
                    }
                    else if (xe.Name.LocalName.Equals("dc_food_nutrients.dc_sugar", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!String.IsNullOrEmpty(xe.Value))
                        {
                            food.Sugar = Convert.ToDecimal(xe.Value);
                        }
                    }
                    else if (xe.Name.LocalName.Equals("dc_food_nutrients.dc_vit_a_iu", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!String.IsNullOrEmpty(xe.Value))
                        {
                            food.VitaminA = Convert.ToDecimal(xe.Value);
                        }
                    }
                    else if (xe.Name.LocalName.Equals("dc_food_nutrients.dc_vit_c", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!String.IsNullOrEmpty(xe.Value))
                        {
                            food.VitaminC = Convert.ToDecimal(xe.Value);
                        }
                    }
                    else if (xe.Name.LocalName.Equals("dc_food_nutrients.dc_calcium", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!String.IsNullOrEmpty(xe.Value))
                        {
                            food.Calcium = Convert.ToDecimal(xe.Value);
                        }
                    }
                    else if (xe.Name.LocalName.Equals("dc_food_nutrients.dc_iron", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!String.IsNullOrEmpty(xe.Value))
                        {
                            food.Iron = Convert.ToDecimal(xe.Value);
                        }
                    }
                    else if (xe.Name.LocalName.Equals("dc_food_nutrients.dc_vit_d", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!String.IsNullOrEmpty(xe.Value))
                        {
                            food.VitaminD = Convert.ToDecimal(xe.Value);
                        }
                    }
                    else if (xe.Name.LocalName.Equals("dc_food_nutrients.dc_vit_e", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!String.IsNullOrEmpty(xe.Value))
                        {
                            food.VitaminE = Convert.ToDecimal(xe.Value);
                        }
                    }
                    else if (xe.Name.LocalName.Equals("dc_food_nutrients.dc_vit_k", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!String.IsNullOrEmpty(xe.Value))
                        {
                            food.VitaminK = Convert.ToDecimal(xe.Value);
                        }
                    }
                    else if (xe.Name.LocalName.Equals("dc_food_nutrients.dc_thiamin", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!String.IsNullOrEmpty(xe.Value))
                        {
                            food.Thiamin = Convert.ToDecimal(xe.Value);
                        }
                    }
                    else if (xe.Name.LocalName.Equals("dc_food_nutrients.dc_riboflavin", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!String.IsNullOrEmpty(xe.Value))
                        {
                            food.Riboflavin = Convert.ToDecimal(xe.Value);
                        }
                    }
                    else if (xe.Name.LocalName.Equals("dc_food_nutrients.dc_niacin", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!String.IsNullOrEmpty(xe.Value))
                        {
                            food.Niacin = Convert.ToDecimal(xe.Value);
                        }
                    }
                    else if (xe.Name.LocalName.Equals("dc_food_nutrients.dc_vit_b6", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!String.IsNullOrEmpty(xe.Value))
                        {
                            food.VitaminB6 = Convert.ToDecimal(xe.Value);
                        }
                    }
                    else if (xe.Name.LocalName.Equals("dc_food_nutrients.dc_folate", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!String.IsNullOrEmpty(xe.Value))
                        {
                            food.Folate = Convert.ToDecimal(xe.Value);
                        }
                    }
                    else if (xe.Name.LocalName.Equals("dc_food_nutrients.dc_vit_b12", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!String.IsNullOrEmpty(xe.Value))
                        {
                            food.VitaminB12 = Convert.ToDecimal(xe.Value);
                        }
                    }
                    else if (xe.Name.LocalName.Equals("dc_food_nutrients.dc_biotin", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!String.IsNullOrEmpty(xe.Value))
                        {
                            food.Biotin = Convert.ToDecimal(xe.Value);
                        }
                    }
                    else if (xe.Name.LocalName.Equals("dc_food_nutrients.dc_panto_acid", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!String.IsNullOrEmpty(xe.Value))
                        {
                            food.PantothenicAcid = Convert.ToDecimal(xe.Value);
                        }
                    }
                    else if (xe.Name.LocalName.Equals("dc_food_nutrients.dc_potassium", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!String.IsNullOrEmpty(xe.Value))
                        {
                            food.Potassium = Convert.ToDecimal(xe.Value);
                        }
                    }
                    else if (xe.Name.LocalName.Equals("dc_food_nutrients.dc_magnesium", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!String.IsNullOrEmpty(xe.Value))
                        {
                            food.Magnesium = Convert.ToDecimal(xe.Value);
                        }
                    }
                    else if (xe.Name.LocalName.Equals("dc_food_nutrients.dc_zinc", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!String.IsNullOrEmpty(xe.Value))
                        {
                            food.Zinc = Convert.ToDecimal(xe.Value);
                        }
                    }
                    else if (xe.Name.LocalName.Equals("dc_food_nutrients.dc_selenium", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!String.IsNullOrEmpty(xe.Value))
                        {
                            food.Selenium = Convert.ToDecimal(xe.Value);
                        }
                    }
                    else if (xe.Name.LocalName.Equals("dc_food_nutrients.dc_copper", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!String.IsNullOrEmpty(xe.Value))
                        {
                            food.Copper = Convert.ToDecimal(xe.Value);
                        }
                    }
                    else if (xe.Name.LocalName.Equals("dc_food_nutrients.dc_manganese", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!String.IsNullOrEmpty(xe.Value))
                        {
                            food.Manganese = Convert.ToDecimal(xe.Value);
                        }
                    }
                    else if (xe.Name.LocalName.Equals("dc_food_nutrients.dc_choline", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!String.IsNullOrEmpty(xe.Value))
                        {
                            food.Choline = Convert.ToDecimal(xe.Value);
                        }
                    }
                    else if (xe.Name.LocalName.Equals("dc_food_nutrients.dc_chromium", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!String.IsNullOrEmpty(xe.Value))
                        {
                            food.Chromium = Convert.ToDecimal(xe.Value);
                        }
                    }
                    else if (xe.Name.LocalName.Equals("dc_food_nutrients.dc_phosphorus", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!String.IsNullOrEmpty(xe.Value))
                        {
                            food.Phosphorus = Convert.ToDecimal(xe.Value);
                        }
                    }
                    else if (xe.Name.LocalName.Equals("dc_food_nutrients.dc_protein", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!String.IsNullOrEmpty(xe.Value))
                        {
                            food.Protein = Convert.ToDecimal(xe.Value);
                        }
                    }
                    else if (xe.Name.LocalName.Equals("dc_food_nutrients.dc_alcohol", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!String.IsNullOrEmpty(xe.Value))
                        {
                            food.Alcohol = Convert.ToDecimal(xe.Value);
                        }
                    }
                    else if (xe.Name.LocalName.Equals("dc_food_nutrients.dc_food_nutrientsid", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!String.IsNullOrEmpty(xe.Value))
                        {
                            food.FoodNutrientId = new Guid(xe.Value);
                        }
                    } 
                    #endregion
                }
            }
            food.RaisePropertyChanged();
            busyIndicator.IsBusy = false;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LayoutRoot.DataContext = food;
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
                    if(App.Current.GetType() == typeof(DynamicConnections.NutriStyle.MenuGenerator.App)) {
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
            }
            //load up dropdowns
            if (App.Current.GetType() == typeof(DynamicConnections.NutriStyle.MenuGenerator.App))
            {
                dc_portiontypeid.SelectedPair = new PairWithList(String.Empty, String.Empty, String.Empty, ((App)App.Current).PortionTypes);
                dc_foodgroup.SelectedPair  = new PairWithList(String.Empty, String.Empty, String.Empty, ((App)App.Current).FoodGroups);
            }
            if (FoodId != null && FoodId != Guid.Empty)
            {
                RetrieveFood(FoodId);
            }
            //Add red '*' to required labels
            foreach (String str in requiredFields)
            {
                TextBlock tb = this.GetVisualDescendants().OfType<TextBlock>().Where(txb => txb.Name == str+"Label").FirstOrDefault();
                Run r = new Run();
                r.Text = "*";
                //{StaticResource defaultFontRequiredColor}
                r.Foreground = Application.Current.Resources["defaultFontRequiredColor"] as SolidColorBrush;
                tb.Inlines.Add(r);
            }
        }

        /// <summary>
        /// Save the food.  Build an XElement document and sumbit to the webserver.  Validates the data first.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveFood(object sender, RoutedEventArgs e)
        {
            bool error = false;
            //Need to validate first
            var textBoxList = General.GetChildren<WatermarkTextBox>(FormData);
            var comboBoxList = General.GetChildren<ComboBoxWithValidation>(FormData);
            var checkboxList = General.GetChildren<CheckBox>(FormData);

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



            ingredient.GetBindingExpression(ToggleButton.IsCheckedProperty).UpdateSource();
            both.GetBindingExpression(ToggleButton.IsCheckedProperty).UpdateSource();
            //validate radio button
            if (ingredient.IsChecked.Value)
            {
                ingredient.GetBindingExpression(ToggleButton.IsCheckedProperty).UpdateSource();
            }
            if (both.IsChecked.Value)
            {
                both.GetBindingExpression(ToggleButton.IsCheckedProperty).UpdateSource();
            }

            if (!ingredient.IsChecked.Value && !both.IsChecked.Value)
            {
                ingredient.GetBindingExpression(ToggleButton.IsCheckedProperty).UpdateSource();
                both.GetBindingExpression(ToggleButton.IsCheckedProperty).UpdateSource();
            }
            if (Validation.GetHasError(ingredient) && Validation.GetHasError(both))
            {
                error = true;
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

            
            if (!error)
            {
                busyIndicator.IsBusy = true;
                XElement entities = new XElement("entities");
                //Create dc_foods_nutrient first
                XElement foodNutrients = new XElement("dc_food_nutrients");

                if (food.FoodNutrientId == null || food.FoodNutrientId == Guid.Empty)
                {
                    //Add create attribute to the foodNutrients
                    XAttribute create = new XAttribute("create", "true");
                    foodNutrients.Add(create);
                    food.FoodNutrientId = Guid.NewGuid();
                }
                
                foodNutrients.Add(new XElement("dc_food_nutrientsid", food.FoodNutrientId.ToString()));

                //Create dc_foods
                XElement foods = new XElement("dc_foods");

                if (food.FoodId != null && food.FoodId != Guid.Empty)
                {
                    foods.Add(new XElement("dc_foodsid", food.FoodId.ToString()));
                }
                //Relate to dc_food_nutrients
                XElement foodNutrientId = new XElement("dc_foodnutrientid", food.FoodNutrientId.ToString());
                XAttribute attribute = new XAttribute("entityname", "dc_food_nutrients");
                foodNutrientId.Add(attribute);
                foods.Add(foodNutrientId);
                //Add dc_name
                foods.Add(new XElement("dc_name", food.Name));
                foods.Add(new XElement("dc_sourceofinformationdateobtained", food.SourceOfInformation));
                foods.Add(new XElement("dc_canuseinmenu", food.MenuFood));
                foods.Add(new XElement("dc_unit_gram_weight", food.UnitGramWeight));
                foods.Add(new XElement("dc_portion_amount", food.PortionAmount));
                foods.Add(new XElement("dc_foodmanufacturerweb", food.FoodManufacturerWeb));
                foods.Add(new XElement("dc_foodgroup", food.FoodGroup.Id));
                //Portion Type Id
                XElement portionType = new XElement("dc_portiontypeid", food.PortionTypeId.Id);
                XAttribute portionTypeEntityName = new XAttribute("entityname", "dc_portion_types");
                portionType.Add(portionTypeEntityName);
                foods.Add(portionType);
                
                //contact Id
                XElement contactId = new XElement("dc_contactid", ((App)App.Current).contactId.ToString());
                XAttribute contactIdEntityName = new XAttribute("entityname", "contact");
                contactId.Add(contactIdEntityName);
                foods.Add(contactId);
                foods.Add(new XElement("dc_availabletoallusers", food.AvailableToAllUsers));

                foodNutrients.Add(new XElement("dc_kcals", food.Kcals));
                foodNutrients.Add(new XElement("dc_fat", food.Fat));
                foodNutrients.Add(new XElement("dc_protein", food.Protein));
                foodNutrients.Add(new XElement("dc_alcohol", food.Alcohol));
                foodNutrients.Add(new XElement("dc_fa_sat", food.SaturatedFat));
                foodNutrients.Add(new XElement("dc_fa_trans", food.TransFat));
                foodNutrients.Add(new XElement("dc_cholestrol", food.Cholesterol));
                foodNutrients.Add(new XElement("dc_sodium", food.Sodium));
                foodNutrients.Add(new XElement("dc_carbohydrate", food.Carbohydrate));
                foodNutrients.Add(new XElement("dc_fiber", food.Fiber));
                foodNutrients.Add(new XElement("dc_sugar", food.Sugar));
                foodNutrients.Add(new XElement("dc_vit_a_iu", food.VitaminA));
                foodNutrients.Add(new XElement("dc_vit_c", food.VitaminC));
                foodNutrients.Add(new XElement("dc_calcium", food.Calcium));
                foodNutrients.Add(new XElement("dc_iron", food.Iron));


                foodNutrients.Add(new XElement("dc_vit_d", food.VitaminD));
                foodNutrients.Add(new XElement("dc_vit_e", food.VitaminE));
                foodNutrients.Add(new XElement("dc_vit_k", food.VitaminK));
                foodNutrients.Add(new XElement("dc_thiamin", food.Thiamin));
                foodNutrients.Add(new XElement("dc_niacin", food.Niacin));
                foodNutrients.Add(new XElement("dc_riboflavin", food.Riboflavin));

                foodNutrients.Add(new XElement("dc_vit_b6", food.VitaminB6));
                foodNutrients.Add(new XElement("dc_folate", food.Folate));
                foodNutrients.Add(new XElement("dc_vit_b12", food.VitaminB12));
                foodNutrients.Add(new XElement("dc_biotin", food.Biotin));
                foodNutrients.Add(new XElement("dc_panto_acid", food.PantothenicAcid));
                foodNutrients.Add(new XElement("dc_potassium", food.Potassium));
                foodNutrients.Add(new XElement("dc_magnesium", food.Magnesium));
                foodNutrients.Add(new XElement("dc_zinc", food.Zinc));

                foodNutrients.Add(new XElement("dc_selenium", food.Selenium));
                foodNutrients.Add(new XElement("dc_copper", food.Copper));
                foodNutrients.Add(new XElement("dc_manganese", food.Manganese));
                foodNutrients.Add(new XElement("dc_choline", food.Choline));
                foodNutrients.Add(new XElement("dc_chromium", food.Chromium));
                foodNutrients.Add(new XElement("dc_phosphorus", food.Phosphorus));
                foods.Add(new XElement("dc_ingredientlist", food.IngredientList));

                foods.Add(new XElement("dc_corn", food.Corn));
                foods.Add(new XElement("dc_egg", food.Eggs));
                foods.Add(new XElement("dc_dairy", food.Dairy));
                foods.Add(new XElement("dc_artificialsweeteners", food.ArtificialSweeteners));
                foods.Add(new XElement("dc_peanut", food.Peanuts));
                foods.Add(new XElement("dc_caffeine", food.Caffeine));
                foods.Add(new XElement("dc_animalproduct", food.AnimalProduct));
                foods.Add(new XElement("dc_soy", food.Soy));
                foods.Add(new XElement("dc_artificialcolorsadditives", food.ArtificialColorsAdditives));

                //Add to entities node
                entities.Add(foodNutrients);
                entities.Add(foods);

                //Save
                CrmSdk.WebServicesSoapClient cms = new CrmSdk.WebServicesSoapClient(((App)App.Current).webServicesName);
                cms.CreateUpdateReturnEntitiesCompleted += new EventHandler<CrmSdk.CreateUpdateReturnEntitiesCompletedEventArgs>(cms_CreateUpdateFoodLike);
                cms.CreateUpdateReturnEntitiesAsync(entities, true);
            }
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
                SavedClicked(e.Result, new EventArgs());
                busyIndicator.IsBusy = false;
            }
            else
            {
                var nodes = e.Result.Descendants("resultset");
                SavedClicked(e.Result, new EventArgs());
                busyIndicator.IsBusy = false;
            }
        }

        private void ShowAdditionalFields(object sender, RoutedEventArgs e)
        {
            if (GridAdditionalFields.Visibility == Visibility.Collapsed)
            {
                GridAdditionalFields.Visibility = Visibility.Visible;
                FormDataHeight.Height = new GridLength(540);
            }
            else if (GridAdditionalFields.Visibility == Visibility.Visible)
            {
                GridAdditionalFields.Visibility = Visibility.Collapsed;
                FormDataHeight.Height = new GridLength(0);
            }
        }
    }
}

