using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActivityMonitorApp
{
    internal class Settings
    {
        //die Minuten sollen nicht höher als standby time sein
        public static int pausePufferMinutes = 5;

        //standard sind 5 Minuten
        public static int autoSaveTime =52;

        public static TimeSpan pausePuffer = new TimeSpan(00, pausePufferMinutes, 00);
    }
}
