using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DynamicConnections.CRM2011.Common.Utility;
using DynamicConnections.NutriStyle.CRM2011.Testing;

using NUnit.Framework;

namespace DynamicConnections.NutriStyle.CRM2011.Testing.Tests
{
    [TestFixture]
    public class WebServiceTest : General
    {
        User u = new User();
        [SetUp]
        public void CreateUser()
        {
            
            Assert.True(u.CreateUser());//Did the user get created
            Console.WriteLine("Created Contact: "+u.ContactId);
            Assert.True(u.SetHeight());
            Console.WriteLine("Updated Contact Height");
            Assert.True(u.SetBirthdate());
            Console.WriteLine("Updated Contact Birthdate");
            Assert.True(u.SetWeight());
            Console.WriteLine("Updated Contact Weight");
            Assert.True(u.SetREE());
            Console.WriteLine("Updated Contact REE");
            Assert.True(u.SetDEE());
            Console.WriteLine("Updated Contact DEE");
            Assert.True(u.AddFavorite());
            Console.WriteLine("Added Food Like");
            Assert.True(u.AddFoodDislike());
            Console.WriteLine("Added Food Dislike");
            
        }
        [Test]
        public void TestGenerateMenu()
        {
            WebService ws = new WebService();
            ws.ContactId = u.ContactId;
            Console.WriteLine("Starting testing of menu generation: " + ws.ContactId);
            ws.GenerateMenu();
            Console.WriteLine("Tested menu generation: " + u.ContactId);
        }

        [Test]
        public void TestUpdateContact()
        {
            WebService ws = new WebService();
            ws.ContactId = u.ContactId;
            ws.UpdateContact();
            Console.WriteLine("Updated Contact");
        }
        [Test]
        public void TestAddFoodLike()
        {
            WebService ws = new WebService();
            ws.ContactId = u.ContactId;
            ws.AddFoodLike();
            Console.WriteLine("Added Food Like");
        }
        [Test]
        public void TestAddFoodDisLike()
        {
            WebService ws = new WebService();
            ws.ContactId = u.ContactId;
            ws.AddFoodDislike();
            Console.WriteLine("Added Food DisLike");
        }
        [Test]
        public void TestUpdateMenuOptions()
        {
            WebService ws = new WebService();
            ws.ContactId = u.ContactId;
            ws.UpdateMenuOptions();
            Console.WriteLine("Updated Menu Options");
        }
        [TearDown]
        public void DeleteUser()
        {
            Assert.True(u.DeleteUser());//Did the user get deleted
            Console.WriteLine("Deleted Contact");
        }
    }
}
