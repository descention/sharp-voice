using System;// I'm pretty sure this is required...
using System.Windows.Forms;//Console app, but I like to show message boxes for errors...
using System.IO;// this is needed.
using System.Threading;// yay threads!
using System.Diagnostics;//Helpful in getting this code working.
using SharpVoice;
using System.Xml;
using Newtonsoft.Json.Serialization;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Runtime.Serialization.Formatters.Binary;

namespace GoogleTests
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        //[STAThread]
        static void Main()
        {
            try {
                /*Visual Form = new Visual();
                Form.ShowDialog();
                */

                string cookieData = "cookie.dat";

                var formatter = new BinaryFormatter();

                if (File.Exists(cookieData))
                    using (Stream s = File.OpenRead(cookieData))
                        Voice.cookiejar = formatter.Deserialize(s) as System.Net.CookieContainer;

                ConsoleKeyInfo c = new ConsoleKeyInfo();
                string email = "", password = "", smsPin = "";

                Console.Write("Email: ");
                email = Console.ReadLine();

                Console.Write("Password: ");
                while(c.Key != ConsoleKey.Enter)
                {
                    c = Console.ReadKey(true);
                    if(c.Key != ConsoleKey.Enter)
                        password += c.KeyChar;
                }
                Console.WriteLine();

                Console.Write("PIN (leave blank if you don't use 2-factor auth): ");
                smsPin = Console.ReadLine();

                Voice v;
                
                if (string.IsNullOrEmpty(smsPin))
                {
                    v = new Voice(email, password, true);
                }
                else
                {
                    v = new Voice(email, password, true, smsPin);
                }

                Console.Write("To (phone #): ");
                string to = Console.ReadLine();
                Console.Write("Message: ");
                string msg = Console.ReadLine();
                v.SendSMS(to, msg);

                using (Stream s = File.Create(cookieData))
                    formatter.Serialize(s, Voice.cookiejar);

                //Console.Read();
		    } catch (Exception e) {
                Console.WriteLine(e);
                //MessageBox.Show(e.Message);//more Debug help
                Console.Read();
		    }
        }
    }
}
