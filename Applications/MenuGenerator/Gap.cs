using System;
using System.Collections.Generic;

using DynamicConnections.CRM2011.Common.Utility;
using Microsoft.Xrm.Sdk;

namespace DynamicConnections.NutriStyle.CRM2011.MenuGenerator
{
    public class Gap : General
    {
        private static decimal carbohydrateGapPercent { get; set; }
        private static decimal fatGapPercent { get; set; }
        private static decimal proteinGapPercent { get; set; }
        
        public static Entity CalculateGap(Entity entityMeal)
        {
            try
            {
                // get carbohydrate, fat and protein gap absolute value                
                decimal carbohydrateGapDecimal = Math.Abs(Convert.ToDecimal(entityMeal["dc_carbohydrate_targets"]) - Convert.ToDecimal(entityMeal["dc_carbohydrate"]));                              
                decimal fatGapDecimal = Math.Abs(Convert.ToDecimal(entityMeal["dc_fat_targets"]) - Convert.ToDecimal(entityMeal["dc_fat"]));                              
                decimal proteinGapDecimal = Math.Abs(Convert.ToDecimal(entityMeal["dc_protein_targets"]) - Convert.ToDecimal(entityMeal["dc_protein"]));

                // calculate ratio carbohydrate, fat and protein
                /*
                decimal ratioCarbohydrateDecimal = Ratio.CalculateRatio(carbohydrateGapDecimal, carbohydrateGapDecimal, fatGapDecimal, proteinGapDecimal);                            
                decimal ratioFatDecimal = Ratio.CalculateRatio(fatGapDecimal, carbohydrateGapDecimal, fatGapDecimal, proteinGapDecimal);                                                                                                          
                decimal ratioProteinDecimal = Ratio.CalculateRatio(proteinGapDecimal, carbohydrateGapDecimal, fatGapDecimal, proteinGapDecimal);
                */
                // update Meal enitty - set carbohydrate, fat and protein gaps 
                entityMeal["dc_carbohydrate_gap"] = carbohydrateGapDecimal;
                entityMeal["dc_fat_gap"] = fatGapDecimal;
                entityMeal["dc_protein_gap"] = proteinGapDecimal;
                /*
                // update Meal enitty - set carbohydrate, fat and protein ratios 
                entityMeal["dc_carbohydrate_radio"] = ratioCarbohydrateDecimal;
                entityMeal["dc_fat_radio"] = ratioFatDecimal;
                entityMeal["dc_protein_radio"] = ratioProteinDecimal;
                */
                // update Meal enitty - set carbohydrate, fat and protein gap percents 
                // get carbohydrate, fat and protein Gap Percent  
                decimal carbohydrateGapPercentDecimal = CalculatePercent(carbohydrateGapDecimal, Convert.ToDecimal(entityMeal["dc_carbohydrate_targets"]), 2);                                               
                decimal fatGapPercentDecimal = CalculatePercent(fatGapDecimal, Convert.ToDecimal(entityMeal["dc_fat_targets"]), 2);                                               
                decimal proteinGapPercentDecimal = CalculatePercent(proteinGapDecimal, Convert.ToDecimal(entityMeal["dc_protein_targets"]), 2);     
                        
                carbohydrateGapPercent = carbohydrateGapPercentDecimal;
                fatGapPercent = fatGapPercentDecimal;
                proteinGapPercent = proteinGapPercentDecimal;

                return (entityMeal);
            }
            catch (Exception)
            {
                //Logger needed
                //errorMessageString = "An error occurred in CalculateGAP method. " + "Message: " + ex.Message + " StackTrace: " + ex.StackTrace;
                return (null);
            }
        }// end CalculateGAP
    }
}
