using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            SqlConnection dbConnection = new SqlConnection(connectionString);

            DataSet ds = null;
            using (dbConnection)
            {
                var sqlQuery = "SELECT UserId, U_Username, FirstName, LastName, Age, Email, JoinedAppOn, UserRole FROM UserDetails WHERE UserId != " + userId;
                var existingUsers = new SqlCommand(sqlQuery, dbConnection);
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

        //A method that reads a user's role from the database
        public static string UserRole(int userId)
        {
            SqlConnection dbConnection = new SqlConnection(connectionString);

            dbConnection.Open();
            var sqlQuery = "SELECT UserRole FROM UserDetails WHERE UserId = " + userId;
            var checkUserRoles = new SqlCommand( sqlQuery, dbConnection);
            var userRoleReader = checkUserRoles.ExecuteScalar();
            dbConnection.Close();

            string userRole = userRoleReader.ToString();

            return userRole;
        }

        //A method to check if a user exists in the database before the execution of any action
        public static int CheckUserExistsInDb(string userToEdit)
        {
            SqlConnection dbConnection = new SqlConnection(connectionString);

            dbConnection.Open();
            var retrieveId = "SELECT UserId FROM UserDetails WHERE U_Username = '" + userToEdit + "'";
            var IdForThisUser = new SqlCommand(retrieveId, dbConnection);
            var userToBeChanged = IdForThisUser.ExecuteScalar();
            int usersId = Convert.ToInt32(userToBeChanged);

            if (userToBeChanged == null)
            {
                return 0;
            }
            else
                return usersId;
        }

        //A method to delete a user from the database along with his friendships and messages
        public static void DeleteUser(int userId, int usersIdToDelete)
        {
            SqlConnection dbConnection = new SqlConnection(connectionString);

            dbConnection.Open();
            var sqlQuery = "DELETE FROM UserFriends WHERE User1Id = " + usersIdToDelete + " OR User2Id = " + usersIdToDelete;
            var deleteFromFriendships = new SqlCommand(sqlQuery, dbConnection);
            deleteFromFriendships.ExecuteNonQuery();

            var sqlQuery2 = "DELETE FROM UserMessages WHERE SendersId = " + usersIdToDelete + " OR ReceiversId = " + usersIdToDelete;
            var deleteFromMessages = new SqlCommand(sqlQuery2, dbConnection);
            deleteFromMessages.ExecuteNonQuery();

            var sqlQuery3 = "DELETE FROM UserDetails WHERE UserId = " + usersIdToDelete;
            var deleteUser = new SqlCommand(sqlQuery3, dbConnection);
            var userDeleted = deleteUser.ExecuteNonQuery();
            dbConnection.Close();

            if (userDeleted == 1)
            {
                Console.WriteLine("\nUser deleted successfully.\nPress Enter to return to the Menu");
                Console.ReadLine();
            }
        }

        //A method to change the role of a user in the database
        public static void ChangeUserRole (int userId, int usersIdToEdit, string userRole)
        {
            SqlConnection dbConnection = new SqlConnection(connectionString);

            dbConnection.Open();
            var sqlQuery = "UPDATE UserDetails SET UserRole = '" + userRole + "' WHERE UserId = " + usersIdToEdit;
            var updateUsersRole = new SqlCommand(sqlQuery, dbConnection);
            var usersRoleUpdated = updateUsersRole.ExecuteNonQuery();
            dbConnection.Close();

            if (usersRoleUpdated == 1)
            {
                Console.WriteLine("\nUser's role updated successfully.\nPress Enter to return to the Menu");
                Console.ReadLine();
            }
        }
    }
}
