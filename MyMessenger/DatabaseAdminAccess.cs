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
        public void AdminViewUserAccounts(int userId)
        {
            string connectionString = "Server=sotiros-minipc\\SQLEXPRESS; Database=MyMessengerDB; User Id=admin; Password=admin";
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


    }
}
