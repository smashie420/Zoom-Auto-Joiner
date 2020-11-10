using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Media;
using System.Net.Http;
using System.Threading;
using DiscordRPC;
using DiscordRPC.Logging;

// USE THIS
using JNogueira.Discord.Webhook.Client;


namespace Zoom_Auto_Join
{
    class Program
    {
        /*
          {
            { "Physics", "12:00:00 AM", "google.com"  },
            { "English", "12:01:00 AM", "youutube.com" },
            { "Math", "12:03:00 AM", "foo.com" },
          }
         */

        public class Zoom
        {
            // Constructor that takes one argument:
            public Zoom(string[,] classess)
            {
                
                    Console.WriteLine(classess.Length);
               
                
            }

            //var zoom = new Zoom(new string[] { new string[] { "Physics", "12:00:00 AM", "youtube.com" }, new string[] { "English", "12:01:00 AN", "google.com" } });


            //string[] classess = { { "Period 1", "12:00:00 AM", "Google.com" }, { "Period 2", "12:01:00 AM", "youtube.com" } };
            /*string[,] classess = new string[,]
            { 
                { "A", "AB", "AC" }, { "B", "BC", "BA" }, { "C", "CA", "CB" }
            };*/





            



            public void joinClass(string link)
            {
                Process.Start("chrome.exe", link);
                Console.WriteLine("Joining class link: " + link);
            }



            // Auto-implemented readonly property:
            public string Link { get; }
            public string Time { get; }
            public object Properties { get; private set; }

            // Method that overrides the base class (System.Object) implementation.
            public override string ToString()
            {
                return "Current Link: " + Link + "\nCurrent Time: " + Time;

            }
        }




        // Discord ShiT
        public static readonly DiscordRpcClient client = new DiscordRpcClient("775260345597034526");
        public static void DiscordStatus()
        {
            Random rnd = new Random();

            int single = rnd.Next(1, 3);
            string[] imagetextArr = { "Dying", "Im fucking bored", "Lazzy", "Holy fuck im coding this at 3AM", "Epic" };
            int randomTxtArr = rnd.Next(1, imagetextArr.Length + 1);



            client.Initialize();

            client.SetPresence(new RichPresence()
            {
                Details = "By smashguns#6175",
                //State = "csharp example",
                Assets = new Assets()
                {
                    LargeImageKey = "small" + single,
                    LargeImageText = imagetextArr[randomTxtArr],
                    SmallImageKey = "zoom"

                }
            });
            client.UpdateStartTime();
        }

        /*
        private static readonly HttpClient http = new HttpClient();
        public static async void SendWebHook(string message)
        {
            var values = new Dictionary<string, string>
                        {
                            { "username", "Zoom Logs" },
                            { "content", message },
                            { "avatar", "https://www.publiccounsel.net/train/wp-content/uploads/sites/13/5e8ce318664eae0004085461.png" },
                            { "embeds", "" }
                        };
            
            var content = new FormUrlEncodedContent(values);
            var response = await http.PostAsync("https://discordapp.com/api/webhooks/775279806773985292/XqzTnS5Ako0fRdeXkiAa18CgOwtliRCrHGwqNX-O5LYesg4rCak26PsAa7soHSdwaKWe", content);
            var responseString = await response.Content.ReadAsStringAsync();
        }*/

        public static string AmpmTo24(DateTime time)
        {
            return Convert.ToDateTime(time).ToString("HH:mm:ss");
        }
        public static string MilitaryToAmPm(DateTime time)
        {
            return Convert.ToDateTime(time).ToString("hh:mm:ss") + " " + Convert.ToDateTime(time).ToString("tt");
        }







