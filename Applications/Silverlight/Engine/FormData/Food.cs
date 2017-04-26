using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using DynamicConnections.NutriStyle.MenuGenerator.Engine.Helpers;
using System.ComponentModel;
using System.Xml.Linq;
using System.Reflection;


namespace DynamicConnections.NutriStyle.MenuGenerator.Engine.FormData
{
    public class Food :  INotifyPropertyChanged
    {
        //dc_foods
        private String dc_name;
        private String dc_foodmanufacturerweb;
       
        private String dc_sourceofinformationdateobtained;
        private MenuFoodOptions dc_canuseinmenu;
        private AvailableToAllUsersOptions dc_availabletoallusers;

        private decimal? dc_unit_gram_weight;
        private decimal? dc_portion_amount;
        private PairWithList dc_portiontypeid;
        private PairWithList dc_foodgroup;
        
        //dc_food_nutrients
        private decimal? dc_kcals;
        private decimal? dc_fat;
        private decimal? dc_carbohydrate;
        private decimal? dc_protein;

        

        public enum MenuFoodOptions
        {
            None = 0,
            False = 1,
            True = 2
        }

        public enum AvailableToAllUsersOptions
        {
            None = 0,
            False = 1,
            True = 2
        }
        public Food()
        {
            dc_canuseinmenu = MenuFoodOptions.None;
            dc_availabletoallusers = AvailableToAllUsersOptions.None;
        }
        //public methods

        public Guid FoodId { get; set; }
        /// <summary>
        /// For dc_foods.dc_name
        /// </summary>
        public String Name
        {
            get{return(dc_name);}
            set{
                //Get label name
                String label = General.RetrieveAttributeLabelName("dc_name", (XElement)((App)App.Current).metadataList["dc_foods"]);
                if (String.IsNullOrEmpty(value) || value.Equals(label, StringComparison.OrdinalIgnoreCase))
                {
                    throw new Exception("Please enter " + label);
                }
                else
                {
                    dc_name = value;
                    OnPropertyChanged("Name");
                }
            }
        }
        /// <summary>
        /// For dc_foods.dc_foodmanufacturerweb
        /// </summary>
        public String FoodManufacturerWeb
        {
            get
            {
                return (dc_foodmanufacturerweb);
            }
            set
            {
                String label = General.RetrieveAttributeLabelName("dc_foodmanufacturerweb", (XElement)((App)App.Current).metadataList["dc_foods"]);
                if (String.IsNullOrEmpty(value) || value.Equals(label, StringComparison.OrdinalIgnoreCase))
                {
                    throw new Exception("Please enter " + label);
                }
                else
                {
                    dc_foodmanufacturerweb = value;
                    OnPropertyChanged("FoodManufacturerWeb");
                }
            }
        }

        /// <summary>
        /// For dc_foods.dc_sourceofinformationdateobtained
        /// </summary>
        public String SourceOfInformation
        {
            get { return (dc_sourceofinformationdateobtained); }
            set
            {
                String label = General.RetrieveAttributeLabelName("dc_sourceofinformationdateobtained", (XElement)((App)App.Current).metadataList["dc_foods"]);
                if (String.IsNullOrEmpty(value) || value.Equals(label, StringComparison.OrdinalIgnoreCase))
                {
                    throw new Exception("Please enter " + label);
                }
                else
                {
                    dc_sourceofinformationdateobtained = value;
                    OnPropertyChanged("SourceOfInformation");
                }
            }
        }

        /// <summary>
        /// dc_foods.dc_canuseinmenu
        /// </summary>
        public MenuFoodOptions MenuFood
        {
            get
            {
                return (dc_canuseinmenu);
            }
            set
            {
                if (value.Equals(MenuFoodOptions.None))
                {
                    throw new Exception("Please choose one");
                }
                else
                {
                    dc_canuseinmenu = value;
                }
                OnPropertyChanged("MenuFood");
            }
        }


        /// <summary>
        /// For dc_foods.dc_unit_gram_weight
        /// </summary>
        public decimal? UnitGramWeight
        {
            get { return (dc_unit_gram_weight); }
            set
            {
                String label = General.RetrieveAttributeLabelName("dc_unit_gram_weight", (XElement)((App)App.Current).metadataList["dc_foods"]);
                if (value == decimal.MinValue || value <0m )
                {
                    throw new Exception("Please enter "+label);
                }
                else
                {
                    dc_unit_gram_weight = value;
                    OnPropertyChanged("UnitGramWeight");
                }
            }
        }
        /// <summary>
        /// For dc_foods_nutrients.dc_kcals
        /// </summary>
        public decimal? Kcals
        {
            get { return (dc_kcals); }
            set
            {
                String label = General.RetrieveAttributeLabelName("dc_kcals", (XElement)((App)App.Current).metadataList["dc_food_nutrients"]);
                if (value == decimal.MinValue || value < 0m)
                {
                    throw new Exception("Please enter " + label);
                }
                else
                {
                    dc_kcals = value;
                    OnPropertyChanged("Kcals");
                }
            }
        }


