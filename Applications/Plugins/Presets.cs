using System;
using System.Collections.Generic;

using Microsoft.Xrm.Sdk;
using DynamicConnections.CRM2011.CustomAssemblies.Utilities;
using Microsoft.Xrm.Sdk.Query;


namespace DynamicConnections.NutriStyle.CRM2011.Plugins
{
    /// <summary>
    /// Plugin pulls values from presets.  If the total percentage is not 100%, calculate the difference.
    /// 
    /// 
    /// </summary>
    public class Presets : IPlugin
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

                logger.debug("Preset: starting: " + pluginExecutionContext.PrimaryEntityName + ":" + pluginExecutionContext.MessageName);

                Entity presets;

                double breakfast = 0d;
                double lunch = 0d;
                double dinner = 0d;
                double fat = 0d;
                double cho = 0d;
                double pro = 0d;
                double missingPercent = 0d;
               
                if (pluginExecutionContext.InputParameters.Contains("Target") && pluginExecutionContext.InputParameters["Target"] is Entity)
                {

                    // Obtain the target business entity from the input parmameters.
                    presets = (Entity)pluginExecutionContext.InputParameters["Target"];

                }
                else
                {
                    return;
                }

                if (presets.Attributes.Contains("dc_breakfast_percent") && presets.Attributes.Contains("dc_lunch_percent") && presets.Attributes.Contains("dc_dinner_percent"))
                {
                    logger.debug("All three meals contain percentages");
                }
                else
                {

                    if (presets.Attributes.Contains("dc_breakfast_percent"))
                    {
                        //logger.debug("breakfast percent: " + presets.Attributes["dc_breakfast_percent"].GetType());
                        breakfast = Convert.ToDouble((decimal)presets.Attributes["dc_breakfast_percent"]);
                    }

                    else
                    {
                        logger.debug("No breakfast percentage");


                    }
                    if (presets.Attributes.Contains("dc_lunch_percent"))
                    {
                        //logger.debug("lunch percent: " + presets.Attributes["dc_lunch_percent"].GetType());
                        lunch =  Convert.ToDouble((decimal)presets.Attributes["dc_lunch_percent"]);
                    }

                    else
                    {
                        logger.debug("No lunch percentage");

                    }
                    if (presets.Attributes.Contains("dc_dinner_percent"))
                    {
                        //logger.debug("dinner percent: " + presets.Attributes["dc_dinner_percent"].GetType());
                        dinner =  Convert.ToDouble((decimal)presets.Attributes["dc_dinner_percent"]);
                    }

                    else
                    {
                        logger.debug("No dinner percentage");

                    }
                    missingPercent = 100 - (breakfast + lunch + dinner);
                    logger.debug("missing percent: " + missingPercent);

                    if(breakfast != 0 && lunch != 0)
                    {
                        presets.Attributes["dc_dinner_percent"] = missingPercent;   
                        //logger.debug("dinner percent: " + presets.Attributes["dc_dinner_percent"].ToString());
                    }
                    else if(lunch != 0 && dinner != 0)
                    {
                        presets.Attributes["dc_breakfast_percent"] = missingPercent;
                        //logger.debug("breakfast percent: " + presets.Attributes["dc_breakfast_percent"].ToString());
                    }
                    else if(breakfast != 0 && dinner != 0)
                    {
                        presets.Attributes["dc_lunch_percent"] = missingPercent;
                        //logger.debug("lunch percent: " + presets.Attributes["dc_lunch_percent"].ToString());
                    }
                }


                if (presets.Attributes.Contains("dc_cho_pct") && presets.Attributes.Contains("dc_pro_pct") && presets.Attributes.Contains("dc_fat_pct"))
                {
                    logger.debug("All three nutrients contain percentages");
                }
                else
                {

                    if (presets.Attributes.Contains("dc_cho_pct"))
                    {
                        //logger.debug("cholesterol percent: " + presets.Attributes["dc_cho_pct"].GetType());
                        cho = Convert.ToDouble((int)presets.Attributes["dc_cho_pct"]);
                    }

                    else
                    {
                        logger.debug("No cholesterol percentage");


                    }
                    if (presets.Attributes.Contains("dc_pro_pct"))
                    {
                        //logger.debug("protein percent: " + presets.Attributes["dc_pro_pct"].GetType());
                        pro = Convert.ToDouble((int)presets.Attributes["dc_pro_pct"]);
                    }

                    else
                    {
                        logger.debug("No protein percentage");

                    }
                    if (presets.Attributes.Contains("dc_fat_pct"))
                    {
                        //logger.debug("fat percent: " + presets.Attributes["dc_fat_pct"].GetType());
                        fat = Convert.ToDouble((int)presets.Attributes["dc_fat_pct"]);
                    }

                    else
                    {
                        logger.debug("No fat percentage");

                    }
                    missingPercent = 100 - (pro + cho + fat);
                    //logger.debug("missing percent: " + missingPercent);

                    if (pro != 0 && cho != 0)
                    {
                        presets.Attributes["dc_fat_pct"] = missingPercent;
                        //logger.debug("fat percent: " + presets.Attributes["dc_fat_pct"].ToString());
                    }
                    else if (cho != 0 && fat != 0)
                    {
                        presets.Attributes["dc_pro_pct"] = missingPercent;
                        //logger.debug("protein percent: " + presets.Attributes["dc_pro_pct"].ToString());
                    }
                    else if (fat != 0 && pro != 0)
                    {
                        presets.Attributes["dc_cho_pct"] = missingPercent;
                        //logger.debug("cholesterol percent: " + presets.Attributes["dc_cho_pct"].ToString());
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
    }
}
