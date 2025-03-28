
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
                if (!Timer.timerRunning)
                {
                    Timer.StartTimer();
                    Timer.StartMousePosTimer();
                    Timer.timerRunning = true;
                }


                //Switch-Statement um zwischen den Zuständen zu wechseln
                switch (States.currentState)
                {
                    case States.State.Create:
                        States.Create(path, time);
                        break;
                    case States.State.SetStartTime:
                        States.SetStartTime(time);
                        break;
                    case States.State.Active:
                        States.Active(date, path, time);
                        break;
                    case States.State.Reset:
                        States.Reset(time);
                        break;
                    case States.State.SetEndTime:
                        States.SetEndTime(time);
                        break;
                    case States.State.Inactive:
                        States.Inactive(date, path, time);
                        break;
                }
            }
        }
    }
}