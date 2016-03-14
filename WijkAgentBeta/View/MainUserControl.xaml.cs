using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WijkAgentBeta.ContentHandling;
using WijkAgentBeta.Models;

namespace WijkAgentBeta
{
    /// <summary>
    /// Interaction logic for MainUserControl.xaml
    /// </summary>
    public partial class MainUserControl : UserControl
    {
        /*
          Author: Rob Nibourg
          Modified by: Rob Nibourg & Simon Brink & Joshua van Gelder

          Explain the main thing this class does.
         */
        #region Attributes
        ChatController chat = new ChatController();
        string chatSessionWith;

        UserController user = new UserController();

        private Timer receiveTimer = new Timer();

        AlertController alertcontroller = new AlertController();
        public static int selectedId;
        public static string searchWord;
        public static string selectedTitle;

        // Chatmessages receiving..
        ChatMessage newMessage;
        ChatMessage oldMessage;

        public Timer twitterTimer = new Timer();
        public Timer feedTimer = new Timer();
        #endregion

        #region constructor
        public MainUserControl()
        {
            InitializeComponent();

            // Timer for receiving messages.
            receiveTimer = new Timer();
            receiveTimer.Enabled = true;
            receiveTimer.Interval = 10000;
            receiveTimer.Elapsed += new ElapsedEventHandler(receiveTimer_Tick);
            receiveTimer.Start();

            twitterTimer.Enabled = true;
            twitterTimer.Interval = 2000;
            twitterTimer.Elapsed += new ElapsedEventHandler(twitterTimer_Tick);
            twitterTimer.Start();

            feedTimer.Enabled = true;
            feedTimer.Interval = 10000;
            feedTimer.Elapsed += new ElapsedEventHandler(feedTimer_Tick);
            feedTimer.Start();

            //getComboboxItem();
            addListToComboBox();

        }


        #endregion

        // This function makes a window for the settings.
        public static bool WindowOpen;

        #region WPF Form item handling
        private void Settings(object sender, RoutedEventArgs e)
        {
            if (WindowOpen == false)
            {
                View.SettingsWindow settings = new View.SettingsWindow();
                settings.Show();
                WindowOpen = true;
            }
        }

        #endregion

        #region ChatSystem

