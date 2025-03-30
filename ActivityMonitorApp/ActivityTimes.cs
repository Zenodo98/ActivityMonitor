using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActivityMonitorApp
{
    internal class ActivityTimes
    {
        private TimeSpan Time {  get; set; }

        public TimeSpan StartTime {  get; set; }
        public TimeSpan AktivEndTime { get; set; }
        public TimeSpan InaktivEndTime {  get; set; }

        public TimeSpan BreakTime { get; set; }
        public TimeSpan WorkTime { get; set; }

        public bool ResetStartTime = true;

        public ActivityTimes()
        {
            Time = DateTime.Now.TimeOfDay;
        }

        public void ClearDateTimes() 
        { 
            StartTime = TimeSpan.Zero;
            AktivEndTime = TimeSpan.Zero;
            InaktivEndTime = TimeSpan.Zero;
            BreakTime = TimeSpan.Zero;
            WorkTime = TimeSpan.Zero;
        }

        public void SetStartTime()
        {
            if (ResetStartTime)
            {
                Time = DateTime.Now.TimeOfDay;
                Time = TimeSpan.FromSeconds(Math.Round(Time.TotalSeconds));
                StartTime = Time;
                ResetStartTime = false;
            }
        }

        public void SetAktivEndTime() 
        {
            Time = DateTime.Now.TimeOfDay;
            Time = TimeSpan.FromSeconds(Math.Round(Time.TotalSeconds));
            AktivEndTime = Time;
        }

        public void SetInaktivEndTime()
        {
            Time = DateTime.Now.TimeOfDay;
            Time = TimeSpan.FromSeconds(Math.Round(Time.TotalSeconds));
            InaktivEndTime = Time;
        }

        public void CalculateWorkTime()
        {
            Time = DateTime.Now.TimeOfDay;
            Time = TimeSpan.FromSeconds(Math.Round(Time.TotalSeconds));
            WorkTime = AktivEndTime - StartTime;
        }

        public void CalculateBreakTime() 
        {
            Time = DateTime.Now.TimeOfDay;
            Time = TimeSpan.FromSeconds(Math.Round(Time.TotalSeconds));
            BreakTime = InaktivEndTime - AktivEndTime;
        }
    }

}
