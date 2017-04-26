using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using DynamicConnections.CRM2011.CustomAssemblies.Utilities;

using Microsoft.Xrm.Sdk.Query;

namespace DynamicConnections.NutriStyle.CRM2011.Plugins


{
    public class FoodToSubCategory_MTM : IPlugin
    {

        Logger logger;
        public void Execute(IServiceProvider serviceProvider)
        {
            StringBuilder sb = new StringBuilder();
            logger = new Logger("DEBUG", @"c:\windows\temp\nutristyle_plugins_output.txt", 1);
            try
            {
                // Obtain the execution context from the service provider.
                IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

                // Obtain the organization service reference.
                IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService crmService = serviceFactory.CreateOrganizationService(context.UserId);

                EntityReference dc_foodsid = (EntityReference)context.InputParameters["Target"];
                EntityReference dc_meal_component = ((EntityReferenceCollection)context.InputParameters["RelatedEntities"])[0];

                logger.debug("dc_foodsid type: " + dc_foodsid.LogicalName);//foodid
                logger.debug("dc_meal_component type: " + dc_meal_component.LogicalName);//dc_meal_componetnid
                if (dc_foodsid.LogicalName.Equals("dc_foods", StringComparison.OrdinalIgnoreCase))
                {
                    Guid foodId = dc_foodsid.Id;

                    // Assemble FetchXML to query the intersection entity directly
                    string fetchXmlrelationship = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                     <entity name='dc_dc_foods_dc_meal_component'>
                         <all-attributes />
                         <filter>
                            <condition attribute='dc_foodsid' operator='eq' value ='@FOODID' />
                         </filter>
                         </entity>
                     </fetch>";

                    fetchXmlrelationship = fetchXmlrelationship.Replace("@FOODID", foodId.ToString());
                    EntityCollection relationship = null;
                    //EntityCollection objects = null;
                    try
                    {
                        relationship = crmService.RetrieveMultiple(new FetchExpression(fetchXmlrelationship));
                        //objects = organizationServiceProxy.RetrieveMultiple(new FetchExpression(fetchXmlobjects));
                    }
                    catch (Exception ex)
                    {
                        logger.error(ex.Message);
                        logger.error(ex.StackTrace);
                    }
                    //if (relationship != null && relationship.Entities.Count > 0)
                    foreach (Entity foodsMealComponent in relationship.Entities)
                    {

                        //get values
                        Guid foodsMealComponentId = (Guid)foodsMealComponent["dc_meal_componentid"];

                        string fetchXmlobjects = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                          <entity name='dc_meal_component'>
                            <attribute name='dc_name' />
                            <filter> 
                                <condition operator='eq' attribute='dc_meal_componentid' value='@DC_FOODSMEALCOMPONENT' />
                            </filter>
                          </entity>
                        </fetch>";

                        fetchXmlobjects = fetchXmlobjects.Replace("@DC_FOODSMEALCOMPONENT", foodsMealComponentId.ToString());

                        EntityCollection subCategories = null;

                        try
                        {
                            subCategories = crmService.RetrieveMultiple(new FetchExpression(fetchXmlobjects));
                            //objects = organizationServiceProxy.RetrieveMultiple(new FetchExpression(fetchXmlobjects));
                        }
                        catch (Exception ex)
                        {
                            logger.error(ex.Message);
                            logger.error(ex.StackTrace);
                        }
                        if (subCategories != null && subCategories.Entities.Count > 0)
                        {
                            Entity subCategory = (Entity)subCategories.Entities[0];

                            String name = (String)subCategory["dc_name"];
                            sb.Append(name);
                            sb.Append(";");
                        }
                    }//end of for loop
                    //trim string
                    if (sb.Length > 0)
                    {
                        sb.Remove(sb.Length - 1, 1);//remove the last comma
                    }
                    Console.WriteLine("Sub categories: " + sb.ToString());

                    //update CRM
                    Entity food = new Entity("dc_foods");
                    food.Id = foodId;//This will get passed in by the mtm
                    food.Attributes["dc_sub_categories"] = sb.ToString();
                    crmService.Update(food);
                    //all done

                }
            }
            catch (Exception ex)
            {
                logger.error(ex.Message);
                logger.error(ex.StackTrace);
            }
        }
    }
}
