using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WijkAgentBeta.Database;
using WijkAgentBeta.View;

namespace WijkAgentBeta.ContentHandling
{
    public class AlertController
    {
        /*
            Author: Joshua van Gelder
            Modified by: Rob Nibourg
            Description: This class handles the alerts in the database
        */
        #region Attributes 
        //main attributes and lists       
        string query;
        public int id;
        public string alert;
        public string service;
        public int idCount;

        public static bool updatedAlertTable = false;

        public List<int> idList = new List<int>();
        public List<string> alertTitles = new List<string>();
        public List<string> alertListSearchBox = new List<string>();
        public List<Alert> alertList = new List<Alert>();

        //Database connection        
        dbConnection dbCon = new dbConnection();
        MySqlCommand command;
        MySqlDataReader reader;

        #endregion

        #region constructors
        //Create default constructor
        public AlertController()
        {

        }
        #endregion

        #region alertLists
        //created by Joshua
        public List<Alert> getAllAlerts()
        {
            //query for selecting data from alert
            query = "SELECT id,latitude,longitude,alert,services,dateofcreation FROM alert";
            //bind query
            command = new MySqlCommand(query, dbCon.GetConnection());
            try
            {
                //execute query and store results in the reader
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    //get values with column index
                    int id = (int)reader.GetValue(0);
                    double latitude = (double)reader.GetValue(1);
                    double longitude = (double)reader.GetValue(2);
                    string report = reader.GetValue(3).ToString();
                    string services = reader.GetValue(4).ToString();
                    DateTime date = (DateTime)reader.GetValue(5);
                    //create new user
                    Alert alert = new Alert(id, latitude, longitude, report, services, date);
                    //add user to list
                    alertList.Add(alert);
                }
            }
            catch (MySqlException se)
            {
                Debug.WriteLine(se);
            }
            //Close the databaseconnection
            reader.Close();
            dbCon.CloseConnection();

            return alertList;
        }
        //created by Joshua
        public List<Alert> newAlertList()
        {
            //List that holds all alerts from 4 hours ago untill now.
            List<Alert> newAlertList = new List<Alert>();

            query = "SELECT id, latitude, longitude FROM kbs2rob.alert WHERE dateofcreation >= DATE_SUB(NOW(), INTERVAL 4 HOUR)  AND longitude >= 1 && latitude >=1";
            //bind query
            command = new MySqlCommand(query, dbCon.GetConnection());
            try
            {
                //execute query and store results in the reader                
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    //get values with column index
                    int id = (int)reader.GetValue(0);
                    double latitude = (double)reader.GetValue(1);
                    double longitude = (double)reader.GetValue(2);
                    //create new user
                    Alert alert = new Alert(id, latitude, longitude);
                    //add user to list
                    newAlertList.Add(alert);
                }
            }
            catch (MySqlException se)
            {
                Debug.WriteLine(se);
            }
            //Close the databaseconnection
            reader.Close();
            dbCon.CloseConnection();

