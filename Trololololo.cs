using System;
using System.Runtime.InteropServices;
using WMPLib;

/*  Copyright (c) Chris Henk 2016. All rights reserved.
 *  THIS PRODUCT COMES WITH ABSOLUTELY NO WARRANTY, EXPRESSED OR IMPLIED. USE AT YOUR OWN RISK.
 *  
 *  THIS PROGRAM IS INTENDED FOR USE ON YOUR OWN MACHINES. INSTALLING AND/OR EXECUTING THIS
 *  CODE ON ANY MACHINE THAT YOU ARE NOT THE OWNER OF OR WHOSE OWNER NOT HAS GRANTED EXPLICIT WRITTEN 
 *  PERMISSION FOR IS A FEDERAL OFFENSE.
 *  
 *  THE AUTHOR OF THIS CODE WILL NOT BE RESPONSIBLE FOR ANY MISSUSE OF IT ON THE PART OF THE USER 
 *  
 *  This is a pre-release beta, DO NOT DISTRIBUTE. It will be made available
 *  with an appropriate open source license once distribution arrangements
 *  have been made.
 */ 

/*  Command Line args:
 *  rick_roll_keyboard.exe <SAMPLE_RATE> <CHAR_REPEAT_LIMIT> <GHOST_PERIOD> <MIN_WPM>
 *  -r <SAMPLE_RATE> [int]: Rate (Hz) the keyboard is checked at. Default=20
 *  -g <GHOST_PERIOD> [int]: Period of time after a key input the music plays. This prevents chopy
 *                           effects caused by slow typing speed. By default it is calculated from
 *                           MIN_WPM. MIN_WPM will be ignored if this flag is provided
 *  -w <MIN_WPM> [int]: The minimum words per minute required to maintain constant audio. This is
 *                      a convenience flag that is a more user friendly method of manipulating the
 *                      GHOST_PERIOD. Default=45
 *  -c [void]: Including the -c flag will cause the program to continually play the music. Useful
 *             for pulling the ole switcheroo e.g. replace the I.E. desktop icon with a batch
 *             script that runs this executable with the -c flag
 */

namespace Rick_Roll_Keyboard
{
    class Trololololo
    {
        // Divide any WPM by this number to conver to characters per second
        const int WPM_TO_CPM_FACTOR = 13;

        // Create media player
        static WindowsMediaPlayer music = new WindowsMediaPlayer();

        static void LoadMusic(bool play)
        {
            // Load the mp3
            music.URL = "rekt.mp3";

            // Stop the music if instructed to do so
            if (!play)
            {
                System.Threading.Thread.Sleep(1000); // This delay prevents a crash later in the code
                music.controls.pause();              // Audio plays by default, see issue 1
            }
        }
        
        static void Main(string[] args)
        {
            // Initialize controls with their default values
            int sampleRate = 20;
            int minWPM = 45;
            int ghostPeriod = 1000 / (minWPM / WPM_TO_CPM_FACTOR);

            // Remember if -g and/or -w flags are given
            bool ignoreWPM = false;
            bool wFlagPresent = false;

            // Apply any command line arguements
            for (int i = 1; i < args.Length; i+=2)
            {
                switch(args[i])
                {
                    // Sample Rate
                    case "-r":
                        if (!Int32.TryParse(args[i + 1], out sampleRate))
                            Console.WriteLine("Unparsable value given with flag -r, using default instead...");
                        break;

                    // Ghost Period
                    case "-g":
                        ignoreWPM = true;
                        if (!Int32.TryParse(args[i + 1], out sampleRate))
                            Console.WriteLine("Unparsable value given with flag -g, using default instead...");
                        break;

                    // Minimum WPM
                    case "-w":
                        wFlagPresent = true;
                        if (!Int32.TryParse(args[i + 1], out sampleRate))
                            Console.WriteLine("Unparsable value given with flag -w, using default instead...");
                        break;

                    // Continuous Loop
                    case "-c":
                        Trololololo.LoadMusic(true);
                        while(true) System.Threading.Thread.Sleep(10000);
                }
            }

            // Give warning if -w and -g flags were both given
            if (ignoreWPM && wFlagPresent)
                Console.WriteLine("Both -g and -w flags were given, dropping -w flag...");

            // Convert Hertz to sleep time
            sampleRate = 1000 / sampleRate;

            // Calculate Ghost Period
            if (!ignoreWPM)
                ghostPeriod = 1000 / (minWPM / WPM_TO_CPM_FACTOR);

            // Load the music, don't start it
            Trololololo.LoadMusic(false);

            while (true)
            {
                string keyBuffer = string.Empty;

                // Test if each key is presed
                foreach (System.Int32 i in Enum.GetValues(typeof(System.Windows.Forms.Keys)))
                {
                    int x = GetAsyncKeyState(i);
                    if ((x == 1) || (x == Int16.MinValue)) 
                    {
                        // Record the key that was pressed
                        keyBuffer += Enum.GetName(typeof(System.Windows.Forms.Keys), i) + " ";
                    }
                }

                // Key pressed
                if (keyBuffer != "none" && keyBuffer != "")
                {
                    music.controls.play();
                    System.Threading.Thread.Sleep(ghostPeriod);
                }
                // Key not pressed
                else
                {
                    music.controls.pause();
                    System.Threading.Thread.Sleep(sampleRate);
                }

            }
            
        }

        [DllImport("User32.dll")]
        private static extern short GetAsyncKeyState(System.Windows.Forms.Keys vKey); // Keys enumeration

        [DllImport("User32.dll")]
        private static extern short GetAsyncKeyState(System.Int32 vKey);
    }
}
