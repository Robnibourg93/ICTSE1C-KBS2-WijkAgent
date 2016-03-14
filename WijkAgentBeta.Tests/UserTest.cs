using System;
using WijkAgentBeta.Tests;
using WijkAgentBeta;
using WijkAgentBeta.ContentHandling;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WijkAgentBeta.Tests
{
    [TestClass]
    public class UserTest
    {
        [TestMethod]
        public void User_checklocation_noLocation()
        {
            //Arrange
            Boolean result;
            User UserTest = new User();
            UserTest.latitude = 0;
            UserTest.longitude = 0;


            //act
            result = UserTest.checkLocation();

            //assert
            Assert.IsFalse(result);


        }
        [TestMethod]
        public void User_checklocation_noLatitude()
        {
            //Arrange
            Boolean result;
            User UserTest = new User();
            UserTest.latitude = 0;
            UserTest.longitude = 123.49f;

            //act
            result = UserTest.checkLocation();

            //assert
            Assert.IsFalse(result);
        }
        [TestMethod]
        public void User_checklocation_noLongitude()
        {
            //Arrange
            Boolean result;
            User UserTest = new User();
            UserTest.latitude = 124.67f;
            UserTest.longitude = 0;

            //act
            result = UserTest.checkLocation();


            //assert
            Assert.IsFalse(result);
        }
        [TestMethod]
        public void User_checklocation_locationAvailable()
        {
            //Arrange
            Boolean result;
            User UserTest = new User();
            UserTest.latitude = 124.67f;
            UserTest.longitude = 364.38f;

            //act
            result = UserTest.checkLocation();


            //assert
            Assert.IsTrue(result);
        }

    }
}
