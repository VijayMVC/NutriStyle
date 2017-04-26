using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;


namespace DynamicConnections.NutriStyle.CRM2011.MenuGenerator.Wrappers
{
    public class MealFoodWrapper : General, IComparable<MealFoodWrapper>
    {

        public MealFoodWrapper() {
            IsFavorite = false;
        }

        public MealFoodWrapper(Entity mealFood, Entity food) {
            this.MealFood           = mealFood;
            this.Food               = food;
            portionSizeMultiplier   = 1;
            amountMultiplier        = 1;
        }
        private decimal portionSizeMultiplier;
        private decimal amountMultiplier;

        public Entity MealFood  { get; set; }
        public Entity Food      { get; set; }
        public Guid Category    { get; set; }
        public String Name      { get; set; }

        public decimal ProteinRatio         { get; set; }
        public decimal FatRatio             { get; set; }
        public decimal CarbohydrateRatio    { get; set; }
        public decimal AlcoholRatio         { get; set; }

        public Guid Id                      { get; set; }
        public bool IsFavorite              { get; set; }

        /// <summary>
        /// This is used to mulitply the initial portion size when the meal is contructed.  Defaults to 1
        /// </summary>
        public decimal PortionSizeMultiplier {
            get
            {
                return (portionSizeMultiplier);
            }
            set
            {
                portionSizeMultiplier = value;
            }
        }
        /// <summary>
        /// Amount to muliple the max amount by
        /// </summary>
        public decimal AmountMultiplier
        {
            get
            {
                return (amountMultiplier);
            }
            set
            {
                amountMultiplier = value;
            }
        }

        
        public int CompareTo(MealFoodWrapper mfw)
        {
            return this.CompareTo(mfw);
        }
        /// <summary>
        /// Perform deep copy
        /// </summary>
        /// <returns></returns>
        public static MealFoodWrapper Copy(MealFoodWrapper current)
        {
            MealFoodWrapper mfw = new MealFoodWrapper();
            mfw.MealFood = new Entity("dc_mealfood");
            mfw.Food = new Entity("dc_foods");
            mfw.Category = current.Category;
            mfw.Name = current.Name;
            mfw.ProteinRatio = current.ProteinRatio;
            mfw.FatRatio = current.FatRatio;
            mfw.CarbohydrateRatio = current.CarbohydrateRatio;
            mfw.Id = current.Id;
            mfw.IsFavorite = current.IsFavorite;

            //Deep entity copy
            foreach (KeyValuePair<String, object> at in current.MealFood.Attributes)
            {
                
                mfw.MealFood.Attributes.Add(at);
            }
            foreach (KeyValuePair<String, object> at in current.Food.Attributes)
            {
                mfw.Food.Attributes.Add(at);
            }
            return (mfw);
        }
        /// <summary>
        /// Performs deep copy of list.  Returns new list
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<MealFoodWrapper> Copy(List<MealFoodWrapper> list)
        {
            List<MealFoodWrapper> newList = new List<MealFoodWrapper>();
            foreach (MealFoodWrapper mfw2 in list)
            {
                newList.Add(MealFoodWrapper.Copy(mfw2));
            }
            return (newList);
        }
        public MealFoodWrapper(Entity food, Guid mealId, Guid foodCategoryId, decimal portionSizeMultiplier, decimal amountMultiplier, bool isFavorite)
        {
            Entity mealFood                     = new Entity("dc_mealfood");
            mealFood["dc_alcoholoriginal"]      = food.Attributes.Contains("fn.dc_alcohol") ? Convert.ToDecimal(((AliasedValue)food["fn.dc_alcohol"]).Value) : 0m;
            mealFood["dc_proteinoriginal"]      = food.Attributes.Contains("fn.dc_protein") ? Convert.ToDecimal(((AliasedValue)food["fn.dc_protein"]).Value) : 0m;
            mealFood["dc_fatoriginal"]          = food.Attributes.Contains("fn.dc_fat") ? Convert.ToDecimal(((AliasedValue)food["fn.dc_fat"]).Value) : 0m;
            mealFood["dc_carbohydrateoriginal"] = food.Attributes.Contains("fn.dc_carbohydrate") ? Convert.ToDecimal(((AliasedValue)food["fn.dc_carbohydrate"]).Value) : 0m;

            mealFood["dc_portionsize"]          = Convert.ToDecimal(food["dc_portion_amount"]) * portionSizeMultiplier;
            mealFood["dc_portiontypeid"]        = food["dc_portiontypeid"];
            mealFood["dc_portion_amount"]       = Convert.ToDecimal(food["dc_portion_amount"]);
            mealFood["dc_unit_gram_weight"]     = Convert.ToDecimal(food["dc_unit_gram_weight"]) / 100m;
            mealFood["dc_name"]                 = food["dc_name"];
            mealFood["dc_adjusted"]             = false;

            mealFood["dc_canbeadjusted"]        = true;
            mealFood["dc_includeinshoppinglist"]= true;
            mealFood["dc_foodid"]               = new EntityReference("dc_foods", food.Id);
            mealFood["dc_mealid"]               = new EntityReference("dc_meal", mealId);
            mealFood                            = DynamicConnections.NutriStyle.CRM2011.MenuGenerator.MealFood.CalculateKcal(mealFood);//calculate kcals and set fat, carb and protein
            mealFood["dc_amount_incr"]          = food.Attributes.Contains("dc_amount_incr") ? Convert.ToDecimal(food["dc_amount_incr"]) : 0m;
            mealFood["dc_min_amount"]           = food.Attributes.Contains("dc_min_amount") ? Convert.ToDecimal(food["dc_min_amount"]) : 0m;
            mealFood["dc_max_amount"]           = food.Attributes.Contains("dc_max_amount") ? Convert.ToDecimal(food["dc_max_amount"]) : 0m;

            if (((decimal)mealFood["dc_min_amount"] == (decimal)mealFood["dc_max_amount"]) && (decimal)mealFood["dc_min_amount"] > 0)
            {
                mealFood["dc_portionsize"] = Convert.ToDecimal(food["dc_portion_amount"]);
            }
            else
            {
                mealFood["dc_portionsize"] = Convert.ToDecimal(food["dc_portion_amount"])*portionSizeMultiplier;
            }

            //Set ratios
            decimal foodProtein             = food.Attributes.Contains("fn.dc_protein") ? Convert.ToDecimal(((AliasedValue)food["fn.dc_protein"]).Value) : 0m;
            decimal foodFat                 = food.Attributes.Contains("fn.dc_fat") ? Convert.ToDecimal(((AliasedValue)food["fn.dc_fat"]).Value) : 0m;
            decimal foodCarbohydrate        = food.Attributes.Contains("fn.dc_carbohydrate") ? Convert.ToDecimal(((AliasedValue)food["fn.dc_carbohydrate"]).Value) : 0m;
            decimal foodAlcohol             = food.Attributes.Contains("fn.dc_alcohol") ? Convert.ToDecimal(((AliasedValue)food["fn.dc_alcohol"]).Value) : 0m;
            if (foodAlcohol > 0)
            {
               GetLogger().debug("Food Alcohol");
            }
            decimal foodProteinRatio        = 0m;
            decimal foodFatRatio            = 0m;
            decimal foodCarbohydrateRatio   = 0m;
            decimal foodAlcoholRatio        = 0m;

            //Some foods have no macro values (fat, carb, protein).  Check for 0s
            if (foodProtein > 0 && (foodProtein + foodFat + foodCarbohydrate) > 0)
            {
                foodProteinRatio = (foodProtein / (foodProtein + foodFat + foodCarbohydrate)) * 100m;
            }
            if (foodFat> 0 && (foodProtein + foodFat + foodCarbohydrate) > 0)
            {
                foodFatRatio = (foodFat / (foodProtein + foodFat + foodCarbohydrate)) * 100m;
            }
            if (foodCarbohydrate > 0 && (foodProtein + foodFat + foodCarbohydrate) > 0)
            {
                foodCarbohydrateRatio = (foodCarbohydrate / (foodProtein + foodFat + foodCarbohydrate)) * 100m;
            }
            if (foodAlcohol > 0 && (foodProtein + foodFat + foodCarbohydrate) > 0)
            {
                foodAlcoholRatio = (foodAlcohol / (foodProtein + foodFat + foodCarbohydrate)) * 100m;
            }
            this.FatRatio               = foodFatRatio;
            this.ProteinRatio           = foodProteinRatio;
            this.CarbohydrateRatio      = foodCarbohydrateRatio;
            this.AlcoholRatio           = foodAlcoholRatio;
            this.Id                     = food.Id;
            this.PortionSizeMultiplier  = portionSizeMultiplier;
            this.AmountMultiplier       = amountMultiplier;
            this.MealFood               = mealFood;
            this.Food                   = food;
            //Set food category
            if (foodCategoryId != Guid.Empty)
            {
                this.Category           = foodCategoryId;
            }
            //set name - for debugging
            this.Name                   = (String)food["dc_name"];
            this.IsFavorite             = isFavorite;
        }
        public MealFoodWrapper(Entity food, Guid mealId, Guid foodCategoryId, decimal portionSizeMultipler, decimal amountMultiplier) :
            this(food, mealId, foodCategoryId, portionSizeMultipler, amountMultiplier, false)
        {
            
        }
    }
    public class ProteinRatioSort : IComparer<MealFoodWrapper>
    {
        public int Compare(MealFoodWrapper obj1, MealFoodWrapper obj2)
        {
            return obj2.ProteinRatio.CompareTo(obj1.ProteinRatio);
        }
    }
    public class FatRatioSort : IComparer<MealFoodWrapper>
    {
        public int Compare(MealFoodWrapper obj1, MealFoodWrapper obj2)
        {
            return obj2.FatRatio.CompareTo(obj1.FatRatio);
        }
    }
    public class CarbohydrateRatioSort : IComparer<MealFoodWrapper>
    {
        public int Compare(MealFoodWrapper obj1, MealFoodWrapper obj2)
        {
            return obj2.CarbohydrateRatio.CompareTo(obj1.CarbohydrateRatio);
        }
    }
    
    
}
