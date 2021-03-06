﻿using System;
using System.Collections.Generic;
using System.Linq;
using DynamicConnections.CRM2011.Common.Utility;
using Microsoft.Xrm.Sdk.Client;
using System.Xml;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk;
using System.Web.Services.Protocols;


namespace DynamicConnections.NutriStyle.CRM2011.WebServices.Engine
{
    public class DatabaseHelper
    {
        private static int orderNotNeeded = 1000;

        public static XmlDocument BuildXml(EntityCollection entityCollection, String entityName,
            XmlDocument results, OrganizationServiceProxy crmService, Dictionary<String, int> columnOrder)
        {
            SortedDictionary<int, XmlNode> columnNodes = new SortedDictionary<int, XmlNode>();
            results = new XmlDocument();
            try
            {
                XmlNode resultSet = results.CreateNode(XmlNodeType.Element, "resultset", "");
                results.AppendChild(resultSet);
                
                //Build out column collection
                XmlNode columnsNode = results.CreateNode(XmlNodeType.Element, "columns", "");
                
                resultSet.AppendChild(columnsNode);
                //foreach (String name in columnNames)
                EntityMetadata metadata = null;

                //if (entityCollection.Entities.Count > 0) {
                columnNodes = BuildColumnsNode(results, entityName, metadata, columnNodes, crmService, columnOrder);

                foreach (KeyValuePair<int, XmlNode> pair in columnNodes)
                {
                    if (columnsNode.SelectSingleNode(((XmlNode)pair.Value).Name) == null)
                    {
                        columnsNode.AppendChild((XmlNode)pair.Value);
                    }
                    
                }
                Logger.Write(Logger.DEBUG, "Looping through entities");
                foreach (Entity evt in entityCollection.Entities)
                {
                    resultSet.AppendChild(BuildEntityNode(results, evt, metadata, crmService));
                }
                if (entityCollection == null || entityCollection.Entities.Count() == 0)
                {
                    //No results found
                    XmlNode elementNode = results.CreateNode(XmlNodeType.Element, entityName, "");

                }
            }
            catch (Exception e)
            {
                Logger.Write(e.Message);
                Logger.Write(e.StackTrace);
            }
            return (results);

        }
        private static SortedDictionary<int, XmlNode> BuildColumnsNode(XmlDocument results, String entityName, EntityMetadata metadata,
            SortedDictionary<int, XmlNode> columnsNode, OrganizationServiceProxy crmService, Dictionary<String, int> columnOrder)
        {
            if (columnOrder != null)
            {

                foreach (KeyValuePair<String, int> pair in columnOrder)
                {
                    XmlNode node = results.CreateNode(XmlNodeType.Element, pair.Key, "");
                    String columnName = pair.Key;

                    //Validate metadata
                    if (columnName.Contains('.'))
                    {
                        Logger.Write(Logger.DEBUG, "Found '.': "+columnName);
                        String tmpEntityName = columnName.Substring(0, pair.Key.IndexOf('.'));
                        metadata = MetadataHelper.GetEntityMetadata(tmpEntityName, crmService);
                        columnName = columnName.Substring(columnName.IndexOf('.') + 1);
                        node = results.CreateNode(XmlNodeType.Element, pair.Key, "");
                    }
                    else //have to reset metadata
                    {
                        try
                        {
                            metadata = MetadataHelper.GetEntityMetadata(entityName, crmService);
                        }
                        catch (Exception e)
                        {

                            Logger.Write("entityName: "+entityName);
                            Logger.Write("columnName: " + columnName);
                            Logger.Write(Logger.ERROR, e.Message);
                            Logger.Write(Logger.ERROR, e.StackTrace);

                        }
                    }

                    if (columnName.StartsWith("alias_", StringComparison.OrdinalIgnoreCase))
                    {
                        columnName = columnName.Replace("alias_", "");
                    }
                    if (!((String)pair.Key).Equals("transactioncurrencyid", StringComparison.OrdinalIgnoreCase))
                    {

                        node.InnerText = columnName;

                        XmlAttribute labelAttribute = results.CreateAttribute("LabelName");
                        //Logger.Write("LabelName: " + columnName + ": Entity: " + metadata.DisplayName.UserLocalizedLabel.Label);
                        if (MetadataHelper.GetAttributeMetaData(metadata, columnName) != null)
                        {
                            //Logger.Write("LabelName: " + columnName);
                            if (MetadataHelper.GetAttributeMetaData(metadata, columnName) != null &&
                                MetadataHelper.GetAttributeMetaData(metadata, columnName).DisplayName != null &&
                                MetadataHelper.GetAttributeMetaData(metadata, columnName).DisplayName.UserLocalizedLabel != null &&
                                MetadataHelper.GetAttributeMetaData(metadata, columnName).DisplayName.UserLocalizedLabel.Label != null)
                            {
                                labelAttribute.InnerText = MetadataHelper.GetAttributeMetaData(metadata, columnName).DisplayName.UserLocalizedLabel.Label;
                                node.Attributes.Append(labelAttribute);
                            }
                            else
                            {
                                Logger.Write("Label is null. " + entityName + ":" + columnName);
                            }
                        }

                        XmlAttribute typeAttribute = results.CreateAttribute("Type");
                        if (MetadataHelper.GetAttributeMetaData(metadata, columnName) != null)
                        {
                            typeAttribute.InnerText = MetadataHelper.GetAttributeMetaData(metadata, columnName).AttributeType.Value.ToString();
                            node.Attributes.Append(typeAttribute);
                        }
                        else
                        {
                            /*
                            Logger.Write("columnName: " + columnName);
                            Logger.Write("entityName: " + entityName);
                            */
                        }

                        XmlAttribute entityAttribute = results.CreateAttribute("Entity");
                        if ((AttributeMetadata)MetadataHelper.GetAttributeMetaData(metadata, columnName) != null)
                        {
                            AttributeMetadata am = (AttributeMetadata)MetadataHelper.GetAttributeMetaData(metadata, columnName);
                            //Logger.Write("Type: " + am.GetType());
                            if (am.GetType() == typeof(LookupAttributeMetadata))
                            {
                                foreach (String c in ((LookupAttributeMetadata)am).Targets)
                                {
                                    //Logger.Write("target: " + c);
                                }
                                entityAttribute.InnerText = ((LookupAttributeMetadata)am).Targets[0];
                            }
                            else
                            {
                                entityAttribute.InnerText = entityName;
                            }
                            node.Attributes.Append(entityAttribute);
                        }

                        XmlAttribute friendlyNameAttribute = results.CreateAttribute("DisplayName");
                        if (metadata.DisplayName != null)
                        {
                            friendlyNameAttribute.InnerText = metadata.DisplayName.UserLocalizedLabel.Label;
                            node.Attributes.Append(friendlyNameAttribute);
                        }
                        //columnsNode.AppendChild(node);
                        if (columnOrder != null)
                        {
                            if (columnOrder.ContainsKey(pair.Key) && !columnsNode.ContainsKey(columnOrder[pair.Key]))
                            {
                                columnsNode.Add(columnOrder[pair.Key], node);
                            }
                            else
                            {
                                columnsNode.Add(orderNotNeeded, node);
                                orderNotNeeded++;
                            }
                        }
                        else
                        {
                            columnsNode.Add(orderNotNeeded, node);
                            orderNotNeeded++;
                        }
                    }
                }
            }
            return (columnsNode);
        }