            return newAlertList;
        }
        //created by Joshua
        //Alertlist with alerts from past day
        public List<Alert> alertListPastDay()
        {
            //init list to hold all alerts of the past day
            List<Alert> alertListPastDay = new List<Alert>();
            //query that selects data from last 24 hours
            query = "SELECT id, latitude, longitude, IsRead FROM alert WHERE dateofcreation >= DATE_SUB(CURDATE(), INTERVAL 24 HOUR) AND longitude >= 1 && latitude >=1; ";

            command = new MySqlCommand(query, dbCon.GetConnection());

            try
            {
                //execute query and store results in the reader                                
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    //get values with column index
                    int id = (int)reader.GetValue(0);
                    double latitude = (double)reader.GetValue(1);
                    double longitude = (double)reader.GetValue(2);
                    //create new user
                    Alert alert = new Alert(id, latitude, longitude);
                    alert.IsRead = (int)reader.GetValue(3);
                    //add user to list
                    alertListPastDay.Add(alert);
                }
            }
            catch (MySqlException se)
            {
                Debug.WriteLine(se);
            }
            //Close the databaseconnection
            reader.Close();
            dbCon.CloseConnection();

            return alertListPastDay;
        }
        //created by Joshua
        public void updateAlerts()
        {
            //query for copying data from alert to alerthistory
            query = "INSERT INTO alerthistory(id,latitude,longitude,alert,services,title,adres,dateofcreation,isRead) SELECT id,latitude,longitude,alert,services,title,adres,dateofcreation,IsRead FROM alert WHERE dateofcreation < CURDATE()";
            command = new MySqlCommand(query, dbCon.GetConnection());

            try
            {
                //execute the reader
                reader = command.ExecuteReader();
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine(ex);
            }
            //Close the databaseconnection
            reader.Close();
            dbCon.CloseConnection();

        }
        //created by Joshua
        public void moveAlertToHistoryById(int id)
        {
            //query for copying data from alert to alerthistory
            try
            {
                command = dbCon.GetConnection().CreateCommand();
                command.Connection = dbCon.GetConnection();
                command.CommandText = "INSERT INTO alerthistory(id,latitude,longitude,alert,services,title,adres,dateofcreation,isRead) SELECT id,latitude,longitude,alert,services,title,adres,dateofcreation,IsRead FROM alert WHERE id =" + id;
                //execute the reader
                command.ExecuteNonQuery();

                command.CommandText = "DELETE FROM alert WHERE id =" + id;
                command.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine(ex);
            }
            //Close the databaseconnection
            dbCon.CloseConnection();
        }
        //created by Joshua
        public void deleteAlerts()
        {
            //query for deleting moved data from database
            query = "DELETE FROM alert WHERE dateofcreation < CURDATE()";
            command = new MySqlCommand(query, dbCon.GetConnection());

            try
            {
                //execute the reader
                reader = command.ExecuteReader();
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine(ex);
            }
            updatedAlertTable = true;
            //Close the databaseconnection
            reader.Close();
            dbCon.CloseConnection();
        }
        //created by Joshua
        public Alert getAlertById(int id)
        {
            //query for selecting data from alert by specific id
            query = "SELECT longitude, latitude, dateofcreation, alert, services, id FROM alert WHERE id=" + id;
            command = new MySqlCommand(query, dbCon.GetConnection());
            reader = command.ExecuteReader();

            //create new bindable alert
            Alert alert = new Alert(0, 0d, 0d);

            try
            {
                //loop throug query results and bind data
                while (reader.Read())
                {
                    double longitude = (double)reader.GetValue(0);
                    double latitude = (double)reader.GetValue(1);
                    DateTime datetime = (DateTime)reader.GetValue(2);
                    string message = (string)reader.GetValue(3);
                    string services = (string)reader.GetValue(4);
                    int idRead = (int)reader.GetValue(5);

                    //bind data to new alert
                    alert = new Alert(idRead, longitude, latitude, message, services, datetime);
                    alert.services = services;

                    return alert;
                }
            }
            catch
            {

            }
            //Close the databaseconnection
            reader.Close();
            dbCon.CloseConnection();

            return alert;
        }

        public List<Alert> getAllAlertsFeed()
        {
            //list of all available alerts
            List<Alert> list = new List<Alert>();
            //query for selecting data from past 4 hours
            query = "SELECT longitude, latitude, dateofcreation, alert, services, id FROM alert WHERE dateofcreation >= NOW() - interval 4 HOUR";
            command = new MySqlCommand(query, dbCon.GetConnection());
            reader = command.ExecuteReader();
            //default alert for binding
            string services;
            Alert alert = new Alert(0, 0d, 0d);
            try
            {
                //looping through reader alerts
                while (reader.Read())
                {
                    //bind data to attributes
                    double longitude = (double)reader.GetValue(0);
                    double latitude = (double)reader.GetValue(1);
                    DateTime datetime = (DateTime)reader.GetValue(2);
                    string message = (string)reader.GetValue(3);

                    services = (string)reader.GetValue(4);
                    int id = (int)reader.GetValue(5);

                    //bind data to alert
                    alert = new Alert(message, longitude, latitude, datetime, services);
                    //add alert to list
                    list.Add(alert);
                }
            }
            catch (InvalidCastException)
            {
                //catch if reader returns a null value
                if (reader.IsDBNull(4))
                {
                    services = "Onbekend";
                }
            }
            //close connection
            reader.Close();
            dbCon.CloseConnection();

            return list;
        }
        #endregion

        #region alertId's
        //method for getting the ammount of id's in alert
        public int getIdCount()
        {
            //query for selectin the amount of id's in alert
            query = "SELECT COUNT(id) FROM alert";
            command = new MySqlCommand(query, dbCon.GetConnection());

            try
            {
                reader = command.ExecuteReader();
                //loop through results
                while (reader.Read())
                {
                    //get values with column index
                    this.idCount = (int)reader.GetInt32(0);
                }
            }
            catch (MySqlException se)
            {
                Debug.WriteLine(se);
            }
            //Close the databaseconnection
            reader.Close();
            dbCon.CloseConnection();

            return idCount;
        }

        //method for getting a list of all the id's in alert
        public List<int> getAlertId()
        {
            query = "SELECT id FROM alert";

            command = new MySqlCommand(query, dbCon.GetConnection());
            try
            {
                //execute the query
                reader = command.ExecuteReader();
                //loop through results
                while (reader.Read())
                {
                    for (int i = 1; i <= idCount; i++)
                    {
                        idList.Add(reader.GetInt32(0));
                    }
                }
            }
            catch (MySqlException se)
            {
                Debug.WriteLine(se);
            }
            reader.Close();
            dbCon.CloseConnection();

            return idList;
        }
        #endregion

        #region searchboxHandling
        public void addSearchedAlertsToList()
        {
            //query for searching through alerthistory           
            query = "SELECT title FROM alerthistory WHERE title LIKE '%" + MainUserControl.searchWord + "%'";
            command = new MySqlCommand(query, dbCon.GetConnection());

            try
            {
                //execute query
                reader = command.ExecuteReader();
                //loop through results
                while (reader.Read())
                {
                    //get values with column index                    
                    string title = (string)reader.GetValue(0);
                    //add values to list
                    alertListSearchBox.Add(title);
                }
            }
            catch (MySqlException se)
            {
                Debug.WriteLine(se);
            }
            //Close the databaseconnection
            reader.Close();
            dbCon.CloseConnection();
        }

        public void purgeSearchedList()
        {
            //clear the list for resetting the titlebox
            alertListSearchBox.Clear();
        }

        public List<string> getAlertTitles()
        {
            string title;
            //query for getting all titles from alerthistory
            query = "SELECT title FROM alerthistory WHERE title IS NOT NULL";
            command = new MySqlCommand(query, dbCon.GetConnection());
            try
            {
                reader = command.ExecuteReader();
                //loop through results
                while (reader.Read())
                {
                    title = (string)reader.GetValue(0);
                    alertTitles.Add(title);
                }
            }
            catch (MySqlException se)
            {
                Debug.WriteLine(se);
            }
            //Close the databaseconnection
            reader.Close();
            dbCon.CloseConnection();

            return alertTitles;
        }
        //method for getting the report from alert associated with the selected id
        public void addTextToBox()
        {
            query = "SELECT id, alert, services FROM alerthistory WHERE title = '" + MainUserControl.selectedTitle + "'";
            command = new MySqlCommand(query, dbCon.GetConnection());

            try
            {
                reader = command.ExecuteReader();
                //loop through results
                while (reader.Read())
                {
                    //get values with column index
                    this.id = (int)reader.GetValue(0);
                    this.alert = (string)reader.GetValue(1);
                    this.service = (string)reader.GetValue(2);
                }
            }
            catch (InvalidCastException ex)
            {
                //check if reader returns null values             
                Debug.WriteLine("Error: " + ex);
                if (reader.IsDBNull(0))
                {
                    this.id = 0;
                }
                if (reader.IsDBNull(1))
                {
                    this.alert = "Onbekend";
                }
                if (reader.IsDBNull(2))
                {
                    this.service = "Niet bekend";
                }
            }
            catch (MySqlException se)
            {
                Debug.WriteLine(se);
            }
            //Close the databaseconnection
            reader.Close();
            dbCon.CloseConnection();
        }
        #endregion

    }
}
