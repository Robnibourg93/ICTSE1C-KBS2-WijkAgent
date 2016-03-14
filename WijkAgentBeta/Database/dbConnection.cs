using System;
using System.Windows;
using MySql.Data.MySqlClient;

namespace WijkAgentBeta.Database
{
    public class dbConnection
    {
        /*
            Author: Rob Nibourg
            Modified: Rob Nibourg, Ronne Timmerman
            Description: This class handles the database connection. 

        */

        private MySqlConnection conn;
        string myConnectionString;

        //connects to the server
        //Made by Rob Nibourg
        public dbConnection()
        {
            myConnectionString = "server=server.berryh.tk;port=8080;uid=kbs2rob;" +
            "pwd=windesheimrob;database=kbs2rob;Pooling=False;";
        }
        //closes the connection
        //Made by Rob Nibourg
        public void CloseConnection() {
            conn.Close();
        }

        //checks if connection is open, if not open it tries to open a connection.
        //Made by Rob Nibourg
        public bool IsConnect()
        {
            bool result = false;
            if (conn != null)
            {
                result = true;
            }
            else
            {
                try
                {
                    conn = new MySqlConnection(myConnectionString);
                    conn.Open();
                    result = true;
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            return result;

        }

        //returns a connection;
        //Made by Rob Nibourg
        public MySqlConnection GetConnection()
        {
            try
            {
                conn = new MySqlConnection(myConnectionString);
                conn.Open();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            return conn;
        }

       

    }


}
