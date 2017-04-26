using System;
using System.Collections.Generic;
using System.Configuration;

using Microsoft.Xrm.Sdk;
using DynamicConnections.CRM2011.CustomAssemblies.Utilities;

namespace DynamicConnections.NutriStyle.CRM2011.Plugins
{
    public class ContactPreUpdate_KG : IPlugin
    {      
        //Tools tools = new Tools("nutristyle_plugins_output");
        Logger logger;
        public decimal ConvertPoundToKG(int weightPound)
        {
            decimal corvertNumber = (decimal)2.2;
            decimal weightKG = (decimal)0.0;
            try
            {
                weightKG = (decimal)weightPound / corvertNumber;
                return (weightKG);
                }
            catch (Exception ex)
            {                
                logger.error("ConvertPoundToKG Method");
                logger.error(ex.Message);
                logger.error(ex.StackTrace);
                return (weightKG);
            }
        }

        public void Execute(IServiceProvider serviceProvider)
        {
            // starts        
            logger = new Logger("DEBUG", @"c:\windows\temp\nutristyle_plugins_output.txt", 1);
            try
            {
            // get the execution context from the service provider
            IPluginExecutionContext pluginExecutionContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            logger.debug("ContactPreUpdate_KG: starting: " + pluginExecutionContext.PrimaryEntityName);

            // get the organization service factory - don't need it!
            //IOrganizationServiceFactory organizationServiceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

            // get the crm organization service - don't need it!
            //IOrganizationService crmOrganizationService = organizationServiceFactory.CreateOrganizationService(pluginExecutionContext.UserId);

                if (pluginExecutionContext.InputParameters.Contains("Target")) {
                    logger.debug(" has target");
                }

                // get tge Entity object
                Entity contact = (Entity)pluginExecutionContext.InputParameters["Target"];

                // check for dc_currentweight field
                if (contact.Attributes.Contains("dc_currentweight"))
                {
                    // get the value of dc_currentweight in decimal
                    int currentwieght = (int)contact["dc_currentweight"];
                    logger.debug("currentwieght: " + currentwieght);

                    // convert pound to KG
                    decimal wieghtKG = ConvertPoundToKG(currentwieght);
                    logger.debug("wieghtKG: "+ wieghtKG);

                    // set the dc_weightkg field to wieghtKG
                    contact["dc_weightkg"] = wieghtKG;
                }
            }
            catch(Exception ex) 
            {
                logger.error("Execute Method");
                logger.error(ex.Message);
                logger.error(ex.StackTrace);
            }            
        } // end Execute

    }
    
}

   