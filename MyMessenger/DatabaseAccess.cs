using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace MyMessenger
{
    public class DatabaseAccess
    {
        static string connectionString = Properties.Settings.Default.connectionString;
        SqlConnection dbConnection = new SqlConnection(connectionString);

        //A method for inserting new users in the database
        public int InsertNewUser()
        {
            //INSERT NEW USER IN THE DATABASE
            Console.WriteLine("======================================================================" +
                            "\n=======================>   NEW USER ACCOUNT   <=======================\n" +
                              "======================================================================\n");

            
            //Asking new user for his details in order to create an account
            Console.Write("Enter a Username: ");
            string newUsername = Console.ReadLine();
            Console.Write("Enter a Password: ");
            string newPassword = Console.ReadLine();
            Console.Write("Enter an Email Address: ");
            string email = Console.ReadLine();
            string newEmail = email.ToLower();

            while (true)
            {
                var registeredEmail = CheckEmailAlreadyRegistered(newEmail.ToLower());
                if (registeredEmail == true)
                {
                    Console.WriteLine("\nThis Email is already being used.\nEnter another email address:");
                    newEmail = Console.ReadLine();
                }
                else
                    break;
            }

            Console.Write("Enter Firstname: ");
            string newFirstName = Console.ReadLine();
            Console.Write("Enter Lastname: ");
            string newLastName = Console.ReadLine();
            Console.Write("Enter Age: ");
            string newAge = Console.ReadLine();

            if (Convert.ToInt32(newAge) < 15)
            {
                Console.WriteLine("\nUser must be older than 15.\nProgram will now terminate.");
                Console.ReadLine();
                Environment.Exit(0);
            }
            
            
            //Hashing for securely storing the password in the database
            string hashedPassword = PasswordHashing.Sha256_hash(newPassword);


            //Reading all User Ids from every table in the database and produces new Ids for the new user in each table
            var newUserId = NewIdCreation("UserId", "UserDetails");

            //Saving User Details in the database
            dbConnection.Open();

            var insertNewUserDetails = new SqlCommand("INSERT INTO UserDetails VALUES(@UserId, @U_Username, @U_Password, @FirstName, @LastName, @Age, @Email, @JoinedAppOn, @UserRole)", dbConnection);
            insertNewUserDetails.Parameters.AddWithValue("@UserId", newUserId);
            insertNewUserDetails.Parameters.AddWithValue("@U_Username", newUsername);
            insertNewUserDetails.Parameters.AddWithValue("@U_Password", hashedPassword);
            insertNewUserDetails.Parameters.AddWithValue("@FirstName", newFirstName);
            insertNewUserDetails.Parameters.AddWithValue("@LastName", newLastName);
            insertNewUserDetails.Parameters.AddWithValue("@Age", newAge);
            insertNewUserDetails.Parameters.AddWithValue("@Email", newEmail);
            insertNewUserDetails.Parameters.AddWithValue("@JoinedAppOn", DateTime.Now.ToString("d/M/yyyy"));
            insertNewUserDetails.Parameters.AddWithValue("@UserRole", "SimpleUser");
            insertNewUserDetails.ExecuteNonQuery();
            
            dbConnection.Close();
            return newUserId;
        }
        
        
        //A method for checking existing Ids in a table and producing a new unique id
        private int NewIdCreation(string columnName, string tableName)
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


        //A method for verification of users' credentials
        public int VerifyCredentials(string inputUser, string inputPass)
        {
            dbConnection.Open();

            var usersList = new List<string>();
            var checkExistingUsers = new SqlCommand("SELECT U_Username FROM UserDetails", dbConnection);
            var userReader = checkExistingUsers.ExecuteReader();
            
            int i = 0;
            while (userReader.Read())
                usersList.Add(userReader.GetValue(i).ToString());
            userReader.Close();

            
            var retrievePassword = "select U_Password from UserDetails where U_Username = '" + inputUser + "'";
            var validPassForThisUser = new SqlCommand( retrievePassword, dbConnection);
            var passReader = (string)validPassForThisUser.ExecuteScalar();

            dbConnection.Close();

            if (usersList.Contains(inputUser) && (passReader == inputPass))
            {
                dbConnection.Open();
                var retrieveId = "select UserId from UserDetails where U_Username = '" + inputUser + "'";
                var IdForThisUser = new SqlCommand(retrieveId, dbConnection);
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


        //A method for checking that the Email Address provided by the user while creating a new account doesn't exist in the database
        private bool CheckEmailAlreadyRegistered(string emailInput)
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
        


        //MAIN MENU OPTIONS

        //Compose a new Message.
        public void NewMessage(int userId, string receiver)
        {
            int newMessageId = NewIdCreation("MessageId", "UserMessages");

            dbConnection.Open();
            var retrieveId = "SELECT UserId FROM UserDetails WHERE U_Username = '" + receiver + "'";
            var IdForThisUser = new SqlCommand(retrieveId, dbConnection);
            var receiversId = IdForThisUser.ExecuteScalar();
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
                Console.WriteLine("\n(Maximum message length 250 characters)\nPress Enter to send the message or M to return to the Main Menu\nEnter your message here: ");
                var messageContent = Console.ReadLine();
                if (messageContent.Length > 250)
                {
                    Console.WriteLine("Message is longer than 250 characters. Please reduce its length.\nPress Enter to return to the Main Menu");
                    Console.ReadLine();
                }
                if (messageContent.ToLower() == "m")
                {
                    Console.Clear();
                    ApplicationMenus.MenuOptions(userId);
                }
                if (string.IsNullOrWhiteSpace(messageContent))
                {
                    Console.WriteLine("Invalid Input.\nPress Enter to return to the Main Menu.");
                    Console.ReadLine();
                    Console.Clear();
                    ApplicationMenus.MenuOptions(userId);
                }


                dbConnection.Open();
                var checkExistingFriends = "SELECT FriendshipId FROM UserFriends WHERE (User1Id = " + userId + " AND User2Id = " + receiversId + ") OR (User1Id = " + receiversId + " AND User2Id = " + userId + ")";
                var checkAlreadyAdded = new SqlCommand(checkExistingFriends, dbConnection);
                var friendshipFound = checkAlreadyAdded.ExecuteScalar();

                var sqlQuery = "INSERT INTO UserMessages VALUES ( @MessageId, @FriendshipId, @MessageContent, @SentOn, @SendersId, @ReceiversId, @UnreadMessage)";
                var insertNewMessage = new SqlCommand(sqlQuery, dbConnection);
                insertNewMessage.Parameters.AddWithValue("@MessageId", newMessageId);
                insertNewMessage.Parameters.AddWithValue("@FriendshipId", friendshipFound);
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
                var retrieveUsername = "SELECT U_Username FROM UserDetails WHERE UserId = " + userId ;
                var IdForLoggedInUser = new SqlCommand(retrieveUsername, dbConnection);
                var sendersId = IdForLoggedInUser.ExecuteScalar();
                dbConnection.Close();

                FilesAccess.MessageToFile((string)sendersId, receiver, messageContent);
            }
        }

        //View old Messages.
        public void ViewOldMessages(int userId)
        {
            DataSet ds = null;
            dbConnection.Open();
            var sqlQuery = "SELECT m.SentOn, u.U_Username, m.MessageContent FROM UserMessages m INNER JOIN UserDetails u ON m.SendersId = u.UserId WHERE ReceiversId = " + userId + " AND UnreadMessage = 0 ORDER BY SentOn";
            var findOldMessages = new SqlCommand(sqlQuery, dbConnection);
            var adapter = new SqlDataAdapter(findOldMessages);
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

        //View new Messages.
        public void ViewNewMessages(int userId)
        {
            DataSet ds = null;
            dbConnection.Open();
            var sqlQuery = "SELECT m.SentOn, u.U_Username, m.MessageContent FROM UserMessages m INNER JOIN UserDetails u ON m.SendersId = u.UserId WHERE ReceiversId = " + userId + " AND UnreadMessage = 1";
            var findNewMessages = new SqlCommand(sqlQuery, dbConnection);
            var adapter = new SqlDataAdapter(findNewMessages);
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
            var sqlUpdateQuery = "UPDATE UserMessages SET UnreadMessage = 0 WHERE ReceiversId = " + userId + " AND UnreadMessage = 1";
            var messagesRead = new SqlCommand(sqlUpdateQuery, dbConnection);
            messagesRead.ExecuteNonQuery();
            dbConnection.Close();
        }

        public int NewMessagesNumber(int userId)
        {
            DataSet ds = null;
            using (dbConnection)
            {
                var sqlQuery = "SELECT m.SentOn, u.U_Username, m.MessageContent FROM UserMessages m INNER JOIN UserDetails u ON m.SendersId = u.UserId WHERE ReceiversId = " + userId + " AND UnreadMessage = 1";
                var findNewMessages = new SqlCommand(sqlQuery, dbConnection);
                var adapter = new SqlDataAdapter(findNewMessages);
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


        //View sent Messages.
        public void ViewSentMessages(int userId)
        {
            DataSet ds = null;
            dbConnection.Open();
            var sqlQuery = "SELECT m.SentOn, u.U_Username, m.MessageContent FROM UserMessages m INNER JOIN UserDetails u ON m.ReceiversId = u.UserId WHERE SendersId = " + userId + " ORDER BY SentOn";
            var findSentMessages = new SqlCommand(sqlQuery, dbConnection);
            var adapter = new SqlDataAdapter(findSentMessages);
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

        //View users' friends.
        public void ViewFriends(int loggedInUsersId)
        {
            DataSet ds = null;
            using (dbConnection)
            {
                var sqlQuery = "(SELECT u.U_Username, u.FirstName, u.LastName, u.Age, u.Email, f.FriendsSince from UserFriends f INNER JOIN UserDetails u ON f.User2Id = u.UserId WHERE (f.User1Id = " + loggedInUsersId + " OR f.User2Id = " + loggedInUsersId + ")" + " AND u.UserId != (" + loggedInUsersId + ")) UNION (SELECT u.U_Username, u.FirstName, u.LastName, u.Age, u.Email, f.FriendsSince from UserFriends f INNER JOIN UserDetails u ON f.User1Id = u.UserId WHERE (f.User1Id = " + loggedInUsersId + " OR f.User2Id = " + loggedInUsersId + ")" + " AND u.UserId != (" + loggedInUsersId + ")) ORDER BY FriendsSince";
                var existingFriends = new SqlCommand(sqlQuery, dbConnection);
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

        
        //Add a friend by his username.
        public void AddFriend(int userId, string userNameToAdd)
        {
            int newFriendshipId = NewIdCreation("FriendshipId", "UserFriends");

            dbConnection.Open();
            var retrieveId = "SELECT UserId FROM UserDetails WHERE U_Username = '" + userNameToAdd + "'";
            var IdForThisUser = new SqlCommand(retrieveId, dbConnection);
            var userToBeAdded = IdForThisUser.ExecuteScalar();

            if (userToBeAdded == null)
            {
                Console.WriteLine("No user found with this username.\nPress Enter to return to the Main Menu");
                Console.ReadLine();
                Console.Clear();
                ApplicationMenus.MenuOptions(userId);
            }

            var checkExistingFriends= "SELECT FriendshipId FROM UserFriends WHERE (User1Id = "+ userId + " AND User2Id = " + userToBeAdded + ") OR (User1Id = " + userToBeAdded + " AND User2Id = " + userId + ")";
            var checkAlreadyAdded = new SqlCommand(checkExistingFriends, dbConnection);
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


        //Remove a friend.
        public void RemoveFriend(int userId, string friendToBeRemoved)
        {
            dbConnection.Open();
            var retrieveId = "SELECT UserId FROM UserDetails WHERE U_Username = '" + friendToBeRemoved + "'";
            var IdForThisUser = new SqlCommand(retrieveId, dbConnection);
            var friendsIdToBeRemoved = IdForThisUser.ExecuteScalar();
            dbConnection.Close();

            if (friendsIdToBeRemoved == null)
            {
                Console.WriteLine("No friend found with this username.\nPress Enter to return to the Main Menu");
                Console.ReadLine();
                Console.Clear();
                ApplicationMenus.MenuOptions(userId);
            }
            else
            {
                dbConnection.Open();
                var sqlQuery = "DELETE FROM UserFriends WHERE (User1Id = " + userId + " AND User2Id = " + friendsIdToBeRemoved + ") OR (User1Id = " + friendsIdToBeRemoved + " AND User2Id = " + userId + ")";
                var deleteFriend = new SqlCommand(sqlQuery, dbConnection);
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


        //Select 5 random users from the database and ask the user if he wants to add one of them in his Friends' List.
        public void FriendSuggestions(int userId)
        {
            DataSet ds = null;
            using (dbConnection)
            {
                dbConnection.Open();
                var sqlQuery = "SELECT U_Username, FirstName, LastName, Age, Email FROM UserDetails WHERE UserId NOT IN(SELECT User2Id FROM UserFriends Where User1Id = " + userId + " UNION SELECT User1Id From UserFriends Where User2Id = " + userId + ") AND UserId != " + userId;
                var otherUsers = new SqlCommand(sqlQuery, dbConnection);
                var adapter = new SqlDataAdapter(otherUsers);
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

        //View user details
        public void ViewUserDetails(int userId)
        {
            DataSet ds = null;
            using (dbConnection)
            {
                var sqlQuery = "SELECT U_Username, FirstName, LastName, Age, Email from UserDetails WHERE UserId = " + userId;
                var existingFriends = new SqlCommand(sqlQuery, dbConnection);
                var adapter = new SqlDataAdapter(existingFriends);
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
        //Change Users' Details
        public void ChangeDetail(int userId, string fieldToChange)
        {
            Console.WriteLine("\nInput new value for this field:\n(or press M to return to the Main Menu");
            var newValue = Console.ReadLine();
            if (newValue.ToLower() == "m")
            {
                Console.Clear();
                ApplicationMenus.MenuOptions(userId);
            }
            else
            {
                dbConnection.Open();
                var sqlUpdateQuery = "UPDATE UserDetails SET " + fieldToChange + " = '" + newValue + "' WHERE UserId = " + userId;
                var userDetailsRead = new SqlCommand(sqlUpdateQuery, dbConnection);
                var fieldUpdated = userDetailsRead.ExecuteNonQuery();
                dbConnection.Close();

                if (fieldUpdated == 1)
                {
                    Console.WriteLine("\nField updated.\nPress Enter to return to the Menu");
                    Console.ReadLine();
                }
            }
        }
    }
}
