using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActivityMonitorProgram
{
    internal class Globals
    {
        //variablen
        public static bool timerRunning = false;
        public static bool firstLoop = true;
        public static bool firstStart = true;
        public static bool save = false;
        
        public static Point previousMouse;
        public static Point currentMouse;
        public static TimeSpan firstAfkTime;
        public static TimeSpan secondAfkTime;

        public static TimeSpan startTime;
        public static TimeSpan endTime;
        public static TimeSpan pauseTime;
        public static TimeSpan workTime;
    }
}
