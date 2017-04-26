﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;
using DynamicConnections.CRM2011.CustomAssemblies.Utilities;
using Microsoft.Xrm.Sdk.Query;

using DynamicConnections.NutriStyle.CRM2011.Plugins.Helpers;

namespace DynamicConnections.NutriStyle.CRM2011.Plugins
{
    /// <summary>
    /// See if shoppinglist needs updated or if food log needs created.  Register against the post create.  Needs a postimage named 'postimage'
    /// Also register against the post update.  Needs both a preimage and a postimage.  Have to get the pre portion size.
    /// </summary>
    public class MealFoodPostUpdate : IPlugin
    {
        //Tools tools = new Tools("nutristyle_plugins_output");
        Logger logger;
        public void Execute(IServiceProvider serviceProvider)
        {
            logger = new Logger("DEBUG", @"c:\windows\temp\nutristyle_plugins_output.txt", 1);
            try
            {
                // get the execution context from the service provider temp code to makea  change
                IPluginExecutionContext pluginExecutionContext  = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

                // Obtain the organization service reference.
                IOrganizationServiceFactory serviceFactory      = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService crmService                 = serviceFactory.CreateOrganizationService(pluginExecutionContext.UserId);
                logger.debug("MealFoodPostUpdate: starting: " + pluginExecutionContext.PrimaryEntityName + ": " + pluginExecutionContext.MessageName);

                Entity preImage = null;
                Entity postImage = null;
                if (pluginExecutionContext.PostEntityImages.Count() > 0)
                {
                    postImage = (Entity)pluginExecutionContext.PostEntityImages["postimage"];
                }
                if (pluginExecutionContext.PreEntityImages.Count() > 0)
                {
                    preImage = (Entity)pluginExecutionContext.PreEntityImages["preimage"];
                }

                Guid preFoodId          = Guid.Empty;
                Guid postFoodId         = Guid.Empty;
                decimal prePortionSize  = 0m;

                if (preImage != null && preImage.Contains("dc_foodid"))
                {
                    preFoodId = ((EntityReference)preImage["dc_foodid"]).Id;
                }
                if (preImage != null && preImage.Contains("dc_portionsize"))
                {
                    prePortionSize = (decimal)preImage["dc_portionsize"];
                }

                if (postImage != null && postImage.Contains("dc_foodid"))
                {
                    postFoodId = ((EntityReference)postImage["dc_foodid"]).Id;
                }

                //See if added by user (dc_addedbyuser)
                //if (postImage.Contains("dc_addedbyuser") && (bool)postImage["dc_addedbyuser"]) {
                //Add to shopping list and to food log
                String fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                      <entity name='dc_mealfood'>
                        <attribute name='dc_name' />
                        <attribute name='dc_foodid' />
                        <attribute name='dc_fat' />
                        <attribute name='dc_protein' />
                        <attribute name='dc_carbohydrate' />
                        <attribute name='dc_alcohol' />
                        <attribute name='dc_portionsize' />
                        <attribute name='dc_portiontypeid' />
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
                                    <link-entity name='contact' from='contactid' to='dc_contactid' alias='contact'>
                                        <attribute name='dc_parentcontactid' />
                                        <attribute name='dc_rollshoppinglisttoparent' />
                                    </link-entity>    
                                </link-entity>
                          </link-entity>
                        </link-entity>
                      </entity>
                    </fetch>";

                fetchXml = fetchXml.Replace("@ID", postImage.Id.ToString());
                logger.debug("Fetching meal food with id: " + postImage.Id.ToString());
                EntityCollection list = crmService.RetrieveMultiple(new FetchExpression(fetchXml));

                if (list != null && list.Entities.Count() > 0)
                {

                    logger.debug("MealFood found: " + list.Entities[0].Id.ToString());

                    Guid menuId         = Guid.Empty;
                    Guid portionTypeId  = Guid.Empty;
                    Guid contactId      = Guid.Empty;
                    Guid foodId         = Guid.Empty;
                    Guid parentContactId= Guid.Empty;
                    bool rollupList     = false;

                    int day             = -1;
                    int meal            = -1;

                    decimal fat             = 0m;
                    decimal protein         = 0m;
                    decimal carboydrate     = 0m;
                    decimal portionSize     = 0m;
                    decimal alcohol         = 0m;

                    bool isRecipe       = false;
                    DateTime dc_date = DateTime.MinValue;

                    String foodName     = String.Empty;
                    Entity mealFood     = list.Entities[0];

                    if (mealFood.Contains("dc_fat"))
                    {
                        fat = (decimal)mealFood["dc_fat"];
                    }
                    if (mealFood.Contains("dc_protein"))
                    {
                        protein = (decimal)mealFood["dc_protein"];
                    }
                    if (mealFood.Contains("dc_carbohydrate"))
                    {
                        carboydrate = (decimal)mealFood["dc_carbohydrate"];
                    }
                    if (mealFood.Contains("dc_alcohol"))
                    {
                        alcohol = (decimal)mealFood["dc_alcohol"];
                    }
                    if (mealFood.Contains("dc_foodid"))
                    {
                        foodId = ((EntityReference)mealFood["dc_foodid"]).Id;
                        if (((EntityReference)mealFood["dc_foodid"]).Name != null)
                        {
                            foodName = ((EntityReference)mealFood["dc_foodid"]).Name;
                        }
                    }
                  
                    if (mealFood.Contains("dc_portionsize"))
                    {
                        portionSize = (decimal)mealFood["dc_portionsize"];
                    }
                    if (mealFood.Contains("dc_portiontypeid"))
                    {
                        portionTypeId = ((EntityReference)mealFood["dc_portiontypeid"]).Id;
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
                    if (mealFood.Contains("aa.dc_meal"))
                    {
                        meal = ((OptionSetValue)((AliasedValue)mealFood["aa.dc_meal"]).Value).Value;
                    }
                    if (mealFood.Contains("dc_foods.dc_recipefood"))
                    {
                        isRecipe = (bool)(((AliasedValue)mealFood["dc_foods.dc_recipefood"]).Value);
                    }
                   
                    if (mealFood.Contains("contact.dc_parentcontactid"))
                    {
                        parentContactId = ((EntityReference)(((AliasedValue)mealFood["contact.dc_parentcontactid"]).Value)).Id;
                    }
                    if (mealFood.Contains("contact.dc_rollshoppinglisttoparent"))
                    {
                        rollupList = (Boolean)(((AliasedValue)mealFood["contact.dc_rollshoppinglisttoparent"]).Value);
                    }
                    
                    logger.debug("fat: " + fat);
                    logger.debug("protein: " + protein);
                    logger.debug("carboydrate: " + carboydrate);
                    logger.debug("alcohol: " + alcohol);
                    logger.debug("portionSize: " + portionSize);
                    logger.debug("portionTypeId: " + portionTypeId);
                    logger.debug("menuId: " + menuId);
                    logger.debug("meal: " + meal);
                    logger.debug("day: " + day);
                    logger.debug("contactId: " + contactId);
                    logger.debug("foodId: " + foodId);
                    logger.debug("isRecipe: " + isRecipe);
                    logger.debug("rollupList: " + rollupList);
                    logger.debug("mealfoodId: " + postImage.Id.ToString());
                    
                    //find date
                    /*
                    Sunday = 948,170,000 
                    */
                    DateTime currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                    //Find sunday
                    currentDate = currentDate.AddDays(-(int)currentDate.DayOfWeek);
                    if (day == 948170001)
                    {
                        currentDate = currentDate.AddDays(1);
                    }
                    else if (day == 948170002)
                    {
                        currentDate = currentDate.AddDays(2);
                    }
                    else if (day == 948170003)
                    {
                        currentDate = currentDate.AddDays(3);
                    }
                    else if (day == 948170004)
                    {
                        currentDate = currentDate.AddDays(4);
                    }
                    else if (day == 948170005)
                    {
                        currentDate = currentDate.AddDays(5);
                    }
                    else if (day == 948170006)
                    {
                        currentDate = currentDate.AddDays(6);
                    }

                    //if this is an update make sure to check and see if the food log already exists.  Otherwise dups will get created.
                    if (pluginExecutionContext.MessageName.Equals("create", StringComparison.OrdinalIgnoreCase))
                    {
                        Entity foodLog = CreateFoodLogEntity(menuId, contactId, foodId, currentDate, crmService, fat, carboydrate, protein, alcohol, portionSize, portionTypeId, meal, postImage);
                        crmService.Create(foodLog);
                    }
                    else if (pluginExecutionContext.MessageName.Equals("update", StringComparison.OrdinalIgnoreCase))
                    {
                        //Have to look to see if the food log needs updated.  Keep in mind that the food log could have been deleted.  If so a create will needed
                        //Have to make sure that the pre and post image food Id is the same.  If not it's been swapped via the compare
                        if (preFoodId != Guid.Empty && postFoodId != Guid.Empty && preFoodId == postFoodId)
                        {
                            fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='dc_foodlog'>
                                <attribute name='dc_foodlogid' />
                                <attribute name='dc_name' />
                                <attribute name='dc_date' />
                                <attribute name='createdon' />
                                <attribute name='dc_contactid' />
                                <order attribute='dc_name' descending='false' />
                                <filter type='and'>
                                  <condition attribute='statecode' operator='eq' value='0' />
                                  <condition attribute='dc_mealfoodid' operator='eq' value='@MEALFOODID' />
                                </filter>
                              </entity>
                            </fetch>";

                            fetchXml = fetchXml.Replace("@MEALFOODID", postImage.Id.ToString());
                            

                            list = crmService.RetrieveMultiple(new FetchExpression(fetchXml));

                            if (list != null && list.Entities.Count() > 0)
                            {
                                logger.debug("Found existing food log (" + list.Entities.Count() + ").  Need to update portion size");
                                Entity foodLog = (Entity)list.Entities[0];

                                foodLog["dc_fat"]           = fat;
                                foodLog["dc_protein"]       = protein;
                                foodLog["dc_carbohydrate"]  = carboydrate;
                                foodLog["dc_alcohol"]       = alcohol;
                                foodLog["dc_portionsize"]   = portionSize;
                                crmService.Update(foodLog);
                                logger.debug("Updated foodLog Id: " + foodLog.Id.ToString() + " to portion size of: " + portionSize);
                            }
                            else
                            {
                                logger.debug("Didn't find existing food log.  Need to create");
                                Entity foodLog = CreateFoodLogEntity(menuId, contactId, foodId, currentDate, crmService, fat, carboydrate, protein, alcohol, portionSize, portionTypeId, meal, postImage);
                                crmService.Create(foodLog);
                            }
                        }
                        else if (preFoodId != Guid.Empty && postFoodId != Guid.Empty && preFoodId != postFoodId)
                        {
                            logger.debug("Food Id changed.  Meal food was swapped.  Need to update foodId, portionsize and macro nutrients");
                            fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='dc_foodlog'>
                                <attribute name='dc_foodlogid' />
                                <attribute name='dc_name' />
                                <attribute name='dc_date' />
                                <attribute name='createdon' />
                                <attribute name='dc_contactid' />
                                <order attribute='dc_name' descending='false' />
                                <filter type='and'>
                                  <condition attribute='statecode' operator='eq' value='0' />
                                  <condition attribute='dc_mealfoodid' operator='eq' value='@MEALFOODID' />
                                   
                                </filter>
                              </entity>
                            </fetch>";

                            fetchXml    = fetchXml.Replace("@MEALFOODID", postImage.Id.ToString());
                            list        = crmService.RetrieveMultiple(new FetchExpression(fetchXml));

                            if (list != null && list.Entities.Count() > 0)
                            {
                                Entity entity   = (Entity)list.Entities[0];
                                Entity foodLog  = CreateFoodLogEntity(menuId, contactId, foodId, currentDate, crmService, fat, carboydrate, protein, alcohol, portionSize, portionTypeId, meal, postImage);
                                foodLog.Id      = entity.Id;
                                crmService.Update(foodLog);
                            }
                        }
                    }
                    logger.debug("MealFoodPostUpdate: dealing with shopping list");
                    
                    List<Guid> contactIds = new List<Guid>();
                    if (parentContactId != Guid.Empty && rollupList)
                    {
                        contactIds.Add(parentContactId);
                    }
                    contactIds.Add(contactId);
                    foreach (Guid Id in contactIds)
                    {
                        logger.debug("Processing shopping list for contactId: " + Id);
                        if (parentContactId == Id)
                        {
                            logger.debug("Processing parent contact id");
                        }
                        logger.debug("--------------------------------------------------");
                        //Now deal with the shoppinglist.
                        //find shopping list.  Create if needed
                        Guid shoppingListId = RetrieveShoppingList(Id, crmService, menuId);
                        
                        //Is this a recipe?
                        if (isRecipe)
                        {
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
                            if (IsCreate(pluginExecutionContext))
                            {
                                fetchXml = fetchXml.Replace("@MEALFOODID", postImage.Id.ToString());
                            }
                            else
                            {
                                fetchXml = fetchXml.Replace("@MEALFOODID", postImage.Id.ToString());
                            }

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
                                logger.debug("Found ingredient (post): " + slh.Name);
                            }
                            //List is built.  Now need to write to shopping list
                            if (IsCreate(pluginExecutionContext))
                            {
                                //Check to see if the shopping list aleady has an item with this foodId
                                foreach (KeyValuePair<Guid, ShoppingListHelper> pair in slhList)
                                {
                                    Entity shoppingListItem = IsInShoppingList(shoppingListId, pair.Key, crmService);
                                    if (shoppingListItem != null)
                                    {
                                        logger.debug("Found matching foodId in shopping list.  Updating: " + (decimal)shoppingListItem["dc_portionsize"] + ":" + pair.Value.Portionsize);

                                        shoppingListItem["dc_portionsize"] = (decimal)shoppingListItem["dc_portionsize"] + pair.Value.Portionsize;
                                        crmService.Update(shoppingListItem);
                                        logger.debug("Updated shopping list item: " + pair.Value.Name);
                                    }
                                    else
                                    {
                                        logger.debug("ShoppinglistItem needs created");
                                        CreateFromShoppingListHelper(pair.Value, crmService, shoppingListId);
                                        logger.debug("IsRecipe: Created shopping list item: " + pair.Value.Name);
                                    }
                                }
                            }
                            else
                            {//update
                                if (IsSwap(preFoodId, postFoodId))
                                {
                                    logger.debug("Swapping recipe: " + preFoodId);
                                    //remove the pre first
                                    fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical'>
                                      <entity name='dc_ingredient'>
                                        <attribute name='dc_foodingredientid'  />
                                        <attribute name='dc_portiontypeid' />
                                        <attribute name='dc_portionsize'  />
                                        <link-entity name='dc_foods' from='dc_foodsid' to='dc_foodid' alias='dc_foods'>
                                            <attribute name='dc_portion_amount'  />
                                        </link-entity>
                                        <filter type='and'>
                                            <condition attribute='dc_foodid' operator='eq' value='@FOODID' />
                                        </filter>
                                      </entity>
                                    </fetch>";

                                    fetchXml = fetchXml.Replace("@FOODID", preFoodId.ToString());
                                    ec = crmService.RetrieveMultiple(new FetchExpression(fetchXml));

                                    Dictionary<Guid, ShoppingListHelper> slhPreList = new Dictionary<Guid, ShoppingListHelper>();
                                    foreach (Entity mf in ec.Entities)
                                    {
                                        //multiplier
                                        decimal pSize = prePortionSize;//(decimal)(((AliasedValue)mf.Attributes["dc_mealfood.dc_portionsize"]).Value);
                                        double orgPortionSize = (double)(((AliasedValue)mf.Attributes["dc_foods.dc_portion_amount"]).Value);
                                        decimal multiplier = pSize / Convert.ToDecimal(orgPortionSize);
                                        logger.debug("multiplier: " + multiplier + ": prePortionSize: " + prePortionSize);

                                        ShoppingListHelper slh  = new ShoppingListHelper();
                                        slh.Name                = ((EntityReference)mf.Attributes["dc_foodingredientid"]).Name;
                                        slh.FoodId              = ((EntityReference)mf.Attributes["dc_foodingredientid"]).Id;
                                        slh.Portionsize         = ((decimal)mf.Attributes["dc_portionsize"]) * multiplier;
                                        slh.PortionTypeId       = ((EntityReference)mf.Attributes["dc_portiontypeid"]).Id;

                                        if (slhList.ContainsKey(slh.FoodId))
                                        {
                                            slhPreList[slh.FoodId].Portionsize += slh.Portionsize;
                                        }
                                        else
                                        {
                                            slhPreList.Add(slh.FoodId, slh);
                                        }
                                        logger.debug("Found ingredient (swap): " + slh.Name);
                                    }
                                    //now remove
                                    foreach (KeyValuePair<Guid, ShoppingListHelper> pair in slhPreList)
                                    {
                                        Entity shoppingListItem = IsInShoppingList(shoppingListId, pair.Key, crmService);
                                        if (shoppingListItem != null)
                                        {
                                            logger.debug("Found matching foodId in shopping list.  Updating: " + (decimal)shoppingListItem["dc_portionsize"] + ":" + pair.Value.Portionsize);
                                            shoppingListItem["dc_portionsize"] = (decimal)shoppingListItem["dc_portionsize"] - pair.Value.Portionsize;
                                            crmService.Update(shoppingListItem);
                                            logger.debug("IsRecipe: Swap: Updated (remove) shopping list item: " + pair.Value.Name);
                                        }

                                    }

                                    //Add post
                                    foreach (KeyValuePair<Guid, ShoppingListHelper> pair in slhList)
                                    {
                                        Entity shoppingListItem = IsInShoppingList(shoppingListId, pair.Key, crmService);
                                        if (shoppingListItem != null)
                                        {
                                            logger.debug("Found matching foodId in shopping list.  Updating: " + (decimal)shoppingListItem["dc_portionsize"] + ":" + pair.Value.Portionsize);
                                            shoppingListItem["dc_portionsize"] = (decimal)shoppingListItem["dc_portionsize"] + pair.Value.Portionsize;
                                            crmService.Update(shoppingListItem);
                                            logger.debug("IsRecipe: Swap: Updated (adding) shopping list item: " + pair.Value.Name);
                                        }
                                        else //Create
                                        {
                                            logger.debug("ShoppinglistItem needs created");
                                            CreateFromShoppingListHelper(pair.Value, crmService, shoppingListId);
                                            logger.debug("IsRecipe: Swap: Created shopping list item: " + pair.Value.Name);
                                        }
                                    }

                                }
                                else
                                {
                                    bool increase = IsIncrease(preImage, postImage);
                                    foreach (KeyValuePair<Guid, ShoppingListHelper> pair in slhList)
                                    {
                                        Entity shoppingListItem = IsInShoppingList(shoppingListId, pair.Key, crmService);
                                        if (shoppingListItem != null)
                                        {
                                            logger.debug("Found matching foodId in shopping list.  Updating: " + (decimal)shoppingListItem["dc_portionsize"] + ":" + pair.Value.Portionsize);

                                            if (increase)
                                            {
                                                shoppingListItem["dc_portionsize"] = (decimal)shoppingListItem["dc_portionsize"] + pair.Value.Portionsize;
                                            }
                                            else
                                            {
                                                shoppingListItem["dc_portionsize"] = (decimal)shoppingListItem["dc_portionsize"] - pair.Value.Portionsize;
                                            }
                                            crmService.Update(shoppingListItem);
                                            logger.debug("Updated shopping list item: " + pair.Value.Name);
                                        }
                                        else //Create
                                        {
                                            logger.debug("ShoppinglistItem needs created");
                                            CreateFromShoppingListHelper(pair.Value, crmService, shoppingListId);
                                            logger.debug("IsRecipe Created shopping list item: " + pair.Value.Name);
                                        }
                                    }
                                }
                            }
                        }
                        else//non recipe
                        {
                            ShoppingList(pluginExecutionContext, foodId, shoppingListId, portionTypeId, crmService, preImage, portionSize, foodName, preFoodId, postFoodId, postImage);
                        }
                        logger.debug("----------------------------------------------");
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
        private Guid RetrieveShoppingList(Guid contactId, IOrganizationService crmService, Guid menuId)
        {
            logger.debug("Retreiving shopping list");

            Guid shoppingListId = Guid.Empty;
            
            String fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                    <entity name='dc_shoppinglist'>
                    <attribute name='dc_shoppinglistid' />
                    <filter type='and'>
                        <condition attribute='dc_contactid' operator='eq' value='@CONTACTID' />
                    </filter>
                    <link-entity name='dc_menu' alias='menu' to='dc_menuid' from='dc_menuid'>
                        <filter type='and'> 
                            <condition attribute='dc_primarymenu' value='1' operator='eq'/> 
                            <condition attribute='dc_contactid' value='@CONTACTID' operator='eq'/> 
                        </filter> 
                    </link-entity>
                    </entity>
                </fetch>";
            
            fetchXml = fetchXml.Replace("@CONTACTID", contactId.ToString());
            
            EntityCollection list = crmService.RetrieveMultiple(new FetchExpression(fetchXml));
            
            if (list != null && list.Entities.Count() == 0)
            {
                //Need to create shopping list
                Entity shoppingList = new Entity("dc_shoppinglist");
                shoppingList["dc_contactid"] = new EntityReference("contact", contactId);
                shoppingList["dc_menuid"] = new EntityReference("dc_menu", menuId);
                shoppingList["dc_name"] = DateTime.Now.ToString();

                shoppingListId = crmService.Create(shoppingList);
                logger.debug("Shopping list created: " + shoppingListId.ToString());
            }
            else if (list != null && list.Entities.Count() > 0)
            {
                Entity shoppingList = list.Entities[0];
                shoppingListId = shoppingList.Id;
                logger.debug("Found shopping list: " + shoppingListId);
            }
            return (shoppingListId);
        }
        /// <summary>
        /// Creates a shoppinglistitem from the shoppinglisthelper object
        /// </summary>
        /// <param name="slh"></param>
        /// <param name="crmService"></param>
        /// <param name="shoppingListId"></param>
        private void CreateFromShoppingListHelper(ShoppingListHelper slh, IOrganizationService crmService, Guid shoppingListId)
        {
            Entity sli                  = new Entity("dc_shoppinglistitem");
            sli["dc_shoppinglistid"]    = new EntityReference("dc_shoppinglist", shoppingListId);
            sli["dc_foodid"]            = new EntityReference("dc_foods", slh.FoodId);
            sli["dc_portiontypeid"]     = new EntityReference("dc_portion_types", slh.PortionTypeId);
            sli["dc_portionsize"]       = slh.Portionsize;
            sli["dc_name"]              = slh.Name;
            crmService.Create(sli);
        }
        /// <summary>
        /// Does the shopping list contain this foodId?  Returns null if not found
        /// </summary>
        /// <param name="shoppingListId"></param>
        /// <param name="foodId"></param>
        /// <param name="crmService"></param>
        /// <returns></returns>
        private Entity IsInShoppingList(Guid shoppingListId, Guid foodId, IOrganizationService crmService)
        {
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
                Entity entity = list.Entities[0];
                return (entity);
            }
            else
            {
                return (null);
            }
        }
        /// <summary>
        /// Is this a create message?
        /// </summary>
        /// <param name="pluginExecutionContext"></param>
        /// <returns></returns>
        private bool IsCreate(IPluginExecutionContext pluginExecutionContext)
        {
            if (pluginExecutionContext.MessageName.Equals("create", StringComparison.OrdinalIgnoreCase))
            {
                return (true);
            }
            else
            {
                return (false);
            }
        }
        /// <summary>
        /// Is this a swapped food?
        /// </summary>
        /// <returns></returns>
        private bool IsSwap(Guid preFoodId, Guid postFoodId)
        {
            bool swap = false;
            if (preFoodId != Guid.Empty && postFoodId != Guid.Empty && preFoodId != postFoodId) {
                return (true);
            }
            return (swap);
        }
        /// <summary>
        /// Is the food portion size being increased (true) or decreased (false)
        /// </summary>
        /// <returns></returns>
        private bool IsIncrease(Entity preEntity, Entity postEntity)
        {
            bool increase = false;
            if (preEntity.Contains("dc_portionsize") && postEntity.Contains("dc_portionsize"))
            {
                decimal prePortionSize = (decimal)preEntity["dc_portionsize"];
                decimal postPortionSize = (decimal)postEntity["dc_portionsize"];

                if (postPortionSize > prePortionSize)
                {
                    return (true);
                }
            }
            return (increase);
        }

        private void ShoppingList(IPluginExecutionContext pluginExecutionContext, Guid foodId, Guid shoppingListId, Guid portionTypeId, 
            IOrganizationService crmService, Entity preImage, decimal portionSize, String foodName, Guid preFoodId, Guid postFoodId, Entity postImage)
        {
            //Check to see of the foodid and portiontypeId are already in the shopping list
            if (pluginExecutionContext.MessageName.Equals("create", StringComparison.OrdinalIgnoreCase))
            {
                //Simple create
                ProcessShoppingListItem(foodId, shoppingListId, portionTypeId, crmService, preImage, portionSize, foodName, pluginExecutionContext.MessageName, false, -1);
            }
            else if (preFoodId != Guid.Empty && postFoodId != Guid.Empty && preFoodId == postFoodId)
            {
                //no swap - Should be an update
                ProcessShoppingListItem(foodId, shoppingListId, portionTypeId, crmService, preImage, portionSize, foodName, pluginExecutionContext.MessageName, false, -1);
            }
            else if (preFoodId != Guid.Empty && postFoodId != Guid.Empty && preFoodId != postFoodId)
            {
                //swap - deal with pre first
                if (preImage.Contains("dc_portionsize"))
                {
                    portionSize = (decimal)preImage["dc_portionsize"];
                }
                if (preImage.Contains("dc_foodid"))
                {
                    if (((EntityReference)preImage["dc_foodid"]).Name != null)
                    {
                        foodName = ((EntityReference)preImage["dc_foodid"]).Name;
                    }
                }
                if (preImage.Contains("dc_portiontypeid"))
                {
                    portionTypeId = ((EntityReference)preImage["dc_portiontypeid"]).Id;
                }
                int multiplier = -1;
                ProcessShoppingListItem(preFoodId, shoppingListId, portionTypeId, crmService, preImage, portionSize, foodName, pluginExecutionContext.MessageName, true, -1);
                //now post
                if (postImage.Contains("dc_portionsize"))
                {
                    portionSize = (decimal)postImage["dc_portionsize"];
                }
                if (postImage.Contains("dc_foodid"))
                {
                    if (((EntityReference)postImage["dc_foodid"]).Name != null)
                    {
                        foodName = ((EntityReference)postImage["dc_foodid"]).Name;
                    }
                }
                if (postImage.Contains("dc_portiontypeid"))
                {
                    portionTypeId = ((EntityReference)postImage["dc_portiontypeid"]).Id;
                }
                ProcessShoppingListItem(postFoodId, shoppingListId, portionTypeId, crmService, preImage, portionSize, foodName, pluginExecutionContext.MessageName, true, 1);
            }
        }

        private void ProcessShoppingListItem(Guid foodId, Guid shoppingListId, Guid portionTypeId, IOrganizationService crmService, Entity preImage, decimal portionSize,
            String foodName, String messageName, bool swap, int multiplier)
        {
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
                Entity entity = list.Entities[0];

                //update
                decimal prePortionSize = 0m;
                if (preImage != null && preImage.Contains("dc_portionsize"))
                {
                    prePortionSize = (decimal)preImage["dc_portionsize"];
                }
                logger.debug("prePortionSize: " + prePortionSize);
                Entity shoppingListItem = new Entity("dc_shoppinglistitem");
                shoppingListItem.Id = entity.Id;
                if (messageName.Equals("create", StringComparison.OrdinalIgnoreCase))
                {
                    shoppingListItem["dc_portionsize"] = ((decimal)entity["dc_portionsize"]) + portionSize;
                    decimal d = (((decimal)entity["dc_portionsize"]) + portionSize);
                    logger.debug("Updating (Create) portion size to: " + d + " for shopplistItem: " + shoppingListItem.Id);
                }
                else//update
                {
                    logger.debug("multiplier: " + multiplier);
                    decimal d = (((decimal)entity["dc_portionsize"]) + ((multiplier * prePortionSize) + portionSize));

                    if (swap)
                    {
                        d = ((decimal)entity["dc_portionsize"]) + ((multiplier * prePortionSize));
                    }
                    
                    shoppingListItem["dc_portionsize"] = d;
                    logger.debug("Updating portion size to: " + d + " for shopplistItem: " + shoppingListItem.Id);
                }
                crmService.Update(shoppingListItem);
                logger.debug("Updated shopping list item: "+foodName);
            }
            else
            {
                //Create
                Entity shoppingListItem                 = new Entity("dc_shoppinglistitem");
                shoppingListItem["dc_shoppinglistid"]   = new EntityReference("dc_shoppinglist", shoppingListId);
                shoppingListItem["dc_foodid"]           = new EntityReference("dc_foods", foodId);
                shoppingListItem["dc_portiontypeid"]    = new EntityReference("dc_portion_types", portionTypeId);
                shoppingListItem["dc_portionsize"]      = portionSize;
                shoppingListItem["dc_name"]             = foodName;
                crmService.Create(shoppingListItem);
                logger.debug("ProcessShoppingListItem: Created shopping list item: " + foodName);
            }
        }
        /// <summary>
        /// Creates a food log.
        /// </summary>
        /// <param name="menuId"></param>
        /// <param name="contactId"></param>
        /// <param name="foodId"></param>
        /// <param name="currentDate"></param>
        /// <param name="crmService"></param>
        /// <param name="fat"></param>
        /// <param name="carboydrate"></param>
        /// <param name="protein"></param>
        /// <param name="portionSize"></param>
        /// <param name="portionTypeId"></param>
        /// <param name="meal"></param>
        /// <param name="postImage"></param>
        private Entity CreateFoodLogEntity(Guid menuId, Guid contactId, Guid foodId, DateTime currentDate, IOrganizationService crmService,
            decimal fat, decimal carboydrate, decimal protein, decimal alcohol, decimal portionSize, Guid portionTypeId, int meal, Entity postImage)
        {
            //Look for food log day
            String fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                <entity name='dc_foodlogday'>
                <attribute name='dc_foodlogdayid' />
                <filter type='and'>
                    <condition attribute='dc_menuid' operator='eq' value='@MENUID' />
                    <condition attribute='dc_date' operator='on' value='@DATE' />
                    <condition attribute='dc_contactid' operator='eq' value='@CONTACTID' />
                </filter>
                </entity>
            </fetch>";

            fetchXml = fetchXml.Replace("@MENUID", menuId.ToString());
            fetchXml = fetchXml.Replace("@DATE", currentDate.ToString());
            fetchXml = fetchXml.Replace("@CONTACTID", contactId.ToString());

            EntityCollection list = crmService.RetrieveMultiple(new FetchExpression(fetchXml));

            Entity foodLog              = new Entity("dc_foodlog");
            foodLog["dc_fat"]           = fat;
            foodLog["dc_carbohydrate"]  = carboydrate;
            foodLog["dc_protein"]       = protein;
            foodLog["dc_alcohol"]       = alcohol;
            foodLog["dc_kcals"]         = Convert.ToDecimal(((fat * 9) + (protein * 4) + (carboydrate * 4) + (alcohol * 7)) ); ;


            foodLog["dc_portionsize"]       = portionSize;
            foodLog["dc_portiontypeid"]     = new EntityReference("dc_portion_types", portionTypeId);
            foodLog["dc_foodid"]            = new EntityReference("dc_foods", foodId);
            foodLog["dc_contactid"]         = new EntityReference("contact", contactId);
            foodLog["dc_date"]              = currentDate;
            foodLog["dc_meal"]              = new OptionSetValue(meal);
            foodLog["dc_mealfoodid"]        = new EntityReference("dc_mealfood", postImage.Id);
            //set name
            if (postImage.Contains("dc_name"))
            {
                foodLog["dc_name"] = postImage["dc_name"];
            }
            else
            {
                foodLog["dc_name"] = currentDate.ToString("MM/dd/yyyy");
            }
            if (list != null && list.Entities.Count() > 0)
            {
                Entity foodLogDay = list.Entities[0];
                logger.debug("Found fitness log day.  Adding food log: " + foodLogDay.Id);
                foodLog["dc_foodlogdayid"] = new EntityReference("dc_foodlogday", foodLogDay.Id);

            }
            //crmService.Create(foodLog);
            return (foodLog);
        }
    }
}