        /// <summary>
        /// For dc_foods.dc_portion_amount
        /// </summary>
        public decimal? PortionAmount
        {
            get { return (dc_portion_amount); }
            set
            {
                String label = General.RetrieveAttributeLabelName("dc_portion_amount", (XElement)((App)App.Current).metadataList["dc_foods"]);
                if (value == decimal.MinValue || value < 0m)
                {
                    throw new Exception("Please enter " + label);
                }
                else
                {
                    dc_portion_amount = value;
                    OnPropertyChanged("PortionAmount");
                }
            }
        }

        /// <summary>
        /// For dc_foods.dc_foodgroup
        /// </summary>
        public PairWithList FoodGroup
        {
            get { return (dc_foodgroup); }
            set
            {
                String label = General.RetrieveAttributeLabelName("dc_foodgroup", (XElement)((App)App.Current).metadataList["dc_foods"]);
                if (value == null ||
                    (value is PairWithList && String.IsNullOrEmpty(((PairWithList)value).Name))
                )
                {
                    throw new Exception("Please enter " + label);
                }
                else
                {
                    dc_foodgroup = value;
                    OnPropertyChanged("FoodGroup");
                }
            }
        }

        /// <summary>
        /// For dc_foods.dc_portiontypeid
        /// </summary>
        public PairWithList PortionTypeId
        {
            get
            {
                return (dc_portiontypeid);
            }
            set
            {
                String label = General.RetrieveAttributeLabelName("dc_portiontypeid", (XElement)((App)App.Current).metadataList["dc_foods"]);
                if (value == null ||
                    (value is PairWithList && String.IsNullOrEmpty(((PairWithList)value).Name))
                )
                {
                    throw new Exception("Please enter " + label);
                }
                else
                {
                    dc_portiontypeid = value;
                    OnPropertyChanged("PortionTypeId");
                }
            }
        }

        /// <summary>
        /// For dc_foods_nutrients.dc_fat
        /// </summary>
        public decimal? Fat
        {
            get { return (dc_fat); }
            set
            {
                String label = General.RetrieveAttributeLabelName("dc_fat", (XElement)((App)App.Current).metadataList["dc_food_nutrients"]);
                if (value == decimal.MinValue || value < 0m)
                {
                    throw new Exception("Please enter " + label);
                }
                else
                {
                    dc_fat = value;
                    OnPropertyChanged("Fat");
                }
            }
        }
        /// <summary>
        /// For dc_foods_nutrients.dc_fa_sat
        /// </summary>
        public decimal? SaturatedFat { get; set; }

        /// <summary>
        /// For dc_foods_nutrients.dc_fa_trans
        /// </summary>
        public decimal? TransFat { get; set; }

        /// <summary>
        /// For dc_foods_nutrients.dc_cholestrol
        /// </summary>
        public decimal? Cholesterol { get; set; }

        /// <summary>
        /// For dc_foods_nutrients.dc_sodium
        /// </summary>
        public decimal? Sodium { get; set; }

        /// <summary>
        /// For dc_foods_nutrients.dc_carboydrate
        /// </summary>
        public decimal? Carbohydrate
        {
            get { return (dc_carbohydrate); }
            set
            {
                String label = General.RetrieveAttributeLabelName("dc_carbohydrate", (XElement)((App)App.Current).metadataList["dc_food_nutrients"]);
                if (value == decimal.MinValue || value < 0m)
                {
                    throw new Exception("Please enter " + label);
                }
                else
                {
                    dc_carbohydrate = value;
                    OnPropertyChanged("Carbohydrate");
                }
            }

        }
            /// <summary>
        /// For dc_foods_nutrients.dc_protein
        /// </summary>
        public decimal? Protein
        {
            get { return (dc_protein); }
            set
            {
                String label = General.RetrieveAttributeLabelName("dc_protein", (XElement)((App)App.Current).metadataList["dc_food_nutrients"]);
                if (value == decimal.MinValue || value < 0m)
                {
                    throw new Exception("Please enter " + label);
                }
                else
                {
                    dc_protein = value;
                    OnPropertyChanged("Protein");
                }
            }
        }
        /// <summary>
        /// For dc_foods_nutrients.dc_fiber
        /// </summary>
        public decimal? Fiber { get; set; }

        /// <summary>
        /// For dc_foods_nutrients.dc_suger
        /// </summary>
        public decimal? Sugar { get; set; }

        /// <summary>
        /// For dc_foods_nutrients.dc_vit_a_iu
        /// </summary>
        public decimal? VitaminA { get; set; }

        /// <summary>
        /// For dc_foods_nutrients.dc_vit_c
        /// </summary>
        public decimal? VitaminC { get; set; }

        /// <summary>
        /// For dc_foods_nutrients.dc_calcium
        /// </summary>
        public decimal? Calcium { get; set; }

        /// <summary>
        /// For dc_foods_nutrients.dc_iron
        /// </summary>
        public decimal? Iron { get; set; }