        private static XmlNode BuildEntityNode(XmlDocument results, Entity evt, EntityMetadata metadata, OrganizationServiceProxy crmService)
        {
            XmlNode elementNode = results.CreateNode(XmlNodeType.Element, evt.LogicalName, "");
            //Logger.Write(Logger.DEBUG, "BuildEntityNode: starting");
            //foreach (String name in columnNames)
            foreach (KeyValuePair<String, object> pair in evt.Attributes)
            {
                String columnName = pair.Key;
                String entityName = evt.LogicalName;
                //Logger.Write("top: columnName: " + columnName);
                //Validate metadata
                if (columnName.Contains('.'))
                {
                    entityName = columnName.Substring(0, pair.Key.IndexOf('.'));
                    metadata = MetadataHelper.GetEntityMetadata(entityName, crmService);
                    columnName = columnName.Substring(columnName.IndexOf('.') + 1);
                }
                else //have to reset metadata
                {
                    metadata = MetadataHelper.GetEntityMetadata(evt.LogicalName, crmService);
                }
               
                /*create node*/
                XmlNode node = results.CreateNode(XmlNodeType.Element, pair.Key, "");
                
                String typeAttribute = "String";
                //Logger.Write(Logger.DEBUG, "columnName: "+columnName);

                if(!columnName.EndsWith("name")) {
                    typeAttribute = MetadataHelper.GetAttributeMetaData(metadata, columnName.Replace("alias_", "")).AttributeType.Value.ToString();
                }
                //Logger.Write("entityName: " + entityName + ": columnName: " + columnName + ": typeAttribute: " + typeAttribute);

                if (typeAttribute.Equals("String", StringComparison.OrdinalIgnoreCase))
                {
                    var cdata = results.CreateCDataSection(String.Empty);

                    if (evt.Attributes.Contains(pair.Key) && evt.Attributes[pair.Key] != null)
                    {
                        if (evt[pair.Key].GetType() == typeof(AliasedValue))
                        {
                            AliasedValue av = (AliasedValue)evt[pair.Key];
                            //node.InnerText = (String)av.Value;
                            cdata = results.CreateCDataSection((String)av.Value);
                        }
                        else
                        {
                            //node.InnerText = (String)evt[pair.Key];
                            cdata = results.CreateCDataSection((String)evt[pair.Key]);
                        }
                    }
                    node.AppendChild(cdata);
                }
                else if (typeAttribute.Equals("Memo", StringComparison.OrdinalIgnoreCase))
                {
                    var cdata = results.CreateCDataSection(String.Empty);
                    if (evt.Attributes.Contains(pair.Key) && evt.Attributes[pair.Key] != null)
                    {
                        if (evt[pair.Key].GetType() == typeof(AliasedValue))
                        {
                            AliasedValue av = (AliasedValue)evt[pair.Key];
                            //node.InnerText = (String)av.Value;
                            cdata = results.CreateCDataSection((String)av.Value);
                        }
                        else
                        {
                            //node.InnerText = (String)evt[pair.Key];
                            cdata = results.CreateCDataSection((String)evt[pair.Key]);
                        }
                    }
                    node.AppendChild(cdata);
                }
                else if (typeAttribute.Equals("Status", StringComparison.OrdinalIgnoreCase))
                {
                    //Logger.Write("--------Type--------------: " + evt[pair.Key].GetType());

                    if (evt.Attributes.Contains(pair.Key) && evt.Attributes[pair.Key] != null)
                    {

                        XmlAttribute idAttribute = results.CreateAttribute("Id");
                        idAttribute.InnerText = ((OptionSetValue)evt[pair.Key]).Value.ToString();
                        if (idAttribute.InnerText != String.Empty)
                        {
                            //TODO: Check for int value
                            String type = MetadataHelper.RetrieveStatusSetValueString(metadata, columnName, Convert.ToInt32(idAttribute.InnerText));
                            node.InnerText = type;
                            //Logger.Write("type: " + type);
                        }
                    }
                }
                else if (typeAttribute.Equals("Boolean", StringComparison.OrdinalIgnoreCase))
                {
                    if (evt.Attributes.Contains(pair.Key) && evt.Attributes[pair.Key] != null)
                    {
                        if (evt[pair.Key].GetType() == typeof(AliasedValue))
                        {
                            AliasedValue av = (AliasedValue)evt[pair.Key];
                            node.InnerText = Convert.ToBoolean(av.Value).ToString();
                        }
                        else
                        {
                            node.InnerText = Convert.ToBoolean(evt[pair.Key]).ToString() ;
                        }
                    }
                }
                else if (typeAttribute.Equals("DateTime", StringComparison.OrdinalIgnoreCase))
                {
                    if (evt.Attributes.Contains(pair.Key) && evt.Attributes[pair.Key] != null)
                    {
                        try
                        {
                            if (evt[pair.Key].GetType() == typeof(AliasedValue))//for group by
                            {
                                AliasedValue av = (AliasedValue)evt[pair.Key];
                                //Logger.Write(Logger.INFO, "Type: "+av.Value.GetType());
                                node.InnerText = av.Value.ToString();
                            }
                            else
                            {
                                node.InnerText = ((DateTime)evt[pair.Key]).ToString();
                            }
                        }
                        catch (FormatException)
                        {

                        }
                    }
                }
                else if (typeAttribute.Equals("Money", StringComparison.OrdinalIgnoreCase))
                {
                    //Logger.Write("Processing Money: " + pair.Key);
                    if (evt.Attributes.Contains(pair.Key) && evt.Attributes[pair.Key] != null)
                    {
                        try
                        {
                            Logger.Write(((Money)evt[pair.Key]).Value.ToString());
                            node.InnerText = ((Money)evt[pair.Key]).Value.ToString();
                        }
                        catch (FormatException)
                        {

                        }
                    }
                }
                else if (typeAttribute.Equals("Integer", StringComparison.OrdinalIgnoreCase))
                {
                   // Logger.Write("Processing Integer: " + pair.Key);
                    if (evt.Attributes.Contains(pair.Key) && evt.Attributes[pair.Key] != null)
                    {
                        try
                        {
                            Logger.Write(((int)evt[pair.Key]).ToString());
                            node.InnerText = ((int)evt[pair.Key]).ToString();
                        }
                        catch (FormatException)
                        {

                        }
                    }
                }
                else if (typeAttribute.Equals("Decimal", StringComparison.OrdinalIgnoreCase))
                {
                    //Logger.Write("Processing Decimal: " + pair.Key);
                    if (evt.Attributes.Contains(pair.Key) && evt.Attributes[pair.Key] != null)
                    {
                        try
                        {

                            if (evt[pair.Key].GetType() == typeof(AliasedValue))
                            {
                                AliasedValue av = (AliasedValue)evt[pair.Key];
                                node.InnerText = Math.Round((decimal)av.Value, 2).ToString();
                            }
                            else
                            {
                                //node.InnerText = Convert.ToBoolean(evt[pair.Key]).ToString();
                                node.InnerText = Math.Round((decimal)evt[pair.Key], 2).ToString();
                            }
                            //Logger.Write(node.InnerText);
                        }
                        catch (FormatException)
                        {

                        }
                    }
                }
                else if (typeAttribute.Equals("Double", StringComparison.OrdinalIgnoreCase))
                {
                    //Logger.Write("Processing Double: " + pair.Key);
                    if (evt.Attributes.Contains(pair.Key) && evt.Attributes[pair.Key] != null)
                    {
                        try
                        {

                            if (evt[pair.Key].GetType() == typeof(AliasedValue))
                            {
                                AliasedValue av = (AliasedValue)evt[pair.Key];
                                node.InnerText = Math.Round((double)av.Value, 2).ToString();
                            }
                            else
                            {
                                //node.InnerText = Convert.ToBoolean(evt[pair.Key]).ToString();
                                node.InnerText = Math.Round((double)evt[pair.Key], 2).ToString();
                            }
                            //Logger.Write(node.InnerText);
                        }
                        catch (FormatException)
                        {

                        }
                    }
                }
                else if (typeAttribute.Equals("Uniqueidentifier", StringComparison.OrdinalIgnoreCase))
                {
                    if (evt[pair.Key].GetType() == typeof(AliasedValue))
                    {
                        AliasedValue av = (AliasedValue)evt[pair.Key];
                        node.InnerText = ((Guid)av.Value).ToString();
                    }
                    else
                    {
                        node.InnerText = ((Guid)evt[pair.Key]).ToString();
                    }
                }
                else if (typeAttribute.Equals("Lookup", StringComparison.OrdinalIgnoreCase))
                {
                    //Logger.Write("columnName: "+columnName);
                    //Logger.Write("pair.Key: " + pair.Key);
                    foreach (KeyValuePair<String, object> pairII in evt.Attributes)
                    {
                        String columnNameII = pairII.Key;
                        String entityNameII = evt.LogicalName;
                        //Logger.Write("Lookup : columnName: " + columnNameII);
                        //Logger.Write("Lookup : entityNameII: " + entityNameII);
                    }
                    if (evt.Contains(pair.Key) && evt[pair.Key] != null && evt[pair.Key] != null)
                    {
                        if (evt[pair.Key].GetType() == typeof(AliasedValue))
                        {
                            //Logger.Write("Found aliased lookup");
                            AliasedValue av = (AliasedValue)evt[pair.Key];
                            //node.InnerText = Math.Round((decimal)av.Value, 2).ToString();
                           
                            node.InnerText = ((EntityReference)av.Value).Name.ToString();
                            XmlAttribute idAttribute = results.CreateAttribute("Id");
                            idAttribute.InnerText = ((EntityReference)av.Value).Id.ToString();
                            node.Attributes.Append(idAttribute);
                        }
                        else
                        {
                            //Logger.Write("Processing lookup: type: " + evt[pair.Key].GetType());
                            if (((EntityReference)evt[pair.Key]) != null && ((EntityReference)evt[pair.Key]).Name != null &&
                               ((EntityReference)evt[pair.Key]).Id != null)
                            {
                                node.InnerText = ((EntityReference)evt[pair.Key]).Name.ToString();
                                XmlAttribute idAttribute = results.CreateAttribute("Id");
                                idAttribute.InnerText = ((EntityReference)evt[pair.Key]).Id.ToString();
                                node.Attributes.Append(idAttribute);
                                //Logger.Write("node.InnerText: " + node.InnerText);
                            }
                        }
                    }

                }
                else if (typeAttribute.Equals("owner", StringComparison.OrdinalIgnoreCase))
                {

                    if (evt.Attributes.Contains(columnName) && evt.Attributes[columnName] != null)
                    {
                        node.InnerText = ((EntityReference)evt[pair.Key]).Name.ToString();
                        XmlAttribute idAttribute = results.CreateAttribute("Id");
                        idAttribute.InnerText = ((EntityReference)evt[pair.Key]).Id.ToString();
                        node.Attributes.Append(idAttribute);
                    }

                }
                else if (typeAttribute.Equals("Picklist", StringComparison.OrdinalIgnoreCase))
                {
                    XmlAttribute idAttribute = results.CreateAttribute("Id");
                    //Logger.Write("Processing picklist");
                   

                    if (evt[pair.Key].GetType() == typeof(AliasedValue))
                    {
                        AliasedValue av = (AliasedValue)evt[pair.Key];
                        idAttribute.InnerText = ((OptionSetValue)av.Value).Value.ToString();
                    }
                    else
                    {
                        idAttribute.InnerText = ((OptionSetValue)evt[pair.Key]).Value.ToString();
                    }
                    //Logger.Write("idAttribute.InnerText: " + idAttribute.InnerText);
                    node.Attributes.Append(idAttribute);
                    if (idAttribute.InnerText != String.Empty)
                    {
                        //TODO: Check for int value
                        String type = MetadataHelper.RetrieveOptionSetValueString(metadata, columnName, Convert.ToInt32(idAttribute.InnerText));
                        node.InnerText = type;
                        //Logger.Write("type: " + type);
                    }
                }
                if (node.InnerText != String.Empty)
                {
                    elementNode.AppendChild(node);
                }
            }//End of for loop
            return (elementNode);
        }

