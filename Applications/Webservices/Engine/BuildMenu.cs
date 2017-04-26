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
    public class BuildMenu
    {
        
        Logger logger;
        public static OrganizationServiceProxy crmService = null;
        String level    = ConfigurationManager.AppSettings["LogLevel"];

        String location;
        String user;
        String password;
        String orgName;
        String hostname;
        String domain;
        String portnumber;

        public BuildMenu()
        {

            level    = ConfigurationManager.AppSettings["LogLevel"];
            location = ConfigurationManager.AppSettings["LogLocation"];
            logger = new Logger(level, location);
            user     = ConfigurationManager.AppSettings["CrmUser"];
            password = ConfigurationManager.AppSettings["Password"];
            orgName  = ConfigurationManager.AppSettings["CrmOrganization"];
            hostname = ConfigurationManager.AppSettings["Hostname"];
            domain   = ConfigurationManager.AppSettings["Domain"];
            portnumber = ConfigurationManager.AppSettings["Portnumber"];

        }
        
        public XmlDocument Execute(Guid contactId)
        {
            XmlDocument xml = Success.Create("worked");
            MenuGenerator.BuildMenu bm = new MenuGenerator.BuildMenu(user, password, domain, hostname, orgName, portnumber, logger);
            xml = bm.Build(contactId);
            //Build food log
            FoodLog fl = new FoodLog();
            Guid menuId = Guid.Empty;
            logger.debug("MenuId: " + xml.FirstChild.InnerText);
            menuId = new Guid(xml.FirstChild.InnerText);

            return(xml);
        }

        
    }
}