using System;

namespace MyMessenger
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WindowWidth = 135;

            LoginScreen newLogin = new LoginScreen();
            newLogin.AppBanner();
            newLogin.LoginCredentials();
        }
    }
}
