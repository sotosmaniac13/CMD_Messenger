using System;
using System.Data;
using System.Data.SqlClient;

namespace MyMessenger
{
    public class DatabaseAdminAccess
    {
        static string connectionString = Properties.Settings.Default.connectionString;

        //A method to display all created accounts and their details
        public static void AdminViewUserAccounts(int userId)
        {
            try
            {
                SqlConnection dbConnection = new SqlConnection(connectionString);

                DataSet ds = null;
                using (dbConnection)
                {
                    var sqlQuery = "SELECT UserId, U_Username, FirstName, LastName, Age, Email, JoinedAppOn, UserRole FROM UserDetails WHERE UserId != @userId";
                    var existingUsers = new SqlCommand(sqlQuery, dbConnection);
                    existingUsers.Parameters.AddWithValue("@userId", userId);
                    var adapter = new SqlDataAdapter(existingUsers);
                    ds = new DataSet();
                    adapter.Fill(ds);
                    adapter.Dispose();
                }

                var table = ds.Tables[0];
                Console.WriteLine($"\n {"ID",-5 }{"Username",-18}{"Firstname",-20}{"Lastname",-20}{"Age",-6}{"Email Address",-35}{"Registered On",-18}{"Role",-10}");
                Console.WriteLine($" {"--",-5 }{"--------",-18}{"---------",-20}{"--------",-20}{"---",-6}{"-------------",-35}{"-------------",-18}{"----",-10}");
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    var row = table.Rows[i];
                    var ID = row[0];
                    var U_Username = row[1];
                    var FirstName = row[2];
                    var LastName = row[3];
                    var Age = row[4];
                    var Email = row[5];
                    var JoinedAppOn = row[6];
                    var UserRole = row[7];

                    Console.WriteLine($" {ID,-5}{U_Username,-18}{FirstName,-20}{LastName,-20}{Age,-6}{Email,-35}{JoinedAppOn,-18}{UserRole,-10}");
                }
            }
            catch (SqlException sqlEx)
            {
                DatabaseAccess.SqlError(sqlEx);
            }
        }

        //A method that reads a user's role from the database
        public static string UserRole(int userId)
        {
            try
            {
                SqlConnection dbConnection = new SqlConnection(connectionString);

                dbConnection.Open();
                var sqlQuery = "SELECT UserRole FROM UserDetails WHERE UserId = @userId";
                var checkUserRoles = new SqlCommand(sqlQuery, dbConnection);
                checkUserRoles.Parameters.AddWithValue("@userId", userId);
                var userRoleReader = checkUserRoles.ExecuteScalar();
                dbConnection.Close();

                string userRole = userRoleReader.ToString();

                return userRole;
            }
            catch (SqlException sqlEx)
            {
                DatabaseAccess.SqlError(sqlEx);
                return "";
            }
        }

        //A method to check if a user exists in the database before the execution of any action
        public static int CheckUserExistsInDb(string userToEdit)
        {
            try
            {
                SqlConnection dbConnection = new SqlConnection(connectionString);

                dbConnection.Open();
                var retrieveId = "SELECT UserId FROM UserDetails WHERE U_Username = @userToEdit";
                var idForThisUser = new SqlCommand(retrieveId, dbConnection);
                idForThisUser.Parameters.AddWithValue("@userToEdit", userToEdit);
                var userToBeChanged = idForThisUser.ExecuteScalar();
                int usersId = Convert.ToInt32(userToBeChanged);
                dbConnection.Close();

                if (userToBeChanged == null)
                {
                    return 0;
                }
                else
                    return usersId;
            }
            catch (SqlException sqlEx)
            {
                DatabaseAccess.SqlError(sqlEx);
                return 0;
            }
        }

        //A method to delete a user from the database along with his friendships and messages
        public static void DeleteUser(int usersIdToDelete)
        {
            try
            {
                SqlConnection dbConnection = new SqlConnection(connectionString);

                dbConnection.Open();
                var sqlQuery = "DELETE FROM UserMessages WHERE SendersId = @usersIdToDelete OR ReceiversId = @usersIdToDelete";
                var deleteFromMessages = new SqlCommand(sqlQuery, dbConnection);
                deleteFromMessages.Parameters.AddWithValue("@usersIdToDelete", usersIdToDelete);
                deleteFromMessages.ExecuteNonQuery();

                var sqlQuery2 = "DELETE FROM UserFriends WHERE User1Id = @usersIdToDelete OR User2Id = @usersIdToDelete";
                var deleteFromFriendships = new SqlCommand(sqlQuery2, dbConnection);
                deleteFromFriendships.Parameters.AddWithValue("@usersIdToDelete", usersIdToDelete);
                deleteFromFriendships.ExecuteNonQuery();

                var sqlQuery3 = "DELETE FROM UserDetails WHERE UserId = @usersIdToDelete";
                var deleteUser = new SqlCommand(sqlQuery3, dbConnection);
                deleteUser.Parameters.AddWithValue("@usersIdToDelete", usersIdToDelete);
                var userDeleted = deleteUser.ExecuteNonQuery();
                dbConnection.Close();

                if (userDeleted == 1)
                {
                    Console.WriteLine("\nUser deleted successfully.\nPress Enter to return to the Menu");
                    Console.ReadLine();
                }
            }
            catch (SqlException sqlEx)
            {
                DatabaseAccess.SqlError(sqlEx);
            }
        }

