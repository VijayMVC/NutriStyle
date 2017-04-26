using System;
using System.Collections.Generic;

using DynamicConnections.CRM2011.Common.Utility;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;

namespace DynamicConnections.NutriStyle.CRM2011.MenuGenerator
{
    public class CalcRatio : General
    {
        private static Logger logger = GetLogger();

        public static decimal Execute(decimal specificNutritionValue, decimal carbohydrateValue, decimal fatValue, decimal proteinValue)                                                     
        {           
            decimal sumValue, resultRadio;
            try
            {
                sumValue = 0.0m;
                sumValue = (carbohydrateValue + fatValue + proteinValue);                
                resultRadio = (sumValue > 0) ? (specificNutritionValue / sumValue) * (decimal)100 : 0.0m;
                resultRadio = Decimal.Round(resultRadio, 2, MidpointRounding.AwayFromZero);
                return (resultRadio);
            }
            catch (Exception ex)
            {
                string errorMessageString = "An error occurred in CalcRatio - Execute method. " + "\nMessage: " + ex.Message + "\nStackTrace: " + ex.StackTrace;
                logger.error(errorMessageString);              
                resultRadio = 0.0m;
                return (resultRadio);
            }
        }// end Execute

    }
}
