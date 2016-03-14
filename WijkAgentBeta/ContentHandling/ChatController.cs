using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using WijkAgentBeta.Database;

namespace WijkAgentBeta.ContentHandling
{
    public class ChatController
    {
        /*
            Author: S1079813 - Simon Brink
            Modified by: Simon Brink

            Description: This class does all the functionality for the chatbox. It sends and receives messages and saves them to the database.

        */
        public static List<User> usersOnline = new List<User>();
        public UserController userController = new UserController();

        // Mysql needed for executing querys
        private dbConnection dbConnection = new dbConnection();
        private string query { get; set; }
        private MySqlCommand command { get; set; }
        private MySqlDataReader reader { get; set; }


        // Needed for sendMessage()
        public int from;
        public int to;
        public DateTime sendDate = DateTime.Now;

        public ChatMessage newMessage;

        public void sendMessage(int from, int to, string message)
        {
            try
            {
                query = "INSERT INTO message (`sender`, `receiver`, `message`, `sendDate`) VALUES (@sender, @receiver, @message, @sendDate)";

                command = dbConnection.GetConnection().CreateCommand();
                command.CommandText = query;
                command.Connection = dbConnection.GetConnection();
                command.Parameters.AddWithValue("@sender", from);
                command.Parameters.AddWithValue("@receiver", to);
                command.Parameters.AddWithValue("@message", message);
                command.Parameters.AddWithValue("@sendDate", sendDate);

                command.ExecuteNonQuery();
            } catch (Exception e )
            {
                Debug.WriteLine(e);
            }
            dbConnection.CloseConnection();
        }

        public ChatMessage receiveMessage(int to)
        {

            query = "SELECT * FROM message WHERE `receiver` = " + to + " AND received = 0 GROUP BY sendDate DESC LIMIT 0, 1";
            command = new MySqlCommand(query, dbConnection.GetConnection());
            reader = command.ExecuteReader();

            while (reader.Read())
            {
                int id = (int)reader.GetValue(0);
                int messageFrom = (int)reader.GetValue(1);
                int messageTo = (int)reader.GetValue(2);
                string messageText = reader.GetValue(3).ToString();
                DateTime sendDate = (DateTime)reader.GetValue(4);
                int received = (int)reader.GetValue(5);

                newMessage = new ChatMessage(id, messageFrom, messageTo, messageText, sendDate, received);
            }
            reader.Close();
            dbConnection.CloseConnection();
            return newMessage;
        }

        public void setReceived(ChatMessage message)
        {
            query = "UPDATE message SET received = 1 WHERE id = " + message.id + "";
            command = new MySqlCommand(query, dbConnection.GetConnection());
            reader = command.ExecuteReader();
            reader.Close();
            dbConnection.CloseConnection();
        }

        public List<ChatMessage> getChatLog (int to, int from)
        {
            List<ChatMessage> chatLog = new List<ChatMessage>();

            try
            {
                query = "SELECT * FROM message WHERE `receiver` = " + to + " AND `sender` = " + from + " OR receiver = " + from + " AND sender = " + to + " AND received = 1 AND sendDate >= CURDATE() - INTERVAL 1 Day";
                command = new MySqlCommand(query, dbConnection.GetConnection());
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int id = (int)reader.GetValue(0);
                    int messageFrom = (int)reader.GetValue(1);
                    int messageTo = (int)reader.GetValue(2);
                    string message = reader.GetValue(3).ToString();
                    DateTime sendDate = (DateTime)reader.GetValue(4);
                    int received = (int)reader.GetValue(5);
                    chatLog.Add(new ChatMessage(id, messageFrom, messageTo, message, sendDate, received));
                }
            } catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            reader.Close();
            dbConnection.CloseConnection();
            return chatLog;
        }

    }
}
