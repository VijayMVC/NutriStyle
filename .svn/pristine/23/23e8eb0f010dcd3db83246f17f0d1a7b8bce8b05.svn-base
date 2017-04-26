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
    public class UserTest : General
    {
        User u = new User();
        [SetUp]
        public void CreateUser()
        {
            
            Assert.True(u.CreateUser());//Did the user get created
            Console.WriteLine("Created Contact: "+u.ContactId);
            
            
        }
        [Test]
        public void TestPlugins()
        {
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
            Assert.True(u.AddFitnessLog());
            Console.WriteLine("Added Fitness Log");
            /*
            Assert.True(u.GenerateMenu());
            Console.WriteLine("Tested Menu Generation");
            */
        }
        [Test]
        public void TestFoodLike()
        {
            Assert.True(u.AddFavorite());
            Console.WriteLine("Added Food Like");
        }
        [Test]
        public void TestFoodDislike()
        {
            Assert.True(u.AddFoodDislike());
            Console.WriteLine("Added Food Dislike");
        }
        [Test]
        public void TestCreateMenu()
        {
            Assert.True(u.GenerateMenu());
            Console.WriteLine("Tested Menu Generation");
        }
        [TearDown]
        public void DeleteUser()
        {
            Assert.True(u.DeleteUser());//Did the user get deleted
            Console.WriteLine("Deleted Contact");
        }
    }
}
