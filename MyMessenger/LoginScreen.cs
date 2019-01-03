using System;
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

                    Console.WriteLine("\nYou created a new account!\nPress Enter to continue..");
                    Console.ReadLine();
                    Console.Clear();
                    ApplicationMenus.MenuOptions(newUserId);
                }
                if (!String.IsNullOrWhiteSpace(usernameInput))
                    break;
            }

            if (usernameInput.ToLower() != "signup")
            {
                while (true)
                {
                    Console.Write("============================================================================\nEnter your Password:");
                    passwordInput = Console.ReadLine();

                    if (String.IsNullOrWhiteSpace(passwordInput))
                    {
                        Console.WriteLine("Invalid Input");
                    }
                    
                    else
                    {
                        string hashedInputPassword = PasswordHashing.Sha256_hash(passwordInput);
                        DatabaseAccess verifyUser = new DatabaseAccess();
                        int userId = verifyUser.VerifyCredentials(usernameInput, hashedInputPassword);

                        //if (usernameInput == "admin" && userId != 0)
                        //{
                        //    AdminMenu.AdminsMenu(userId);
                        //    break;
                        //}

                        Console.Clear();
                        ApplicationMenus.MenuOptions(userId);
                        break;
                    }
                }
            }
        }
    }
}
