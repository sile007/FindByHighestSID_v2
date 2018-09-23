using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;
using System.Security;
using System.Collections;
using System.Text.RegularExpressions; 

namespace FindByHighestSID
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> foundUser = new List<string>();

            Console.WriteLine("Active Directory Tool to sort all Users by SID");

            Console.WriteLine("IP");
            string ip = Console.ReadLine();

            Console.WriteLine("Username");
            string userName = Console.ReadLine();

            Console.WriteLine("Password");
            string password = GetConsolePassword();

            if (authenticate(ip, userName, password))
            {

                while (true)
                {
                    Console.WriteLine("Authentication Successfull");
                    string account = Console.ReadLine();
                    foundUser = search(account, ip, userName, password);
                    string[] test = search(account, ip, userName, password).ToArray();

                    List<string> ascUser = new List<string>(foundUser.OrderBy(i => PadNumbers(i)));
                    //Console.WriteLine(ascUser[0]);
                    foreach (var item in ascUser)
                    {
                        Console.WriteLine(item.ToString());
                    }

                    Console.Write("another search? (y or n)");
                    string result = Console.ReadLine();

                    if(!result.Contains("y"))
                    {
                        Console.WriteLine("stopping program");
                        break; 
                    }

                }


            }
            else
            {
                Console.Write("Authentication error");
                Console.WriteLine("retry?: ");

            }

            Console.ReadKey();
        }


        public static string PadNumbers(string input)
        {
            return Regex.Replace(input, "[0-9]+", match => match.Value.PadLeft(10, '0'));
        }

        private static bool authenticate(string directory, string userName, string password)
        {
            using (PrincipalContext ct = new PrincipalContext(ContextType.Domain, directory))
            {
                return ct.ValidateCredentials(userName, password);
            }
        }

        private static List<string> search(string account, string domain, string userName, string password)
        {
            List<string> foundUser = new List<string>();

            using (PrincipalContext ctx = new PrincipalContext(ContextType.Domain, domain, userName, password))
            {
                UserPrincipal userPrincipal = new UserPrincipal(ctx);
                userPrincipal.SamAccountName = account;

                using (PrincipalSearcher searcher = new PrincipalSearcher(userPrincipal))
                {
                    foreach(Principal principal in searcher.FindAll())
                    {
                        //Console.WriteLine(principal.Sid);
                        foundUser.Add(principal.Sid.ToString());
                    }
                }
                
                
            }
            return foundUser; 

        }

        private static string GetConsolePassword()
        {

            StringBuilder sb = new StringBuilder();

            while (true)
            {
                ConsoleKeyInfo cki = Console.ReadKey(true);

                if (cki.Key == ConsoleKey.Enter)
                {

                    Console.WriteLine();
                    break;

                }
                
                if (cki.Key == ConsoleKey.Backspace)
                {
                    if (sb.Length > 0)

                    {
                        Console.Write("\b\0\b");
                        sb.Length--;
                    }
                    continue;

                }
                Console.Write('*');
                sb.Append(cki.KeyChar);
 
            }
            return sb.ToString();
        }
    }
}
