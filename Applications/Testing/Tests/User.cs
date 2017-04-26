using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DynamicConnections.CRM2011.Common.Utility;
using DynamicConnections.NutriStyle.CRM2011.Testing;
using Microsoft.Xrm.Sdk;
using System.ServiceModel;
using DynamicConnections.NutriStyle.CRM2011.MenuGenerator;

namespace DynamicConnections.NutriStyle.CRM2011.Testing.Tests
{
    public class User : General
    {

        Logger logger;
        private Guid contactId;

        public User()
        {
            logger = GetLogger();
        }
        public Guid ContactId {

            get { return (contactId); }

        }
        public bool CreateUser()
        {
            bool success = true;
            Entity contact = new Entity("contact");
            contact["firstname"] = "caleb";
            contact["lastname"] = "skinner";
            contact["emailaddress1"] = "caleb@test.com";
            contact["dc_password"] = "test";
            contact["address1_stateorprovince"] = "OR";
            contact["address1_postalcode"] = "97229";
            contact["dc_morningsnack"] = true;
            contact["dc_menupresetid"] = new EntityReference("dc_menu", new Guid("07ADE8E6-CE16-E111-A860-00155D0A0205")); //Heart Healthy
            contact["dc_maintaintargetweight"] = true;
            contact["dc_activitylevel"] = new OptionSetValue(948170001);//Lightly Active
            contact["dc_countryid"] = new EntityReference("dc_country", new Guid("79AA100A-3CA5-E111-8777-00155D0A0C06"));
            contact["dc_grocerprimaryid"] = new EntityReference("dc_grocer", new Guid("6B3C82E0-F209-E111-9777-00155D0A0205"));
            contact["gendercode"] = new OptionSetValue(1);//male
            contact["dc_userspecifiedkcaltarget"] = true; //yes
            contact["dc_kcaltarget"] = Convert.ToDecimal(2200);
            try
            {
                contactId = GetCrmService().Create(contact);
                logger.debug("Created contact: " + contactId);
            }
            catch (FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> ex)
            {
                logger.error("Code: " + ex.Detail.ErrorCode);
                logger.error("Message: " + ex.Detail.Message);
                logger.error("Trace: " + ex.Detail.TraceText);
                logger.error("Inner Fault: " + ex.Detail.InnerFault);
                success = false;
            }
            return (success);
        }
        /// <summary>
        /// Will trigger ContactPreUpdate_Age
        /// </summary>
        /// <returns></returns>
        public bool SetBirthdate()
        {

            bool success = true;
            try
            {
                logger.debug("Testing ContactPreUpdate_Age plugin");
                Entity contact = new Entity("contact");
                contact["contactid"] = contactId;
                contact["birthdate"] = new DateTime(1976, 8, 23);

                GetCrmService().Update(contact);
                //Retrieve contact to see cm
                contact = GetCrmService().Retrieve("contact", contactId, new Microsoft.Xrm.Sdk.Query.ColumnSet(new String[] { "dc_age" }));

                if (contact.Contains("dc_age"))
                {
                    logger.debug("dc_age: " + (int)contact["dc_age"]);
                }


                logger.debug("Updated birthdate contact: " + contactId);
            }
            catch (FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> ex)
            {
                logger.error("Code: " + ex.Detail.ErrorCode);
                logger.error("Message: " + ex.Detail.Message);
                logger.error("Trace: " + ex.Detail.TraceText);
                logger.error("Inner Fault: " + ex.Detail.InnerFault);
                success = false;
            }
            return (success);
        }
        /// <summary>
        /// Will fire ContactPreUpdate_KG plugin
        /// </summary>
        /// <returns></returns>
        public bool SetWeight()
        {
            bool success = true;
            try
            {
                logger.debug("Testing ContactPreUpdate_KG plugin");
                Entity contact = new Entity("contact");
                contact["contactid"] = contactId;
                contact["dc_currentweight"] = 170;

                GetCrmService().Update(contact);
                //Retrieve contact to see cm
                contact = GetCrmService().Retrieve("contact", contactId, new Microsoft.Xrm.Sdk.Query.ColumnSet(new String[] { "dc_weightkg" }));

                if (contact.Contains("dc_weightkg"))
                {
                    logger.debug("dc_weightkg: " + (decimal)contact["dc_weightkg"]);
                }


                logger.debug("Updated current weight contact: " + contactId);
            }
            catch (FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> ex)
            {
                logger.error("Code: " + ex.Detail.ErrorCode);
                logger.error("Message: " + ex.Detail.Message);
                logger.error("Trace: " + ex.Detail.TraceText);
                logger.error("Inner Fault: " + ex.Detail.InnerFault);
                success = false;
            }
            return (success);
        }

