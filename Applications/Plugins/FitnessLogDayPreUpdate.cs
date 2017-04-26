using System;
using System.Collections.Generic;

using Microsoft.Xrm.Sdk;
using DynamicConnections.CRM2011.CustomAssemblies.Utilities;
using Microsoft.Xrm.Sdk.Query;
using System.ServiceModel;

namespace DynamicConnections.NutriStyle.CRM2011.Plugins
{
    /// <summary>
    /// Calculates the kcals for the day
    /// </summary>
    public class FitnessLogDayPreUpdate : IPlugin
    {
        //Tools tools = new Tools("nutristyle_plugins_output");
        public enum ActivityLevel { Sedentary = 948170000, LightlyActive = 948170001, ModeratelyActive = 948170002, HighlyActive = 948170003 }
        Logger logger;


        public void Execute(IServiceProvider serviceProvider)
        {
            logger = new Logger("DEBUG", @"c:\windows\temp\nutristyle_plugins_output.txt", 1);
            Guid menuId = Guid.Empty;
            try
            {
                // get the execution context from the service provider
                IPluginExecutionContext pluginExecutionContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

                // Obtain the organization service reference.
                IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService crmService = serviceFactory.CreateOrganizationService(pluginExecutionContext.UserId);

                logger.debug("FitnessLogDayPreUpdate: starting: " + pluginExecutionContext.PrimaryEntityName);

                // initilize the variables 
                string gendeName = string.Empty;
                int gendeCodeValue = 0;
                decimal weightKgValue = (decimal)0.0;
                decimal heightCmValue = (decimal)0.0;
                int ageValue = 0;
                int activityLevel = 0;
                decimal currentWeight = 0m;

                // get the Entity contact
                Entity fitnessLogDay = (Entity)pluginExecutionContext.InputParameters["Target"];

                // get the Entity preContact
                Entity preFitnessLogDay = new Entity();
                if (pluginExecutionContext.PreEntityImages.Count > 0)
                {
                    preFitnessLogDay = (Entity)pluginExecutionContext.PreEntityImages["preimage"];
                }

                Guid contactId = Guid.Empty;
                bool detailedLogging = false;
                if (fitnessLogDay.Contains("dc_contactid"))
                {
                    contactId = ((EntityReference)fitnessLogDay["dc_contactid"]).Id;
                }
                else
                {
                    contactId = ((EntityReference)preFitnessLogDay["dc_contactid"]).Id;
                }

                if (fitnessLogDay.Contains("dc_detailedlogging"))
                {
                    detailedLogging = ((bool)fitnessLogDay["dc_detailedlogging"]);
                }
                else
                {
                    detailedLogging = ((bool)preFitnessLogDay["dc_detailedlogging"]);
                }


                logger.debug("Found entities.  Starting gathering of variables");
                //Find default menu
                String fetchXmlMenuId = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                          <entity name='dc_menu'>
                            <attribute name='dc_menuid' />
                            <attribute name='dc_name' />
                            <attribute name='createdon' />
                            <order attribute='dc_name' descending='false' />
                            <filter type='and'>
                              <condition attribute='dc_primarymenu' operator='eq' value='1' />
                            </filter>
                            <link-entity name='contact' from='contactid' to='dc_contactid' alias='aa'>
                              <filter type='and'>
                                <condition attribute='contactid' operator='eq'  value='@CONTACTID' />
                              </filter>
                            </link-entity>
                          </entity>
                        </fetch>";

                fetchXmlMenuId = fetchXmlMenuId.Replace("@CONTACTID", contactId.ToString());

                EntityCollection response = null;

                try
                {
                    response = crmService.RetrieveMultiple(new FetchExpression(fetchXmlMenuId));
                }
                catch (FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> ex)
                {
                    logger.error("Code: " + ex.Detail.ErrorCode);
                    logger.error("Message: " + ex.Detail.Message);
                    logger.error("Trace: " + ex.Detail.TraceText);
                    logger.error("Inner Fault: " + ex.Detail.InnerFault);
                }
                if (response != null && response.Entities.Count > 0)
                {
                    Entity menu = (Entity)response.Entities[0];
                    menuId = menu.Id;
                }

                Entity contact = crmService.Retrieve("contact", contactId, new Microsoft.Xrm.Sdk.Query.ColumnSet(new String[] {"gendercode", "dc_weightkg", 
                    "dc_heightcm", "dc_age", "dc_activitylevel", "dc_currentweight"}));

                if (contact.Contains("gendercode") ||
                    contact.Contains("dc_weightkg") ||
                    contact.Contains("dc_heightcm") ||
                    contact.Contains("dc_age") ||
                    contact.Contains("dc_activitylevel") ||
                    contact.Contains("dc_currentweight")
                    )
                {
                    // gendercode
                    if (contact.Contains("gendercode"))
                    {
                        gendeCodeValue = ((OptionSetValue)contact["gendercode"]).Value;
                    }

                    logger.debug("gendercode: " + gendeCodeValue);
                    // dc_weightkg
                    if (contact.Contains("dc_weightkg"))
                    {
                        weightKgValue = (decimal)contact["dc_weightkg"];
                    }

                    logger.debug("dc_weightkg: " + weightKgValue);
                    // dc_heightcm
                    if (contact.Contains("dc_heightcm"))
                    {
                        heightCmValue = (decimal)contact["dc_heightcm"];
                    }

                    logger.debug("dc_heightcm: " + heightCmValue);
                    // dc_age
                    if (contact.Contains("dc_age"))
                    {
                        ageValue = (int)contact["dc_age"];
                    }

                    logger.debug("dc_age: " + ageValue);

                    // activity level
                    if (fitnessLogDay.Contains("dc_activitylevel"))
                    {
                        activityLevel = ((OptionSetValue)fitnessLogDay["dc_activitylevel"]).Value;
                    }

                    logger.debug("activityLevel: " + activityLevel);

                    if (contact.Contains("dc_currentweight"))
                    {
                        currentWeight = (int)contact["dc_currentweight"];
                    }
                    logger.debug("dc_currentweight: " + currentWeight);
                }

                if (gendeCodeValue > 0)
                {
                    if (gendeCodeValue == 1)
                    {
                        gendeName = "Male";
                    }
                    else if (gendeCodeValue == 2)
                    {
                        gendeName = "Female";
                    }

                    logger.debug("gendeCodeValue: " + gendeCodeValue);
                    logger.debug("gendeName: " + gendeName);
                    if (!detailedLogging)
                    {
                        //Start by calculating the BMI

                        decimal BMI = CalculateBMI(weightKgValue, heightCmValue / 100m);
                        logger.debug("BMI: " + BMI);
                        //overweight?
                        bool overweight = IsOverWeight(BMI);
                        logger.debug("overweight: " + overweight);
                        decimal PA = CalculatePA(gendeCodeValue, ageValue, overweight, activityLevel);
                        logger.debug("PA: " + PA);
                        decimal kcals = CalculateKcals(gendeCodeValue, ageValue, PA, weightKgValue, heightCmValue / 100m, overweight);
                        logger.debug("kcals: " + kcals);
                        fitnessLogDay["dc_kcals"] = kcals;
                        if (menuId != Guid.Empty)
                        {
                            fitnessLogDay["dc_menuid"] = new EntityReference("dc_menu", menuId);
                        }
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
        /// <summary>
        /// Calculate Kcals
        /// </summary>
        /// <param name="genderCode"></param>
        /// <param name="ageValue"></param>
        /// <param name="PA"></param>
        /// <param name="weightKgValue"></param>
        /// <param name="heightMValue"></param>
        /// <param name="overweight"></param>
        /// <returns></returns>
        private decimal CalculateKcals(int genderCode, int ageValue, decimal PA, decimal weightKgValue, decimal heightMValue, bool overweight)
        {
            //males
            if (genderCode == 1)
            {
                if (ageValue >= 3 && ageValue <= 8 && !overweight)
                {
                    return (88.5m - (61.9m * ageValue) + PA * ((26.7m * weightKgValue) + (903m * heightMValue)) + 20m);
                }
                else if (ageValue >= 9 && ageValue <= 18 && !overweight)
                {
                    return (88.5m - (61.9m * ageValue) + PA * ((26.7m * weightKgValue) + (903m * heightMValue)) + 25m);
                }
                else if (ageValue >= 3 && ageValue <= 18 && overweight)
                {
                    return (114m - (50.9m * ageValue) + PA * ((19.5m * weightKgValue) + (1161.4m * heightMValue)));
                }
                else if (ageValue >= 19 && !overweight)
                {
                    return (662m - (9.53m * ageValue) + PA * ((15.91m * weightKgValue) + (539.6m * heightMValue)));
                }
                else if (ageValue >= 19 && overweight)
                {
                    return (864m - (9.72m * ageValue) + PA * ((14.2m * weightKgValue) + (503m * heightMValue)));
                }
            }
            //females
            else if (genderCode == 2)
            {
                if (ageValue >= 3 && ageValue <= 8 && !overweight)
                {
                    return (135.3m - (30.8m * ageValue) + PA * ((10.0m * weightKgValue) + (934m * heightMValue)) + 20m);
                }
                else if (ageValue >= 9 && ageValue <= 18 && !overweight)
                {
                    return (135.3m - (30.8m * ageValue) + PA * ((10.0m * weightKgValue) + (934m * heightMValue)) + 25m);
                }
                else if (ageValue >= 3 && ageValue <= 18 && overweight)
                {
                    return (389m - (41.2m * ageValue) + PA * ((15.0m * weightKgValue) + (701.6m * heightMValue)));
                }
                else if (ageValue >= 19 && !overweight)
                {
                    return (354m - (6.91m * ageValue) + PA * ((9.36m * weightKgValue) + (726m * heightMValue)));
                }
                else if (ageValue >= 19 && overweight)
                {
                    return (387m - (7.31m * ageValue) + PA * ((10.9m * weightKgValue) + (660.7m * heightMValue)));
                }
            }
            return (1);
        }
        /// <summary>
        /// Calculate the PA value
        /// </summary>
        /// <param name="gendeCodeValue"></param>
        /// <param name="ageValue"></param>
        /// <param name="overweight"></param>
        /// <returns></returns>
        private decimal CalculatePA(int gendeCodeValue, int ageValue, bool overweight, int activityLevel)
        {
            //Male
            if (gendeCodeValue == 1)
            {
                if (ageValue >= 3 && ageValue <= 18 && !overweight)
                {
                    if (activityLevel == (int)ActivityLevel.Sedentary)
                    {
                        return (1m);
                    }
                    else if (activityLevel == (int)ActivityLevel.LightlyActive)
                    {
                        return (1.13m);
                    }
                    else if (activityLevel == (int)ActivityLevel.ModeratelyActive)
                    {
                        return (1.26m);
                    }
                    else if (activityLevel == (int)ActivityLevel.HighlyActive)
                    {
                        return (1.42m);
                    }
                }
                else if (ageValue >= 3 && ageValue <= 18 && overweight)
                {
                    if (activityLevel == (int)ActivityLevel.Sedentary)
                    {
                        return (1m);
                    }
                    else if (activityLevel == (int)ActivityLevel.LightlyActive)
                    {
                        return (1.12m);
                    }
                    else if (activityLevel == (int)ActivityLevel.ModeratelyActive)
                    {
                        return (1.24m);
                    }
                    else if (activityLevel == (int)ActivityLevel.HighlyActive)
                    {
                        return (1.45m);
                    }
                }

                if (ageValue >= 19 && !overweight)
                {
                    if (activityLevel == (int)ActivityLevel.Sedentary)
                    {
                        return (1m);
                    }
                    else if (activityLevel == (int)ActivityLevel.LightlyActive)
                    {
                        return (1.11m);
                    }
                    else if (activityLevel == (int)ActivityLevel.ModeratelyActive)
                    {
                        return (1.25m);
                    }
                    else if (activityLevel == (int)ActivityLevel.HighlyActive)
                    {
                        return (1.48m);
                    }
                }
                else if (ageValue >= 19 && overweight)
                {
                    if (activityLevel == (int)ActivityLevel.Sedentary)
                    {
                        return (1m);
                    }
                    else if (activityLevel == (int)ActivityLevel.LightlyActive)
                    {
                        return (1.12m);
                    }
                    else if (activityLevel == (int)ActivityLevel.ModeratelyActive)
                    {
                        return (1.27m);
                    }
                    else if (activityLevel == (int)ActivityLevel.HighlyActive)
                    {
                        return (1.54m);
                    }
                }
            }
            //Female
            else if (gendeCodeValue == 2)
            {
                if (ageValue >= 3 && ageValue <= 18 && !overweight)
                {
                    if (activityLevel == (int)ActivityLevel.Sedentary)
                    {
                        return (1m);
                    }
                    else if (activityLevel == (int)ActivityLevel.LightlyActive)
                    {
                        return (1.16m);
                    }
                    else if (activityLevel == (int)ActivityLevel.ModeratelyActive)
                    {
                        return (1.31m);
                    }
                    else if (activityLevel == (int)ActivityLevel.HighlyActive)
                    {
                        return (1.56m);
                    }
                }
                else if (ageValue >= 3 && ageValue <= 18 && overweight)
                {
                    if (activityLevel == (int)ActivityLevel.Sedentary)
                    {
                        return (1m);
                    }
                    else if (activityLevel == (int)ActivityLevel.LightlyActive)
                    {
                        return (1.18m);
                    }
                    else if (activityLevel == (int)ActivityLevel.ModeratelyActive)
                    {
                        return (1.35m);
                    }
                    else if (activityLevel == (int)ActivityLevel.HighlyActive)
                    {
                        return (1.60m);
                    }
                }

                if (ageValue >= 19 && !overweight)
                {
                    if (activityLevel == (int)ActivityLevel.Sedentary)
                    {
                        return (1m);
                    }
                    else if (activityLevel == (int)ActivityLevel.LightlyActive)
                    {
                        return (1.12m);
                    }
                    else if (activityLevel == (int)ActivityLevel.ModeratelyActive)
                    {
                        return (1.27m);
                    }
                    else if (activityLevel == (int)ActivityLevel.HighlyActive)
                    {
                        return (1.45m);
                    }
                }
                else if (ageValue >= 19 && overweight)
                {
                    if (activityLevel == (int)ActivityLevel.Sedentary)
                    {
                        return (1m);
                    }
                    else if (activityLevel == (int)ActivityLevel.LightlyActive)
                    {
                        return (1.14m);
                    }
                    else if (activityLevel == (int)ActivityLevel.ModeratelyActive)
                    {
                        return (1.27m);
                    }
                    else if (activityLevel == (int)ActivityLevel.HighlyActive)
                    {
                        return (1.45m);
                    }
                }
            }
            return (1m);
        }
        /// <summary>
        /// Calculate BMI
        /// </summary>
        /// <param name="weightKgValue"></param>
        /// <param name="heightInMeters"></param>
        /// <returns></returns>
        private decimal CalculateBMI(decimal weightKgValue, decimal heightInMeters)
        {
            return (weightKgValue / (heightInMeters * heightInMeters));
        }
        /// <summary>
        /// Figure out if person is overweight
        /// </summary>
        /// <param name="BMI"></param>
        /// <returns></returns>
        private bool IsOverWeight(decimal BMI)
        {
            if (BMI > 25)
            {
                return (true);
            }
            else
            {
                return (false);
            }
        }
    }
}