﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMessenger
{
    public class ApplicationMenus
    {
        public static void MenuOptions(int userId)
        {
            Console.WriteLine("======================================================================" +
                            "\n===========================>   MAIN MENU   <==========================\n" +
                              "======================================================================\n");

            var dbConnForNewMessages = new DatabaseAccess();
            var newMessagesReceived = dbConnForNewMessages.NewMessagesNumber(userId);

            Console.Write("\n===========================" +
                          "\n1. Compose a new Message" +
                          "\n2. Old Messages" +
                          "\n3. New Messages(" + newMessagesReceived + ")" +
                          "\n4. Sent Messages" +
                          "\n===========================" +
                          "\n5. View friends"+
                          "\n6. Add a friend"+
                          "\n7. Friend suggestions" +
                          "\n===========================" +
                          "\n8. Log Out"+
                          "\n9. Exit Program"+
                          "\n===========================\n" +
                          "\nWhat do you want to do (press 1-9): ");

            try
            {
                var choice = Convert.ToByte(Console.ReadLine());
                switch (choice)
                {
                    case 1:
                        Console.Clear();
                        Console.WriteLine("======================================================================" +
                                        "\n==========================>   NEW MESSAGE   <=========================\n" +
                                          "======================================================================\n");

                        Console.WriteLine("\nEnter the username of the friend you want to send the message to:\n(Or press M to return to Main Menu.)");
                        string receiversUsername = Console.ReadLine();
                        if (receiversUsername.ToLower() == "m")
                        {
                            Console.Clear();
                            ApplicationMenus.MenuOptions(userId);
                        }
                        if (string.IsNullOrWhiteSpace(receiversUsername))
                        {
                            Console.WriteLine("Invalid Input\nPress Enter to return to Main Menu.");
                            Console.ReadLine();
                        }
                        else
                        {
                            var sendNewMessage = new DatabaseAccess();
                            sendNewMessage.NewMessage(userId, receiversUsername);
                        }
                        Console.Clear();
                        ApplicationMenus.MenuOptions(userId);
                        break;
                    case 2:
                        Console.Clear();
                        Console.WriteLine("======================================================================" +
                                        "\n=========================>   OLD MESSAGES   <=========================\n" +
                                          "======================================================================\n");
                        var dbConnOldMess = new DatabaseAccess();
                        dbConnOldMess.ViewOldMessages(userId);
                        Console.WriteLine("\nPress Enter to return to Main Menu");
                        Console.ReadLine();
                        Console.Clear();
                        ApplicationMenus.MenuOptions(userId);
                        break;
                    case 3:
                        Console.Clear();
                        Console.WriteLine("======================================================================" +
                                        "\n=========================>   NEW MESSAGES   <=========================\n" +
                                          "======================================================================\n");
                        var dbConnMess = new DatabaseAccess();
                        dbConnMess.ViewNewMessages(userId);
                        Console.WriteLine("\nPress Enter to return to Main Menu");
                        Console.ReadLine();
                        Console.Clear();
                        ApplicationMenus.MenuOptions(userId);
                        break;
                    case 4:
                        Console.Clear();
                        Console.WriteLine("======================================================================" +
                                        "\n=========================>   SENT MESSAGES   <========================\n" +
                                          "======================================================================\n");
                        var dbConnSentMess = new DatabaseAccess();
                        dbConnSentMess.ViewSentMessages(userId);
                        Console.WriteLine("\nPress Enter to return to Main Menu");
                        Console.ReadLine();
                        Console.Clear();
                        ApplicationMenus.MenuOptions(userId);
                        break;
                    case 5:
                        Console.Clear();
                        Console.WriteLine("======================================================================" +
                                        "\n=========================>   FRIENDS'LIST   <=========================\n" +
                                          "======================================================================\n");
                        var dbConn = new DatabaseAccess();
                        dbConn.ViewFriends(userId);

                        while (true)
                        {
                            Console.WriteLine("\nPress M to return to Main Menu\nPress R to remove a friend");
                            string viewChoice = (Console.ReadLine());
                            if (viewChoice.ToLower() == "m")
                            {
                                Console.Clear();
                                ApplicationMenus.MenuOptions(userId);
                                break;
                            }
                            if (viewChoice.ToLower() == "r")
                            {
                                Console.WriteLine("Enter the Username of the friend you want to delete from your Friends'List:\n(Or Press M to return to Main Menu)");
                                var deleteUser = Console.ReadLine();
                                if (deleteUser.ToLower() == "m")
                                {
                                    Console.Clear();
                                    ApplicationMenus.MenuOptions(userId);
                                }
                                else
                                {
                                    var removeAFriend = new DatabaseAccess();
                                    removeAFriend.RemoveFriend(userId, deleteUser);
                                }
                                break;
                            }
                            else
                                Console.WriteLine("\nInvalid Input.");
                        }
                        break;
                    case 6:
                        Console.Clear();
                        Console.WriteLine("======================================================================" +
                                        "\n=========================>   ADD A FRIEND   <=========================\n" +
                                          "======================================================================\n");

                        Console.WriteLine("\nEnter the username of the user you want to add to your Friends'List:\n(Or press M to return to Main Menu.)");
                        string addFriend = Console.ReadLine();
                        if (addFriend.ToLower() == "m")
                        {
                            Console.Clear();
                            ApplicationMenus.MenuOptions(userId);
                        }
                        if (string.IsNullOrWhiteSpace(addFriend))
                        {
                            Console.WriteLine("Invalid Input.\nPress Enter to return to Main Menu.");
                            Console.ReadLine();
                        }
                        else
                        {
                            var addNewFriend = new DatabaseAccess();
                            addNewFriend.AddFriend(userId, addFriend);
                        }
                        Console.Clear();
                        ApplicationMenus.MenuOptions(userId);
                        break;
                    case 7:
                        Console.Clear();
                        Console.WriteLine("======================================================================" +
                                        "\n=======================>   FRIEND SUGGESTIONS   <=====================\n" +
                                          "======================================================================\n");

                        var suggestFriends = new DatabaseAccess();
                        suggestFriends.FriendSuggestions(userId);

                        while (true)
                        {
                            Console.WriteLine("\nPress A to add a user in your Friends'List\nPress M to return to Main Menu");
                            var nextAction = Console.ReadLine();
                            if (nextAction.ToLower() == "a")
                            {
                                Console.WriteLine("\nEnter the username of the user you want to add to your Friends'List:\n(Or press M to return to Main Menu.)");
                                string addFriend2 = Console.ReadLine();

                                if (addFriend2.ToLower() == "m")
                                {
                                    Console.Clear();
                                    ApplicationMenus.MenuOptions(userId);
                                    break;
                                }
                                if (string.IsNullOrWhiteSpace(addFriend2))
                                {
                                    Console.WriteLine("Invalid Input.\nPress Enter to try again.");
                                    Console.ReadLine();
                                }
                                else
                                {
                                    var addNewFriend = new DatabaseAccess();
                                    addNewFriend.AddFriend(userId, addFriend2);
                                    break;
                                }
                                Console.Clear();
                                ApplicationMenus.MenuOptions(userId);
                                break;
                            }
                            if (nextAction.ToLower() == "m")
                            {
                                Console.Clear();
                                ApplicationMenus.MenuOptions(userId);
                                break;
                            }
                            else
                                Console.WriteLine("Invalid Input.");
                        }
                        break;
                    case 8:
                        Console.Clear();
                        LoginScreen newLogin = new LoginScreen();
                        newLogin.AppBanner();
                        newLogin.LoginCredentials();
                        break;
                    case 9:
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("\nInvalid Input\nPress Enter to continue.");
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
