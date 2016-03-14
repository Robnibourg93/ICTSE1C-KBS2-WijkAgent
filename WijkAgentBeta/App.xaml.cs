using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using WijkAgentBeta.ContentHandling;

namespace WijkAgentBeta
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //set the availability for user record in the databse to not logged when application closes
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            try {
                UserController userController = new UserController();
                AuthenticationController.loggedInUser.availability = UserController.notLogged;
                userController.updateUser(AuthenticationController.loggedInUser); }
            catch
            {

            }
        }
    }
}
