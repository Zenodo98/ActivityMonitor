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
        //Zustände
        public enum State
        {
            Create,
            Active,
            Reset,
            Inactive,
        }

        //variablen
        public static bool firstLoop = true;
        public static bool firstStart = true;
        public static TimeSpan firstAfkTime;
        public static TimeSpan secondAfkTime;

        public static TimeSpan startTime;
        public static TimeSpan endTime;
        public static TimeSpan pauseTime;
        public static TimeSpan workTime;

        public static State currentState = State.Create;

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

            if (States.firstStart)
            {
                States.firstStart = false;
            }

            States.endTime = time;

            currentState = State.Reset;
        }


        /// <summary>
        /// Der Reset-Zustand zurücksetzen des Zeitzählers gedacht. Wechselt zurück zu Aktiv-Zustand
        /// </summary>
        /// <param name="time">Systemzeit</param>
        public static void Reset(TimeSpan time)
        {
            States.secondAfkTime = time;
            currentState = State.Active;
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
                currentState = State.Create;
            }

            States.startTime = time;
            States.workTime = Difference(States.startTime, States.endTime);


            if (ManageCSV.save)
            {
                ManageCSV.Save(date, path);
                ManageCSV.save = false;
            }

            States.firstAfkTime = time;

            TimeSpan afkTime = Difference(States.firstAfkTime, States.secondAfkTime);
            TimeSpan.Compare(afkTime, Settings.pausePuffer);
            CursorPosition.currentMouse = CursorPosition.GetCursorPosition();

            //Wenn Taste gedrückt wird, dann reset
            if (CursorPosition.DifferentMousePosition(CursorPosition.previousMouse, CursorPosition.currentMouse))
            {
                currentState = State.Reset;
            }

            if (TimeSpan.Compare(afkTime, Settings.pausePuffer) == 1)
            {
                Console.WriteLine("switch to inactive");
                States.workTime = Difference(States.startTime, States.endTime).Add(-Settings.pausePuffer);
                currentState = State.Inactive;
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
            CursorPosition.currentMouse = CursorPosition.GetCursorPosition();

            States.firstLoop = false;

            States.endTime = time;

            States.pauseTime = Difference(States.endTime, States.startTime).Add(Settings.pausePuffer);

            if (ManageCSV.save)
            {
                ManageCSV.Save(date, path);
                ManageCSV.save = false;
            }

            if (!File.Exists(@path))
            {
                currentState = State.Create;
            }

            if (CursorPosition.DifferentMousePosition(CursorPosition.previousMouse, CursorPosition.currentMouse))
            {
                Console.WriteLine("switch to active");
                ManageCSV.Save(date, path);
                ManageCSV.WriteCsvLine(path);
                ManageCSV.WriteCsvLine(path);
                States.pauseTime = TimeSpan.Zero;
                currentState = State.Create;
            }
        }
    }
}
