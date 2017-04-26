using System;
using System.Collections.Generic;

using DynamicConnections.CRM2011.Common.Utility;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using DynamicConnections.NutriStyle.CRM2011.MenuGenerator.Wrappers;

namespace DynamicConnections.NutriStyle.CRM2011.MenuGenerator
{
    class AnalyzeRatios : General
    {
        private static Logger logger = GetLogger();

        public static bool Execute(Entity meal, MealFoodWrapper mealFoodWrapper)
        {
            string stringDirection = string.Empty;
            bool adjusted = false;
            try
            {
                // run upperTarget analysis
                if (CheckPercentages.upperTarget(meal))
                {
                    stringDirection = "decrease";
                }
                else
                {
                    // run lowerTarget analysis
                    if (CheckPercentages.lowerTarget(meal))
                    {
                        stringDirection = "increase";
                    }
                }
                //logger.info("AnalyzeRatios - Execute - stringDirection = " + stringDirection);
                
                // execute the AdjustFoodPortion procedure
                adjusted = AdjustFoodPortion.Execute(mealFoodWrapper,  stringDirection);
                return (adjusted);
            }
            catch (Exception ex)
            {
                string errorMessageString = "An error occurred in AnalyzeRatios - Execute method. " + "\nMessage: " + ex.Message + "\n StackTrace: " + ex.StackTrace;
                logger.error(errorMessageString);
                return (false);
            }
        }// end Execute

    }
}
