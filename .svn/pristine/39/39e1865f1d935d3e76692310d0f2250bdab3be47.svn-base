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
    public class Picklist
    {
        
        Logger logger;
        public static OrganizationServiceProxy crmService = null;

        public Picklist()
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
        public XmlDocument RetrieveOptionSet(String entityName, String attributeName)
        {
            logger.debug("RetrieveOptionSet: starting");
            XmlDocument results = Success.Create("worked");

            //Get values for picklist
            try
            {
                EntityMetadata metadata = MetadataHelper.GetEntityMetadata(entityName, crmService);
                PicklistAttributeMetadata pam = (PicklistAttributeMetadata)MetadataHelper.GetAttributeMetaData(metadata, attributeName);

                DataContractSerializer serializer = new DataContractSerializer(typeof(PicklistAttributeMetadata), null, int.MaxValue, false, false, null, new KnownTypesResolver());
                MemoryStream ms = new MemoryStream();
                XmlDictionaryWriter xdw = XmlDictionaryWriter.CreateTextWriter(ms);
                serializer.WriteObject(xdw, pam);
                xdw.Flush();
                ms.Position = 0;
                StreamReader sr = new StreamReader(ms);
                String serializedXML = sr.ReadToEnd();
                xdw.Close();
                //logger.debug("serializedXML: " + serializedXML);
                XmlDocument xmlDoc = new XmlDocument();

                xmlDoc.LoadXml(serializedXML);
                XmlNamespaceManager xnm = new XmlNamespaceManager(xmlDoc.NameTable);
                xnm.AddNamespace("i", "http://www.w3.org/2001/XMLSchema-instance");
                xnm.AddNamespace("b", "http://schemas.microsoft.com/xrm/2011/Metadata");
                xnm.AddNamespace("a", "http://schemas.microsoft.com/xrm/2011/Contracts");

                //get the options
                XmlNode node = xmlDoc.GetElementsByTagName("Options")[0];
                results = new XmlDocument();
                XmlNode resultSet = results.CreateNode(XmlNodeType.Element, "resultset", "");
                results.AppendChild(resultSet);
                int x = 0;
                foreach (XmlNode optionSet in node)
                {
                    //logger.debug("Option Set :" + optionSet.OuterXml);
                    //logger.debug("xml: " + optionSet.SelectNodes("//b:Value", xnm)[x].InnerText);
                    int value = Convert.ToInt32(optionSet.SelectNodes("//b:Value", xnm)[x].InnerText);
                    //logger.debug("value: " + value);
                    //logger.debug("get label");
                    //logger.debug("xml: " + optionSet.SelectNodes("//b:Label/a:LocalizedLabels/a:LocalizedLabel/a:Label", xnm)[x].InnerText);
                    XmlNode pair = results.CreateNode(XmlNodeType.Element, "pair", "");
                    XmlNode nameNode = results.CreateNode(XmlNodeType.Element, "name", "");
                    XmlNode valueNode = results.CreateNode(XmlNodeType.Element, "value", "");
                    nameNode.InnerText = optionSet.SelectNodes("//b:Label/a:LocalizedLabels/a:LocalizedLabel/a:Label", xnm)[x].InnerText;
                    valueNode.InnerText = value.ToString();

                    pair.AppendChild(nameNode);
                    pair.AppendChild(valueNode);
                    resultSet.AppendChild(pair);
                    x++;
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
    }
}