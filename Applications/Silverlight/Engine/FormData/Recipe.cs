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
    public class Recipe :  INotifyPropertyChanged
    {
        //dc_foods
        private String dc_name;
        private String dc_sourceofinformationdateobtained;
        private AvailableToAllUsersOptions dc_availabletoallusers;

        
        private decimal dc_portion_amount;
        private PairWithList dc_portiontypeid;

        private int dc_preparationtime;
        private String dc_directions;
        private decimal dc_numberofservings;

        public enum AvailableToAllUsersOptions
        {
            None = 0,
            False = 1,
            True = 2
        }
        public Recipe()
        {
            dc_availabletoallusers = AvailableToAllUsersOptions.None;
        }
        //public methods

        public Guid FoodId { get; set; }
        
        public Guid FoodNutrientId { get; set; }

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
        /// For dc_foods.dc_directions
        /// </summary>
        public String Directions
        {
            get { return (dc_directions); }
            set
            {
                //Get label name
                String label = General.RetrieveAttributeLabelName("dc_directions", (XElement)((App)App.Current).metadataList["dc_foods"]);
                if (String.IsNullOrEmpty(value) || value.Equals(label, StringComparison.OrdinalIgnoreCase))
                {
                    throw new Exception("Please enter " + label);
                }
                else
                {
                    dc_directions = value;
                    OnPropertyChanged("Directions");
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
        /// For dc_foods.dc_preparationtime
        /// </summary>
        public int PreparationTime
        {
            get { return (dc_preparationtime); }
            set
            {
                String label = General.RetrieveAttributeLabelName("dc_preparationtime", (XElement)((App)App.Current).metadataList["dc_foods"]);
                if (value == int.MinValue || value < 0)
                {
                    throw new Exception("Please enter " + label);
                }
                else
                {
                    dc_preparationtime = value;
                    OnPropertyChanged("PreparationTime");
                }
            }
        }

        /// <summary>
        /// For dc_foods.dc_portion_amount
        /// </summary>
        public decimal PortionAmount
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
        /// For dc_foods.dc_numberofservings
        /// </summary>
        public decimal NumberOfServings
        {
            get { return (dc_numberofservings); }
            set
            {
                String label = General.RetrieveAttributeLabelName("dc_portion_amount", (XElement)((App)App.Current).metadataList["dc_foods"]);
                if (value == decimal.MinValue || value < 0m)
                {
                    throw new Exception("Please enter " + label);
                }
                else
                {
                    dc_numberofservings = value;
                    OnPropertyChanged("NumberOfServings");
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

        public SortableCollectionView IngredientData { get; set; }


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
