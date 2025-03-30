using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActivityMonitorApp
{
    internal class Settings
    {
        //Sollte am besten nicht höher als 5min sein. Bei 10 min gibt es einen bug mit standby-mode
        public static int pausePufferMinutes = 5;

        //standard sind 5 Minuten
        public static int autoSaveTime = 5;

        public static TimeSpan pausePuffer = new TimeSpan(00, pausePufferMinutes, 00);
    }
}
