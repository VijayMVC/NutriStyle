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

using System.Text;


namespace DynamicConnections.NutriStyle.CRM2011.WebServices.Engine
{
    public class Queries
    {
        
        Logger logger;
        public static OrganizationServiceProxy crmService = null;

        public Queries()
        {

            String level    = ConfigurationManager.AppSettings["LogLevel"];
            String location = ConfigurationManager.AppSettings["LogLocation"];
            logger = new Logger(level, location);
            String user     = ConfigurationManager.AppSettings["CrmUser"];
            String password = ConfigurationManager.AppSettings["Password"];
            String orgName  = ConfigurationManager.AppSettings["CrmOrganization"];
            String hostname = ConfigurationManager.AppSettings["Hostname"];
            String domain   = ConfigurationManager.AppSettings["Domain"];
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
        /// <summary>
        /// Retrieve the foods that are available to be selected as a food favorite
        /// </summary>
        /// <param name="contactId"></param>
        /// <param name="searchText"></param>
        /// <returns></returns>
        public static XmlDocument RetrieveFoodLikes(Guid contactId, String searchText)
        {
            return(RetrieveFoodIngredients(contactId, searchText));
        }
        /// <summary>
        /// retrieve the foods that are available to be added to a menu or food log
        /// </summary>
        /// <param name="contactId"></param>
        /// <param name="searchText"></param>
        /// <returns></returns>
        public static XmlDocument RetrieveFoods(Guid contactId, String searchText)
        {
            return (RetrieveFoodIngredients(contactId, searchText));

        }
        /// <summary>
        /// retrieve the foods that are available to be selected as a food dislike
        /// </summary>
        /// <param name="contactId"></param>
        /// <param name="searchText"></param>
        /// <returns></returns>
        public static XmlDocument RetrieveFoodDislikes(Guid contactId, String searchText)
        {
            return (RetrieveFoodIngredients(contactId, searchText));
        }
        /// <summary>
        /// retreives all foods that a user/contact can use for ingredients.  Filter criteria: (reviewed, active, not a contactid food) or (active, contactid food, not available to all users) 
        /// </summary>
        /// <param name="contactId"></param>
        /// <returns></returns>
        public static XmlDocument RetrieveFoodIngredients(Guid contactId, String searchText)
        {
            Logger.Write(Logger.DEBUG, "RetrieveFoodIngredients: starting");
            XmlDocument results = Success.Create("Failed");

            try
            {

                String fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
                          <entity name='dc_foods'>
                            <attribute name='dc_name' />
                            <attribute name='dc_portion_amount' />
                            <attribute name='dc_portiontypeid' />
                            <attribute name='dc_unit_gram_weight' />
                            <attribute name='dc_foodsid' />
                            <attribute name='dc_recipefood' />
                            <order attribute='dc_name' descending='false' />
                            <filter type='and'>
                                <filter type='or'>
                                     <filter type='and'>
                                        <condition attribute='dc_name' value='%@TEXT%' operator='like'/>
                                        <condition attribute='dc_food_id' operator='null'/>
                                        <condition attribute='dc_reviewed' operator='eq' value='1'/>
                                        <condition attribute='statecode' operator='eq' value='0'/>
                                    </filter>
                                    <filter type='and'>
                                        <condition attribute='dc_name' value='%@TEXT%' operator='like'/>
                                        <condition attribute='statecode' operator='eq' value='0'/>
                                        <filter type='or'>
                                            <filter type='and'>
                                                <condition attribute='dc_contactid' operator='eq' value='@CONTACTID'/>
                                                <condition attribute='dc_availabletoallusers' operator='eq' value='0'/>
                                            </filter>
                                            <filter type='and'>
                                                <condition attribute='dc_contactid' operator='eq' value='@CONTACTID'/>
                                                <condition attribute='dc_availabletoallusers' operator='eq' value='1'/>
                                            </filter>
                                        </filter>
                                    </filter>
                                </filter>
                            </filter>
                            <link-entity name='dc_portion_types' from='dc_portion_typesid' to='dc_portiontypeid' alias='dc_portion_types'>
                              <attribute name='dc_name' />
                              <attribute name='dc_abbreviation' />
                            </link-entity>
                            <link-entity name='dc_food_nutrients' from='dc_food_nutrientsid' to='dc_foodnutrientid' alias='dc_food_nutrients'>
                                <attribute name='dc_fat' />
                                <attribute name='dc_protein' />
                                <attribute name='dc_carbohydrate' />
                                <attribute name='dc_alcohol' />
                            </link-entity>
                          </entity>
                        </fetch>";

                fetchXml = fetchXml.Replace("@TEXT", searchText);
                fetchXml = fetchXml.Replace("@CONTACTID", contactId.ToString());

                XmlDocument fetchXmlDoc = new XmlDocument();
                fetchXmlDoc.LoadXml(fetchXml);

                Database db     = new Database();
                results         = db.RetrieveFetchXml(fetchXmlDoc, General.CreateOrderXml(fetchXmlDoc), String.Empty);
            }
            catch (SoapException e)
            {
                Logger.Write(Logger.ERROR, e.Message);
                Logger.Write(Logger.ERROR, e.Detail.InnerText);
                Logger.Write(Logger.ERROR, e.StackTrace);
            }
            catch (Exception e)
            {
                Logger.Write(Logger.ERROR, e.Message);
                Logger.Write(Logger.ERROR, e.StackTrace);
            }

            return (results);
        }


        /// <summary>
        /// Retrieves the additional profiles for the associated contact/users
        /// </summary>
        /// <param name="contactId"></param>
        /// <returns></returns>
        public static XmlDocument RetrieveAdditionalProfiles(Guid contactId)
        {
            Logger.Write(Logger.DEBUG, "RetrieveAdditionalProfiles: starting");
            XmlDocument results = Success.Create("Failed");

            try
            {

                String fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
                          <entity name='contact'>
                            <attribute name='firstname' />
                            <attribute name='lastname' />
                            <attribute name='dc_rollshoppinglisttoparent' />
                            <attribute name='contactid' />
                            <filter type='and'> 
                                <condition attribute='dc_parentcontactid' value='@CONTACTID' operator='eq'/> 
                            </filter>
                          </entity>
                        </fetch>";

                fetchXml = fetchXml.Replace("@CONTACTID", contactId.ToString());

                XmlDocument fetchXmlDoc = new XmlDocument();
                fetchXmlDoc.LoadXml(fetchXml);

                Database db = new Database();
                results = db.RetrieveFetchXml(fetchXmlDoc, General.CreateOrderXml(fetchXmlDoc), String.Empty);
            }
            catch (SoapException e)
            {
                Logger.Write(Logger.ERROR, e.Message);
                Logger.Write(Logger.ERROR, e.Detail.InnerText);
                Logger.Write(Logger.ERROR, e.StackTrace);
            }
            catch (Exception e)
            {
                Logger.Write(Logger.ERROR, e.Message);
                Logger.Write(Logger.ERROR, e.StackTrace);
            }

            return (results);
        }
        /// <summary>
        /// Retrieve the contact based ont the contactId
        /// </summary>
        /// <param name="contactId"></param>
        /// <returns></returns>
        public static XmlDocument RetrieveContactId(Guid contactId)
        {
            Logger.Write(Logger.DEBUG, "RetrieveContactId: starting");
            XmlDocument results = Success.Create("Failed");

            try
            {
                String fetchXml = @"<fetch distinct='false' mapping='logical' output-format='xml-platform' version='1.0' >
                        <entity name='contact'> 
                            <attribute name='firstname'/>
                            <attribute name='emailaddress1'/> 
                            <attribute name='lastname'/> 
                            <attribute name='gendercode'/>
                            <attribute name='dc_targetweight'/>
                            <attribute name='dc_currentweight'/>
                            <attribute name='dc_heightfeet'/>
                            <attribute name='dc_heightinches'/>
                            <attribute name='birthdate'/>
                            <attribute name='dc_activitylevel'/>
                            <attribute name='dc_poundsperweek'/>
                            <attribute name='dc_maintaintargetweight'/>
                            <attribute name='dc_kcalcalculatedtarget'/>
                            <attribute name='dc_weightkg'/> 
                            <attribute name='dc_heightcm'/> 
                            <attribute name='dc_age'/>
                            <attribute name='dc_bmi'/>
                            <attribute name='contactid'/>

                            <attribute name='dc_morningsnack'/>
                            <attribute name='dc_afternoonsnack'/>
                            <attribute name='dc_eveningsnack'/>
                            <attribute name='dc_menupresetid'/>
                            <attribute name='dc_dee'/>
                            <attribute name='dc_userspecifiedkcaltarget'/>
                            <attribute name='dc_kcaltarget'/>

                            <attribute name='dc_rollshoppinglisttoparent'/>

                            <filter type='and'>
                                <condition attribute='contactid' value='@CONTACTID' operator='eq'/>
                            </filter>
                            <link-entity name='dc_menu' alias='dc_menu' to='contactid' from='dc_contactid' link-type='outer'>
                                 <attribute name='dc_menuid'/>
                                <filter type='and'> <condition attribute='dc_primarymenu' value='1' operator='eq'/> 
                                </filter> 
                            </link-entity>
                        </entity> 
                    </fetch>";

                fetchXml = fetchXml.Replace("@CONTACTID", contactId.ToString());

                XmlDocument fetchXmlDoc = new XmlDocument();
                fetchXmlDoc.LoadXml(fetchXml);

                Database db = new Database();
                results     = db.RetrieveFetchXml(fetchXmlDoc, General.CreateOrderXml(fetchXmlDoc), String.Empty);
            }
            catch (SoapException e)
            {
                Logger.Write(Logger.ERROR, e.Message);
                Logger.Write(Logger.ERROR, e.Detail.InnerText);
                Logger.Write(Logger.ERROR, e.StackTrace);
            }
            catch (Exception e)
            {
                Logger.Write(Logger.ERROR, e.Message);
                Logger.Write(Logger.ERROR, e.StackTrace);
            }

            return (results);
        }

    }
}