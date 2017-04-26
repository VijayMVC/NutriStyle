using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Configuration;

// These namespaces are found in the Microsoft.Xrm.Sdk.dll assembly
// located in the SDK\bin folder of the SDK download.
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Crm.Sdk.Messages;
using System.Data.SqlClient;
using DynamicConnections.CRM2011.Common;
using DynamicConnections.CRM2011.Common.Utility;
using Microsoft.Xrm.Sdk.Messages;
using System.Collections.Generic;
using System.Web.Services.Protocols;
namespace ConsoleApplication2
{
    class Program
    {
        static void Main(string[] args)
        {

            string username = "crmadmin";
            string password = "P@ssw0rd";
            string domain = "dc";
            string server = "https://crmdev2011.dynamiconnections.com";
            string organization = "NS";
            string port = "443";
            String lookupEntityA = "dc_foods";
            String lookupEntityB = "dc_meal_component";


            OrganizationServiceProxy organizationServiceProxy = CrmHelper.CreateCrmService(username, password, domain, server, organization, Convert.ToInt32(port));


            String SQL = @"select F.dc_usda_ndb_no, F.dc_usda_ndbno, DC.dc_foodsid, dc_meal_componentid from dc_dc_foods_dc_meal_component DC 
                        inner join dc_foods F on F.dc_foodsId = DC.dc_foodsid 
                        where F.ModifiedBy 
                        in (select systemuserid from SystemUser where FirstName in ('scott', 'kayla'))";

            //Connect to SQL
            String connectionString = "Data Source=10.1.10.3; Initial Catalog=NS_MSCRM; User ID=sa;Password=P@ssw0rd; Trusted_Connection=False; Connection Timeout=0";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(SQL, connection))
                {

                    using (SqlDataReader rs = command.ExecuteReader())
                    {
                        int count = 0;
                        while (rs.Read())//reads row from database
                        {
                            //dc_usda_ndbno
                            String usdaNdbId = rs["dc_usda_ndbno"].GetType() == typeof(DBNull) ? String.Empty : (String)rs["dc_usda_ndbno"];
                            String usdaId = rs["dc_usda_ndb_no"].GetType() == typeof(DBNull) ? String.Empty : (String)rs["dc_usda_ndb_no"] ;
                            Guid foodsId = (Guid)rs["dc_foodsid"];
                            Guid componentCategoryId = (Guid)rs["dc_meal_componentid"];

                            //lookup in ne crm
                            string fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                  <entity name='dc_foods'>
                                    <attribute name='dc_foodsid' />
                                    <filter type='and'>
                                      <condition attribute='dc_usda_ndb_no' operator='eq' value='@VALUE' />
                                    </filter>
                                  </entity>
                                </fetch>";

                            fetchXml = fetchXml.Replace("@VALUE", usdaId.ToString());

                            EntityCollection collection = organizationServiceProxy.RetrieveMultiple(new FetchExpression(fetchXml));

                            if (collection.Entities.Count > 0)
                            {
                                Entity food = (Entity)collection.Entities[0];
                                foodsId = food.Id;
                            }


                            if (!DoesRelationshipExist(organizationServiceProxy, "dc_dc_foods_dc_meal_component", lookupEntityA, foodsId,
                                lookupEntityB, componentCategoryId))
                            {
                                AssociateRequest request = new AssociateRequest()
                                {
                                    Target = new EntityReference(lookupEntityA, foodsId),
                                    RelatedEntities = new EntityReferenceCollection
                                    {
                                        new EntityReference(lookupEntityB, componentCategoryId)
                                    },
                                    Relationship = new Relationship("dc_dc_foods_dc_meal_component")
                                };
                                try
                                {
                                    organizationServiceProxy.Execute(request);
                                    System.Console.WriteLine("Created Relationship: " + count);
                                }
                                catch (Exception e)
                                {
                                    System.Console.WriteLine("Food most likely doesn't exist");
                                }
                                
                            }
                            else
                            {
                                System.Console.WriteLine("Skipping row.  Relationship is in place: "+count);
                            }
                            count++;
                        }
                    }
                }
            }
        }




        public static bool DoesRelationshipExist(OrganizationServiceProxy crmService, string relationshipSchemaName,
            string entity1SchemaName, Guid entity1KeyValue, string entity2SchemaName, Guid entity2KeyValue)
        {
            // Assemble FetchXML to query the intersection entity directly
            string fetchXml = "<fetch mapping='logical'> <entity name='" + relationshipSchemaName + "'>"
            + "<all-attributes />"
            + "<filter>"
            + "<condition attribute='" + entity1SchemaName + "id' operator='eq' value ='" + entity1KeyValue.ToString() + "' />"
            + "<condition attribute='" + entity2SchemaName + "id' operator='eq' value='" + entity2KeyValue.ToString() + "' />"
            + "</filter>"
            + "</entity>"
            + "</fetch>";

            // Perform the query
            EntityCollection collection = crmService.RetrieveMultiple(new FetchExpression(fetchXml));

            if (collection.Entities.Count == 0)
                return false;
            else
                return true;
        }

    }
}
