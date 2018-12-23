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
        static string connectionString = "Server=sotirosi7Laptop\\SQLEXPRESS; Database=MyMessengerDB; User Id=admin; Password=admin";
        SqlConnection dbConnection = new SqlConnection(connectionString);

        public void InsertNewUser()
        {
            dbConnection.Open();

            //INSERT NEW USER IN THE DATABASE
            Console.WriteLine("================\nNEW USER ACCOUNT\n================\n");

            
            //Asking new user for his details in order to create an account
            Console.Write("Enter your Username: ");
            string newUsername = Console.ReadLine();
            Console.Write("Enter your Password: ");
            string newPassword = Console.ReadLine();
            Console.Write("Enter your Email Address: ");
            string newEmail = Console.ReadLine();
            CheckEmailNotInDatabase(newEmail);
            Console.Write("Enter your Firstname: ");
            string newFirstName = Console.ReadLine();
            Console.Write("Enter your Lastname: ");
            string newLastName = Console.ReadLine();
            Console.Write("Enter your Age: ");
            string newAge = Console.ReadLine();
            
            
            //Hashing for securely storing the password in the database
            string hashedPassword = PasswordHashing.sha256_hash(newPassword);


            //Reading all User Ids from every table in the database and produces new Ids for the new user in each table
            var newUserId = NewIdCreation("UserId", "UserDetails");
            var newLoginId = NewIdCreation("LoginId", "LoginCredentials");


            //Saving User Details in the database
            var insertNewUserDetails = new SqlCommand("INSERT INTO UserDetails VALUES(@UserId, @FirstName, @LastName, @Age, @Email, @JoinedAppOn, @UserRole)", dbConnection);
            insertNewUserDetails.Parameters.AddWithValue("@UserId", newUserId);
            insertNewUserDetails.Parameters.AddWithValue("@FirstName", newFirstName);
            insertNewUserDetails.Parameters.AddWithValue("@LastName", newLastName);
            insertNewUserDetails.Parameters.AddWithValue("@Age", newAge);
            insertNewUserDetails.Parameters.AddWithValue("@Email", newEmail);
            insertNewUserDetails.Parameters.AddWithValue("@JoinedAppOn", DateTime.Now.ToString("d/M/yyyy"));
            insertNewUserDetails.Parameters.AddWithValue("@UserRole", "User1");
            var affectedrows = insertNewUserDetails.ExecuteNonQuery();

            //Saving Login Credentials in the database
            var insertNewUserLogin = new SqlCommand("INSERT INTO LoginCredentials VALUES(@LoginId, @UserId, @U_Username, @U_Password)", dbConnection);
            insertNewUserLogin.Parameters.AddWithValue("@LoginId", newLoginId);
            insertNewUserLogin.Parameters.AddWithValue("@UserId", newUserId);
            insertNewUserLogin.Parameters.AddWithValue("@U_Username", newUsername);
            insertNewUserLogin.Parameters.AddWithValue("@U_Password", hashedPassword);
            var affectedrows2 = insertNewUserLogin.ExecuteNonQuery();

            dbConnection.Close();
        }

        
        //A method for checking existing Ids in a table and producing a new unique id
        private int NewIdCreation(string columnName, string tableName)
        {
            var selectQuery = ("SELECT " + columnName + " FROM " + tableName);
            var checkForExistingIds = new SqlCommand(selectQuery, dbConnection);
            var idsList = new List<int>();
            var reader = checkForExistingIds.ExecuteReader();

            int i = 0;
            while (reader.Read())
                idsList.Add(reader.GetInt32(i));
            reader.Close();

            var newId = 1;
            while (idsList.Contains(newId))
                newId++;

            return newId;
        }

        public void VerifyCredentials(string inputUser, string inputPass)
        {
            dbConnection.Open();

            var usersList = new List<string>();
            var checkExistingUsers = new SqlCommand("SELECT U_Username FROM LoginCredentials", dbConnection);
            var userReader = checkExistingUsers.ExecuteReader();

            int i = 0;
            while (userReader.Read())
                usersList.Add(userReader.GetValue(i).ToString());
            userReader.Close();

            var hashList = new List<string>();
            var retrievePassword = "select U_Password from LoginCredentials where U_Username = ' " + inputUser + " '";
            var validPassForThisUser = new SqlCommand( retrievePassword, dbConnection);
            var passReader = validPassForThisUser.ExecuteReader();///////////////////////////////////////////////////////

            int j = 0;
            while (passReader.Read())
            {
                hashList.Add(passReader.GetValue(j).ToString());
                Console.WriteLine("meh" + j);
            }

            if (hashList.Contains(inputPass))
            {
                ApplicationMenus continueToMenu = new ApplicationMenus();
                continueToMenu.meh();//////////////////////////////////////////////////////////////////
            }
            
            else
                Console.WriteLine("\nInvalid password. \nPress Enter to close the program.");

            passReader.Close();
        }


        private void CheckEmailNotInDatabase(string emailInput)
        {
            var checkExistingEmails = new SqlCommand("SELECT Email FROM UserDetails", dbConnection);

            var emailsList = new List<string>();
            var userReader = checkExistingEmails.ExecuteReader();

            int i = 0;
            while (userReader.Read())
                emailsList.Add(userReader.GetValue(i).ToString());
            userReader.Close();

            if (emailsList.Contains(emailInput))
            {
                Console.WriteLine("\nThis Email is already registered.\nPress Enter to close the program..");
                var exit = Console.ReadKey();
                if (exit.Key == ConsoleKey.Enter)
                    Environment.Exit(0);////////////////////////////////////// ti ginetai an den pathsei enter o user??
            }
        }
    }
}
