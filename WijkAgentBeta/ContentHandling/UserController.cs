using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using WijkAgentBeta.Database;

namespace WijkAgentBeta.ContentHandling
{
    public class UserController
    {
        /*
            Author: Rob Nibourg
            Modified:
            Description: This class handles the users in the Database

        */
        #region Attributes
        //privates and components
        string query;
        MySqlCommand command;
        MySqlDataReader reader;
        dbConnection dbCon = new dbConnection();

        //constants integers
        public const int available = 1;
        public const int responding = 2;
        public const int onSite = 3;
        public const int busy = 4;
        public const int notLogged = 5;
        public const int noLocation = 6;
        #endregion

        #region Contructors
        public UserController()
        {

        }

        #endregion

        #region Load Data Users
        //get all available users
        //Made by Rob Nibourg
        public List<User> getAllAvailableUsers()
        {
            //init list to hold all users
            List<User> userList = new List<User>();

            //the query
            query = "SELECT id,code,name,longitude,latitude FROM user WHERE longitude != 0 AND latitude != 0 AND availability = " + available;
            //bind query
            command = new MySqlCommand(query, dbCon.GetConnection());
            try
            {
                //execute query and store results in the reader
                reader = command.ExecuteReader();
                //loop through results
                while (reader.Read())
                {
                    //get values with column index
                    int id = (int)reader.GetValue(0);
                    string code = reader.GetValue(1).ToString();
                    string name = reader.GetValue(2).ToString();
                    float longitude = (float)reader.GetValue(3);
                    float latitude = (float)reader.GetValue(4);
                    //create new user
                    User user = new User(id, code, name, available, latitude, longitude);
                    //add user to list
                    userList.Add(user);
                }
            }
            catch (MySqlException se)
            {
                Debug.WriteLine(se);
            }

            reader.Close();
            dbCon.CloseConnection();

            return userList;
        }

        //Get a user out of the Database by id
        //Made by Rob Nibourg
        public User getUserById(int id)
        {
            //the query
            query = "SELECT id,name,code,longitude,latitude,availability,lastUpdate FROM user WHERE id = " + id;
            //bind query
            command = new MySqlCommand(query, dbCon.GetConnection());
            //execute query and store results in the reader
            reader = command.ExecuteReader();
            User user = new User();

            //loop through results
            while (reader.Read())
            {
                //get values with column index
                int Id = (int)reader.GetValue(0);
                string name = reader.GetValue(1).ToString();
                string code = reader.GetValue(2).ToString();
                float longitude = (float)reader.GetValue(3);
                float latitude = (float)reader.GetValue(4);
                int availability = (int)reader.GetValue(5);
                DateTime lastUpdate = (DateTime)reader.GetValue(6);

                //create new user
                user = new User(id, code, name, available, latitude, longitude);
                user.lastUpdate = lastUpdate;
            }

            reader.Close();
            dbCon.CloseConnection();

            return user;
        }

        //Get a user out of the Database by code
        //Made by Rob Nibourg
        public User getUserByCode(string code)
        {
            User user = new User();
            try
            {
                //the query
                query = "SELECT * FROM user WHERE code = '" + code + "'";
                //bind query
                command = new MySqlCommand(query, dbCon.GetConnection());
                //execute query and store results in the reader
                reader = command.ExecuteReader();

                //loop through results
                while (reader.Read())
                {
                    //get values with column index
                    int id = (int)reader.GetValue(0);
                    string Code = reader.GetValue(2).ToString();
                    string name = reader.GetValue(1).ToString();
                    float longitude = (float)reader.GetValue(4);
                    float latitude = (float)reader.GetValue(5);
                    //create new user
                    user = new User(id, Code, name, available, latitude, longitude);
                }
            }
            catch (MySqlException se)
            {
                Debug.WriteLine(se);
            }

            reader.Close();
            dbCon.CloseConnection();

            return user;
        }

        //Get a user out of the Database by name
        //Made by Rob Nibourg
        public User getUserByName(string username)
        {

            User user = new User();
            try
            {
                //the query
                query = "SELECT * FROM user WHERE name = '" + username + "'";
                //bind query
                command = new MySqlCommand(query, dbCon.GetConnection());
                //execute query and store results in the reader
                reader = command.ExecuteReader();

                //loop through results
                while (reader.Read())
                {
                    //get values with column index
                    int id = (int)reader.GetValue(0);
                    string Code = reader.GetValue(2).ToString();
                    string name = reader.GetValue(1).ToString();
                    float longitude = (float)reader.GetValue(4);
                    float latitude = (float)reader.GetValue(5);
                    //create new user
                    user = new User(id, Code, name, available, latitude, longitude);
                }
            }
            catch (MySqlException se)
            {
                Debug.WriteLine(se);
            }

            reader.Close();
            dbCon.CloseConnection();

            return user;
        }
        #endregion

        #region Create/update users
        //Update a User record in the Database
        //Made by Rob Nibourg
        public void updateUser(User user)
        {
            //Building the query
            query = "UPDATE user SET code='" + user.code +
                "',name='" + user.name +
                "',latitude=" + user.latitude +
                ",longitude=" + user.longitude +
                ",availability=" + user.availability +
                " WHERE id=" + user.id;
            //Binding query
            command = new MySqlCommand(query, dbCon.GetConnection());
            try
            {
                //execute query and catch result
                reader = command.ExecuteReader();
                reader.Close();
            }
            catch (MySqlException se)
            {
                Debug.WriteLine(se);
            }
            dbCon.CloseConnection();
        }

        //Create a User Record in the Database
        //Made by Rob Nibourg
        public void createUser(User user, SecureString password)
        {
            //Hash the password
            string pass = AuthenticationController.ConvertToUnsecureString(password);
            //Building query
            query = "INSERT INTO user (code,name,availability,longitude,latitude,password) VALUES ('" + user.code + "','" + user.name + "'," + user.availability + "," + user.longitude + "," + user.latitude + "," + pass + ")";
            //Binding query
            command = new MySqlCommand(query, dbCon.GetConnection());

            try
            {
                //execute query and catch result
                reader = command.ExecuteReader();
                reader.Close();
            }
            catch (MySqlException se)
            {
                Debug.WriteLine(se);
            }
            dbCon.CloseConnection();
        }
        #endregion
    }
}
