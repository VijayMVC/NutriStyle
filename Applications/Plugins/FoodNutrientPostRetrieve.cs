using System;
using System.Collections.Generic;

using Microsoft.Xrm.Sdk;
using DynamicConnections.CRM2011.CustomAssemblies.Utilities;
using Microsoft.Xrm.Sdk.Query;

namespace DynamicConnections.NutriStyle.CRM2011.Plugins
{
    /// <summary>
    /// Plugin pulls values to populate fields when looking at a specific food.
    /// 
    /// 
    /// </summary>
    public class FoodNutrientPostRetrieve : IPlugin
    {
        //Tools tools = new Tools("nutristyle_plugins_output");
        Logger logger;

        
        public void Execute(IServiceProvider serviceProvider)
        {
            logger = new Logger("DEBUG", @"c:\windows\temp\nutristyle_plugins_output.txt", 1);

            try
            {
                // get the execution context from the service provider
                IPluginExecutionContext pluginExecutionContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

                // Obtain the organization service reference.
                IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService crmService = serviceFactory.CreateOrganizationService(pluginExecutionContext.UserId);

                String name = String.Empty;
                EntityReference portionTypeId = null;
                double portionAmount = 0d;
                double unitGramWeight = 0d;


                logger.debug("FoodNutrientPostRetrieve: starting: " + pluginExecutionContext.PrimaryEntityName);
                //logger.debug("test");
                //Find the guid that draws the relationship back to the dc_food entity (dc_foodnutrientid)
                Entity foodNutrient = (Entity)pluginExecutionContext.OutputParameters["BusinessEntity"];
                Guid dc_foodnutrientid = Guid.Empty;
                /*
                if (foodNutrient.Contains("dc_foodnutrientid"))
                {
                    dc_foodnutrientid = foodNutrient.Attributes["dc_foodnutrientid"] == null ? Guid.Empty : (Guid)foodNutrient.Attributes["dc_foodnutrientid"];
                }
                */
                DataCollection<Entity> list = CrmHelper.GetEntitiesByAttribute("dc_foods", "dc_foodnutrientid", foodNutrient.Id, new ColumnSet(new String[] { "dc_name", "dc_portiontypeid", "dc_portion_amount", "dc_unit_gram_weight" }), crmService);//need to add other three attributes
                

                if (list != null && list.Count > 0)
                {
                    Entity food = list[0];
                    if (food.Contains("dc_name"))
                    {
                        name = food.Attributes["dc_name"] == null ? String.Empty : (String)food.Attributes["dc_name"];
                    }
                    else
                    {
                        logger.debug("Not able to find related food (dc_foods)");
                    }
                    if (food.Contains("dc_portiontypeid"))
                    {
                        //logger.debug("type:" + food.Attributes["dc_portiontypeid"].GetType());
                        portionTypeId = food.Attributes["dc_portiontypeid"] == null ? null : (EntityReference)food.Attributes["dc_portiontypeid"];
                    }
                    else
                    {
                        logger.debug("Not able to find related food (dc_portiontypeid)");
                    }
                    if (food.Contains("dc_portion_amount"))
                    {
                        //logger.debug("type:" + food.Attributes["dc_portion_amount"].GetType());
                        portionAmount = food.Attributes["dc_portion_amount"] == null ? 0.0 : (double)food.Attributes["dc_portion_amount"];
                    }
                    else
                    {
                        //logger.debug("type:" + food.Attributes["dc_portiontypeid"].GetType());
                        logger.debug("Not able to find related food (dc_portiontypeid)");
                    }
                    if (food.Contains("dc_unit_gram_weight"))
                    {
                        unitGramWeight = food.Attributes["dc_unit_gram_weight"] == null ? 0.0 : (double)food.Attributes["dc_unit_gram_weight"];
                    }
                    else
                    {
                        logger.debug("Not able to find related food (dc_unit_gram_weight)");
                    }
                }
               
                //logger.debug("Name: " + name );
                //logger.debug("Portion Type ID: " + portionTypeId.Id+":"+portionTypeId.LogicalName+":"+portionTypeId.Name);
                //logger.debug("Portion Amount: " + portionAmount );
                //logger.debug("Unit Gram Weight: " + unitGramWeight );

                //update retreived entity
                if (!String.IsNullOrEmpty(name))
                {
                    foodNutrient["dc_name"] = name;
                }
                if (portionTypeId != null)
                {
                    foodNutrient["dc_portiontypeid"] = portionTypeId;
                }
                if (portionAmount != 0d)
                {
                    foodNutrient["dc_portion_amount"] = portionAmount;
                }
                if (unitGramWeight != 0d)
                {
                    foodNutrient["dc_unit_gram_weight"] = unitGramWeight;
                }

            }
            catch (Exception ex)
            {
                logger.error("Execute Method");
                logger.error(ex.Message);
                logger.error(ex.StackTrace);
            }

        }



    }
}
