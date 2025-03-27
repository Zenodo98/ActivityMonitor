using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ActivityMonitorProgram
{
    internal class States
    {

        /// <summary>
        /// Die Methode subtrahiert alte Zeit von neuer Zeit
        /// </summary>
        /// <param name="time1">erste Zeit</param>
        /// <param name="time2">zweite Zeit</param>
        /// <returns>Differenz der Zeiten</returns>
        static TimeSpan Difference(TimeSpan time1, TimeSpan time2)
        {
            return time1 - time2;
        }

        //create -> Active -> reset -> activeidle -> inactiv -> inactividle ->create


        /// <summary>
        /// In dem Zustand wird die Datei Erstellt, wenn die nicht vorhanden ist.
        /// Danach wird zum Aktiv-Zustand gewechselt
        /// </summary>
        /// <param name="date">Systemdatum</param>
        /// <param name="path">Pfadverzeichnis</param>
        /// <param name="time">Systemzeit</param>
        public static void Create(DateTime date, string path, TimeSpan time)
        {
            ManageCSV.CreateFile(path);

            if (Globals.firstStart)
            {
                Globals.firstStart = false;
            }

            Globals.endTime = time;

            Globals.currentState = Globals.State.Reset;
        }


        /// <summary>
        /// Der Reset-Zustand zurücksetzen des Zeitzählers gedacht. Wechselt zurück zu Aktiv-Zustand
        /// </summary>
        /// <param name="time">Systemzeit</param>
        public static void Reset(TimeSpan time)
        {
            Globals.secondAfkTime = time;
            Globals.currentState = Globals.State.Active;
        }

        /// <summary>
        /// Schaut ob der Nutzer aktiv ist, wenn nicht wechselt zu Inaktiv-Zustand.
        /// </summary>
        /// <param name="date">Systemdatum</param>
        /// <param name="path">Pfadverzeichnis</param>
        /// <param name="time">Systemzeit</param>
        public static void Active(DateTime date, string path, TimeSpan time)
        {

            if (!File.Exists(@path))
            {
                Globals.currentState = Globals.State.Create;
            }

            Globals.startTime = time;
            Globals.workTime = Difference(Globals.startTime, Globals.endTime);


            if (Globals.save)
            {
                ManageCSV.Save(date, path);
                Globals.save = false;
            }

            Globals.firstAfkTime = time;

            TimeSpan afkTime = Difference(Globals.firstAfkTime, Globals.secondAfkTime);
            TimeSpan.Compare(afkTime, Settings.pausePuffer);
            Globals.currentMouse = CursorPosition.GetCursorPosition();

            //Wenn Taste gedrückt wird, dann reset
            if (CursorPosition.DifferentMousePosition(Globals.previousMouse, Globals.currentMouse))
            {
                Globals.currentState = Globals.State.Reset;
            }

            if (TimeSpan.Compare(afkTime, Settings.pausePuffer) == 1)
            {
                Console.WriteLine("switch to inactive");
                Globals.workTime = Difference(Globals.startTime, Globals.endTime).Add(-Settings.pausePuffer);
                Globals.currentState = Globals.State.Inactive;
            }
        }

        /// <summary>
        /// Schaut ob der Nutzer aktiv ist, wenn nicht wechselt zu Inaktiv-Zustand.
        /// </summary>
        /// <param name="date">Systemdatum</param>
        /// <param name="path">Pfadverzeichnis</param>
        /// <param name="time">Systemzeit</param>
        public static void Inactive(DateTime date, string path, TimeSpan time)
        {
            Globals.currentMouse = CursorPosition.GetCursorPosition();

            Globals.firstLoop = false;

            Globals.endTime = time;

            Globals.pauseTime = Difference(Globals.endTime, Globals.startTime).Add(Settings.pausePuffer);

            if (Globals.save)
            {
                ManageCSV.Save(date, path);
                Globals.save = false;
            }

            if (!File.Exists(@path))
            {
                Globals.currentState = Globals.State.Create;
            }

            if (CursorPosition.DifferentMousePosition(Globals.previousMouse, Globals.currentMouse))
            {
                Console.WriteLine("switch to active");
                ManageCSV.Save(date, path);
                ManageCSV.WriteCsvLine(path);
                ManageCSV.WriteCsvLine(path);
                Globals.pauseTime = TimeSpan.Zero;
                Globals.currentState = Globals.State.Create;
            }
        }
    }
}
