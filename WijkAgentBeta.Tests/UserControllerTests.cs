using WijkAgentBeta.ContentHandling;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace WijkAgentBeta.Tests
{
    [TestClass]
    public class UserControllerTests
    {
        [TestMethod]
        public void UserController_getAllAvailableUsers_UsersAvailable()
        {
            //Arrange
            UserController usercon = new UserController();
            List<User> userlist;
            
           

            //act
            userlist = usercon.getAllAvailableUsers();

            //Assert
            Assert.IsNotNull(userlist);

        }

        [TestMethod]
        public void UserController_getUserByCode_available()
        {
            //arrange
            UserController usercon = new UserController();
            User user;

            //act
            user = usercon.getUserByCode("test");

            //Assert
            Assert.IsNotNull(user.code);
        }

        [TestMethod]
        public void UserController_getUserByCode_nonExciting()
        {
            UserController usercon = new UserController();
            User user;

            //act
            user = usercon.getUserByCode("AAAAAA");

            //Assert
            Assert.IsNull(user.code);
        }
        [TestMethod]
        public void UserContorller_getUserById_Available()
        {
            UserController usercon = new UserController();
            User user;

            //act
            user = usercon.getUserById(1);

            //Assert
            Assert.IsNotNull(user.code);
        }
        [TestMethod]
        public void UserContorller_getUserById_nonExciting()
        {
            UserController usercon = new UserController();
            User user;

            //act
            user = usercon.getUserById(0);

            //Assert
            Assert.IsNull(user.code);
        }


    }
}
