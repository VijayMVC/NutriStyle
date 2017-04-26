using System;
using System.Collections.Generic;

using Microsoft.Xrm.Sdk;
using DynamicConnections.CRM2011.CustomAssemblies.Utilities;


namespace DynamicConnections.NutriStyle.CRM2011.Plugins
{
    public class ContactPreUpdate_FTCM : IPlugin
    {
        Tools tools = new Tools("nutristyle_plugins_output");
        Logger logger;
        public decimal ConvertFeetToCM(int valueFeet)
        {

            decimal corvertNumber = (decimal)30.48;
            decimal valueCM = (decimal)0.0;
            try
            {
                valueCM = (decimal)valueFeet * corvertNumber;
                return (valueCM);
            }
            catch (Exception ex)
            {
                logger.debug("ConvertFootToCM Method");
                logger.debug(ex.Message);
                logger.debug(ex.StackTrace);
                return (valueCM);
            }
        }

        public decimal ConvertInchesToCM(int valueInches)
        {
            decimal corvertNumber = (decimal)2.54;
            decimal valueCM = (decimal)0.0;
            try
            {
                valueCM = (decimal)valueInches * corvertNumber;
                return (valueCM);
            }
            catch (Exception ex)
            {
                logger.debug("ConvertInchToCM Method");
                logger.debug(ex.Message);
                logger.debug(ex.StackTrace);
                return (valueCM);
            }
        }

        public void Execute(IServiceProvider serviceProvider)
        {
            logger = new Logger("DEBUG", @"c:\windows\temp\nutristyle_plugins_output.txt", 1);
            
                // get the execution context from the service provider
                IPluginExecutionContext pluginExecutionContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

                try
                {
                logger.debug("ContactPreUpdate_FTCM: starting: " + pluginExecutionContext.PrimaryEntityName+":"+pluginExecutionContext.MessageName);
                
                // initilize the variables 
                int heightFeet = 0;
                int heightInches = 0;

                int heightFeetValue = 0;
                int heightInchesValue = 0;
                
                //Form.  Data that the user changed
                Entity contact = (Entity)pluginExecutionContext.InputParameters["Target"];

                //RE Image.  Data before the user changed it
                Entity preContact = new Entity();
                if (pluginExecutionContext.PreEntityImages.Contains("preimage"))
                {
                    preContact = (Entity)pluginExecutionContext.PreEntityImages["preimage"];
                }

                if(contact.Attributes.Contains("dc_heightfeet") || contact.Attributes.Contains("dc_heightinches"))
                {
                    //Something has changed get values from form or preimage
                    if(contact.Attributes.Contains("dc_heightfeet")) 
                    {
                        heightFeetValue = ((OptionSetValue)contact["dc_heightfeet"]).Value;
                    }
                    else if (preContact.Attributes.Contains("dc_heightfeet")) 
                    {//get from pre image
                        heightFeetValue = ((OptionSetValue)preContact["dc_heightfeet"]).Value;
                    }

                    //Something has changed get values from form or preimage
                    if (contact.Attributes.Contains("dc_heightinches"))
                    {
                        heightInchesValue = ((OptionSetValue)contact["dc_heightinches"]).Value;
                    } 
                    else if (preContact.Attributes.Contains("dc_heightinches"))
                    {//get from pre image
                        heightInchesValue = ((OptionSetValue)preContact["dc_heightinches"]).Value;
                    }                    
                }

                logger.debug("heightFeetValue: "+heightFeetValue);
                logger.debug("heightInchesValue: " + heightInchesValue);

                if(heightInchesValue > 0 && heightFeetValue > 0) {
                    
                    //Feeds
                    if(heightFeetValue == 948170002) { //4 feet
                        heightFeet = 4;
                    }
                    else if (heightFeetValue == 948170000) 
                    { //5 feet
                        heightFeet = 5;
                    }
                    else if (heightFeetValue == 948170001)
                    { //6 feet
                        heightFeet = 6;
                    }
                    else if (heightFeetValue == 948170003)
                    { //7 feet
                        heightFeet = 7;
                    }
                    else if (heightFeetValue == 948170005)
                    { //2 feet
                        heightFeet = 2;
                    }
                    else if (heightFeetValue == 948170004)
                    { //3 feet
                        heightFeet = 3;

                    }

                    //Inches
                    if(heightInchesValue == 948170000) { 
                    // 1 n
                        heightInches = 1;
                    }
                    else if (heightInchesValue == 948170001) 
                    { // 2 n
                        heightInches = 2;
                    }
                    else if (heightInchesValue == 948170002) 
                    { // 3 n
                        heightInches = 3;
                    }
                    else if (heightInchesValue == 948170003)
                    { // 4 n
                        heightInches = 4;
                    }
                    else if (heightInchesValue == 948170004)
                    { // 5 n
                        heightInches = 5;
                    }
                    else if (heightInchesValue == 948170005)
                    { // 6 n
                        heightInches = 6;
                    }
                    else if (heightInchesValue == 948170006)
                    { // 7 n
                        heightInches = 7;
                    }
                    else if (heightInchesValue == 948170007)
                    { // 8 n
                        heightInches = 8;
                    }
                    else if (heightInchesValue == 948170008)
                    { // 9 n
                        heightInches = 9;
                    }
                    else if (heightInchesValue == 948170009)
                    { // 10 n
                        heightInches = 10;
                    }
                    else if (heightInchesValue == 948170010)
                    { // 11 n
                        heightInches = 11;
                    }
                    else if (heightInchesValue == 948170011)
                    { // 0 n
                        heightInches = 0;
                    }
                }

                logger.debug("heightFeet: " + heightFeet);
                logger.debug("heightInches: " + heightInches);

                if (heightFeet > 0 && heightInches >= 0)
                {
                   // get the feet in CM
                   decimal valueFeetToCM = ConvertFeetToCM(heightFeet);
                   logger.debug("FeetToCM: " + valueFeetToCM);

                   // calculate  the inches in CM
                   decimal valueInchesToCM = ConvertInchesToCM(heightInches);
                   logger.debug("InchesToCM: " + valueInchesToCM);

                   // get total CM
                   decimal totalCM = valueFeetToCM + valueInchesToCM;

                   // set the field value dc_heightcm to totalCM
                   contact["dc_heightcm"] = totalCM;
                   logger.debug("Total Feet + Inches: " + totalCM);
                }
            }
            catch (Exception ex)
            {
                logger.error("Execute Method: " + pluginExecutionContext.PrimaryEntityName + ":" + pluginExecutionContext.MessageName);
                logger.error(ex.Message);
                logger.error(ex.StackTrace);
            }

        }
    }
}
