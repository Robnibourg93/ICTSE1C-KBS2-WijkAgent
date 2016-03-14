using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WijkAgentBeta.Database;
using WijkAgentBeta.Models;

namespace WijkAgentBeta.ContentHandling
{

    class PanicController
    {
        /*
            Author: Ronne Timmerman
            Modified: Ronne Timmerman
            Description: handles the event when the panic button is clicked


        */
        public Panic panicModel;
        MySqlCommand command { get; set; }
        private dbConnection dbConnection = new dbConnection();
        private string query;
        private MySqlDataReader reader;

        Random random = new Random();
        int randomnum;

        public PanicController()
        {

        }

        //gets the information needed and saves it to the datebase
        public void Start_panic()
        {
            Console.WriteLine("panic");
            double latitude = AuthenticationController.loggedInUser.latitude;
            double longitude = AuthenticationController.loggedInUser.longitude;
            int userid = AuthenticationController.loggedInUser.id;

            this.randomnum = this.random.Next(1000000000);

            try
            {

                query = "INSERT INTO `panic` (`id`, `randomnum`, `datetime`, `latitude`, `longitude`, `active`, `userid`) VALUES (NULL, @randomnum, @datetime, @latitude, @longitude, @active, @userid)";
                //Console.WriteLine(query);
                command = dbConnection.GetConnection().CreateCommand();
                command.CommandText = query;
                command.Connection = dbConnection.GetConnection();
                command.Parameters.AddWithValue("@randomnum", this.randomnum);
                command.Parameters.AddWithValue("@datetime", DateTime.Now);
                command.Parameters.AddWithValue("@latitude", latitude);
                command.Parameters.AddWithValue("@longitude", longitude);
                command.Parameters.AddWithValue("@active", true);
                command.Parameters.AddWithValue("@userid", userid);
                command.ExecuteNonQuery();

            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            Thread SetToNonActiveThread = new Thread(setToNotActive);
            SetToNonActiveThread.IsBackground = true;
            SetToNonActiveThread.Start();
        }


        //after 30 seconds set situation to normal
        private void setToNotActive()
        {
            try
            {
                Thread.Sleep(30000);

                this.query = "UPDATE `panic` SET `active`=@active WHERE `randomnum`=@randomnum";
                Console.WriteLine(query);
                command = dbConnection.GetConnection().CreateCommand();
                command.CommandText = query;
                command.Connection = dbConnection.GetConnection();
                command.Parameters.AddWithValue("@randomnum", this.randomnum);
                command.Parameters.AddWithValue("@active", false);
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        //check if panic button is clicked if so start panic sequence.
        public Boolean checkPanic()
        {
            double longitude = 0;
            double latitude = 0;
            int userid = 0;
            int active = 0;
            DateTime datetime = Convert.ToDateTime("01-01-1995 12:12:12");


            this.query = "select * from panic Order by datetime desc limit 1";

            Console.WriteLine(query);

            command = new MySqlCommand(query, dbConnection.GetConnection());
            reader = command.ExecuteReader();

            while (reader.Read())
            {
                longitude = (double)reader.GetValue(4);
                latitude = (double)reader.GetValue(3);
                userid = (int)reader.GetValue(6);
                active = (int)reader.GetValue(5);
                datetime = (DateTime)reader.GetValue(2);

                //Console.WriteLine(datetime + " " + longitude + " " + latitude);
            }

            if (active == 1)
            {
                panicModel = new Panic(datetime, longitude, latitude, userid);
                return true;
            }
            else
            {
                return false;
            }
        }


    }
}
