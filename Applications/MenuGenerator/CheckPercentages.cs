using System;
using System.Collections.Generic;

using DynamicConnections.CRM2011.Common.Utility;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;

namespace DynamicConnections.NutriStyle.CRM2011.MenuGenerator
{
    public class CheckPercentages : General
    {
        private static Logger logger = GetLogger();

        private static decimal carbohydrateActualsPercent { get; set; }
        private static decimal fatActualsPercent { get; set; }
        private static decimal proteinActualsPercent { get; set; }
        private static decimal kcalsActualsPercent { get; set; }
       
        private static void setActualsPercents(Entity entityMeal) 
        {
            try
            {
                // From Actuals tabs
                carbohydrateActualsPercent = Convert.ToDecimal(entityMeal["dc_carbohydrate_actuals_percent"]);
                fatActualsPercent = Convert.ToDecimal(entityMeal["dc_fat_actuals_percent"]);
                proteinActualsPercent = Convert.ToDecimal(entityMeal["dc_protein_actuals_percent"]);
                /*
                logger.info("CheckPercentages - Execute - carbohydrateActualsPercent = " + carbohydrateActualsPercent);
                logger.info("CheckPercentages - Execute - fatActualsPercent = " + fatActualsPercent);
                logger.info("CheckPercentages - Execute - proteinActualsPercent = " + proteinActualsPercent);
                */
            }
            catch (Exception ex)
            {
                string errorMessageString = "An error occurred in CheckPercentages - setActualsPercents method. " + "Message: " + ex.Message + " StackTrace: " + ex.StackTrace;
                logger.error(errorMessageString);
            }
        }

        private static void setActualsKcals(Entity entityMeal)
        {
            try
            {
                // From Actuals tabs
                kcalsActualsPercent = Convert.ToDecimal(entityMeal["dc_kcals_actuals_percent"]);           
            }
            catch (Exception ex)
            {
                string errorMessageString = "An error occurred in CheckPercentages - setActualsPercents method. " + "Message: " + ex.Message + " StackTrace: " + ex.StackTrace;
                logger.error(errorMessageString);
            }
        }

        public static int Execute(Entity meal)
        {
            bool percentOK = false;

            //0: OK foods,  1: remove foods,  2: stop adding foods
            int resultValue = 0;

            try
            {
                // calculate actuals / targets percents
                decimal carbohydratePercent = CalculatePercent(Convert.ToDecimal(meal["dc_carbohydrate_actuals"]), Convert.ToDecimal(meal["dc_carbohydrate_targets"]), 2);
                decimal fafPercent = CalculatePercent(Convert.ToDecimal(meal["dc_fat_actuals"]), Convert.ToDecimal(meal["dc_fat_targets"]), 2);
                decimal proteinPercent = CalculatePercent(Convert.ToDecimal(meal["dc_protein_actuals"]), Convert.ToDecimal(meal["dc_protein_targets"]), 2);
                /*
                logger.info("CheckPercentages - Execute - carbohydratePercent = " + carbohydratePercent);
                logger.info("CheckPercentages - Execute - fafPercent = " + fafPercent);
                logger.info("CheckPercentages - Execute - proteinPercent = " + proteinPercent);
                */
                percentOK = ((carbohydratePercent <= 90) && (fafPercent <= 90) && (proteinPercent <= 90)) ? true : false;

                if (percentOK)
                {                          
                    // set the Actuals Percents
                    setActualsPercents(meal);
                    
                    // From Targets tabs
                    decimal carbohydrateTargetsPercentDecimal = Convert.ToDecimal(meal["dc_carbohydrate_targets_percent"]);
                    decimal fatTargetsPercentDecimal = Convert.ToDecimal(meal["dc_fat_targets_percent"]);
                    decimal proteinTargetsPercentDecimal = Convert.ToDecimal(meal["dc_protein_targets_percent"]);

                    resultValue = ((carbohydrateActualsPercent <= carbohydrateTargetsPercentDecimal) &&
                                           (fatActualsPercent <= fatTargetsPercentDecimal) &&
                                           (proteinActualsPercent <= proteinTargetsPercentDecimal)) ? 0 : 2; // 0: food is good, 2: stop adding more food to the meal.
                }
                else
                {
                    //logger.debug("CheckPercentages: food percentage over 90, food will be removed");
                    resultValue = 1;                   
                }
                //logger.info("CheckPercentages - Execute - resultValue = " + resultValue);
                return (resultValue);
            }
            catch (Exception ex)
            {
                string errorMessageString = "An error occurred in CheckPercentages - Execute method. " + "Message: " + ex.Message + " StackTrace: " + ex.StackTrace;
                logger.error(errorMessageString);
                return (resultValue);
            }
        } // end Execute

