using System;
using WMPLib;
using System.IO;
using System.Threading;
using System.Collections;

/*
 * CHANGE LOG
 *
 * 1.0.0    - Completed the foundation.
 * 1.1.0    - Added timer, no refresh.
 * 1.2.0    - Added improved timer; code fixes.
 * 1.2.1    - Addition of functions, rearranging code.
 * 1.2.2    - Added creating playlists. Added creating a playlist folder.
 * 1.2.3    - Rewriting some code, added going back to the main menu from the 'playsingle(int)' function. Fixed adding songs for the playlist.
 * 1.2.4    - Rearranging playlist code, 'playsingle(int, bool)' has a new argument; some code added for 'playPList();'.
 * 1.2.5b   - Some fixing around the playlist.
 * 1.2.5    - Code fixes.
 * 1.3.0    - Replaced streamreader with hashtable. The playlist should work now.
 * 1.3.1    - Cursor optimisation. Instead of the whole screen printing out again when you press an arrow in cursor menu, now the cursor is set using coordinates.
 *            Added some (non functional) playlist selection menu cursor code. Currently commented at line 365.
 * 1.3.1-1  - You can now drag and drop songs. Cursor fix from select play list to main menu.
 * 1.3.1-2  - Select playlist cursor code edits.
 * 1.3.1-3  - Drag and drop fix.
 * 1.3.1-4  - Added exit from main menu.
 *
 * PLANNED
 *
 * Fix bugs.
 * Make it so that 'playing()' is not being refreshed every second.
 * Timeline.
 * File browser, so you don't have to type in the file path.
 * Auto refill path. If you put in half of a name, you should be able to press TAB and it will complete the other half.
 * Adding support for other playlist extensions.
 * When the song finishes, if it's in a playlist the next song should play, if not then it should return to the single song selection menu. Currently when a song finishes, it just stays in the 'playing()' function.
 *
 * KNOWN BUGS AND ERRORS
 * 
 *
*/


namespace SAP
{
    class Program
    {
        static void printlogo()  //printing of the program logo text
        {
            string version = "1.3.1-4";

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

        //coordinates
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
            int selection = 1;
            printlogo();

            //selection screen
            Console.WriteLine("     Play single song");
            Console.WriteLine("     Create a new playlist");
            Console.WriteLine("     Select an existing playlist");
            Console.WriteLine("");
            Console.WriteLine("     Exit");

            //hide cursor, setting variables, read key input and do selection
            Console.CursorVisible = false;
            int x = 1, y = 13;
            bool cursorLoop = true;
            while (cursorLoop == true)  //loop for selection
            {
                WriteAt("  >", x, y);  //draw cursor

                System.ConsoleKey input = Console.ReadKey().Key;
                if (input == System.ConsoleKey.UpArrow)
                {
                    WriteAt("   ", x, y);
                    if (y == 13)
                    {
                        y += 4;
                        selection = 4;
                    }
                    else if (y == 17)
                    {
                        y -= 2;
                        selection = 3;
                    }
                    else if (y <= 15)
                    {
                        y -= 1;
                        selection -= 1;
                    }
                }
                else if (input == System.ConsoleKey.DownArrow)
                {
                    WriteAt("   ", x, y);
                    if (y == 15)
                    {
                        y += 2;
                        selection = 4;
                    }
                    else if (y == 17)
                    {
                        y = 13;
                        selection = 1;
                    }
                    else if (y >= 13)
                    {
                        y += 1;
                        selection += 1;
                    }
                }
                else if (input == System.ConsoleKey.Enter)
                {
                    cursorLoop = false;
                }
            }

            Console.CursorVisible = true;
            return selection;
        }

