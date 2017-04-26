
using System;
using System.Collections.Generic;

using Microsoft.Xrm.Sdk;
using DynamicConnections.CRM2011.CustomAssemblies.Utilities;
using Microsoft.Xrm.Sdk.Query;
using System.ServiceModel;
using DynamicConnections.NutriStyle.CRM2011.Plugins.Helpers;
using Microsoft.Xrm.Sdk.Messages;

namespace DynamicConnections.NutriStyle.CRM2011.Plugins
{
    /// <summary>
    /// Fires when there are more than four attributes in the collection and the dc_sub_categories is populated.  
    /// Register against the post image on create and update
    /// </summary>
    public class FoodPost : IPlugin
    {
        Logger logger;
        private IOrganizationService crmService;

        private Entity food = null;

        public void Execute(IServiceProvider serviceProvider)
        {
            logger = new Logger("DEBUG", @"c:\windows\temp\nutristyle_plugins_output.txt", 1);
            IPluginExecutionContext pluginExecutionContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            // Obtain the organization service reference.
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            crmService = serviceFactory.CreateOrganizationService(pluginExecutionContext.UserId);

            try
            {
                logger.debug("----------------------------------------");
                logger.debug("FoodPost: Starting: " + pluginExecutionContext.PrimaryEntityName);

                // get the Entity contact
                food = (Entity)pluginExecutionContext.InputParameters["Target"];

                if (food.Attributes.Count > 4 && food.Contains("dc_sub_categories"))
                {
                    //Need to create a mtm relationship to the sub category.
                    Guid foodId = food.Id;
                    //Find the sub category
                    String fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                      <entity name='dc_meal_component'>
                        <attribute name='dc_meal_componentid' />
                        <order attribute='dc_name' descending='false' />
                        <filter type='and'>
                            <condition attribute='dc_name' operator='in'>
                            @VALUE
                            </condition>
                        </filter>
                      </entity>
                    </fetch>";

                    String[] values = ((String)food["dc_sub_categories"]).Split(';');
                    String value = String.Empty;
                    foreach (String str in values)
                    {
                        value += "<value>" + str + "</value>";
                    }
                    fetchXml = fetchXml.Replace("@VALUE", value);
                    logger.debug("value: " + value);
                    logger.debug("fetchXml: " + fetchXml);

                    EntityCollection list = crmService.RetrieveMultiple(new FetchExpression(fetchXml));

                    if (list != null && list.Entities.Count > 0)
                    {
                        foreach (Entity subCategory in list.Entities)
                        {
                            //check to see if MTM relationship exists
                            bool doesRelationshipExist = false;
                               
                            if (pluginExecutionContext.MessageName.Equals("update", StringComparison.OrdinalIgnoreCase))
                            {
                                fetchXml = "<fetch mapping='logical' no-lock='true' > <entity name='dc_dc_foods_dc_meal_component'>"
                                    + "<all-attributes />"
                                    + "<filter>"
                                    + "<condition attribute='dc_foodsid' operator='eq' value ='" + foodId.ToString() + "' />"
                                    + "<condition attribute='dc_meal_componentid' operator='eq' value='" + subCategory.Id.ToString() + "' />"
                                    + "</filter>"
                                    + "</entity>"
                                    + "</fetch>";

                                // Perform the query
                                EntityCollection collection = crmService.RetrieveMultiple(new FetchExpression(fetchXml));

                                if (collection != null && collection.Entities.Count > 0)
                                {
                                    doesRelationshipExist = true;
                                }
                            }

                            logger.debug("Found sub category: " + subCategory.Id);
                            if (!doesRelationshipExist)
                            {
                                //Create MTM

                                //Create relationship
                                AssociateRequest request = new AssociateRequest()
                                {
                                    Target = new EntityReference("dc_foods", foodId),
                                    RelatedEntities = new EntityReferenceCollection
                                {
                                    new EntityReference("dc_meal_component", subCategory.Id)
                                },
                                    Relationship = new Microsoft.Xrm.Sdk.Relationship("dc_dc_foods_dc_meal_component")
                                };
                                crmService.Execute(request);
                                logger.debug("Added the sub category relationship");
                            }
                            else
                            {
                                logger.debug("Relationship already exists.  Skipping create");
                            }
                        }
                    }

                }
                else
                {
                    logger.debug("Food attributes count: " + food.Attributes.Count);
                    logger.debug("Does food contain dc_sub_categories: " + food.Contains("dc_sub_categories"));
                }
            }
            catch (Exception ex)
            {
                logger.error("Execute Method");
                logger.error(ex.Message);
                logger.error(ex.StackTrace);
            }
        }
    }
}