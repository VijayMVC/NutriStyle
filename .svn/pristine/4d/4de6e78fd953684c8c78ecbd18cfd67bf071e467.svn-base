using System;
using System.Collections.Generic;

using DynamicConnections.CRM2011.Common.Utility;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;

namespace DynamicConnections.NutriStyle.CRM2011.MenuGenerator
{
    public class CalcGAP : General
    {
        private static Logger logger = GetLogger();
        /// <summary>
        /// Used for setting the various attributes around the meal.  Ratios and gaps
        /// </summary>
        /// <param name="entityMeal"></param>
        /// <returns></returns>
        public static Entity Execute(Entity meal)
        {            
            try
            {
                 decimal carbohydrateGap = 0.0m;
                 decimal fatGap = 0.0m;
                 decimal proteinGap = 0.0m;

                 decimal carbohydrateGapPercentDecimal = 0.0m;
                 decimal fatGapPercentDecimal = 0.0m;
                 decimal proteinGapPercentDecimal = 0.0m;

                 // get carbohydrate, fat and protein gap absolute value                
                 carbohydrateGap = Math.Abs(Convert.ToDecimal(meal["dc_carbohydrate_targets"]) - Convert.ToDecimal(meal["dc_carbohydrate_actuals"]));
                 fatGap = Math.Abs(Convert.ToDecimal(meal["dc_fat_targets"]) - Convert.ToDecimal(meal["dc_fat_actuals"]));
                 proteinGap = Math.Abs(Convert.ToDecimal(meal["dc_protein_targets"]) - Convert.ToDecimal(meal["dc_protein_actuals"]));
                
                // calculate ratio carbohydrate, fat and protein
                decimal carbohydrateRatio = CalcRatio.Execute(carbohydrateGap, carbohydrateGap, fatGap, proteinGap);
                decimal fatRatio = CalcRatio.Execute(fatGap, carbohydrateGap, fatGap, proteinGap);
                decimal proteinRatio = CalcRatio.Execute(proteinGap, carbohydrateGap, fatGap, proteinGap);
                
                // update Meal enitty - set carbohydrate, fat and protein gaps 
                meal["dc_carbohydrate_gaps"] = carbohydrateGap;
                meal["dc_fat_gaps"] = fatGap;
                meal["dc_protein_gaps"] = proteinGap;

                // update Meal entity - set carbohydrate, fat and protein ratios 
                meal["dc_carbohydrate_ratio"] = carbohydrateRatio;
                meal["dc_fat_ratio"] = fatRatio;
                meal["dc_protein_ratio"] = proteinRatio;

                // update Meal enitty - set carbohydrate, fat and protein gap percents 
                // get carbohydrate, fat and protein Gap Percent  
                carbohydrateGapPercentDecimal = CalculatePercent(carbohydrateGap, Convert.ToDecimal(meal["dc_carbohydrate_targets"]), 2);
                fatGapPercentDecimal = CalculatePercent(fatGap, Convert.ToDecimal(meal["dc_fat_targets"]), 2);
                proteinGapPercentDecimal = CalculatePercent(proteinGap, Convert.ToDecimal(meal["dc_protein_targets"]), 2);
               
                // update Meal enitty - set carbohydrate, fat and protein ratios                
                meal["dc_carbohydrate_gaps_percent"] = carbohydrateGapPercentDecimal;
                meal["dc_fat_gaps_percent"] = fatGapPercentDecimal;
                meal["dc_protein_gaps_percent"] = proteinGapPercentDecimal;

                return (meal);
            }
            catch (Exception ex)
            {
                string errorMessageString = "An error occurred in CalcGAP - Execute method. " + "Message: " + ex.Message + " StackTrace: " + ex.StackTrace;
                logger.error(errorMessageString);
                return (null);
            }
        }// end CalculateGAP
    }
}
