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
            var userRole = DatabaseAdminAccess.UserRole(userId);
            Console.Clear();
            Console.WriteLine("======================================================================" +
                            "\n======================>   ADMINISTRATOR'S MENU   <====================\n" +
                              "======================================================================\n");

            Console.Write("\n================================" +
                          "\n1. View registered users" +
                          "\n2. Create a new user's account" +
                          "\n3. Edit existing users' details" +
                          "\n4. Delete a user" +
                          "\n5. Change the role of a user" +
                          "\n================================" +
                          "\n6. View users' messages" +
                          "\n7. Edit users' messages" +
                          "\n8. Delete users' messages" +
                          "\n================================" +
                          "\n9. Return to Main Menu" +
                          "\n10. Log Out" +
                          "\n11. Exit Program" +
                          "\n================================\n" +
                          "\nWhat do you want to do (press 1-11): ");

            try
            {
                var choice = Convert.ToByte(Console.ReadLine());
                switch (choice)
                {
                    case 1:
                        if(userRole != "Admin")
                        {
                            Console.Clear();
                            Console.WriteLine("\nYou do not have permission to access this information.\nPress enter to return to the Menu");
                            Console.ReadLine();
                            AdminMenu.AdminsMenu(userId);
                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine("======================================================================" +
                                            "\n========================>   REGISTERED USERS   <======================\n" +
                                              "======================================================================\n");

                            DatabaseAdminAccess.AdminViewUserAccounts(userId);

                            Console.WriteLine("\nPress Enter to return to the Menu");
                            Console.ReadLine();
                            Console.Clear();
                            AdminMenu.AdminsMenu(userId);
                        }
                        break;
                    case 2:
                        if (userRole != "Admin")
                        {
                            Console.Clear();
                            Console.WriteLine("\nYou do not have permission to access this information.\nPress Enter to return to the Menu");
                            Console.ReadLine();
                            AdminMenu.AdminsMenu(userId);
                        }
                        else
                        {
                            Console.Clear();
                            DatabaseAccess newSignUp = new DatabaseAccess();
                            newSignUp.InsertNewUser();

                            Console.WriteLine("\nNew User's account created.\nPress Enter to return to Main Menu");
                            Console.ReadLine();
                            Console.Clear();
                            AdminMenu.AdminsMenu(userId);
                        }
                        break;
                    case 3:
                        if (userRole != "Admin")
                        {
                            Console.Clear();
                            Console.WriteLine("\nYou do not have permission to access this information.\nPress Enter to return to the Menu");
                            Console.ReadLine();
                            AdminMenu.AdminsMenu(userId);
                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine("======================================================================" +
                                            "\n========================>   EDIT USERS DETAILS   <====================\n" +
                                              "======================================================================\n");

                            DatabaseAdminAccess.AdminViewUserAccounts(userId);

                            Console.WriteLine("\nEnter the username of the user you want to edit:\n(or press M to return to the Menu)");
                            var userToEdit = Console.ReadLine();
                            var usersIdToEdit = DatabaseAdminAccess.CheckUserExistsInDb(userToEdit);

                            if (userToEdit == "m")
                            {
                                Console.Clear();
                                AdminMenu.AdminsMenu(userId);
                            }
                            if (usersIdToEdit == 0)
                            {
                                Console.WriteLine("\nNo user found with this username.\nPress Enter to return to the Menu");
                                Console.ReadLine();
                                Console.Clear();
                                AdminMenu.AdminsMenu(userId);
                            }
                            else
                            {
                                Console.WriteLine(" ");
                                var retrieveUserDetails = new DatabaseAccess();
                                retrieveUserDetails.ViewUserDetails(usersIdToEdit);

                                while (true)
                                {
                                    Console.WriteLine("\nEnter the number of the field you want to change:\n(Or press M to return to the Menu)");
                                    var userInput = Console.ReadLine();
                                    var changeField = Int32.TryParse(userInput, out int number);

                                    if (userInput.ToLower() == "m")
                                    {
                                        Console.Clear();
                                        ApplicationMenus.MenuOptions(userId);
                                        break;
                                    }
                                    if (string.IsNullOrWhiteSpace(userInput) || number == 0)
                                    {
                                        Console.WriteLine("\nInvalid Input.\nPress Enter to try again");
                                        Console.ReadLine();
                                        continue;
                                    }
                                    else
                                    {
                                        switch (number)
                                        {
                                            case 1:
                                                var changeUsername = new DatabaseAccess();
                                                changeUsername.ChangeDetail(usersIdToEdit, "U_Username");
                                                break;
                                            case 2:
                                                var changeFirstname = new DatabaseAccess();
                                                changeFirstname.ChangeDetail(usersIdToEdit, "FirstName");
                                                break;
                                            case 3:
                                                var changeLastname = new DatabaseAccess();
                                                changeLastname.ChangeDetail(usersIdToEdit, "LastName");
                                                break;
                                            case 4:
                                                var changeAge = new DatabaseAccess();
                                                changeAge.ChangeDetail(usersIdToEdit, "Age");
                                                break;
                                            case 5:
                                                var changeEmail = new DatabaseAccess();
                                                changeEmail.ChangeDetail(usersIdToEdit, "Email");
                                                break;
                                            default:
                                                Console.WriteLine("\nInvalid Input.\nPress Enter to return to the Menu");
                                                Console.ReadLine();
                                                Console.Clear();
                                                AdminMenu.AdminsMenu(userId);
                                                break;
                                        }
                                        Console.Clear();
                                        AdminMenu.AdminsMenu(userId);
                                        break;
                                    }
                                }
                                Console.Clear();
                                AdminMenu.AdminsMenu(userId);
                                break;
                            }
                        }
                        break;
                    case 4:
                        if (userRole != "Admin")
                        {
                            Console.Clear();
                            Console.WriteLine("\nYou do not have permission to access this information.\nPress Enter to return to the Menu");
                            Console.ReadLine();
                            AdminMenu.AdminsMenu(userId);
                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine("======================================================================" +
                                            "\n===========================>   DELETE A USER   <======================\n" +
                                              "======================================================================\n");

                            DatabaseAdminAccess.AdminViewUserAccounts(userId);


                            Console.WriteLine("\nEnter the username of the user you want to delete:\n(or press M to return to the Menu)");
                            var userToDelete = Console.ReadLine();
                            var usersIdToDelete = DatabaseAdminAccess.CheckUserExistsInDb(userToDelete);

                            if (userToDelete == "m")
                            {
                                Console.Clear();
                                AdminMenu.AdminsMenu(userId);
                            }
                            if (usersIdToDelete == 0)
                            {
                                Console.WriteLine("\nNo user found with this username.\nPress Enter to return to the Menu");
                                Console.ReadLine();
                                Console.Clear();
                                AdminMenu.AdminsMenu(userId);
                            }
                            else
                            {
                                DatabaseAdminAccess.DeleteUser(userId, usersIdToDelete);
                                Console.Clear();
                                AdminMenu.AdminsMenu(userId);
                            }
                        }
                        break;
                    case 5:
                        if (userRole != "Admin")
                        {
                            Console.Clear();
                            Console.WriteLine("\nYou do not have permission to access this information.\nPress Enter to return to the Menu");
                            Console.ReadLine();
                            AdminMenu.AdminsMenu(userId);
                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine("======================================================================" +
                                            "\n=======================>   CHANGE A USER'S ROLE   <===================\n" +
                                              "======================================================================\n");

                            DatabaseAdminAccess.AdminViewUserAccounts(userId);


                            Console.WriteLine("\nEnter the username of the user whose role you want to change:\n(or press M to return to the Menu)");
                            var userToEdit = Console.ReadLine();
                            var usersIdToEdit = DatabaseAdminAccess.CheckUserExistsInDb(userToEdit);

                            if (userToEdit == "m")
                            {
                                Console.Clear();
                                AdminMenu.AdminsMenu(userId);
                            }
                            if (usersIdToEdit == 0)
                            {
                                Console.WriteLine("\nNo user found with this username.\nPress Enter to return to the Menu");
                                Console.ReadLine();
                                Console.Clear();
                                AdminMenu.AdminsMenu(userId);
                            }
                            else
                            {
                                Console.WriteLine("\nAvailable user roles:" +
                                    "\n 1 Role1 --> User can VIEW all messages between users" +
                                    "\n 2 Role2 --> User can VIEW and EDIT all messages between users" +
                                    "\n 3 Role3 --> User can VIEW, EDIT and DELETE all messages between users\n" +
                                    "\nEnter the number of the role you want to give to this user:" +
                                    "\n(or press M to return to the Menu)");
                                var roleSelected = Console.ReadLine();
                                var numberOfRole = int.TryParse(roleSelected, out int number);

                                if (roleSelected.ToLower() == "m")
                                {
                                    Console.Clear();
                                    AdminMenu.AdminsMenu(userId);
                                    break;
                                }
                                if (string.IsNullOrWhiteSpace(roleSelected) || number == 0)
                                {
                                    Console.WriteLine("\nInvalid Input.\nPress Enter to return to the Menu");
                                    Console.ReadLine();
                                    Console.Clear();
                                    AdminMenu.AdminsMenu(userId);
                                }
                                else
                                {
                                    string usersRole = "";

                                    switch (number)
                                    {
                                        case 1:
                                            usersRole = "Role1";
                                            break;
                                        case 2:
                                            usersRole = "Role3";
                                            break;
                                        case 3:
                                            usersRole = "Role3";
                                            break;
                                        default:
                                            Console.WriteLine("\nInvalid Input.\nPress Enter to return to the Menu");
                                            Console.ReadLine();
                                            Console.Clear();
                                            AdminMenu.AdminsMenu(userId);
                                            break;
                                    }
                                    DatabaseAdminAccess.ChangeUserRole(userId, usersIdToEdit, usersRole);
                                    Console.Clear();
                                    AdminMenu.AdminsMenu(userId);
                                }
                            }
                        }
                        break;
                    case 6:
                        if (userRole != "Admin" || userRole != "Role1" || userRole != "Role2" || userRole != "Role3")
                        {
                            Console.Clear();
                            Console.WriteLine("\nYou do not have permission to access this information.\nPress Enter to return to the Menu");
                            Console.ReadLine();
                            AdminMenu.AdminsMenu(userId);
                        }
                        else
                        {

                        }
                        break;
                    case 7:
                        if (userRole != "Admin" || userRole != "Role2" || userRole != "Role3")
                        {
                            Console.Clear();
                            Console.WriteLine("\nYou do not have permission to access this information.\nPress Enter to return to the Menu");
                            Console.ReadLine();
                            AdminMenu.AdminsMenu(userId);
                        }
                        else
                        {

                        }
                        break;
                    case 8:
                        if (userRole != "Admin" || userRole != "Role3")
                        {
                            Console.Clear();
                            Console.WriteLine("\nYou do not have permission to access this information.\nPress Enter to return to the Menu");
                            Console.ReadLine();
                            AdminMenu.AdminsMenu(userId);
                        }
                        else
                        {

                        }
                        break;
                    case 9:
                        Console.Clear();
                        ApplicationMenus.MenuOptions(userId);
                        break;
                    case 10:
                        Console.Clear();
                        LoginScreen newLogin = new LoginScreen();
                        newLogin.AppBanner();
                        newLogin.LoginCredentials();
                        break;
                    case 11:
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("\nInvalid Input.\nPress Enter to continue");
                        Console.ReadLine();
                        Console.Clear();
                        ApplicationMenus.MenuOptions(userId);
                        break;
                }

            }
            catch (FormatException)
            {
                Console.WriteLine("\nInvalid Input\nPress Enter to continue");
                Console.ReadLine();
                Console.Clear();
                AdminMenu.AdminsMenu(userId);
            }
        }
    }
}
