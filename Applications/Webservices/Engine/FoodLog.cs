﻿using System;
using System.Collections.Generic;

using System.Configuration;
using DynamicConnections.CRM2011.Common.Utility;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Client;
using System.Xml;

using Microsoft.Xrm.Sdk.Metadata;


using Microsoft.Xrm.Sdk;
using System.Web.Services.Protocols;

using System.ServiceModel;

using Microsoft.Xrm.Sdk.Query;


using DynamicConnections.NutriStyle.CRM2011.Webservices.Engine.Helpers;


namespace DynamicConnections.NutriStyle.CRM2011.WebServices.Engine
{
    public class FoodLog
    {

        Logger logger;
        public static OrganizationServiceProxy crmService = null;

        public FoodLog()
        {

            String level = ConfigurationManager.AppSettings["LogLevel"];
            String location = ConfigurationManager.AppSettings["LogLocation"];
            logger = new Logger(level, location);
            String user = ConfigurationManager.AppSettings["CrmUser"];
            String password = ConfigurationManager.AppSettings["Password"];
            String orgName = ConfigurationManager.AppSettings["CrmOrganization"];
            String hostname = ConfigurationManager.AppSettings["Hostname"];
            String domain = ConfigurationManager.AppSettings["Domain"];
            String portnumber = ConfigurationManager.AppSettings["Portnumber"];

            try
            {

                logger.debug("orgName:" + orgName);
                logger.debug("user:" + user);
                logger.debug("password:" + password);
                logger.debug("domain:" + domain);
                logger.debug("hostname:" + hostname);
                logger.debug("portnumber:" + portnumber);

                if (crmService == null)
                {
                    crmService = CrmHelper.CreateCrmService(user, password, domain, hostname, orgName, Convert.ToInt32(portnumber));


                    logger.debug("Built crm service object");
                }
                else
                {
                    logger.debug("Value was set");
                    logger.debug(crmService.CallerId.ToString());
                }
            }
            catch (FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> e)
            {
                logger.error(e.Message);
                logger.error(e.StackTrace);
            }

            catch (SoapException e)
            {
                logger.error(e.Message);
                logger.error(e.Detail.InnerText);
                logger.error(e.StackTrace);
            }
            catch (Exception e)
            {
                logger.error(e.Message);
                logger.error(e.StackTrace);
            }
        }
        public XmlDocument CreateFromMealFood(Entity mealFood, Guid contactId)
        {
            XmlDocument results = Error.Create("CreateFromFoodId failed");
            try
            {
                //need to find the active food log
                DateTime currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                //Find sunday
                currentDate = currentDate.AddDays(-(int)currentDate.DayOfWeek);
                logger.debug("currentDate: " + currentDate);
                EntityMetadata day = MetadataHelper.GetEntityMetadata("dc_day", crmService);

                Guid foodLogId = CreateFoodLog(mealFood, contactId, day, currentDate);
                logger.debug("Created foodLog: " + foodLogId);
            }
            catch (FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> e)
            {
                logger.error(e.Message);
                logger.error(e.StackTrace);
                results = Error.Create(e.Message + " " + e.StackTrace);
            }

            catch (SoapException e)
            {
                logger.error(e.Message);
                logger.error(e.Detail.InnerText);
                logger.error(e.StackTrace);
                results = Error.Create(e.Message + " " + e.Detail.InnerText + " " + e.StackTrace);
            }
            catch (Exception e)
            {
                logger.error(e.Message);
                logger.error(e.StackTrace);
                results = Error.Create(e.Message + " " + e.StackTrace);
            }
            return (results);
        }
        /// <summary>
        /// Deactivate all existing food logs for this contact
        /// </summary>
        /// <param name="contactId"></param>
        /// <returns></returns>
        public XmlDocument Deactivate(Guid contactId)
        {
            XmlDocument results = Error.Create("Deactivate failed");
            try
            {
                String fetchXml =
                    @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                  <entity name='dc_foodlog'>
                    <attribute name='dc_foodlogid' />
                    <filter type='and'>
                      <condition attribute='statecode' operator='eq' value='0' />
                        <condition attribute='dc_contactid' operator='eq' value='@CONTACTID' />
                        <condition attribute='statecode' operator='eq' value='0' />
                    </filter>
                    </entity>
                </fetch>";

                fetchXml = fetchXml.Replace("@CONTACTID", contactId.ToString());

                EntityCollection response = crmService.RetrieveMultiple(new FetchExpression(fetchXml));

                foreach (Entity entity in response.Entities)
                {
                    SetStateRequest setState        = new SetStateRequest();
                    setState.EntityMoniker          = new EntityReference();
                    setState.EntityMoniker.Id       = entity.Id;
                    setState.EntityMoniker.Name     = "dc_foodlog";
                    setState.EntityMoniker.LogicalName = entity.LogicalName;
                    setState.State                  = new OptionSetValue();
                    setState.State.Value            = 1;
                    setState.Status                 = new OptionSetValue();
                    setState.Status.Value           = -1;
                    crmService.Execute(setState);
                }
            }
            catch (FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> e)
            {
                logger.error(e.Message);
                logger.error(e.StackTrace);
                results = Error.Create(e.Message + " " + e.StackTrace);
            }

            catch (SoapException e)
            {
                logger.error(e.Message);
                logger.error(e.Detail.InnerText);
                logger.error(e.StackTrace);
                results = Error.Create(e.Message + " " + e.Detail.InnerText + " " + e.StackTrace);
            }
            catch (Exception e)
            {
                logger.error(e.Message);
                logger.error(e.StackTrace);
                results = Error.Create(e.Message + " " + e.StackTrace);
            }
            return (results);

            
        }

