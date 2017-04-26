using System;
using System.Collections.Generic;

using Microsoft.Xrm.Sdk;
using DynamicConnections.CRM2011.CustomAssemblies.Utilities;

namespace DynamicConnections.NutriStyle.CRM2011.Plugins
{
    public class ContactPreUpdate_Age : IPlugin
    {
        Tools tools = new Tools("nutristyle_plugins_output");
        Logger logger;

        public int CalculateAgeFromBirthdate(DateTime birthDate, DateTime currentDate)
        {
            
            int ageValue = 0;
            try
            {

                int now = int.Parse(currentDate.ToString("yyyyMMdd"));
                int dob = int.Parse(birthDate.ToString("yyyyMMdd"));
                string dif = (now - dob).ToString();
                string age = "0";
                if (dif.Length > 4)
                {
                    age = dif.Substring(0, dif.Length - 4);
                }
                return (Convert.ToInt32(age));
             /*
                if (currentDate > birthDate)
                {
                    ageValue = currentDate.Year - birthDate.Year;//Fix for month
                    if (ageValue > 0)
                    {
                        return (ageValue);
                    }
                    else
                    {
                        return (0);
                    }
                }
                else
                {
                    return (0);
                }         
              */                      
            }
            catch  (Exception ex)
            {
                logger.error("CalculateAgeFromBirthdate Method");
                logger.error(ex.Message);
                logger.error(ex.StackTrace);
                return (ageValue);
            }
        }

        public void Execute(IServiceProvider serviceProvider)
        {
            logger = new Logger("DEBUG", @"c:\windows\temp\nutristyle_plugins_output.txt", 1);
            try
            {
                // get the execution context from the service provider
                IPluginExecutionContext pluginExecutionContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

                logger.debug("ContactPreUpdate_Age: starting: " + pluginExecutionContext.PrimaryEntityName);

                // get tge Entity object
                Entity contact = (Entity)pluginExecutionContext.InputParameters["Target"];

                if (contact.Attributes.Contains("birthdate"))
                {
                    // get the birthdate abd current date
                    DateTime birthdate = (DateTime)contact["birthdate"];
                    DateTime dateTimeCurrent = DateTime.Now;

                    logger.debug("birthdate: " + birthdate);
                    logger.debug("dateTimeCurrent: " + dateTimeCurrent);

                    // calculate the age 
                    int ageValue = CalculateAgeFromBirthdate(birthdate, dateTimeCurrent);

                    logger.debug("ageValue: " + ageValue);

                    // set the dc_age field to the calculated age
                    contact["dc_age"] = ageValue;
                }                                
            }
            catch (Exception ex)
            {
                logger.error("Execute Method");
                logger.error(ex.Message);
                logger.error(ex.StackTrace);
            }
        } // end Execute

    }
}
