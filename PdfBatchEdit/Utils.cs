using System;
using System.Collections.Generic;

namespace PdfBatchEdit
{
    public class Utils
    {
        public static string MainDirectory
        {
            get { return AppDomain.CurrentDomain.BaseDirectory; }
        }

        public static string GetRandomString(int length)
        {
            string chars = "abcdefghijklmnopqrstuvwxyz";
            string randomString = "";
            Random random = new Random();

            for (int i = 0; i < length; i++)
            {
                randomString += chars[random.Next(chars.Length)];
            }
            return randomString;
        }

        public static Dictionary<string, string> GetArgumentsDictionary()
        {
            string[] args = Environment.GetCommandLineArgs();
            var argsDict = new Dictionary<string, string>();
            argsDict["path"] = args[0];

            string currentArgumentName = null;
            for (int i = 1; i < args.Length; i++) {
                string argument = args[i];
                if (argument.StartsWith("-"))
                    currentArgumentName = argument.Substring(1);
                else if (currentArgumentName == null)
                    Console.WriteLine("The command line argument has a wrong format.");
                else
                    argsDict[currentArgumentName] = argument;
            }

            return argsDict;
        }
    } 
}
