using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace MyMessenger
{
    public class DatabaseAccess
    {
        static string connectionString = Properties.Settings.Default.connectionString;
        SqlConnection dbConnection = new SqlConnection(connectionString);

        //A method for inserting new users in the database
        public void InsertNewUser()
        {
            //INSERT NEW USER IN THE DATABASE
            Console.WriteLine("======================================================================" +
                            "\n=======================>   NEW USER ACCOUNT   <=======================\n" +
                              "======================================================================\n");

            
            //Asking new user for his details in order to create an account
            Console.Write("Enter a Username: ");
            string newUsername = ValidUsername(Console.ReadLine());

            Console.Write("Enter a Password: ");
            string newPassword = ValidPassword(Console.ReadLine());

            Console.Write("Enter an Email Address: ");
            string newEmail = ValidEmail(Console.ReadLine());

            Console.Write("Enter Firstname: ");
            string newFirstName = ValidName(Console.ReadLine());

            Console.Write("Enter Lastname: ");
            string newLastName = ValidName(Console.ReadLine());

            Console.Write("Enter Age: ");
            string newAge = ValidAge(Console.ReadLine());
            
            
            ////Hashing for securely storing the password in the database
            //string hashedPassword = PasswordHashing.Sha256_hash(newPassword);

            
            //Producing a unique salt for this user to append to his password
            string passwordSalt = PasswordHashing.SaltGenarator();
            //Joining users' input with his salt
            string newUsersPassword = newPassword + passwordSalt;
            //Hashing for securely storing the password in the database
            string hashedPassword = PasswordHashing.Sha256_hash(newUsersPassword);


            //Reading all User Ids from every table in the database and produces new Ids for the new user in each table
            var newUserId = NewIdCreation("UserId", "UserDetails");

            //Saving User Details in the database

            try
            {
                dbConnection.Open();

                var insertNewUserDetails = new SqlCommand("INSERT INTO UserDetails VALUES(@UserId, @U_Username, @U_Password, @Salt, @FirstName, @LastName, @Age, @Email, @JoinedAppOn, @UserRole)", dbConnection);
                insertNewUserDetails.Parameters.AddWithValue("@UserId", newUserId);
                insertNewUserDetails.Parameters.AddWithValue("@U_Username", newUsername);
                insertNewUserDetails.Parameters.AddWithValue("@U_Password", hashedPassword);
                insertNewUserDetails.Parameters.AddWithValue("@Salt", passwordSalt);
                insertNewUserDetails.Parameters.AddWithValue("@FirstName", newFirstName);
                insertNewUserDetails.Parameters.AddWithValue("@LastName", newLastName);
                insertNewUserDetails.Parameters.AddWithValue("@Age", newAge);
                insertNewUserDetails.Parameters.AddWithValue("@Email", newEmail);
                insertNewUserDetails.Parameters.AddWithValue("@JoinedAppOn", DateTime.Now.ToString("d/M/yyyy"));
                insertNewUserDetails.Parameters.AddWithValue("@UserRole", "SimpleUser");
                insertNewUserDetails.ExecuteNonQuery();

                dbConnection.Close();
            }
            catch (SqlException sqlEx)
            {
                SqlError(sqlEx);
            }

        }



        //---------------------------------------------------------------------------------------------------------------------------
        //VALID USER INPUTS

        //Valid Username
        private static string ValidUsername (string username)
        {
            while (true)
            {
                if (DatabaseAdminAccess.CheckUserExistsInDb(username) != 0)
                {
                    Console.Write("\nUser " + username + " already exists.\nPlease enter a different username: ");
                    username = Console.ReadLine();
                }
                if (String.IsNullOrWhiteSpace(username) || username.Contains(" "))
                {
                    Console.Write("\nInvalid input.\nPlease enter a different username: ");
                    username = Console.ReadLine();
                }
                else
                    break;
            }
            return username;
        }


        //Valid Password
        private static string ValidPassword(string password)
        {
            while (true)
            {
                if (password.Length < 8)
                {
                    Console.Write("\nPassword must contain more than 8 characters.\nPlease enter a different password: ");
                    password = Console.ReadLine();
                }
                else if (String.IsNullOrWhiteSpace(password))
                {
                    Console.Write("\nInvalid input.\nPlease enter a different password: ");
                    password = Console.ReadLine();
                }
                else
                    break;
            }
            return password;
        }


        //Valid Email Address
        private static string ValidEmail(string emailInput)
        {
            while (true)
            {
                var checkEmailInDb = new DatabaseAccess();

                if (!(emailInput.Contains("@") && emailInput.Contains(".")) || emailInput.Contains(" "))
                {
                    Console.Write("\nInvalid Email format.\nPlease enter a valid Email Address: ");
                    emailInput = Console.ReadLine();
                }
                else if (checkEmailInDb.CheckEmailAlreadyRegistered(emailInput))
                {
                    Console.Write("\nThis Email is already being used.\nPlease enter a different email address: ");
                    emailInput = Console.ReadLine();
                }
                else
                    break;
            }
            return emailInput;
        }


        //A method for checking that the Email Address provided by the user while creating a new account doesn't exist in the database
        private bool CheckEmailAlreadyRegistered(string emailInput)
        {
            try
            {
                dbConnection.Open();
                var checkExistingEmails = new SqlCommand("SELECT Email FROM UserDetails", dbConnection);

                var emailsList = new List<string>();
                var emailReader = checkExistingEmails.ExecuteReader();

                int i = 0;
                while (emailReader.Read())
                    emailsList.Add(emailReader.GetValue(i).ToString());
                emailReader.Close();
                dbConnection.Close();

                if (emailsList.Contains(emailInput))
                    return true;

                return false;
            }
            catch (SqlException sqlEx)
            {
                SqlError(sqlEx);
                return false;
            }
        }


        //Valid Firstname and Lastname
        private static string ValidName(string name)
        {
            while (true)
            {
                if (String.IsNullOrWhiteSpace(name))
                {
                    Console.Write("\nInvalid input.\nPlease enter a new value: ");
                    name = Console.ReadLine();
                }
                else
                    break;
            }
            return name;
        }


        //Valid Age
        private static string ValidAge(string age)
        {
            while (true)
            {
                int.TryParse(age, out int number);

                if (String.IsNullOrWhiteSpace(age) || age.Contains(" ") || number == 0)
                {
                    Console.Write("\nInvalid input.\nPlease enter a new value: ");
                    age = Console.ReadLine();
                }
                else if (Convert.ToInt32(age) < 15)
                {
                    Console.WriteLine("\nUser must be older than 15.\nProgram will now terminate.");
                    Console.ReadLine();
                    Environment.Exit(0);
                }
                else
                    break;
            }
            return age;
        }
        


        //---------------------------------------------------------------------------------------------------------------------------
        //MAIN MENU OPTIONS

        //Compose a new Message.
        public void NewMessage(int userId, string receiver)
        {
            int newMessageId = NewIdCreation("MessageId", "UserMessages");

            try
            {
                dbConnection.Open();
                var retrieveId = "SELECT UserId FROM UserDetails WHERE U_Username = @receiver";
                var idForThisUser = new SqlCommand(retrieveId, dbConnection);
                idForThisUser.Parameters.AddWithValue("@receiver", receiver);
                var receiversId = idForThisUser.ExecuteScalar();
                dbConnection.Close();

                if (receiversId == null)
                {
                    Console.WriteLine("\nNo user found with this username.\nPress Enter to return to the Main Menu");
                    Console.ReadLine();
                    Console.Clear();
                    ApplicationMenus.MenuOptions(userId);
                }
                if ((int)receiversId == userId)
                {
                    Console.WriteLine("\nYou can't message yourself.\nPress Enter to return to the Main Menu");
                    Console.ReadLine();
                    Console.Clear();
                    ApplicationMenus.MenuOptions(userId);
                }
                else
                {
                    Console.WriteLine("\n(Maximum message length 250 characters)\nType your message and then press Enter to send it\n(or leave empty and press Enter to return to the Main Menu)");
                    var messageContent = Console.ReadLine();
                    if (messageContent.Length > 250)
                    {
                        Console.WriteLine("Message is longer than 250 characters. Please reduce its length.\nPress Enter to return to the Main Menu");
                        Console.ReadLine();
                    }
                    if (String.IsNullOrWhiteSpace(messageContent))
                    {
                        Console.Clear();
                        ApplicationMenus.MenuOptions(userId);
                    }


                    dbConnection.Open();
                    var checkExistingFriends = "SELECT FriendshipId FROM UserFriends " +
                                               "WHERE (User1Id = @userId AND User2Id = @receiversId) OR (User1Id = @receiversId AND User2Id = @userId)";
                    var checkAlreadyAdded = new SqlCommand(checkExistingFriends, dbConnection);
                    checkAlreadyAdded.Parameters.AddWithValue("@userId", userId);
                    checkAlreadyAdded.Parameters.AddWithValue("@receiversId", receiversId);
                    var friendshipFound = checkAlreadyAdded.ExecuteScalar();

                    var sqlQuery = "INSERT INTO UserMessages VALUES ( @MessageId, @MessageContent, @SentOn, @SendersId, @ReceiversId, @UnreadMessage)";
                    var insertNewMessage = new SqlCommand(sqlQuery, dbConnection);
                    insertNewMessage.Parameters.AddWithValue("@MessageId", newMessageId);
                    insertNewMessage.Parameters.AddWithValue("@MessageContent", messageContent);
                    insertNewMessage.Parameters.AddWithValue("@SentOn", DateTime.Now.ToString());
                    insertNewMessage.Parameters.AddWithValue("@SendersId", userId);
                    insertNewMessage.Parameters.AddWithValue("@ReceiversId", receiversId);
                    insertNewMessage.Parameters.AddWithValue("@UnreadMessage", 1);
                    int newMessageAdded = insertNewMessage.ExecuteNonQuery();

                    if (newMessageAdded == 1)
                    {
                        Console.WriteLine("\nMessage sent.\nPress Enter to return to the Main Menu.");
                        Console.ReadLine();
                    }
                    dbConnection.Close();


                    dbConnection.Open();
                    var retrieveUsername = "SELECT U_Username FROM UserDetails " +
                                           "WHERE UserId = @userId";
                    var usernameOfLoggedInUser = new SqlCommand(retrieveUsername, dbConnection);
                    usernameOfLoggedInUser.Parameters.AddWithValue("@userId", userId);
                    var sender = usernameOfLoggedInUser.ExecuteScalar();
                    dbConnection.Close();

                    FilesAccess.MessageToFile((string)sender, receiver, messageContent);
                }
            }
            catch (SqlException sqlEx)
            {
                SqlError(sqlEx);
            }
        }


        //View old Messages.
        public void ViewOldMessages(int userId)
        {
            try
            {
                DataSet ds = null;
                dbConnection.Open();
                var sqlQuery = "SELECT m.SentOn, u.U_Username, m.MessageContent FROM UserMessages m " +
                               "INNER JOIN UserDetails u ON m.SendersId = u.UserId " +
                               "WHERE ReceiversId = @userId AND UnreadMessage = 0 " +
                               "ORDER BY SentOn";
                var retrieveOldMessages = new SqlCommand(sqlQuery, dbConnection);
                retrieveOldMessages.Parameters.AddWithValue("@userId", userId);
                var adapter = new SqlDataAdapter(retrieveOldMessages);
                ds = new DataSet();
                adapter.Fill(ds);
                adapter.Dispose();
                dbConnection.Close();


                var table = ds.Tables[0];
                var tableRows = table.Rows.Count;
                Console.WriteLine($"\n {"No",-5 }{"Sent on",-24}{"Sender",-18}{"Message"}");
                Console.WriteLine($" {"--",-5 }{"-------",-24}{"------",-18}{"-------"}");
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    var row = table.Rows[i];
                    var SentOn = row[0];
                    var U_Username = row[1];
                    var MessageContent = row[2];

                    Console.WriteLine($" {i + 1,-5}{SentOn,-24}{U_Username,-18}{MessageContent}");
                }
                if (tableRows == 0)
                    Console.WriteLine("\nYou have no messages.");
            }
            catch (SqlException sqlEx)
            {
                SqlError(sqlEx);
            }
        }


        //View new Messages.
        public void ViewNewMessages(int userId)
        {
            try
            {
                DataSet ds = null;
                dbConnection.Open();
                var sqlQuery = "SELECT m.SentOn, u.U_Username, m.MessageContent FROM UserMessages m " +
                               "INNER JOIN UserDetails u ON m.SendersId = u.UserId " +
                               "WHERE ReceiversId = @userId AND UnreadMessage = 1";
                var retrieveNewMessages = new SqlCommand(sqlQuery, dbConnection);
                retrieveNewMessages.Parameters.AddWithValue("@userId", userId);
                var adapter = new SqlDataAdapter(retrieveNewMessages);
                ds = new DataSet();
                adapter.Fill(ds);
                adapter.Dispose();
                dbConnection.Close();


                var table = ds.Tables[0];
                var tableRows = table.Rows.Count;
                Console.WriteLine($"\n {"No",-5 }{"Sent on",-24}{"Sender",-18}{"Message"}");
                Console.WriteLine($" {"--",-5 }{"-------",-24}{"------",-18}{"-------"}");
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    var row = table.Rows[i];
                    var SentOn = row[0];
                    var U_Username = row[1];
                    var MessageContent = row[2];

                    Console.WriteLine($" {i + 1,-5}{SentOn,-24}{U_Username,-18}{MessageContent}");
                }
                if (tableRows == 0)
                    Console.WriteLine("\nYou have no new messages.");

                dbConnection.Open();
                var sqlUpdateQuery = "UPDATE UserMessages SET UnreadMessage = 0 " +
                                     "WHERE ReceiversId = @userId AND UnreadMessage = 1";
                var messagesRead = new SqlCommand(sqlUpdateQuery, dbConnection);
                messagesRead.Parameters.AddWithValue("@userId", userId);
                messagesRead.ExecuteNonQuery();
                dbConnection.Close();
            }
            catch (SqlException sqlEx)
            {
                SqlError(sqlEx);
            }
        }
        public int NewMessagesNumber(int userId)
        {
            try
            {
                DataSet ds = null;
                using (dbConnection)
                {
                    var sqlQuery = "SELECT m.SentOn, u.U_Username, m.MessageContent FROM UserMessages m " +
                                   "INNER JOIN UserDetails u ON m.SendersId = u.UserId " +
                                   "WHERE ReceiversId = @userId AND UnreadMessage = 1";
                    var countNewMessages = new SqlCommand(sqlQuery, dbConnection);
                    countNewMessages.Parameters.AddWithValue("@userId", userId);
                    var adapter = new SqlDataAdapter(countNewMessages);
                    ds = new DataSet();
                    adapter.Fill(ds);
                    adapter.Dispose();
                }
                var table = ds.Tables[0];
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    var row = table.Rows[i];
                    var SentOn = row[0];
                    var U_Username = row[1];
                    var MessageContent = row[2];
                }
                return table.Rows.Count;
            }
            catch (SqlException sqlEx)
            {
                SqlError(sqlEx);
                return 0;
            }
        }


        //View sent Messages.
        public void ViewSentMessages(int userId)
        {
            try
            {
                DataSet ds = null;
                dbConnection.Open();
                var sqlQuery = "SELECT m.SentOn, u.U_Username, m.MessageContent FROM UserMessages m " +
                               "INNER JOIN UserDetails u ON m.ReceiversId = u.UserId " +
                               "WHERE SendersId = @userId ORDER BY SentOn";
                var retrieveSentMessages = new SqlCommand(sqlQuery, dbConnection);
                retrieveSentMessages.Parameters.AddWithValue("@userId", userId);
                var adapter = new SqlDataAdapter(retrieveSentMessages);
                ds = new DataSet();
                adapter.Fill(ds);
                adapter.Dispose();
                dbConnection.Close();


                var table = ds.Tables[0];
                var tableRows = table.Rows.Count;
                Console.WriteLine($"\n {"No",-5 }{"Sent on",-24}{"Receiver",-18}{"Message"}");
                Console.WriteLine($" {"--",-5 }{"-------",-24}{"--------",-18}{"-------"}");
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    var row = table.Rows[i];
                    var SentOn = row[0];
                    var U_Username = row[1];
                    var MessageContent = row[2];

                    Console.WriteLine($" {i + 1,-5}{SentOn,-24}{U_Username,-18}{MessageContent}");
                }
                if (tableRows == 0)
                    Console.WriteLine("\nYou have no sent messages.");
            }
            catch (SqlException sqlEx)
            {
                SqlError(sqlEx);
            }
        }


        //View users' friends.
        public void ViewFriends(int loggedInUsersId)
        {
            try
            {
                DataSet ds = null;
                using (dbConnection)
                {
                    var sqlQuery = "(SELECT u.U_Username, u.FirstName, u.LastName, u.Age, u.Email, f.FriendsSince from UserFriends f " +
                                    "INNER JOIN UserDetails u ON f.User2Id = u.UserId " +
                                    "WHERE (f.User1Id = @userId OR f.User2Id = @userId) AND u.UserId != @userId) " +
                                    "UNION " +
                                    "(SELECT u.U_Username, u.FirstName, u.LastName, u.Age, u.Email, f.FriendsSince FROM UserFriends f " +
                                    "INNER JOIN UserDetails u ON f.User1Id = u.UserId " +
                                    "WHERE (f.User1Id = @userId OR f.User2Id = @userId) AND u.UserId != @userId) " +
                                    "ORDER BY FriendsSince";
                    var existingFriends = new SqlCommand(sqlQuery, dbConnection);
                    existingFriends.Parameters.AddWithValue("@userId", loggedInUsersId);
                    var adapter = new SqlDataAdapter(existingFriends);
                    ds = new DataSet();
                    adapter.Fill(ds);
                    adapter.Dispose();
                }

                var table = ds.Tables[0];
                Console.WriteLine($"\n {"No",-5 }{"Username",-18}{"Firstname",-20}{"Lastname",-20}{"Age",-6}{"Email Address",-35}{"Friends Since"}");
                Console.WriteLine($" {"--",-5 }{"--------",-18}{"---------",-20}{"--------",-20}{"---",-6}{"-------------",-35}{"-------------"}");
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    var row = table.Rows[i];
                    var U_Username = row[0];
                    var FirstName = row[1];
                    var LastName = row[2];
                    var Age = row[3];
                    var Email = row[4];
                    var FriendsSince = row[5];

                    Console.WriteLine($" {(i + 1),-5}{U_Username,-18}{FirstName,-20}{LastName,-20}{Age,-6}{Email,-35}{FriendsSince}");
                }
            }
            catch (SqlException sqlEx)
            {
                SqlError(sqlEx);
            }
        }

        
        //Add a friend by his username.
        public void AddFriend(int userId, string userNameToAdd)
        {
            int newFriendshipId = NewIdCreation("FriendshipId", "UserFriends");

            try
            {
                dbConnection.Open();
                var retrieveId = "SELECT UserId FROM UserDetails " +
                                 "WHERE U_Username = @userNameToAdd";
                var idForThisUser = new SqlCommand(retrieveId, dbConnection);
                idForThisUser.Parameters.AddWithValue("@userNameToAdd", userNameToAdd);
                var userToBeAdded = idForThisUser.ExecuteScalar();

                if (userToBeAdded == null)
                {
                    Console.WriteLine("No user found with this username.\nPress Enter to return to the Main Menu");
                    Console.ReadLine();
                    Console.Clear();
                    ApplicationMenus.MenuOptions(userId);
                }

                if (Convert.ToInt32(userToBeAdded) == userId)
                {
                    Console.WriteLine("\nYou can not add yourself in your Friends' List.\nPress Enter to return to the Main Menu");
                    Console.ReadLine();
                    Console.Clear();
                    ApplicationMenus.MenuOptions(userId);
                }

                var checkExistingFriends = "SELECT FriendshipId FROM UserFriends " +
                                          "WHERE (User1Id = @userId AND User2Id = @userToBeAdded) OR (User1Id = @userToBeAdded AND User2Id = @userId)";
                var checkAlreadyAdded = new SqlCommand(checkExistingFriends, dbConnection);
                checkAlreadyAdded.Parameters.AddWithValue("@userId", userId);
                checkAlreadyAdded.Parameters.AddWithValue("@userToBeAdded", userToBeAdded);
                var friendshipsFound = checkAlreadyAdded.ExecuteScalar();

                if (friendshipsFound == null)
                {
                    var sqlQuery = "INSERT INTO UserFriends VALUES (@FriendshipId, @User1Id, @User2Id, @FriendsSince)";
                    var insertNewFriend = new SqlCommand(sqlQuery, dbConnection);
                    insertNewFriend.Parameters.AddWithValue("@FriendshipId", newFriendshipId);
                    insertNewFriend.Parameters.AddWithValue("@User1Id", userId);
                    insertNewFriend.Parameters.AddWithValue("@User2Id", userToBeAdded);
                    insertNewFriend.Parameters.AddWithValue("@FriendsSince", DateTime.Now.ToString("d/M/yyyy"));
                    int newFriendAdded = insertNewFriend.ExecuteNonQuery();

                    dbConnection.Close();
                    if (newFriendAdded == 1)
                    {
                        Console.WriteLine("\nUser " + userNameToAdd + " has been added to your Friends' List.\nPress Enter to return to the Main Menu");
                        Console.ReadLine();
                    }
                }
                else
                {
                    Console.WriteLine("\n" + userNameToAdd + " is already in your Friends' List.\nPress Enter to return to the Main Menu");
                    Console.ReadLine();
                }
            }
            catch (SqlException sqlEx)
            {
                SqlError(sqlEx);
            }
        }


        //Remove a friend.
        public void RemoveFriend(int userId, string friendToBeRemoved)
        {
            try
            {
                dbConnection.Open();
                var retrieveId = "SELECT UserId FROM UserDetails " +
                                 "WHERE U_Username = '" + friendToBeRemoved + "'";
                var idForThisUser = new SqlCommand(retrieveId, dbConnection);
                idForThisUser.Parameters.AddWithValue("@friendToBeRemoved", friendToBeRemoved);
                var friendsIdToBeRemoved = idForThisUser.ExecuteScalar();
                dbConnection.Close();

                if (friendsIdToBeRemoved == null)
                {
                    Console.WriteLine("No friend found with this username.\nPress Enter to return to the Main Menu");
                    Console.ReadLine();
                    Console.Clear();
                    ApplicationMenus.MenuOptions(userId);
                }
                if (Convert.ToInt32(friendsIdToBeRemoved) == userId)
                {
                    Console.WriteLine("\nYou can not remove yourself from your Friends' List.\nPress Enter to return to the Main Menu");
                    Console.ReadLine();
                    Console.Clear();
                    ApplicationMenus.MenuOptions(userId);
                }
                else
                {
                    dbConnection.Open();
                    var sqlQuery = "DELETE FROM UserFriends " +
                                   "WHERE (User1Id = @userId AND User2Id = @friendsIdToBeRemoved) OR (User1Id = @friendsIdToBeRemoved AND User2Id = @userId)";
                    var deleteFriend = new SqlCommand(sqlQuery, dbConnection);
                    deleteFriend.Parameters.AddWithValue("@userId", userId);
                    deleteFriend.Parameters.AddWithValue("@friendsIdToBeRemoved", friendsIdToBeRemoved);
                    int friendDeleted = (int)deleteFriend.ExecuteNonQuery();
                    dbConnection.Close();

                    if (friendDeleted == 1)
                    {
                        Console.WriteLine("\nUser " + friendToBeRemoved + " has been deleted from your Friends' List.\nPress Enter to return to the Main Menu");
                        Console.ReadLine();
                        Console.Clear();
                        ApplicationMenus.MenuOptions(userId);
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                SqlError(sqlEx);
            }
        }


        //Select 5 random users from the database to suggest them as friends.
        public void FriendSuggestions(int userId)
        {
            try
            {
                DataSet ds = null;
                using (dbConnection)
                {
                    dbConnection.Open();
                    var sqlQuery = "SELECT U_Username, FirstName, LastName, Age, Email FROM UserDetails " +
                                   "WHERE UserId NOT IN(SELECT User2Id FROM UserFriends Where User1Id = @userId " +
                                   "UNION " +
                                   "SELECT User1Id From UserFriends Where User2Id = @userId) AND UserId != @userId";
                    var retrieveOtherUsers = new SqlCommand(sqlQuery, dbConnection);
                    retrieveOtherUsers.Parameters.AddWithValue("@userId", userId);
                    var adapter = new SqlDataAdapter(retrieveOtherUsers);
                    ds = new DataSet();
                    adapter.Fill(ds);
                    adapter.Dispose();
                }


                var table = ds.Tables[0];
                Random rndm = new Random();
                var rndmNumbersList = new List<int>();
                Console.WriteLine($"\n {"No",-5 }{"Username",-18}{"Firstname",-20}{"Lastname",-20}{"Age",-6}{"Email Address"}");
                Console.WriteLine($" {"--",-5 }{"--------",-18}{"---------",-20}{"--------",-20}{"---",-6}{"-------------"}");
                for (int i = 0; i < 5; i++)
                {
                    int randomRow = rndm.Next(0, table.Rows.Count - 1);
                    while (rndmNumbersList.Contains(randomRow))
                    {
                        randomRow = rndm.Next(0, table.Rows.Count - 1);
                    }
                    rndmNumbersList.Add(randomRow);
                    var row = table.Rows[randomRow];
                    var U_Username = row[0];
                    var FirstName = row[1];
                    var LastName = row[2];
                    var Age = row[3];
                    var Email = row[4];

                    Console.WriteLine($" {(i + 1),-5}{U_Username,-18}{FirstName,-20}{LastName,-20}{Age,-6}{Email}");
                }
            }
            catch (SqlException sqlEx)
            {
                SqlError(sqlEx);
            }
        }


        //View user details
        public void ViewUserDetails(int userId)
        {
            try
            {
                DataSet ds = null;
                using (dbConnection)
                {
                    var sqlQuery = "SELECT U_Username, FirstName, LastName, Age, Email from UserDetails " +
                                   "WHERE UserId = @userId";
                    var userDetails = new SqlCommand(sqlQuery, dbConnection);
                    userDetails.Parameters.AddWithValue("@userId", userId);
                    var adapter = new SqlDataAdapter(userDetails);
                    ds = new DataSet();
                    adapter.Fill(ds);
                    adapter.Dispose();
                }

                var table = ds.Tables[0];
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    var row = table.Rows[i];
                    var U_Username = row[0];
                    var FirstName = row[1];
                    var LastName = row[2];
                    var Age = row[3];
                    var Email = row[4];

                    Console.WriteLine($"1 Username: {U_Username} \n2 Firstname: {FirstName} \n3 Lastname: {LastName} \n4 Age: {Age} \n5 Email Address: {Email} ");
                }
            }
            catch (SqlException sqlEx)
            {
                SqlError(sqlEx);
            }
        }
        //Change Users' Details
        public void ChangeDetail(int userId, string fieldToChange)
        {
            Console.WriteLine("\nInput new value for this field:\n(or press Enter to return to the Main Menu)");
            var newValue = Console.ReadLine();

            if (String.IsNullOrWhiteSpace(newValue))
            {
                Console.Clear();
                ApplicationMenus.MenuOptions(userId);
            }
            else
            {
                if (fieldToChange == "U_Username")
                {
                    newValue = ValidUsername(newValue);
                }
                if (fieldToChange == "FirstName" || fieldToChange == "LastName")
                {
                    newValue = ValidName(newValue);
                }
                if (fieldToChange == "Age")
                {
                    newValue = ValidAge(newValue);
                }
                if (fieldToChange == "Email")
                {
                    newValue = ValidEmail(newValue);
                }

                try
                {
                    dbConnection.Open();
                    var sqlUpdateQuery = "UPDATE UserDetails SET " + fieldToChange + " = @newValue " +
                                         "WHERE UserId = @userId";
                    var userDetailsRead = new SqlCommand(sqlUpdateQuery, dbConnection);
                    userDetailsRead.Parameters.AddWithValue("@newValue", newValue);
                    userDetailsRead.Parameters.AddWithValue("@userId", userId);
                    var fieldUpdated = userDetailsRead.ExecuteNonQuery();
                    dbConnection.Close();

                    if (fieldUpdated == 1)
                    {
                        Console.WriteLine("\nField updated.\nPress Enter to return to the Menu");
                        Console.ReadLine();
                    }
                }
                catch (SqlException sqlEx)
                {
                    SqlError(sqlEx);
                }
            }
        }



        //---------------------------------------------------------------------------------------------------------------------------

        //A method for checking existing Ids in a table and producing a new unique id
        private int NewIdCreation(string columnName, string tableName)
        {
            try
            {
                dbConnection.Open();
                var selectQuery = ("SELECT " + columnName + " FROM " + tableName);
                var checkForExistingIds = new SqlCommand(selectQuery, dbConnection);
                var idsList = new List<int>();
                var reader = checkForExistingIds.ExecuteReader();

                int i = 0;
                while (reader.Read())
                    idsList.Add(reader.GetInt32(i));
                reader.Close();
                dbConnection.Close();

                var newId = 1;
                while (idsList.Contains(newId))
                    newId++;
                return newId;
            }
            catch (SqlException sqlEx)
            {
                SqlError(sqlEx);
                return 0;
            }
        }


        //A method for verification of users' credentials
        public int VerifyCredentials(string inputUser, string inputPassword)
        {
            try
            {
                dbConnection.Open();

                //Retrieve user's username from the database
                var usersList = new List<string>();
                var checkExistingUsers = new SqlCommand("SELECT U_Username FROM UserDetails", dbConnection);
                var userReader = checkExistingUsers.ExecuteReader();

                int i = 0;
                while (userReader.Read())
                    usersList.Add(userReader.GetValue(i).ToString());
                userReader.Close();

                //Retrieve user's salt from the database
                var retrieveSalt = "SELECT Salt FROM UserDetails " +
                                   "WHERE U_Username = @userInput";
                var SaltOfThisUser = new SqlCommand(retrieveSalt, dbConnection);
                SaltOfThisUser.Parameters.AddWithValue("@userInput", inputUser);
                var saltReader = (string)SaltOfThisUser.ExecuteScalar();

                //Combine user's password with his salt
                string hashedInputWithSalt = PasswordHashing.Sha256_hash(inputPassword + saltReader);

                //Retrieve user's hashed password from the database
                var retrievePassword = "SELECT U_Password FROM UserDetails " +
                                       "WHERE U_Username = @userInput";
                var validPassForThisUser = new SqlCommand(retrievePassword, dbConnection);
                validPassForThisUser.Parameters.AddWithValue("@userInput", inputUser);
                var passReader = (string)validPassForThisUser.ExecuteScalar();
                dbConnection.Close();

                //Comparing input data with the database
                if (usersList.Contains(inputUser) && (passReader == hashedInputWithSalt))
                {
                    dbConnection.Open();
                    var retrieveId = "SELECT UserId FROM UserDetails " +
                                     "WHERE U_Username = @userInput";
                    var IdForThisUser = new SqlCommand(retrieveId, dbConnection);
                    IdForThisUser.Parameters.AddWithValue("@userInput", inputUser);
                    var userId = (int)IdForThisUser.ExecuteScalar();
                    dbConnection.Close();

                    return userId;
                }
                else
                {
                    Console.WriteLine("\nInvalid username or password. \nPress Enter to try again");
                    Console.ReadLine();
                    var retry = new LoginScreen();
                    retry.LoginCredentials();
                    return 0;
                }
            }
            catch(SqlException sqlEx)
            {
                SqlError(sqlEx);
                return 0;
            }
        }

        //A method that runs when a SqlException is caught and displays Error number(s) to the user before terminating the program
        public static void SqlError(SqlException sql)
        {
            StringBuilder errorMessages = new StringBuilder();

            for (int i = 0; i < sql.Errors.Count; i++)
            {
                errorMessages.Append("Error Number: " + sql.Errors[i].Number + "\n");
            }
            Console.WriteLine("\nError(s) occured. Program will be terminated..");
            Console.WriteLine(errorMessages.ToString());
            Console.ReadLine();
            Environment.Exit(0);
        }
    }
}
