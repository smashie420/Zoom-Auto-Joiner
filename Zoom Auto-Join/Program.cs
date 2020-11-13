using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Threading;
using DiscordRPC;

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
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("WARNING: \n" + txt + "\n");
            Console.ForegroundColor = ConsoleColor.White;
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
                throwSuccess("Waiting for class to start, sit back relax");
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

                    Console.WriteLine("Waiting to join " + className[collumID]+ "...");
                }
                
                
                
              
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
                                if (String.IsNullOrWhiteSpace(JoinSound))
                                {
                                    SoundPlayer audio = new SoundPlayer(Zoom_Auto_Join.Properties.Resources.intro);
                                    if (enableSounds) { audio.Play(); }
                                }
                                else
                                {
                                    SoundPlayer audio = new SoundPlayer(JoinSound);
                                    if (enableSounds) { audio.Play(); }
                                }

                                SendWebHook(DateTime.Now.ToString("HH:mm:ss tt"), classLinks[y]);
                            }
                        }
                        if (isItClassTime(mondayTimes[x]))
                        {
                            if(DateTime.Now.DayOfWeek == DayOfWeek.Monday) { 
                                Console.WriteLine("Today is monday, using mondays schedule!");
                                // Add a check to see if sounds disabled and if custom sound
                                if(String.IsNullOrWhiteSpace(JoinSound))
                                {
                                    SoundPlayer audio = new SoundPlayer(Zoom_Auto_Join.Properties.Resources.intro);
                                    if (enableSounds) { audio.Play(); }
                                }
                                else
                                {
                                    SoundPlayer audio = new SoundPlayer(JoinSound);
                                    if (enableSounds) { audio.Play(); }
                                }
                                
                                
                                

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
		""class"": ""Physics"",
		""mondayTime"": ""10:30:10 PM"",
		""regularTime"": ""10:59:30 PM"",
		""link"": ""google.com""
	    },
	    {
		""class"": ""English"",
		""mondayTime"": ""09:37:10 PM"",
		""regularTime"": ""09:59:20 PM"",
		""link"": ""foo.com""
	    },
        {
		""class"": ""Class3"",
		""mondayTime"": ""09:37:10 PM"",
		""regularTime"": ""09:59:20 PM"",
		""link"": ""foo.com""
	    },
	    
	],
	""Settings"":[
	{
		""Enable Sounds"": ""true"",
		""Startup Sound Path"": """",
		""Join Sound Path"": """"
	}
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







        public static bool enableSounds { get; set; }
        public static string openSound { get; set; }
        public static string JoinSound { get; set; }
        public static void readSoundData()
        {
            var json = File.ReadAllText("settings.json");

            DataSet dataSet = JsonConvert.DeserializeObject<DataSet>(json);
            DataTable dataSettings = dataSet.Tables["Settings"];
            foreach (DataRow row in dataSettings.Rows)
            {
                enableSounds = Convert.ToBoolean(row["Enable Sounds"]);
                openSound = (string)row["Startup Sound Path"];
                JoinSound = (string)row["Join Sound Path"];
            }
        }

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
            Array[] formattedArr = new Array[4];
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

                //var zoom = new Zoom
                //(new string[,]
                //{
                //{ row["class"].ToString(), row["mondayTime"].ToString(), row["regularTime"].ToString(), row["link"].ToString()  },
                //});

                unformattedArr[i] = new string[] { className[i], mondayTimes[i], regularTimes[i], classLinks[i] };
                
                //Console.WriteLine("READSETTINGS()\nClass {0} \nMonday Time {1} \nRegular Time {2} \nClass Link {3}", className[i], mondayTimes[i], regularTimes[i], classLinks[i]);
                 
                var zoom = new Zoom
                (new string[,]
                {
                    { className[i], mondayTimes[i], regularTimes[i], classLinks[i]  },
                });
                i++;
            }
            

            //formattedArr[] = unformattedArr[];
            /*


            Console.WriteLine(unformattedArr.Length);
            for(int a = 0; a < unformattedArr.Length; a++)
            {
                Console.WriteLine(unformattedArr[a]);
            }
            

            string[,] classess = new string[3, 4];
            int fora = 0;
            //int forb = 0;
            
            foreach (string[] text in unformattedArr)
            {
                if (text == null) { break; }
                //Console.WriteLine("FOR A = " + fora);
                Console.WriteLine("TEST" +text[0]); // Returns info to Get
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
            readSoundData();
            DiscordStatus();
            initializeText();

            if (String.IsNullOrWhiteSpace(openSound))
            {
                SoundPlayer audio = new SoundPlayer(Zoom_Auto_Join.Properties.Resources.intro);
                if (enableSounds) { audio.Play(); }
            }
            else
            {
                if (Directory.Exists(openSound))
                {
                    SoundPlayer audio = new SoundPlayer(openSound);
                    if (enableSounds) { audio.Play(); }
                }
                else
                {
                    throwErr("Open Sound Path doesnt exist! Check Path!");
                }
                
            }
            checkForSettings();


            Console.WriteLine("Seems like classess ended");
            
            client.UpdateEndTime();
            client.Dispose();
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
            
        }
    }
}
