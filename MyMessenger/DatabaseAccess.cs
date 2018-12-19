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
        static string connectionString = "Server=DESKTOP-NO1CDE8\\SQLEXPRESS; Database=MyMessengerDB; User Id=admin; Password=admin";

        public void InsertNewUser()
        {
            SqlConnection dbConnection = new SqlConnection(connectionString);

            dbConnection.Open();

            //INSERT NEW USER IN THE DATABASE
            Console.WriteLine("================\nNEW USER ACCOUNT\n================\n");
            Console.WriteLine("Enter your Username:");
            string newUsername = Console.ReadLine();
            Console.WriteLine("Enter your Password:");
            string newPassword = Console.ReadLine();

            Console.WriteLine("Enter your Firstname:");
            string newFirstName = Console.ReadLine();
            Console.WriteLine("Enter your Lastname:");
            string newLastName = Console.ReadLine();
            Console.WriteLine("Enter your Age:");
            string newAge = Console.ReadLine();
            Console.WriteLine("Enter your Email Address:");
            string newEmail = Console.ReadLine();

            string hashedPassword = PasswordHashing.sha256_hash(newPassword);

            var insertNewUserLogin = new SqlCommand("INSERT INTO LoginCredentials VALUES(@U_Username, @U_Password)", dbConnection);
            insertNewUserLogin.Parameters.AddWithValue("@U_Username", newUsername);
            insertNewUserLogin.Parameters.AddWithValue("@U_Password", hashedPassword);

            var affectedrows = insertNewUserLogin.ExecuteNonQuery();
            Console.WriteLine(affectedrows);

            var insertNewUserDetails = new SqlCommand("INSERT INTO UserDetails VALUES(@FirstName, @LastName, @Age)", dbConnection);
            insertNewUserDetails.Parameters.AddWithValue("@FirstName", newFirstName);
            insertNewUserDetails.Parameters.AddWithValue("@LastName", newLastName);
            insertNewUserDetails.Parameters.AddWithValue("@Age", newAge);

            var affectedrows2 = insertNewUserDetails.ExecuteNonQuery();
            Console.WriteLine(affectedrows2);

            dbConnection.Close();
        }


    }
}
