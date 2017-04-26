using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DynamicConnections.CRM2011.Common.Utility;
using System.Configuration;
using Microsoft.Xrm.Sdk.Client;

namespace DynamicConnections.NutriStyle.CRM2011.Testing
{
    public abstract class General
    {
        private static OrganizationServiceProxy crmService;

        protected static Logger GetLogger()
        {
            try
            {
                string LogLevel = "DEBUG";//ConfigurationManager.AppSettings["LogLevel"];
                string LogLocation = "c:\\windows\\temp\\nutristyle-testing.txt"; //ConfigurationManager.AppSettings["LogLocation"];
                Logger logger = new Logger(LogLevel, LogLocation);
                return (logger);
            }
            catch (Exception ex)
            {
                return (null);
            }
        }// end GetLogger


        protected static OrganizationServiceProxy GetCrmService()
        {
            try
            {
                if (crmService == null)
                {
                    crmService = CrmHelper.CreateCrmService("crmadmin", "P@ssw0rd", "DC", "https://crmdev.dynamiconnections.com", "NSDEV", 443);
                }
                return (crmService);
            }
            catch (Exception ex)
            {
                return (null);
            }
        }// end GetLogger

    }
}
