using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Media;
using System.Net.Http;
using System.Threading;
using DiscordRPC;
using DiscordRPC.Logging;
using JNogueira.Discord.Webhook.Client;


namespace Zoom_Auto_Join
{
    class Program
    {

        public class Zoom
        {
            // Constructor that takes one argument:
            public Zoom(string link, string time)
            {
               Link = link;
               Time = time;
            }
            
            public void joinClassOnReady() {
                
                Console.WriteLine("Waiting to join class at " + Convert.ToDateTime(this.Time).ToString("hh:mm:ss") + " " + Convert.ToDateTime(this.Time).ToString("tt"));
                var MyIni = new IniFile("settings.ini");
                var joinSound = MyIni.Read("Join Sound", "Sound");
                var soundsEnabled = Convert.ToBoolean(MyIni.Read("Mute Sounds", "Sound"));
                while (true)
                {
                    if(this.Time == DateTime.Now.ToString("HH:mm:ss"))
                    {

                        

                        if (string.IsNullOrEmpty(joinSound))
                        {
                            SoundPlayer audio = new SoundPlayer(Zoom_Auto_Join.Properties.Resources.join);
                            if (!soundsEnabled) { audio.Play(); }
                        }
                        else
                        {
                            SoundPlayer audio = new SoundPlayer(joinSound);
                            if (!soundsEnabled) { audio.Play(); }
                        }

                        //SoundPlayer audio = new SoundPlayer(Zoom_Auto_Join.Properties.Resources.intro);
                        //if (!soundEnabled) { audio.Play(); }

                        Process.Start("chrome.exe", this.Link);
                        Console.WriteLine("Joining class link: " + Link);
                        break;
                    }
                    Thread.Sleep(100);
                }
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
                Details = "made by smashguns#6175",
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


        static void Main(string[] args)
        {
            DiscordStatus();
            

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
                var Mondayperiod1T = MyIni.Read("Period1Time", "Periods");
                var Mondayperiod2T = MyIni.Read("Period2Time", "Periods");
                var Mondayperiod3T = MyIni.Read("Period3Time", "Periods");

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
                
                

                DateTime MondaytimeValue1 = Convert.ToDateTime(period1T);
                DateTime MondaytimeValue2 = Convert.ToDateTime(period2T);
                DateTime MondaytimeValue3 = Convert.ToDateTime(period3T);

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
                
                if (DateTime.Now.DayOfWeek == DayOfWeek.Monday)
                {
                    var zoom1 = new Zoom(period1L, MondaytimeValue1.ToString("HH:mm:ss"));        
                    zoom1.joinClassOnReady();
                      
                    var zoom2 = new Zoom(period2L, MondaytimeValue2.ToString("HH:mm:ss"));
                    zoom2.joinClassOnReady();

                    var zoom3 = new Zoom(period3L, MondaytimeValue3.ToString("HH:mm:ss"));
                    zoom3.joinClassOnReady();
                }
                else
                {
                    var zoom1 = new Zoom(period1L, timeValue1.ToString("HH:mm:ss"));
                    zoom1.joinClassOnReady();
                    
                    var zoom2 = new Zoom(period2L, timeValue2.ToString("HH:mm:ss"));
                    zoom2.joinClassOnReady();

                    var zoom3 = new Zoom(period3L, timeValue3.ToString("HH:mm:ss"));
                    zoom3.joinClassOnReady();
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
