

using ActivityMonitorProgram;
using System.Runtime.InteropServices;
using System.Threading;
using System.Timers;

namespace ActivityMonitorApp
{
    internal class TimerHandler
    {
        private readonly System.Timers.Timer? Timer;
        private bool Started = false;
        public int Time {  get; set; }
        public bool AwayFromKeyboard = false;
        public bool AutoSave = false;
        public int previousMouseX;
        public int previousMouseY;

        public TimerHandler(int time) 
        {

            Time = time;

            Timer = new System.Timers.Timer(time);
            
        }

        /// <summary>
        /// startet den Timer, welcher vorherige Mausposition speichert
        /// </summary>
        public void StartPreviousMousePositionTimer() 
        {
            if (Timer != null)
            {
                if (!Started)
                {
                    Timer.Elapsed += PreviousMousePositionTimerElapsed;
                    Timer.Enabled = true;
                    Timer.AutoReset = true;

                    Started = true;
                }
            }
        }

        /// <summary>
        /// Die Methode wechselt beim Timeout zu Inaktiv
        /// </summary>
        public void OnTimeoutSwitchToInaktiv()
        {
            if (Timer != null)
            {
                if (!Started)
                {
                    Timer.Elapsed += AFKTimerElapsed;
                    Timer.Enabled = true;
                    Timer.AutoReset = true;

                    Started = true;
                }
            }
        }
        
        /// <summary>
        /// Die Methode started automatisches Speichern
        /// </summary>
        public void StartAutoSave()
        {
            if (Timer != null)
            {
                if (!Started)
                {
                    Timer.Elapsed += AutoSaveTimerElapsed;
                    Timer.Enabled = true;
                    Timer.AutoReset = true;

                    Started = true;
                }
            }
        }

        /// <summary>
        /// Die Methode beendet den Timer
        /// </summary>
        public void StopTimer()
        {
            if (Timer != null)
            {
                Timer.Enabled = false;
                Timer.AutoReset = false;

                Started = false;
            }
        }


        private void AutoSaveTimerElapsed(object source, ElapsedEventArgs e)
        {
            AutoSave = true;
        }
        private void AFKTimerElapsed(object source, ElapsedEventArgs e)
        {
            AwayFromKeyboard = true;
        }
        private void PreviousMousePositionTimerElapsed(object source, ElapsedEventArgs e)
        {
            previousMouseX = CursorPosition.GetCursorPosition().X;
            previousMouseY = CursorPosition.GetCursorPosition().Y;
        }
    }
}