        static void Main(string[] args)
        {
            /*
            string[] links = { "google.com", "youtube.com" };
            string[] periods = { "Physics": {"12:00:00AM", "google.com"}, "English": {"12:01:00AM", "youtube.com"}, "Math": {"12:02:00AM", "yahoo.com"}}
            string[] times = { "12:00:00 AM", "12:01:00 AM"};
            var zoom = new Zoom(links,times);*/

            //string[] periods = { "Physics": {"12:00:00AM", "google.com"}, "English": {"12:01:00AM", "youtube.com"}, "Math": {"12:02:00AM", "yahoo.com"}}

       
            DiscordStatus();
            var zoom = new Zoom
                    (new string[,]
                        {
                            { "Physics", "12:00:00 AM", "google.com"  },
                            { "English", "12:01:00 AM", "youutube.com" },
                            { "Math", "12:03:00 AM", "foo.com" },
                        }
                    );


            if (!File.Exists("settings.ini"))
            {
                var MyIni = new IniFile("settings.ini");

                MyIni.Write("MondayPeriod1Time", "09:10:00 AM", "Periods");
                MyIni.Write("MondayPeriod2Time", "10:35:00 AM", "Periods");
                MyIni.Write("MondayPeriod3Time", "12:00:00 PM", "Periods");

                MyIni.Write("Period1Time", "08:30:00 AM", "Periods");
                MyIni.Write("Period2Time", "09:55:00 AM", "Periods");
                MyIni.Write("Period3Time", "11:20:00 AM", "Periods");

                MyIni.Write("Period1Link", "google.com", "Links");
                MyIni.Write("Period2Link", "google.com", "Links");
                MyIni.Write("Period3Link", "google.com", "Links");

                MyIni.Write("Mute Sounds", "False", "Sound");

                MyIni.Write("Start Up Sound", "", "Sound");
                MyIni.Write("Join Sound", "", "Sound");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n\n");
                Console.WriteLine("Check settings.txt and configure it to said periods");
                Console.WriteLine("\n\n");
                Console.ForegroundColor = ConsoleColor.White;

            }
            else
            {
                
                var MyIni = new IniFile("settings.ini");
                var Mondayperiod1T = MyIni.Read("MondayPeriod1Time", "Periods");
                var Mondayperiod2T = MyIni.Read("MondayPeriod2Time", "Periods");
                var Mondayperiod3T = MyIni.Read("MondayPeriod3Time", "Periods");

                var period1T = MyIni.Read("Period1Time", "Periods");
                var period2T = MyIni.Read("Period2Time", "Periods");
                var period3T = MyIni.Read("Period3Time", "Periods");

                var period1L = MyIni.Read("Period1Link", "Links");
                var period2L = MyIni.Read("Period2Link", "Links");
                var period3L = MyIni.Read("Period3Link", "Links");

                var soundsEnabled = Convert.ToBoolean(MyIni.Read("Mute Sounds", "Sound"));
                var introSound = MyIni.Read("Start Up Sound", "Sound");


                if(string.IsNullOrEmpty(introSound))
                {
                    SoundPlayer audio = new SoundPlayer(Zoom_Auto_Join.Properties.Resources.join);
                    if (!soundsEnabled) { audio.Play(); }
                }
                else
                {
                    SoundPlayer audio = new SoundPlayer(introSound);
                    if (!soundsEnabled) { audio.Play(); }
                }
                
                

                DateTime MondaytimeValue1 = Convert.ToDateTime(Mondayperiod1T);
                DateTime MondaytimeValue2 = Convert.ToDateTime(Mondayperiod2T);
                DateTime MondaytimeValue3 = Convert.ToDateTime(Mondayperiod3T);


                DateTime timeValue1 = Convert.ToDateTime(period1T);
                DateTime timeValue2 = Convert.ToDateTime(period2T);
                DateTime timeValue3 = Convert.ToDateTime(period3T);
                // timeValue1.ToString("HH:mm:ss") RETURNS AM/PM TO 24HR clock

                Console.ForegroundColor = ConsoleColor.Red;
                Console.BackgroundColor = ConsoleColor.White;
                Console.WriteLine("███████╗░█████╗░░█████╗░███╗░░░███╗");
                Console.WriteLine("╚════██║██╔══██╗██╔══██╗████╗░████║");
                Console.WriteLine("░░███╔═╝██║░░██║██║░░██║██╔████╔██║");
                Console.WriteLine("██╔══╝░░██║░░██║██║░░██║██║╚██╔╝██║");
                Console.WriteLine("███████╗╚█████╔╝╚█████╔╝██║░╚═╝░██║");
                Console.WriteLine("╚══════╝░╚════╝░░╚════╝░╚═╝░░░░░╚═╝");
                Console.WriteLine("");
                Console.WriteLine("░█████╗░██╗░░░██╗████████╗░█████╗░░░░░░░░░░░░██╗░█████╗░██╗███╗░░██╗███████╗██████╗░");
                Console.WriteLine("██╔══██╗██║░░░██║╚══██╔══╝██╔══██╗░░░░░░░░░░░██║██╔══██╗██║████╗░██║██╔════╝██╔══██╗");
                Console.WriteLine("███████║██║░░░██║░░░██║░░░██║░░██║█████╗░░░░░██║██║░░██║██║██╔██╗██║█████╗░░██████╔╝");
                Console.WriteLine("██╔══██║██║░░░██║░░░██║░░░██║░░██║╚════╝██╗░░██║██║░░██║██║██║╚████║██╔══╝░░██╔══██╗");
                Console.WriteLine("██║░░██║╚██████╔╝░░░██║░░░╚█████╔╝░░░░░░╚█████╔╝╚█████╔╝██║██║░╚███║███████╗██║░░██║");
                Console.WriteLine("╚═╝░░╚═╝░╚═════╝░░░░╚═╝░░░░╚════╝░░░░░░░░╚════╝░░╚════╝░╚═╝╚═╝░░╚══╝╚══════╝╚═╝░░╚═╝");
                Console.BackgroundColor = ConsoleColor.Black;
                Console.WriteLine("Made by smashguns#6175");
                Console.ForegroundColor = ConsoleColor.White;
                //new Zoom(new string[] { new string[] { "Physics", "12:00:00 AM", "youtube.com" }, new string[] { "English", "12:01:00 AN", "google.com" } });
                

                if (DateTime.Now.DayOfWeek == DayOfWeek.Monday)
                {
                    Console.WriteLine("Today is monday");
                    /*
                    var zoom1 = new Zoom(period1L, MondaytimeValue1.ToString("HH:mm:ss"));        
                    zoom1.joinClassOnReady();
                      
                    var zoom2 = new Zoom(period2L, MondaytimeValue2.ToString("HH:mm:ss"));
                    zoom2.joinClassOnReady();

                    var zoom3 = new Zoom(period3L, MondaytimeValue3.ToString("HH:mm:ss"));
                    zoom3.joinClassOnReady();*/
                }
                else
                {
                    /*
                    var zoom1 = new Zoom(period1L, timeValue1.ToString("HH:mm:ss"));
                    zoom1.joinClassOnReady();
                    
                    var zoom2 = new Zoom(period2L, timeValue2.ToString("HH:mm:ss"));
                    zoom2.joinClassOnReady();

                    var zoom3 = new Zoom(period3L, timeValue3.ToString("HH:mm:ss"));
                    zoom3.joinClassOnReady();*/
                }
                Console.WriteLine("Seems like classess ended");
            }
            
            
            client.UpdateEndTime();
            client.Dispose();
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
        }
    }
}
