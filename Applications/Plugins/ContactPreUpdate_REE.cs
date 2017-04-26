using System;
using System.Collections.Generic;

using Microsoft.Xrm.Sdk;
using DynamicConnections.CRM2011.CustomAssemblies.Utilities;

namespace DynamicConnections.NutriStyle.CRM2011.Plugins
{
    public class ContactPreUpdate_REE : IPlugin
    {
        //Tools tools = new Tools("nutristyle_plugins_output");
        Logger logger;
        public decimal CalculateREE(string genderName, 
                                                   decimal weightKgValue, 
                                                   decimal heightCmValue, 
                                                   int ageValue)
        {
           

            // female coefficients
            decimal coefficientFirstFemale = (decimal)655;
            decimal coefficientWeightKgFemale = (decimal)9.56;
            decimal coefficientHeightCmFemale = (decimal)1.85;
            decimal coefficientAgeFemale = (decimal)4.68;
            // male coefficients
            decimal coefficientFirstMale = (decimal)66.5;
            decimal coefficientWeightKgMale = (decimal)13.75;
            decimal coefficientHeightCmMale = (decimal)5.0;
            decimal coefficientAgeMale = (decimal)6.78;
            // result REE
            decimal REEValue = (decimal)0.0;
            try
            {
                decimal ageDecimalValue = (decimal)ageValue;
                
                if (genderName.Equals("Female", StringComparison.OrdinalIgnoreCase))
                {
                    // ree = 655 + (9.56*weight_kg) + (1.85*height_cm) - (4.68*age);
                    REEValue = coefficientFirstFemale + (coefficientWeightKgFemale * weightKgValue) + (coefficientHeightCmFemale * heightCmValue) - (coefficientAgeFemale * ageDecimalValue);
                }
                else if (genderName.Equals("Male", StringComparison.OrdinalIgnoreCase))
                {
                    // ree = 66.5 + (13.75*weight_kg) + (5.0*height_cm) - (6.78*age);
                    REEValue = coefficientFirstMale + (coefficientWeightKgMale * weightKgValue) + (coefficientHeightCmMale * heightCmValue) - (coefficientAgeMale * ageDecimalValue);
                }
                return (REEValue);
            }
            catch (Exception ex)
            {
                logger.error("ConvertFootToCM Method");
                logger.error(ex.Message);
                logger.error(ex.StackTrace);
                return (REEValue);
            }
        }

        public void Execute(IServiceProvider serviceProvider)
        {
            logger = new Logger("DEBUG", @"c:\windows\temp\nutristyle_plugins_output.txt", 1);
           
                // get the execution context from the service provider
                IPluginExecutionContext pluginExecutionContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
                try
                {
                logger.debug("ContactPreUpdate_REE: starting: " + pluginExecutionContext.PrimaryEntityName +":"+pluginExecutionContext.MessageName);

                // initilize the variables 
                string gendeName = string.Empty;
                int gendeCodeValue = 0;
                decimal weightKgValue = (decimal)0.0;
                decimal heightCmValue = (decimal)0.0;
                int ageValue = 0;
                
                // get the Entity contact
                Entity contact = (Entity)pluginExecutionContext.InputParameters["Target"];

                // get the Entity preContact
                Entity preContact = new Entity();
                if (pluginExecutionContext.PreEntityImages.Contains("preimage"))
                {
                    preContact = (Entity)pluginExecutionContext.PreEntityImages["preimage"];
                }

                logger.debug("Found entities.  Starting gathering of variables");

                if (contact.Attributes.Contains("gendercode") ||
                    contact.Attributes.Contains("dc_weightkg") ||
                    contact.Attributes.Contains("dc_heightcm") ||
                    contact.Attributes.Contains("dc_age"))
                {
                    // gendercode
                    if (contact.Attributes.Contains("gendercode"))
                    {
                        gendeCodeValue = ((OptionSetValue)contact["gendercode"]).Value;    
                    }
                    else if (preContact.Attributes.Contains("gendercode"))
                    {
                        gendeCodeValue = ((OptionSetValue)preContact["gendercode"]).Value;    
                    }
                    logger.debug("gendercode: " + gendeCodeValue);
                    // dc_weightkg
                    if (contact.Attributes.Contains("dc_weightkg"))
                    {
                        weightKgValue = (decimal)contact["dc_weightkg"];
                    }
                    else if (preContact.Attributes.Contains("dc_weightkg"))
                    {
                        weightKgValue = (decimal)preContact["dc_weightkg"];
                    }
                    logger.debug("dc_weightkg: " + weightKgValue);
                    // dc_heightcm
                    if (contact.Attributes.Contains("dc_heightcm"))
                    {
                        heightCmValue = (decimal)contact["dc_heightcm"];
                    }
                    else if (preContact.Attributes.Contains("dc_heightcm"))
                    {
                        heightCmValue = (decimal)preContact["dc_heightcm"];
                    }
                    logger.debug("dc_heightcm: " + heightCmValue);
                    // dc_age
                    if (contact.Attributes.Contains("dc_age"))
                    {
                        ageValue = (int)contact["dc_age"];
                    }
                    else if (preContact  .Attributes.Contains("dc_age"))
                    {
                        ageValue = (int)preContact["dc_age"];
                    }
                }
                logger.debug("dc_age: " + ageValue);
                
               

                if (gendeCodeValue > 0 )
                {
                    if (gendeCodeValue == 1)
                    {
                        gendeName = "Male";
                    }
                    else if (gendeCodeValue == 2)
                    {
                        gendeName = "Female";
                    }

                    logger.debug("gendeCodeValue: " + gendeCodeValue);
                    logger.debug("gendeName: " + gendeName);      

                    // calculate the REE value
                    decimal REEValue = CalculateREE(gendeName, weightKgValue, heightCmValue, ageValue);

                    // set the filed value dc_ree to REEValue
                    contact["dc_ree"] = REEValue;
                    logger.debug("dc_ree: " + REEValue);                                                   
                }                                 
            }
            catch (Exception ex)
            {
                logger.error("Execute Method: "+ pluginExecutionContext.PrimaryEntityName +":"+pluginExecutionContext.MessageName);
                logger.error(ex.Message);
                logger.error(ex.StackTrace);
            }
           }
        }
}