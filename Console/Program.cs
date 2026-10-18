using System;
using DiscoryAuthSDK;

namespace DiscoryAuthConsole
{
    class Program
    {
        // ==============================================================================
        // CONFIGURATION (Grab from https://discoryauth.cc/app/)
        // ==============================================================================
        const string AppName    = "My C# Protected App";
        const string AppId      = "app_xxxxxxxx";             // Replace with your App ID
        const string AppSecret  = "sec_xxxxxxxxxxxxxxxxxxxx"; // Replace with your App Secret
        const string AppVersion = "1.0";
        const string ApiUrl     = "https://discoryauth.cc/api/v1";

        static void Main(string[] args)
        {
            Console.Title = "DiscoryAuth C# Example | https://discoryauth.cc";
            PrintBanner();

            var auth = new DiscoryAuth(AppName, AppId, AppSecret, AppVersion, ApiUrl);

            Console.WriteLine("[*] Initializing DiscoryAuth Client...");
            if (!auth.Init())
            {
                Console.WriteLine("[-] Failed to initialize. Check credentials or connection.");
                Console.ReadLine();
                return;
            }

            Console.WriteLine($"[+] Application: {auth.AppData.name} (v{auth.AppData.version})");
            Console.WriteLine($"[+] Hardware ID: {auth.Hwid}\n");

            Console.WriteLine("Select an Option:");
            Console.WriteLine(" [1] Login");
            Console.WriteLine(" [2] Register");
            Console.WriteLine(" [3] License Key Only");
            Console.WriteLine(" [4] Exit");

            Console.Write("\nChoice > ");
            string choice = Console.ReadLine()?.Trim();

            if (choice == "1")
            {
                Console.Write("Username: ");
                string user = Console.ReadLine();
                Console.Write("Password: ");
                string pass = Console.ReadLine();

                Console.WriteLine("[*] Verifying credentials...");
                if (!auth.Login(user, pass))
                {
                    Console.ReadLine();
                    return;
                }
                Console.WriteLine($"\n[+] Logged in successfully as: {auth.UserData.username}");
            }
            else if (choice == "2")
            {
                Console.Write("Enter Username: ");
                string user = Console.ReadLine();
                Console.Write("Enter Password: ");
                string pass = Console.ReadLine();
                Console.Write("Enter License Key: ");
                string key = Console.ReadLine();

                Console.WriteLine("[*] Registering user account...");
                if (!auth.Register(user, pass, key))
                {
                    Console.ReadLine();
                    return;
                }
                Console.WriteLine("\n[+] Registration successful! You can now log in.");
                Console.ReadLine();
                return;
            }
            else if (choice == "3")
            {
                Console.Write("Enter License Key: ");
                string key = Console.ReadLine();

                Console.WriteLine("[*] Validating License Key...");
                if (!auth.License(key))
                {
                    Console.ReadLine();
                    return;
                }
                Console.WriteLine($"\n[+] License Validated! Welcome {auth.UserData.username}!");
            }
            else
            {
                return;
            }

            // Post-Auth Information
            Console.WriteLine("\n" + new string('=', 50));
            Console.WriteLine("           AUTHENTICATED USER DETAILS");
            Console.WriteLine(new string('=', 50));
            Console.WriteLine($" Username    : {auth.UserData.username}");
            Console.WriteLine($" Expires At  : {auth.UserData.expires_at}");
            Console.WriteLine($" Hardware ID : {auth.UserData.hwid}");
            Console.WriteLine(new string('=', 50));

            // Variable download demo
            Console.WriteLine("\n[*] Fetching remote variable 'secret_data'...");
            string secretVal = auth.Var("secret_data");
            if (!string.IsNullOrEmpty(secretVal))
            {
                Console.WriteLine($"[+] Cloud Variable: {secretVal}");
            }

            auth.Log($"C# user {auth.UserData.username} initialized payload.");
            Console.WriteLine("[+] Session Heartbeat: Active & Valid!");
            Console.WriteLine("[*] Starting protected payload...\n");

            Console.WriteLine("Press Enter to close...");
            Console.ReadLine();
        }

        static void PrintBanner()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(@"
 ____  _                               _         _   _     
|  _ \(_)___  ___ ___  _ __ _   _     / \  _   _| |_| |__  
| | | | / __|/ __/ _ \| '__| | | |   / _ \| | | | __| '_ \ 
| |_| | \__ \ (_| (_) | |  | |_| |  / ___ \ |_| | |_| | | |
|____/|_|___/\___\___/|_|   \__, | /_/   \_\__,_|\__|_| |_|
                            |___/                          
  :: Official C# Example | https://discoryauth.cc ::
");
            Console.ResetColor();
        }
    }
}
