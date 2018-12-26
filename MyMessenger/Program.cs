using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMessenger
{
    class Program
    {
        static void Main(string[] args)
        {
            LoginScreen newLogin = new LoginScreen();
            newLogin.AppBanner();
            newLogin.LoginCredentials();
            
            Console.ReadLine();

        }
    }
}
