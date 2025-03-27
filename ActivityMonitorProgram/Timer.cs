using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ActivityMonitorProgram
{
    internal class Timer
    {
        /// <summary>
        /// Timer für das automatische Speichern der Blöcke.
        /// </summary>
        public static void StartTimer()
        {
            System.Timers.Timer timer;
            timer = new System.Timers.Timer(1000 * 60 * Settings.autoSaveValue);

            // gibt dem Timer eine Anweisung
            timer.Elapsed += TimerElapsed;
            timer.Enabled = true;
            timer.AutoReset = true;
        }

        
        private static void TimerElapsed(object source, ElapsedEventArgs e)
        {
            Console.WriteLine("Start: " + Globals.startTime);
            Console.WriteLine("End: " + Globals.endTime);
            Console.WriteLine("BreakTime: " + Globals.pauseTime);
            Console.WriteLine("WorkTime: " + Globals.workTime);
            Console.WriteLine();
            Globals.save = true;
        }


        /// <summary>
        /// Timer zum Aktualisieren der Mausposition.
        /// </summary>
        public static void StartMousePosTimer()
        {
            System.Timers.Timer timer;
            timer = new System.Timers.Timer(100);

            // gibt dem Timer eine Anweisung
            timer.Elapsed += MousePosTimerElapsed;
            timer.Enabled = true;
            timer.AutoReset = true;

        }


        private static void MousePosTimerElapsed(object source, ElapsedEventArgs e)
        {
            Globals.previousMouse = CursorPosition.GetCursorPosition();
        }

    }
}