        //A method to change the role of a user in the database
        public static void ChangeUserRole (int usersIdToEdit, string userRole)
        {
            try
            {
                SqlConnection dbConnection = new SqlConnection(connectionString);

                dbConnection.Open();
                var sqlQuery = "UPDATE UserDetails SET UserRole = @userRole WHERE UserId = @usersIdToEdit";
                var updateUsersRole = new SqlCommand(sqlQuery, dbConnection);
                updateUsersRole.Parameters.AddWithValue("@userRole", userRole);
                updateUsersRole.Parameters.AddWithValue("@usersIdToEdit", usersIdToEdit);
                var usersRoleUpdated = updateUsersRole.ExecuteNonQuery();
                dbConnection.Close();

                if (usersRoleUpdated == 1)
                {
                    Console.WriteLine("\nUser's role updated successfully.\nPress Enter to return to the Menu");
                    Console.ReadLine();
                }
            }
            catch (SqlException sqlEx)
            {
                DatabaseAccess.SqlError(sqlEx);
            }
        }

        //A method to view every message in the database
        public static void ViewUsersMessages()
        {
            try
            {
                SqlConnection dbConnection = new SqlConnection(connectionString);

                DataSet ds = null;
                using (dbConnection)
                {
                    var sqlQuery = "SELECT m.MessageId, u.U_Username, d.U_Username, m.SentOn, m.MessageContent FROM UserMessages m " +
                                   "INNER JOIN UserDetails u ON u.UserId = m.SendersId " +
                                   "INNER JOIN UserDetails d ON d.UserId = m.ReceiversId " +
                                   "ORDER BY SentOn";
                    var retrieveMessages = new SqlCommand(sqlQuery, dbConnection);
                    var adapter = new SqlDataAdapter(retrieveMessages);
                    ds = new DataSet();
                    adapter.Fill(ds);
                    adapter.Dispose();
                }

                var table = ds.Tables[0];
                Console.WriteLine($"\n {"ID",-5 }{"Sender",-18}{"Receiver",-18}{"Sent On",-22}{"Message"}");
                Console.WriteLine($" {"--",-5 }{"------",-18}{"--------",-18}{"-------",-22}{"-------"}");
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    var row = table.Rows[i];
                    var ID = row[0];
                    var Sender = row[1];
                    var Receiver = row[2];
                    var SentOn = row[3];
                    var Message = row[4];

                    Console.WriteLine($" {ID,-5}{Sender,-18}{Receiver,-18}{SentOn,-22}{Message}");
                }
            }
            catch (SqlException sqlEx)
            {
                DatabaseAccess.SqlError(sqlEx);
            }
        }

        //A method to edit a message in the database
        public static void EditUsersMessage(int messageId, string newValue)
        {
            try
            {
                SqlConnection dbConnection = new SqlConnection(connectionString);

                dbConnection.Open();
                var sqlQuery = "UPDATE UserMessages SET MessageContent = @newValue WHERE MessageId = @messageId";
                var updateMessage = new SqlCommand(sqlQuery, dbConnection);
                updateMessage.Parameters.AddWithValue("@newValue", newValue);
                updateMessage.Parameters.AddWithValue("@messageId", messageId);
                var messageUpdated = updateMessage.ExecuteNonQuery();
                dbConnection.Close();

                if (messageUpdated == 1)
                {
                    Console.WriteLine("\nMessage updated successfully.\nPress Enter to return to the Menu");
                    Console.ReadLine();
                }
            }
            catch (SqlException sqlEx)
            {
                DatabaseAccess.SqlError(sqlEx);
            }
        }

        //A method to delete a message in the database
        public static void DeleteUsersMessage(int messageId)
        {
            try
            {
                SqlConnection dbConnection = new SqlConnection(connectionString);

                dbConnection.Open();
                var sqlQuery = "DELETE FROM UserMessages WHERE MessageId = @messageId";
                var deleteMessage = new SqlCommand(sqlQuery, dbConnection);
                deleteMessage.Parameters.AddWithValue("@messageId", messageId);
                var messageDeleted = deleteMessage.ExecuteNonQuery();
                dbConnection.Close();

                if (messageDeleted == 2)
                {
                    Console.WriteLine("\nMessage deleted successfully.\nPress Enter to return to the Menu");
                    Console.ReadLine();
                }
                else
                {
                    Console.WriteLine("\nMessage could not be deleted.\nPress Enter to return to the Menu");
                    Console.ReadLine();
                }
            }
            catch (SqlException sqlEx)
            {
                DatabaseAccess.SqlError(sqlEx);
            }
        }
    }
}
