using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk.Query;
using DynamicConnections.CRM2011.Common.Utility;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Crm.Sdk.Messages;

namespace SyncDatabases
{
    class Program
    {
        static void Main(string[] args)
        {
            String[] entities = { 
                                     "dc_help",
                                   
                                    "dc_foodmanufacturer", "dc_tipoftheday", "dc_verifycustomer",  "dc_portion_types", "dc_grocer", "dc_dri", 
                                    "dc_physicalactivitycategory", "dc_physicalactivity", "dc_restaurant", "dc_foodservicemanagementcompany",
                                "dc_foodcompoundofinterest", "dc_genericpopup", "dc_activitylevelspopup",
                                "dc_presets", "dc_component_category", "dc_meal_component", 
                                "dc_component_template_sel", 
                                "dc_food_nutrients",
                                "dc_foods","dc_country",
                                "dc_ingredient"
                                };

            Logger logger = new Logger("DEBUG", @"c:\windows\temp\Nutristyle_Sync.txt");


            OrganizationServiceProxy crmServiceSource = CrmHelper.CreateCrmService("crmadmin", "P@ssw0rd", "DC", "https://crmdev.dynamiconnections.com", "NSDEV", 443);
            OrganizationServiceProxy crmServiceTarget = CrmHelper.CreateCrmService("crmadmin", "P@ssw0rd", "DC", "https://stagingcrm.dynamiconnections.com", "NS", 443);
            
            // The number of records per page to retrieve.
            int fetchCount = 100;
            // Initialize the page number.
            int pageNumber = 1;
            // Initialize the number of records.

            //delete out all the dc_component_template_sel from the target
            QueryExpression qe2 = new QueryExpression();
            qe2.EntityName = "dc_component_template_sel";
            qe2.ColumnSet = new ColumnSet(true);

            qe2.PageInfo = new PagingInfo();
            qe2.PageInfo.Count = fetchCount;
            qe2.PageInfo.PageNumber = pageNumber;


            //Make sure the entity is active
            ConditionExpression ce2 = new ConditionExpression();
            ce2.AttributeName = "statecode";
            ce2.Values.Add(0);//active
            ce2.Operator = ConditionOperator.Equal;

            FilterExpression fe2 = new FilterExpression();
            fe2.Conditions.Add(ce2);
            qe2.Criteria = fe2;

            // The current paging cookie. When retrieving the first page, 
            // pagingCookie should be null.
            qe2.PageInfo.PagingCookie = null;
            //run query
            /*
            while (true)
            {
                EntityCollection entityCollection = crmServiceTarget.RetrieveMultiple(qe2);
                if (entityCollection.Entities != null)
                {

                    for (int x = 0; x < entityCollection.Entities.Count(); x++)
                    {
                        Entity entity = entityCollection.Entities[x];
                        crmServiceTarget.Delete("dc_component_template_sel", entity.Id);
                        logger.debug("Deleting: " + entity.Id.ToString());
                    }

                }
                if (entityCollection.MoreRecords)
                {
                    // Increment the page number to retrieve the next page.
                    qe2.PageInfo.PageNumber++;
                    // Set the paging cookie to the paging cookie returned from current results.
                    qe2.PageInfo.PagingCookie = entityCollection.PagingCookie;
                }
                else
                {
                    // If no more records are in the result nodes, exit the loop.
                    break;
                }
            }
            */
            foreach (String entityName in entities)
            {
                logger.debug("Processing: " + entityName);

                //Get the metadata
                EntityMetadata metadata = MetadataHelper.GetEntityMetadata(entityName, crmServiceSource);

                QueryExpression qe = new QueryExpression();
                qe.EntityName = entityName;
                qe.ColumnSet = new ColumnSet(true);

                qe.PageInfo = new PagingInfo();
                qe.PageInfo.Count = fetchCount;
                qe.PageInfo.PageNumber = pageNumber;


                //Make sure the entity is active
                ConditionExpression ce = new ConditionExpression();
                ce.AttributeName = "statecode";
                ce.Values.Add(0);//active
                ce.Operator = ConditionOperator.Equal;

                FilterExpression fe = new FilterExpression();
                fe.Conditions.Add(ce);
                qe.Criteria = fe;

                // The current paging cookie. When retrieving the first page, 
                // pagingCookie should be null.
                qe.PageInfo.PagingCookie = null;
                //run query
                int count = 0;
                while (true)
                {
                    EntityCollection entityCollection = crmServiceSource.RetrieveMultiple(qe);

                    if (entityCollection.Entities != null)
                    {
                        foreach (Entity sourceEntity in entityCollection.Entities)
                        {
                            //logger.debug("Processing source: " + sourceEntity.Id);
                            //See if the soure entity exists in the target
                            Entity targetEntity = null;
                            try
                            {
                                targetEntity = crmServiceTarget.Retrieve(sourceEntity.LogicalName, sourceEntity.Id, new ColumnSet(true));
                            }
                            catch (Exception e)
                            {
                                logger.error(entityName + " : " + e.Message);
                            }
                            if (targetEntity != null)
                            {
                                logger.debug(entityName + ": found matching target.  Will update: " + count);

                                try
                                {
                                    crmServiceTarget.Update(sourceEntity);
                                }
                                catch (Exception e)
                                {
                                    logger.error(entityName + " : " + e.Message + " : " + targetEntity.Id.ToString());
                                }
                            }
                            else
                            {
                                logger.debug("Will need to create: " + count);
                                try
                                {
                                    crmServiceTarget.Create(sourceEntity);
                                }
                                catch (Exception e)
                                {
                                    logger.error(entityName + " : " + e.Message);
                                }
                            }
                            if (targetEntity != null && targetEntity.LogicalName.Equals("dc_foods", StringComparison.OrdinalIgnoreCase))
                            {
                                //deal with MtM relationship
                                String fetchXml = "<fetch mapping='logical' no-lock='true' > <entity name='dc_dc_foods_dc_meal_component'>"
                                    + "<all-attributes />"
                                    + "<filter>"
                                    + "<condition attribute='dc_foodsid' operator='eq' value ='" + sourceEntity.Id.ToString() + "' />"
                                    + "</filter>"
                                    + "</entity>"
                                    + "</fetch>";

                                // Perform the query
                                EntityCollection collection = crmServiceSource.RetrieveMultiple(new FetchExpression(fetchXml));
                                if (collection != null && collection.Entities.Count > 0)
                                {
                                    foreach (Entity entity in collection.Entities)
                                    {
                                        //Make sure relationship doesn't exist in the target

                                        fetchXml = "<fetch mapping='logical' no-lock='true' > <entity name='dc_dc_foods_dc_meal_component'>"
                                        + "<all-attributes />"
                                        + "<filter>"
                                        + "<condition attribute='dc_foodsid' operator='eq' value ='" + sourceEntity.Id.ToString() + "' />"
                                        + "</filter>"
                                        + "</entity>"
                                        + "</fetch>";

                                        EntityCollection collectionTarget = crmServiceTarget.RetrieveMultiple(new FetchExpression(fetchXml));
                                        if (collectionTarget == null || collectionTarget.Entities.Count == 0)
                                        {
                                            logger.debug("Creating MTM");

                                            if (entity.Contains("dc_meal_componentid"))
                                            {
                                                Guid subCategoryId = (Guid)entity["dc_meal_componentid"];
                                                logger.debug("subCategoryId: " + subCategoryId);
                                                //Create relationship
                                                AssociateRequest request = new AssociateRequest()
                                                {
                                                    Target = new EntityReference("dc_foods", sourceEntity.Id),
                                                    RelatedEntities = new EntityReferenceCollection
                                                    {
                                                        new EntityReference("dc_meal_component", subCategoryId)
                                                    },
                                                    Relationship = new Microsoft.Xrm.Sdk.Relationship("dc_dc_foods_dc_meal_component")
                                                };
                                                crmServiceTarget.Execute(request);
                                            }

                                        }

                                    }
                                }
                                else
                                {
                                    logger.debug("No MtM found");
                                }
                            }

                            count++;
                        }

                    }
                    if (entityCollection.MoreRecords)
                    {
                        // Increment the page number to retrieve the next page.
                        qe.PageInfo.PageNumber++;
                        // Set the paging cookie to the paging cookie returned from current results.
                        qe.PageInfo.PagingCookie = entityCollection.PagingCookie;
                    }
                    else
                    {
                        // If no more records are in the result nodes, exit the loop.
                        break;
                    }
                }//end of while
                

                //Deal with inactive
                qe = new QueryExpression();
                qe.EntityName = entityName;
                qe.ColumnSet = new ColumnSet(true);

                qe.PageInfo = new PagingInfo();
                qe.PageInfo.Count = fetchCount;
                qe.PageInfo.PageNumber = pageNumber;


                //Make sure the entity is active
                ce = new ConditionExpression();
                ce.AttributeName = "statecode";
                ce.Values.Add(1);//inactive
                ce.Operator = ConditionOperator.Equal;

                fe = new FilterExpression();
                fe.Conditions.Add(ce);
                qe.Criteria = fe;

                // The current paging cookie. When retrieving the first page, 
                // pagingCookie should be null.
                qe.PageInfo.PagingCookie = null;
                //run query
                count = 0;
                while (true)
                {
                    EntityCollection entityCollection = crmServiceSource.RetrieveMultiple(qe);

                    if (entityCollection.Entities != null)
                    {
                        foreach (Entity sourceEntity in entityCollection.Entities)
                        {
                            //logger.debug("Processing source: " + sourceEntity.Id);
                            //See if the soure entity exists in the target
                            Entity targetEntity = null;
                            try
                            {
                                targetEntity = crmServiceTarget.Retrieve(sourceEntity.LogicalName, sourceEntity.Id, new ColumnSet(true));
                            }
                            catch (Exception e)
                            {
                                logger.error(entityName + " : " + e.Message);
                            }
                            if (targetEntity != null)
                            {
                                logger.debug(entityName + ": found matching target.  Will set status to inactive: " + count);

                                try
                                {
                                    SetStateRequest ssr = new SetStateRequest();
                                    ssr.EntityMoniker = new EntityReference(entityName, sourceEntity.Id);
                                    ssr.Status = new OptionSetValue(-1);//let system decide
                                    ssr.State = new OptionSetValue(Convert.ToInt32(1));//inactive
                                    crmServiceTarget.Execute(ssr);
                                }
                                catch (Exception e)
                                {
                                    logger.error(entityName + " : " + e.Message);
                                }
                            }

                            count++;
                        }

                    }
                    if (entityCollection.MoreRecords)
                    {
                        // Increment the page number to retrieve the next page.
                        qe.PageInfo.PageNumber++;
                        // Set the paging cookie to the paging cookie returned from current results.
                        qe.PageInfo.PagingCookie = entityCollection.PagingCookie;
                    }
                    else
                    {
                        // If no more records are in the result nodes, exit the loop.
                        break;
                    }
                }//end of while
            }


            //deal with food likes for menu presets
            
            qe2 = new QueryExpression();
            qe2.EntityName = "dc_foodlike";
            qe2.ColumnSet = new ColumnSet(true);

            qe2.PageInfo = new PagingInfo();
            qe2.PageInfo.Count = fetchCount;
            qe2.PageInfo.PageNumber = pageNumber;


            //Make sure the entity is active
            ce2 = new ConditionExpression();
            ce2.AttributeName = "statecode";
            ce2.Values.Add(0);//active
            ce2.Operator = ConditionOperator.Equal;

            ConditionExpression ce3 = new ConditionExpression();
            ce3.AttributeName = "dc_menupresetid";
            ce3.Operator = ConditionOperator.NotNull;

            fe2 = new FilterExpression();
            fe2.Conditions.Add(ce2);
            fe2.Conditions.Add(ce3);
            qe2.Criteria = fe2;

            // The current paging cookie. When retrieving the first page, 
            // pagingCookie should be null.
            qe2.PageInfo.PagingCookie = null;
            //run query

            while (true)
            {
                EntityCollection entityCollection = crmServiceSource.RetrieveMultiple(qe2);
                if (entityCollection.Entities != null)
                {

                    for (int x = 0; x < entityCollection.Entities.Count(); x++)
                    {
                        Entity entity = entityCollection.Entities[x];
                        //See if exists first
                        Entity existingFoodLike = crmServiceTarget.Retrieve("dc_foodlike", entity.Id, new ColumnSet(new String[] { "dc_foodlikeid" }));
                        
                        if (existingFoodLike != null)//need to update
                        {
                            crmServiceTarget.Update(entity);
                            logger.debug("Updated food like: " + entity.Id.ToString());
                        }else {

                            crmServiceTarget.Create(entity);
                            logger.debug("Creating food like: " + entity.Id.ToString());
                        }
                    }
                }
                if (entityCollection.MoreRecords)
                {
                    // Increment the page number to retrieve the next page.
                    qe2.PageInfo.PageNumber++;
                    // Set the paging cookie to the paging cookie returned from current results.
                    qe2.PageInfo.PagingCookie = entityCollection.PagingCookie;
                }
                else
                {
                    // If no more records are in the result nodes, exit the loop.
                    break;
                }
            }
            
        }
        private static bool DoesRelationshipExist(IOrganizationService crmService, string relationshipSchemaName,
            string entity1SchemaName, Guid entity1KeyValue, string entity2SchemaName, Guid entity2KeyValue)
        {
            // Assemble FetchXML to query the intersection entity directly
            string fetchXml = "<fetch mapping='logical' no-lock='true' > <entity name='" + relationshipSchemaName + "'>"
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
