using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using DynamicConnections.CRM2011.Common.Utility;
using Microsoft.Xrm.Sdk;
using System.Xml;
using DynamicConnections.NutriStyle.CRM2011.WebServices.Engine;
using DynamicConnections.NutriStyle.CRM2011.MenuGenerator;

namespace DynamicConnections.NutriStyle.CRM2011.Webservices.Pages
{
    /// <summary>
    /// Summary description for WebServices
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WebServices : System.Web.Services.WebService
    {
        public static String SESSION_TIMEOUT = "Your Session has Expired.\nYou must close out and Login back into the form";

        [WebMethod(EnableSession = true)]
        public XmlDocument CreateUser(String emailaddress, String password, String zipCode, String firstname, String lastname,
            String grocerPrimaryId, String grocerSecondaryId, String grocerTertiaryId, String countryId, String grocerOther, String verificationCodeId)
        {
            XmlDocument xml = new XmlDocument();
            DynamicConnections.NutriStyle.CRM2011.WebServices.Engine.User user = new DynamicConnections.NutriStyle.CRM2011.WebServices.Engine.User();
            try
            {
                xml = user.CreateUser(emailaddress, password, zipCode, firstname, lastname,
                    new Guid(grocerPrimaryId), new Guid(grocerSecondaryId), new Guid(grocerTertiaryId), new Guid(countryId), grocerOther, new Guid(verificationCodeId));
                if (xml.SelectNodes("error").Count > 0)
                {
                    return (xml);//return error
                }
                else
                {
                    Logger.Write("Xml: " + xml.OuterXml);
                    Logger.Write("ContactId: " + xml.FirstChild.InnerText);
                    Session.Add("USER_VALID", true);
                    Session.Add("OBJECTID", xml.FirstChild.InnerText);
                }
                
            }catch(Exception e) {
                Logger.Write(Logger.ERROR, e.Message);
                Logger.Write(Logger.ERROR, e.StackTrace);
            }

            return (xml);
        }
        [WebMethod(EnableSession = true)]
        public XmlDocument Login(String emailaddress, String password)
        {
            DynamicConnections.NutriStyle.CRM2011.WebServices.Engine.User db = new DynamicConnections.NutriStyle.CRM2011.WebServices.Engine.User();
            
            //XmlDocument xmlDoc = db.LoginContact(emailaddress, password, accountId);
            Entity c = db.LoginContact(emailaddress, password);
            /*look for Propery child nodes*/

            XmlDocument xmlDoc = new XmlDocument();
            XmlNode results = xmlDoc.CreateNode(XmlNodeType.Element, "results", "");
            XmlNode valid = xmlDoc.CreateNode(XmlNodeType.Element, "valid", "");
            XmlNode guid = xmlDoc.CreateNode(XmlNodeType.Element, "guid", "");

            xmlDoc.AppendChild(results);
            results.AppendChild(valid);
            results.AppendChild(guid);

            valid.InnerText = false.ToString();
            guid.InnerText = Guid.Empty.ToString();
            if (c != null)
            {
                Session.Add("USER_VALID", true);
                xmlDoc = db.xmlDocResults;//get results of the login
                /*Find node that contactid is id*/
                if (c != null)
                {
                    Guid contactId = c.Id;
                    guid.InnerText = c.Id.ToString();
                    valid.InnerText = true.ToString();
                    Logger.Write("Xml: " + xmlDoc.OuterXml);
                    Session.Add("OBJECTID", contactId);
                }
            }
            else
            {
                Session.Add("USER_VALID", false);
            }
            return (xmlDoc);
        }
        [WebMethod(EnableSession = true)]
        public XmlDocument SessionValid()
        {
            
                if (Convert.ToBoolean(Session["USER_VALID"]))
                {
                    return (Success.Create("Valid"));
                }
           
            return (Error.Create(SESSION_TIMEOUT));
        }

        [WebMethod(EnableSession = true)]
        public XmlDocument RetrieveOptionSetValues(String entityName, String attributeName)
        {
            Picklist p = new Picklist();
            return(p.RetrieveOptionSet(entityName, attributeName)); 
        }
        [WebMethod(EnableSession = true)]
        public XmlDocument CreateUpdate(XmlDocument xmlDoc)
        {
            Database db = new Database();
            return (db.CreateUpdate(xmlDoc));
        }
        /// <summary>
        /// Create or update entity.  Returns all attributes in XML entity
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public XmlDocument CreateUpdateReturnEntity(XmlDocument xmlDoc, bool returnEntity)
        {
            Database db = new Database();
            return (db.CreateUpdate(xmlDoc, returnEntity));
        }

        /// <summary>
        /// Create or update entities.  Returns all attributes in XML entity
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public XmlDocument CreateUpdateReturnEntities(XmlDocument xmlDoc, bool returnEntity)
        {
            Database db = new Database();
            Logger.Write(Logger.DEBUG, "test: " + xmlDoc.OuterXml);
            return (db.CreateUpdateEntities(xmlDoc, returnEntity));
        }

        [WebMethod]
        public XmlDocument RetrieveFetchXml(String fetchXml, String columnOrder)
        {
            Database db = new Database();
            try
            {
                //Logger.Write("fetchXml: " + fetchXml);
                //Logger.Write("columnOrder: " + columnOrder);

                XmlDocument xmlDocFetch = new XmlDocument();
                xmlDocFetch.LoadXml(fetchXml);

                XmlDocument xmlDocColumnOrder = new XmlDocument();
                if (columnOrder != null && !String.IsNullOrEmpty(columnOrder))
                {
                    xmlDocColumnOrder.LoadXml(columnOrder);
                }
                return (db.RetrieveFetchXml(xmlDocFetch, xmlDocColumnOrder, String.Empty));
            }
            catch (Exception e)
            {
                Logger.Write(e.Message);
                Logger.Write(e.StackTrace);
            }
            return (null);
        }
        [WebMethod]
        public XmlDocument RetrieveFetchXmlRowId(String fetchXml, String columnOrder, String rowId)
        {
            Database db = new Database();
            try
            {
                Logger.Write("fetchXml: " + fetchXml);
                Logger.Write("columnOrder: " + columnOrder);

                XmlDocument xmlDocFetch = new XmlDocument();
                xmlDocFetch.LoadXml(fetchXml);

                XmlDocument xmlDocColumnOrder = new XmlDocument();
                if (columnOrder != null && !String.IsNullOrEmpty(columnOrder))
                {
                    xmlDocColumnOrder.LoadXml(columnOrder);
                }
                return (db.RetrieveFetchXmlRowId(xmlDocFetch, xmlDocColumnOrder, rowId));
            }
            catch (Exception e)
            {
                Logger.Write(e.Message);
                Logger.Write(e.StackTrace);
            }
            return (null);
        }
        [WebMethod(EnableSession = true)]
        public XmlDocument Delete(XmlDocument xmlDoc)
        {
            Database db = new Database();
            return (db.Delete(xmlDoc));
        }

        [WebMethod(EnableSession = true)]
        public XmlDocument CreateUpdateMealFood(XmlDocument xmlDoc)
        {
            DynamicConnections.NutriStyle.CRM2011.WebServices.Engine.MealFood mf = 
                new DynamicConnections.NutriStyle.CRM2011.WebServices.Engine.MealFood();
            return (mf.CreateFoodMeal(xmlDoc));
        }
        [WebMethod(EnableSession = true)]
        public XmlDocument RetrieveDailyTip()
        {
            TipsofTheDay tp = new TipsofTheDay();
            return (tp.Retrieve());
        }

        [WebMethod(EnableSession = true)]
        public XmlDocument RemoveFromShippingList(String contactId, String foodId)
        {
            ShoppingList sl = new ShoppingList();
            return (sl.RemoveFood(new Guid(contactId), new Guid(foodId)));
        }
        [WebMethod(EnableSession = true)]
        public XmlDocument ResetShoppingList(String contactId)
        {
            ShoppingList sl = new ShoppingList();
            return (sl.Reset(new Guid(contactId)));
        }
        [WebMethod(EnableSession = true)]
        public XmlDocument GenerateMenu(String contactId)
        {
            CRM2011.WebServices.Engine.BuildMenu bm = new CRM2011.WebServices.Engine.BuildMenu();
            return (bm.Execute(new Guid(contactId)));
        }
        [WebMethod]
        public XmlDocument GenerateShoppingListFromMenu(String menuId)
        {
            ShoppingList sl = new ShoppingList();
            return (sl.CreateFromMenu(new Guid(menuId)));
        }

        [WebMethod]
        public XmlDocument FoodNutrients(String menuId, String dayNumber, string contactId, String entityName, String date)
        {
            FoodNutrients fn = new FoodNutrients();
            return (fn.Retrieve(new Guid(menuId), Convert.ToInt32(dayNumber), new Guid(contactId), entityName, Convert.ToDateTime(date)));
        }
        [WebMethod]
        public XmlDocument FoodNutrientsForEachFood(String menuId, String dayNumber, string contactId, String entityName, String date)
        {
            FoodNutrients fn = new FoodNutrients();
            return (fn.RetrieveForEachFood(new Guid(menuId), Convert.ToInt32(dayNumber), new Guid(contactId), entityName, Convert.ToDateTime(date)));
        }
        [WebMethod]
        public XmlDocument FoodNutrientsFoodId(String foodId)
        {
            FoodNutrients fn = new FoodNutrients();
            return (fn.RetrieveForEachFood(new Guid(foodId)));
        }
        [WebMethod]
        public XmlDocument FoodNutrientsFoodIdPortionSize(String foodId, String portionSize)
        {
            FoodNutrients fn = new FoodNutrients();
            return (fn.RetrieveForEachFood(new Guid(foodId), Convert.ToDecimal(portionSize)));
        }
        [WebMethod]
        public XmlDocument CreateFoodLogFromMenu(String menuId, String contactId)
        {
            FoodLog fl = new FoodLog();
            return (fl.CreateFromMenu(new Guid(menuId), new Guid(contactId)));
        }

        [WebMethod]
        public XmlDocument SetPrimary(String menuId, String contactId)
        {
            Menus m = new Menus();
            return (m.SetPrimaryMenu(new Guid(menuId), new Guid(contactId)));
        }

        [WebMethod]
        public XmlDocument ResendPassword(String emailAddress)
        {
            User u = new User();
            return (u.SendPassword(emailAddress));
        }

        [WebMethod]
        public XmlDocument RetrieveMetadata(String entityName)
        {
            Metadata m = new Metadata();
            return (m.Retrieve(entityName));
        }

        [WebMethod]
        public XmlDocument RetrieveFoodIngredients(String contactId, String searchText)
        {
            return (Queries.RetrieveFoodIngredients(new Guid(contactId), searchText));
        }

        [WebMethod]
        public XmlDocument RetrieveFoodLikes(String contactId, String searchText)
        {
            return (Queries.RetrieveFoodLikes(new Guid(contactId), searchText));
        }
        [WebMethod]
        public XmlDocument RetrieveFoodDislikes(String contactId, String searchText)
        {
            return (Queries.RetrieveFoodDislikes(new Guid(contactId), searchText));
        }

        [WebMethod]
        public XmlDocument RetrieveFoods(String contactId, String searchText)
        {
            return (Queries.RetrieveFoods(new Guid(contactId), searchText));
        }
        [WebMethod]
        public XmlDocument RetrieveAdditionalProfiles(String contactId)
        {
            return (Queries.RetrieveAdditionalProfiles(new Guid(contactId)));
        }
        [WebMethod]
        public XmlDocument RetrieveContactId(String contactId)
        {
            return (DynamicConnections.NutriStyle.CRM2011.WebServices.Engine.User.RetrieveContactId(new Guid(contactId)));
        }
    }
}
