using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DynamicConnections.CRM2011.Common.Utility;
using DynamicConnections.NutriStyle.CRM2011.Testing;
using Microsoft.Xrm.Sdk;
using System.ServiceModel;
using DynamicConnections.NutriStyle.CRM2011.MenuGenerator;
using System.Xml.Schema;
using System.Xml;
using System.IO;
using System.Xml.Linq;

namespace DynamicConnections.NutriStyle.CRM2011.Testing.Tests
{
    public class WebService : General
    {

        Logger logger;
        private Guid contactId;

        public WebService()
        {
            logger = GetLogger();
        }
        public Guid ContactId {
            get {return (contactId); }
            set {contactId = value;}
        }
        public bool AddFoodDislike()
        {
            bool success = true;

            try
            {
                String foodDislikeXml = @"<dc_fooddislike xmlns='urn:dc_fooddislike-schema'>
                        <dc_contactid entityname='contact'>@CONTACTID</dc_contactid>
                        <dc_mealcomponentid entityname='dc_meal_component'>b0eff466-46b0-e111-9dd8-00155d0a0c06</dc_mealcomponentid>
                    </dc_fooddislike>";
                //<dc_mealcomponentid entityname='dc_meal_component'>b0eff466-46b0-e111-9dd8-00155d0a0c06</dc_mealcomponentid>

                foodDislikeXml = foodDislikeXml.Replace("@CONTACTID", contactId.ToString());

                String foodDislikeXSD = @"<xsd:schema xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns='urn:dc_fooddislike-schema' elementFormDefault='qualified' targetNamespace='urn:dc_fooddislike-schema'>
                 
                    <xsd:complexType name='relationshipType'>
                        <xsd:simpleContent>
                            <xsd:extension base='xsd:string' >
                                <xsd:attribute name='entityname' type='xsd:string' use='required'/> 
                            </xsd:extension>
                        </xsd:simpleContent>
                    </xsd:complexType>
                    
                    <xsd:element name='dc_fooddislike' type='foodDislikeType'/>

                     <xsd:complexType name='foodDislikeType'>
                      <xsd:sequence maxOccurs='1'>
                        <xsd:element name='dc_contactid' type='relationshipType' minOccurs='1'/>
                        <xsd:choice>
                            <xsd:element name='dc_mealcomponentid' type='relationshipType'/>
                            <xsd:element name='dc_foodid' type='relationshipType'/>
                        </xsd:choice>
                      </xsd:sequence>
                     </xsd:complexType>
                </xsd:schema>";

                TextReader tr = new StringReader(foodDislikeXSD);
                TextReader booksReader = new StringReader(foodDislikeXml);

                XmlReader schemaReader = XmlReader.Create(tr);

                XmlSchemaSet schemaSet = new XmlSchemaSet();
                schemaSet.Add("urn:dc_fooddislike-schema", schemaReader);

                Console.WriteLine();
                Console.WriteLine("\r\nValidating XML file");

                XmlSchema compiledSchema = null;

                foreach (XmlSchema schema in schemaSet.Schemas())
                {
                    compiledSchema = schema;
                }

                XmlReaderSettings settings = new XmlReaderSettings();
                settings.Schemas.Add(compiledSchema);
                settings.ValidationEventHandler += new ValidationEventHandler(ValidationCallBack);
                settings.ValidationType = ValidationType.Schema;

                //Create the schema validating reader.
                XmlReader vreader = XmlReader.Create(booksReader, settings);

                while (vreader.Read())
                {
                    if (vreader.NodeType == XmlNodeType.Text || vreader.NodeType == XmlNodeType.CDATA)
                    {

                        String lastValue = vreader.Value;
                        if (lastValue.Contains("Validation error"))
                        {
                            Console.WriteLine("lastValue: " + lastValue);
                            success = false;
                            return (success);
                        }
                    }
                }
                //Close the reader.
                vreader.Close();

                //now update contact
                XElement foodDislikeXmlDoc = XElement.Parse(foodDislikeXml);

                NSWS.WebServicesSoapClient wsc = new NSWS.WebServicesSoapClient();
                wsc.CreateUpdate(foodDislikeXmlDoc);
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


        public bool AddFoodLike()
        {
            bool success = true;
            try
            {
                String foodLikeXml = @"<entities xmlns='urn:foodlikeEntities-schema'>
                    <dc_foodlike>
                        <dc_contactid entityname='contact'>@CONTACTID</dc_contactid>
                        <dc_day>948170004</dc_day>
                        <dc_meal>948170005</dc_meal>
                        <dc_foodid entityname='dc_foods'>d5b24db5-5e74-e111-bb4f-00155d0a0c06</dc_foodid>
                    </dc_foodlike>
                   
                </entities>";

                foodLikeXml = foodLikeXml.Replace("@CONTACTID", contactId.ToString());

                String foodLikeXSD = @"<xsd:schema xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns='urn:foodlikeEntities-schema' elementFormDefault='qualified' targetNamespace='urn:foodlikeEntities-schema'>
                    
                    <xsd:complexType name='relationshipType'>
                        <xsd:simpleContent>
                            <xsd:extension base='xsd:string' >
                                <xsd:attribute name='entityname' type='xsd:string' use='required'/> 
                            </xsd:extension>
                        </xsd:simpleContent>
                    </xsd:complexType>

                    <xsd:complexType name='foodlikeType'>
                        <xsd:sequence>
                            <xsd:element name='dc_contactid' type='relationshipType' minOccurs='1'/>
                            <xsd:element name='dc_day' type='xsd:integer' minOccurs='1'/>
                            <xsd:element name='dc_meal' type='xsd:integer' minOccurs='1'/>
                            <xsd:element name='dc_foodid' type='relationshipType' minOccurs='1'/>
                        </xsd:sequence>
                    </xsd:complexType>
                 
                    <xsd:complexType name='foodlikeEntitiesType'>   
                        <xsd:sequence maxOccurs='10'>
                            <xsd:element name='dc_foodlike' type='foodlikeType'/>
                        </xsd:sequence> 
                    </xsd:complexType>
                    
                    <xsd:element name='entities' type='foodlikeEntitiesType'/>
                    
                </xsd:schema>";

                TextReader tr = new StringReader(foodLikeXSD);
                TextReader booksReader = new StringReader(foodLikeXml);

                XmlReader schemaReader = XmlReader.Create(tr);

                XmlSchemaSet schemaSet = new XmlSchemaSet();
                schemaSet.Add("urn:foodlikeEntities-schema", schemaReader);

                Console.WriteLine();
                Console.WriteLine("Validating XML file");

                XmlSchema compiledSchema = null;

                foreach (XmlSchema schema in schemaSet.Schemas())
                {
                    compiledSchema = schema;
                }

                XmlReaderSettings settings = new XmlReaderSettings();
                settings.Schemas.Add(compiledSchema);
                settings.ValidationEventHandler += new ValidationEventHandler(ValidationCallBack);
                settings.ValidationType = ValidationType.Schema;

                //Create the schema validating reader.
                XmlReader vreader = XmlReader.Create(booksReader, settings);

                while (vreader.Read())
                {
                    if (vreader.NodeType == XmlNodeType.Text || vreader.NodeType == XmlNodeType.CDATA)
                    {

                        String lastValue = vreader.Value;
                        if (lastValue.Contains("Validation error"))
                        {
                            Console.WriteLine("lastValue: " + lastValue);
                            success = false;
                            return (success);
                        }
                    }
                }

                //Close the reader.
                vreader.Close();

                //now create foodlike
                XElement contactXmlDoc = XElement.Parse(foodLikeXml);

                NSWS.WebServicesSoapClient wsc = new NSWS.WebServicesSoapClient();
                wsc.CreateUpdateReturnEntities(contactXmlDoc, false);

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

        public bool UpdateMenuOptions()
        {
            bool success = true;

            try
            {
                String contactXml = @"<contact xmlns='urn:contact-schema'>
                        <contactid>@CONTACTID</contactid>
                        <dc_kcaltarget>2450.05</dc_kcaltarget>
                        <dc_userspecifiedkcaltarget>True</dc_userspecifiedkcaltarget>
                        <dc_morningsnack>True</dc_morningsnack>
                        <dc_afternoonsnack>False</dc_afternoonsnack>
                        <dc_eveningsnack>False</dc_eveningsnack>
                        <dc_menupresetid entityname='dc_presets'>ee70ca06-0900-e111-ba65-00155d0a0205</dc_menupresetid>
                    </contact>";

                contactXml = contactXml.Replace("@CONTACTID", contactId.ToString());

                String contactXSD = @"<xsd:schema xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns='urn:contact-schema' elementFormDefault='qualified' targetNamespace='urn:contact-schema'>
                 <xsd:complexType name='relationshipType'>
                          <xsd:simpleContent>
                            <xsd:extension base='xsd:string' >
                              <xsd:attribute name='entityname' type='xsd:string' use='required'/> 
                            </xsd:extension>
                          </xsd:simpleContent>
                        </xsd:complexType>

                 <xsd:element name='contact' type='contactType'/>
                 <xsd:complexType name='contactType'>
                  <xsd:sequence maxOccurs='1'>
                    <xsd:element name='contactid' type='xsd:string' minOccurs='1'/>
                    <xsd:element name='dc_heightfeet' type='xsd:string' minOccurs='0'/>
                    <xsd:element name='dc_heightinches' type='xsd:string' minOccurs='0'/>
                    <xsd:element name='birthdate' type='xsd:string' minOccurs='0'/>
                    <xsd:element name='gendercode' type='xsd:string' minOccurs='0'/>
                    <xsd:element name='dc_currentweight' type='xsd:string' minOccurs='0'/>
                    <xsd:element name='dc_activitylevel' type='xsd:string' minOccurs='0'/>
                    <xsd:element name='dc_maintaintargetweight' type='xsd:string' minOccurs='0'/>
                    <xsd:element name='dc_targetweight' type='xsd:string' minOccurs='0'/>
                    <xsd:element name='dc_poundsperweek' type='xsd:string' minOccurs='0'/>
                    <xsd:element name='dc_presetsid' type='xsd:string' minOccurs='0'/>
                    <xsd:element name='dc_kcaltarget' type='xsd:string' minOccurs='0'/>
                    <xsd:element name='dc_userspecifiedkcaltarget' type='xsd:string' minOccurs='0'/>
                    <xsd:element name='dc_morningsnack' type='xsd:string' minOccurs='0'/>
                    <xsd:element name='dc_afternoonsnack' type='xsd:string' minOccurs='0'/>
                    <xsd:element name='dc_eveningsnack' type='xsd:string' minOccurs='0'/>
                    <xsd:element name='dc_menupresetid' type='relationshipType' minOccurs='0'/>
                  </xsd:sequence>
                 </xsd:complexType>
                </xsd:schema>";

                TextReader tr = new StringReader(contactXSD);
                TextReader booksReader = new StringReader(contactXml);

                XmlReader schemaReader = XmlReader.Create(tr);

                XmlSchemaSet schemaSet = new XmlSchemaSet();
                schemaSet.Add("urn:contact-schema", schemaReader);

                Console.WriteLine();
                Console.WriteLine("\r\nValidating XML file");

                XmlSchema compiledSchema = null;

                foreach (XmlSchema schema in schemaSet.Schemas())
                {
                    compiledSchema = schema;
                }

                XmlReaderSettings settings = new XmlReaderSettings();
                settings.Schemas.Add(compiledSchema);
                settings.ValidationEventHandler += new ValidationEventHandler(ValidationCallBack);
                settings.ValidationType = ValidationType.Schema;

                //Create the schema validating reader.
                XmlReader vreader = XmlReader.Create(booksReader, settings);

                while (vreader.Read())
                {
                    if (vreader.NodeType == XmlNodeType.Text || vreader.NodeType == XmlNodeType.CDATA)
                    {

                        String lastValue = vreader.Value;
                        if (lastValue.Contains("Validation error"))
                        {
                            Console.WriteLine("lastValue: " + lastValue);
                            success = false;
                            return (success);
                        }
                    }

                }

                //Close the reader.
                vreader.Close();

                //now update contact
                XElement contactXmlDoc = XElement.Parse(contactXml);

                NSWS.WebServicesSoapClient wsc = new NSWS.WebServicesSoapClient();
                wsc.CreateUpdate(contactXmlDoc);
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

        public bool UpdateContact()
        {
            bool success = true;

            try
            {
                String contactXml = @"<contact xmlns='urn:contact-schema'>
                        <contactid>@CONTACTID</contactid>
                        <dc_heightfeet>948170001</dc_heightfeet>
                        <dc_heightinches>948170001</dc_heightinches>
                        <birthdate>3/4/1993 12:00:00 AM</birthdate>
                        <gendercode>1</gendercode>
                        <dc_currentweight>185</dc_currentweight>
                        <dc_activitylevel>948170001</dc_activitylevel>
                        <dc_maintaintargetweight>True</dc_maintaintargetweight>
                        <dc_targetweight>170</dc_targetweight>
                        <dc_poundsperweek>948170000</dc_poundsperweek>
                    </contact>";

                contactXml = contactXml.Replace("@CONTACTID", contactId.ToString());

                String contactXSD = @"<xsd:schema xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns='urn:contact-schema' elementFormDefault='qualified' targetNamespace='urn:contact-schema'>
                 <xsd:element name='contact' type='contactType'/>
                 <xsd:complexType name='contactType'>
                  <xsd:sequence maxOccurs='1'>
                    <xsd:element name='contactid' type='xsd:string' minOccurs='1'/>
                    <xsd:element name='dc_heightfeet' type='xsd:string' minOccurs='0'/>
                    <xsd:element name='dc_heightinches' type='xsd:string' minOccurs='0'/>
                    <xsd:element name='birthdate' type='xsd:string' minOccurs='0'/>
                    <xsd:element name='gendercode' type='xsd:string' minOccurs='0'/>
                    <xsd:element name='dc_currentweight' type='xsd:string' minOccurs='0'/>
                    <xsd:element name='dc_activitylevel' type='xsd:string' minOccurs='0'/>
                    <xsd:element name='dc_maintaintargetweight' type='xsd:string' minOccurs='0'/>
                    <xsd:element name='dc_targetweight' type='xsd:string' minOccurs='0'/>
                    <xsd:element name='dc_poundsperweek' type='xsd:string' minOccurs='0'/>
                  </xsd:sequence>
                 </xsd:complexType>
                </xsd:schema>";

                TextReader tr           = new StringReader(contactXSD);
                TextReader booksReader  = new StringReader(contactXml);

                XmlReader schemaReader  = XmlReader.Create(tr);

                XmlSchemaSet schemaSet  = new XmlSchemaSet();
                schemaSet.Add("urn:contact-schema", schemaReader);

                Console.WriteLine();
                Console.WriteLine("\r\nValidating XML file");

                XmlSchema compiledSchema = null;

                foreach (XmlSchema schema in schemaSet.Schemas())
                {
                    compiledSchema = schema;
                }

                XmlReaderSettings settings = new XmlReaderSettings();
                settings.Schemas.Add(compiledSchema);
                settings.ValidationEventHandler += new ValidationEventHandler(ValidationCallBack);
                settings.ValidationType = ValidationType.Schema;
                
                //Create the schema validating reader.
                XmlReader vreader = XmlReader.Create(booksReader, settings);
   
                while (vreader.Read() ) 
                {
                    if (vreader.NodeType == XmlNodeType.Text || vreader.NodeType == XmlNodeType.CDATA)
                    {

                        String lastValue = vreader.Value;
                        if (lastValue.Contains("Validation error"))
                        {
                            Console.WriteLine("lastValue: " + lastValue);
                            success = false;
                            return (success);
                        }
                    }
                   
                }

                //Close the reader.
                vreader.Close();

                //now update contact
                XElement contactXmlDoc = XElement.Parse(contactXml);

                NSWS.WebServicesSoapClient wsc = new NSWS.WebServicesSoapClient();
                wsc.CreateUpdate(contactXmlDoc);
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

        //Display any warnings or errors.
        private static void ValidationCallBack(object sender, ValidationEventArgs args)
        {
            if (args.Severity == XmlSeverityType.Warning)
            {
                Console.WriteLine("\tWarning: Matching schema not found.  No validation occurred." + args.Message);
            }
            else 
            {
                Console.WriteLine("\tValidation error: " + args.Message);
            }
        }

        public bool GenerateMenu()
        {
            bool success = true;

            try
            {
                NSWS.WebServicesSoapClient wsc = new NSWS.WebServicesSoapClient();
                wsc.GenerateMenu(contactId.ToString());
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