        /// <summary>
        /// Checks to see if the meal marcos are insode the acceptable limites (AND)
        /// </summary>
        /// <param name="entityMeal"></param>
        /// <returns></returns>
        public static bool finalTarget(Entity entityMeal)         
        {           
            try
            {
                // set the Actuals Percents
                setActualsPercents(entityMeal);

                bool rangeValue = ((carbohydrateActualsPercent >= 85.0m && carbohydrateActualsPercent <= 115.0m) &&
                                                (fatActualsPercent >= 85.0m && fatActualsPercent <= 115.0m) &&
                                                (proteinActualsPercent >= 85.0m && proteinActualsPercent <= 115.0m)) ? true : false;
                //logger.info("CheckPercentages - finalTarget - rangeValue = " + rangeValue);
                return (rangeValue);
            }
            catch (Exception ex)
            {
                string errorMessageString = "An error occurred in CheckPercentages - finalTarget method. " + "Message: " + ex.Message + " StackTrace: " + ex.StackTrace;
                logger.error(errorMessageString);
                return (false);
            }

        } // end finalTarget

        /// <summary>
        /// Consumes a meal entity.  Checks to see if the meal entity macro percents are less than the lower limits (OR)
        /// </summary>
        /// <param name="entityMeal"></param>
        /// <returns></returns>
        public static bool lowerTarget(Entity entityMeal)         
        {
            try
            {
                // set the Actuals Percents
                setActualsPercents(entityMeal);
            
                bool rangeValue = ((carbohydrateActualsPercent < 85.0m) || 
                                               (fatActualsPercent < 85.0m) || 
                                               (proteinActualsPercent < 85.0m)) ? true : false;
                //logger.info("CheckPercentages - lowerTarget - rangeValue = " + rangeValue);
                return (rangeValue);
            }
            catch (Exception ex)
            {
                string errorMessageString = "An error occurred in CheckPercentages - lowerTarget method. " + "Message: " + ex.Message + " StackTrace: " + ex.StackTrace;
                logger.error(errorMessageString);
                return (false);
            }
        }// end lowerTarget
        /// <summary>
        /// Consumes a meal entity.  Checks to see if the meal entity macro percents are greater than the upper limits (OR)
        /// </summary>
        /// <param name="entityMeal"></param>
        /// <returns></returns>
        public static bool upperTarget(Entity entityMeal)
        {
            try
            {
                // set the Actuals Percents
                setActualsPercents(entityMeal);

                bool rangeValue = ((carbohydrateActualsPercent >= 115.0m) || 
                                               (fatActualsPercent >= 115.0m) || 
                                               (proteinActualsPercent >= 115.0m)) ? true : false;
                //logger.info("CheckPercentages - upperTarget - rangeValue = " + rangeValue);
                return (rangeValue);
            }
            catch (Exception ex)
            {
                string errorMessageString = "An error occurred in CheckPercentages - upperTarget method. " + "Message: " + ex.Message + " StackTrace: " + ex.StackTrace;
                logger.error(errorMessageString);
                return (false);
            }
        }// end upperTarget

      
        public static bool snacksUpperTarget(Entity entityMeal)
        {
            try
            {
                // get Actuals Kcals
                setActualsKcals(entityMeal);

                bool resultValue = kcalsActualsPercent >= 155.0m ? true : false;
                //logger.info("CheckPercentages - snacksUpperTarget - rangeValue = " + resultValue);
                return (resultValue);        
            }
            catch (Exception ex)
            {
                string errorMessageString = "An error occurred in CheckPercentages - snacksUpperTarget method. " + "Message: " + ex.Message + " StackTrace: " + ex.StackTrace;
                logger.error(errorMessageString);
                return (false);
            }    
        }// end snacksUpperTarget

        public static bool snacksLowerTarget(Entity entityMeal)
        {
            try
            {
                // get Actuals Kcals
                setActualsKcals(entityMeal);

                bool resultValue = kcalsActualsPercent <= 85.0m ? true : false;
                //logger.info("CheckPercentages - snacksLowerTarget - rangeValue = " + resultValue);
                return (resultValue);
            }
            catch (Exception ex)
            {
                string errorMessageString = "An error occurred in CheckPercentages - snacksLowerTarget method. " + "Message: " + ex.Message + " StackTrace: " + ex.StackTrace;
                logger.error(errorMessageString);
                return (false);
            }
        }// end snacksLowerTarget

    }
}
