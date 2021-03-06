﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using DynamicConnections.CRM2011.Common.Utility;
using Microsoft.Xrm.Sdk.Client;
using System.Xml;
using Microsoft.Xrm.Sdk;
using System.Web.Services.Protocols;
using System.ServiceModel;
using Microsoft.Xrm.Sdk.Query;

using DynamicConnections.NutriStyle.CRM2011.Webservices.Engine.Helpers;


namespace DynamicConnections.NutriStyle.CRM2011.WebServices.Engine
{
    public class FoodNutrients
    {

        Logger logger;
        public static OrganizationServiceProxy crmService = null;

        public FoodNutrients()
        {

            String level = ConfigurationManager.AppSettings["LogLevel"];
            String location = ConfigurationManager.AppSettings["LogLocation"];
            logger = new Logger(level, location);
            String user = ConfigurationManager.AppSettings["CrmUser"];
            String password = ConfigurationManager.AppSettings["Password"];
            String orgName = ConfigurationManager.AppSettings["CrmOrganization"];
            String hostname = ConfigurationManager.AppSettings["Hostname"];
            String domain = ConfigurationManager.AppSettings["Domain"];
            String portnumber = ConfigurationManager.AppSettings["Portnumber"];

            try
            {

                logger.debug("orgName:" + orgName);
                logger.debug("user:" + user);
                logger.debug("password:" + password);
                logger.debug("domain:" + domain);
                logger.debug("hostname:" + hostname);
                logger.debug("portnumber:" + portnumber);

                if (crmService == null)
                {
                    crmService = CrmHelper.CreateCrmService(user, password, domain, hostname, orgName, Convert.ToInt32(portnumber));


                    logger.debug("Built crm service object");
                }
                else
                {
                    logger.debug("Value was set");
                    logger.debug(crmService.CallerId.ToString());
                }
            }
            catch (FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> e)
            {
                logger.debug(e.Message);
                logger.debug(e.StackTrace);
            }

            catch (SoapException e)
            {
                logger.debug(e.Message);
                logger.debug(e.Detail.InnerText);
                logger.debug(e.StackTrace);
            }
            catch (Exception e)
            {
                logger.debug(e.Message);
                logger.debug(e.StackTrace);
            }
        }
        public XmlDocument Retrieve(Guid menuId, int dayId, Guid contactId, String entityName, DateTime date)
        {
            logger.debug("menuId:" + menuId);
            logger.debug("dayId:" + dayId);
            logger.debug("contactId:" + contactId);
            logger.debug("entityName:" + entityName);
            logger.debug("date:" + date.ToString());

            String fetchXml = String.Empty;
            if (entityName.Equals("dc_mealfood", StringComparison.OrdinalIgnoreCase))
            {
                fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                      <entity name='dc_mealfood'>
                        <attribute name='dc_portionsize' />
                        <attribute name='dc_kcals' />
                        <attribute name='dc_alcohol' />
                        <attribute name='dc_carbohydrate' />
                        <attribute name='dc_protein' />
                        
                        <attribute name='dc_fat' />
                        
                        <link-entity name='dc_foods' from='dc_foodsid' to='dc_foodid' alias='dc_foods'>
                                <attribute name='dc_portion_amount' />
                            <link-entity name='dc_food_nutrients' from='dc_food_nutrientsid' to='dc_foodnutrientid' alias='dc_food_nutrients'>
                                <attribute name='dc_fa_unsat' />
                                <attribute name='dc_fa_mono' />
                                <attribute name='dc_fa_poly' />
                                <attribute name='dc_fa_sat' />
                                <attribute name='dc_cholestrol' />
                                <attribute name='dc_sodium' />
                                <attribute name='dc_fiber' />
                                <attribute name='dc_vit_a' />
                                <attribute name='dc_vit_c' />
                                <attribute name='dc_vitamindd2d3ing' />
                                <attribute name='dc_vit_e' />
                                <attribute name='dc_vit_k' />
                                <attribute name='dc_thiamin' />
                                <attribute name='dc_riboflavin' />
                                <attribute name='dc_niacin' />
                                <attribute name='dc_vit_b6' />
                                <attribute name='dc_folate' />
                                <attribute name='dc_vit_b12' />
                                <attribute name='dc_panto_acid' />
                                <attribute name='dc_biotin' />
                                <attribute name='dc_choline' />
                                <attribute name='dc_calcium' />
                                <attribute name='dc_magnesium' />
                                <attribute name='dc_potassium' />
                                <attribute name='dc_iron' />
                                <attribute name='dc_zinc' />
                                <attribute name='dc_copper' />
                                <attribute name='dc_manganese' />
                                <attribute name='dc_chromium' />
                                <attribute name='dc_selenium' />
                                <attribute name='dc_phosphorus' />
                            </link-entity>
                        </link-entity>
                        <link-entity name='dc_meal' from='dc_mealid' to='dc_mealid' alias='dc_meal'>
                          
                          <link-entity name='dc_day' from='dc_dayid' to='dc_dayid' alias='dc_day'>
                           
                                <filter type='and'>
                                    <condition attribute='dc_day' operator='eq' value='@DAY' />
                                </filter>
                            <link-entity name='dc_menu' from='dc_menuid' to='dc_menuid' alias='dc_menu'>
                            
                              <filter type='and'>
                                <condition attribute='dc_contactid' operator='eq' value='@CONTACTID' />
                                <condition attribute='dc_menuid' operator='eq' value='@MENUID' />
                                
                              </filter>
                            </link-entity>
                          </link-entity>
                        </link-entity>
                      </entity>
                    </fetch>";
                fetchXml = fetchXml.Replace("@DAY", dayId.ToString());
                fetchXml = fetchXml.Replace("@CONTACTID", contactId.ToString());
                fetchXml = fetchXml.Replace("@MENUID", menuId.ToString());
            }
            else if (entityName.Equals("dc_foodlog", StringComparison.OrdinalIgnoreCase))
            {
                fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                      <entity name='dc_foodlog'>
                        <attribute name='dc_portionsize' />
                        <attribute name='dc_kcals' />
                        <attribute name='dc_carbohydrate' />
                        <attribute name='dc_protein' />
                        <attribute name='dc_fat' />
                        <attribute name='dc_alcohol' />
                       
                        
                        <link-entity name='dc_foods' from='dc_foodsid' to='dc_foodid' alias='dc_foods'>
                                <attribute name='dc_portion_amount' />
                            <link-entity name='dc_food_nutrients' from='dc_food_nutrientsid' to='dc_foodnutrientid' alias='dc_food_nutrients'>
                                <attribute name='dc_fa_unsat' />
                                <attribute name='dc_fa_mono' />
                                <attribute name='dc_fa_poly' />
                                <attribute name='dc_fa_sat' />
                                <attribute name='dc_cholestrol' />
                                <attribute name='dc_sodium' />
                                <attribute name='dc_fiber' />
                                <attribute name='dc_vit_a' />
                                <attribute name='dc_vit_c' />
                                <attribute name='dc_vitamindd2d3ing' />
                                <attribute name='dc_vit_e' />
                                <attribute name='dc_vit_k' />
                                <attribute name='dc_thiamin' />
                                <attribute name='dc_riboflavin' />
                                <attribute name='dc_niacin' />
                                <attribute name='dc_vit_b6' />
                                <attribute name='dc_folate' />
                                <attribute name='dc_vit_b12' />
                                <attribute name='dc_panto_acid' />
                                <attribute name='dc_biotin' />
                                <attribute name='dc_choline' />
                                <attribute name='dc_calcium' />
                                <attribute name='dc_magnesium' />
                                <attribute name='dc_potassium' />
                                <attribute name='dc_iron' />
                                <attribute name='dc_zinc' />
                                <attribute name='dc_copper' />
                                <attribute name='dc_manganese' />
                                <attribute name='dc_chromium' />
                                <attribute name='dc_selenium' />
                                <attribute name='dc_phosphorus' />
                            </link-entity>
                        </link-entity>
                        <link-entity name='dc_foodlogday' alias='aa' to='dc_foodlogdayid' from='dc_foodlogdayid'> 
                            <link-entity name='dc_menu' alias='ab' to='dc_menuid' from='dc_menuid'> 
                            <filter type='and'>
                                <condition attribute='dc_menuid' operator='eq' value='@MENUID' />
                             </filter>
                            </link-entity>
                        </link-entity>
                         <filter type='and'>
                          <condition attribute='statecode' operator='eq' value='0' />
                          <condition attribute='dc_date' operator='eq' value='@DATE' />
                          <condition attribute='dc_contactid' operator='eq' value='@CONTACTID' />
                        </filter>
                      </entity>
                    </fetch>";
                fetchXml = fetchXml.Replace("@CONTACTID", contactId.ToString());
                fetchXml = fetchXml.Replace("@MENUID", menuId.ToString());
                fetchXml = fetchXml.Replace("@DATE", date.ToString());
            }


            Dictionary<String, int> columnOrder = new Dictionary<String, int>();

            columnOrder.Add("dc_portionsize", columnOrder.Count());

            columnOrder.Add("dc_kcals", columnOrder.Count());
            columnOrder.Add("dc_alcohol", columnOrder.Count());
            columnOrder.Add("dc_carbohydrate", columnOrder.Count());
            columnOrder.Add("dc_protein", columnOrder.Count());
            columnOrder.Add("dc_fat", columnOrder.Count());
            
           

            columnOrder.Add("dc_food_nutrients.dc_fa_unsat", columnOrder.Count());
            columnOrder.Add("dc_food_nutrients.dc_fa_mono", columnOrder.Count());
            columnOrder.Add("dc_food_nutrients.dc_fa_poly", columnOrder.Count());
            columnOrder.Add("dc_food_nutrients.dc_fa_sat", columnOrder.Count());
            columnOrder.Add("dc_food_nutrients.dc_cholestrol", columnOrder.Count());
            columnOrder.Add("dc_food_nutrients.dc_sodium", columnOrder.Count());
            columnOrder.Add("dc_food_nutrients.dc_fiber", columnOrder.Count());
            columnOrder.Add("dc_food_nutrients.dc_vit_a", columnOrder.Count());
            columnOrder.Add("dc_food_nutrients.dc_vit_c", columnOrder.Count());
            columnOrder.Add("dc_food_nutrients.dc_vitamindd2d3ing", columnOrder.Count());
            columnOrder.Add("dc_food_nutrients.dc_vit_e", columnOrder.Count());

            columnOrder.Add("dc_food_nutrients.dc_vit_k", columnOrder.Count());
            columnOrder.Add("dc_food_nutrients.dc_thiamin", columnOrder.Count());
            columnOrder.Add("dc_food_nutrients.dc_riboflavin", columnOrder.Count());
            columnOrder.Add("dc_food_nutrients.dc_niacin", columnOrder.Count());
            columnOrder.Add("dc_food_nutrients.dc_vit_b6", columnOrder.Count());
            columnOrder.Add("dc_food_nutrients.dc_folate", columnOrder.Count());

            columnOrder.Add("dc_food_nutrients.dc_vit_b12", columnOrder.Count());
            columnOrder.Add("dc_food_nutrients.dc_panto_acid", columnOrder.Count());
            columnOrder.Add("dc_food_nutrients.dc_biotin", columnOrder.Count());
            columnOrder.Add("dc_food_nutrients.dc_choline", columnOrder.Count());
            columnOrder.Add("dc_food_nutrients.dc_calcium", columnOrder.Count());
            columnOrder.Add("dc_food_nutrients.dc_magnesium", columnOrder.Count());
            columnOrder.Add("dc_food_nutrients.dc_potassium", columnOrder.Count());

            columnOrder.Add("dc_food_nutrients.dc_iron", columnOrder.Count());
            columnOrder.Add("dc_food_nutrients.dc_zinc", columnOrder.Count());
            columnOrder.Add("dc_food_nutrients.dc_copper", columnOrder.Count());
            columnOrder.Add("dc_food_nutrients.dc_manganese", columnOrder.Count());
            columnOrder.Add("dc_food_nutrients.dc_chromium", columnOrder.Count());
            columnOrder.Add("dc_food_nutrients.dc_selenium", columnOrder.Count());
            columnOrder.Add("dc_food_nutrients.dc_phosphorus", columnOrder.Count());

            EntityCollection response = null;
            XmlDocument results = Success.Create("worked");

            try
            {
                response = crmService.RetrieveMultiple(new FetchExpression(fetchXml));
            }
            catch (FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> ex)
            {
                logger.error("Code: " + ex.Detail.ErrorCode);
                logger.error("Message: " + ex.Detail.Message);
                logger.error("Trace: " + ex.Detail.TraceText);
                logger.error("Inner Fault: " + ex.Detail.InnerFault);
            }
            Dictionary<String, Nutrients> nutrientsList = new Dictionary<String, Nutrients>();

            results = DatabaseHelper.BuildXml(response, entityName, results, crmService, columnOrder);
            logger.debug("results: " + results.OuterXml);

            XmlNodeList records = results.GetElementsByTagName(entityName);

            String[] roundTenths = new String[] {
               
                "dc_food_nutrients.dc_thiamin", 
                "dc_food_nutrients.dc_riboflavin",
                "dc_food_nutrients.dc_vit_b6",
                "dc_food_nutrients.dc_vit_b12",
                
                "dc_food_nutrients.dc_manganese" 
            };
            String[] nutrients = new String[] {
                "dc_kcals", 

                "dc_carbohydrate", 
                "dc_protein", 
                
                "dc_fat", 
                "dc_portionsize", 
                "dc_foods.dc_portion_amount",
                "dc_food_nutrients.dc_fa_unsat", 
                "dc_food_nutrients.dc_fa_mono", 
                "dc_food_nutrients.dc_fa_poly", 
                "dc_food_nutrients.dc_fa_sat", 
                "dc_food_nutrients.dc_cholestrol", 
                "dc_food_nutrients.dc_sodium", 
                "dc_food_nutrients.dc_fiber",
                "dc_food_nutrients.dc_vit_a", 
                "dc_food_nutrients.dc_vit_c", 
                "dc_food_nutrients.dc_vitamindd2d3ing", 
                "dc_food_nutrients.dc_vit_e",
                "dc_food_nutrients.dc_vit_k",
                "dc_food_nutrients.dc_thiamin", 
                "dc_food_nutrients.dc_riboflavin", 
                "dc_food_nutrients.dc_niacin", 
                "dc_food_nutrients.dc_vit_b6", 
                "dc_food_nutrients.dc_folate", 
                "dc_food_nutrients.dc_vit_b12", 
                "dc_food_nutrients.dc_panto_acid", 
                "dc_food_nutrients.dc_biotin", 
                "dc_food_nutrients.dc_choline",
                "dc_food_nutrients.dc_calcium",
                "dc_food_nutrients.dc_magnesium", 
                "dc_food_nutrients.dc_potassium", 
                "dc_food_nutrients.dc_iron", 
                "dc_food_nutrients.dc_zinc",  
                "dc_food_nutrients.dc_copper", 
                "dc_food_nutrients.dc_manganese",  
                "dc_food_nutrients.dc_selenium", 
                "dc_food_nutrients.dc_chromium", 
                "dc_food_nutrients.dc_phosphorus",
                "dc_alcohol"
                };
            try
            {
                //now find the needed DRI for this user/contact (contact)

                Entity c = crmService.Retrieve("contact", contactId, new ColumnSet(new String[] { "gendercode", "dc_age", "dc_weightkg", "dc_kcaltarget" }));

                String contactFetchXml = String.Empty;
                int gender = 0;
                decimal kcalTarget = 0m;

                if (((OptionSetValue)c["gendercode"]).Value == 1)
                {//male
                    gender = 948170000;
                }
                else if (((OptionSetValue)c["gendercode"]).Value == 2)
                {//female
                    gender = 948170001;
                }
                kcalTarget = (decimal) c["dc_kcaltarget"];

                if ((int)c["dc_age"] >= 1 && (int)c["dc_age"] <= 8) //gender doesn't matter here
                {
                    contactFetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                      <entity name='dc_dri'>
                        <attribute name='dc_driid' />

                        <attribute name='dc_vitamina' />
                        <attribute name='dc_vitaminc' />
                        <attribute name='dc_vitamind' />
                        <attribute name='dc_riboflavin' />
                        <attribute name='dc_niacin' />
                        <attribute name='dc_thiamin' />
                        <attribute name='dc_vitaminb6' />
                        <attribute name='dc_folate' />
                        <attribute name='dc_biotin' />
                        <attribute name='dc_vitaminb12' />
                        <attribute name='dc_pantothenicacid' />
                        <attribute name='dc_biotin' />
                        <attribute name='dc_choline' />
                        <attribute name='dc_vitamink' />
                        <attribute name='dc_vitamine' />
                        
                        <attribute name='dc_magnesium' />
                        <attribute name='dc_chromium' />
                        <attribute name='dc_copper' />
                        <attribute name='dc_calcium' />
                        <attribute name='dc_iron' />
                        <attribute name='dc_manganese' />
                        <attribute name='dc_phosphorus' />
                        <attribute name='dc_selenium' />
                        <attribute name='dc_potassium' />
                        <attribute name='dc_zinc' />
                        <attribute name='dc_protein' />
                        <attribute name='dc_fiber' />
                        <attribute name='dc_sodiumupperlimit' />
                        

                        <order attribute='dc_name' descending='false' />
                        <filter type='and'>
                          <condition attribute='statecode' operator='eq' value='0' />
                          <condition attribute='dc_startyear' operator='ge' value='@AGE' />
                          <condition attribute='dc_endyear' operator='le' value='@AGE' />
                        </filter>
                      </entity>
                    </fetch>";
                }
                else
                {
                    contactFetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                      <entity name='dc_dri'>
                        <attribute name='dc_driid' />
                        
                        <attribute name='dc_vitamina' />
                        <attribute name='dc_vitaminc' />
                        <attribute name='dc_vitamind' />
                        <attribute name='dc_riboflavin' />
                        <attribute name='dc_niacin' />
                        <attribute name='dc_thiamin' />
                        <attribute name='dc_vitaminb6' />
                        <attribute name='dc_folate' />
                        <attribute name='dc_biotin' />
                        <attribute name='dc_vitaminb12' />
                        <attribute name='dc_pantothenicacid' />
                        <attribute name='dc_choline' />
                        <attribute name='dc_vitamink' />
                        <attribute name='dc_vitamine' />
                        
                        <attribute name='dc_magnesium' />
                        <attribute name='dc_chromium' />
                        <attribute name='dc_copper' />
                        <attribute name='dc_calcium' />
                        <attribute name='dc_iron' />
                        <attribute name='dc_manganese' />
                        <attribute name='dc_phosphorus' />
                        <attribute name='dc_selenium' />
                        <attribute name='dc_potassium' />
                        <attribute name='dc_zinc' />
                        <attribute name='dc_protein' />
                        <attribute name='dc_fiber' />
                        <attribute name='dc_sodiumupperlimit' />


                        <order attribute='dc_name' descending='false' />
                        <filter type='and'>
                          <condition attribute='statecode' operator='eq' value='0' />
                          <condition attribute='dc_gendercode' operator='eq' value='@GENDERCODE' />
                          <condition attribute='dc_startyear' operator='le' value='@AGE' />
                          <condition attribute='dc_endyear' operator='ge' value='@AGE' />
                        </filter>
                      </entity>
                    </fetch>";
                }

                contactFetchXml = contactFetchXml.Replace("@AGE", ((int)c["dc_age"]).ToString());

                contactFetchXml = contactFetchXml.Replace("@GENDERCODE", gender.ToString());

                response = null;

                logger.debug("---------------------------------------------------");
                //logger.debug("ContactXml: " + contactFetchXml);
                logger.debug("---------------------------------------------------");
                try
                {
                    response = crmService.RetrieveMultiple(new FetchExpression(contactFetchXml));
                }
                catch (FaultException<OrganizationServiceFault> ex)
                {
                    logger.error("Code: " + ex.Detail.ErrorCode);
                    logger.error("Message: " + ex.Detail.Message);
                    logger.error("Trace: " + ex.Detail.TraceText);
                    logger.error("Inner Fault: " + ex.Detail.InnerFault);
                }

                logger.debug("Found " + response.Entities.Count + " DRI entity");

                Dictionary<String, decimal> DRI = new Dictionary<string, decimal>();

                DRI.Add("dc_food_nutrients.dc_iron", 0m);
                DRI.Add("dc_food_nutrients.dc_potassium", 0m);
                DRI.Add("dc_food_nutrients.dc_magnesium", 0m);
                DRI.Add("dc_food_nutrients.dc_calcium", 0m);
                DRI.Add("dc_food_nutrients.dc_vit_b12", 0m);
                DRI.Add("dc_food_nutrients.dc_folate", 0m);
                DRI.Add("dc_food_nutrients.dc_vit_b6", 0m);
                DRI.Add("dc_food_nutrients.dc_niacin", 0m);
                DRI.Add("dc_food_nutrients.dc_vit_e", 0m);
                DRI.Add("dc_food_nutrients.dc_vit_c", 0m);
                DRI.Add("dc_food_nutrients.dc_fiber", 0m);

                DRI.Add("dc_food_nutrients.dc_sodium", 0m);
                DRI.Add("dc_food_nutrients.dc_vit_a", 0m);
                DRI.Add("dc_food_nutrients.dc_vitamindd2d3ing", 0m);
                DRI.Add("dc_food_nutrients.dc_vit_k", 0m);
                DRI.Add("dc_food_nutrients.dc_thiamin", 0m);

                DRI.Add("dc_food_nutrients.dc_panto_acid", 0m);
                DRI.Add("dc_food_nutrients.dc_choline", 0m);
                DRI.Add("dc_food_nutrients.dc_zinc", 0m);
                DRI.Add("dc_food_nutrients.dc_copper", 0m);
                DRI.Add("dc_food_nutrients.dc_manganese", 0m);
                DRI.Add("dc_food_nutrients.dc_selenium", 0m);
                DRI.Add("dc_food_nutrients.dc_chromium", 0m);
                DRI.Add("dc_food_nutrients.dc_phosphorus", 0m);

                DRI.Add("dc_food_nutrients.dc_riboflavin", 0m);

                DRI.Add("dc_food_nutrients.dc_biotin", 0m);

                if (response != null && response.Entities.Count > 0)
                {
                    Entity entity = response.Entities[0];

                    if (entity.Contains("dc_vitamina"))
                    {
                        DRI["dc_food_nutrients.dc_vit_a"] = (decimal)entity["dc_vitamina"];
                    }
                    if (entity.Contains("dc_vitaminc"))
                    {
                        DRI["dc_food_nutrients.dc_vit_c"] = (decimal)entity["dc_vitaminc"];
                    }
                    if (entity.Contains("dc_vitamine"))
                    {
                        DRI["dc_food_nutrients.dc_vit_e"] = (decimal)entity["dc_vitamine"];
                    }

                    if (entity.Contains("dc_vitamind"))
                    {
                        DRI["dc_food_nutrients.dc_vitamindd2d3ing"] = (decimal)entity["dc_vitamind"];
                    }

                    if (entity.Contains("dc_thiamin"))
                    {
                        DRI["dc_food_nutrients.dc_thiamin"] = (decimal)entity["dc_thiamin"];
                    }

                    if (entity.Contains("dc_riboflavin"))
                    {
                        DRI["dc_food_nutrients.dc_riboflavin"] = (decimal)entity["dc_riboflavin"];
                    }
                    if (entity.Contains("dc_vitaminb12"))
                    {
                        DRI["dc_food_nutrients.dc_vit_b12"] = (decimal)entity["dc_vitaminb12"];
                    }
                    if (entity.Contains("dc_pantothenicacid"))
                    {
                        DRI["dc_food_nutrients.dc_panto_acid"] = (decimal)entity["dc_pantothenicacid"];
                    }
                    if (entity.Contains("dc_calcium"))
                    {
                        DRI["dc_food_nutrients.dc_calcium"] = (decimal)entity["dc_calcium"];
                    }
                    if (entity.Contains("dc_copper"))
                    {
                        DRI["dc_food_nutrients.dc_copper"] = (decimal)entity["dc_copper"];
                    }
                    if (entity.Contains("dc_iron"))
                    {
                        DRI["dc_food_nutrients.dc_iron"] = (decimal)entity["dc_iron"];
                    }
                    if (entity.Contains("dc_magnesium"))
                    {
                        DRI["dc_food_nutrients.dc_magnesium"] = (decimal)entity["dc_magnesium"];
                    }
                    if (entity.Contains("dc_folate"))
                    {
                        DRI["dc_food_nutrients.dc_folate"] = (decimal)entity["dc_folate"];
                    }
                    if (entity.Contains("dc_niacin"))
                    {
                        DRI["dc_food_nutrients.dc_niacin"] = (decimal)entity["dc_niacin"];
                    }

                    if (entity.Contains("dc_manganese"))
                    {
                        DRI["dc_food_nutrients.dc_manganese"] = (decimal)entity["dc_manganese"];
                    }

                    if (entity.Contains("dc_phosphorus"))
                    {
                        DRI["dc_food_nutrients.dc_phosphorus"] = (decimal)entity["dc_phosphorus"];
                    }

                    if (entity.Contains("dc_selenium"))
                    {
                        DRI["dc_food_nutrients.dc_selenium"] = (decimal)entity["dc_selenium"];
                    }

                    if (entity.Contains("dc_zinc"))
                    {
                        DRI["dc_food_nutrients.dc_zinc"] = (decimal)entity["dc_zinc"];
                    }
                    if (entity.Contains("dc_potassium"))
                    {
                        DRI["dc_food_nutrients.dc_potassium"] = (decimal)entity["dc_potassium"];
                    }
                    if (entity.Contains("dc_biotin"))
                    {
                        DRI["dc_food_nutrients.dc_biotin"] = (decimal)entity["dc_biotin"];
                    }
                    if (entity.Contains("dc_sodiumupperlimit"))
                    {
                        DRI["dc_food_nutrients.dc_sodium"] = (decimal)entity["dc_sodiumupperlimit"];
                    }


                    if (entity.Contains("dc_vitamink"))
                    {
                        DRI["dc_food_nutrients.dc_vit_k"] = (decimal)entity["dc_vitamink"];
                    }

                    if (entity.Contains("dc_vitaminb6"))
                    {
                        DRI["dc_food_nutrients.dc_vit_b6"] = (decimal)entity["dc_vitaminb6"];
                    }

                    if (entity.Contains("dc_choline"))
                    {
                        DRI["dc_food_nutrients.dc_choline"] = (decimal)entity["dc_choline"];
                    }

                    if (entity.Contains("dc_chromium"))
                    {
                        DRI["dc_food_nutrients.dc_chromium"] = (decimal)entity["dc_chromium"];
                    }

                    if (entity.Contains("dc_fiber"))
                    {
                        //need the kcal target of the user here

                        //DRI["dc_food_nutrients.dc_fiber"] = ((decimal)entity["dc_fiber"] / 1000m) * 14m;
                        DRI["dc_food_nutrients.dc_fiber"] = (kcalTarget / 1000m) * 14m;
                        logger.debug("fiber (DRI): " + DRI["dc_food_nutrients.dc_fiber"]);
                    }
                }
              

                //Add all values.  Set missing = true.
                foreach (String str in nutrients)
                {
                    nutrientsList.Add(str, new Nutrients() { Missing = false, Value = 0 });
                }
                //Grouping - also determine if a value is missing
                foreach (XmlNode record in records)
                {
                    //logger.debug("Record: " + record.OuterXml);
                    Dictionary<String, Nutrients> list = new Dictionary<String, Nutrients>();

                    //Add all values.  Set missing = true.
                    foreach (String str in nutrients)
                    {
                        list.Add(str, new Nutrients() { Missing = true, Value = 0 });
                    }

                    foreach (XmlNode child in record.ChildNodes)
                    {
                        logger.debug("Processing: " + child.Name);
                        if (child.Name.Equals(entityName + "id", StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }
                        list[child.Name].Missing = false;
                        list[child.Name].Value = Convert.ToDecimal(child.InnerText);
                    }
                    //Now merge the list and nutrient collection.  Use the appropriate mulipliers
                    decimal orgPortionSize = list["dc_foods.dc_portion_amount"].Value;
                    decimal portionSize = list["dc_portionsize"].Value;

                    decimal multiplier = portionSize / orgPortionSize;
                    /*
                    logger.debug("orgPortionSize: " + orgPortionSize);
                    logger.debug("portionSize: " + portionSize);
                    logger.debug("multiplier: " + multiplier);
                    */
                    //Now multiple the values.  Skip dc_fat, dc_protein, dc_carbohydrate, dc_portionsize, dc_portion_amount
                    foreach (KeyValuePair<String, Nutrients> n in list)
                    {
                        if (!n.Value.Missing && !n.Key.Equals("dc_fat") && !n.Key.Equals("dc_protein") && !n.Key.Equals("dc_carbohydrate")
                            && !n.Key.Equals("dc_portionsize") && !n.Key.Equals("dc_foods.dc_portion_amount") && !n.Key.Equals("dc_kcals"))
                        {
                            if (n.Key.Equals("dc_food_nutrients.dc_cholestrol", StringComparison.OrdinalIgnoreCase))
                            {
                                logger.debug("orgPortionSize: " + orgPortionSize);
                                logger.debug("portionSize: " + portionSize);
                                logger.debug("multiplier: " + multiplier);
                                logger.debug("Processing: " + n.Key + " : " + n.Value.Value + " : " + multiplier);
                            }
                            n.Value.Value = n.Value.Value * multiplier;
                        }
                    }
                    //Data is now correct.  Update primary list: nutrientsList

                    foreach (KeyValuePair<String, Nutrients> n in list)
                    {
                        if (!n.Value.Missing)
                        {
                            //logger.debug("Adding: " + n.Key);
                            nutrientsList[n.Key].Value = nutrientsList[n.Key].Value + list[n.Key].Value;

                        }
                        else//value is missing.  Update master list with this data
                        {
                            nutrientsList[n.Key].Missing = true;
                        }
                    }
                }

                foreach (KeyValuePair<String, Nutrients> n in nutrientsList)
                {
                    logger.debug(n.ToString());
                }

                //now remove all dc_mealfood nodes and re-add just one
                XmlNodeList nodes = results.GetElementsByTagName(entityName);
                while (nodes.Count > 0)
                {
                    nodes[0].ParentNode.RemoveChild(nodes[0]);
                }
                //Cleaned out.  Now add back in
                XmlNode grams = results.CreateNode(XmlNodeType.Element, "grams", "");
                results.FirstChild.AppendChild(grams);
                XmlNode columns = results.GetElementsByTagName("columns")[0];
                foreach (KeyValuePair<String, Nutrients> n in nutrientsList)
                {
                    if (!n.Key.Equals("dc_portionsize") && !n.Key.Equals("dc_foods.dc_portion_amount"))
                    {
                        XmlNode node = results.CreateNode(XmlNodeType.Element, n.Key, "");
                        if (roundTenths.Contains(n.Key))
                        {
                            node.InnerText = Math.Round(n.Value.Value, 1).ToString("#,##.#");
                        }
                        else
                        {
                            node.InnerText = Math.Round(n.Value.Value, 0).ToString("#,##.#");
                        }
                        if (n.Value.Missing)
                        {
                            XmlAttribute at = results.CreateAttribute("missing");
                            at.InnerText = Boolean.TrueString;
                            node.Attributes.Append(at);
                        }

                        XmlAttribute dailyIntake = results.CreateAttribute("DI");
                       
                        if (DRI.ContainsKey(n.Key))
                        {
                            if (DRI[n.Key] != 0 && n.Value.Value != 0)
                            {
                                dailyIntake.InnerText = Math.Round((n.Value.Value / DRI[n.Key]) * 100, 0).ToString() + "%";
                            }
                            else
                            {
                                dailyIntake.InnerText = String.Empty;
                            }
                        }

                        node.Attributes.Append(dailyIntake);


                        XmlAttribute recommendedDailyIntake = results.CreateAttribute("RDI");
                        if (DRI.ContainsKey(n.Key))
                        {


                            //recommendedDailyIntake.InnerText = Math.Round(DRI[n.Key], 0).ToString();

                            if (roundTenths.Contains(n.Key))
                            {
                                recommendedDailyIntake.InnerText = Math.Round(DRI[n.Key], 1).ToString("#,##.#");
                            }
                            else
                            {
                                recommendedDailyIntake.InnerText = Math.Round(DRI[n.Key], 0).ToString("#,##.#");
                            }
                        }
                        else
                        {
                            recommendedDailyIntake.InnerText = "Multiple Recommendations";
                        }
                        node.Attributes.Append(recommendedDailyIntake);

                        grams.AppendChild(node);
                    }
                }
                //percents
                XmlNode percents = results.CreateNode(XmlNodeType.Element, "percents", "");
                results.FirstChild.AppendChild(percents);
                foreach (KeyValuePair<String, decimal> key in DRI)
                {
                    Nutrients n = nutrientsList[key.Key];

                    XmlNode node = results.CreateNode(XmlNodeType.Element, key.Key, "");
                    //Add attribute for daily intake
                    XmlAttribute dailyIntake = results.CreateAttribute("DI");
                    dailyIntake.InnerText = Math.Round(n.Value, 0).ToString();
                   
                    node.Attributes.Append(dailyIntake);

                    XmlAttribute recommendedDailyIntake = results.CreateAttribute("RDI");
                    if (roundTenths.Contains(key.Key))
                    {
                        recommendedDailyIntake.InnerText = Math.Round(key.Value, 1).ToString("#,##.#");
                    }
                    else
                    {
                        recommendedDailyIntake.InnerText = Math.Round(key.Value, 0).ToString("#,##.#");
                    }

                    node.Attributes.Append(recommendedDailyIntake);


                    if (key.Key.Equals("dc_protein"))
                    {
                        node.InnerText = Math.Round(((n.Value / key.Value) * 100), 0).ToString("#,##.#");
                    }

                    else if (key.Key.Equals("dc_food_nutrients.dc_fiber"))
                    {
                        //decimal kcalTarget = (decimal)c["dc_kcaltarget"];
                        logger.debug("kcalTarget: " + kcalTarget);
                        logger.debug("Fiber: " + n.Value);
                        node.InnerText = Math.Round((n.Value / ((kcalTarget / 1000) * 14)), 0).ToString();
                        //recommendedDailyIntake.InnerText = Math.Round(( ((kcalTarget / 1000) * 14)), 0).ToString();

                        logger.debug("node.InnerText (fiber): " + node.InnerText);
                        //logger.debug("recommendedDailyIntake.InnerText (fiber): " + recommendedDailyIntake.InnerText);
                    }
                    else if (DRI.ContainsKey(key.Key) && Convert.ToDecimal(DRI[key.Key]) > 0 && Convert.ToDecimal(n.Value.ToString()) > 0)
                    {

                        if (roundTenths.Contains(key.Key))
                        {
                            node.InnerText = Math.Round((n.Value / key.Value) * 100, 1).ToString("#,##.#");
                        }
                        else
                        {
                            node.InnerText = Math.Round((n.Value / key.Value) * 100, 1).ToString("#,##.#");
                        }

                        if (n.Missing)
                        {
                            XmlAttribute at = results.CreateAttribute("missing");
                            at.InnerText = Boolean.TrueString;
                            node.Attributes.Append(at);
                        }
                    }
                    else
                    {
                        node.InnerText = "0";
                        XmlAttribute at = results.CreateAttribute("missing");
                        at.InnerText = Boolean.TrueString;
                        node.Attributes.Append(at);
                    }
                    percents.AppendChild(node);
                }
            }
            catch (Exception e)
            {
                logger.error(e.Message);
                logger.error(e.StackTrace);
            }
            //logger.debug("Results returned");
            return (results);
        }
        /// <summary>
        /// Return xml for passed in food
        /// </summary>
        /// <param name="foodId"></param>
        /// <returns></returns>
        public XmlDocument RetrieveForEachFood(Guid foodId)
        {
            return (RetrieveForEachFood(Guid.Empty, int.MinValue, Guid.Empty, String.Empty, DateTime.MinValue, foodId, -1));
        }
        /// <summary>
        /// Return xml for passed in food
        /// </summary>
        /// <param name="foodId"></param>
        /// <returns></returns>
        public XmlDocument RetrieveForEachFood(Guid foodId, decimal portionSize)
        {
            return (RetrieveForEachFood(Guid.Empty, int.MinValue, Guid.Empty, String.Empty, DateTime.MinValue, foodId, portionSize));
        }
        public XmlDocument RetrieveForEachFood(Guid menuId, int dayId, Guid contactId, String entityName, DateTime date)
        {
            return (RetrieveForEachFood(menuId, dayId, contactId, entityName, date, Guid.Empty, -1m));
        }
        /// <summary>
        /// If foodId is not Guid.Empty it will only return the xml for the specified food.  If portionSize is a negative number it is ignored
        /// </summary>
        /// <param name="menuId"></param>
        /// <param name="dayId"></param>
        /// <param name="contactId"></param>
        /// <param name="entityName"></param>
        /// <param name="date"></param>
        /// <param name="foodId"></param>
        /// <returns></returns>
        public XmlDocument RetrieveForEachFood(Guid menuId, int dayId, Guid contactId, String entityName, DateTime date, Guid foodId, decimal portionSize)
        {
            logger.debug("menuId:" + menuId);
            logger.debug("dayId:" + dayId);
            logger.debug("contactId:" + contactId);

            logger.debug("foodId:" + foodId);
            logger.debug("portionSize: " + portionSize);

            XmlDocument results = Success.Create("worked");

            try
            {

                String fetchXml = String.Empty;
                if (foodId != Guid.Empty)
                {
                    entityName = "dc_foods";
                    fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                      <entity name='dc_foods'>
                        <attribute name='dc_portion_amount' />
                        <attribute name='dc_portiontypeid' />
                        <attribute name='dc_foodsid' />
                        <filter type='and'>
                            <condition attribute='dc_foodsid' operator='eq' value='@FOODID' />
                        </filter>
                        <link-entity name='dc_food_nutrients' alias='dc_food_nutrients' to='dc_foodnutrientid' from='dc_food_nutrientsid'>
                            <attribute name='dc_kcals' />
                            <attribute name='dc_carbohydrate' />
                            <attribute name='dc_alcohol' />
                            <attribute name='dc_protein' />
                            <attribute name='dc_fat' />
                            
                            <attribute name='dc_fa_unsat' />
                            <attribute name='dc_fa_mono' />
                            <attribute name='dc_fa_poly' />
                            <attribute name='dc_fa_sat' />
                            <attribute name='dc_cholestrol' />
                            <attribute name='dc_sodium' />
                            <attribute name='dc_fiber' />
                            <attribute name='dc_vit_a' />
                            <attribute name='dc_vit_c' />
                            <attribute name='dc_vitamindd2d3ing' />
                            <attribute name='dc_vit_e' />
                            <attribute name='dc_vit_k' />
                            <attribute name='dc_thiamin' />
                            <attribute name='dc_riboflavin' />
                            <attribute name='dc_niacin' />
                            <attribute name='dc_vit_b6' />
                            <attribute name='dc_folate' />
                            <attribute name='dc_vit_b12' />
                            <attribute name='dc_panto_acid' />
                            <attribute name='dc_biotin' />
                            <attribute name='dc_choline' />
                            <attribute name='dc_calcium' />
                            <attribute name='dc_magnesium' />
                            <attribute name='dc_potassium' />
                            <attribute name='dc_iron' />
                            <attribute name='dc_zinc' />
                            <attribute name='dc_copper' />
                            <attribute name='dc_manganese' />
                            <attribute name='dc_chromium' />
                            <attribute name='dc_selenium' />
                            <attribute name='dc_phosphorus' />
                        </link-entity>
                      </entity>
                    </fetch>";
                    fetchXml = fetchXml.Replace("@FOODID", foodId.ToString());
                    logger.debug("Fetchxml: " + fetchXml);
                }
                else if (entityName.Equals("dc_mealfood", StringComparison.OrdinalIgnoreCase))
                {
                    fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                      <entity name='dc_mealfood'>
                        <attribute name='dc_portionsize' />
                        <attribute name='dc_portiontypeid' />
                        <attribute name='dc_foodid' />
                        <attribute name='dc_kcals' />
                        <attribute name='dc_carbohydrate' />
                        <attribute name='dc_alcohol' />
                        <attribute name='dc_protein' />
                        <attribute name='dc_fat' />
                        <order attribute='dc_mealid' descending='false' />
                        <link-entity name='dc_foods' from='dc_foodsid' to='dc_foodid' alias='dc_foods'>
                                <attribute name='dc_portion_amount' />
                            <link-entity name='dc_food_nutrients' from='dc_food_nutrientsid' to='dc_foodnutrientid' alias='dc_food_nutrients'>
                                <attribute name='dc_fa_unsat' />
                                <attribute name='dc_fa_mono' />
                                <attribute name='dc_fa_poly' />
                                <attribute name='dc_fa_sat' />
                                <attribute name='dc_cholestrol' />
                                <attribute name='dc_sodium' />
                                <attribute name='dc_fiber' />
                                <attribute name='dc_vit_a' />
                                <attribute name='dc_vit_c' />
                                <attribute name='dc_vitamindd2d3ing' />
                                <attribute name='dc_vit_e' />
                                <attribute name='dc_vit_k' />
                                <attribute name='dc_thiamin' />
                                <attribute name='dc_riboflavin' />
                                <attribute name='dc_niacin' />
                                <attribute name='dc_vit_b6' />
                                <attribute name='dc_folate' />
                                <attribute name='dc_vit_b12' />
                                <attribute name='dc_panto_acid' />
                                <attribute name='dc_biotin' />
                                <attribute name='dc_choline' />
                                <attribute name='dc_calcium' />
                                <attribute name='dc_magnesium' />
                                <attribute name='dc_potassium' />
                                <attribute name='dc_iron' />
                                <attribute name='dc_zinc' />
                                <attribute name='dc_copper' />
                                <attribute name='dc_manganese' />
                                <attribute name='dc_chromium' />
                                <attribute name='dc_selenium' />
                                <attribute name='dc_phosphorus' />
                            </link-entity>
                        </link-entity>
                        <link-entity name='dc_meal' from='dc_mealid' to='dc_mealid' alias='dc_meal'>
                          <attribute name='dc_meal' />
                          <link-entity name='dc_day' from='dc_dayid' to='dc_dayid' alias='dc_day'>
                           
                                <filter type='and'>
                                    <condition attribute='dc_day' operator='eq' value='@DAY' />
                                </filter>
                            <link-entity name='dc_menu' from='dc_menuid' to='dc_menuid' alias='dc_menu'>
                            
                              <filter type='and'>
                                <condition attribute='dc_contactid' operator='eq' value='@CONTACTID' />
                                <condition attribute='dc_menuid' operator='eq' value='@MENUID' />
                                
                              </filter>
                            </link-entity>
                          </link-entity>
                        </link-entity>
                      </entity>
                    </fetch>";
                    fetchXml = fetchXml.Replace("@DAY", dayId.ToString());
                    fetchXml = fetchXml.Replace("@CONTACTID", contactId.ToString());
                    fetchXml = fetchXml.Replace("@MENUID", menuId.ToString());
                }
                else if (entityName.Equals("dc_foodlog", StringComparison.OrdinalIgnoreCase))
                {
                    fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                      <entity name='dc_foodlog'>
                        <attribute name='dc_portionsize' />
                        <attribute name='dc_portiontypeid' />
                        <attribute name='dc_foodid' />
                        <attribute name='dc_kcals' />
                        <attribute name='dc_carbohydrate' />
                        <attribute name='dc_protein' />
                        <attribute name='dc_alcohol' />
                        <attribute name='dc_fat' />
                        <attribute name='dc_meal' />

                        <link-entity name='dc_foods' from='dc_foodsid' to='dc_foodid' alias='dc_foods'>
                                <attribute name='dc_portion_amount' />
                            <link-entity name='dc_food_nutrients' from='dc_food_nutrientsid' to='dc_foodnutrientid' alias='dc_food_nutrients'>
                                <attribute name='dc_fa_unsat' />
                                <attribute name='dc_fa_mono' />
                                <attribute name='dc_fa_poly' />
                                <attribute name='dc_fa_sat' />
                                <attribute name='dc_cholestrol' />
                                <attribute name='dc_sodium' />
                                <attribute name='dc_fiber' />
                                <attribute name='dc_vit_a' />
                                <attribute name='dc_vit_c' />
                                <attribute name='dc_vitamindd2d3ing' />
                                <attribute name='dc_vit_e' />
                                <attribute name='dc_vit_k' />
                                <attribute name='dc_thiamin' />
                                <attribute name='dc_riboflavin' />
                                <attribute name='dc_niacin' />
                                <attribute name='dc_vit_b6' />
                                <attribute name='dc_folate' />
                                <attribute name='dc_vit_b12' />
                                <attribute name='dc_panto_acid' />
                                <attribute name='dc_biotin' />
                                <attribute name='dc_choline' />
                                <attribute name='dc_calcium' />
                                <attribute name='dc_magnesium' />
                                <attribute name='dc_potassium' />
                                <attribute name='dc_iron' />
                                <attribute name='dc_zinc' />
                                <attribute name='dc_copper' />
                                <attribute name='dc_manganese' />
                                <attribute name='dc_chromium' />
                                <attribute name='dc_selenium' />
                                <attribute name='dc_phosphorus' />
                            </link-entity>
                        </link-entity>
                        <link-entity name='dc_foodlogday' alias='aa' to='dc_foodlogdayid' from='dc_foodlogdayid'> 
                            <link-entity name='dc_menu' alias='ab' to='dc_menuid' from='dc_menuid'> 
                                <filter type='and'>
                                    <condition attribute='dc_menuid' operator='eq' value='@MENUID' />
                                </filter>
                            </link-entity>
                        </link-entity>
                         <filter type='and'>
                          <condition attribute='statecode' operator='eq' value='0' />
                          <condition attribute='dc_date' operator='eq' value='@DATE' />
                          <condition attribute='dc_contactid' operator='eq' value='@CONTACTID' />
                          
                        </filter>
                      </entity>
                    </fetch>";
                    fetchXml = fetchXml.Replace("@DAY", dayId.ToString());
                    fetchXml = fetchXml.Replace("@CONTACTID", contactId.ToString());
                    fetchXml = fetchXml.Replace("@MENUID", menuId.ToString());
                    fetchXml = fetchXml.Replace("@DATE", date.ToString());
                }
                /*
                fetchXml = fetchXml.Replace("@DAY", dayId.ToString());
                fetchXml = fetchXml.Replace("@CONTACTID", contactId.ToString());
                fetchXml = fetchXml.Replace("@MENUID", menuId.ToString());
                */
                Dictionary<String, int> columnOrder = new Dictionary<String, int>();
                if (entityName.Equals("dc_mealfood", StringComparison.OrdinalIgnoreCase))
                {
                    columnOrder.Add("dc_meal.dc_meal", columnOrder.Count());
                }
                else if (entityName.Equals("dc_foodlog", StringComparison.OrdinalIgnoreCase))
                {
                    columnOrder.Add("dc_meal", columnOrder.Count());
                }

                columnOrder.Add("dc_portiontypeid", columnOrder.Count());

                if (foodId != Guid.Empty)
                {
                    columnOrder.Add("dc_portion_amount", columnOrder.Count());
                    columnOrder.Add("dc_foodsid", columnOrder.Count());

                    columnOrder.Add("dc_food_nutrients.dc_kcals", columnOrder.Count());
                    columnOrder.Add("dc_food_nutrients.dc_carbohydrate", columnOrder.Count());
                    columnOrder.Add("dc_food_nutrients.dc_alcohol", columnOrder.Count());
                    columnOrder.Add("dc_food_nutrients.dc_protein", columnOrder.Count());
                    columnOrder.Add("dc_food_nutrients.dc_fat", columnOrder.Count());
                }
                else
                {
                    columnOrder.Add("dc_portionsize", columnOrder.Count());
                    columnOrder.Add("dc_foodid", columnOrder.Count());
                    columnOrder.Add("dc_kcals", columnOrder.Count());
                    columnOrder.Add("dc_carbohydrate", columnOrder.Count());
                    columnOrder.Add("dc_alcohol", columnOrder.Count());
                    columnOrder.Add("dc_protein", columnOrder.Count());
                    columnOrder.Add("dc_fat", columnOrder.Count());
                }
                columnOrder.Add("dc_food_nutrients.dc_fa_unsat", columnOrder.Count());
                columnOrder.Add("dc_food_nutrients.dc_fa_mono", columnOrder.Count());
                columnOrder.Add("dc_food_nutrients.dc_fa_poly", columnOrder.Count());
                columnOrder.Add("dc_food_nutrients.dc_fa_sat", columnOrder.Count());
                columnOrder.Add("dc_food_nutrients.dc_cholestrol", columnOrder.Count());
                columnOrder.Add("dc_food_nutrients.dc_sodium", columnOrder.Count());
                columnOrder.Add("dc_food_nutrients.dc_fiber", columnOrder.Count());
                columnOrder.Add("dc_food_nutrients.dc_vit_a", columnOrder.Count());
                columnOrder.Add("dc_food_nutrients.dc_vit_c", columnOrder.Count());
                columnOrder.Add("dc_food_nutrients.dc_vitamindd2d3ing", columnOrder.Count());
                columnOrder.Add("dc_food_nutrients.dc_vit_e", columnOrder.Count());

                columnOrder.Add("dc_food_nutrients.dc_vit_k", columnOrder.Count());
                columnOrder.Add("dc_food_nutrients.dc_thiamin", columnOrder.Count());
                columnOrder.Add("dc_food_nutrients.dc_riboflavin", columnOrder.Count());
                columnOrder.Add("dc_food_nutrients.dc_niacin", columnOrder.Count());
                columnOrder.Add("dc_food_nutrients.dc_vit_b6", columnOrder.Count());
                columnOrder.Add("dc_food_nutrients.dc_folate", columnOrder.Count());

                columnOrder.Add("dc_food_nutrients.dc_vit_b12", columnOrder.Count());
                columnOrder.Add("dc_food_nutrients.dc_panto_acid", columnOrder.Count());
                columnOrder.Add("dc_food_nutrients.dc_biotin", columnOrder.Count());
                columnOrder.Add("dc_food_nutrients.dc_choline", columnOrder.Count());
                columnOrder.Add("dc_food_nutrients.dc_calcium", columnOrder.Count());
                columnOrder.Add("dc_food_nutrients.dc_magnesium", columnOrder.Count());
                columnOrder.Add("dc_food_nutrients.dc_potassium", columnOrder.Count());

                columnOrder.Add("dc_food_nutrients.dc_iron", columnOrder.Count());
                columnOrder.Add("dc_food_nutrients.dc_zinc", columnOrder.Count());
                columnOrder.Add("dc_food_nutrients.dc_copper", columnOrder.Count());
                columnOrder.Add("dc_food_nutrients.dc_manganese", columnOrder.Count());
                columnOrder.Add("dc_food_nutrients.dc_chromium", columnOrder.Count());
                columnOrder.Add("dc_food_nutrients.dc_selenium", columnOrder.Count());
                columnOrder.Add("dc_food_nutrients.dc_phosphorus", columnOrder.Count());

                EntityCollection response = null;

                try
                {
                    response = crmService.RetrieveMultiple(new FetchExpression(fetchXml));
                }
                catch (FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> ex)
                {
                    logger.error("Code: " + ex.Detail.ErrorCode);
                    logger.error("Message: " + ex.Detail.Message);
                    logger.error("Trace: " + ex.Detail.TraceText);
                    logger.error("Inner Fault: " + ex.Detail.InnerFault);
                }

                results = DatabaseHelper.BuildXml(response, entityName, results, crmService, columnOrder);

                XmlNodeList list = results.GetElementsByTagName(entityName);
                //Figure out portion size info
                foreach (XmlNode node in list)
                {
                    decimal foodPortionSize = 0m;
                    decimal orgPortionSize = 0m;

                    if (foodId != Guid.Empty)
                    {
                        foodPortionSize = String.IsNullOrEmpty(node.SelectSingleNode("dc_portion_amount").InnerText) ? 0m : Convert.ToDecimal(node.SelectSingleNode("dc_portion_amount").InnerText);
                        orgPortionSize = String.IsNullOrEmpty(node.SelectSingleNode("dc_portion_amount").InnerText) ? 0m : Convert.ToDecimal(node.SelectSingleNode("dc_portion_amount").InnerText);
                    }
                    else
                    {
                        foodPortionSize = String.IsNullOrEmpty(node.SelectSingleNode("dc_portionsize").InnerText) ? 0m : Convert.ToDecimal(node.SelectSingleNode("dc_portionsize").InnerText);
                        orgPortionSize = String.IsNullOrEmpty(node.SelectSingleNode("dc_foods.dc_portion_amount").InnerText) ? 0m : Convert.ToDecimal(node.SelectSingleNode("dc_foods.dc_portion_amount").InnerText);
                    }
                    decimal multipler = 1m;
                    if (portionSize > 0)
                    {
                        multipler = portionSize / orgPortionSize;
                        //Need to update the value of dc_portionsize to match the value passed in
                        if (node.SelectSingleNode("dc_portion_amount") != null)
                        {
                            node.SelectSingleNode("dc_portion_amount").InnerText = portionSize.ToString();
                        }
                    }
                    else
                    {
                        multipler = foodPortionSize / orgPortionSize;
                    }
                    //Update the values
                    foreach (KeyValuePair<String, int> pair in columnOrder)
                    {
                        if (pair.Key.StartsWith("dc_food_nutrients") && node.SelectSingleNode(pair.Key) != null)
                        {
                            node.SelectSingleNode(pair.Key).InnerText =
                                String.IsNullOrEmpty(node.SelectSingleNode(pair.Key).InnerText) ? String.Empty : (Convert.ToDecimal(node.SelectSingleNode(pair.Key).InnerText) * multipler).ToString();

                            //logger.debug("Setting value to: " + node.SelectSingleNode(pair.Key).InnerText);
                        }
                    }
                }
                //logger.debug("results: " + results.OuterXml);


                String[] roundTenths = new String[] {
                
                "dc_food_nutrients.dc_thiamin", 
                "dc_food_nutrients.dc_riboflavin",
                "dc_food_nutrients.dc_vit_b6",
                "dc_food_nutrients.dc_vit_b12",
               
                "dc_food_nutrients.dc_manganese", 
                "dc_portionsize", 
                "dc_portion_amount" 
                };

                XmlNodeList records = results.GetElementsByTagName(entityName);
                Decimal d = 0m;
                logger.debug("Entity Name: " + entityName);
                logger.debug("Found " + records.Count + " nodes");
                foreach (XmlNode mealFood in records)
                {
                    //logger.debug("Processing: " + mealFood.OuterXml);
                    if (mealFood.HasChildNodes)
                    {
                        foreach (XmlNode node in mealFood.ChildNodes)
                        {
                            if (roundTenths.Contains(node.Name))
                            {
                                if (!String.IsNullOrEmpty(node.InnerText) && Decimal.TryParse(node.InnerText, out d))
                                {
                                    node.InnerText = Math.Round(d, 1).ToString();
                                }
                            }
                            else
                            {
                                if (!String.IsNullOrEmpty(node.InnerText) && Decimal.TryParse(node.InnerText, out d))
                                {
                                    node.InnerText = Math.Round(d, 0).ToString();
                                }
                            }
                        }
                    }
                    else
                    {
                        if (roundTenths.Contains(mealFood.Name))
                        {
                            if (!String.IsNullOrEmpty(mealFood.InnerText) && Decimal.TryParse(mealFood.InnerText, out d))
                            {
                                mealFood.InnerText = Math.Round(d, 1).ToString();
                            }
                        }
                        else
                        {
                            if (!String.IsNullOrEmpty(mealFood.InnerText) && Decimal.TryParse(mealFood.InnerText, out d))
                            {
                                mealFood.InnerText = Math.Round(d, 0).ToString();
                            }
                        }

                    }
                }
            }
            catch (Exception e)
            {
                logger.error(e.Message);
                logger.error(e.StackTrace);
            }
            //logger.debug("Results returned: " + results.OuterXml);
            return (results);
        }

    }
}