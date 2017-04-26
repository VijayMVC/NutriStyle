﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using DynamicConnections.CRM2011.Common.Utility;
using System.ServiceModel;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using DynamicConnections.NutriStyle.CRM2011.MenuGenerator.Wrappers;
using DynamicConnections.NutriStyle.CRM2011.MenuGenerator.Helpers;

namespace DynamicConnections.NutriStyle.CRM2011.MenuGenerator
{
    public class BuildMenu : General
    {
        private static readonly Logger logger = GetLogger();
        OrganizationServiceProxy crmService;

        public BuildMenu()
        {
            crmService = CrmHelper.CreateCrmService("crmadmin", "P@ssw0rd", "DC", "https://crmdev.dynamiconnections.com", "NS", 443);
        }
        public BuildMenu(OrganizationServiceProxy crmService) {
            this.crmService = crmService;
        }
        public BuildMenu(String user, String password, String domain, String hostname, String orgName, String portnumber, Logger logger)
        {
            //crmService = CrmHelper.CreateCrmService("crmadmin", "P@ssw0rd", "DC", "crmdev2001.dynamiconnections.com", "NS", 443);
            crmService = CrmHelper.CreateCrmService(user, password, domain, hostname, orgName, Convert.ToInt32(portnumber));
        }

        public XmlDocument Build(Guid contactId)
        {
            XmlDocument results         = Success.Create("worked");
            Entity menuPreset           = null;
            Entity contact              = null;
            Guid menuPresetId           = Guid.Empty;
            int amountMultiplier        = 1;
            int portionSizeMultipler    = 1;
            int minMultiplier           = 1;
            decimal kcalPercent         = 0m;
            decimal kcal                = -1m;//This is the target per day
            decimal fatPercent          = 0m;
            decimal carbohydratePercent = 0m;
            decimal proteinPercent      = 0m;
            String menuName             = String.Empty;
            int numberOfSnacks          = 0;
            bool multipleProfiles       = false;
            try
            {
                //Lookup contact.  Need to get the menu preset, kcal target
                contact = crmService.Retrieve("contact", contactId,
                    new ColumnSet(new String[] { "dc_menupresetid", "dc_kcaltarget", "dc_morningsnack", "dc_afternoonsnack", "dc_eveningsnack", "fullname" }));

                if (contact != null)
                {
                    menuPresetId = contact.Attributes.Contains("dc_menupresetid") ? ((EntityReference)contact.Attributes["dc_menupresetid"]).Id : Guid.Empty;

                    kcal = contact.Contains("dc_kcaltarget") ? (decimal)contact["dc_kcaltarget"] : -2m;
                    //kcal = kcal2;
                    //Find preset
                    if (menuPresetId != Guid.Empty)
                    {
                        menuPreset = crmService.Retrieve("dc_presets", menuPresetId,
                            new ColumnSet(new String[] { "dc_fat_pct", "dc_fat_grams", "dc_pro_pct", "dc_pro_grams", "dc_cho_pct", "dc_cho_grams",
                            "dc_breakfast_percent", "dc_lunch_percent", "dc_dinner_percent", "dc_num_snacks", 
                            "dc_incl_morningsnack", "dc_incl_afternoonsnack", "dc_incl_eveningsnack", "dc_name", "dc_snacks_percent"}));
                    }
                }
                else
                {
                    logger.error("Can't find contact: " + contactId);
                }

                if (menuPreset != null)
                {
                    fatPercent = menuPreset.Attributes.Contains("dc_fat_pct") ? (int)menuPreset.Attributes["dc_fat_pct"] : 0m;
                    carbohydratePercent = menuPreset.Attributes.Contains("dc_cho_pct") ? (int)menuPreset.Attributes["dc_cho_pct"] : 0m;
                    proteinPercent = menuPreset.Attributes.Contains("dc_pro_pct") ? (int)menuPreset.Attributes["dc_pro_pct"] : 0m;
                    menuName = menuPreset.Attributes.Contains("dc_name") ? (String)menuPreset.Attributes["dc_name"] : String.Empty;
                }
                else
                {
                    logger.error("Can't find menu preset: " + menuPresetId);
                }
                //Additional profiles?
                AdditionalProfiles ap = new AdditionalProfiles();
                if(ap.AreThereChildren(contactId, crmService)) {
                    kcal += ap.RetrieveAdditionalProfileKcals(contactId, crmService);
                    multipleProfiles = true;
                }

                #region Figure out the amountMultiplier

                if (kcal >= 2000 && kcal < 2500)
                {
                    amountMultiplier = 1;
                }
                else if (kcal >= 2500 && kcal < 3000)
                {
                    amountMultiplier = 2;
                }
                else if (kcal >= 3000 && kcal < 3500)
                {
                    amountMultiplier = 3;
                }
                else if (kcal >= 3500 && kcal < 4000)
                {
                    amountMultiplier = 4;
                }
                else if (kcal >= 4000 && kcal < 4500)
                {
                    amountMultiplier = 5;
                }
                else if (kcal >= 5000 && kcal < 5500)
                {
                    amountMultiplier = 6;
                }
                else if (kcal >= 6000 && kcal < 6500)
                {
                    amountMultiplier = 7;
                }
                else if (kcal >= 7000 && kcal < 7500)
                {
                    amountMultiplier = 8;
                }
                else if (kcal >= 8000 && kcal < 8500)
                {
                    amountMultiplier = 9;
                }
                else if (kcal >= 9000 && kcal < 9500)
                {
                    amountMultiplier = 10;
                }
                else if (kcal >= 10000)
                {
                    amountMultiplier = 11;
                }
                else if (kcal < 1400)
                {
                    minMultiplier = 1;
                }

                if (kcal > 5000 && kcal < 7500)
                {
                    portionSizeMultipler = 2;
                }
                else if (kcal > 7500 && kcal < 10000)
                {
                    portionSizeMultipler = 3;
                }
                else if (kcal > 10000)
                {
                    portionSizeMultipler = 3;
                }
                #endregion

                //layout meal pattern
                List<Meals> mealPattern                 = new List<Meals>();
                List<Meals> snacksFavoritesPattern      = new List<Meals>();

                mealPattern.Add(Meals.Breakfast);//breakfast
                mealPattern.Add(Meals.Lunch);//lunch
                mealPattern.Add(Meals.Dinner);//dinner

                if (contact.Contains("dc_morningsnack") && (bool)contact["dc_morningsnack"])
                {
                    mealPattern.Add(Meals.Morning_Snack);//morning snack
                    numberOfSnacks++;
                }
                if (contact.Contains("dc_afternoonsnack") && (bool)contact["dc_afternoonsnack"])
                {
                    mealPattern.Add(Meals.Afternoon_Snack);//afternoon snack
                    numberOfSnacks++;
                }
                if (contact.Contains("dc_eveningsnack") && (bool)contact["dc_eveningsnack"])
                {
                    mealPattern.Add(Meals.Evening_Snack);//evening snack
                    numberOfSnacks++;
                }
                List<Days> days = new List<Days>();
                days.Add(Days.Sunday);
                days.Add(Days.Monday);
                days.Add(Days.Tuesday);
                days.Add(Days.Wednesday);
                days.Add(Days.Thursday);
                days.Add(Days.Friday);
                days.Add(Days.Saturday);

                
                //Delete all foodlogs
                DateTime currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                //Find sunday
                currentDate = currentDate.AddDays(-(int)currentDate.DayOfWeek);
                logger.debug("currentDate: " + currentDate);
                
                //figure out if there are related contacts/profiles.  If so get the kcal amount

                #region Retreive food likes

                String foodLikesXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
                  <entity name='dc_foodlike'>
                    <attribute name='dc_day' />
                    <attribute name='dc_meal' />
                    <attribute name='dc_foodid' />
                    <attribute name='dc_foodlikeid' />
                    <order attribute='dc_foodid' descending='false' />
                    <filter type='and'>
                        <condition attribute='dc_contactid' value='@CONTACTID' operator='eq'/>
                        <condition attribute='dc_foodid' operator='not-null' />
                    </filter>
                  </entity>
                </fetch>";

                foodLikesXml = foodLikesXml.Replace("@CONTACTID", contactId.ToString());

                EntityCollection foodLikesCollection = crmService.RetrieveMultiple(new FetchExpression(foodLikesXml));

                //alter meal pattern to include snacks if needed
                foreach (Entity foodLikes in foodLikesCollection.Entities)
                {
                    int x = ((OptionSetValue)foodLikes["dc_meal"]).Value;
                    //Convert to meal 
                    var m = Meals.Morning_Snack;
                    if (x == (int)Meals.Morning_Snack)
                    {
                        if (!mealPattern.Contains(m))
                        {
                            mealPattern.Add(m);
                        }
                        if (!snacksFavoritesPattern.Contains(m))
                        {
                            snacksFavoritesPattern.Add(m);

                            if (!mealPattern.Contains(m))
                            {
                                numberOfSnacks++;
                            }
                            
                        }
                    }
                    if (x == (int)Meals.Afternoon_Snack)
                    {
                        m = Meals.Afternoon_Snack;
                        if (!mealPattern.Contains(m))
                        {
                            mealPattern.Add(m);
                        }
                        if (!snacksFavoritesPattern.Contains(m))
                        {
                            snacksFavoritesPattern.Add(m);
                            if (!mealPattern.Contains(m))
                            {
                                numberOfSnacks++;
                            }
                        }
                    }
                    if (x == (int)Meals.Evening_Snack)
                    {
                        m = Meals.Evening_Snack;
                        if (!mealPattern.Contains(m))
                        {
                            mealPattern.Add(m);
                        }
                        if (!snacksFavoritesPattern.Contains(m))
                        {
                            snacksFavoritesPattern.Add(m);
                            if (!mealPattern.Contains(m))
                            {
                                numberOfSnacks++;
                            }
                        }
                    }
                }
                #endregion

                #region Retreive food dislikes
                String foodDislikesXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
                  <entity name='dc_fooddislike'>
                    <attribute name='dc_foodid' />
                    <attribute name='dc_mealcomponentid' />
                    <attribute name='dc_fooddislikeid' />
                    <order attribute='dc_foodid' descending='false' />
                    <filter type='and'>
                        <condition attribute='dc_contactid' value='@CONTACTID' operator='eq'/>
                    </filter>
                  </entity>
                </fetch>";

                foodDislikesXml = foodDislikesXml.Replace("@CONTACTID", contactId.ToString());

                EntityCollection foodDislikesCollection = crmService.RetrieveMultiple(new FetchExpression(foodDislikesXml));
                String foodDislikeIdsFilter = @"<filter type='and'>
                              <condition attribute='dc_foodsid' operator='not-in'>
                                   @VALUE
                              </condition>
                            </filter>";

                String foodDislikeSubCategoriesFilter = @"<filter type='and'>
                              <condition attribute='dc_meal_componentid' operator='not-in'>
                                   @VALUE
                              </condition>
                            </filter>";
                String foodDislikeIds = String.Empty;
                String foodDislikeSubCategories = String.Empty;

                String foodRestrictionFilter = String.Empty;

                foreach (Entity entity in foodDislikesCollection.Entities)
                {
                    if (entity.Contains("dc_foodid") && entity["dc_foodid"] != null)
                    {
                        Guid foodId = ((EntityReference)entity["dc_foodid"]).Id;
                        foodDislikeIds += "<value>" + foodId.ToString() + "</value>";
                    }
                    else if (entity.Contains("dc_mealcomponentid") && entity["dc_mealcomponentid"] != null)
                    {
                        Guid Id = ((EntityReference)entity["dc_mealcomponentid"]).Id;
                        foodDislikeSubCategories += "<value>" + Id.ToString() + "</value>";
                    }
                }
                if (!String.IsNullOrEmpty(foodDislikeIds))
                {
                    foodDislikeIdsFilter = foodDislikeIdsFilter.Replace("@VALUE", foodDislikeIds);
                }
                else
                {
                    foodDislikeIdsFilter = String.Empty;
                }
                if (!String.IsNullOrEmpty(foodDislikeSubCategories))
                {
                    foodDislikeSubCategoriesFilter = foodDislikeSubCategoriesFilter.Replace("@VALUE", foodDislikeSubCategories);
                }
                else
                {
                    foodDislikeSubCategoriesFilter = String.Empty;
                }

                #endregion
                
                //int currentMeal = 1; //Keeps track of what we're currently building
                //Start building menu
                Entity menu                     = new Entity("dc_menu");
                menu["dc_primarymenu"]          = true;
                menu["dc_name"]                 = menuName + " - " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
                menu["dc_contactid"]            = new EntityReference("contact", contactId);
                menu["dc_menuid"]               = Guid.NewGuid();
                EntityCollection daysCollection = new EntityCollection();
                daysCollection.EntityName       = "dc_day";
                Relationship menuDayRelationship= new Relationship("dc_dc_menu_dc_day");
                
                //set relationship
                menu.RelatedEntities[menuDayRelationship] = daysCollection;

                foreach (Days day in days)
                {
                    Entity menuDay          = new Entity("dc_day");
                    menuDay["dc_day"]       = new OptionSetValue((int)day);
                    menuDay["dc_name"]      = day.ToString();
                    menuDay["dc_dayid"]     = Guid.NewGuid();
                    menuDay["dc_menuid"]    = new EntityReference("dc_menu", (Guid)menu["dc_menuid"]);
                    //Create relationship from day to menu
                    //All in memory
                    daysCollection.Entities.Add(menuDay);
                    //Create relationship from day to meal
                    Relationship dayMealRelationship                = new Relationship("dc_dc_day_dc_meal");
                    EntityCollection mealsCollection                = new EntityCollection();
                    mealsCollection.EntityName                      = "dc_meal";
                    menuDay.RelatedEntities[dayMealRelationship]    = mealsCollection;
                    Dictionary<String, decimal> carryOvers          = new Dictionary<string, decimal>();
                    decimal carryOversKcals                         = 0m;
                    foreach (Meals currentMeal in mealPattern)
                    {
                        bool foodFavoriteEntree = false;//local to meal

                        logger.debug("-------------------------------------------------");
                        logger.debug("Processing: " + day.ToString() + ": Meal: " + currentMeal.ToString());
                        bool mealSuccess = false;
                        //Process meal order: breakfast, lunch, dinner, morning snack, lunch snack, evening snack
                        kcalPercent = DetermineKcalsForMeal.Execute(currentMeal, numberOfSnacks);

                        decimal mealKcals = (kcal * kcalPercent) / 100m;
                        if (carryOversKcals > 0 && currentMeal == Meals.Breakfast || currentMeal == Meals.Lunch || currentMeal == Meals.Dinner)
                        {
                            mealKcals = mealKcals + carryOversKcals;
                        }

                        Entity meal = new Entity("dc_meal");
                        //Add to the day
                        mealsCollection.Entities.Add(meal);
                        //Build relationship to mealFoods
                        Relationship mealMealFoodsRelationship = new Relationship("dc_dc_meal_dc_mealfood");
                        EntityCollection mealFoodsCollection = new EntityCollection();
                        mealFoodsCollection.EntityName = "dc_mealfood";
                        meal.RelatedEntities[mealMealFoodsRelationship] = mealFoodsCollection;
                        meal["dc_carbohydrate_targets"] = (((mealKcals * carbohydratePercent) / 100m) / 4m);
                        meal["dc_fat_targets"] = (((mealKcals * fatPercent) / 100m) / 9m);
                        meal["dc_protein_targets"] = (((mealKcals * proteinPercent) / 100m) / 4m);
                        meal["dc_kcals_targets"] = mealKcals;//kcalPercent;
                        meal["dc_day"] = new OptionSetValue((int)day);
                        meal["dc_meal"] = new OptionSetValue((int)currentMeal);
                        meal["dc_name"] = day.ToString() + " " + currentMeal.ToString();
                        meal["dc_carbohydrate_targets_percent"] = carbohydratePercent;
                        meal["dc_fat_targets_percent"] = fatPercent;
                        meal["dc_protein_targets_percent"] = proteinPercent;
                        meal["dc_dayid"] = new EntityReference("dc_day", (Guid)menuDay["dc_dayid"]);
                        meal["dc_mealid"] = Guid.NewGuid();

                        //AddCarryOvers
                        #region Load up foods
                        //Start building out menu
                        //Get collection of foods that represents component/sub component collection
                        Dictionary<Guid, List<MealFoodWrapper>> foodComponents = new Dictionary<Guid, List<MealFoodWrapper>>();

                        List<MealFoodWrapper> foodLikesList = new List<MealFoodWrapper>();
                        Dictionary<String, decimal> list = new Dictionary<string, decimal>();

                        //Deal with food likes
                        logger.debug("Loading food favorites/likes");

                        foreach (Entity foodLike in foodLikesCollection.Entities)
                        {
                            if (((OptionSetValue)foodLike["dc_day"]).Value == ((int)day) &&
                                ((OptionSetValue)foodLike["dc_meal"]).Value == ((int)currentMeal))
                            {
                                //found match.  Look up food - Need to use in clause - faster
                                String fetchFoodXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
                                      <entity name='dc_foods'>
                                        <attribute name='dc_foodsid' />
                                        <attribute name='dc_name' />
                                        <attribute name='dc_portion_amount' />
                                        <attribute name='dc_min_amount' />
                                        <attribute name='dc_max_amount' />
                                        <attribute name='dc_amount_incr' />
                                        <attribute name='dc_portiontypeid' />
                                        <attribute name='dc_unit_gram_weight' />
                                        <attribute name='dc_canusefoodinmenu' />
                                        <order attribute='dc_name' descending='false' />
                                        <filter type='and'>
                                            <condition attribute='dc_foodsid' operator='eq'  value='@FOODID' />
                                        </filter>
                                        <link-entity name='dc_food_nutrients' alias='fn' to='dc_foodnutrientid' from='dc_food_nutrientsid'> 
                                            <attribute name='dc_carbohydrate'/>
                                            <attribute name='dc_fat'/> 
                                            <attribute name='dc_protein'/>
                                            <attribute name='dc_kcals'/>
                                            <attribute name='dc_alcohol'/>
                                        </link-entity>
                                      </entity>
                                    </fetch>";

                                fetchFoodXml = fetchFoodXml.Replace("@FOODID", ((EntityReference)foodLike["dc_foodid"]).Id.ToString());

                                EntityCollection foodCollection = crmService.RetrieveMultiple(new FetchExpression(fetchFoodXml));

                                if (foodCollection.Entities.Count > 0)
                                {
                                    Entity food = foodCollection.Entities[0];

                                    #region Create MealFoodWrapper objects
                                    MealFoodWrapper mfw = new MealFoodWrapper(food, (Guid)meal["dc_mealid"], Guid.Empty, portionSizeMultipler, amountMultiplier, true);
                                    
                                    if (Foods.IsEntree(food.Id, crmService))
                                    {
                                        foodFavoriteEntree = true;
                                    }
                                    foodLikesList.Add(mfw);
                                    #endregion
                                }
                            }
                        } 
                        
                        //Get the rest food restrictions
                        logger.debug("Loading preset food favorites/likes");
                        #region Preset Food Likes
                        String presetFoodLikeFetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
                              <entity name='dc_foods'>
                                <attribute name='dc_foodsid' />
                                <attribute name='dc_name' />
                                <attribute name='dc_portion_amount' />
                                <attribute name='dc_min_amount' />
                                <attribute name='dc_max_amount' />
                                <attribute name='dc_amount_incr' />
                                <attribute name='dc_portiontypeid' />
                                <attribute name='dc_unit_gram_weight' />
                                <attribute name='dc_canusefoodinmenu' />

                                <link-entity name='dc_foodlike' from='dc_foodid' to='dc_foodsid' alias='aa'>
                                  <filter type='and'>
                                    <condition attribute='dc_day' operator='eq' value='@DAY' />
                                    <condition attribute='dc_meal' operator='eq' value='@MEAL' />
                                  </filter>
                                  <link-entity name='dc_presets' from='dc_presetsid' to='dc_menupresetid' alias='ab'>
                                    <filter type='and'>
                                      <condition attribute='dc_presetsid' operator='eq' value='{@PRESETID}' />
                                    </filter>
                                  </link-entity>
                                </link-entity>
                                <link-entity name='dc_food_nutrients' alias='fn' to='dc_foodnutrientid' from='dc_food_nutrientsid'>
                                    <attribute name='dc_carbohydrate'/>
                                    <attribute name='dc_fat'/> 
                                    <attribute name='dc_protein'/>
                                    <attribute name='dc_kcals'/>
                                    <attribute name='dc_alcohol'/>
                                </link-entity>
                              </entity>
                            </fetch>";

                        presetFoodLikeFetchXml = presetFoodLikeFetchXml.Replace("@PRESETID", menuPresetId.ToString());
                        presetFoodLikeFetchXml = presetFoodLikeFetchXml.Replace("@DAY", ((int)day).ToString());
                        presetFoodLikeFetchXml = presetFoodLikeFetchXml.Replace("@MEAL", ((int)currentMeal).ToString());
                        try
                        {
                            EntityCollection presetFoodLikeCollection =
                                crmService.RetrieveMultiple(new FetchExpression(presetFoodLikeFetchXml));
                            logger.debug("Found " + presetFoodLikeCollection.TotalRecordCount + " dc_foods favorites/likes");
                            if (presetFoodLikeCollection != null && presetFoodLikeCollection.Entities.Count > 0)
                            {
                                foreach (Entity food in presetFoodLikeCollection.Entities)
                                {
                                    #region Create MealFoodWrapper objects

                                    Entity mealFood = new Entity("dc_mealfood");
                                    mealFood.LogicalName = "dc_mealfood";

                                    MealFoodWrapper mfw = new Wrappers.MealFoodWrapper(food, (Guid) meal["dc_mealid"],
                                                                                       Guid.Empty, portionSizeMultipler,
                                                                                       amountMultiplier, true);
                                    foodLikesList.Add(mfw);
                                    #endregion
                                }
                            }
                        }
                        catch (FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> ex)
                        {
                            logger.error("Code: " + ex.Detail.ErrorCode);
                            logger.error("Message: " + ex.Detail.Message);
                            logger.error("Trace: " + ex.Detail.TraceText);
                            logger.error("Inner Fault: " + ex.Detail.InnerFault);
                            results = Error.Create(ex.Detail.Message);
                        }
                        catch (Exception e)
                        {
                            logger.error(e.Message);
                            logger.error(e.StackTrace);
                        }
                        #endregion
                        //Get the reset food restriction filter
                        logger.debug("Get the reset food restriction filter");
                        String presetFoodRestrictionFilter = @"<fetch distinct='false' mapping='logical' output-format='xml-platform' version='1.0'>
                            <entity name='dc_presets'> 
                                <attribute name='dc_name'/> 
                                <attribute name='dc_none' />
                                <attribute name='dc_vegan' />
                                <attribute name='dc_vegetarian' />
                                <attribute name='dc_glutenfree' />
                                <attribute name='dc_dairyfree' /> 
                                <attribute name='dc_presetsid'/> 
                                    <filter type='and'> 
                                        <condition attribute='dc_presetsid' value='@MENUPRESETID' operator='eq'/> 
                                    </filter> 
                                </entity> 
                            </fetch>";

                        presetFoodRestrictionFilter = presetFoodRestrictionFilter.Replace("@MENUPRESETID", menuPresetId.ToString());

                        EntityCollection restrictionCollection = crmService.RetrieveMultiple(new FetchExpression(presetFoodRestrictionFilter));

                        if (restrictionCollection != null && restrictionCollection.Entities.Count > 0)
                        {
                            logger.debug("Found " + restrictionCollection.TotalRecordCount + " dc_foods restriction filter");

                            Entity entity = (Entity)restrictionCollection.Entities[0];
                            if (entity.Contains("dc_glutenfree"))
                            {
                                if ((bool)entity["dc_glutenfree"])
                                {
                                    foodRestrictionFilter += @"<filter type='and'>
                                      <condition attribute='dc_gluten' operator='eq' value='0' />
                                    </filter>";
                                }
                                
                            }
                            else if (entity.Contains("dc_vegan")) {
                                if ((bool)entity["dc_vegan"])
                                {
                                    foodRestrictionFilter += @"<filter type='and'>
                                          <condition attribute='dc_animalproduct' operator='eq' value='0' />
                                        </filter>";
                                }
                            }
                            else if (entity.Contains("dc_dairyfree"))
                            {
                                if ((bool)entity["dc_dairyfree"])
                                {
                                    foodRestrictionFilter += @"<filter type='and'>
                                          <condition attribute='dc_dairy' operator='eq' value='0' />
                                        </filter>";
                                }
                            }
                        }
                        //Populate foods
                        String fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                  <entity name='dc_component_template_sel'>
                                    <attribute name='dc_component_template_selid' />
                                    <attribute name='dc_name' />
                                    <attribute name='dc_mealcomponentid' />
                                    <attribute name='dc_componentcategoryid' />
                                    <order attribute='dc_name' descending='false' />
                                    <filter type='and'>
                                      <condition attribute='dc_dayoptionset' operator='eq' value='@DAY' />
                                      <condition attribute='dc_mealoptionset' operator='eq' value='@MEAL' />
                                      <condition attribute='dc_menupresetid' operator='eq' value='@MENUPRESETID' />
                                    </filter>
                                    <link-entity name='dc_component_category' alias='aa' to='dc_componentcategoryid' from='dc_component_categoryid'>
                                        <attribute name='dc_name' />
                                    </link-entity>
                                  </entity>
                                </fetch>";

                        fetchXml = fetchXml.Replace("@MENUPRESETID", menuPresetId.ToString());
                        fetchXml = fetchXml.Replace("@DAY", ((int)day).ToString());
                        fetchXml = fetchXml.Replace("@MEAL", ((int)currentMeal).ToString());
                        EntityCollection components = crmService.RetrieveMultiple(new FetchExpression(fetchXml));

                        //Get foods

                        #region Populate meal foods
                        //Only query snacks if the user has selected them
                        if ( (currentMeal == Meals.Breakfast || 
                            currentMeal == Meals.Lunch || 
                            currentMeal == Meals.Dinner)
                            ||
                            ( (currentMeal == Meals.Morning_Snack && contact.Contains("dc_morningsnack") && (bool)contact["dc_morningsnack"]) ||
                            (currentMeal == Meals.Afternoon_Snack && contact.Contains("dc_afternoonsnack") && (bool)contact["dc_afternoonsnack"]) ||
                            (currentMeal == Meals.Evening_Snack && contact.Contains("dc_eveningsnack") && (bool)contact["dc_eveningsnack"]) )
                            )
                        {
                            foreach (Entity component in components.Entities)
                            {
                                Guid dc_mealcomponentid = component.Attributes.Contains("dc_mealcomponentid") ? ((EntityReference)component.Attributes["dc_mealcomponentid"]).Id : Guid.Empty;
                                Guid dc_componentcategoryid = component.Attributes.Contains("dc_componentcategoryid") ? ((EntityReference)component.Attributes["dc_componentcategoryid"]).Id : Guid.Empty;
                                Guid dc_component_template_selId = component.Attributes.Contains("dc_component_template_selid") ? ((Guid)component.Attributes["dc_component_template_selid"]) : Guid.Empty;
                                String categoryName = component.Attributes.Contains("aa.dc_name") ? (String)((AliasedValue)component.Attributes["aa.dc_name"]).Value : String.Empty;
                                if (foodFavoriteEntree && categoryName.Equals("entree", StringComparison.OrdinalIgnoreCase))
                                {
                                    logger.debug("Not processing.  Food like has an entree");
                                }
                                else
                                {
                                    #region New Fetch Xml: Foods

                                    fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
                                      <entity name='dc_foods'>
                                        <attribute name='dc_foodsid' />
                                        <attribute name='dc_name' />
                                        <attribute name='dc_portion_amount' />
                                        <attribute name='dc_min_amount' />
                                        <attribute name='dc_max_amount' />
                                        <attribute name='dc_amount_incr' />
                                        <attribute name='dc_portiontypeid' />
                                        <attribute name='dc_unit_gram_weight' />
                                        <attribute name='dc_canusefoodinmenu' />
                                        @FOODRESTRICTIONFILTER
                                        @FOODDISLIKEFILTER
                                        <filter type='and'>
                                            <condition attribute='dc_common_food' operator='eq'  value='1' />
                                            <condition attribute='dc_canuseinmenu' operator='eq'  value='1' />
                                            <condition attribute='statecode' operator='eq'  value='0' />
                                        </filter>
                                        <link-entity name='dc_dc_foods_dc_meal_component' from='dc_foodsid' to='dc_foodsid' visible='false' intersect='true'>
                                          <link-entity name='dc_meal_component' from='dc_meal_componentid' to='dc_meal_componentid' alias='aa'>
                                            <filter type='and'>
                                              <condition attribute='dc_meal_componentid' operator='eq' value='@COMPONENTID' />
                                              @FOODDISLIKESSUBCATEGORYFILTER  
                                            </filter>
                                            <link-entity name='dc_component_template_sel' from='dc_mealcomponentid' to='dc_meal_componentid' alias='ab'>
                                              <filter type='and'>
                                                <condition attribute='dc_component_template_selid' operator='eq' value='@TEMPLATESELID' />
                                              </filter>
                                            </link-entity>
                                          </link-entity>
                                        </link-entity>
                                        <link-entity name='dc_food_nutrients' alias='fn' to='dc_foodnutrientid' from='dc_food_nutrientsid'> 
                                            <attribute name='dc_carbohydrate'/>
                                            <attribute name='dc_fat'/> 
                                            <attribute name='dc_protein'/>
                                            <attribute name='dc_kcals'/>
                                            <attribute name='dc_alcohol'/>
                                        </link-entity>
                                      </entity>
                                    </fetch>";
                                    #endregion

                                    #region Setup Filters
                                    fetchXml = fetchXml.Replace("@TEMPLATESELID", dc_component_template_selId.ToString());
                                    fetchXml = fetchXml.Replace("@COMPONENTID", dc_mealcomponentid.ToString());
                                    fetchXml = fetchXml.Replace("@FOODDISLIKEFILTER", foodDislikeIdsFilter);
                                    fetchXml = fetchXml.Replace("@FOODDISLIKESSUBCATEGORYFILTER", foodDislikeSubCategoriesFilter);
                                    fetchXml = fetchXml.Replace("@FOODRESTRICTIONFILTER", foodRestrictionFilter);

                                    #endregion

                                    EntityCollection foodCollection = crmService.RetrieveMultiple(new FetchExpression(fetchXml));

                                    if (foodCollection != null && foodCollection.Entities.Count > 0)
                                    {
                                        foodComponents.Add(dc_component_template_selId, new List<MealFoodWrapper>());

                                        foreach (Entity food in foodCollection.Entities)
                                        {
                                            #region Create MealFoodWrapper objects
                                            //Create meal food
                                            Entity mealFood = new Entity("dc_mealfood");
                                            if (food.Contains("dc_portiontypeid") && dc_component_template_selId != Guid.Empty)
                                            {
                                                mealFood.LogicalName = "dc_mealfood";
                                                MealFoodWrapper mfw = new Wrappers.MealFoodWrapper(food, (Guid)meal["dc_mealid"], dc_component_template_selId, portionSizeMultipler, amountMultiplier);
                                                
                                                foodComponents[dc_component_template_selId].Add(mfw);
                                            }
                                            #endregion
                                        }
                                    }
                                    else
                                    {
                                        logger.debug("Not able to load up and foods for this component/meal component setup");
                                    }
                                }
                            }
                        }
                        
                        #endregion
                        #endregion

                        //Foods are loaded up.  Set up meal 
                        logger.debug("Foods are loaded up.  Set up meal");
                        List<MealFoodWrapper> bestMeal = new List<MealFoodWrapper>();
                        decimal bestScore = 100000m;
                        
                        //set up the targets for fat, carbs and proteins

                        List<MealFoodWrapper> mealFoods = new List<MealFoodWrapper>();
                        //Randomly select foods
                        int loopCount = 0;
                        int maxTries = 10;
                        for (int y = 0; y < maxTries; y++)
                        {
                            logger.debug("count: " + y);
                            loopCount++;
                            foreach (KeyValuePair<Guid, List<MealFoodWrapper>> key in foodComponents)
                            {
                                int max = ((List<MealFoodWrapper>)key.Value).Count;
                                int min = 0;
                                Random r = new Random();
                                int index = r.Next(min, max);
                                MealFoodWrapper mfw = ((List<MealFoodWrapper>)key.Value)[index];
                                mealFoods.Add(mfw);
                            }
                            foreach (MealFoodWrapper mfw in foodLikesList)
                            {
                                //logger.debug("Added food like: " + mfw.Name);
                                mealFoods.Add(mfw);
                            }
                            //Add up totals: mealFoods collection
                            meal = Meal.UpdateTotals(meal, mealFoods);

                            if (currentMeal == Meals.Morning_Snack || currentMeal == Meals.Afternoon_Snack || currentMeal == Meals.Evening_Snack) //snacks
                            {
                                logger.debug("Generating: current snack: " + currentMeal);
                                AdjustSnacks.Execute(meal, mealFoods);
                                meal = Meal.UpdateTotals(meal, mealFoods);

                                foreach (MealFoodWrapper mfw in mealFoods)
                                {
                                    mealFoodsCollection.Entities.Add(mfw.MealFood);
                                }
                                logger.debug("Adding snack to menu");
                                break;//bail out out of for loop
                                /*
                                AdjustSnacks adjustSnack = new AdjustSnacks();
                                adjustSnack.setMeal(meal);
                                adjustSnack.execute();
                                meal = adjustSnack.getMeal();
                                if (meal.getSnackScore() > 0)
                                {
                                    SubstituteFood substituteFood = new SubstituteFood();
                                    substituteFood.setMeal(meal);
                                    substituteFood.setComponentFoodsCollection(cfc);
                                    substituteFood.snack();
                                    meal = substituteFood.getMeal();
                                }
                                logger.debug("BuildMenu: Snack will be added...");
                                retry = false;
                                */
                            }
                            else if (currentMeal == Meals.Breakfast || currentMeal == Meals.Lunch || currentMeal == Meals.Dinner) //breakfast, lunch, dinner
                            {

                                logger.debug("Generating: current meal: "+currentMeal);
                                // gap calculations  
                                meal = CalcGAP.Execute(meal);
                                // food ratio calculations
                                GetFoodRatios.Execute(meal, mealFoods);

                                //Adjust the portion sizes for the meal
                                for (int x = 0; x < 20; x++)
                                {
                                    if (PerformRatioGapAnalysis.Execute(meal, mealFoods))
                                    {
                                        logger.debug("BuildMenu: Generated a meal within acceptable parameters.  Moving to next meal");
                                        mealSuccess = true;
                                        break;
                                    }
                                    else //meal gen failed.  Score meal
                                    {
                                        decimal score = Meal.Score(meal);
                                        if (score < bestScore)
                                        {
                                            bestScore = score;
                                            //mealFoods.CopyTo(bestMeal);
                                            //Copy to best meal
                                            bestMeal = MealFoodWrapper.Copy(mealFoods);

                                        }
                                        //reset adjust variables and retry
                                        MealFood.ResetAdjust(mealFoods);
                                    }
                                }
                                if (mealSuccess)
                                {
                                    mealSuccess = true;
                                    break;
                                }
                                else
                                {
                                    //Food substituion logic
                                    mealFoods = SubstituteFood.Execute(meal, mealFoods, foodComponents);
                                    meal = Meal.UpdateTotals(meal, mealFoods);
                                    //Score meal
                                    decimal currentScore = Meal.Score(meal);
                                    if (currentScore < bestScore)
                                    {
                                        //Copy to best meal
                                        bestMeal = MealFoodWrapper.Copy(mealFoods);
                                        logger.debug("Updating best meal: " + currentScore);
                                        bestScore = currentScore;
                                    }
                                    if (currentScore == 0)
                                    {
                                        mealSuccess = true;
                                        break;
                                    }
                                    if (loopCount == maxTries)
                                    {
                                        logger.debug("Was not able to hit targets.  Using best meal: " + bestScore);
                                        //Create relationship from day to meal
                                        decimal kcals   = 0m;
                                        decimal fat     = 0m;
                                        decimal protein = 0m;
                                        decimal carbs   = 0m;
                                        decimal alcohol = 0m;

                                        meal = Meal.UpdateTotals(meal, bestMeal);
                                        decimal testScore = Meal.Score(meal);
                                        foreach (MealFoodWrapper mfw in bestMeal)
                                        {
                                            mfw.MealFood = MealFood.CalculateKcal(mfw.MealFood);
                                            kcals       += (decimal)mfw.MealFood["dc_kcals"];
                                            fat         += (decimal)mfw.MealFood["dc_fat"];
                                            protein     += (decimal)mfw.MealFood["dc_protein"];
                                            carbs       += (decimal)mfw.MealFood["dc_carbohydrate"];
                                            alcohol     += mfw.MealFood.Contains("dc_alcohol") ? (decimal)mfw.MealFood["dc_alcohol"] : 0;
                                            mealFoodsCollection.Entities.Add(mfw.MealFood);

                                        }
                                        logger.debug("Kcals: " + Math.Round(kcals, 2) + ": mealKcals Target: " + Math.Round(mealKcals));

                                        if (currentMeal == Meals.Breakfast || currentMeal == Meals.Lunch || currentMeal == Meals.Dinner)
                                        {
                                            carryOvers = Meal.CarryOvers(meal);
                                            carryOversKcals = mealKcals - kcals;
                                        }
                                    }
                                    mealFoods.Clear();
                                }
                            }//For loop that retreives new meal foods
                        }//Done with main meals
                        if (mealSuccess)
                        {
                            logger.debug("--------------------------------");
                            logger.debug("SUCCESS: Write meal to db");


                            decimal fat = 0m;
                            decimal protein = 0m;
                            decimal carbs = 0m;
                            decimal alcohol = 0m;
                            //Create relationship from day to meal
                            decimal kcals = 0m;
                            meal = Meal.UpdateTotals(meal, mealFoods);
                            foreach (MealFoodWrapper mfw in mealFoods)
                            {
                                mfw.MealFood = MealFood.CalculateKcal(mfw.MealFood);
                                mealFoodsCollection.Entities.Add(mfw.MealFood);
                                kcals       += (decimal)mfw.MealFood["dc_kcals"];
                                fat         += (decimal)mfw.MealFood["dc_fat"];
                                protein     += (decimal)mfw.MealFood["dc_protein"];
                                alcohol     += mfw.MealFood.Contains("dc_alcohol") ? (decimal)mfw.MealFood["dc_alcohol"] : 0;
                                carbs       += (decimal)mfw.MealFood["dc_carbohydrate"];
                            }
                            logger.debug("Kcals: " + Math.Round(kcals, 2) + ": mealKcals Target: " + Math.Round(mealKcals));

                            if (currentMeal == Meals.Breakfast || currentMeal == Meals.Lunch || currentMeal == Meals.Dinner)
                            {
                                carryOvers = Meal.CarryOvers(meal);
                                carryOversKcals = mealKcals - kcals;
                            }
                        }
                    }
                }//done with days
                //Write to database
                logger.debug("Menu generation complete.  Writing to database");
                if (multipleProfiles)
                {
                    //Need to figure out breakdown of percentage.  Get dictonary of contact Guids and kcal percents
                    Dictionary<Guid, decimal> list =  ap.RetrieveAdditionalProfilePrecents(contactId, crmService, kcal);
                    Clone c = new Clone();
                    foreach (KeyValuePair<Guid, decimal> pair in list.Reverse())  //reverse order.  Process primary profile first
                    {
                        logger.debug("Processing: " + pair.Key + " : " + pair.Value);
                        Entity menuClone            = c.Execute(menu, crmService, new KeyValuePair<String, EntityReference>(String.Empty, null));
                        menuClone["dc_contactid"]   = new EntityReference("contact", pair.Key);
                        //Now size the menu down to the proper percent
                        logger.debug("Adjust Portion Size");
                        ap.AdjustPortionSize(menuClone, pair.Value);
                        logger.debug("Round Up Portion Size");
                        Clone.RoundUpPortionSize(menuClone);
                        logger.debug("Remove Min Amount");
                        Clone.RemoveMinAmount(menuClone);
                        //set menus primary to false
                        logger.debug("Set Primary To False");
                        Menu.SetPrimaryToFalse(pair.Key, crmService);
                        logger.debug("Creating the clone");
                        Guid menuId = crmService.Create(menuClone);
                        //Generate fitness logs
                        User u = new User();
                        logger.debug("Generate Default Activity Level");
                        u.GenerateDefaultActivityLevel(pair.Key, menuId, crmService);
                        logger.debug("Created menu (clone): " + menuId);
                        //Need to pass back the primary menu Id
                        if (pair.Key == contactId)
                        {
                            results = Success.Create(menuId.ToString());
                        }
                    }
                }
                else
                {
                    Clone.RemoveMinAmount(menu);
                    //set menus primary to false
                    Menu.SetPrimaryToFalse(contactId, crmService);
                    Guid menuId = crmService.Create(menu);
                    logger.debug("Created menu: " + menuId);
                    //Generate fitness logs
                    User u = new User();
                    u.GenerateDefaultActivityLevel(contactId, menuId, crmService);
                    results     = Success.Create(menuId.ToString());
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                logger.error("Code: " + ex.Detail.ErrorCode);
                logger.error("Message: " + ex.Detail.Message);
                logger.error("Trace: " + ex.Detail.TraceText);
                logger.error("Inner Fault: " + ex.Detail.InnerFault);
                results = Error.Create(ex.Detail.Message);
            }
            return (results);
        }
    }
}