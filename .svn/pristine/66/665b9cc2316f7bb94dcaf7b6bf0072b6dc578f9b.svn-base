using System;
using System.Collections.Generic;

using Microsoft.Xrm.Sdk;
using DynamicConnections.CRM2011.CustomAssemblies.Utilities;
using Microsoft.Xrm.Sdk.Query;
using System.ServiceModel;
using DynamicConnections.NutriStyle.CRM2011.Plugins.Helpers;

namespace DynamicConnections.NutriStyle.CRM2011.Plugins
{
    /// <summary>
    /// Class calculates the kcals for each exercise. Creates parent log day if needed.
    /// </summary>
    public class FitnessLogPreUpdate : IPlugin
    {
        //private Tools tools = new Tools("nutristyle_plugins_output");
        Logger logger;
        private IOrganizationService crmService;

        private decimal minutes = 0m;
        private decimal totalMinutes = 0m;
        private decimal lowMinutes = 10000m;
        private decimal highMinutes = 0m;
        private decimal total = 0m;
        private Entity fitnessLog = null;
        private decimal mets = 0m;
        
        public void Execute(IServiceProvider serviceProvider)
        {
            logger = new Logger("DEBUG", @"c:\windows\temp\nutristyle_plugins_output.txt", 1);

            IPluginExecutionContext pluginExecutionContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            // Obtain the organization service reference.
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            crmService = serviceFactory.CreateOrganizationService(pluginExecutionContext.UserId);

            try
            {
                logger.debug("----------------------------------------");
                logger.debug("FitnessLogPreUpdate: Starting: " + pluginExecutionContext.PrimaryEntityName);

                // get the Entity contact
                fitnessLog = (Entity)pluginExecutionContext.InputParameters["Target"];

                // get the Entity preContact
                Entity preFitnesslog = new Entity();
                if (pluginExecutionContext.PreEntityImages.Count > 0)
                {
                    preFitnesslog = (Entity)pluginExecutionContext.PreEntityImages["preimage"];
                }

                logger.debug("Found entities.  Starting gathering of variables");

                DateTime date   = DateTime.MinValue;
                mets    = 0m;
                Guid contactId  = Guid.Empty;

                int gendercode          = 0;
                decimal weightKG        = 0m;
                decimal heightM         = 0m;
                int age                 = 0;
                decimal BMI             = 0m;


                Guid fitnessLogDayId    = Guid.Empty;
                Guid physicalActivityId = Guid.Empty;
                Guid fitnessLogId       = Guid.Empty;
                Guid menuId             = Guid.Empty;

                if (pluginExecutionContext.MessageName.Equals("update", System.StringComparison.OrdinalIgnoreCase))
                {
                    fitnessLogId = fitnessLog.Id;
                }
                logger.debug("fitnessLogId: " + fitnessLogId);

                if (fitnessLog.Contains("dc_date") ||
                    fitnessLog.Contains("dc_physicalactivityid") ||
                    fitnessLog.Contains("dc_durationminutes") ||
                    fitnessLog.Contains("dc_contactid")
                    )
                {
                    //dc_date
                    if (fitnessLog.Attributes.Contains("dc_date"))
                    {
                        date = ((DateTime)fitnessLog["dc_date"]).ToLocalTime();
                    }
                    else
                    {
                        date = ((DateTime)preFitnesslog["dc_date"]).ToLocalTime();
                    }
                    logger.debug("dc_date: " + date);

                    //dc_mets
                    if (fitnessLog.Attributes.Contains("dc_physicalactivityid"))
                    {
                        physicalActivityId = ((EntityReference)fitnessLog["dc_physicalactivityid"]).Id;
                    }
                    else
                    {
                        physicalActivityId = ((EntityReference)preFitnesslog["dc_physicalactivityid"]).Id;
                    }
                    logger.debug("dc_physicalactivityid: " + physicalActivityId);

                    //dc_durationminutes
                    if (fitnessLog.Attributes.Contains("dc_durationminutes"))
                    {
                        minutes = (decimal)fitnessLog["dc_durationminutes"];
                    }
                    else
                    {
                        minutes = (decimal)preFitnesslog["dc_durationminutes"];
                    }
                    logger.debug("dc_durationminutes: " + minutes);

                    //dc_contctid
                    if (fitnessLog.Attributes.Contains("dc_contactid"))
                    {
                        contactId = ((EntityReference)fitnessLog["dc_contactid"]).Id;
                    }
                    else
                    {
                        contactId = ((EntityReference)preFitnesslog["dc_contactid"]).Id;
                    }
                    logger.debug("dc_contactid: " + contactId);


                    Entity physicalActivity = crmService.Retrieve("dc_physicalactivity", physicalActivityId, new ColumnSet(new String[] {"dc_mets"}));

                    if (physicalActivity != null)
                    {
                        mets = (decimal)physicalActivity["dc_mets"];
                    }
                    logger.debug("mets: " + mets);

                    minutes         = minutes * RetrieveMultipler(mets);
                    totalMinutes    = minutes;
                    lowMinutes      = minutes;
                    highMinutes     = minutes;

                    //Have to retreive mets from the 
                    logger.debug("minutes: " + minutes);

                    //pull contact.  Need: gendercode, dc_weightkg, dc_heightcm, dc_age, dc_bmi
                    
                    Entity contact = crmService.Retrieve("contact", contactId, new ColumnSet(new String[] { "gendercode", "dc_weightkg", "dc_heightcm", "dc_age", "dc_bmi" }));

                    if (contact != null)
                    {
                        if (contact.Contains("gendercode"))
                        {
                            gendercode = ((OptionSetValue)contact["gendercode"]).Value;
                        }
                        if (contact.Contains("dc_age"))
                        {
                            age = (int)contact["dc_age"];
                        }
                        if (contact.Contains("dc_weightkg"))
                        {
                            weightKG = (decimal)contact["dc_weightkg"];
                        }
                        if (contact.Contains("dc_heightcm"))
                        {
                            heightM = (decimal)contact["dc_heightcm"]/100;
                        }
                        if (contact.Contains("dc_bmi"))
                        {
                            BMI = (decimal)contact["dc_bmi"];
                        }
                        
                    }

                    logger.debug("gendercode: " + gendercode);
                    logger.debug("age: " + age);
                    logger.debug("weightKG: " + weightKG);
                    logger.debug("heightM: " + heightM);
                    logger.debug("BMI: " + BMI);
                    //contact squared away

                    //find parent dc_foodlogday entity.  contactid and date
                    String fetchXml = @"<fetch distinct='false' mapping='logical' output-format='xml-platform' version='1.0' >
                        <entity name='dc_fitnesslogday'> 
                            <attribute name='dc_fitnesslogdayid'/>
                            <filter type='and'>
                                <condition attribute='dc_contactid' value='@CONTACTID' operator='eq'/>
                                <condition attribute='dc_date' value='@DATE' operator='eq'/>
                            </filter>
                        </entity> 
                    </fetch>";

                    fetchXml = fetchXml.Replace("@CONTACTID", contactId.ToString());
                    logger.debug("date.ToString(\"MM/dd/yyyy\"): "+date.ToString("MM/dd/yyyy"));
                    fetchXml = fetchXml.Replace("@DATE", date.ToString("MM/dd/yyyy"));

                    EntityCollection response = null;
                   
                    try
                    {
                        response = crmService.RetrieveMultiple(new FetchExpression(fetchXml));
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
                        Entity fitnessLogDay = (Entity)response.Entities[0];
                        fitnessLogDayId = fitnessLogDay.Id;
                    }
                    else
                    {

                        //need to create
                        Entity entity               = new Entity("dc_fitnesslogday");
                        entity["dc_date"]           = new DateTime(date.Year, date.Month, date.Day);
                        entity["dc_name"]           = date.ToString("MM/dd/yyyy");
                        entity["dc_contactid"]      = new EntityReference("contact", contactId);
                       
                        fitnessLogDayId = crmService.Create(entity);
                    }
                    logger.debug("fitnessLogDayId: " + fitnessLogDayId.ToString());

                    //Get collection of related fitness logs

                    fetchXml = @"<fetch distinct='false' mapping='logical' output-format='xml-platform' version='1.0' >
                        <entity name='dc_fitnesslog'> 
                            <attribute name='dc_durationminutes'/>
                            <filter type='and'>
                                <condition attribute='dc_fitnesslogdayid' value='@ID' operator='eq'/>
                            </filter>
                            <link-entity name='dc_physicalactivity' alias='aa' to='dc_physicalactivityid' from='dc_physicalactivityid'>
                                <attribute name='dc_mets'/>
                            </link-entity>
                        </entity> 
                    </fetch>";

                    fetchXml = fetchXml.Replace("@ID", fitnessLogDayId.ToString());

                    response = null;

                    try
                    {
                        response = crmService.RetrieveMultiple(new FetchExpression(fetchXml));
                    }
                    catch (FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> ex)
                    {
                        logger.error("Code: " + ex.Detail.ErrorCode);
                        logger.error("Message: " + ex.Detail.Message);
                        logger.error("Trace: " + ex.Detail.TraceText);
                        logger.error("Inner Fault: " + ex.Detail.InnerFault);
                    }
                    logger.debug("Found " + response.Entities.Count + " related fitness log entities");

                    bool overweight = Fitness.IsOverWeight(BMI) ? true : false; 
                    logger.debug("overweight: " + overweight);

                    // calcualtes the entity in the preimage
                    CalculateFitness(preFitnesslog, gendercode, age, overweight, weightKG, heightM, fitnessLogDayId);

                    if (response != null && response.Entities.Count > 0)
                    {
                        foreach (Entity entity in response.Entities)
                        {
                            if (entity.Id != fitnessLogId)
                            {
                                //goes through the days fitness logs and calculates
                                CalculateFitness(entity, gendercode, age, overweight, weightKG, heightM, fitnessLogDayId);
                            }
                        }
                    }

                }//end of if
            }
            catch (Exception ex)
            {
                logger.error("Execute Method");
                logger.error(ex.Message);
                logger.error(ex.StackTrace);
            }
        }
        private decimal RetrieveMultipler(decimal mets) 
        {
            if (mets >= 1.6m && mets < 3)
            {
                return(.5m);
            }
            else if (mets >= 3m && mets < 6)
            {
                return(1m);
            }
            else if (mets >= 6m )
            {
                return(2m);
            }
            return (1);
        }

