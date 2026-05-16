using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;



    public class PasswordGenerator
    {
        public static void main()
        {
            string password = "admin123";
            string hash = BCrypt.Net.BCrypt.HashPassword(password);
            Console.WriteLine(hash);
        }
    }
