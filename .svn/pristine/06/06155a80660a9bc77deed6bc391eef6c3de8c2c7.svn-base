using DynamicConnections.CRM2011.Common.Utility;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicConnections.NutriStyle.CRM2011.MenuGenerator.Helpers 
{
    class Menu : General
    {
        private static Logger logger = GetLogger();

        public Menu()
        {

        }
        /// <summary>
        /// Set all menus to inactive
        /// </summary>
        /// <param name="contactId"></param>
        public static void SetPrimaryToFalse(Guid contactId, OrganizationServiceProxy crmService)
        {
            //Set all previous menus to inactive
            String menuFetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                  <entity name='dc_menu'>
                    <attribute name='dc_menuid' />
                    <attribute name='dc_name' />
                    <attribute name='createdon' />
                    <order attribute='dc_name' descending='false' />
                    <filter type='and'> <condition attribute='statecode' value='0' operator='eq'/> </filter>
                    <link-entity name='contact' from='contactid' to='dc_contactid' alias='aa'>
                      <filter type='and'>
                        <condition attribute='contactid' operator='eq' value='@CONTACTID' />
                      </filter>
                    </link-entity>
                  </entity>
                </fetch>";

            menuFetchXml = menuFetchXml.Replace("@CONTACTID", contactId.ToString());

            EntityCollection menuCollection = crmService.RetrieveMultiple(new FetchExpression(menuFetchXml));

            foreach (Entity m in menuCollection.Entities)
            {
                Entity menu2            = new Entity("dc_menu");
                menu2["dc_menuid"]      = m.Id;
                menu2["dc_primarymenu"] = false;

                crmService.Update(menu2);
            }
        }
        
    }
}
