﻿using System;

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
    }
}
