using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using DynamicConnections.CRM2011.Common.Utility;
using Microsoft.Xrm.Sdk.Client;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;

namespace DynamicConnections.NutriStyle.CRM2011.CreateEntities
{
    class Create
    {
        Logger logger;
        OrganizationServiceProxy crmService;

        public Create()
        {
            //Setup CRM connection
            String user = ConfigurationManager.AppSettings["CrmUser"];
            String password = ConfigurationManager.AppSettings["Password"];
            String orgName = ConfigurationManager.AppSettings["CrmOrganization"];
            String domain = ConfigurationManager.AppSettings["Domain"];
            String host = ConfigurationManager.AppSettings["Hostname"];
            String port = ConfigurationManager.AppSettings["Portnumber"];
            String level = ConfigurationManager.AppSettings["LogLevel"];
            String location = ConfigurationManager.AppSettings["LogLocation"];

            logger = new Logger(level, location);

            crmService = CrmHelper.CreateCrmService(user, password, domain, host, orgName, Convert.ToInt32(port));
            logger.debug("Created connection");
        }

        public void Execute()
        {
            try
            {
                //do stuff
                Entity contactEntity = new Entity();
                contactEntity.LogicalName = "contact";
                contactEntity["firstname"] = "Ernest";
                Guid contactId = crmService.Create(contactEntity);
                //delete
                crmService.Delete("contact", contactId);

            }
            catch (FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> ex)
            {
                logger.error("Code: " + ex.Detail.ErrorCode);
                logger.error("Message: " + ex.Detail.Message);
                logger.error("Trace: " + ex.Detail.TraceText);
                logger.error("Inner Fault: " + ex.Detail.InnerFault);
            }
            catch (Exception ex)
            {
                logger.error(ex.Message);
                logger.error(ex.StackTrace);
            }
        }
    }
}
