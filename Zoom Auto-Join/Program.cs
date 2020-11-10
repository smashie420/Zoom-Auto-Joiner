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
                { "Physics", "11:40:30 PM", "12:00:00 AM", "google.com"  },
                { "English", "9:00:00 AM", "12:01:00 AM", "youutube.com" },
                { "Math", "10:00:00 AM", "12:03:00 AM", "foo.com" },
           }
         */
        
        public class Zoom
        {
            // Constructor that takes one argument:
            public Zoom(string[,] classess)
            {
                string[] className = { classess[0,0], classess[1,0], classess[2,0] };
                string[] mondayTimes = { AmpmTo24(Convert.ToDateTime(classess[0, 1])), AmpmTo24(Convert.ToDateTime(classess[1, 1])), AmpmTo24(Convert.ToDateTime(classess[2, 1])) };
                string[] regularTimes = { AmpmTo24(Convert.ToDateTime(classess[0, 2])), AmpmTo24(Convert.ToDateTime(classess[1, 2])), AmpmTo24(Convert.ToDateTime(classess[2, 2])) };
                string[] classLinks = { classess[0, 3], classess[1, 3], classess[2, 3] };

                while (true)
                {
                    for (int x = 0; x < mondayTimes.Length; x++)
                    {
                        for (int y = 0; y < regularTimes.Length; y++)
                        {
                            
                            if (isItClassTime(regularTimes[y]))
                            {
                                Console.WriteLine("{0} has started, joinning link {1}", className[y], classLinks[y]);
                                joinClass(classLinks[y]);
                                SendWebHook(DateTime.Now.ToString("HH:mm:ss tt"), classLinks[y]);
                            }
                        }
                        if (isItClassTime(mondayTimes[x]))
                        {
                            if(DateTime.Now.DayOfWeek == DayOfWeek.Monday) { 
                                Console.WriteLine("Today is monday, using mondays schedule!");
                                // Add a check to see if sounds disabled and if custom sound
                                SoundPlayer audio = new SoundPlayer(Zoom_Auto_Join.Properties.Resources.intro);
                                audio.Play();

                                Console.WriteLine("{0} has started, joinning link {1}", className[x], classLinks[x]);
                                joinClass(classLinks[x]);
                                SendWebHook(DateTime.Now.ToString("HH:mm:ss tt"), classLinks[x]);
                            }
                        }
                    }
                    Thread.Sleep(100);
                }
            }

            public bool isItClassTime(string time)
            {
                if(DateTime.Now.ToString("HH:mm:ss") == time)
                {
                    return true;
                }
                return false;
            }

            public void joinClass(string link)
            {
                Process.Start("chrome.exe", link);
                Console.WriteLine("Joining class link: " + link);
                Thread.Sleep(5000);
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

            int single = rnd.Next(1, 6);
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

        
        public async static void SendWebHook(string joinTime, string link)
        {
            var client = new DiscordWebhookClient("https://discordapp.com/api/webhooks/775279806773985292/XqzTnS5Ako0fRdeXkiAa18CgOwtliRCrHGwqNX-O5LYesg4rCak26PsAa7soHSdwaKWe");

            // Create your DiscordMessage with all parameters of your message.
            var message = new DiscordMessage(
                DateTime.Now.ToString("MMM dd, yyyy HH:mm:ss tt"),
                username: "Zoom Logs",
                avatarUrl: "https://www.publiccounsel.net/train/wp-content/uploads/sites/13/5e8ce318664eae0004085461.png",
                tts: false,
                embeds: new[]
                {
                    new DiscordMessageEmbed(
                        "Current Logs",
                        color: 0,
                        author: new DiscordMessageEmbedAuthor("By smashguns#6175"),
                        //url: "https://www.publiccounsel.net/train/wp-content/uploads/sites/13/5e8ce318664eae0004085461.png",
                        //description: "This is a embed description.",
                        fields: new[]
                        {
                            new DiscordMessageEmbedField("Joined Time", joinTime),
                            new DiscordMessageEmbedField("Link", link)
                        },
                       
                        footer: new DiscordMessageEmbedFooter("Made by smashguns#6175 • " + DateTime.Now, "https://cdn.discordapp.com/avatars/242889488785866752/40ee66d845e1a6341e03c450fcf6d221.png?size=256")
                    )
                }
                );

            // Send the message!
            await client.SendToDiscord(message);
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
                            { "embeds", embed }
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

                var zoom = new Zoom
                (new string[,]
                {
                    { "Physics", "01:02:30 AM", "01:11:30 AM", "google.com"  },
                    { "English", "12:09:40 AM", "12:19:35 AM", "youtube.com" },
                    { "Math", "10:00:00 AM", "12:03:00 AM", "foo.com" },
                });
                Console.WriteLine("Seems like classess ended");
            }
            
            
            client.UpdateEndTime();
            client.Dispose();
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
        }
    }
}
