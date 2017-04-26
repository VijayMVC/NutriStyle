using System;
using System.Collections.Generic;

using Microsoft.Xrm.Sdk;
using DynamicConnections.CRM2011.CustomAssemblies.Utilities;
using Microsoft.Xrm.Sdk.Query;
using System.ServiceModel;
using DynamicConnections.NutriStyle.CRM2011.Plugins.Helpers;

namespace DynamicConnections.NutriStyle.CRM2011.Plugins
{
    /// <summary>
    /// Only fires when the number of servings or the portion size of the food changes and the food is a recipe
    /// </summary>
    public class FoodPreUpdate : IPlugin
    {
        //private Tools tools = new Tools("nutristyle_plugins_output");
        Logger logger;
        private IOrganizationService crmService;


        private Entity food = null;

        bool isRecipe = false;

        public void Execute(IServiceProvider serviceProvider)
        {
            logger = new Logger("DEBUG", @"c:\windows\temp\nutristyle_plugins_output.txt", 1);

            IPluginExecutionContext pluginExecutionContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            // Obtain the organization service reference.
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            crmService = serviceFactory.CreateOrganizationService(pluginExecutionContext.UserId);

            try
            {
                logger.debug("----------------------------------------");
                logger.debug("FoodPreUpdate: Starting: " + pluginExecutionContext.PrimaryEntityName);

                // get the Entity contact
                food = (Entity)pluginExecutionContext.InputParameters["Target"];

                // get the Entity preContact
                Entity preFood = new Entity();
                if (pluginExecutionContext.PreEntityImages.Count > 0)
                {
                    preFood = (Entity)pluginExecutionContext.PreEntityImages["preimage"];
                }

                logger.debug("Found entities.  Starting gathering of variables");
                if (food.Contains("dc_numberofservings") || food.Contains("dc_portion_amount"))
                {
                    //Figure out unitGramWeight
                    double unitGramWeight = food.Contains("dc_unit_gram_weight") ? (double)food["dc_unit_gram_weight"] : 0d;
                    double portionSize = food.Contains("dc_portion_amount") ? (double)food["dc_portion_amount"] : 0d;
                    double numberOfServings = food.Contains("dc_numberofservings") ? (double)food["dc_numberofservings"] : 0d;
                    if (unitGramWeight == 0d)
                    {
                        unitGramWeight = preFood.Contains("dc_unit_gram_weight") ? (double)preFood["dc_unit_gram_weight"] : 0d;
                    }

                    if (food.Contains("dc_recipefood"))
                    {
                        isRecipe = (bool)food["dc_recipefood"];
                    }
                    else if (preFood.Contains("dc_recipefood"))
                    {
                        isRecipe = (bool)preFood["dc_recipefood"];
                    }

                    if (portionSize == 0d)
                    {
                        portionSize = preFood.Contains("dc_portion_amount") ? (double)preFood["dc_portion_amount"] : 0d;
                    }

                    if (numberOfServings == 0d)
                    {
                        numberOfServings = preFood.Contains("dc_numberofservings") ? (double)preFood["dc_numberofservings"] : 0d;
                    }
                    logger.debug("isRecipe: " + isRecipe);
                    logger.debug("numberOfServings: " + numberOfServings);
                    logger.debug("portionSize: " + portionSize);

                    if (numberOfServings > 0d && isRecipe)
                    {
                        Ingredient i = new Ingredient();
                        unitGramWeight = i.Rollup(crmService, food.Id, numberOfServings, portionSize, true);
                        logger.debug("unitGramWeight: " + unitGramWeight);
                        //food["dc_unit_gram_weight"] = unitGramWeight;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.error("Execute Method");
                logger.error(ex.Message);
                logger.error(ex.StackTrace);
            }
        }
        private decimal RetrieveMultipler(decimal mets) 
        {
            if (mets >= 1.6m && mets < 3)
            {
                return(.5m);
            }
            else if (mets >= 3m && mets < 6)
            {
                return(1m);
            }
            else if (mets >= 6m )
            {
                return(2m);
            }
            return (1);
        }
    }
}