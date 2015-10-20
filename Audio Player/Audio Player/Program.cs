using System;
using WMPLib;
using System.IO;
using System.Threading;

/*
 * CHANGE LOG
 *
 * 1.0.0  - Zavrsen osnovni program.
 * 1.1.0  - Dodat tajmer, nema refresh.
 * 1.2.0  - Dodat ispravan tajmer; code fixes.
 * 1.2.1  - Dodavanje funkcija, odvajanje i rasporedjivanje koda.
 * 1.2.2  - Dodavanje koda za kreiranje plejliste. Dodato kreiranje foldera za plejliste.
 * 1.2.3  - Menjanja koda, dodato vracanje u glavni meni iz funkcije playsingle(). Popravljeno dodavanja pesama u plejlistu.
 * 1.2.4  - Preuredjivanje koda za plejlistu, playsingle() ima novi argument; dodavanje koda za playPList();
 * 1.2.5b - Dodat funkcionalan playlist();
 * 1.2.5  - Popravke.
 *
 * PLANIRANO
 *
 * Popraviti bugove.
 * Srediti playing() da se ne refreshuje svake sekunde.
 * Timeline.
 * File browser, da se ne kuca cela adresa za pesmu.
 * Auto refil file name-a. Kad se ukuca polovicno ime i klikne tab, da ga popuni.
 * Dodati podrsku za druge ekstenzije plejlista
 * Kad se zavrsi pesma, ako je u plejlisti da se pokrene sledeca ili ako nije da se vrati u biranje pesme. Trenutno ako se zavrsi pesma samo ostane u playing() funkciji.
 * 
 * TRENUTNE GRESKE
 * 
 * Plejlista: ako se pokrene prazan .txt fajl program kresuje.
 * Plejlista baguje ko kurac, nzm zasto i sta.
 *
*/


namespace SAP
{
    class Program
    {
        static void printlogo()  //stampanje znaka programa
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

        static void firstSetup()  //kreiranje potrebnih foldera
        {
            string path = @"playlists";

            try
            {
                // gleda da li postoji direktorijum
                if (Directory.Exists(path))
                {
                    return;
                }

                // kreira novi direktorijum
                DirectoryInfo di = Directory.CreateDirectory(path);
            }
            catch (Exception e)
            {
                //poruka ako proces failuje i izlazenje iz programa
                Console.WriteLine("Directorium creation process failed: {0}", e.ToString());
                Environment.Exit(0);
            } 
        }

        protected static int origRow;
        protected static int origCol;
        protected static void WriteAt(string s, int x, int y)  //funkcija za ispisivanje stringa na zeljenim kordinatama
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

        static int homescreen()  //pocetni meni, biranje opcija.
        {
            string cursor1 = " ", cursor2 = " ", cursor3 = " ";
            int selection = 1;

            //petlja selection screen-a
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

                //citanje key inputa i odredjivanje selekcije
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
            //gledanje da li je mp3 ili wav fajl, pustanje muzike i kontrole
            if ((source.Contains(".mp3") || source.Contains(".wav")) && File.Exists(source))
            {
                WMPLib.WindowsMediaPlayer wplayer = new WMPLib.WindowsMediaPlayer();
                string position;
                ConsoleKeyInfo input;
                wplayer.URL = source;
                wplayer.controls.stop();
                wplayer.controls.play();

                wplayer.settings.volume = 50;

                //petlja za kontrole
                bool loop = true;
                while (loop == true)
                {
                    if (playlistMode == false)  //stampa dva razlicita menija
                    {
                        while (Console.KeyAvailable == false)
                        {

                            printlogo();
                            Console.WriteLine("Now playing: {0}", Path.GetFileNameWithoutExtension(source));
                            Console.WriteLine("Volume: {0}", wplayer.settings.volume);

                            position = wplayer.controls.currentPositionString; //tajmer

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
                            //Console.WriteLine("Playlist: {0}", Program.playPList.input);                                     //<<<                        Ne radi, treba popraviti
                            Console.WriteLine("Now playing: {0}", Path.GetFileNameWithoutExtension(source));
                            Console.WriteLine("Volume: {0}", wplayer.settings.volume);

                            position = wplayer.controls.currentPositionString; //tajmer

                            Console.WriteLine("\n{0}\n", position);
                            Console.WriteLine("Up/Down   - Volume control");
                            Console.WriteLine("P         - Pause");
                            Console.WriteLine("S         - Skip song");
                            Console.WriteLine("Backspace - Return to main menu");

                            Thread.Sleep(500); //refresh
                        }
                    }

                    input = Console.ReadKey(); //citanje key input-a

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
                    else if (input.Key == ConsoleKey.Backspace) //vracanje u glavni meni
                    {
                        wplayer.controls.stop();
                        loop = false;
                        return 0;
                    }
                }
            }
            else
            {
                //poruka ako je ukucana pogresna lokacija ili ime
                Console.WriteLine("\nThe location is either incorrect or the file type is not supported.");
                Console.ReadLine();
            }
            return 1;
        }

        static void playsingle()  //pustanje jedne pesme
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

        static void createPList()  //kreiranje plejliste
        {
            printlogo();
            Console.WriteLine("Type in 'back' to return");
            Console.Write("Enter playlist name: ");
            string playlistName = Console.ReadLine();

            if (playlistName == "back")
            {
                return;
            }
            
            //kreiranje tekst dokumenta
            string playpath = @"playlists\" + playlistName + ".txt";  

            using (StreamWriter writer = new StreamWriter(playpath))
            {
                bool loop = true;
                while (loop == true) //unosenje pesama u plejlistu
                {
                    Console.WriteLine("Type in 'done' to finish.");
                    Console.WriteLine("Write in the location of an audio file (C:\\user\\music\\jam.mp3):");
                    Console.Write("> ");
                    string source = Console.ReadLine();

                    //provera da li je dobro unesen source
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

            //poruka i zatvaranje pisanja kreiranje plejliste
            Console.WriteLine("");
            Console.WriteLine("The playlist has been created, select it from the main menu.");
            Console.WriteLine("Press any key to return to main menu.");
            Console.ReadKey();
        }

        static void playPList()  //selektovanje i pustanje plejliste
        {
            origRow = Console.CursorTop;
            origCol = Console.CursorLeft;

            printlogo();
            Console.WriteLine("There will be a cursor added to this part, for now use text.\n");
            Console.WriteLine("Select a playlist: ");
            
            //ispisivanje tekstualnih fajlova u direktorijumu (ovde ima problem ako postoji neki tekstualni dokument koji ne sadrzi lokacije pesama)
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
            else if(!input.Contains(".txt"))
            {
                Console.WriteLine("\nThe playlist name is either incorrect or the type is not supported.\nPress any key to return to main menu.");
                Console.ReadKey();
                return;
            }

            string playpath = @"playlists\" + input + ".txt";

            int loop = 1;
            string source;
            using (StreamReader reader = new StreamReader(playpath))  //pustanje pesama iz plejliste
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

            /*             ˅ ˅ ˅       CURSOR DEO                                                                                                         <<<---  OVO PLANIRANO
            //skrivanje kursora, postavljanje varijabla.
            Console.CursorVisible = false;
            int x = 1, y = -2;
            bool loop = true;
            while (loop == true)  //petlja za biranje plejliste
            {
                WriteAt(">", x, y);  //crtanje kursora

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