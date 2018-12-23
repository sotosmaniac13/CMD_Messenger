using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace MyMessenger
{
    class LoginScreen
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
        string checkForSignUpRequest;

        public void LoginCredentials()
        {
            while (true)
            {
                Console.Write("============================================================================\n" +
                            "Enter your Username (or type SIGNUP to create a new account):");
                usernameInput = Console.ReadLine();
                
                if (String.IsNullOrWhiteSpace(usernameInput))
                    Console.WriteLine("Invalid Input");

                checkForSignUpRequest = usernameInput.ToLower();
                
                if (checkForSignUpRequest == "signup")
                {
                    DatabaseAccess newSignUp = new DatabaseAccess();
                    Console.Clear();
                    
                    newSignUp.InsertNewUser();
                    Console.WriteLine("\nYour created a new account!\nPress Enter to Continue..");
                    var continueToMenu = Console.ReadKey(true);
                    if (continueToMenu.Key == ConsoleKey.Enter)
                    {
                        Console.Clear();
                        ApplicationMenus accountCreated = new ApplicationMenus();
                        accountCreated.meh();///////////////////////////////////////////////////////////////////////////////
                    }
                    
                    break;
                }

                if (!String.IsNullOrWhiteSpace(usernameInput))
                    break;
            }

            while (checkForSignUpRequest != "signup")
            {
                Console.Write("============================================================================\n" + 
                            "Enter your Password:");
                passwordInput = Console.ReadLine();

                if (String.IsNullOrWhiteSpace(passwordInput))
                {
                    Console.WriteLine("Invalid Input");
                }

                else
                {
                    string hashedInputPassword = PasswordHashing.sha256_hash(passwordInput);
                    DatabaseAccess verifyUser = new DatabaseAccess();
                    verifyUser.VerifyCredentials( usernameInput, hashedInputPassword);
                    break;
                }
            }

            
        }
    }
}
