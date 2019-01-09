using System;
using System.IO;

namespace MyMessenger
{
    public static class FilesAccess
    {
        static string desktopPath = Environment.GetFolderPath(System.Environment.SpecialFolder.DesktopDirectory);
        static string path = desktopPath + "\\Messages.txt";

        public static void MessageToFile(string sender, string receiver, string message)
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
    }
}
