﻿using DynamicConnections.CRM2011.Common.Utility;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;


namespace DynamicConnections.NutriStyle.CRM2011.MenuGenerator.Helpers
{
    class AdditionalProfiles : General
    {

        private static Logger logger = GetLogger();
        
        public AdditionalProfiles()
        {
        }
        /// <summary>
        /// Is there any additional profiles related to this contact?
        /// </summary>
        /// <param name="contactId"></param>
        /// <param name="crmService"></param>
        /// <returns></returns>
        public bool AreThereChildren(Guid contactId, OrganizationServiceProxy crmService)
        {

            bool additionalProfiles = false;

            try
            {
                String fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
                          <entity name='contact'>
                            <attribute name='firstname' />
                            <attribute name='lastname' />
                            <attribute name='contactid' />
                            <filter type='and'> 
                                <condition attribute='dc_parentcontactid' value='@CONTACTID' operator='eq'/> 
                                <condition attribute='dc_createmenu' value='1' operator='eq'/> 
                            </filter>
                          </entity>
                        </fetch>";

                fetchXml = fetchXml.Replace("@CONTACTID", contactId.ToString());

                EntityCollection collection = crmService.RetrieveMultiple(new FetchExpression(fetchXml));
                if (collection != null && collection.Entities.Count() > 0)
                {
                    return (true);
                }
            }
            catch (FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> ex)
            {
                logger.error("Code: " + ex.Detail.ErrorCode);
                logger.error("Message: " + ex.Detail.Message);
                logger.error("Trace: " + ex.Detail.TraceText);
                logger.error("Inner Fault: " + ex.Detail.InnerFault);
            }
            catch (Exception e)
            {
                logger.error("Code: " + e.Message);
                logger.error("Message: " + e.StackTrace);
            }
            return (additionalProfiles);

        }
        /// <summary>
        /// Get the total of the kcal targets for the additional profiles
        /// </summary>
        /// <param name="contactId"></param>
        /// <returns></returns>
        public decimal RetrieveAdditionalProfileKcals(Guid contactId, OrganizationServiceProxy crmService)
        {
            decimal kcals = 0m;
            logger.debug("RetrieveAdditionalProfileKcals("+contactId+", crmService): starting");
            try 
            {
                //dc_kcaltarget
                //
                String fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true' aggregate='true'>
                          <entity name='contact'>
                            <attribute name='dc_kcalcalculatedtarget' alias='dc_kcaltarget_sum' aggregate='sum' />
                            <filter type='and'> 
                                <condition attribute='dc_parentcontactid' value='@CONTACTID' operator='eq'/> 
                                <condition attribute='dc_createmenu' value='1' operator='eq'/>
                            </filter>
                          </entity>
                        </fetch>";

                fetchXml = fetchXml.Replace("@CONTACTID", contactId.ToString());

                EntityCollection collection = crmService.RetrieveMultiple(new FetchExpression(fetchXml));
                if (collection != null && collection.Entities.Count() > 0)
                {
                    Entity entity = collection.Entities[0];
                    kcals = (decimal) (((AliasedValue)entity["dc_kcaltarget_sum"]).Value);
                    logger.debug("kcals: " + kcals);
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                logger.error("Code: " + ex.Detail.ErrorCode);
                logger.error("Message: " + ex.Detail.Message);
                logger.error("Trace: " + ex.Detail.TraceText);
                logger.error("Inner Fault: " + ex.Detail.InnerFault);
            }
            catch (Exception e)
            {
                logger.error("Code: " + e.Message);
                logger.error("Message: " + e.StackTrace);
            }
            return (kcals);
        }

        /// <summary>
        /// Get the total of the kcal targets for the additional profiles
        /// </summary>
        /// <param name="contactId"></param>
        /// <returns></returns>
        public Dictionary<Guid, decimal> RetrieveAdditionalProfilePrecents(Guid contactId, OrganizationServiceProxy crmService, decimal kcalTotal)
        {
            decimal kcals                   = 0m;
            decimal additionaTotal          = 0m;
            Dictionary<Guid, decimal> list  = new Dictionary<Guid, decimal>();

            logger.debug("RetrieveAdditionalProfilePrecents(" + contactId + ", crmService, " + kcalTotal + "): starting");
            try
            {
                //dc_kcaltarget
                //
                String fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
                          <entity name='contact'>
                            <attribute name='dc_kcalcalculatedtarget' />
                            <attribute name='contactid' />
                            <filter type='and'> 
                                <condition attribute='dc_parentcontactid' value='@CONTACTID' operator='eq'/> 
                            </filter>
                          </entity>
                        </fetch>";

                fetchXml = fetchXml.Replace("@CONTACTID", contactId.ToString());

                EntityCollection collection = crmService.RetrieveMultiple(new FetchExpression(fetchXml));
                //if (collection != null && collection.Entities.Count() > 0)
                foreach(Entity entity in collection.Entities) 
                {
                   
                    kcals = (decimal)entity["dc_kcalcalculatedtarget"];
                    Guid Id = entity.Id;
                    logger.debug("kcals: " + kcals);
                    additionaTotal += kcals;
                    if (!list.ContainsKey(Id))
                    {
                        list.Add(Id, kcals / kcalTotal);
                        logger.debug("Adding: " + Id + " : " + kcals / kcalTotal);
                    }
                }
                //Deal with primary
                list.Add(contactId, ((kcalTotal - additionaTotal) / kcalTotal) );
               
                
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                logger.error("Code: " + ex.Detail.ErrorCode);
                logger.error("Message: " + ex.Detail.Message);
                logger.error("Trace: " + ex.Detail.TraceText);
                logger.error("Inner Fault: " + ex.Detail.InnerFault);
            }
            catch (Exception e)
            {
                logger.error("Code: " + e.Message);
                logger.error("Message: " + e.StackTrace);
            }
            return (list);
        }
        /// <summary>
        /// Splits the menu up pased on the portionSizeMultipler
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="portionSizeMultiplier"></param>
        public void AdjustPortionSize(Entity parent, decimal portionSizeMultiplier)
        {
            //relationships
            foreach (KeyValuePair<Relationship, EntityCollection> pair in parent.RelatedEntities)
            {
                
                EntityCollection collection = new EntityCollection();
                foreach (Entity e in ((EntityCollection)pair.Value).Entities)
                {
                    if (e.LogicalName.Equals("dc_mealfood", StringComparison.OrdinalIgnoreCase))
                    {
                        e["dc_portionsize"]     = (decimal)e["dc_portionsize"] * portionSizeMultiplier;
                        e["dc_fat"]             = (decimal)e["dc_fat"] * portionSizeMultiplier;
                        e["dc_protein"]         = (decimal)e["dc_protein"] * portionSizeMultiplier;
                        e["dc_carbohydrate"]    = (decimal)e["dc_carbohydrate"] * portionSizeMultiplier;
                        e["dc_alcohol"]         = (decimal)e["dc_alcohol"] * portionSizeMultiplier;
                        e["dc_kcals"]           = General.CalculateKcals((decimal)e["dc_carbohydrate"], (decimal)e["dc_fat"], (decimal)e["dc_protein"], (decimal)e["dc_alcohol"]);
                        
                    }
                    else
                    {
                        AdjustPortionSize(e, portionSizeMultiplier);
                    }
                }
            }
        }
    }
}
