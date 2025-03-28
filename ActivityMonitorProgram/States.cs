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
        
        public static TimeSpan firstAfkTime;
        public static TimeSpan secondAfkTime;
        public static TimeSpan startTime;
        public static TimeSpan endTime;
        public static TimeSpan pauseTime;
        public static TimeSpan workTime;

        public static State currentState = State.Create;


        //Zustände
        public enum State
        {
            Create,
            Active,
            Reset,
            Inactive,
        }


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
        public static void Create(string path, TimeSpan time)
        {
            ManageCSV.CreateFile(path);

            States.endTime = time;

            currentState = State.Reset;
        }


        /// <summary>
        /// Der Reset-Zustand zurücksetzen des Zeitzählers der afk-Zeit gedacht. Wechselt zurück zu Aktiv-Zustand
        /// </summary>
        /// <param name="time">Systemzeit</param>
        public static void Reset(TimeSpan time)
        {

            States.startTime = time;
            //zweite Zeit für die Difference Methode
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
            CursorPosition.currentMouseX = CursorPosition.GetCursorPosition().X;
            CursorPosition.currentMouseY = CursorPosition.GetCursorPosition().Y;

            States.startTime = time;

            //zweite Zeit für die Difference Methode
            States.firstAfkTime = time;

            //Berechnet die Arbeitszeit
            States.workTime = Difference(States.startTime, States.endTime);

            //Zeitzähler
            TimeSpan afkTime = Difference(States.firstAfkTime, States.secondAfkTime);

            //vergleicht die afk-Zeit mit der Pufferzeit
            TimeSpan.Compare(afkTime, Settings.pausePuffer);


            //wenn datei nicht vorhanden wechselt zu create
            //speichert, wechselt zu create, wenn 23:59
            //speichert automatisch, wenn Zeit
            //wenn Mausbewegung, wechsel zu reset
            //wenn afk zeit höher als Puffer, wechsel zu Inaktiv

            if (!File.Exists(@path))
            {
                currentState = State.Create;
            }
            else if (time >= new TimeSpan(23,59,00) & time <= new TimeSpan(23,59,59))
            {
                ManageCSV.Save(date, path);
                currentState = State.Create;
            }
            else if (ManageCSV.save)
            {
                Console.WriteLine();
                Console.WriteLine("BreakTime: " + States.pauseTime);
                Console.WriteLine("WorkTime: " + States.workTime);
                Console.WriteLine("auto saved while activ");

                ManageCSV.Save(date, path);
                ManageCSV.save = false;
            }
            else if (CursorPosition.currentMouseX != CursorPosition.previousMouseX & CursorPosition.currentMouseY != CursorPosition.previousMouseY)
            {
                currentState = State.Reset;
            }
            else if (TimeSpan.Compare(afkTime, Settings.pausePuffer) == 1)
            {
                Console.WriteLine();
                Console.WriteLine("BreakTime: " + States.pauseTime);
                Console.WriteLine("WorkTime: " + States.workTime);
                Console.WriteLine("switch to inactive");

                States.workTime = Difference(States.startTime, States.endTime).Add(-Settings.pausePuffer);
                CursorPosition.previousMouseX = CursorPosition.GetCursorPosition().X;
                CursorPosition.previousMouseY = CursorPosition.GetCursorPosition().Y;
                currentState = State.Inactive;
            }
        }

        /// <summary>
        /// Schaut ob der Nutzer inaktiv ist, wenn nicht wechselt zu aktiv-Zustand.
        /// </summary>
        /// <param name="date">Systemdatum</param>
        /// <param name="path">Pfadverzeichnis</param>
        /// <param name="time">Systemzeit</param>
        public static void Inactive(DateTime date, string path, TimeSpan time)
        {
            CursorPosition.currentMouseX = CursorPosition.GetCursorPosition().X;
            CursorPosition.currentMouseY = CursorPosition.GetCursorPosition().Y;

            States.endTime = time;

            States.pauseTime = Difference(States.endTime, States.startTime).Add(Settings.pausePuffer);


            //wenn datei nicht vorhanden wechselt zu create
            //speichert, wechselt zu create, wenn 23:59
            //wenn Mausbewegung, dann wechsel zu create
            //speichert automatisch, wenn Zeit
            if (!File.Exists(@path))
            {
                currentState = State.Create;
            }
            else if (time >= new TimeSpan(23, 59, 00) & time <= new TimeSpan(23, 59, 59))
            {
                ManageCSV.Save(date, path);
                currentState = State.Create;
            }
            else if (CursorPosition.currentMouseX <= CursorPosition.previousMouseX - 50 || CursorPosition.currentMouseX >= CursorPosition.previousMouseX + 50 &
                CursorPosition.currentMouseY <= CursorPosition.previousMouseY - 50 || CursorPosition.currentMouseY >= CursorPosition.previousMouseY + 50)
            {
                Console.WriteLine();
                Console.WriteLine("BreakTime: " + States.pauseTime);
                Console.WriteLine("WorkTime: " + States.workTime);
                Console.WriteLine("switch to active");

                ManageCSV.Save(date, path);
                ManageCSV.WriteCsvLine(path);
                ManageCSV.WriteCsvLine(path);
                States.pauseTime = TimeSpan.Zero;
                currentState = State.Create;
            }
            else if (ManageCSV.save)
            {
                Console.WriteLine();
                Console.WriteLine("BreakTime: " + States.pauseTime);
                Console.WriteLine("WorkTime: " + States.workTime);
                Console.WriteLine("auto saved while inactiv");

                ManageCSV.Save(date, path);
                ManageCSV.save = false;
            }
        }
    }
}
