using System;
using WMPLib;
using System.IO;
using System.Threading;

/*
 * CHANGE LOG
 *
 * 1.0.0  - Completed the foundation.
 * 1.1.0  - Added timer, no refresh.
 * 1.2.0  - Added improved timer; code fixes.
 * 1.2.1  - Addition of functions, rearranging code.
 * 1.2.2  - Added creating playlists. Added creating a playlist folder.
 * 1.2.3  - Rewriting some code, added going back to the main menu from the 'playsingle(int)' function. Fixed adding songs for the playlist.
 * 1.2.4  - Rearranging playlist code, 'playsingle(int, bool)' has a new argument; some code added for 'playPList();'.
 * 1.2.5b - Some fixing around the playlist.
 * 1.2.5  - Code fixes.
 *
 * PLANNED
 *
 * Fix bugs.
 * Make it so that 'playing()' is not being refreshed every second.
 * Timeline.
 * File browser, so you dont have to type in the file path.
 * Auto refil path. If you put in half of a name, you should be able to press TAB and it will complete the other half.
 * Adding support for other playlist extensions.
 * When the song finishes, if it's in a playlist the next song should play, if not then it should return to the signle song selection menu. Currently when a song finishes, it just stays in the 'playing()' function.
 *
 * KNOWN BUGS AND ERRORS
 * 
 * Playlist: The program crashes if it's an empty .txt file.
 * Playlist: Buggy as fuck, don't know why.
 *
*/


namespace SAP
{
    class Program
    {
        static void printlogo()  //printing of the program logo text
        {
            string version = "1.2.5";

            Console.Clear();
            Console.WriteLine("{0}", version);
            Console.WriteLine("     _______ __                  __          _______           __ __        ");
            Console.WriteLine("    |     __|__|.--------.-----.|  |.-----. |   _   |.--.--.--|  |__|.-----.");
            Console.WriteLine("    |__     |  ||        |  _  ||  ||  -__| |       ||  |  |  _  |  ||  _  |");
            Console.WriteLine("    |_______|__||__|__|__|   __||__||_____| |___|___||_____|_____|__||_____|");
            Console.WriteLine("                         |__|                                               ");
            Console.WriteLine("                       ______ __                         ");
            Console.WriteLine("                      |   __ \\  |.---.-.--.--.-----.----.");
            Console.WriteLine("                      |    __/  ||  _  |  |  |  -__|   _|");
            Console.WriteLine("                      |___|  |__||___._|___  |_____|__|  ");
            Console.WriteLine("                                       |_____|           ");
            Console.WriteLine("");
            Console.WriteLine("");
        }

        static void firstSetup()  //creating the necessary folder
        {
            string path = @"playlists";

            try
            {
                // check if folder exists
                if (Directory.Exists(path))
                {
                    return;
                }

                // create new folder
                DirectoryInfo di = Directory.CreateDirectory(path);
            }
            catch (Exception e)
            {
                //message if error
                Console.WriteLine("Directorium creation process failed: {0}", e.ToString());
                Environment.Exit(0);
            } 
        }

        protected static int origRow;
        protected static int origCol;
        protected static void WriteAt(string s, int x, int y)  //function for printing text on specified coordinates
        {
            try
            {
                Console.SetCursorPosition(origCol + x, origRow + y);
                Console.Write(s);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.Clear();
                Console.WriteLine(e.Message);
            }
        }

        static int homescreen()  //main menu / selection screen
        {
            string cursor1 = " ", cursor2 = " ", cursor3 = " ";
            int selection = 1;

            //selection screen loop
            bool loop = true;
            while (loop)
            {
                printlogo();

                if (selection == 1)
                {
                    cursor1 = ">";
                    cursor2 = " ";
                    cursor3 = " ";
                }
                else if (selection == 2)
                {
                    cursor1 = " ";
                    cursor2 = ">";
                    cursor3 = " ";
                }
                else if (selection == 3)
                {
                    cursor1 = " ";
                    cursor2 = " ";
                    cursor3 = ">";
                }

                Console.WriteLine("  {0} Play single song", cursor1);
                Console.WriteLine("  {0} Create a new playlist", cursor2);
                Console.WriteLine("  {0} Select an existing playlist", cursor3);

                //read key input and do selection
                System.ConsoleKey input = Console.ReadKey().Key;

                if (input == System.ConsoleKey.UpArrow)
                {
                    if (selection == 1)
                    {
                        selection = 3;
                    }
                    else if (selection == 2)
                    {
                        selection = 1;
                    }
                    else if (selection == 3)
                    {
                        selection = 2;
                    }
                }

                if (input == System.ConsoleKey.DownArrow)
                {
                    if (selection == 1)
                    {
                        selection = 2;
                    }
                    else if (selection == 2)
                    {
                        selection = 3;
                    }
                    else if (selection == 3)
                    {
                        selection = 1;
                    }
                }

                if (input == System.ConsoleKey.Enter)
                {
                    loop = false;
                }

            }

            return selection;
        }

