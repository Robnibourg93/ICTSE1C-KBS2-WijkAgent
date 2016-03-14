using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using WijkAgentBeta.ContentHandling;

namespace WijkAgentBeta.Tests
{
    [TestClass]
    public class AuthenticationControllerTests
    {
        string succes = "succes";

        [TestMethod]
        public void AuthenticationController_MD5Hash_StringAvailable() {

            //Arrange
            SecureString pass = new SecureString();
            pass.InsertAt(0, 't');
            pass.InsertAt(1, 'e');
            pass.InsertAt(2, 's');
            pass.InsertAt(3, 't');

            //Act
            string result = AuthenticationController.MD5Hash(pass);

            //Assert
            Assert.IsNotNull(result, succes);
        }

        [TestMethod]
        public void AuthenticationController_ConvertToUnsecureString_StringAvailable_CheckExpectedResult() {

            //Arrange
            SecureString pass = new SecureString();
            pass.InsertAt(0, 't');
            pass.InsertAt(1, 'e');
            pass.InsertAt(2, 's');
            pass.InsertAt(3, 't');
            string test = "test";

            //Act
            string result = AuthenticationController.ConvertToUnsecureString(pass);


            //Assert
            Assert.IsNotNull(result, succes);
            Assert.AreEqual(test, result);
        }

        [TestMethod]
        public void AuthenticationController_logIn_BoolAvailable_CheckExpectedResult_SetLoggedInUser_IsLoggedBoolCheck() {

            //Arrange
            string codeCorrect = "test";
            string codeIncorrect = "ddf";
            SecureString pass = new SecureString();
            pass.InsertAt(0, 't');
            pass.InsertAt(1, 'e');
            pass.InsertAt(2, 's');
            pass.InsertAt(3, 't');
            AuthenticationController authCon = new AuthenticationController();
            User Test;

            //Act
            bool resultIncorrect = authCon.logIn(codeIncorrect, pass);
            bool resultCorrect = authCon.logIn(codeCorrect,pass);
            
            Test = AuthenticationController.loggedInUser;
            //Assert
            Assert.IsNotNull(Test);
            Assert.IsNotNull(resultCorrect, succes);
            Assert.IsNotNull(resultIncorrect, succes);
            Assert.IsTrue(resultCorrect);
            Assert.IsTrue(AuthenticationController.isLogged);
            Assert.IsFalse(resultIncorrect);



        }

        [TestMethod]
        public void AuthenticationController_logOut_IsLoggedBoolCheck() {
            //Arrange
            AuthenticationController.isLogged = true;
            AuthenticationController.loggedInUser = new User();
            AuthenticationController authCon = new AuthenticationController();

            //Act
            authCon.LogOut();

            //Assert
            Assert.IsFalse(AuthenticationController.isLogged);

        }

    }
}
