using System;

namespace MyMessenger
{
    class Program
    {
        static void Main(string[] args)
        {
            const int presetWindowWidth = 135;
            Console.WindowWidth = presetWindowWidth;

            LoginScreen newLogin = new LoginScreen();
            newLogin.AppBanner();
            newLogin.LoginCredentials();
        }
    }
}
