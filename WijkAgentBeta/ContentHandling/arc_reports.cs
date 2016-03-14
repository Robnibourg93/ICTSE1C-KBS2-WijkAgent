using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WijkAgentBeta.Database;
using MySql.Data.MySqlClient;
using System.Diagnostics;

namespace WijkAgentBeta.ContentHandling
{


    class arc_reports
    {
        /*
            Author: Ronne Timmerman
            Modified: Ronne Timmerman
            Description: //handels the format of a arc_report, that comes from the P2000 RSSfeed.
                        //it arranges the data in good format

        */

        public string title { get; set; }
        public string description { get; set; }
        public string pubDate { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string service { get; set; }
        public string adres { get; set; }
        public DateTime DateTime { get; set; }
        int count;
        string query;

        MySqlDataReader reader;
        MySqlCommand command { get; set; }
        private dbConnection dbConnection = new dbConnection();

        Boolean excists;

        public arc_reports(string title, string description, string pubDate, double latitude, double longitude)
        {
            this.title = title;
            this.description = description;
            this.pubDate = pubDate;
            this.latitude = latitude;
            this.longitude = longitude;

            arrangeData();

        }


        public void printinfo()
        {
            Console.WriteLine(title);
            Console.WriteLine(description);
            Console.WriteLine(pubDate);
            Console.WriteLine(latitude);
            Console.WriteLine(longitude);

        }

        //arange data, to avoid reading and saving problems
        private void arrangeData()
        {
            //split description string
            string[] descriptionSplit = this.description.Split(' ');

            Boolean found = false;
            string result = "";

            //the following foreach loop does the following:
            //search for the "(" 
            //save the data before the "(" sign into this.adres
            //save the first word after "(" into this.service
            foreach (string word in descriptionSplit)
            {
                //if found save the data and leave foreach loop

                //point A
                if (found == true)
                {
                    this.service = word;
                    found = false;
                    this.adres = result;


                    break;
                }
                else
                {
                    int x = 0;
                    //if word is not "(" expand result (this is only data before the "(" sign
                    if (word != "(")
                    {

                        result = result + " " + word;
                        x++;
                    }
                    else
                    {
                        //if "(" is found in array, then stop searching, save nothing and go to point A in foreach loop
                        found = true;
                        this.adres = result;
                    }
                }
            }

            //arange pubDate to DateTime Format
            string[] pubDateSplit = this.pubDate.Split(' ');

            this.pubDate = pubDateSplit[1] + " " + pubDateSplit[2] + " " + pubDateSplit[3] + " " + pubDateSplit[4];

            this.DateTime = Convert.ToDateTime(this.pubDate);
        }

        //save a arc report to the database if non excisting.
        public void saveToDataBase()
        {
            dbConnection DB = new dbConnection();

            if (DB.IsConnect())
            {
                //create query to check if excists
                string query = "SELECT COUNT(*) FROM `alert` WHERE `dateofcreation` = '" + this.DateTime + "' OR `adres` ='" + this.adres + "'";

                try
                {
                    //start db conection, check if excists
                    command = new MySqlCommand(query, dbConnection.GetConnection());
                    reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        this.count = reader.GetInt32(0);
                    }


                    //if database returns 0 the report does not excists
                    if (this.count == 0)
                    {
                        this.excists = false;
                    }
                    else
                    {
                        this.excists = true;
                    }

                    reader.Close();
                    DB.CloseConnection();
                }
                catch (MySqlException se)
                {
                    Debug.Write(se);
                }

                //if non excisting and there is a latitude and longitude, store to DB
                if (this.excists == false)
                {
                    if (this.latitude != 0 || this.longitude != 0)
                    {
                        try
                        {

                            query = "INSERT INTO `alert` (`id`, `latitude`, `longitude`, `alert`, `services`, `title`, `adres`, `dateofcreation`) VALUES (NULL, @latitutde, @longitude, @alert, @services, @title, @adres, @dateofcreation)";

                            command = dbConnection.GetConnection().CreateCommand();
                            command.CommandText = query;
                            command.Connection = dbConnection.GetConnection();
                            command.Parameters.AddWithValue("@latitutde", this.latitude);
                            command.Parameters.AddWithValue("@longitude", this.longitude);
                            command.Parameters.AddWithValue("@alert", this.title);
                            command.Parameters.AddWithValue("@services", this.service);
                            command.Parameters.AddWithValue("@title", this.title);
                            command.Parameters.AddWithValue("@adres", this.adres);
                            command.Parameters.AddWithValue("@dateofcreation", this.DateTime);

                            command.ExecuteNonQuery();

                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine(e);
                        }
                    }
                }


            }
            DB.CloseConnection();
        }

    }
}
