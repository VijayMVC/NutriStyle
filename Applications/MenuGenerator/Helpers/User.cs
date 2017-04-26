﻿using DynamicConnections.CRM2011.Common.Utility;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Xml;

namespace DynamicConnections.NutriStyle.CRM2011.MenuGenerator.Helpers 
{
    class User : General
    {
        private static Logger logger = GetLogger();

        public User()
        {

        }
        /// <summary>
        /// Generates the default activity level for the profile.  Creates the fitness log day for each day of the week
        /// </summary>
        /// <param name="contactId"></param>
        /// <param name="menuId"></param>
        /// <returns></returns>
        public XmlDocument GenerateDefaultActivityLevel(Guid contactId, Guid menuId, OrganizationServiceProxy crmService)
        {
            logger.debug("GenerateDefaultActivityLevel: " + contactId + ":" + menuId);
            XmlDocument results     = new XmlDocument();
            DateTime currentDate    = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            //Find sunday
            currentDate = currentDate.AddDays(-(int)currentDate.DayOfWeek);

            try
            {
                //retrieve contact
                Entity contact = crmService.Retrieve("contact", contactId, new ColumnSet(new String[] { "dc_activitylevel" }));
                OptionSetValue osv = (OptionSetValue)contact["dc_activitylevel"];
                EntityMetadata em = MetadataHelper.GetEntityMetadata("contact", crmService);
                String activityLevelName = MetadataHelper.RetrieveOptionSetValueString(em, "dc_activitylevel", osv.Value);

                //Create fitnesslogday for each day of the week
                for (int x = 0; x < 7; x++)
                {
                    Entity fitnessLogDay                = new Entity("dc_fitnesslogday");
                    //set name
                    fitnessLogDay["dc_name"]            = currentDate.AddDays(x).ToString("MM/dd/yyyy") + " - " + activityLevelName;
                    //relate to contact
                    fitnessLogDay["dc_contactid"]       = new EntityReference("contact", contactId);
                    //relate to menu
                    fitnessLogDay["dc_menuid"]          = new EntityReference("dc_menu", menuId);
                    //detailed logging
                    fitnessLogDay["dc_detailedlogging"] = false;
                    //Set date
                    fitnessLogDay["dc_date"]            = currentDate.AddDays(x);
                    //kcals: are generated by a plugin: see FitnessLogDayPreUdpate
                    //activity level:  From contact profile
                    fitnessLogDay["dc_activitylevel"]   = contact["dc_activitylevel"];

                    crmService.Create(fitnessLogDay);
                }
            }
            catch (FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> ex)
            {
                logger.error("Code: " + ex.Detail.ErrorCode);
                logger.error("Message: " + ex.Detail.Message);
                logger.error("Message: " + ex.Message);
                logger.error("Trace: " + ex.Detail.TraceText);
                logger.error("Inner Fault: " + ex.Detail.InnerFault);
            }
            catch (Exception e)
            {
                logger.error(e.Message);
                logger.error(e.StackTrace);
            }
            return (results);
        }
    }
}
