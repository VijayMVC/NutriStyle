using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using DynamicConnections.CRM2011.CustomAssemblies.Utilities;
using Microsoft.Xrm.Sdk.Query;
using System.ServiceModel;
using DynamicConnections.NutriStyle.CRM2011.Plugins.Helpers;

namespace DynamicConnections.NutriStyle.CRM2011.Plugins
{
    /// <summary>
    /// Need to properly manage the shopping list when the meal food gets deleted.  Register against dc_mealfood; pre-validate.  
    /// Needs a preimage named 'preimage'
    /// </summary>
    public class MealFoodPostDelete : IPlugin
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
                logger.debug("MealFoodPostDelete: starting: " + pluginExecutionContext.PrimaryEntityName + ": " + pluginExecutionContext.MessageName);

                Entity preImage = (Entity)pluginExecutionContext.PreEntityImages["preimage"];


                if (pluginExecutionContext.PreEntityImages.Contains("preimage"))
                {


                    String fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                      <entity name='dc_mealfood'>
                        <attribute name='dc_foodid' />
                        <attribute name='dc_portionsize' />
                        <filter type='and'>
                          <condition attribute='statecode' operator='eq' value='0' />
                          <condition attribute='dc_mealfoodid' operator='eq' value='@ID' />
                        </filter>
                        <link-entity name='dc_foods' from='dc_foodsid' to='dc_foodid' alias='dc_foods'>
                            <attribute name='dc_recipefood'/>
                        </link-entity>
                        <link-entity name='dc_meal' from='dc_mealid' to='dc_mealid' alias='aa'>
	                        <attribute name='dc_meal' />	
                          <link-entity name='dc_day' from='dc_dayid' to='dc_dayid' alias='ab'>
                                <attribute name='dc_menuid' />
	                            <attribute name='dc_day' />
                                <link-entity name='dc_menu' from='dc_menuid' to='dc_menuid' alias='ac'>
                                    <attribute name='dc_contactid' />
                                </link-entity>
                          </link-entity>
                        </link-entity>
                      </entity>
                    </fetch>";

                    fetchXml = fetchXml.Replace("@ID", preImage.Id.ToString());
                    logger.debug("Fetching meal food with id: " + preImage.Id.ToString());
                    EntityCollection list = crmService.RetrieveMultiple(new FetchExpression(fetchXml));

                    if (list != null && list.Entities.Count() > 0)
                    {
                        Guid menuId = Guid.Empty;
                        Guid contactId = Guid.Empty;
                        Guid foodId = Guid.Empty;
                        decimal portionSize = 0m;
                        bool isRecipe = false;
                        int day = -1;
                        int meal = -1;

                        Entity mealFood = list.Entities[0];


                        if (mealFood.Contains("dc_foodid"))
                        {
                            foodId = ((EntityReference)mealFood["dc_foodid"]).Id;
                        }

                        if (mealFood.Contains("dc_portionsize"))
                        {
                            portionSize = (decimal)mealFood["dc_portionsize"];
                        }
                        if (mealFood.Contains("ac.dc_contactid"))
                        {
                            contactId = ((EntityReference)((AliasedValue)mealFood["ac.dc_contactid"]).Value).Id;
                        }

                        if (mealFood.Contains("ab.dc_menuid"))
                        {
                            menuId = ((EntityReference)((AliasedValue)mealFood["ab.dc_menuid"]).Value).Id;
                        }
                        if (mealFood.Contains("ab.dc_day"))
                        {
                            day = ((OptionSetValue)((AliasedValue)mealFood["ab.dc_day"]).Value).Value;
                        }

                        if (mealFood.Contains("aa.dc_meal"))
                        {
                            meal = ((OptionSetValue)((AliasedValue)mealFood["aa.dc_meal"]).Value).Value;
                        }
                        if (mealFood.Contains("dc_foods.dc_recipefood"))
                        {
                            isRecipe = (bool)(((AliasedValue)mealFood["dc_foods.dc_recipefood"]).Value);
                        }
                        logger.debug("portionSize: " + portionSize);
                        logger.debug("menuId: " + menuId);
                        logger.debug("meal: " + meal);
                        logger.debug("day: " + day);
                        logger.debug("contactId: " + contactId);
                        logger.debug("foodId: " + foodId);
                        logger.debug("isRecipe: " + isRecipe);

                        //Now find parent shopping list

                        fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                          <entity name='dc_shoppinglist'>
                            <attribute name='dc_shoppinglistid' />
                            <filter type='and'>
                              <condition attribute='dc_menuid' operator='eq' value='@MENUID' />
                              <condition attribute='dc_contactid' operator='eq' value='@CONTACTID' />
                            </filter>
                          </entity>
                        </fetch>";

                        fetchXml = fetchXml.Replace("@MENUID", menuId.ToString());
                        fetchXml = fetchXml.Replace("@CONTACTID", contactId.ToString());

                        list = crmService.RetrieveMultiple(new FetchExpression(fetchXml));
                        Guid shoppingListId = Guid.Empty;
                        if (list != null && list.Entities.Count() > 0)
                        { 
                            Entity shoppingList = list.Entities[0];
                            shoppingListId = shoppingList.Id;

                            if (isRecipe)
                            {
                                logger.debug("Found shopping list.  Looking for matching recipe");
                                //Find the shoppinglisthelper stuff
                                logger.debug("Processing recipe: foodId: " + foodId);
                                fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical'>
                                  <entity name='dc_ingredient'>
                                    <attribute name='dc_foodingredientid'  />
                                    <attribute name='dc_portiontypeid' />
                                    <attribute name='dc_portionsize'  />
                                    <link-entity name='dc_foods' from='dc_foodsid' to='dc_foodid' alias='dc_foods'>
                                        <attribute name='dc_portion_amount'  />
                                        <link-entity name='dc_mealfood' from='dc_foodid' to='dc_foodsid' alias='dc_mealfood'>
                                            <attribute name='dc_portionsize'  />
                                            <filter type='and'>
                                                <condition attribute='dc_mealfoodid' operator='eq' value='@MEALFOODID' />
                                            </filter>
                                        </link-entity>
                                    </link-entity>
                                    <filter type='and'>
                                        <condition attribute='dc_foodid' operator='eq' value='@FOODID' />
                                    </filter>
                                  </entity>
                                </fetch>";

                                fetchXml = fetchXml.Replace("@FOODID", foodId.ToString());
                                fetchXml = fetchXml.Replace("@MEALFOODID", preImage.Id.ToString());

                                EntityCollection ec = crmService.RetrieveMultiple(new FetchExpression(fetchXml));

                                Dictionary<Guid, ShoppingListHelper> slhList = new Dictionary<Guid, ShoppingListHelper>();
                                foreach (Entity mf in ec.Entities)
                                {

                                    //multiplier
                                    decimal pSize = (decimal)(((AliasedValue)mf.Attributes["dc_mealfood.dc_portionsize"]).Value);
                                    double orgPortionSize = (double)(((AliasedValue)mf.Attributes["dc_foods.dc_portion_amount"]).Value);
                                    decimal multiplier = pSize / Convert.ToDecimal(orgPortionSize);
                                    //logger.debug("multiplier: " + multiplier);

                                    ShoppingListHelper slh = new ShoppingListHelper();
                                    slh.Name = ((EntityReference)mf.Attributes["dc_foodingredientid"]).Name;
                                    slh.FoodId = ((EntityReference)mf.Attributes["dc_foodingredientid"]).Id;
                                    slh.Portionsize = ((decimal)mf.Attributes["dc_portionsize"]) * multiplier;
                                    slh.PortionTypeId = ((EntityReference)mf.Attributes["dc_portiontypeid"]).Id;

                                    if (slhList.ContainsKey(slh.FoodId))
                                    {
                                        slhList[slh.FoodId].Portionsize += slh.Portionsize;
                                    }
                                    else
                                    {
                                        slhList.Add(slh.FoodId, slh);
                                    }
                                    logger.debug("Found ingredient: " + slh.Name);
                                }
                                foreach (KeyValuePair<Guid, ShoppingListHelper> pair in slhList)
                                {
                                    logger.debug("Removing recipe: foodId: " + pair.Value.ToString() + ":" + pair.Value.Name);
                                    ProcessItem(shoppingListId, pair.Key, crmService, pair.Value.Portionsize);
                                }
                            }
                            else
                            {
                                ProcessItem(shoppingListId, foodId, crmService, portionSize);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.error("Execute Method");
                logger.error(ex.Message);
                logger.error(ex.StackTrace);
            }
        }
        private void ProcessItem(Guid shoppingListId, Guid foodId, IOrganizationService crmService, decimal portionSize)
        {
            logger.debug("Found shopping list.  Looking for matching food");

            String fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                  <entity name='dc_shoppinglistitem'>
                                    <attribute name='dc_shoppinglistitemid' />
                                    <attribute name='dc_portionsize' />
                                    <filter type='and'>
                                      <condition attribute='dc_shoppinglistid' operator='eq' value='@SHOPPINGLISTID' />
                                      <condition attribute='dc_foodid' operator='eq' value='@FOODID' />
                                    </filter>
                                  </entity>
                                </fetch>";


            fetchXml = fetchXml.Replace("@SHOPPINGLISTID", shoppingListId.ToString());
            fetchXml = fetchXml.Replace("@FOODID", foodId.ToString());

            EntityCollection list = crmService.RetrieveMultiple(new FetchExpression(fetchXml));

            if (list != null && list.Entities.Count() > 0)
            {

                Entity shoppingListItem = (Entity)list.Entities[0];
                decimal itemPortionSize = 0m;
                if (shoppingListItem.Contains("dc_portionsize"))
                {
                    itemPortionSize = (decimal)shoppingListItem["dc_portionsize"];
                    logger.debug("Shopping List Item portion size: " + itemPortionSize);
                    logger.debug("Found foodId in shopping list.  Need to remove " + portionSize + " portions from it");

                    if ((itemPortionSize - portionSize) <= 0m)
                    {
                        logger.debug("Need to remove the shopping list item");
                        crmService.Delete("dc_shoppinglistitem", shoppingListItem.Id);
                    }
                    else
                    {
                        logger.debug("Need to update the shopping list item");

                        Entity entity = new Entity("dc_shoppinglistitem");
                        entity["dc_shoppinglistitemid"] = shoppingListItem.Id;
                        entity["dc_portionsize"] = itemPortionSize - portionSize;
                        logger.debug("Setting portion size to: " + (itemPortionSize - portionSize));
                        crmService.Update(entity);
                    }
                }
            }
            else
            {
                logger.debug("foodId not found in shopping list");
            }
        }
    }
}
