using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DynamicConnections.CRM2011.Common.Utility;
using Microsoft.Xrm.Sdk.Client;
using DynamicConnections.NutriStyle.CRM2011.Testing.Tests;

namespace DynamicConnections.NutriStyle.CRM2011.Testing
{
    class Program  : General
    {
        static void Main(string[] args)
        {
            Logger logger = GetLogger();

            //Creaet connection to CRM
            OrganizationServiceProxy crmService = CrmHelper.CreateCrmService("crmadmin", "P@ssw0rd", "DC", "https://crmdev.dynamiconnections.com", "NSDEV", 443);
            logger.debug("Created connection to CRM");

            //Create new user test
            User cu = new User();
            cu.CreateUser();
        }

    }
}
