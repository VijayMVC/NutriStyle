using System;
using System.Collections.Generic;

using Microsoft.Xrm.Sdk;
using DynamicConnections.CRM2011.CustomAssemblies.Utilities;

namespace DynamicConnections.NutriStyle.CRM2011.Plugins
{
    public class ContactPreUpdate : IPlugin
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

                logger.debug("ContactPreUpdate: starting: " + pluginExecutionContext.PrimaryEntityName);

                // initilize the variables                
                int activitylevelValue = 0;
                string activitylevelName= string.Empty;

                

                // form - data that the user changed
                Entity contact = (Entity)pluginExecutionContext.InputParameters["Target"];

                // get the dc_activitylevel values from OptionSetValue
                if (contact.Attributes.Contains("dc_activitylevel"))
                {
                    activitylevelValue = ((OptionSetValue)contact["dc_activitylevel"]).Value;
                }

                logger.debug("activitylevelValue: " + activitylevelValue);
                logger.debug("activitylevelName: " + activitylevelName);

                // get the activitylevelName as s, la, ma and ha
                if (activitylevelValue > 0)
                {
                    if (activitylevelValue == 948170000)
                    {
                        activitylevelName = "s";
                    }
                    else if (activitylevelValue == 948170001)
                    {
                        activitylevelName = "la";
                    }
                    else if (activitylevelValue == 948170002)
                    {
                        activitylevelName = "ma";
                    }
                    else if (activitylevelValue == 948170003)
                    {
                        activitylevelName = "ha";
                    }
                }

                logger.debug("activitylevelValue: " + activitylevelValue);
                logger.debug("activitylevelName: " + activitylevelName);
                
                // all fields must be in contact Entity
                if (activitylevelName != string.Empty)
                {                   
                    // for activitylevelName = s
                    if (activitylevelName.Equals("s", StringComparison.OrdinalIgnoreCase))
                    {
                        contact["dc_restinghours"] = (decimal)12.0;
                        contact["dc_verylighthours"] = (decimal)12.0;
                        contact["dc_lighthours"] = (decimal)0.0;
                        contact["dc_moderatehours"] = (decimal)0.0;
                        contact["dc_heavyhours"] = (decimal)0.0;
                    }
                    // for activitylevelName = la
                    if (activitylevelName.Equals("la", StringComparison.OrdinalIgnoreCase))
                    {
                        contact["dc_restinghours"] = (decimal)8.0;
                        contact["dc_verylighthours"] = (decimal)12.0;
                        contact["dc_lighthours"] = (decimal)3.0;
                        contact["dc_moderatehours"] = (decimal)1.0;
                        contact["dc_heavyhours"] = (decimal)0.0;                            
                    }
                    // for activitylevelName = ma
                    if (activitylevelName.Equals("ma", StringComparison.OrdinalIgnoreCase))
                    {
                        contact["dc_restinghours"] = (decimal)8.0;
                        contact["dc_verylighthours"] = (decimal)9.0;
                        contact["dc_lighthours"] = (decimal)4.0;
                        contact["dc_moderatehours"] = (decimal)2.0;
                        contact["dc_heavyhours"] = (decimal)1.0;
                    }
                    // for activitylevelName = ha
                    if (activitylevelName.Equals("ha", StringComparison.OrdinalIgnoreCase))
                    {
                        contact["dc_restinghours"] = (decimal)8.0;
                        contact["dc_verylighthours"] = (decimal)5.0;
                        contact["dc_lighthours"] = (decimal)4.0;
                        contact["dc_moderatehours"] = (decimal)2.0;
                        contact["dc_heavyhours"] = (decimal)5.0;
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