        static int playing(string source, bool playlistMode)
        {
            //music player - check if .mp3 or .wav file; error if not, play song if otherwise 
            if ((source.Contains(".mp3") || source.Contains(".wav")) && File.Exists(source))
            {
                WMPLib.WindowsMediaPlayer wplayer = new WMPLib.WindowsMediaPlayer();
                string position;
                ConsoleKeyInfo input;
                wplayer.URL = source;
                wplayer.controls.stop();
                wplayer.controls.play();

                wplayer.settings.volume = 50;

                //loop for the player controls
                bool loop = true;
                while (loop == true)
                {
                    if (playlistMode == false)  //prints different screents for playlist mode                                           <- needs to be better done, without the massive printing
                    {
                        while (Console.KeyAvailable == false)
                        {

                            printlogo();
                            Console.WriteLine("Now playing: {0}", Path.GetFileNameWithoutExtension(source));
                            Console.WriteLine("Volume: {0}", wplayer.settings.volume);

                            position = wplayer.controls.currentPositionString; //timer

                            Console.WriteLine("\n{0}\n", position);
                            Console.WriteLine("Up/Down   - Volume control");
                            Console.WriteLine("P         - Pause");
                            Console.WriteLine("S         - Stop");
                            Console.WriteLine("Backspace - Return to main menu");

                            Thread.Sleep(500); //refresh
                        }
                    }
                    else
                    {
                        while (Console.KeyAvailable == false)
                        {

                            printlogo();
                            //Console.WriteLine("Playlist: {0}", Program.playPList.input);                                         //   <- This part doesnt work, needs to be fixed
                            Console.WriteLine("Now playing: {0}", Path.GetFileNameWithoutExtension(source));
                            Console.WriteLine("Volume: {0}", wplayer.settings.volume);

                            position = wplayer.controls.currentPositionString; //timer

                            Console.WriteLine("\n{0}\n", position);
                            Console.WriteLine("Up/Down   - Volume control");
                            Console.WriteLine("P         - Pause");
                            Console.WriteLine("S         - Skip song");
                            Console.WriteLine("Backspace - Return to main menu");

                            Thread.Sleep(500); //refresh
                        }
                    }

                    input = Console.ReadKey(); //read key input

                    if (input.Key == ConsoleKey.UpArrow) //volume up
                    {
                        wplayer.settings.volume += 1;
                    }
                    else if (input.Key == ConsoleKey.DownArrow) //volume down
                    {
                        wplayer.settings.volume -= 1;
                    }
                    else if (input.Key == ConsoleKey.P) //song pause
                    {
                        wplayer.controls.pause();
                        Console.WriteLine("\nThe song is paused, press any key to unpause.");
                        Console.ReadKey();
                        wplayer.controls.play();
                    }
                    else if (input.Key == ConsoleKey.S) //stop
                    {
                        wplayer.controls.stop();
                        loop = false;
                        return 1;
                    }
                    else if (input.Key == ConsoleKey.Backspace) //return to main menu
                    {
                        wplayer.controls.stop();
                        loop = false;
                        return 0;
                    }
                }
            }
            else
            {
                //message if the file path is wrong or wrong format
                Console.WriteLine("\nThe location is either incorrect or the file type is not supported.");
                Console.ReadLine();
            }
            return 1;
        }

        static void playsingle()  //single song selection
        {
            int loop = 1;
            while (loop == 1)
            {
                printlogo();
                Console.WriteLine("Write in 'back' to return.");
                Console.WriteLine("Write in the location of an audio file (C:\\user\\music\\jam.mp3):");
                Console.Write("> ");
                string source = Console.ReadLine();

                if (source != "back")
                {
                    loop = playing(source, false);
                }
                else
                {
                    loop = 0;
                }
            }
        }

