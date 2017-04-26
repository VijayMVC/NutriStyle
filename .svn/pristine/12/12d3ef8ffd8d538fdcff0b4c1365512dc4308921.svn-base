using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using DynamicConnections.CRM2011.CustomAssemblies.Utilities;
using Microsoft.Xrm.Sdk.Query;
using System.ServiceModel;
using Microsoft.Xrm.Sdk.Messages;

namespace DynamicConnections.NutriStyle.CRM2011.Plugins
{
    public class CloneFoodPreUpdate : IPlugin
    {
        //Tools tools = new Tools("nutristyle_plugins_output");
        Logger logger;

        public void Execute(IServiceProvider serviceProvider)
        {
            try
            {

                logger = new Logger("DEBUG", @"c:\windows\temp\nutristyle_plugins_output.txt", 1);
                // get the execution context from the service provider temp code to makea  change
                IPluginExecutionContext pluginExecutionContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

                // Obtain the organization service reference.
                IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService crmService = serviceFactory.CreateOrganizationService(pluginExecutionContext.UserId);
                logger.debug("CloneFoodPreUpdate: starting: " + pluginExecutionContext.PrimaryEntityName + ": " + pluginExecutionContext.MessageName);

                Entity preImage = (Entity)pluginExecutionContext.PreEntityImages["preimage"];
                Entity target = (Entity)pluginExecutionContext.InputParameters["Target"];
                
                if (target.Contains("dc_clone_food"))
                {
                    logger.debug("Check: value of dc_clone_food: " + target["dc_clone_food"]);

                    if ((Boolean)target["dc_clone_food"])
                    {
                        target["dc_clone_food"] = false;

                        Guid newId = Guid.NewGuid();
                        logger.debug("Condition: dc_clone_food is checked. Cloning the food: " + preImage["dc_name"]);
                        //Need to create a new entity.
                        Entity cloneFood = crmService.Retrieve("dc_foods", preImage.Id, new ColumnSet(true));//This gets all the relationships



                        cloneFood["dc_foodsid"] = newId;
                        cloneFood.Id = newId;
                        preImage["dc_clone_food"] = false;
                        
                        Boolean cloneNutrient = false;

                        cloneFood["dc_name"] = "Copy " + cloneFood["dc_name"];
                        Guid nutrientId = new Guid();

                        if (cloneFood.Contains("dc_foodnutrientid"))
                        {
                            logger.debug("Condition: Contains a dc_foodnutrientid. Store guid and set clone to true");
                            // Store the Guid of the nutrient if it exists
                            nutrientId = ((EntityReference)cloneFood["dc_foodnutrientid"]).Id;
                            cloneNutrient = true;
                            logger.debug("Condition: finished Guid copy and cloneNutrient to true");
                            cloneFood["dc_foodnutrientid"] = newId;
                        }

                        if (cloneNutrient)
                        {
                            // clone the nutrient
                            logger.debug("Condition: Nutrient for the food exists, Cloning nutrient");

                            Entity nutrient = crmService.Retrieve("dc_food_nutrients", nutrientId, new ColumnSet(true));

                            nutrient["dc_food_nutrientsid"] = newId;
                            nutrient.Id = newId;
                            nutrient["dc_name"] = "Copy - " + nutrient["dc_name"];
                            //create the nutrient and set the food dc_foodnutrientid to the new guid
                            Guid nId = crmService.Create(nutrient);
                            cloneFood["dc_foodnutrientid"] = new EntityReference("dc_food_nutrients", nId);
                            logger.debug("Success: Nutrient created");
                        }
                        else
                        {
                            logger.debug("No nutrient information on the food");
                        }

                        //create the Food
                        logger.debug("Creating food");
                        Guid food = crmService.Create(cloneFood);
                        logger.debug("Complete: Clone has executed successfully: "+food+":"+cloneFood.Id);
                        //Deal with ingredients
                        String fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                          <entity name='dc_ingredient'>
                            <all-attributes />
                            <filter type='and'>
                              <condition attribute='dc_foodid' operator='eq' value='@FOODID' />
                            </filter>
                          </entity>
                        </fetch>";

                        fetchXml = fetchXml.Replace("@FOODID", preImage.Id.ToString());

                        EntityCollection collection = crmService.RetrieveMultiple(new FetchExpression(fetchXml));

                        if (collection != null && collection.Entities.Count > 0)
                        {
                            foreach (Entity entity in collection.Entities)
                            {
                                entity.Id = Guid.NewGuid();
                                entity["dc_ingredientid"] = entity.Id;
                                entity["dc_foodid"] = new EntityReference("dc_foods", food);
                                crmService.Create(entity);
                                logger.debug("Created ingredient: " + entity.Id.ToString());

                            }
                        }
                        logger.debug("Done with ingredients");
                        //deal with MtM relationship
                        fetchXml = "<fetch mapping='logical' no-lock='true' > <entity name='dc_dc_foods_dc_meal_component'>"
                            + "<all-attributes />"
                            + "<filter>"
                            + "<condition attribute='dc_foodsid' operator='eq' value ='" + preImage.Id.ToString() + "' />"
                            + "</filter>"
                            + "</entity>"
                            + "</fetch>";

                        // Perform the query
                        collection = crmService.RetrieveMultiple(new FetchExpression(fetchXml));
                        if (collection != null && collection.Entities.Count > 0)
                        {
                            logger.debug("Found " + collection.Entities.Count + " MtM relationships that need cloned");
                            foreach (Entity entity in collection.Entities)
                            {
                                if(entity.Contains("dc_meal_componentid")) {
                                    Guid subCategoryId = (Guid)entity["dc_meal_componentid"];
                                    logger.debug("subCategoryId: " + subCategoryId);
                                    logger.debug("cloneFood.Id: " + cloneFood.Id);
                                    //Create relationship
                                    AssociateRequest request = new AssociateRequest()
                                    {
                                        RequestId = Guid.NewGuid(),
                                        Target = new EntityReference("dc_foods", cloneFood.Id),
                                        RelatedEntities = new EntityReferenceCollection
                                        {
                                            new EntityReference("dc_meal_component", subCategoryId)
                                        },
                                        Relationship = new Microsoft.Xrm.Sdk.Relationship("dc_dc_foods_dc_meal_component")
                                    };
                                    logger.debug("Creating relationship");
                                    //crmService.Execute(request);
                                    logger.debug("Created relationship");
                                }
                            }
                        }
                    }
                    else
                    {
                        logger.debug("Condition: dc_clone_food is not checked, not cloning the food");
                    }
                }
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
                logger.error("Execute Method");
                logger.error(ex.Message);
                logger.error(ex.StackTrace);
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