        static int playing(string source, bool playlistMode)
        {
            //enables drag and drop
            if (source.Contains("\""))
            {
                source = source.Replace("\"", "");
            }

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
                            //Console.WriteLine("Playlist: {0}", Program.playPList.input);                                         //   <- This part doesn't work, needs to be fixed
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
                Console.WriteLine("Write in the path of an audio file (C:\\user\\music\\jam.mp3) or drag and drop:");
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
                    Console.WriteLine("Write in the path of an audio file (C:\\user\\music\\jam.mp3) or drag and drop:");
                    Console.Write("> ");
                    string source = Console.ReadLine();

                    //enables drag and drop
                    if (source.Contains("\""))
                    {
                        source = source.Replace("\"", "");
                    }

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
            printlogo();
            Console.WriteLine("Select a playlist: ");

            //display all text documents inside playlist directory        <-       (There's a problem here if the .txt does not contain music path sources or is empty. There should be an exclusive file type for playlist files (eg. '.sapplist'))
            int numberOfFiles = 0;
            string[] filePaths = Directory.GetFiles(@"playlists");
            for (int i = 0; i < filePaths.Length; ++i)
            {
                string path = filePaths[i];
                if (path.Contains(".txt"))
                {
                    Console.WriteLine("     {0}\t\tPlay\tEdit", System.IO.Path.GetFileNameWithoutExtension(path));
                }
                numberOfFiles = i;
            }
            Console.WriteLine("\n     Back");

            Console.Write("\nType in the playlist name: ");
            string input = Console.ReadLine();

            if (input == "back")
            {
                return;
            }

            /*//hide cursor, setting variables, read key input and do selection                     <-Cursor
            Console.CursorVisible = false;
            int selection = 1;
            int x = 1, y = 14;
            WriteAt("  >", x, y);
            bool cursorLoop = true;
            while (cursorLoop == true)  //loop for selection
            {
                WriteAt("  >", x, y);  //draw cursor
                Console.Write("|SEL: {0}|", selection);
                Console.Write("|Y  : {0}|", y);
                Console.Write("|NUM: {0}|", numberOfFiles);

                System.ConsoleKey keyInput = Console.ReadKey().Key;
                if (keyInput == System.ConsoleKey.UpArrow)
                {
                    WriteAt("   ", x, y);
                    if (selection == 1)
                    {
                        y = numberOfFiles-1;
                        selection = numberOfFiles-1;
                    }
                    else if (selection < numberOfFiles)
                    {
                        y -= 1;
                        selection -= 1;
                    }
                }
                else if (keyInput == System.ConsoleKey.DownArrow)
                {
                    WriteAt("   ", x, y);
                    if (selection == numberOfFiles)
                    {
                        y = 1;
                        selection = 1;
                    }
                    else if (selection > 4)
                    {
                        y += 1;
                        selection += 1;
                    }
                }
                else if (keyInput == System.ConsoleKey.Enter)
                {
                    cursorLoop = false;
                }
            }
            Console.CursorVisible = true;*/

            //string input = "asd";
            string playpath = @"playlists\" + input + ".txt";

            //read all paths from playlist.txt and input them into a hashtable
            Hashtable hashtable = new Hashtable();
            int j = 1;
            if (File.Exists(playpath))
            {
                using (StreamReader reader = new StreamReader(playpath))
                {
                    while (true)
                    {
                        hashtable[j] = reader.ReadLine();
                        if (hashtable[j] == null)
                        {
                            break;
                        }
                        j += 1;
                    }
                }
            }
            else
            {
                printlogo();
                Console.WriteLine("The selected playlist does not exist or is an incorrect format.\nPress any key to return to main menu.");
                Console.ReadKey();
                return;
            }

            int loop = 1;
            string source;

            //play song from playlist/hashtable
            for (int i = 1; i < j; i++) {
                source = hashtable[i].ToString();
                loop = playing(source, true);

                if(loop == 0)  //break if user selected 'return to main menu'
                {
                    break;
                }
            }

            printlogo();
            Console.WriteLine("The playlist is over. Press any key to return to main menu.");
            Console.ReadKey();
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
                else if (input == 4)
                {
                    return;
                }
            }
        }
    }
}