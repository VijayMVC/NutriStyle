using System;
using System.Collections.Generic;

using Microsoft.Xrm.Sdk;
using DynamicConnections.CRM2011.CustomAssemblies.Utilities;

namespace DynamicConnections.NutriStyle.CRM2011.Plugins
{
    public class ContactPreUpdate_DEE : IPlugin
    {
        Tools tools = new Tools("nutristyle_plugins_output");
        Logger logger;

        public decimal CalculateActivityFactor(decimal restingHoursValue, 
                                                                decimal veryLightHoursValue, 
                                                                decimal lightHoursValue, 
                                                                decimal moderateHoursValue,
                                                                decimal heavyHoursValue)
        {
           

            decimal factorVeryLightHours = (decimal)1.5;
            decimal factorLightHours = (decimal)2.5;
            decimal factorModerateHours = (decimal)5.0;
            decimal factorHeavyHours = (decimal)7.0;
            decimal factor24 = (decimal)24;        
            decimal activityFactorValue = (decimal)0.0; 
            try
            {
                activityFactorValue = (restingHoursValue + 
                                                (factorVeryLightHours * veryLightHoursValue) + 
                                                (factorLightHours * lightHoursValue) +
                                                (factorModerateHours * moderateHoursValue) +
                                                (factorHeavyHours * heavyHoursValue)) / factor24;                         
                return (activityFactorValue);
            }
            catch (Exception ex)
            {
                logger.error("CalculateActivityFactor Method");
                logger.error(ex.Message);
                logger.error(ex.StackTrace);
                return (activityFactorValue);
            }
        } // end CalculateActivityFactor

        public void Execute(IServiceProvider serviceProvider)
        {
            logger = new Logger("DEBUG", @"c:\windows\temp\nutristyle_plugins_output.txt", 1);
           
            // get the execution context from the service provider
            IPluginExecutionContext pluginExecutionContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            try
            {
                logger.debug("ContactPreUpdate_DEE: starting: " + pluginExecutionContext.PrimaryEntityName+":"+pluginExecutionContext.MessageName);

                decimal restingHoursValue = (decimal)0.0;
                decimal veryLightHoursValue = (decimal)0.0;
                decimal lightHoursValue = (decimal)0.0;
                decimal moderateHoursValue = (decimal)0.0;
                decimal heavyHoursValue = (decimal)0.0;
                decimal REEValue = (decimal)0.0;

                // get the Entity contact
                Entity contact = (Entity)pluginExecutionContext.InputParameters["Target"];            

                // get the Entity preContact
                Entity preContact = new Entity();
                if (pluginExecutionContext.PreEntityImages.Contains("preimage"))
                {
                    preContact = (Entity)pluginExecutionContext.PreEntityImages["preimage"];
                }
                if (contact.Attributes.Contains("dc_restinghours") ||
                   contact.Attributes.Contains("dc_verylighthours") ||
                   contact.Attributes.Contains("dc_lighthours") ||
                   contact.Attributes.Contains("dc_moderatehours") ||
                   contact.Attributes.Contains("dc_heavyhours") ||
                   contact.Attributes.Contains("dc_ree"))                   
                {
                    // restingHoursValue
                    if (contact.Attributes.Contains("dc_restinghours"))
                    {
                        restingHoursValue = (decimal)contact["dc_restinghours"];                       
                    }
                    else if (preContact.Attributes.Contains("dc_restinghours"))
                    {
                        restingHoursValue = (decimal)preContact["dc_restinghours"];
                    }

                    logger.debug("dc_restinghours: " + restingHoursValue);

                    // veryLightHoursValue
                    if (contact.Attributes.Contains("dc_verylighthours"))
                    {
                        veryLightHoursValue = (decimal)contact["dc_verylighthours"];                       
                    }
                    else if (preContact.Attributes.Contains("dc_verylighthours"))
                    {
                        veryLightHoursValue = (decimal)preContact["dc_verylighthours"];
                    }

                    logger.debug("dc_verylighthours: " + veryLightHoursValue);

                    // lightHoursValue
                    if (contact.Attributes.Contains("dc_lighthours"))
                    {
                        lightHoursValue = (decimal)contact["dc_lighthours"];
                    }
                    else if (preContact.Attributes.Contains("dc_lighthours"))
                    {
                        lightHoursValue = (decimal)preContact["dc_lighthours"];
                    }

                    logger.debug("dc_lighthours: " + lightHoursValue);

                    // moderateHoursValue
                    if (contact.Attributes.Contains("dc_moderatehours"))
                    {
                        moderateHoursValue = (decimal)contact["dc_moderatehours"];
                    }
                    else if (preContact.Attributes.Contains("dc_moderatehours"))
                    {
                        moderateHoursValue = (decimal)preContact["dc_moderatehours"];
                    }

                    logger.debug("dc_moderatehours: " + moderateHoursValue);

                    // heavyHoursValue
                    if (contact.Attributes.Contains("dc_heavyhours"))
                    {
                        heavyHoursValue = (decimal)contact["dc_heavyhours"];
                    }
                    else if (preContact.Attributes.Contains("dc_heavyhours"))
                    {
                        heavyHoursValue = (decimal)preContact["dc_heavyhours"];
                    }

                    logger.debug("dc_heavyhours: " + heavyHoursValue);

                    // REEValue - just in case
                    if (contact.Attributes.Contains("dc_ree"))
                    {
                        REEValue = (decimal)contact["dc_ree"];
                    }
                    else if (preContact.Attributes.Contains("dc_ree"))
                    {
                        REEValue = (decimal)preContact["dc_ree"];
                    }

                    logger.debug("dc_restinghours: " + restingHoursValue);
                    logger.debug("dc_verylighthours: " + veryLightHoursValue);
                    logger.debug("dc_lighthours: " + lightHoursValue);
                    logger.debug("dc_moderatehours: " + moderateHoursValue);
                    logger.debug("dc_heavyhours: " + heavyHoursValue);
                    logger.debug("dc_ree: " + REEValue);

                    // calculate the Activity Factor
                    decimal activityFactorValue = CalculateActivityFactor(restingHoursValue, 
                                                                                                  veryLightHoursValue, 
                                                                                                  lightHoursValue, 
                                                                                                  moderateHoursValue, 
                                                                                                  heavyHoursValue);
                    logger.debug("Activity Factor: " + activityFactorValue);

                   // calculate the DEE value
                   decimal DEEValue = activityFactorValue * REEValue;

                   // set the field value dc_dee to DEEValue
                   contact["dc_dee"] = DEEValue;
                   logger.debug("dc_dee: " + DEEValue);
                }
                
            }
            catch (Exception ex)
            {
                logger.error("Execute Method: " + pluginExecutionContext.PrimaryEntityName + ":" + pluginExecutionContext.MessageName);
                logger.error(ex.Message);
                logger.error(ex.StackTrace);
            }
        } // end Execute

    }

}
