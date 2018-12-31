using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMessenger
{
    public class AdminMenu
    {
        public static void AdminsMenu(int userId)
        {
            Console.Clear();
            Console.WriteLine("======================================================================" +
                            "\n======================>   ADMINISTRATOR'S MENU   <====================\n" +
                              "======================================================================\n");

            Console.Write("\n===========================" +
                          "\n1. View registered users" +
                          "\n2. Create a new user's account" +
                          "\n3. Edit existing users' details" +
                          "\n4. Delete a user" +
                          "\n===========================" +
                          "\n5. Log Out" +
                          "\n6. Exit Program" +
                          "\n===========================\n" +
                          "\nWhat do you want to do (press 1-6): ");

            try
            {
                var choice = Convert.ToByte(Console.ReadLine());
                switch (choice)
                {
                    case 1:
                        Console.Clear();
                        Console.WriteLine("======================================================================" +
                                        "\n========================>   REGISTERED USERS   <======================\n" +
                                          "======================================================================\n");
                        
                        var viewUsers = new DatabaseAdminAccess();
                        viewUsers.AdminViewUserAccounts(userId);

                        Console.WriteLine("\nPress Enter to return to Main Menu");
                        Console.ReadLine();
                        Console.Clear();
                        AdminMenu.AdminsMenu(userId);
                        break;
                    case 2:
                        Console.Clear();
                        DatabaseAccess newSignUp = new DatabaseAccess();
                        newSignUp.InsertNewUser();

                        Console.WriteLine("\nNew User's account created.\nPress Enter to return to Main Menu.");
                        Console.ReadLine();
                        Console.Clear();
                        AdminMenu.AdminsMenu(userId);
                        break;
                    case 3:
                        Console.Clear();
                        Console.WriteLine("======================================================================" +
                                        "\n========================>   EDIT USER DETAILS   <=====================\n" +
                                          "======================================================================\n");

                        var viewUsers2 = new DatabaseAdminAccess();
                        viewUsers2.AdminViewUserAccounts(userId);

                        Console.WriteLine("\nEnter the username of the user you want to edit:\n(or press  M to return to Main Menu.)");
                        var userToEdit = Console.ReadLine();

                        if (userToEdit == "m")
                        {
                            Console.Clear();
                            AdminMenu.AdminsMenu(userId);
                        }
                        else
                        {
                            Console.WriteLine("\nInvalid Input.");
                        }
                        break;
                    case 4:

                        break;
                    case 5:
                        Console.Clear();
                        LoginScreen newLogin = new LoginScreen();
                        newLogin.AppBanner();
                        newLogin.LoginCredentials();
                        break;
                    case 6:
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("\nInvalid Input.\nPress Enter to continue.");
                        Console.ReadLine();
                        Console.Clear();
                        ApplicationMenus.MenuOptions(userId);
                        break;
                }

            }
            catch (FormatException)
            {
                Console.WriteLine("\nInvalid Input\nPress Enter to continue.");
                Console.ReadLine();
                Console.Clear();
                ApplicationMenus.MenuOptions(userId);
            }
        }
    }
}