        /// <summary>
        /// Will fire CalculateREE plugin
        /// </summary>
        /// <returns></returns>
        public bool SetREE()
        {
            bool success = true;
            try
            {
                logger.debug("Testing ContactPreUpdate_KG plugin");
                Entity contact = new Entity("contact");
                contact["contactid"] = contactId;
                contact["gendercode"] = new OptionSetValue(1);//male
                contact["dc_age"] = 35;
                contact["dc_heightcm"] = 195m;
                contact["dc_weightkg"] = 77.2m;

                GetCrmService().Update(contact);
                //Retrieve contact to see cm
                contact = GetCrmService().Retrieve("contact", contactId, new Microsoft.Xrm.Sdk.Query.ColumnSet(new String[] { "dc_ree" }));

                if (contact.Contains("dc_ree"))
                {
                    logger.debug("dc_ree: " + (decimal)contact["dc_ree"]);
                }


                logger.debug("Updated dc_ree contact: " + contactId);
            }
            catch (FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> ex)
            {
                logger.error("Code: " + ex.Detail.ErrorCode);
                logger.error("Message: " + ex.Detail.Message);
                logger.error("Trace: " + ex.Detail.TraceText);
                logger.error("Inner Fault: " + ex.Detail.InnerFault);
                success = false;
            }
            return (success);
        }

        /// <summary>
        /// Will fire ContactPreUpdate_DEE plugin
        /// </summary>
        /// <returns></returns>
        public bool SetDEE()
        {
            bool success = true;
            try
            {
                logger.debug("Testing ContactPreUpdate_KG plugin");
                Entity contact = new Entity("contact");
                contact["contactid"] = contactId;
                contact["dc_restinghours"] = 20m;
                contact["dc_verylighthours"] = 1m;
                contact["dc_lighthours"] = 1m;
                contact["dc_moderatehours"] = 1m;
                contact["dc_heavyhours"] = 1m;
                contact["dc_ree"] = 1856m;

                GetCrmService().Update(contact);
                //Retrieve contact to see cm
                contact = GetCrmService().Retrieve("contact", contactId, new Microsoft.Xrm.Sdk.Query.ColumnSet(new String[] { "dc_dee" }));

                if (contact.Contains("dc_dee"))
                {
                    logger.debug("dc_dee: " + (decimal)contact["dc_dee"]);
                }


                logger.debug("Updated dc_ree contact: " + contactId);
            }
            catch (FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> ex)
            {
                logger.error("Code: " + ex.Detail.ErrorCode);
                logger.error("Message: " + ex.Detail.Message);
                logger.error("Trace: " + ex.Detail.TraceText);
                logger.error("Inner Fault: " + ex.Detail.InnerFault);
                success = false;
            }
            return (success);
        }

        /// <summary>
        /// Will trigger ContactPreUpdate_FTCM
        /// </summary>
        /// <returns></returns>
        public bool SetHeight()
        {
            bool success = true;
            //if(contact.Attributes.Contains("dc_heightfeet") || contact.Attributes.Contains("dc_heightinches"))
            try
            {
                logger.debug("Testing ContactPreUpdate_FTCM plugin");
                Entity contact = new Entity("contact");
                contact["contactid"] = contactId;
                contact["dc_heightfeet"] = new OptionSetValue(948170001);
                contact["dc_heightinches"] = new OptionSetValue(948170003);

                GetCrmService().Update(contact);
                //Retrieve contact to see cm
                contact = GetCrmService().Retrieve("contact", contactId, new Microsoft.Xrm.Sdk.Query.ColumnSet(new String[] { "dc_heightcm" }));

                if (contact.Contains("dc_heightcm"))
                {
                    logger.debug("dc_heightcm: " + (decimal)contact["dc_heightcm"]);
                }


                logger.debug("Updated height contact: " + contactId);
            }
            catch (FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> ex)
            {
                logger.error("Code: " + ex.Detail.ErrorCode);
                logger.error("Message: " + ex.Detail.Message);
                logger.error("Trace: " + ex.Detail.TraceText);
                logger.error("Inner Fault: " + ex.Detail.InnerFault);
                success = false;
            }

            return (success);

        }
        /// <summary>
        /// Delete created user
        /// </summary>
        /// <returns></returns>
        public bool DeleteUser()
        {
            bool success = true;

            try
            {
                GetCrmService().Delete("contact", contactId);
                logger.debug("Deleted contact: " + contactId);
            }
            catch (FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> ex)
            {
                logger.error("Code: " + ex.Detail.ErrorCode);
                logger.error("Message: " + ex.Detail.Message);
                logger.error("Trace: " + ex.Detail.TraceText);
                logger.error("Inner Fault: " + ex.Detail.InnerFault);
                success = false;
            }

            return (success);
        }