        public static bool ValidateOptionSet(String entityName, String attributeName, int value, OrganizationServiceProxy crmService)
        {
            Logger.Write("ValidateOptionSet: starting");
            bool valid = false;
            //Get values for picklist
            try
            {
                EntityMetadata metadata = MetadataHelper.GetEntityMetadata(entityName, crmService);
                PicklistAttributeMetadata pam = (PicklistAttributeMetadata)MetadataHelper.GetAttributeMetaData(metadata, attributeName);

                foreach (OptionMetadata om in pam.OptionSet.Options)
                {
                    if (om.Value == value)
                    {
                        valid = true;
                        break;
                    }
                }
            }
            catch (SoapException e)
            {
                throw (e);
            }
            catch (Exception e)
            {
                throw (e);
            }
            return (valid);
        }
        public static bool ValidateDate(String entityName, String attributeName, DateTime value, OrganizationServiceProxy crmService)
        {
            bool valid = false;
            EntityMetadata metadata = MetadataHelper.GetEntityMetadata(entityName, crmService);
            DateTimeAttributeMetadata pam = (DateTimeAttributeMetadata)MetadataHelper.GetAttributeMetaData(metadata, attributeName);
            if (value < DateTimeAttributeMetadata.MaxSupportedValue && value > DateTimeAttributeMetadata.MinSupportedValue)
            {
                valid = true;
            }
            return (valid);
        }
        public static bool ValidateMoney(String entityName, String attributeName, decimal value, OrganizationServiceProxy crmService)
        {
            bool valid = false;
            EntityMetadata metadata = MetadataHelper.GetEntityMetadata(entityName, crmService);
            MoneyAttributeMetadata sam = (MoneyAttributeMetadata)MetadataHelper.GetAttributeMetaData(metadata, attributeName);
            if (Convert.ToDouble(value) >= sam.MinValue.Value && Convert.ToDouble(value) <= sam.MaxValue.Value)
            {
                //p = new CrmMoneyProperty(target, new DynamicConnections.Crm.Common.Sdk.CrmServiceSdk.CrmMoney(d));
                valid = true;
            }


            return (valid);
        }
    }
}