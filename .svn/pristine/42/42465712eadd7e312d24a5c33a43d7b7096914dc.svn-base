using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Crm.Sdk.Messages;
using DynamicConnections.CRM2011.CustomAssemblies.Utilities;
using Microsoft.Xrm.Sdk.Query;
using System.ServiceModel;

namespace DynamicConnections.NutriStyle.CRM2011.Plugins
{
    public class FoodStateChangePost : IPlugin
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
                logger.debug("Preset: starting: " + pluginExecutionContext.PrimaryEntityName + ": " + pluginExecutionContext.MessageName);
                EntityReference target = (EntityReference)pluginExecutionContext.InputParameters["EntityMoniker"];
                Entity postImage = crmService.Retrieve("dc_foods", target.Id, new ColumnSet(new String[] { "dc_name", "dc_foodnutrientid" }));

                if (postImage.Contains("dc_foodnutrientid") && pluginExecutionContext.InputParameters.Contains("State"))
                {
                    /*SetStateRequest ssreq = new SetStateRequest();
                    ssreq.EntityMoniker = new EntityReference(target.LogicalName, target.Id);
                    ssreq.State = new OptionSetValue(-1);
                    ssreq.Status = new OptionSetValue(-1);*/
                    //logger.debug("Test: State = " + ssreq.State.Value + " Status = " + ssreq.Status.Value);
                    logger.debug("Test: State = " + pluginExecutionContext.InputParameters["State"]);

                    Guid nutrientId = ((EntityReference)postImage["dc_foodnutrientid"]).Id;
                    Entity nutrient = crmService.Retrieve("dc_food_nutrients", nutrientId, new ColumnSet(new String[] { "dc_name" }));

                    logger.debug("Retrieved: Nutrient = " + nutrient.LogicalName + ". Updating state to match the food");

                    SetStateRequest nutrientReq = new SetStateRequest();
                    nutrientReq.EntityMoniker = new EntityReference(nutrient.LogicalName, nutrientId);
                    nutrientReq.State = (OptionSetValue)pluginExecutionContext.InputParameters["State"];
                    nutrientReq.Status = new OptionSetValue(-1);
                    logger.debug("Set: Nutrient State: " + nutrientReq.State + ". Nutrient Status: " + nutrientReq.Status);
                    crmService.Execute(nutrientReq);
                    logger.debug("Result: State Changed, the nutrient State has been updated.");
                }
                else
                {
                    logger.debug("Result: Food does not contain a nutrient");
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
