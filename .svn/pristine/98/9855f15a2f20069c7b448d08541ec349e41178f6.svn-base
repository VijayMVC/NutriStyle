using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DynamicConnections.NutriStyle.CRM2011.MenuGenerator;

namespace DynamicConnections.NutriStyle.CRM2011.MenuGenerator.Helpers
{
    public class DetermineKcalsForMeal
    {
        public static decimal Execute(General.Meals meal, int numberOfSnacks)
        {
            if(numberOfSnacks == 0) 
            {
                if(meal == General.Meals.Breakfast) 
                {
                    return(30);
                }else if(meal == General.Meals.Lunch) 
                {
                    return(35);
                }
                else if (meal == General.Meals.Dinner)
                {
                    return (35);
                }
            }
            else if(numberOfSnacks == 1) 
            {
                if(meal == General.Meals.Breakfast || meal == General.Meals.Dinner || meal == General.Meals.Lunch) 
                {
                    return(30);
                }
                else if (meal == General.Meals.Morning_Snack || meal == General.Meals.Afternoon_Snack || meal == General.Meals.Evening_Snack) 
                {
                    return(10);
                }
            }
            else if(numberOfSnacks == 2) 
            {
                if(meal == General.Meals.Breakfast || meal == General.Meals.Dinner) 
                {
                    return(25);
                }else if(meal == General.Meals.Lunch) 
                {
                    return(30);
                }
                else if(meal == General.Meals.Morning_Snack || meal == General.Meals.Afternoon_Snack || meal == General.Meals.Evening_Snack) 
                {
                    return(10);
                }
            }
            else if(numberOfSnacks == 3) 
            {
                if(meal == General.Meals.Breakfast || meal == General.Meals.Lunch) 
                {
                    return(25);
                }else if(meal == General.Meals.Dinner) 
                {
                    return(25);
                }
                else if (meal == General.Meals.Morning_Snack || meal == General.Meals.Afternoon_Snack || meal == General.Meals.Evening_Snack) 
                {
                    return(8.3m);
                }
            }
            return(0m);
        }
    }
}