        static void createPList()  //playlist creation
        {
            printlogo();
            Console.WriteLine("Type in 'back' to return");
            Console.Write("Enter playlist name: ");
            string playlistName = Console.ReadLine();

            if (playlistName == "back")
            {
                return;
            }
            
            //creation of the text document where the song sources are stored
            string playpath = @"playlists\" + playlistName + ".txt";  

            using (StreamWriter writer = new StreamWriter(playpath, true))
            {
                bool loop = true;
                while (loop == true) //inputing sources into the playlist text file
                {
                    Console.WriteLine("Type in 'done' to finish.");
                    Console.WriteLine("Write in the location of an audio file (C:\\user\\music\\jam.mp3):");
                    Console.Write("> ");
                    string source = Console.ReadLine();

                    //check if good path and/or format
                    if ((source.Contains(".mp3") || source.Contains(".wav")) && File.Exists(source))
                    {
                        writer.WriteLine(source);

                        Console.WriteLine("The song has been added.\n");
                    }
                    else if (source == "done")
                    {
                        loop = false;
                        break;
                    }
                    else
                    {
                        Console.WriteLine("\nThe location is either incorrect or the file type is not supported.");
                        Console.ReadLine();
                        Console.WriteLine("");
                    }
                }
            }

            //playlist input end message
            Console.WriteLine("\nThe playlist has been created, select it from the main menu.");
            Console.WriteLine("Press any key to return to main menu.");
            Console.ReadKey();
        }

        static void playPList()  //playlist selection
        {
            origRow = Console.CursorTop;
            origCol = Console.CursorLeft;

            printlogo();
            Console.WriteLine("There will be a cursor added to this part, for now use text.\n");
            Console.WriteLine("Select a playlist: ");
            
            //write all text documents inside playlist directory        <-       (Theres a problem here if the .txt does not contain music path sources or is empty. There should be an exclusive file type for playlist files (eg. '.sapplist'))
            string[] filePaths = Directory.GetFiles(@"playlists");
            for (int i = 0; i < filePaths.Length; ++i)
            {
                string path = filePaths[i];
                if (path.Contains(".txt"))
                {
                    Console.WriteLine("   {0}\t\tPlay\tEdit", System.IO.Path.GetFileNameWithoutExtension(path));
                }
            }
            Console.WriteLine("\n   Back");

            Console.Write("\nType in the playlist name: ");
            string input = Console.ReadLine();

            if (input == "back")
            {
                return;
            }
        /*    else if(!input.Contains(".txt"))
            {
                Console.WriteLine("\nThe playlist name is either incorrect or the type is not supported.\nPress any key to return to main menu.");
                Console.ReadKey();
                return;
            }
*/
            string playpath = @"playlists\" + input + ".txt";

            int loop = 1;
            string source;
            using (StreamReader reader = new StreamReader(playpath))  //play song from playlist
            {
                while ((loop == 1) || (reader.ReadLine() != null)) {
                    source = reader.ReadLine();
                    loop = playing(source, true);

                    if (reader.ReadLine() == null){
                        break;
                    }
                }
            }
            printlogo();
            Console.WriteLine("The playlist is over. Press any key to return to main menu.");
            Console.ReadKey();

            /*             ˅ ˅ ˅       THE CURSOR CODE                                                                                         <- This should be added instead of text input
            //hide cursor, setting variables
            Console.CursorVisible = false;
            int x = 1, y = -2;
            bool loop = true;
            while (loop == true)  //loop for selection
            {
                WriteAt(">", x, y);  //draw cursor

                System.ConsoleKey input = Console.ReadKey().Key;
                if (input == System.ConsoleKey.UpArrow)
                {

                }
                else if (input == System.ConsoleKey.DownArrow)
                {

                }
            }*/

        }

        static void Main(string[] args)
        {
            firstSetup();

            bool loop = true;
            while (loop == true)
            {
                int input;
                input = homescreen();

                if (input == 1)
                {
                    playsingle();
                }
                else if (input == 2)
                {
                    createPList();
                }
                else if (input == 3)
                {
                    playPList();
                }
            }
        }
    }
}