        private void CalculateFitness(Entity entity, int gendercode, int age, bool overweight, decimal weightKG, decimal heightM, Guid fitnessLogDayId)
        {
            logger.debug("Entered CalculateFitness");
            var activityLevel = Fitness.IncrementalActivityLevel.Sedentary;
            decimal metsTemp = 0m;
            //decimal metsTemp = (decimal)((AliasedValue)entity["aa.dc_mets"]).Value;
            decimal minutesTemp = 0m;

            if (entity.Contains("aa.dc_mets"))
            {
                logger.debug("Entity from EntityCollection");
                metsTemp = (decimal)((AliasedValue)entity["aa.dc_mets"]).Value;
                minutesTemp = (decimal)entity["dc_durationminutes"];
                minutesTemp = minutesTemp * RetrieveMultipler(metsTemp);
            }
            else
            {
                logger.debug("Entity from preImage");
                //metsTemp = mets;
                minutesTemp = minutes;
            }

            logger.debug("metsTemp: " + metsTemp);
            logger.debug("minutesTemp: " + minutesTemp);

            totalMinutes += minutesTemp;
            if (lowMinutes > minutesTemp)
            {
                lowMinutes = minutesTemp;
            }
            if (highMinutes < minutesTemp)
            {
                highMinutes = minutesTemp;
            }

            //Calculate as we go
            decimal percentRange = 0m;

            if (totalMinutes > 0 && totalMinutes < 60)
            {
                percentRange = (totalMinutes - 1) / (59 - 1);
                activityLevel = Fitness.IncrementalActivityLevel.LightlyActive;
            }
            else if (totalMinutes > 59 && totalMinutes <= 180)
            {
                percentRange = (totalMinutes - 60) / (180 - 60);
                activityLevel = Fitness.IncrementalActivityLevel.ModeratelyToHighlyActive;
            }
            else if (totalMinutes > 180)
            {
                percentRange = (totalMinutes - 60) / (180 - 60);
                activityLevel = Fitness.IncrementalActivityLevel.ModeratelyToHighlyActive;
            }

            decimal incrementalPA = Fitness.CalculateIncrementalPA(gendercode, age, overweight, activityLevel);
            logger.debug("incrementalPA: " + incrementalPA);

            decimal lowRange = Fitness.RetrieveLowValuePA(gendercode, age, overweight, activityLevel);
            logger.debug("lowRange: " + lowRange);

            decimal PA = (percentRange * incrementalPA) + lowRange;
            logger.debug("PA: " + PA);

            total = Fitness.CalculateKcals(gendercode, age, PA, weightKG, heightM, overweight);
            logger.debug("total: " + total);
            //Upate parent  
            Entity updateFitnessLogDay = new Entity("dc_fitnesslogday");
            updateFitnessLogDay["dc_fitnesslogdayid"] = fitnessLogDayId;
            updateFitnessLogDay["dc_kcals"] = total;
            updateFitnessLogDay["dc_detailedlogging"] = true;

            crmService.Update(updateFitnessLogDay);

            fitnessLog["dc_fitnesslogdayid"] = new EntityReference("dc_fitnesslogday", fitnessLogDayId);
            logger.debug("total minutes: " + totalMinutes);

        }

    }
}