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
using Microsoft.Crm.Sdk.Messages;


namespace DynamicConnections.NutriStyle.CRM2011.WebServices.Engine
{
    public class Metadata
    {
        
        Logger logger;
        public static OrganizationServiceProxy crmService = null;
        public XmlDocument xmlDocResults {get;set;}
        public Metadata()
        {

            String level    = ConfigurationManager.AppSettings["LogLevel"];
            String location = ConfigurationManager.AppSettings["LogLocation"];
            logger          = new Logger(level, location);
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
        /// <summary>
        /// Return xml document representing metadata xml
        /// </summary>
        /// <param name="entityName"></param>
        /// <returns></returns>
        public XmlDocument Retrieve(String entityName)
        {
            XmlDocument xml = Success.Create("success");
            try 
            {
                EntityMetadata metadata = MetadataHelper.GetEntityMetadata(entityName, crmService);
                if (metadata != null)
                {
                    /*
                    XmlSerializer serializer = new XmlSerializer(typeof(EntityMetadata));
                    StringWriter writer = new StringWriter();

                    serializer.Serialize(writer, metadata);

                    writer.Close();

                    String xmlData = writer.ToString();
                    logger.debug("Xml Data: " + xmlData);
                    */

                    StringWriter writer = new StringWriter();

                    // Create Xml Writer.
                    XmlTextWriter metadataWriter = new XmlTextWriter(writer);

                    // Start Xml File.
                    metadataWriter.WriteStartDocument();

                    // Metadata Xml Node.
                    metadataWriter.WriteStartElement("Metadata");



                    // Start Entity Node
                    metadataWriter.WriteStartElement("Entity");

                    // Write the Entity's Information.
                    metadataWriter.WriteElementString("EntityLogicalName", metadata.LogicalName);
                    if (metadata.IsCustomizable.Value == true)
                    {
                        metadataWriter.WriteElementString("IsCustomizable", "yes");
                    }
                    else
                    {
                        metadataWriter.WriteElementString("IsCustomizable", "no");
                    }
                    if (metadata.IsIntersect.Value == true)
                    {
                        metadataWriter.WriteElementString("IsIntersect", "yes");
                    }
                    else
                    {
                        metadataWriter.WriteElementString("IsIntersect", "no");
                    }



                    // Write Entity's Attributes.
                    metadataWriter.WriteStartElement("Attributes");

                    foreach (AttributeMetadata currentAttribute in metadata.Attributes)
                    {
                        // Only write out main attributes.
                        if (currentAttribute.AttributeOf == null)
                        {

                            // Start Attribute Node
                            metadataWriter.WriteStartElement("Attribute");

                            // Write Attribute's information.
                            metadataWriter.WriteElementString("LogicalName", currentAttribute.LogicalName);
                            // Write the Description if it is set.
                            if (currentAttribute.Description.UserLocalizedLabel != null)
                            {
                                metadataWriter.WriteElementString("Description", currentAttribute.Description.UserLocalizedLabel.Label.ToString());
                            }

                            metadataWriter.WriteElementString("Type", currentAttribute.AttributeType.Value.ToString());
                            if (currentAttribute.DisplayName.UserLocalizedLabel != null)
                                metadataWriter.WriteElementString("DisplayName", currentAttribute.DisplayName.UserLocalizedLabel.Label.ToString());
                            if (currentAttribute.SchemaName != null)
                                metadataWriter.WriteElementString("SchemaName", currentAttribute.SchemaName.ToString());
                            if (currentAttribute.DeprecatedVersion != null)
                                metadataWriter.WriteElementString("DeprecatedVersion", currentAttribute.DeprecatedVersion.ToString());
                            metadataWriter.WriteElementString("IsCustomAttribute", currentAttribute.IsCustomAttribute.Value.ToString());
                            metadataWriter.WriteElementString("IsCustomizable", currentAttribute.IsCustomizable.Value.ToString());
                            metadataWriter.WriteElementString("RequiredLevel", currentAttribute.RequiredLevel.Value.ToString());
                            metadataWriter.WriteElementString("IsValidForCreate", currentAttribute.IsValidForCreate.Value.ToString());
                            metadataWriter.WriteElementString("IsValidForRead", currentAttribute.IsValidForRead.Value.ToString());
                            metadataWriter.WriteElementString("IsValidForUpdate", currentAttribute.IsValidForUpdate.Value.ToString());
                            metadataWriter.WriteElementString("CanBeSecuredForCreate", currentAttribute.CanBeSecuredForCreate.Value.ToString());
                            metadataWriter.WriteElementString("CanBeSecuredForRead", currentAttribute.CanBeSecuredForRead.Value.ToString());
                            metadataWriter.WriteElementString("CanBeSecuredForUpdate", currentAttribute.CanBeSecuredForUpdate.Value.ToString());
                            metadataWriter.WriteElementString("IsAuditEnabled", currentAttribute.IsAuditEnabled.Value.ToString());
                            metadataWriter.WriteElementString("IsManaged", currentAttribute.IsManaged.Value.ToString());
                            metadataWriter.WriteElementString("IsPrimaryId", currentAttribute.IsPrimaryId.Value.ToString());
                            metadataWriter.WriteElementString("IsPrimaryName", currentAttribute.IsPrimaryName.Value.ToString());
                            metadataWriter.WriteElementString("IsRenameable", currentAttribute.IsRenameable.Value.ToString());
                            metadataWriter.WriteElementString("IsSecured", currentAttribute.IsSecured.Value.ToString());
                            metadataWriter.WriteElementString("IsValidForAdvancedFind", currentAttribute.IsValidForAdvancedFind.Value.ToString());

                            // End Attribute Node
                            metadataWriter.WriteEndElement();
                        }
                    }
                    // End Attributes Node
                    metadataWriter.WriteEndElement();
                    // End Metadata Xml Node
                    metadataWriter.WriteEndElement();
                    metadataWriter.WriteEndDocument();

                    // Close xml writer.
                    metadataWriter.Close();

                    writer.Close();
                    
                    String xmlData = writer.ToString();
                    //logger.debug("Xml Data: " + xmlData);

                    xml.LoadXml(xmlData);
                }
                else
                {
                    logger.error("metadata is null!!!!");
                }
                
            }
            catch (FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> e)
            {
                logger.debug(e.Message);
                logger.debug(e.StackTrace);
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
            return (xml);
        }
    }
}