        /// <summary>
        /// dc_foods.dc_availabletoallusers
        /// </summary>
        public AvailableToAllUsersOptions AvailableToAllUsers
        {
            get
            {
                return (dc_availabletoallusers);
            }
            set
            {
                if (value.Equals(AvailableToAllUsersOptions.None))
                {
                    throw new Exception("Please choose one");
                }
                else
                {
                    dc_availabletoallusers = value;
                }
                OnPropertyChanged("AvailableToAllUsers");
            }
        }

        /// <summary>
        /// For dc_foods_nutrients.dc_vit_d
        /// </summary>
        public decimal? VitaminD { get; set; }

        /// <summary>
        /// For dc_foods_nutrients.dc_vit_e
        /// </summary>
        public decimal? VitaminE { get; set; }

        /// <summary>
        /// For dc_foods_nutrients.dc_vit_k
        /// </summary>
        public decimal? VitaminK { get; set; }

        /// <summary>
        /// For dc_foods_nutrients.dc_thiamin
        /// </summary>
        public decimal? Thiamin{ get; set; }

        /// <summary>
        /// For dc_foods_nutrients.dc_niacin
        /// </summary>
        public decimal? Niacin{ get; set; }

        /// <summary>
        /// For dc_foods_nutrients.dc_riboflavin
        /// </summary>
        public decimal? Riboflavin { get; set; }

        /// <summary>
        /// For dc_foods_nutrients.dc_vit_b6
        /// </summary>
        public decimal? VitaminB6 { get; set; }

        /// <summary>
        /// For dc_foods_nutrients.dc_folate
        /// </summary>
        public decimal? Folate { get; set; }

        /// <summary>
        /// For dc_foods_nutrients.dc_vit_b12
        /// </summary>
        public decimal? VitaminB12 { get; set; }

        /// <summary>
        /// For dc_foods_nutrients.dc_biotin
        /// </summary>
        public decimal? Biotin { get; set; }

        /// <summary>
        /// For dc_foods_nutrients.dc_panto_acid
        /// </summary>
        public decimal? PantothenicAcid { get; set; }

        /// <summary>
        /// For dc_foods_nutrients.dc_potassium
        /// </summary>
        public decimal? Potassium { get; set; }

        /// <summary>
        /// For dc_foods_nutrients.dc_magnesium
        /// </summary>
        public decimal? Magnesium { get; set; }

        /// <summary>
        /// For dc_foods_nutrients.dc_zinc
        /// </summary>
        public decimal? Zinc { get; set; }

        /// <summary>
        /// For dc_foods_nutrients.dc_selenium
        /// </summary>
        public decimal? Selenium { get; set; }

        /// <summary>
        /// For dc_foods_nutrients.dc_copper
        /// </summary>
        public decimal? Copper  { get; set; }

        /// <summary>
        /// For dc_foods_nutrients.dc_manganese
        /// </summary>
        public decimal? Manganese { get; set; }

        /// <summary>
        /// For dc_foods_nutrients.dc_choline
        /// </summary>
        public decimal? Choline { get; set; }

        /// <summary>
        /// For dc_foods_nutrients.dc_chromium
        /// </summary>
        public decimal? Chromium { get; set; }

        /// <summary>
        /// For dc_foods_nutrients.dc_phosphorus
        /// </summary>
        public decimal? Phosphorus { get; set; }

        /// <summary>
        /// For dc_foods_nutrients.dc_alcohol
        /// </summary>
        public decimal? Alcohol { get; set; }

        public Guid FoodNutrientId { get; set; }

        /// <summary>
        /// For dc_foods.dc_ingredientlist
        /// </summary>
        public String IngredientList { get; set; }

        
        /// <summary>
        /// For dc_foods.dc_corn
        /// </summary>
        public bool Corn { get; set; }

        /// <summary>
        /// For dc_foods.dc_egg
        /// </summary>
        public bool Eggs { get; set; }

        /// <summary>
        /// For dc_foods.dc_dairy
        /// </summary>
        public bool Dairy { get; set; }

        /// <summary>
        /// For dc_foods.dc_artificialsweeteners
        /// </summary>
        public bool ArtificialSweeteners { get; set; }

        /// <summary>
        /// For dc_foods.dc_soy
        /// </summary>
        public bool Soy { get; set; }

        /// <summary>
        /// For dc_foods.dc_peanut
        /// </summary>
        public bool Peanuts { get; set; }

        /// <summary>
        /// For dc_foods.dc_caffeine
        /// </summary>
        public bool Caffeine { get; set; }

        /// <summary>
        /// For dc_foods.dc_animalproduct
        /// </summary>
        public bool AnimalProduct { get; set; }

        /// <summary>
        /// For dc_foods.dc_artificialcolorsadditives
        /// </summary>
        public bool ArtificialColorsAdditives { get; set; }

        //----------------------------------------------------------//

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        /// <summary>
        /// Fire all the PropertyChangedEventHandlers
        /// </summary>
        public void RaisePropertyChanged()
        {
            Food f = new Food();
            Type t = f.GetType();
            MethodInfo[] mi = t.GetMethods();

            foreach (MethodInfo m in mi)
            {
                if (m.Name.StartsWith("set_"))
                {
                    OnPropertyChanged(m.Name.Replace("set_", String.Empty));
                }
            }
        }
    }


}
