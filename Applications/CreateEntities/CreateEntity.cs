using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using DynamicConnections.CRM2011.Common.Utility;
using Microsoft.Xrm.Sdk.Client;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;
using System.Runtime.Serialization;
using System.Collections;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Messages;

namespace DynamicConnections.NutriStyle.CRM2011.CreateEntities
{
    public partial class CreateEntity : Public
    {       
        OrganizationServiceProxy crmOrganizationServiceProxy;

        public CreateEntity()
        {
            string errorMesage = null;
            // get crm connection parameters
            string user = ConfigurationManager.AppSettings["CrmUser"];
            string password = ConfigurationManager.AppSettings["Password"];
            string orgName = ConfigurationManager.AppSettings["CrmOrganization"];
            string domain = ConfigurationManager.AppSettings["Domain"];
            string host = ConfigurationManager.AppSettings["Hostname"];
            string port = ConfigurationManager.AppSettings["Portnumber"];

            Console.WriteLine("Connecting to Organization Service Proxy...");

            // get the OrganizationServiceProxy object
            crmOrganizationServiceProxy = CrmHelper.CreateOrganizationServiceProxy(user, password, domain, host, orgName, Convert.ToInt32(port), out errorMesage);

            //Console.WriteLine("The Organization Service Proxy has been created.");

            if (errorMesage != null)
            {
                Console.WriteLine("The Organization Service Proxy was not created." + errorMesage);                                            
            }
            else
            {
                Console.WriteLine("The Organization Service Proxy has been created.");               
            }            
        } // end CreateEntity

