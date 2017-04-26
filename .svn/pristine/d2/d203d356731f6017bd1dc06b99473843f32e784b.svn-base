using System;

using Microsoft.Xrm.Sdk;
using DynamicConnections.CRM2011.CustomAssemblies.Utilities;
using Microsoft.Xrm.Sdk.Query;
using System.ServiceModel;
using Microsoft.Xrm.Sdk.Messages;

namespace DynamicConnections.NutriStyle.CRM2011.Plugins
{
    public class ClonePresetPreUpdate : IPlugin
    {
        //Tools tools = new Tools("nutristyle_plugins_output");
        Logger logger;
        public void Execute(IServiceProvider serviceProvider)
        {
            logger = new Logger("DEBUG", @"c:\windows\temp\nutristyle_plugins_output.txt", 1);
            try
            {
                // get the execution context from the service provider temp code to makea  change
                IPluginExecutionContext pluginExecutionContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

                // Obtain the organization service reference.
                IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService crmService = serviceFactory.CreateOrganizationService(pluginExecutionContext.UserId);
                logger.debug("ClonePresetPreUpdate: starting: " + pluginExecutionContext.PrimaryEntityName + ": " + pluginExecutionContext.MessageName);

                Entity preImage = (Entity)pluginExecutionContext.PreEntityImages["preimage"];
                Entity target = (Entity)pluginExecutionContext.InputParameters["Target"];

                if (target.Contains("dc_clonepreset"))
                {
                    logger.debug("Check: value of dc_clonepreset: " + target["dc_clonepreset"]);

                    if ((Boolean)target["dc_clonepreset"])
                    {
                        target["dc_clonepreset"] = false;

                        Guid newId = Guid.NewGuid();
                        logger.debug("Condition: dc_clonepreset is checked. Cloning the preset: " + preImage["dc_name"]);
                        //Need to create a new entity.
                        Entity clonePreset = crmService.Retrieve("dc_presets", preImage.Id, new ColumnSet(true));//This gets all the relationships

                        clonePreset["dc_presetsid"] = newId;
                        clonePreset.Id = newId;
                        preImage["dc_clonepreset"] = false;

                        clonePreset["dc_name"] = "Copy " + clonePreset["dc_name"];

                        crmService.Create(clonePreset);
                        logger.debug("Cloned preset");

                        #region sub categories
                        //Find all the food likes
                        String fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                          <entity name='dc_component_template_sel'>
                            <all-attributes/>
                            <filter type='and'>
                              <condition attribute='dc_menupresetid' operator='eq' value='@ID' />
                            </filter>
                          </entity>
                        </fetch>";

                        fetchXml = fetchXml.Replace("@ID", preImage.Id.ToString());

                        EntityCollection collection = crmService.RetrieveMultiple(new FetchExpression(fetchXml));
                        if (collection != null && collection.Entities.Count > 0)
                        {
                            foreach (Entity subCategory in collection.Entities)
                            {

                                Entity newSubCategory = subCategory;
                                newSubCategory["dc_menupresetid"] = new EntityReference("dc_presets", newId);
                                newSubCategory.Id = Guid.NewGuid();
                                newSubCategory["dc_component_template_selid"] = newSubCategory.Id;
                                crmService.Create(newSubCategory);
                                logger.debug("Added subcategory: " + newSubCategory.Id);
                            }
                        }
                        #endregion

                        #region food likes
                        //Find all the food likes
                        fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                          <entity name='dc_foodlike'>
                            <all-attributes/>
                            <filter type='and'>
                              <condition attribute='dc_menupresetid' operator='eq' value='@ID' />
                            </filter>
                          </entity>
                        </fetch>";

                        fetchXml = fetchXml.Replace("@ID", preImage.Id.ToString());

                        collection = crmService.RetrieveMultiple(new FetchExpression(fetchXml));
                        if (collection != null && collection.Entities.Count > 0)
                        {
                            foreach (Entity foodLike in collection.Entities)
                            {

                                Entity newFoodLike = foodLike;
                                newFoodLike["dc_menupresetid"] = new EntityReference("dc_presets", newId);
                                newFoodLike.Id = Guid.NewGuid();
                                newFoodLike["dc_foodlikeid"] = newFoodLike.Id;
                                crmService.Create(newFoodLike);
                                logger.debug("Added food like: " + newFoodLike.Id);
                            }
                        } 
                        #endregion

                        #region food dislikes
                        //deal with MtM relationship
                        fetchXml = "<fetch mapping='logical' no-lock='true' > <entity name='dc_dc_presets_dc_meal_component'>"
                            + "<all-attributes />"
                            + "<filter>"
                            + "<condition attribute='dc_presetsid' operator='eq' value ='" + preImage.Id.ToString() + "' />"
                            + "</filter>"
                            + "</entity>"
                            + "</fetch>";

                        // Perform the query
                        collection = crmService.RetrieveMultiple(new FetchExpression(fetchXml));
                        if (collection != null && collection.Entities.Count > 0)
                        {
                            foreach (Entity entity in collection.Entities)
                            {
                                if (entity.Contains("dc_meal_componentid"))
                                {
                                    Guid subCategoryId = (Guid)entity["dc_meal_componentid"];
                                    logger.debug("subCategoryId: " + subCategoryId);
                                    //Create relationship
                                    AssociateRequest request = new AssociateRequest()
                                    {
                                        Target = new EntityReference("dc_presets", clonePreset.Id),
                                        RelatedEntities = new EntityReferenceCollection
                                        {
                                            new EntityReference("dc_meal_component", subCategoryId)
                                        },
                                        Relationship = new Microsoft.Xrm.Sdk.Relationship("dc_dc_presets_dc_meal_component")
                                    };
                                    crmService.Execute(request);
                                }
                            }
                        }
                        #endregion
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
