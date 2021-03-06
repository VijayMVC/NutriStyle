﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using DynamicConnections.CRM2011.Common.Utility;
using DynamicConnections.NutriStyle.CRM2011.MenuGenerator.Wrappers;

namespace DynamicConnections.NutriStyle.CRM2011.MenuGenerator
{
    class AdjustSnacks : General
    {
        private static Logger logger = GetLogger();

        public AdjustSnacks()
        {

        }

        public static void Execute(Entity meal, List<MealFoodWrapper> mealFoods)
        {
            try
            {
                int adjustAgain = -1;
                while (adjustAgain != 0)
                {
                    decimal score = Meal.SnackScore(meal);
                    logger.debug("AdjustSnacks.execute(): score: " + score);
                    adjustAgain = Adjust(meal, mealFoods);
                    logger.debug("AdjustSnacks: adjustAgain value: " + adjustAgain);
                    if (Meal.SnackScore(meal) > score)
                    {
                        Adjust(meal, mealFoods);
                        adjustAgain = 0;
                    }
                }
            }
            catch (Exception e)
            {

                logger.debug(e.Message);
                logger.debug(e.StackTrace);
            }
        }
        /// <summary>
        /// Adjust the snacks.  Increase or decrease
        /// </summary>
        /// <param name="meal"></param>
        /// <param name="mealFoods"></param>
        /// <returns></returns>
        private static int Adjust(Entity meal, List<MealFoodWrapper> mealFoods)
        {
            int adjustAgain = 0;
            foreach (MealFoodWrapper mfw in mealFoods)
            {
                if (CheckPercentages.upperTarget(meal)) //decrease
                {
                    bool b = AdjustFoodPortion.Execute(mfw, "decrease");
                    if (b) { adjustAgain = 1; }
                    Meal.UpdateTotals(meal, mealFoods);
                }
                else if (CheckPercentages.lowerTarget(meal))//increase
                {
                    bool b = AdjustFoodPortion.Execute(mfw, "increase");
                    if (b) { adjustAgain = 1; }
                    Meal.UpdateTotals(meal, mealFoods);
                }
            }
            return (adjustAgain);
            /*
            for (int x = 0; x < meal.size(); x++)
            {
                MealComponent mc = (MealComponent)meal.getComponent(x);
                CheckPercentages cp = new CheckPercentages();
                cp.setMealValues(meal.getMealValues());
                if (cp.snacksUpperTarget())
                {
                    AdjustFoodPortion adjustFoodPortion = new AdjustFoodPortion();
                    logger.debug("AdjustSnacks: Decreasing " + mc.getFood());
                    adjustFoodPortion.setMeal(meal);
                    adjustFoodPortion.setMealComponent(mc);
                    boolean b = adjustFoodPortion.execute(meal.getDayNbr(), meal.getMeal_ID(), mc.getFood_ID(), "decrease");
                    if (b) { adjustAgain = 1; }
                    meal = adjustFoodPortion.getMeal();
                    mc = adjustFoodPortion.getMealComponent();
                }
                else if (cp.snacksLowerTarget())
                {
                    AdjustFoodPortion adjustFoodPortion = new AdjustFoodPortion();
                    logger.debug("AdjustSnacks: Increasing " + mc.getFood());
                    adjustFoodPortion.setMeal(meal);
                    adjustFoodPortion.setMealComponent(mc);
                    //adjustResult = adjustFoodPortion.execute(meal.getDayNbr(), meal.getMeal_ID(), mc.getFood_ID(), "increase");
                    boolean b = adjustFoodPortion.execute(meal.getDayNbr(), meal.getMeal_ID(), mc.getFood_ID(), "increase");
                    if (b) { adjustAgain = 1; }

                    meal = adjustFoodPortion.getMeal();
                    mc = adjustFoodPortion.getMealComponent();

                }

                //adjustAgain = adjustAgain + adjustResult;
            }//end of for
            */
            
        }//End of adjust
    }
}
