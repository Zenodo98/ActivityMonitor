﻿
using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Timers;



namespace ActivityMonitorProgram
{
    public class ActivityMonitor
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("running");
            //Die Hauptschleife
            while (true)
            {
                await Task.Delay(300);

                TimeSpan time = DateTime.Now.TimeOfDay;
                time = TimeSpan.FromSeconds(Math.Round(time.TotalSeconds));

                DateTime date = DateTime.Now;

                string path = "Data" + "\\" + date.ToString("dd-MM-yyyy") + ".csv";

                //startet die Timer
                if (!Globals.timerRunning)
                {
                    Timer.StartTimer();
                    Timer.StartMousePosTimer();
                    Globals.timerRunning = true;
                }



                //Switch-Statement um zwischen den Zuständen zu wechseln
                switch (Globals.currentState)
                {
                    case Globals.State.Create:
                        States.Create(date, path, time);
                        break;
                    case Globals.State.Active:
                        States.Active(date, path, time);
                        break;
                    case Globals.State.Reset:
                        States.Reset(time);
                        break;
                    case Globals.State.Inactive:
                        States.Inactive(date, path, time);
                        break;
                }
            }
        }
    }
}