        // This function receives all the message that have been sended by officers. If there is a chat session with an officer it runs the functions. Otherwise it doesn't.
        private void receiveTimer_Tick(object sender, ElapsedEventArgs e)
        {
            try
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    // If there is an session and there is a new message run code, otherwise do nothing.
                    if (chatSessionWith != null)
                    {
                        newMessage = chat.receiveMessage(AuthenticationController.loggedInUser.id);
                        if (newMessage != null)
                        {
                            // If there is no old message yet, add the newest message to the messagebox.
                            if (oldMessage == null)
                            {
                                messageBox.Items.Add(user.getUserById(newMessage.from).name + " zegt: " + newMessage.messageText);
                                chat.setReceived(newMessage);
                                oldMessage = newMessage;
                            }
                            // If newMessage is not the same as the last message add the newmessage to the messagebox.
                            else if (newMessage.messageText != oldMessage.messageText)
                            {
                                messageBox.Items.Add(user.getUserById(newMessage.from).name + " zegt: " + newMessage.messageText);
                                chat.setReceived(newMessage);
                                oldMessage = newMessage;
                            }
                        }

                        messageBox.SelectedIndex = messageBox.Items.Count - 1;
                        messageBox.ScrollIntoView(messageBox.SelectedItem);

                    }

                    // If an user is logged in run through this functie.
                    if (AuthenticationController.loggedInUser != null)
                    {
                        // For each user that is online and available add it to the online list.
                        onlineBox.Items.Clear();
                        foreach (User user1 in user.getAllAvailableUsers())
                        {
                            if (user1.id != AuthenticationController.loggedInUser.id && !onlineBox.Items.Contains(user1.name))
                            {
                                onlineBox.Items.Add(user1.name);
                            }
                        }
                    }
                }));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }


        }
        // Send a message to the person you are chatting with at the moment. If you're not chatting with someone it will display an error message.
        private void Send_Copy_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(inputBox.Text) && chatSessionWith != null)
            {

                User userSendTo = user.getUserByName(chatSessionWith);
                chat.sendMessage(AuthenticationController.loggedInUser.id, userSendTo.id, this.inputBox.Text);
                messageBox.Items.Add(AuthenticationController.loggedInUser.name + " zegt: " + this.inputBox.Text);
                inputBox.Clear();
            }
            else
            {
                MessageBox.Show("Geen bericht of je bent met niemand aan het chatten!", "Oops");
            }

            messageBox.SelectedIndex = messageBox.Items.Count - 1;
            messageBox.ScrollIntoView(messageBox.SelectedItem);
        }

        private void onlineBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (chatSessionWith == null)
                {
                    chatSessionWith = onlineBox.SelectedItem.ToString();
                    messageBox.Items.Clear();
                    messageBox.Items.Add("Je bent nu aan het chatten met: " + chatSessionWith + " berichten die onderling worden verzonden worden opgeslagen in de database.");
                }
                else
                {
                    MessageBox.Show("Je chat nu met " + chatSessionWith + " weet je het zeker dat je wilt chatten met " + onlineBox.SelectedItem.ToString() + "? ", "Melding",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning);

                    if (MessageBox.Show("Je chat nu met " + chatSessionWith + " weet je het zeker dat je wilt chatten met " + onlineBox.SelectedItem.ToString() + "? ", "Melding",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        chatSessionWith = onlineBox.SelectedItem.ToString();
                        messageBox.Items.Clear();
                        messageBox.Items.Add("Je bent nu aan het chatten met: " + chatSessionWith + " berichten die onderling worden verzonden worden opgeslagen in de database.");
                    }
                }


                foreach (ChatMessage message in chat.getChatLog(user.getUserByName(chatSessionWith).id, AuthenticationController.loggedInUser.id))
                {
                    messageBox.Items.Add(user.getUserById(message.from).name + " zegt: " + message.messageText);
                }
                //Console.WriteLine("Chatlog received.");

            }
            catch (NullReferenceException ex)
            {
                Debug.WriteLine(ex);
            }

        }

        // Send_Click is clear button, it clears the inputBox
        private void Send_Click(object sender, RoutedEventArgs e)
        {
            inputBox.Clear();
        }

        private void idComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            getComboboxItem();
            alertcontroller.addTextToBox();
            //Add text to boxes
            idTextBox.Text = alertcontroller.id.ToString();
            serviceTextBox.Text = alertcontroller.service;
            meldingTextBox.Text = alertcontroller.alert;
        }
        #endregion

        #region tweet Log

        public TwitterStream twitterStream = new TwitterStream();
        public TweetController tweetController = new TweetController();

        // Whenever the user clicks the button it add's an word to the database searchwords.
        private void AddNewSearchWord_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(newSearchWord.Text))
            {
                if (tweetController.checkIfSearchWordExist(newSearchWord.Text) == false)
                {
                    tweetController.saveSearchWord(newSearchWord.Text);
                    MessageBox.Show("Zoek woord toegevoegd!", "Succes");
                    newSearchWord.Clear();
                }
            }
            else
            {
                MessageBox.Show("Geen woord ingevuld!", "Oops");
            }
        }


        // Delete searchWord.
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(deleteSearchWord.Text))
            {
                // If searchword exists in the databae delete the word.
                if (tweetController.checkIfSearchWordExist(deleteSearchWord.Text) == true)
                {
                    // delete word in database.
                    tweetController.deleteSearchWord(deleteSearchWord.Text);
                    // delete word in list.
                    searchWordsListNonActive.Items.Remove(deleteSearchWord.Text);

                    //Show notification for user
                    MessageBox.Show("Zoek woord verwijderd!", "Succes");
                }
                else
                {
                    // If searchword does not exists.
                    MessageBox.Show("Woord bestaat niet!", "Oops");
                }
            }
            else
            {
                MessageBox.Show("Geen woord ingevuld!", "Oops");
            }
        }


        // Twitter timer. checks continuously if there is a new searchword or a change in active list or inactive list.
        private void twitterTimer_Tick(object sender, ElapsedEventArgs e)
        {
            try
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    // check if there are changes in active words.
                    foreach (string word in tweetController.getSearchWords(1))
                    {
                        if (!searchWordsListActive.Items.Contains(word))
                        {
                            searchWordsListActive.Items.Add(word);

                        }
                    }

                    // check if there are changes in inactive words.
                    foreach (string word in tweetController.getSearchWords(0))
                    {
                        if (!searchWordsListNonActive.Items.Contains(word))
                        {
                            searchWordsListNonActive.Items.Add(word);
                        }

                    }
                    // latest tweet display.
                    if (TwitterStream.tweetMessage != null)
                    {
                        latestTweet.Text = TwitterStream.tweetMessage.ToString();
                    }
                }));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }


        }

        // if selection has changed.
        private void searchWordsListNonActive_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                string word = searchWordsListNonActive.SelectedItem.ToString();
                tweetController.changeSearchWord(1, word);
                searchWordsListNonActive.Items.Remove(word);

            }
            catch (Exception Ex)
            {
                Debug.WriteLine(Ex);
            }

        }

        // if selection has changed.
        private void searchWordsListActive_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                string word = searchWordsListActive.SelectedItem.ToString();
                tweetController.changeSearchWord(0, word);
                searchWordsListActive.Items.Remove(word);

            }
            catch (Exception Ex)
            {
                Debug.WriteLine(Ex);
            }
        }
        #endregion

        #region SearchBox 
        //Made by Joshua van Gelder
        private void searchBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (string.IsNullOrEmpty(searchTextBox.Text))
                {
                    purgeCombobox();
                    alertcontroller.purgeSearchedList();
                    addListToComboBox();
                }
                else
                {
                    searchWord = searchTextBox.Text.ToString();
                    purgeCombobox();
                    alertcontroller.purgeSearchedList();
                    addSearchedListToCombobox();
                }
            }
        }
        #endregion

        #region combobox Modifiers
        //Made by Joshua van Gelder
        //method for adding created list to combobox
        public void addListToComboBox()
        {
            alertcontroller.getAlertTitles();
            foreach (string title in alertcontroller.alertTitles)
            {
                titleComboBox.Items.Add(title);
            }
        }

        public void addSearchedListToCombobox()
        {
            alertcontroller.addSearchedAlertsToList();
            foreach (string title in alertcontroller.alertListSearchBox)
            {
                titleComboBox.Items.Add(title);
            }
        }

        public void purgeCombobox()
        {
            titleComboBox.Items.Clear();
        }

        public void getComboboxItem()
        {
            if (string.IsNullOrEmpty(titleComboBox.Text))
            {
                selectedTitle = titleComboBox.SelectedItem.ToString();
            }
            else
            {
                purgeCombobox();
            }
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(searchTextBox.Text))
            {
                searchEmptyLabel.Visibility = Visibility.Visible;
            }
            else
            {
                searchEmptyLabel.Visibility = Visibility.Hidden;
                if (string.IsNullOrEmpty(searchTextBox.Text))
                {
                    purgeCombobox();
                    alertcontroller.purgeSearchedList();
                    addListToComboBox();
                }
                else
                {
                    searchWord = searchTextBox.Text.ToString();
                    purgeCombobox();
                    alertcontroller.purgeSearchedList();
                    addSearchedListToCombobox();
                }

            }
        }
        #endregion

        #region clickmodifiers
        private void Panic_button_Click(object sender, RoutedEventArgs e)
        {
            PanicController Panic = new PanicController();
            Panic.Start_panic();
        }

        private void UserPinCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (UserPinCheckBox.IsChecked.Value)
            {
                MapController.userPinsActive = true;
            }
            else
            {
                MapController.userPinsActive = false;
            }
        }

        private void AlertPinCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (AlertPinCheckBox.IsChecked.Value)
            {
                MapController.alertPinsActive = true;
            }
            else
            {
                MapController.alertPinsActive = false;
            }
        }

        private void TwitterPinCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (AlertPinCheckBox.IsChecked.Value)
            {
                MapController.twitterPinsActive = true;
            }
            else
            {
                MapController.twitterPinsActive = false;
            }
        }
        #endregion

        #region Twitter Feed and Alerts Feed

        FeedController feedController = new FeedController();

        List<TweetMessage> feedTweets;
        List<String> oldfeedTweets = new List<String>();

        List<Alert> feedAlert;
        List<string> oldfeedAlert = new List<string>();

        // This timer checks frequently if there are new tweets or alerts on the map placed. If there are new items, they will be added to the feed.
        private void feedTimer_Tick(object sender, ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                feedTweets = feedController.tweetFeed(); // Get tweets
                feedAlert = feedController.alertFeed(); // Get alerts

                try
                {
                    // If current tweet message is new, add it to the list. Else do nothing.
                    foreach (TweetMessage tweetmessage in feedTweets)
                    {
                        if (oldfeedTweets != null)
                        {
                            bool alreadyAdded = oldfeedTweets.Contains(tweetmessage.message);

                            if (alreadyAdded == false)
                            {
                                oldfeedTweets.Add(tweetmessage.message);
                                twitterFeed.Items.Add(tweetmessage);
                            }

                        }
                        else
                        {
                            oldfeedTweets.Add(tweetmessage.message);
                            twitterFeed.Items.Add(tweetmessage);

                        }

                    }
                    // If current alert is new, add it to the list. Else do nothing.
                    foreach (Alert alert in feedAlert)
                    {

                        if (oldfeedAlert != null)
                        {
                            bool alreadyAdded = oldfeedAlert.Contains(alert.report);

                            if (alreadyAdded == false)
                            {
                                oldfeedAlert.Add(alert.report);
                                alertFeed.Items.Add("Services: " + alert.services + "\n Bericht: " + alert.report);
                            }
                        }
                        else
                        {
                            oldfeedAlert.Add(alert.report);
                            alertFeed.Items.Add("Services: " + alert.services + "\n Bericht: " + alert.report);
                        }
                    }

                }
                // Catch any errors that we get.
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                // Scroll automatically to the newest item in the list for alerts.
                alertFeed.SelectedIndex = twitterFeed.Items.Count - 1;
                alertFeed.ScrollIntoView(twitterFeed.SelectedItem);

                // Scroll automatically to the newest item in the list for tweets.
                twitterFeed.SelectedIndex = twitterFeed.Items.Count - 1;
                twitterFeed.ScrollIntoView(twitterFeed.SelectedItem);

            }));
        }


        #endregion
    }
}
