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


                         --................                                   
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


        string loginInput = "";
        string passwordInput = "";
        string checkForSignUpRequest;

        public void LoginCredentials()
        {
            while (true)
            {
                Console.Write("Enter your Username (or type SIGNUP to create a new account):");
                loginInput = Console.ReadLine();
                
                if (String.IsNullOrWhiteSpace(loginInput))
                    Console.WriteLine("Invalid Input");

                checkForSignUpRequest = loginInput.ToLower();
                
                if (checkForSignUpRequest == "signup")
                {
                    DatabaseAccess newSignUp = new DatabaseAccess();
                    Console.Clear();
                    
                    newSignUp.InsertNewUser();
                    break;
                }

                if (!String.IsNullOrWhiteSpace(loginInput))
                    break;
            }

            while (checkForSignUpRequest != "signup")
            {
                Console.Write("Enter your Password:");
                passwordInput = Console.ReadLine();

                if (String.IsNullOrWhiteSpace(passwordInput))
                {
                    Console.WriteLine("Invalid Input");
                }
                else
                    break;
            }

            string hashedPassword = PasswordHashing.sha256_hash(passwordInput);//////////////////////////////////////////(TODO)SEARCHFORCREDENTIALSINDATABASE////////////////
        }
    }

    /*public class PasswordHash
    {
        public static byte[] GetHash(string inputString)
        {
            HashAlgorithm algorithm = SHA256.Create();
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        public static string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }
    }*/
}
