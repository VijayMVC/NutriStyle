using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using DynamicConnections.CRM2011.Common.Utility;
using Microsoft.Xrm.Sdk.Client;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Xrm.Sdk.Metadata;
using System.IO;
using System.Runtime.Serialization;

using Microsoft.Xrm.Sdk;
using System.Web.Services.Protocols;

using System.ServiceModel;
using System.ServiceModel.Description;
using Microsoft.Xrm.Sdk.Query;
using DynamicConnections.NutriStyle.CRM2011.WebServices.Engine;
using Microsoft.Xrm.Sdk.Messages;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using Microsoft.Crm.Sdk.Messages;

namespace DynamicConnections.NutriStyle.CRM2011.WebServices.Engine
{
    public class Database
    {
        Logger logger;
        OrganizationServiceProxy crmService;
        String calendarConnectionString;

        public Database()
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
                logger.debug("hostname:" + hostname);
                logger.debug("portnumber:" + portnumber);
                
                crmService = CrmHelper.CreateCrmService(user, password, domain, hostname, orgName, Convert.ToInt32(portnumber));

                // First, get the organization's ID from the system user record.
                Guid userId = ((WhoAmIResponse)crmService.Execute(new WhoAmIRequest())).UserId;
                Entity userEntity = (Entity)crmService.Retrieve("systemuser", userId, new ColumnSet(new String[] {"fullname", "domainname"}));
                logger.debug("Built crm service object: " + userId + ":" + userEntity["fullname"] + ":" + userEntity["domainname"]);
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
        public XmlDocument CreateUpdate(XmlDocument xmlDoc)
        {
            return(CreateUpdate(xmlDoc, false));
        }
        public XmlDocument CreateUpdateEntities(XmlDocument xmlDoc, bool returnEntity)
        {
            XmlDocument results = new XmlDocument();
            logger.debug("CreateUpdateEntities: starting: results: " + xmlDoc.OuterXml);
            XmlNode resultsNode = results.CreateNode(XmlNodeType.Element, "results", String.Empty);

            results.AppendChild(resultsNode);
            //Look for child entities.
            XmlNodeList list = xmlDoc.ChildNodes[0].ChildNodes;
            try
            {
                foreach (XmlNode node in list)
                {
                    logger.debug("Proecssing node: " + node.OuterXml);
                    XmlDocument entity = new XmlDocument();
                    entity.LoadXml(node.OuterXml);
                    logger.debug("Proecssing entity: " + entity.OuterXml);

                    XmlDocument result = CreateUpdate(entity, true);
                    logger.debug("Result: " + result.OuterXml);
                    logger.debug("Result: " + result.ChildNodes[0].OuterXml);

                    XmlNode imported = results.ImportNode(result.ChildNodes[0],true);
                    resultsNode.AppendChild(imported);

                }
            }
            catch (Exception e)
            {
                logger.error(e.Message);
                logger.error(e.StackTrace);
            }
            logger.debug("Returning: "+results.OuterXml);
            return (results);
        }
        public XmlDocument CreateUpdate(XmlDocument xmlDoc, bool returnEntity)
        {
            XmlDocument results = Success.Create("worked");
            logger.debug("CreateUpdate(XmlDoc, "+returnEntity+"): starting");
            logger.debug("Xml: " + xmlDoc.OuterXml);
            Guid Id = Guid.Empty;
            Guid mealId = Guid.Empty;
            bool update = false;
            
            try
            {
                XmlNode entityNode = xmlDoc.FirstChild;
                String entityName = entityNode.Name;
                //See if create is an attribute
                

                Entity entity = new Entity(entityName);

                EntityMetadata metadata = MetadataHelper.GetEntityMetadata(entityName, crmService);
                
                if (entityName.Equals("dc_mealfood", StringComparison.OrdinalIgnoreCase))
                {
                    //see if there is an empty dc_mealid node
                    Guid menuId = Guid.Empty;
                    Guid dayId  = Guid.Empty;
                    int mealValue = 0;
                    bool create = false;
                    foreach (XmlNode child in entityNode.ChildNodes)
                    {
                        if (child.Name.Equals("dc_mealid"))
                        {
                            //see if null
                            if (String.IsNullOrEmpty(child.InnerText))
                            {
                                logger.debug("Need to create meal entity");
                                create = true;
                            }
                        }
                        else if (child.Name.Equals("dc_dayid"))
                        {
                            dayId = new Guid(child.InnerText);
                        }
                        else if (child.Name.Equals("dc_menuid"))
                        {
                            menuId = new Guid(child.InnerText);
                        }
                        else if (child.Name.Equals("dc_mealvalue"))
                        {
                            logger.debug("value: " + child.InnerText);
                            mealValue = Convert.ToInt32(child.InnerText);
                        }
                    }

                    if (create)
                    {
                        //search for a match first
                        String fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                          <entity name='dc_meal'>
                            <attribute name='dc_mealid' />
                            <attribute name='dc_name' />
                            <attribute name='createdon' />
                            <order attribute='dc_name' descending='false' />
                            <filter type='and'>
                              <condition attribute='dc_dayid' operator='eq'  value='@DAYID' />
                              <condition attribute='dc_meal' operator='eq' value='@MEALVALUE' />
                            </filter>
                          </entity>
                        </fetch>";

                        fetchXml = fetchXml.Replace("@DAYID", dayId.ToString());
                        fetchXml = fetchXml.Replace("@MEALVALUE", mealValue.ToString());

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
                        logger.debug("Records found: " + response.Entities.Count);

                        if (response.Entities.Count > 0)
                        {
                            Entity meal = response.Entities[0];
                            mealId = meal.Id;
                        }else {

                            Entity meal = new Entity("dc_meal");
                            meal["dc_dayid"] = new EntityReference("dc_day", dayId);
                            meal["dc_meal"] = new OptionSetValue(mealValue);
                            mealId = crmService.Create(meal);
                        }
                    }
                }
                bool createRecord = false;

                if (entityNode.Attributes["create"] != null)
                {
                    createRecord = Convert.ToBoolean(entityNode.Attributes["create"].Value);
                }
                logger.debug("CreateRecord: " + createRecord);

                foreach (XmlNode child in entityNode.ChildNodes)
                {
                    logger.debug(child.OuterXml);
                    String value = child.InnerText;
                    logger.debug("Value: " + value);
                    

                    //Figure out the attribute type
                    AttributeMetadata am = (AttributeMetadata)MetadataHelper.GetAttributeMetaData(metadata, child.Name);
                    if (am != null) //Do not process values not found in the metadata
                    {
                        logger.debug(am.GetType().ToString()+ ": "+child.Name);
                        if (am.GetType() == typeof(StringAttributeMetadata))
                        {
                            logger.debug("Found string");
                            entity[child.Name] = value;
                        }
                        else if (am.GetType() == typeof(MemoAttributeMetadata))
                        {
                            logger.debug("Found memo");
                            entity[child.Name] = value;
                        }
                        else if (am.GetType() == typeof(LookupAttributeMetadata))
                        {
                            Guid lookupId = Guid.Empty;
                            logger.debug("Found Lookup");
                            String entityReference = child.Attributes["entityname"].Value;
                            logger.debug("reference: " + entityReference);

                            if (String.IsNullOrEmpty(value) && entityName.Equals("dc_mealfood", StringComparison.OrdinalIgnoreCase))
                            {
                                if(mealId != Guid.Empty) {
                                    entity[child.Name] = new EntityReference(entityReference, mealId);
                                }
                            }
                            else if (Guid.TryParse(value, out lookupId))
                            {
                                logger.debug("Valid Guid: " + lookupId.ToString());
                                if (lookupId != Guid.Empty)
                                {
                                    entity[child.Name] = new EntityReference(entityReference, lookupId);
                                }
                            }
                            else
                            {
                                //entity.Attributes.Add(new KeyValuePair<string, object>(child.Name, null));
                                //entity[child.Name] = new EntityReference(entityReference, null);
                            }
                        }
                        else if (am.GetType() == typeof(DateTimeAttributeMetadata))
                        {
                            logger.debug("Found DateTime");
                            entity[child.Name] = Convert.ToDateTime(value);
                        }
                        else if (am.GetType() == typeof(PicklistAttributeMetadata))
                        {
                            logger.debug("Found picklist");
                            entity[child.Name] = new OptionSetValue(Convert.ToInt32(value));
                        }
                        else if (am.GetType() == typeof(BooleanAttributeMetadata))
                        {
                            logger.debug("Found Boolean");
                            entity[child.Name] = Convert.ToBoolean(value);
                        }
                        else if (am.GetType() == typeof(DecimalAttributeMetadata))
                        {
                            logger.debug("Found decimal");
                            decimal d = 0m;
                            if (Decimal.TryParse(value, out d))
                            {
                                entity[child.Name] = Convert.ToDecimal(value);
                            }
                        }
                        else if (am.GetType() == typeof(IntegerAttributeMetadata))
                        {
                            logger.debug("Found integer");
                            Int32 i = 0;
                            if (Int32.TryParse(value, out i))
                            {
                                entity[child.Name] = Convert.ToInt32(value);
                            }
                        }
                        else if (am.GetType() == typeof(DoubleAttributeMetadata))
                        {
                            logger.debug("Found double");
                            double d = 0d;
                            if(double.TryParse(value, out d)) {
                                entity[child.Name] = Convert.ToDouble(value);
                            }
                        }
                        //else if (am.GetType() == typeof(AttributeMetadata))
                        else if(child.Name.Equals(entityName+"id", StringComparison.OrdinalIgnoreCase)) 
                        {
                            logger.debug("Found Key");
                            if (!String.IsNullOrEmpty(value) && new Guid(value) != Guid.Empty)
                            {
                                entity[child.Name] = new Guid(value);
                                Id = new Guid(value);
                                update = true;
                            }
                        }
                    }
                }
                if (createRecord)
                {
                    update = false;//override
                }
                if (update)
                {
                    crmService.Update(entity);

                    XmlAttribute xa = xmlDoc.CreateAttribute("Id");
                    xa.Value = Id.ToString();
                    xmlDoc.FirstChild.Attributes.Append(xa);
                    //results = xmlDoc;
                    results = Success.Create(Id.ToString());
                }
                else
                {
                    Id = crmService.Create(entity);
                    XmlAttribute xa = xmlDoc.CreateAttribute("Id");
                    xa.Value = Id.ToString();
                    xmlDoc.FirstChild.Attributes.Append(xa);
                    //results = xmlDoc;
                    results = Success.Create(Id.ToString());
                }
                //logger.debug("xml: " + results.OuterXml);
                if (entityName.Equals("dc_mealfood", StringComparison.OrdinalIgnoreCase))
                {
                    //Add to the food log
                    //one could argue that this should be a plugin.  We'll leave here for now
                    /*
                    FoodLog fl = new FoodLog();
                    Guid contactId = Guid.Empty;
                    XmlNode contactNode = xmlDoc.GetElementsByTagName("dc_contactid")[0];
                    if (contactNode != null && Guid.TryParse(contactNode.InnerText, out contactId))
                    {
                        if (contactId != Guid.Empty)
                        {
                            fl.CreateFromMealFood(entity, contactId);
                        }
                    }*/
                }

                if (returnEntity)
                {
                    //look up and return crm entity
                    
                    entity = crmService.Retrieve(entityName, Id, new ColumnSet(true));
                    //have entity.  Convert to xml
                    EntityCollection ec = new EntityCollection();
                    ec.Entities.Add(entity);
                    results = DatabaseHelper.BuildXml(ec, entityName, results, crmService, null);
                }
            }
            catch (FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> ex)
            {
                logger.error("Code: " + ex.Detail.ErrorCode);
                logger.error("Message: " + ex.Detail.Message);
                logger.error("Message: " + ex.Message);
                logger.error("Trace: " + ex.Detail.TraceText);
                logger.error("Inner Fault: " + ex.Detail.InnerFault);
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
            return (results);
        }
        public XmlDocument Delete(XmlDocument xmlDoc)
        {
            XmlDocument results = Success.Create("worked");
            logger.debug("Delete: starting");
            logger.debug("Xml: " + xmlDoc.OuterXml);
            try
            {
                XmlNode entityNode = xmlDoc.FirstChild;
                String entityName = entityNode.Name;

                Entity entity = new Entity(entityName);

                Guid Id = new Guid(entityNode.FirstChild.InnerText);

                crmService.Delete(entityName, Id);
                results = Success.Create(Id.ToString());
            }
            catch (SoapException e)
            {
                logger.error(e.Message);
                logger.error(e.Detail.InnerText);
                logger.error(e.StackTrace);
                results = Error.Create(e.Message);
            }
            catch (Exception e)
            {
                logger.error(e.Message);
                logger.error(e.StackTrace);
                results = Error.Create(e.Message);
            }
            return (results);
        }
        public XmlDocument RetrieveOptionSet(String entityName, String attributeName)
        {
            logger.debug("RetrieveOptionSet: starting: "+entityName+": "+attributeName);
            XmlDocument results = Success.Create("worked");
            //Get values for picklist
            try
            {
                EntityMetadata metadata = MetadataHelper.GetEntityMetadata(entityName, crmService);
                PicklistAttributeMetadata pam = (PicklistAttributeMetadata)MetadataHelper.GetAttributeMetaData(metadata, attributeName);
                results = new XmlDocument();
                XmlNode resultSet = results.CreateNode(XmlNodeType.Element, "resultset", "");
                results.AppendChild(resultSet);
                foreach (OptionMetadata om in pam.OptionSet.Options)
                {
                    logger.debug("value: " + om.Value);
                    logger.debug("get label: "+om.Label.UserLocalizedLabel.Label);
                    XmlNode pair = results.CreateNode(XmlNodeType.Element, "pair", "");
                    XmlNode nameNode = results.CreateNode(XmlNodeType.Element, "name", "");
                    XmlNode valueNode = results.CreateNode(XmlNodeType.Element, "value", "");
                    nameNode.InnerText = om.Label.UserLocalizedLabel.Label;
                    valueNode.InnerText = om.Value.ToString();

                    pair.AppendChild(nameNode);
                    pair.AppendChild(valueNode);
                    resultSet.AppendChild(pair);
                }
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
            return (results);
        }

        public XmlDocument RetrieveFetchXml(XmlDocument fetchXml, XmlDocument columnOrderXml, String pagingCookie)
        {
           
            if (pagingCookie != String.Empty)
            {
                XmlAttributeCollection attrs = fetchXml.DocumentElement.Attributes;

                if (pagingCookie != null)
                {
                    XmlAttribute pagingAttr = fetchXml.CreateAttribute("paging-cookie");
                    pagingAttr.Value = HttpUtility.UrlDecode(pagingCookie);
                    attrs.Append(pagingAttr);
                }
            }
            
            Dictionary<String, int> columnOrder = new Dictionary<String, int>();
            int count = 1;
            foreach (XmlNode node in columnOrderXml.FirstChild.ChildNodes)
            {
                if (!columnOrder.ContainsKey(node.InnerText))
                {
                    columnOrder.Add(node.InnerText, count);
                    count++;
                }
            }
            logger.debug("fetchXml: " + fetchXml.OuterXml);
            String entityName = fetchXml.FirstChild.FirstChild.Attributes["name"].InnerText;
            logger.debug("entityName: " + entityName);
            EntityCollection response = null;
            XmlDocument results = Success.Create("worked");

            try
            {
                response = crmService.RetrieveMultiple(new FetchExpression(fetchXml.OuterXml));
            }
            catch (FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> ex)
            {
                logger.error("Code: " + ex.Detail.ErrorCode);
                logger.error("Message: " + ex.Detail.Message);
                logger.error("Trace: " + ex.Detail.TraceText);
                logger.error("Inner Fault: " + ex.Detail.InnerFault);
            }
            logger.debug("Records found: " + response.Entities.Count);
            results = DatabaseHelper.BuildXml(response, entityName, results, crmService, columnOrder);
            logger.debug("Found " + response.Entities.Count + " records");
            //If looking for food log and nothing is found assume that this week needs generated.  Generate current week food log based off of existing primary menu
            if (response.Entities.Count == 0 && 
                entityName.Equals("dc_foodlog", StringComparison.OrdinalIgnoreCase) && 
                fetchXml.OuterXml.Contains("condition attribute=\"dc_date\" operator=\"on\""))
            {
                if (fetchXml.GetElementsByTagName("condition").Count > 3)
                {
                    logger.debug("Need to generate food logs");
                    //Create food logs
                    //Need primary menu
                    //XmlNode menuNode    = fetchXml.GetElementsByTagName("condition")[3];
                    //Guid menuId         = new Guid(menuNode.Attributes[2].InnerText);
                    //need contact
                    XmlNode contactNode = fetchXml.GetElementsByTagName("condition")[3];
                    Guid contactId      = new Guid(contactNode.Attributes[2].InnerText);

                    User u              = new User();
                    Guid menuId         = u.RetrievePrimaryMenuId(contactId);
                    FoodLog fl          = new FoodLog();
                    logger.debug("menuId: " + menuId);
                    logger.debug("contactId: " + contactId);
                    fl.Deactivate(contactId);
                    if (menuId != Guid.Empty && contactId != Guid.Empty)
                    {
                        fl.CreateFromMenu(menuId, contactId);
                        logger.debug("Food logs generated");
                    }
                    return (RetrieveFetchXml(fetchXml, columnOrderXml, pagingCookie));
                }
            }
            return (results);
        }

        public XmlDocument RetrieveFetchXmlRowId(XmlDocument fetchXml, XmlDocument columnOrderXml, String rowId)
        {
            logger.debug("RowId: " + rowId);
            Dictionary<String, int> columnOrder = new Dictionary<String, int>();
            int count = 1;
            foreach (XmlNode node in columnOrderXml.FirstChild.ChildNodes)
            {
                //logger.debug("Adding: " + node.InnerText + ":" + count);
                columnOrder.Add(node.InnerText, count);
                count++;
            }
            //logger.debug("fetchXml: " + fetchXml.OuterXml);
            //logger.debug("fetchXml.FirstChild: " + fetchXml.FirstChild.FirstChild.OuterXml);
            String entityName = fetchXml.FirstChild.FirstChild.Attributes["name"].InnerText;
            //logger.debug("entityName: " + entityName);
            EntityCollection response = null;
            XmlDocument results = Success.Create("worked");

            try
            {
                response = crmService.RetrieveMultiple(new FetchExpression(fetchXml.OuterXml));
            }
            catch (FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> ex)
            {
                logger.error("Code: " + ex.Detail.ErrorCode);
                logger.error("Message: " + ex.Detail.Message);
                logger.error("Trace: " + ex.Detail.TraceText);
                logger.error("Inner Fault: " + ex.Detail.InnerFault);
            }
            logger.debug("Records found: " + response.Entities.Count);
            results = DatabaseHelper.BuildXml(response, entityName, results, crmService, columnOrder);
            XmlAttribute rowAt = results.CreateAttribute("rowId");
            rowAt.Value = rowId;
            results.FirstChild.Attributes.Append(rowAt);
            logger.debug("Found " + response.Entities.Count + " records");
            //logger.debug("results: " + results.OuterXml);
            return (results);
        }
    }
}