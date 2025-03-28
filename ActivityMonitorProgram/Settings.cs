using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActivityMonitorProgram
{
    internal class Settings
    {
        // wenn inaktiv, die Minuten die vergehen müssen, bevor der Zustand sich von aktiv zu inaktiv ändert
        // Standard 10min. Computer gehen in 15min zu standby. 
        public static TimeSpan pausePuffer = new TimeSpan(00, 00, 10);

        //automatisches Speichern, standard jede 5 minuten
        public static int autoSaveValue = 5;
    }
}
