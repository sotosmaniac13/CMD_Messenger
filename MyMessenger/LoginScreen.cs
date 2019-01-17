using System;

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
        }


        private string usernameInput = "";
        private string passwordInput = "";

        public void LoginCredentials()
        {
            Console.Write("============================================================================\n" +
                          "Enter your Username (or type SIGNUP to create a new account):");
            usernameInput = Console.ReadLine();
            
            if (usernameInput.ToLower() == "signup")
            {
                Console.Clear();
                DatabaseAccess newSignUp = new DatabaseAccess();
                newSignUp.InsertNewUser();

                Console.WriteLine("\nYou created a new account!\nPress Enter to Login");
                Console.ReadLine();
                Console.Clear();
                LoginScreen newLogin = new LoginScreen();
                newLogin.AppBanner();
                newLogin.LoginCredentials();
            }

            Console.Write("============================================================================\nEnter your Password:");
            passwordInput = Console.ReadLine();
            
            DatabaseAccess verifyUser = new DatabaseAccess();
            int userId = verifyUser.VerifyCredentials(usernameInput, passwordInput);
            Console.Clear();
            ApplicationMenus.MenuOptions(userId);
        }
    }
}