        public XmlDocument CreateFromMenu(Guid menuId, Guid contactId)
        {

            DateTime currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            //Find sunday
            currentDate = currentDate.AddDays(-(int)currentDate.DayOfWeek);
            logger.debug("currentDate: " + currentDate);

            //clear out foodlogs
            String fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                  <entity name='dc_foodlog'>
                    <attribute name='dc_foodlogid' />
                    <filter type='and'>
                      <condition attribute='statecode' operator='eq' value='0' />
                        <condition attribute='dc_contactid' operator='eq' value='@CONTACTID' />
                        <condition attribute='dc_date' operator='gt' value='@START' />
                        <condition attribute='dc_date' operator='le' value='@END' />
                    </filter>
                    </entity>
                </fetch>";

            fetchXml = fetchXml.Replace("@CONTACTID", contactId.ToString());
            fetchXml = fetchXml.Replace("@START", currentDate.ToString("MM/dd/yyyy"));
            fetchXml = fetchXml.Replace("@END", currentDate.AddDays(7).ToString("MM/dd/yyyy"));

            EntityCollection response = null;
            XmlDocument results = Success.Create("worked");

            fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                      <entity name='dc_mealfood'>
                        <attribute name='dc_foodid' />
                        <attribute name='dc_portionsize' />
                        <attribute name='dc_portiontypeid' />

                        <attribute name='dc_fat' />
                        <attribute name='dc_protein' />
                        <attribute name='dc_carbohydrate' />
                        <attribute name='dc_kcals' />
                        <attribute name='dc_alcohol' />
                        <attribute name='dc_mealid' />

                        <attribute name='dc_mealfoodid' />
                        <order attribute='dc_mealfoodid' descending='false' />
                        <link-entity name='dc_meal' from='dc_mealid' to='dc_mealid' alias='dc_meal'>
                          <attribute name='dc_meal'/>
                          <link-entity name='dc_day' from='dc_dayid' to='dc_dayid' alias='dc_day'>
                            <attribute name='dc_day'/> 
                            <attribute name='dc_dayid'/> 
                            <link-entity name='dc_menu' from='dc_menuid' to='dc_menuid' alias='dc_menu'>
                            <attribute name='dc_menuid'/>  
                              <filter type='and'>
                                <condition attribute='dc_contactid' operator='eq' value='@CONTACTID' />
                                <condition attribute='statecode' operator='eq' value='0' />
                                <condition attribute='dc_menuid' operator='eq' value='@MENUID' />
                              </filter>
                            </link-entity>
                          </link-entity>
                        </link-entity>
                      </entity>
                    </fetch>";
            
            fetchXml = fetchXml.Replace("@CONTACTID", contactId.ToString());
            fetchXml = fetchXml.Replace("@MENUID", menuId.ToString());

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
            Dictionary<String, Nutrients> nutrientsList = new Dictionary<String, Nutrients>();

            EntityMetadata day = MetadataHelper.GetEntityMetadata("dc_day", crmService);
            foreach (Entity mealFood in response.Entities)
            {
                CreateFoodLog(mealFood, contactId, day, currentDate);
            }

            return (results);
        }
        private Guid CreateFoodLog(Entity mealFood, Guid contactId, EntityMetadata day, DateTime currentDate)
        {
            //Create a foodlog for each mealfood
            Guid foodLogId = Guid.Empty;
            Entity foodLog = new Entity("dc_foodlog");
            foodLog["dc_contactid"] = new EntityReference("contact", contactId);
            foodLog["dc_foodid"] = mealFood["dc_foodid"];
            foodLog["dc_portionsize"] = mealFood["dc_portionsize"];
            foodLog["dc_fat"] = mealFood["dc_fat"];
            foodLog["dc_alcohol"] = mealFood["dc_alcohol"];
            foodLog["dc_protein"] = mealFood["dc_protein"];
            foodLog["dc_carbohydrate"] = mealFood["dc_carbohydrate"];
            foodLog["dc_portiontypeid"] = mealFood["dc_portiontypeid"];
            foodLog["dc_mealfoodid"] = new EntityReference("dc_mealfood", mealFood.Id);//need to create relationship back to dc_mealfoode
            if (mealFood.Contains("dc_kcals"))
            {
                foodLog["dc_kcals"] = mealFood["dc_kcals"];
                logger.debug("Setting dc_kcals to: " + mealFood["dc_kcals"]);
            }
            else
            {
                foodLog["dc_kcals"] = General.CalculateKcals((decimal)foodLog["dc_carbohydrate"], (decimal)foodLog["dc_fat"], (decimal)foodLog["dc_protein"], (decimal)foodLog["dc_alcohol"]);
                logger.debug("Setting dc_kcals to (calculated): " + foodLog["dc_kcals"]);
            }

            if (mealFood.Contains("dc_meal.dc_meal") && mealFood["dc_meal.dc_meal"] != null)
            {
                foodLog["dc_meal"] = ((OptionSetValue)((AliasedValue)mealFood["dc_meal.dc_meal"]).Value);
            }
            if (mealFood.Contains("dc_day.dc_day") && mealFood["dc_day.dc_day"] != null)
            {
                int dayNumber = mealFood["dc_day.dc_day"] != null ? ((OptionSetValue)((AliasedValue)mealFood["dc_day.dc_day"]).Value).Value : 0;
                //Get name
                String dayName = MetadataHelper.RetrieveOptionSetValueString(day, "dc_day", dayNumber);

                DateTime date = DateTime.Now;
                //now find the correct date
                for (int x = 0; x < 7; x++)
                {
                    if (currentDate.AddDays(x).DayOfWeek.ToString().Equals(dayName, StringComparison.OrdinalIgnoreCase))
                    {
                        date = currentDate.AddDays(x);
                        break;
                    }
                }
                foodLog["dc_date"] = date.ToUniversalTime();
            }
            //logger.debug("Setting dc_date to: " + date.ToUniversalTime());

            try
            {
                foodLogId = crmService.Create(foodLog);
            }
            catch (FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> ex)
            {
                logger.error("Code: " + ex.Detail.ErrorCode);
                logger.error("Message: " + ex.Detail.Message);
                logger.error("Trace: " + ex.Detail.TraceText);
                logger.error("Inner Fault: " + ex.Detail.InnerFault);
            }
            return (foodLogId);
        }

    }
}