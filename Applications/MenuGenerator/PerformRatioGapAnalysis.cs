using System;
using System.Collections.Generic;

using DynamicConnections.CRM2011.Common.Utility;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using DynamicConnections.NutriStyle.CRM2011.MenuGenerator.Wrappers;

namespace DynamicConnections.NutriStyle.CRM2011.MenuGenerator
{
    public class PerformRatioGapAnalysis : General
    {
        private static Logger logger = GetLogger();
        /// <summary>
        /// Returns true if meal is successfully generated
        /// </summary>
        /// <param name="meal"></param>
        /// <param name="mealFoods"></param>
        /// <param name="organizationServiceProxy"></param>
        /// <returns></returns>
        public static bool Execute(Entity meal, List<MealFoodWrapper> mealFoods)
        {
            bool success = false;
            bool adjust = true;
            bool adjusted = false;
            try
            {
                while (adjust)
                {
                    if (adjusted) 
                    {
                        //Update meal totals
                        meal = Meal.UpdateTotals(meal, mealFoods);
                        // gap calculations  
                        meal = CalcGAP.Execute(meal);
                        // food ratio calculations
                        GetFoodRatios.Execute(meal, mealFoods);                
                    }

                    if (CheckPercentages.finalTarget(meal))
                    {
                        logger.debug("PerformRatioGapAnalysis: Meal is OK.  Stop Adjusting");
                        success = true;
                        adjust = false;
                    }
                    else  //Need to adjust something
                    {
                        MealFoodWrapper mealFoodWrapper = GetFoodToAnalyze(mealFoods);//Get the food with the lowest ttl_var that has not been adjusted yet.
                        
                        if (mealFoodWrapper != null)
                        {
                            adjusted = ExecuteAnalyzeFood(meal, mealFoodWrapper); //Adjust food portion size
                        }
                        else//More more foods can be adjusted.  Break out of loop
                        {
                            adjust = false;
                            //logger.debug("PerformRatioGapAnalysis: no adustments have been made.  Stopping the adjustment cycle.");
                        }
                    }
                }// end while            
            } 
            catch (Exception ex)
            {
                string errorMessageString = "An error occurred in PerformRatioGapAnalysis - Execute method. " + "Message: " + ex.Message + " StackTrace: " + ex.StackTrace;
                logger.error(errorMessageString);            
            }
            return (success);
        }// end Execute
        /// <summary>
        /// Calls AnalyzeRatios.  Adjusts food portion size if needed
        /// </summary>
        /// <param name="meal"></param>
        /// <param name="mealFood"></param>
        /// <param name="organizationServiceProxy"></param>
        /// <returns></returns>
        private static bool ExecuteAnalyzeFood(Entity meal, MealFoodWrapper mealFoodWrapper)
        {               
            try
            {
                bool substitute = AnalyzeRatios.Execute(meal, mealFoodWrapper);//True means food as been adjusted
                // any upgrade to mealFood?
                return (substitute);
            }
            catch (Exception ex)
            {
                string errorMessageString = "An error occurred in PerformRatioGapAnalysis - ExecuteAnalyzeFood method. " + "Message: " + ex.Message + " StackTrace: " + ex.StackTrace;
                logger.error(errorMessageString);        
                return (false);
            }                 
        }// end ExecuteAnalyzeFood
        /// <summary>
        /// Returns next food to be adjusted.  Returns null when there are no more foods left
        /// </summary>
        /// <param name="mealFoods"></param>
        /// <returns></returns>
        private static MealFoodWrapper GetFoodToAnalyze(List<MealFoodWrapper> mealFoods) 
        {
            try
            {
                MealFoodWrapper wrapper = null;
                //Order the list by Var_TTL (dc_ttlvariance)
                decimal lowTTL = 10000;
                foreach (MealFoodWrapper mfw in mealFoods)
                {
                    Entity mealFood = mfw.MealFood;
                    if (!Convert.ToBoolean(mealFood["dc_adjusted"]) && Convert.ToBoolean(mealFood["dc_canbeadjusted"]))
                    {
                        //check ttl
                        decimal ttl = Convert.ToDecimal(mealFood["dc_ttlvariance"]);
                        if (ttl < lowTTL)
                        {
                            wrapper = mfw;
                            lowTTL = ttl;
                        }
                    }
                }
                return (wrapper);
            }
            catch (Exception ex)
            {
                string errorMessageString = "An error occurred in GetFoodToAnalyze - GetFoodToAnalyze method. " + "Message: " + ex.Message + " StackTrace: " + ex.StackTrace;
                logger.error(errorMessageString);
                return (null);
            }
        }// end GetFoodToAnalyze

    }
}
