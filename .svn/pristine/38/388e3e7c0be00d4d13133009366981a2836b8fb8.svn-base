using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using DynamicConnections.CRM2011.CustomAssemblies.Utilities;
using Microsoft.Xrm.Sdk.Query;
using System.ServiceModel;

namespace DynamicConnections.NutriStyle.CRM2011.Plugins
{
    /// <summary>
    /// Delete related food nutrient when the dc_foods entity gets deleted.  needs a preimage named 'preimage' registered against it.
    /// </summary>
    public class FoodsPostDelete : IPlugin
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
                logger.debug("FoodsPostDelete: starting: " + pluginExecutionContext.PrimaryEntityName + ": " + pluginExecutionContext.MessageName);

                //Make sure image is there
                Entity preImage = null;
                if (pluginExecutionContext.PreEntityImages.Count > 0 && pluginExecutionContext.PreEntityImages["preimage"] != null)
                {
                    preImage = (Entity)pluginExecutionContext.PreEntityImages["preimage"];
                }


                //See if added by user (dc_addedbyuser)
                if (preImage != null && preImage.Contains("dc_foodnutrientid"))
                {
                    EntityReference foodNutrient = (EntityReference)preImage["dc_foodnutrientid"];
                    logger.debug("foodNutrient.Id: " + foodNutrient.Id.ToString());
                    crmService.Delete("dc_food_nutrients", foodNutrient.Id);
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
