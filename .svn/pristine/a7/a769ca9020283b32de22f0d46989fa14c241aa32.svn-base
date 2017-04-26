using System;

using System.Configuration;
using DynamicConnections.CRM2011.Common.Utility;
using Microsoft.Xrm.Sdk.Client;
using System.Xml;

using Microsoft.Xrm.Sdk.Metadata;

using Microsoft.Xrm.Sdk;
using System.Web.Services.Protocols;

using System.ServiceModel;

using Microsoft.Xrm.Sdk.Query;




namespace DynamicConnections.NutriStyle.CRM2011.WebServices.Engine
{
    public class MealFood
    {

        Logger logger;
        public static OrganizationServiceProxy crmService = null;

        public MealFood()
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
                logger.debug(e.Message);
                logger.debug(e.StackTrace);
            }

            catch (SoapException e)
            {
                logger.debug(e.Message);
                logger.debug(e.Detail.InnerText);
                logger.debug(e.StackTrace);
            }
            catch (Exception e)
            {
                logger.debug(e.Message);
                logger.debug(e.StackTrace);
            }
        }
        public XmlDocument CreateFoodMeal(XmlDocument xmlDoc)
        {
            XmlDocument xml = Success.Create("success");
            //Make sure that the contact email doesn't exist
            try
            {
                logger.debug("CreateFoodMeal: starting");
                logger.debug("Xml: " + xmlDoc.OuterXml);
                Guid Id             = Guid.Empty;
                XmlNode entityNode  = xmlDoc.FirstChild;
                String entityName   = entityNode.Name;

                Entity entity       = new Entity(entityName);

                int mealOptionSet   = -1;
                int dayOptionSet    = -1;
                Guid foodId         = Guid.Empty;
                Guid menuId         = Guid.Empty;
                Guid dayId          = Guid.Empty;
                Guid mealId         = Guid.Empty;
                Guid mealFoodId     = Guid.Empty;
                Guid contactId      = Guid.Empty;
                Guid portionTypeId  = Guid.Empty;
                decimal portionSize = 0m;

                EntityMetadata metadata = MetadataHelper.GetEntityMetadata(entityName, crmService);
                bool update = false;
                foreach (XmlNode child in entityNode.ChildNodes)
                {
                    logger.debug(child.OuterXml);
                    String value = child.InnerText;
                    if (child.Name.Equals("dc_day", StringComparison.OrdinalIgnoreCase))
                    {
                        dayOptionSet = Convert.ToInt32(value);
                    }

                    else if (child.Name.Equals("dc_meal", StringComparison.OrdinalIgnoreCase))
                    {
                        mealOptionSet = Convert.ToInt32(value);
                    }

                    else if (child.Name.Equals("dc_foodid", StringComparison.OrdinalIgnoreCase))
                    {
                        foodId = new Guid(value);
                    }
                    else if (child.Name.Equals("contactid", StringComparison.OrdinalIgnoreCase))
                    {
                        contactId = new Guid(value);
                    }
                    else if (child.Name.Equals("dc_portiontypeid", StringComparison.OrdinalIgnoreCase))
                    {
                        portionTypeId = new Guid(value);
                    }
                    else if (child.Name.Equals("dc_menuid", StringComparison.OrdinalIgnoreCase))
                    {
                        menuId = new Guid(value);
                    }
                    else if (child.Name.Equals("dc_portionsize", StringComparison.OrdinalIgnoreCase))
                    {
                        portionSize = Convert.ToDecimal(value);
                    }
                    else if (child.Name.Equals("dc_mealfoodid", StringComparison.OrdinalIgnoreCase))
                    {
                        mealFoodId = new Guid(value);
                        if (mealFoodId != Guid.Empty)
                        {
                            update = true;
                        }
                    }
                }

                logger.debug("dayOptionSet: " + dayOptionSet);
                logger.debug("mealOptionSet: " + mealOptionSet);
                logger.debug("foodId: " + foodId);
                logger.debug("menuId: " + menuId);
                logger.debug("mealFoodId: " + mealFoodId);
                logger.debug("contactId: " + contactId);
                logger.debug("portionTypeId: " + portionTypeId);
                logger.debug("portionSize: " + portionSize);

                //Make sure that the menu is selected
                if (menuId == Guid.Empty)
                {
                    EntityCollection menus = CrmHelper.GetEntitiesByAttribute("dc_menu", "dc_contactid", contactId, new String[] { "dc_menuid" }, null, crmService);

                    if (menus != null && menus.Entities.Count > 0)
                    {
                        Entity menu = (Entity)menus.Entities[0];
                        menuId = menu.Id;
                        logger.debug("menuId: " + menuId);
                    }
                }


                QueryExpression qe = new QueryExpression();
                qe.EntityName = "dc_day";
                qe.ColumnSet = new ColumnSet(new String[] { "dc_dayid" });

                //day
                ConditionExpression ce = new ConditionExpression();
                ce.AttributeName = "dc_day";
                ce.Operator = ConditionOperator.Equal;
                ce.Values.Add(dayOptionSet);

                ConditionExpression ce2 = new ConditionExpression();
                ce2.AttributeName = "dc_menuid";
                ce2.Operator = ConditionOperator.Equal;
                ce2.Values.Add(menuId);

                qe.Criteria = new FilterExpression();
                qe.Criteria.AddCondition(ce);
                qe.Criteria.AddCondition(ce2);

                EntityCollection ec = crmService.RetrieveMultiple(qe);

                if (ec != null && ec.Entities.Count > 0)
                {
                    Entity day = (Entity)ec.Entities[0];
                    dayId = day.Id;
                    logger.debug("Found day");
                }
                else
                {
                    Entity day = new Entity("dc_day");
                    day["dc_menuid"] = new EntityReference("dc_menu", menuId);
                    day["dc_day"] = new OptionSetValue(dayOptionSet);
                    dayId = crmService.Create(day);
                    logger.debug("created day:" + dayId);
                }
                //Now look for meal that's associated to the day

                qe = new QueryExpression();
                qe.EntityName   = "dc_meal";
                qe.ColumnSet    = new ColumnSet(new String[] { "dc_mealid" });

                //day
                ce = new ConditionExpression();
                ce.AttributeName    = "dc_meal";
                ce.Operator         = ConditionOperator.Equal;
                ce.Values.Add(mealOptionSet);

                ce2 = new ConditionExpression();
                ce2.AttributeName   = "dc_dayid";
                ce2.Operator        = ConditionOperator.Equal;
                ce2.Values.Add(dayId);

                qe.Criteria = new FilterExpression();
                qe.Criteria.AddCondition(ce);
                qe.Criteria.AddCondition(ce2);

                ec = crmService.RetrieveMultiple(qe);

                if (ec != null && ec.Entities.Count > 0)
                {
                    Entity meal = (Entity)ec.Entities[0];
                    mealId      = meal.Id;
                    logger.debug("Found meal");
                }
                else //Create entity
                {
                    Entity meal = new Entity("dc_meal");
                    meal["dc_dayid"] = new EntityReference("dc_day", dayId);
                    meal["dc_meal"] = new OptionSetValue(mealOptionSet);
                    mealId = crmService.Create(meal);
                    logger.debug("created meal:" + mealId);
                }

                //all needed data is gathered.  Create meal option
                Entity mealFood                 = new Entity("dc_mealfood");
                mealFood["dc_mealid"]           = new EntityReference("dc_meal", mealId);
                mealFood["dc_foodid"]           = new EntityReference("dc_foods", foodId);
                mealFood["dc_portiontypeid"]    = new EntityReference("dc_portion_types", portionTypeId);
                mealFood["dc_portionsize"]      = portionSize;   
                if (update)
                {
                    //mealFood.Id = mealFoodId;
                    mealFood["dc_mealfoodid"] = mealFoodId; 
                    crmService.Update(mealFood);
                    logger.debug("Updated meal food: " + mealFoodId);
                }
                else
                {
                    mealFoodId = crmService.Create(mealFood);
                    logger.debug("Created meal food: " + mealFoodId);
                }
                xml = Success.Create(mealFoodId.ToString());
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                
                logger.error("Code: " + ex.Detail.ErrorCode);
                logger.error("Message: " + ex.Detail.Message);
                logger.error("Trace: " + ex.Detail.TraceText);
                logger.error("Inner Fault: " + ex.Detail.InnerFault);
                logger.error("Stack Trace: " +ex.StackTrace);
                xml = Error.Create(ex.Message + "\n" + ex.StackTrace);
            }
            catch (SoapException e)
            {
                logger.debug(e.Message);
                logger.debug(e.Detail.InnerText);
                logger.debug(e.StackTrace);
                xml = Error.Create(e.Message + "\n" + e.StackTrace);
            }
            catch (Exception e)
            {
                logger.debug(e.Message);
                logger.debug(e.StackTrace);
                xml = Error.Create(e.Message + "\n" + e.StackTrace);
            }
            return (xml);
        }

    }
}