﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace MyMessenger
{
    public class LoginScreen
    {
        public void AppBanner()
        {
            Console.WriteLine(@"


                         ..................                                   
                      +:`               ````.---..                            
                    .+`                         ``.---                        
                   -s`                               `//                      
                  .d`                                  -y.                     
                  y+                                    -d`                    
                 .N`                                     do                    
                 +m                                      hh                    
                 yd             CMD_Messenger            dh                    
                 od                                     `Ms                    
                 /M.                                    +M:                    
                 `Ny                                    dm                     
                  +No`                                 +N:                     
                   /md/`                              :No                      
                    .yNmy/-.`                     ``.+d/                       
                      .+hmmmdhyysooooooossyy`    shhyo.                        
                         `.:/osyhhhhhhhyyymM.   /Mo-`                          
                                          ym   :Ny                             
                                         .N/ `oNs`                             
                                         sy.+dd/                               
                                        :Ndmy:`                                
                                       -mh+.                                   
                                       -.                                      


            ");
            return;
        }


        string usernameInput = "";
        string passwordInput = "";

        public void LoginCredentials()
        {
            while (true)
            {
                Console.Write("============================================================================\n" +
                            "Enter your Username (or type SIGNUP to create a new account):");
                usernameInput = Console.ReadLine();
                
                if (String.IsNullOrWhiteSpace(usernameInput))
                    Console.WriteLine("Invalid Input");
                

                if (usernameInput.ToLower() == "signup")
                {
                    Console.Clear();
                    DatabaseAccess newSignUp = new DatabaseAccess();
                    int newUserId = newSignUp.InsertNewUser();

                    Console.WriteLine("\nYou created a new account!\nPress Enter to Continue..");
                    var continueToMenu = Console.ReadKey(true);
                    if (continueToMenu.Key == ConsoleKey.Enter)
                    {
                        Console.Clear();
                        //ApplicationMenus accountCreated = new ApplicationMenus();
                        //accountCreated.MenuOptions(newUserId);
                        ApplicationMenus.MenuOptions(newUserId);
                    }
                }
                if (!String.IsNullOrWhiteSpace(usernameInput))
                    break;
            }

            if (usernameInput.ToLower() != "signup")
            {
                Console.Write("============================================================================\nEnter your Password:");
                passwordInput = Console.ReadLine();

                if (String.IsNullOrWhiteSpace(passwordInput))
                {
                    Console.WriteLine("Invalid Input");
                }

                else
                {
                    string hashedInputPassword = PasswordHashing.sha256_hash(passwordInput);
                    DatabaseAccess verifyUser = new DatabaseAccess();
                    int userId = verifyUser.VerifyCredentials( usernameInput, hashedInputPassword);

                    Console.Clear();
                    //ApplicationMenus continueToMenu = new ApplicationMenus();
                    //continueToMenu.MenuOptions(userId);
                    ApplicationMenus.MenuOptions(userId);
                }
            }
        }
    }
}
