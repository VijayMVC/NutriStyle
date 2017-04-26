using System;
using System.Collections.Generic;

using DynamicConnections.CRM2011.Common.Utility;
using DynamicConnections.CRM2011.Common;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using DynamicConnections.NutriStyle.CRM2011.MenuGenerator.Wrappers;

namespace DynamicConnections.NutriStyle.CRM2011.MenuGenerator
{
    public class GetFoodRatios : General
    {
        private static Logger logger = GetLogger();
        /// <summary>
        /// Set variance and ratios for mealfood entities that make of the foods for the meal
        /// </summary>
        /// <param name="meal"></param>
        /// <param name="mealFoods"></param>
        public static void Execute(Entity meal, List<MealFoodWrapper> mealFoods)
        {
            Guid dc_mealid = Guid.Empty;

            // meal nutritions
            decimal mealCarbohydrateRatio = 0.0m;
            decimal mealFatRatio = 0.0m;
            decimal mealProteinRatio = 0.0m;

            // meal food nutritions
            decimal mealFoodCarbohydrate = 0.0m;
            decimal mealFoodFat = 0.0m;
            decimal mealFoodProtein = 0.0m;

            // meal food nutritions ratio
            decimal mealFoodCarbohydrateRatio = 0.0m;
            decimal mealFoodFatRatio = 0.0m;
            decimal mealFoodProteinRatio = 0.0m;

            // meal food nutritions Variancee and ttl
            decimal mealFoodCarbohydrateVariance = 0.0m;
            decimal mealFoodFatVariance = 0.0m;
            decimal mealFoodProteinVariance = 0.0m;
            decimal mealFoodVarianceTtl = 0.0m;

            try
            {
                foreach (MealFoodWrapper mfw in mealFoods)
                {
                    Entity mealFood = mfw.MealFood;
                    // get the all nutritions dc_carbohydrate, dc_fat and dc_protein
                    mealFoodCarbohydrate = mealFood.Attributes.Contains("dc_carbohydrate") ? Convert.ToDecimal(mealFood.Attributes["dc_carbohydrate"]) : 0.0m;
                    mealFoodFat = mealFood.Attributes.Contains("dc_fat") ? Convert.ToDecimal(mealFood.Attributes["dc_fat"]) : 0.0m;
                    mealFoodProtein = mealFood.Attributes.Contains("dc_protein") ? Convert.ToDecimal(mealFood.Attributes["dc_protein"]) : 0.0m;
                    /*
                    logger.info("GetFoodRatios - Execute - mealFoodCarbohydrate = " + mealFoodCarbohydrate);
                    logger.info("GetFoodRatios - Execute - mealFoodFat = " + mealFoodFat);
                    logger.info("GetFoodRatios - Execute - mealFoodProtein = " + mealFoodProtein);
                    */
                    // calculate the meal food ratio for each nutrition
                    mealFoodCarbohydrateRatio = CalcRatio.Execute(mealFoodCarbohydrate, mealFoodCarbohydrate, mealFoodFat, mealFoodProtein);
                    mealFoodFatRatio = CalcRatio.Execute(mealFoodFat, mealFoodCarbohydrate, mealFoodFat, mealFoodProtein);
                    mealFoodProteinRatio = CalcRatio.Execute(mealFoodProtein, mealFoodCarbohydrate, mealFoodFat, mealFoodProtein);
                    
                    mealCarbohydrateRatio = meal.Attributes.Contains("dc_carbohydrate_ratio") ? Convert.ToDecimal(meal.Attributes["dc_carbohydrate_ratio"]) : 0.0m;
                    mealFatRatio = meal.Attributes.Contains("dc_fat_ratio") ? Convert.ToDecimal(meal.Attributes["dc_fat_ratio"]) : 0.0m;
                    mealProteinRatio = meal.Attributes.Contains("dc_protein_ratio") ? Convert.ToDecimal(meal.Attributes["dc_protein_ratio"]) : 0.0m;
                   
                    // calculate the meal food Variance for each nutrition
                    mealFoodCarbohydrateVariance = Math.Abs(mealFoodCarbohydrateRatio - mealCarbohydrateRatio);
                    mealFoodFatVariance = Math.Abs(mealFoodFatRatio - mealFatRatio);
                    mealFoodProteinVariance = Math.Abs(mealFoodProteinRatio - mealProteinRatio);
                    
                    // calculate the total meal food Variance
                    mealFoodVarianceTtl = mealFoodCarbohydrateVariance + mealFoodFatVariance + mealFoodProteinVariance;
                    //logger.info("GetFoodRatios - Execute - mealFoodVarianceTtl = " + mealFoodVarianceTtl);

                    //Set the values
                    mealFood["dc_proteinratio"]         = mealFoodProtein;
                    mealFood["dc_fatratio"]             = mealFoodFat;
                    mealFood["dc_carbohydrateratio"]    = mealFoodCarbohydrateRatio;

                    mealFood["dc_fatvariance"]          = mealFoodFatVariance;
                    mealFood["dc_proteinvariance"]      = mealFoodProteinVariance;
                    mealFood["dc_carbohydratevariance"] = mealFoodCarbohydrateVariance;

                    mealFood["dc_ttlvariance"]          = mealFoodVarianceTtl;
                    
                }
            }
            catch (Exception ex)
            {
                string errorMessageString = "An error occurred in GetFoodRatios - Execute method. " + "Message: " + ex.Message + " StackTrace: " + ex.StackTrace;
                logger.error(errorMessageString);
            }

        }// end Execute

    }
}