        // createEntity procedure, passing the the arrrayTable and listColumnMetadata
        public void createEntity(string[] arrayTable, List<Track> listColumnMetadata)
        {
           bool found;
           string errorMessage = null;
           const string tblTable ="tbl";
           const string dc ="dc";

           const string dc_ ="dc_";
           const string s ="s";
           string _tableNameArray;
           string schemaName, displayName, displayCollectionName, descriptionText; 
           EntityMetadata newEnity;

            try
            {              
                 // loop into the tables
                 for (int i = 0; i < arrayTable.Length; i++)
                {                    
                    // get the table name
                    string tableNameArray = arrayTable[i];
                
                    Console.WriteLine("Creating the Create Entity Request...");

                    // add to the tables dc or dc_ and get the schemaName
                    //int foundTbl = tableNameArray.IndexOf(tblTable);
                    if (tableNameArray.IndexOf(tblTable) == 0)
                    {
                        _tableNameArray = tableNameArray.Substring(tblTable.Length, tableNameArray.Length - tblTable.Length);
                        schemaName = dc + _tableNameArray;
                    }
                    else
                    {
                        schemaName = dc_ + tableNameArray;
                    }

                    //get the displayName, displayCollectionName and description
                    displayName = schemaName;                  
                    displayCollectionName = schemaName + s;
                    descriptionText = "Imported from source database";

                    Console.WriteLine("checking if entity exists...");

                    // check if EntityMetadata schemaName exist
                    found = MetadataHelper.IsEntityMetadata(schemaName, crmOrganizationServiceProxy, out errorMessage);

                    if (found == false)
                    {
                        // EntityMetadata not found
                        Console.WriteLine("Creating the Create Entity Request...");

                        // create the EntityMetadata object
                        newEnity = MetadataHelper.CreateEntityMetadata(crmOrganizationServiceProxy, schemaName, displayName, displayCollectionName, descriptionText, out errorMessage);

                        Console.WriteLine("The EntityMetadata has been created!");

                        System.Diagnostics.Debug.WriteLine(tableNameArray + ' ' + schemaName);

                        // check if the EntityMetadata was created
                        if (errorMessage != null)
                        {
                            Console.WriteLine("The EntityMetadata was not created. " + errorMessage);
                        }                        

                        // +++++++++++++++++++++++++++++++++++
                        // add atribute foe that entity
                        // +++++++++++++++++++++++++++++++++++
                        for (int j = 0; j < listColumnMetadata.Count(); j++)
                        {
                            bool isCreated = false;

                            // get the column metadata for each column
                            string tableNameList = listColumnMetadata[j].tableName;
                            string columnNameList = listColumnMetadata[j].columnName;
                            string dataTypeList = listColumnMetadata[j].dataType;
                            string columnSizeList = listColumnMetadata[j].columnSize;
                            
                            string dc_ColumnName = dc_ + columnNameList;
                            
                            int columnSize = Convert.ToInt32(columnSizeList);

                            int intCompare = string.Compare(tableNameArray, tableNameList);
                            if (intCompare == 0)
                            {

                                System.Diagnostics.Debug.WriteLine(tableNameArray + ' ' + schemaName + ' ' + dc_ColumnName);

                                if (dataTypeList.IndexOf("System.String") == 0)
                                {
                                    // String attribute for text data type
                                    if (columnSize == -1)
                                    {
                                        columnSize = 255;
                                    }
                                    // String attribute if one of the fields is name, the code add 1 number!
                                    if (dc_ColumnName == "dc_name")
                                    {
                                        dc_ColumnName += "1";
                                    }

                                    isCreated = MetadataHelper.CreateAttribute(newEnity, dc_ColumnName, dc_ColumnName, columnSize, AttributeTypeCode.String, crmOrganizationServiceProxy);
                                    if (isCreated == false)
                                    {
                                        Console.WriteLine("Unable to create string attribute!");
                                    }
                                }
                                if (dataTypeList.IndexOf("System.Integer") == 0)
                                {
                                    // Integer, Int16 and Int32 attributes
                                    isCreated = MetadataHelper.CreateAttribute(newEnity, dc_ColumnName, dc_ColumnName, 0, AttributeTypeCode.Integer, crmOrganizationServiceProxy);
                                    if (isCreated == false)
                                    {
                                        Console.WriteLine("Unable to create integer attribute!");
                                    }
                                }
                                if (dataTypeList.IndexOf("System.Int16") == 0)
                                {
                                    // Integer, Int16 and Int32 attributes
                                    isCreated = MetadataHelper.CreateAttribute(newEnity, dc_ColumnName, dc_ColumnName, 0, AttributeTypeCode.Integer, crmOrganizationServiceProxy);
                                    if (isCreated == false)
                                    {
                                        Console.WriteLine("Unable to create integer attribute!");
                                    }
                                }   
                                if (dataTypeList.IndexOf("System.Int32") == 0)
                                {
                                    // Integer, Int16 and Int32 attributes
                                    isCreated = MetadataHelper.CreateAttribute(newEnity, dc_ColumnName, dc_ColumnName, 0, AttributeTypeCode.Integer, crmOrganizationServiceProxy);
                                    if (isCreated == false)
                                    {
                                        Console.WriteLine("Unable to create integer attribute!");
                                    }
                                }
                                if (dataTypeList.IndexOf("System.Int64") == 0)
                                {
                                    // BigInt attribute                                   
                                    isCreated = MetadataHelper.CreateAttribute(newEnity, dc_ColumnName, dc_ColumnName, 0, AttributeTypeCode.BigInt, crmOrganizationServiceProxy);
                                    if (isCreated == false)
                                    {
                                        Console.WriteLine("Unable to create bigInt attribute!");
                                    }   
                                }
                                if (dataTypeList.IndexOf("System.Boolean") == 0) // Boolean in SQL Server and Postgre???
                                {
                                    // Boolean attribute                                                            
                                    isCreated = MetadataHelper.CreateAttribute(newEnity, dc_ColumnName, dc_ColumnName, 0, AttributeTypeCode.Boolean, crmOrganizationServiceProxy);
                                    if (isCreated == false)
                                    {
                                        Console.WriteLine("Unable to create Boolean attribute!");
                                    }  
                                }
                                if (dataTypeList.IndexOf("System.DateTime") == 0) 
                                {
                                    // DateTime attribute
                                    isCreated = MetadataHelper.CreateAttribute(newEnity, dc_ColumnName, dc_ColumnName, 0, AttributeTypeCode.DateTime, crmOrganizationServiceProxy);
                                    if (isCreated == false)
                                    {
                                        Console.WriteLine("Unable to create DateTime attribute!");
                                    }  
                                }
                                if (dataTypeList.IndexOf("System.Decimal") == 0)
                                {
                                    // Decimal attribute                                    
                                    isCreated = MetadataHelper.CreateAttribute(newEnity, dc_ColumnName, dc_ColumnName, 0, AttributeTypeCode.Decimal, crmOrganizationServiceProxy);
                                    if (isCreated == false)
                                    {
                                        Console.WriteLine("Unable to create Decimal attribute!");
                                    }  
                                }
                                if (dataTypeList.IndexOf("System.Double") == 0)
                                {
                                    // Double attribute
                                    isCreated = MetadataHelper.CreateAttribute(newEnity, dc_ColumnName, dc_ColumnName, 0, AttributeTypeCode.Double, crmOrganizationServiceProxy);
                                    if (isCreated == false)
                                    {
                                        Console.WriteLine("Unable to create Double attribute!");
                                    }  
                                }
                                //if (dataTypeList.IndexOf("System.Memo") == 0)
                                //{
                                //    // Memo attribute
                                //    isCreated = MetadataHelper.CreateAttribute(newEnity, dc_ColumnName, dc_ColumnName, columnSize, AttributeTypeCode.Memo, crmOrganizationServiceProxy);
                                //    if (isCreated == false)
                                //    {
                                //        Console.WriteLine("Unable to create Memo attribute!");
                                //    }  
                                //}
                                //if (dataTypeList.IndexOf("System.Money") == 0)
                                //{
                                //    // Money attribute
                                //    isCreated = MetadataHelper.CreateAttribute(newEnity, dc_ColumnName, dc_ColumnName, 0, AttributeTypeCode.Money, crmOrganizationServiceProxy);
                                //    if (isCreated == false)
                                //    {
                                //        Console.WriteLine("Unable to create Money attribute!");
                                //    } 
                                //}
                                //if (dataTypeList.IndexOf("System.Picklist") == 0)
                                //{
                                //    // Picklist attribute
                                //    isCreated = MetadataHelper.CreateAttribute(newEnity, dc_ColumnName, dc_ColumnName, 0, AttributeTypeCode.Picklist, crmOrganizationServiceProxy);
                                //    if (isCreated == false)
                                //    {
                                //        Console.WriteLine("Unable to create Picklist attribute!");
                                //    } 
                                //}
                                //else
                                //{
                                    
                                //}                               
                            } // end compare

                        } // end the loop!
                        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                    }
                    else
                    {
                        // the EntityMetadata objecy was found
                        Console.WriteLine("The EntityMetadata was found. " + errorMessage);
                        // exit the process
                    }

                }// end table loop

                 // check the why the Dispose method does not appears in MetadataHelper....
                 if (crmOrganizationServiceProxy != null)
                 {
                     crmOrganizationServiceProxy.Dispose();
                 }
              
                 
                 Console.WriteLine("End of the process!");
                 Console.ReadLine();

            }
           
            catch (Exception ex)
            {
                // write to the log file!
            }

        }


    }
}
