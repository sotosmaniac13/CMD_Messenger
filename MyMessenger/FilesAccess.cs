using System;
using System.IO;
using System.Text;

namespace MyMessenger
{
    public static class FilesAccess
    {
        static string desktopPath = Environment.GetFolderPath(System.Environment.SpecialFolder.DesktopDirectory);
        static string path = desktopPath + "\\Messages.txt";

        //A method that stores every message in a txt file in the desktop
        public static void MessageToFile(string sender, string receiver, string message)
        {
            try
            {
                string dateOfSubmission = Convert.ToString(DateTime.Now);

                if (!File.Exists(path))
                {
                    using (var sw = new StreamWriter(path, true))
                    {
                        sw.WriteLine($" {"Date of submission",-22}{"Sender",-18}{"Receiver",-18}{"Message content"}");
                        sw.WriteLine($" {"------------------",-22}{"------",-18}{"--------",-18}{"---------------"}");
                    }
                }

                using (var sw = new StreamWriter(path, true))
                {
                    sw.WriteLine($" {dateOfSubmission,-22}{sender,-18}{receiver,-18}{message}");
                }
            }
            catch (IOException)
            {
                Console.WriteLine("\nError occured while accessing Txt File.\nPress Enter to continue");
                Console.ReadLine();
            }
        }
    }
}