        public bool AddFavorite()
        {
            bool success = true;

            try
            {
                //Create favorite
                Entity foodLike = new Entity("dc_foodlike");
                foodLike["dc_day"] = new OptionSetValue(948170001);//Monday
                foodLike["dc_meal"] = new OptionSetValue(948170002);//Lunch
                foodLike["dc_contactid"] = new EntityReference("contact", contactId);
                foodLike["dc_foodid"] = new EntityReference("dc_foods", new Guid("5D715612-0201-E111-8F4E-00155D0A0205"));

                GetCrmService().Create(foodLike);
                logger.debug("Created food like");

            }
            catch (FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> ex)
            {
                logger.error("Code: " + ex.Detail.ErrorCode);
                logger.error("Message: " + ex.Detail.Message);
                logger.error("Trace: " + ex.Detail.TraceText);
                logger.error("Inner Fault: " + ex.Detail.InnerFault);
                success = false;
            }

            return (success);
        }
        public bool AddFoodDislike()
        {
            bool success = true;

            try
            {
                //Create favorite
                Entity foodLike = new Entity("dc_fooddislike");
                foodLike["dc_contactid"] = new EntityReference("contact", contactId);
                foodLike["dc_foodid"] = new EntityReference("dc_foods", new Guid("5D715612-0201-E111-8F4E-00155D0A0205"));

                GetCrmService().Create(foodLike);
                logger.debug("Created food dislike");

            }
            catch (FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> ex)
            {
                logger.error("Code: " + ex.Detail.ErrorCode);
                logger.error("Message: " + ex.Detail.Message);
                logger.error("Trace: " + ex.Detail.TraceText);
                logger.error("Inner Fault: " + ex.Detail.InnerFault);
                success = false;
            }
            return (success);
        }
        public bool GenerateMenu()
        {
            bool success = true;

            try
            {
                //retrieve presets
                int kcalBase = 1800;
                EntityCollection presets = CrmHelper.GetEntitiesByAttribute("dc_presets", "statecode", 0, new String[] { "dc_presetsid", "dc_name" }, null, GetCrmService());
                BuildMenu bm = new BuildMenu(GetCrmService());

                foreach (Entity preset in presets.Entities)
                {
                    logger.debug("Preset name: " + (String)preset["dc_name"]);
                    Entity contact = new Entity("contact");
                    contact.Id = contactId;
                    contact["dc_menupresetid"] = new EntityReference("dc_presets", preset.Id);
                    GetCrmService().Update(contact);

                    for (int x = 0; x < 2; x++)
                    {
                        logger.debug("Kcal target: " + Convert.ToDecimal(kcalBase + (x * 100)));
                        contact = new Entity("contact");
                        contact.Id = contactId;
                        contact["dc_userspecifiedkcaltarget"] = true; //yes
                        contact["dc_kcaltarget"] = Convert.ToDecimal(kcalBase + (x * 100));
                        GetCrmService().Update(contact);
                        bm.Build(contactId);
                    }
                }
            }
            catch (FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> ex)
            {
                logger.error("Code: " + ex.Detail.ErrorCode);
                logger.error("Message: " + ex.Detail.Message);
                logger.error("Trace: " + ex.Detail.TraceText);
                logger.error("Inner Fault: " + ex.Detail.InnerFault);
                success = false;
            }
            return (success);
        }
        public bool AddFitnessLog()
        {
            bool success = true;

            try
            {
                Guid activityId = Guid.Empty;

                EntityCollection ec = CrmHelper.GetEntitiesByAttribute("dc_physicalactivity", "dc_name", "aerobic, general", new String[] {"dc_physicalactivityid"}, null, GetCrmService());

                if(ec != null && ec.Entities.Count > 0) 
                {
                    activityId = ((Entity)ec.Entities[0]).Id;

                }
                if(activityId != Guid.Empty) {
                    Guid fitnessLogId = Guid.Empty;
                    for (int x = 0; x < 3; x++)
                    {

                        Entity fitnessLog = new Entity("dc_fitnesslog");
                        //Relate to contact
                        fitnessLog["dc_contactid"] = new EntityReference("contact", contactId);
                        //date
                        fitnessLog["dc_date"] = DateTime.Now;
                        //duration
                        fitnessLog["dc_durationminutes"] = 30m;
                        //activityId
                        fitnessLog["dc_physicalactivityid"] = new EntityReference("dc_physicalactivity", activityId);
                        fitnessLogId = GetCrmService().Create(fitnessLog);
                    }
                    //update duration
                    Entity fitnessLogUpdate = new Entity("dc_fitnesslog");
                    fitnessLogUpdate["dc_durationminutes"] = 45m;
                    fitnessLogUpdate["dc_fitnesslogid"] = fitnessLogId;
                    GetCrmService().Update(fitnessLogUpdate);

                }
            }
            catch (FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> ex)
            {
                logger.error("Code: " + ex.Detail.ErrorCode);
                logger.error("Message: " + ex.Detail.Message);
                logger.error("Trace: " + ex.Detail.TraceText);
                logger.error("Inner Fault: " + ex.Detail.InnerFault);
                success = false;
            }
            return (success);
        }

    }
}
