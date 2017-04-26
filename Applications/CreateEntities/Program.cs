using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;


namespace DynamicConnections.NutriStyle.CRM2011.CreateEntities
{
    class Program
    {
        static void Main(string[] args)
        {
            
            //tbl_mealpattern_option
            //tbl_component_template_sel
            //tbl_meal_component
            //tbl_component_category
            //tbl_foods
            //tbl_menu_foods
            //tbl_food_nutrients
            //tbl_portion_types
          
            // +++++++++++++++++++++++++++++++++
            // coverting string to char[] to find any character
            //string a = "System.Integer";
            //string b = "R";
            //char[] c=  b.ToCharArray();
            //int d = a.IndexOfAny(c);
            //if (d != -1)
            //{
            //   Debug.WriteLine("Found");
            //}
            //else               
            //{
            //    Debug.WriteLine("No Found");
            //}
            // ++++++++++++++++++++++++++++++

            //// array table to create the entities          
            //string[] arrayTable = { "tbl_mealpattern_option", "tbl_component_template_sel", "tbl_meal_component", 
            //                                "tbl_component_category", "tbl_foods", "tbl_menu_foods", "tbl_food_nutrients", 
            //                                "tbl_portion_types" };

            string[] arrayTable = { "presets" };

            // postgres entity object
            using (Postgres postgresEntity = new Postgres())
            {
                // create the entity from the metadata table
                postgresEntity.createEntityMetada(arrayTable);
            }

            //CreateEntity c = new CreateEntity();
            //c.Execute();
        }
    }
}
