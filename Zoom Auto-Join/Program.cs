using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Threading;
using DiscordRPC;

// USE THIS
using JNogueira.Discord.Webhook.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Zoom_Auto_Join
{
    class Program
    {
        
        public static int getCollums(Array arr)
        {
            int rows = arr.Length / 4;
            int result = 0;
            for (int rowsInArray = 0; rowsInArray < rows; rowsInArray++)
            {
                result++;
            }
            return result;
        }
        public static int getRows(Array arr)
        {
            int rows = arr.Length / 4;
            int result = 0;
            for (int a = 0; a <= rows; a++)
            {
                result++;
            }
            return result;
        }

        public static void throwErr(string txt)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("ERROR: \n" + txt + "\n");
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
        }
        public static void throwWarning(string txt)
        {
            Console.BackgroundColor = ConsoleColor.Yellow;
            Console.WriteLine("WARNING: \n" + txt + "\n");
            Console.BackgroundColor = ConsoleColor.Black;
        }
        public static void throwSuccess(string txt)
        {
            Console.BackgroundColor = ConsoleColor.Green;
            Console.WriteLine("Success: \n" + txt + "\n");
            Console.BackgroundColor = ConsoleColor.Black;
        }
        public static void throwDebug(string txt)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Success: \n" + txt + "\n");
            Console.ForegroundColor = ConsoleColor.White;
        }
        /*
            Info to get =               0               1               2               3   
            classess[0,infotoget] = {   "Physics",    "01:02:30 AM",  "01:11:30 AM",  "google.com"  },
            classess[1,infotoget] ={    "English",    "12:09:40 AM",  "12:19:35 AM",  "youtube.com" },
            classess[2,infotoget] = {   "Math",       "10:00:00 AM",  "12:03:00 AM",  "foo.com" },
         */
        public class Zoom
        {
            // Constructor that takes one argument:
            public Zoom(string[,] classess)
            {
                if(getCollums(classess) == 0) { throwErr("Didnt recieve any class data!"); return; }
                //Console.WriteLine("Amount of rows = " + getRows(classess));
                //Console.WriteLine("Amount of Collums = " + getCollums(classess));

                string[] className = new string[classess.Length / 4];
                string[] mondayTimes = new string[classess.Length / 4];
                string[] regularTimes = new string[classess.Length / 4];
                string[] classLinks = new string[classess.Length / 4];
                
                //for (int rowID = 0; rowID < getRows(classess); rowID++)
                //{
                //Console.WriteLine("RowID = " + rowID);
                for (int collumID = 0; collumID < getCollums(classess); collumID++)
                {
                    //Console.WriteLine("CollumID = " + collumID);
                    className[collumID] = classess[collumID, 0].ToString();
                    mondayTimes[collumID] = AmpmTo24(Convert.ToDateTime(classess[collumID, 1].ToString()));
                    regularTimes[collumID] = AmpmTo24(Convert.ToDateTime(classess[collumID, 2].ToString()));
                    classLinks[collumID] = classess[collumID, 3].ToString();
                }
                throwSuccess("Waiting for class to start, sit back relax");
              
                //}
                
                /*
                foreach(string result in mondayTimes)
                {
                    Console.WriteLine(result);
                }
                */

                /*
                string[] className = { classess[0,0], classess[1,0], classess[2,0] };
                string[] mondayTimes = { AmpmTo24(Convert.ToDateTime(classess[0, 1])), AmpmTo24(Convert.ToDateTime(classess[1, 1])), AmpmTo24(Convert.ToDateTime(classess[2, 1])) };
                string[] regularTimes = { AmpmTo24(Convert.ToDateTime(classess[0, 2])), AmpmTo24(Convert.ToDateTime(classess[1, 2])), AmpmTo24(Convert.ToDateTime(classess[2, 2])) };
                string[] classLinks = { classess[0, 3], classess[1, 3], classess[2, 3] };
                */
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

            int single = rnd.Next(1, 5);
            string[] imagetextArr = { "Dying", "Im fucking bored", "Lazzy", "Holy fuck im coding this at 3AM", "Epic" };
            int randomTxtArr = rnd.Next(1, imagetextArr.Length);


            
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
                            new DiscordMessageEmbedField("Joined Time", MilitaryToAmPm(Convert.ToDateTime(joinTime))),
                            new DiscordMessageEmbedField("Link", link)
                        },
                       
                        footer: new DiscordMessageEmbedFooter("Made by smashguns#6175 • " + DateTime.Now, "https://cdn.discordapp.com/avatars/242889488785866752/40ee66d845e1a6341e03c450fcf6d221.png?size=256")
                    )
                }
                );
            await client.SendToDiscord(message);
        }

        public static string AmpmTo24(DateTime time)
        {
            return Convert.ToDateTime(time).ToString("HH:mm:ss");
        }
        public static string MilitaryToAmPm(DateTime time)
        {
            return Convert.ToDateTime(time).ToString("hh:mm:ss") + " " + Convert.ToDateTime(time).ToString("tt");
        }

        public static void initializeText()
        {
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
            Console.WriteLine("Made by smashguns#6175\n\n");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public class settings
        {
            public string className { get; set; }
            public string mondayTime { get; set; }
            public string regularTime { get; set; }
            public string link { get; set; }
        }

        public static void checkForSettings()
        {
            //var MyIni = new IniFile("settings.ini");
            if (File.Exists("settings.json"))
            {
                throwDebug("Reading from settings.json");
                readSettings();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n\n\nsettings.json doesnt exist! Making one now!");
                Console.ForegroundColor = ConsoleColor.White;
                
                using (StreamWriter writer = File.CreateText("settings.json"))
                {
                    writer.WriteLine(@"{
	""Info"": [
	    {
		""class"": ""ClassName3"",
		""mondayTime"": ""12:00:00 AM"",
		""regularTime"": ""12:10:00 AM"",
		""link"": ""google.com""
	    },
	    {
		""class"": ""ClassName2"",
		""mondayTime"": ""01:00:00 AM"",
		""regularTime"": ""02:10:00 AM"",
		""link"": ""google.com""
	    },
	    {
		""class"": ""ClassName3"",
		""mondayTime"": ""03:00:00 AM"",
		""regularTime"": ""04:10:00 AM"",
		""link"": ""google.com""
	    },
	]
}
                   
                    ");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("settings.json has been made!\nEdit the file to your classess!\n\n\n");
                    Console.ForegroundColor = ConsoleColor.White;
                }

            }
        }
        /*
         
            string[] className = new string[classess.Length / 4];
            string[] mondayTimes = new string[classess.Length / 4];
            string[] regularTimes = new string[classess.Length / 4];
            string[] classLinks = new string[classess.Length / 4];
                
            //for (int rowID = 0; rowID < getRows(classess); rowID++)
            //{
            //Console.WriteLine("RowID = " + rowID);
            for (int collumID = 0; collumID < getCollums(classess); collumID++)
            {
                //Console.WriteLine("CollumID = " + collumID);
                className[collumID] = classess[collumID, 0].ToString();
                mondayTimes[collumID] = AmpmTo24(Convert.ToDateTime(classess[collumID, 1].ToString()));
                regularTimes[collumID] = AmpmTo24(Convert.ToDateTime(classess[collumID, 2].ToString()));
                classLinks[collumID] = classess[collumID, 3].ToString();
            }
         */
        public static void readSettings()
        {
            //var settings = MyIni.Read("class[]", "Classess");
            var json = File.ReadAllText("settings.json");

            DataSet dataSet = JsonConvert.DeserializeObject<DataSet>(json);

            DataTable dataTable = dataSet.Tables["Info"];

            //Console.WriteLine(dataTable.Rows.Count);
            // 2


            //string[,] classess = new string[10,4];

            Array[] unformattedArr = new Array[4];

            string[] className = new string[4];
            string[] mondayTimes = new string[4];
            string[] regularTimes = new string[4];
            string[] classLinks = new string[4];
            int i = 0;
            foreach (DataRow row in dataTable.Rows)
            {
                className[i] = row["class"].ToString();
                mondayTimes[i] = row["mondayTime"].ToString();
                regularTimes[i] = row["regularTime"].ToString();
                classLinks[i] = row["link"].ToString();

                //Console.WriteLine(row["class"] + " - " + row["link"]);
                /*
                var zoom = new Zoom
                (new string[,]
                {
                    { row["class"].ToString(), row["mondayTime"].ToString(), row["regularTime"].ToString(), row["link"].ToString()  },
                });*/

                unformattedArr[i] = new string[] { className[i], mondayTimes[i], regularTimes[i], classLinks[i] };
                i++;
            }


            int fora = 0;
            int forb = 0;
            Console.WriteLine(unformattedArr.Length);
            for(int a = 0; a < unformattedArr.Length; a++)
            {
                Console.WriteLine(unformattedArr[a]);
            }
            
            /*

            foreach (string[] text in unformattedArr)
            {
                Console.WriteLine("FOR A = " + fora);
                foreach (string res in text)
                {
                    Console.WriteLine("FOR B = "+ forb);
                    Console.WriteLine(res);
                    
                    forb++;
                }
                fora++;
            }*/

            /*
            Info to get =               0               1               2               3   
            classess[0,infotoget] = {   "Physics",    "01:02:30 AM",  "01:11:30 AM",  "google.com"  },
            classess[1,infotoget] ={    "English",    "12:09:40 AM",  "12:19:35 AM",  "youtube.com" },
            classess[2,infotoget] = {   "Math",       "10:00:00 AM",  "12:03:00 AM",  "foo.com" },
            */
            /*
            var zoom = new Zoom
                (new string[,]
                {
                    { row["class"].ToString(), row["mondayTime"].ToString(), row["regularTime"].ToString(), row["link"].ToString()  },
                });*/
        }


        static void Main(string[] args)
        {
            DiscordStatus();
            initializeText();
            checkForSettings();

            /*
            var zoom = new Zoom
                (new string[,]
                {
                    { "Physics", "01:02:30 PM", "01:11:30 PM", "google.com"  },
                    { "English", "12:09:40 AM", "12:19:35 AM", "youtube.com" },
                    { "Math", "10:00:00 AM", "12:03:00 AM", "foo.com" },
                    { "Math", "10:00:00 AM", "12:03:00 AM", "foo.com" },
                    { "Math", "10:00:00 AM", "12:03:00 AM", "foo.com" },
                    { "Math", "10:00:00 AM", "12:03:00 AM", "foo.com" },
                    { "Math", "10:00:00 AM", "12:03:00 AM", "foo.com" },
                    { "Math", "10:00:00 AM", "12:03:00 AM", "foo.com" },
                    { "Math", "10:00:00 AM", "12:03:00 AM", "foo.com" },
                });
            */

            Console.WriteLine("Seems like classess ended");
            /*
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
            
            */
            client.UpdateEndTime();
            client.Dispose();
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
            
        }
    }
}
