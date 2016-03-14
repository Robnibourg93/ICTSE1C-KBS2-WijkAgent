using MySql.Data.MySqlClient;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using WijkAgentBeta.Database;

namespace WijkAgentBeta.ContentHandling
{


    public class AuthenticationController
    {
        /*
            Author: Rob Nibourg
            Modified: Rob Nibourg

            Description: This class handles the Authentication of the user.

        */
        #region Attributes
        UserController users = new UserController();
        dbConnection dbCon = new dbConnection();


        public static User loggedInUser;
        public static bool isLogged = false;

        string query;
        MySqlCommand command;
        MySqlDataReader reader;
        #endregion

        #region Constructors
        public AuthenticationController()
        {

        }
        #endregion

        #region Encryption/Authentication
        //Hash Password method
        //Made by Rob Nibourg
        public static string MD5Hash(SecureString text)
        {
            MD5 md5 = new MD5CryptoServiceProvider();

            //compute hash from the bytes of text
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(ConvertToUnsecureString(text)));

            //get hash result after compute it
            byte[] result = md5.Hash;

            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                //change it into 2 hexadecimal digits
                //for each byte
                strBuilder.Append(result[i].ToString("x2"));
            }

            return strBuilder.ToString();
        }

        //convert string so we can hash it
        //Made by Rob Nibourg
        public static string ConvertToUnsecureString(SecureString securePassword)
        {
            if (securePassword == null)
                throw new ArgumentNullException("securePassword");

            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(securePassword);

                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }

        //Made by Rob Nibourg
        //Method used to login the user into the application
        public bool logIn(string code, SecureString password)
        {
            //set Authenticated to false
            bool Authenticated = false;
            User user = users.getUserByCode(code);

            //Hash the password
            string hashedPassword = MD5Hash(password);
            //Build query using COUNT(*) to see if we getting 1 matching record
            query = "SELECT COUNT(*) FROM user WHERE id=" + user.id + " AND password='" + hashedPassword + "'";

            try
            {
                //Bind query
                command = new MySqlCommand(query, dbCon.GetConnection());
                //execute query
                reader = command.ExecuteReader();
                //Reading results. 
                while (reader.Read())
                {
                    if (reader.GetInt32(0) == 1)
                    {
                        Authenticated = true;
                    }
                }
            }
            catch (MySqlException se)
            {
                Debug.Write(se);
            }

            reader.Close();
            //if user is Authenticated set static user to Authenticated user.
            if (Authenticated)
            {
                loggedInUser = users.getUserByCode(code);
                loggedInUser.availability = UserController.available;
                users.updateUser(loggedInUser);
                //Console.WriteLine("User :"+loggedInUser.name+" has logged in");
                isLogged = true;
            }

            dbCon.CloseConnection();
            return Authenticated;
        }
        //Log out user.
        //Made by Rob Nibourg
        public void LogOut()
        {
            loggedInUser = null;
            isLogged = false;
        }
        #endregion
    }
}
