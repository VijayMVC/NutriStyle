using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using DynamicConnections.CRM2011.CustomAssemblies.Utilities;
using DynamicConnections.NutriStyle.CRM2011.Plugins.Helpers;
using Microsoft.Xrm.Sdk.Query;
using System.ServiceModel;

namespace DynamicConnections.NutriStyle.CRM2011.Plugins
{
    public class IngredientRollupPostUpdate : IPlugin
    {
        //Tools tools = new Tools("nutristyle_plugins_output");
        Logger logger;

        public void Execute(IServiceProvider serviceProvider)
        {
            logger = new Logger("DEBUG", @"c:\windows\temp\nutristyle_plugins_output.txt", 1);
            try
            {
                // get the execution context from the service provider temp code to makea  change
                IPluginExecutionContext pluginExecutionContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

                // Obtain the organization service reference.
                IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService crmService = serviceFactory.CreateOrganizationService(pluginExecutionContext.UserId);
                logger.debug("Preset: starting: " + pluginExecutionContext.PrimaryEntityName + ":" + pluginExecutionContext.MessageName);

                // get the Entity postIngredient
                Entity postIngredient = new Entity();

                if (pluginExecutionContext.MessageName != "Delete")
                {
                    if (pluginExecutionContext.PostEntityImages.Count > 0)
                    {
                        postIngredient = (Entity)pluginExecutionContext.PostEntityImages["postimage"];
                        logger.debug("Retrieved postimage.");
                    }
                }
                else
                {
                    if (pluginExecutionContext.PreEntityImages.Count > 0)
                    {
                        postIngredient = (Entity)pluginExecutionContext.PreEntityImages["preimage"];
                        logger.debug("Retrieved preimage.");
                    }
                }



                Guid parentFoodId               = Guid.Empty;
                double numberOfServings         = 0d;
                double unitGramWeight           = 0d;
                double portionAmount            = 0d;

                if (postIngredient.Contains("dc_foodid"))
                {
                    parentFoodId = ((EntityReference)postIngredient["dc_foodid"]).Id;
                }
                logger.debug("parentFoodId: " + parentFoodId.ToString());
                
                Entity parentFoodEntity = crmService.Retrieve("dc_foods", parentFoodId, new ColumnSet(new String[] {"dc_unit_gram_weight", "dc_numberofservings", "dc_portion_amount"}));

                unitGramWeight      = parentFoodEntity.Contains("dc_unit_gram_weight") ? (double)parentFoodEntity["dc_unit_gram_weight"] : 0d;
                numberOfServings    = parentFoodEntity.Contains("dc_numberofservings") ? (double)parentFoodEntity["dc_numberofservings"] : 0d;
                portionAmount       = parentFoodEntity.Contains("dc_portion_amount") ? (double)parentFoodEntity["dc_portion_amount"] : 0d;


                logger.debug("unitGramWeight: " + unitGramWeight);
                //logger.debug("numberOfServings: " + numberOfServings);

                Ingredient i = new Ingredient();
                i.Rollup(crmService, parentFoodId, numberOfServings, portionAmount, true);

                crmService.Update(parentFoodEntity);
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
