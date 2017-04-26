using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace DynamicConnections.NutriStyle.CRM2011.MenuGenerator.Helpers
{
    class Foods
    {
        /// <summary>
        /// Returns true if food is an entree
        /// </summary>
        /// <param name="foodId"></param>
        /// <param name="crmService"></param>
        /// <returns></returns>
        public static bool IsEntree(Guid foodId, OrganizationServiceProxy crmService)
        {
           
            String fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
              <entity name='dc_component_category'>
                <attribute name='dc_component_categoryid' />
                <attribute name='dc_name' />
                <attribute name='createdon' />
                <order attribute='dc_name' descending='false' />
                <filter type='and'>
                  <condition attribute='dc_name' operator='eq' value='Entree' />
                </filter>
                <link-entity name='dc_meal_component' from='dc_componentcategoryid' to='dc_component_categoryid' alias='aa'>
                  <link-entity name='dc_dc_foods_dc_meal_component' from='dc_meal_componentid' to='dc_meal_componentid' visible='false' intersect='true'>
                    <link-entity name='dc_foods' from='dc_foodsid' to='dc_foodsid' alias='ab'>
                      <filter type='and'>
                        <condition attribute='dc_foodsid' operator='eq' value='@FOODID' />
                      </filter>
                    </link-entity>
                  </link-entity>
                </link-entity>
              </entity>
            </fetch>";

            fetchXml = fetchXml.Replace("@FOODID", foodId.ToString());

            EntityCollection menuCollection = crmService.RetrieveMultiple(new FetchExpression(fetchXml));

            if (menuCollection != null && menuCollection.Entities.Count > 0)
            {
                return (true);
            }
            else
            {
                return (false);
            }
            
        }
    }
}
