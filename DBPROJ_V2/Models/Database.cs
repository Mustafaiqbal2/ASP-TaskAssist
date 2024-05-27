using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Data.SqlTypes;

namespace DBPROJ_V2.Models
{
    public class Database
    {
        SqlString connectionString = "Data Source=172.23.129.23;User ID=Boys;Password=12345678;Connect Timeout=30;Encrypt=False;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False";
        public bool validateLogin(Cred credentials, string userType)
        {
            Console.WriteLine("Username: " + credentials.username);
            Console.WriteLine("Password: " + credentials.password);
            switch (userType)
            {
                case "admin":
                    Console.WriteLine("Accessing DB for Admin");
                    return true;
                case "mem":
                    Console.WriteLine("Accessing DB for Member");
                    return true;
                case "trn":
                    Console.WriteLine("Accessing DB for Trainer");
                    return true;
                case "own":
                    Console.WriteLine("Accessing DB for Owner");
                    return true;
                default:
                    Console.WriteLine("Invalid User Type");
                    return false;
            }

        }
    }
}
