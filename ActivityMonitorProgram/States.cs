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
        
        public static TimeSpan firstAfkTime = TimeSpan.Zero;
        public static TimeSpan secondAfkTime = TimeSpan.Zero;
        public static TimeSpan firstStartTime = TimeSpan.Zero;
        public static TimeSpan firstEndTime = TimeSpan.Zero;
        public static TimeSpan lastStartTime = TimeSpan.Zero;
        public static TimeSpan lastEndTime = TimeSpan.Zero;
        public static TimeSpan pauseTime = TimeSpan.Zero;
        public static TimeSpan workTime = TimeSpan.Zero;

        public static State currentState = State.Create;


        //Zustände
        public enum State
        {
            Create,
            SetStartTime,
            Active,
            Reset,
            SetEndTime,
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
        /// Danach wird zu SetStartTime gewechselt
        /// </summary>
        /// <param name="date">Systemdatum</param>
        /// <param name="path">Pfadverzeichnis</param>
        /// <param name="time">Systemzeit</param>
        public static void Create(string path, TimeSpan time)
        {
            ManageCSV.CreateFile(path);
            currentState = State.SetStartTime;

            secondAfkTime = time;

            firstStartTime = TimeSpan.Zero ;
            firstEndTime = TimeSpan.Zero ;
            lastStartTime = TimeSpan.Zero ;
            lastEndTime = TimeSpan.Zero ;
            pauseTime = TimeSpan.Zero ;
            workTime = TimeSpan.Zero ;
        }


        /// <summary>
        /// Der Reset-Zustand zurücksetzen des Zeitzählers der afk-Zeit gedacht. Wechselt zurück zu Aktiv-Zustand
        /// </summary>
        /// <param name="time">Systemzeit</param>
        public static void Reset(TimeSpan time)
        {

            //wird benötigt zum berechnen der afk Zeit
            secondAfkTime = time;

            currentState = State.Active;
        }


        /// <summary>
        /// Zum speichern der Startzeit in eine Variable
        /// </summary>
        /// <param name="time">SystemZeit</param>
        public static void SetStartTime(TimeSpan time) 
        {
            //wird gebraucht um die Arbeitszeit zu berechnen
            lastStartTime = time;

            //wird gebraucht um später die Arbeitszeit zu berechnen
            firstStartTime = time;

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



            //wird benötigt zum berechnen der afk Zeit
            firstAfkTime = time;

            //wird gebraucht um die Arbeitszeit zu berechnen
            lastStartTime = time;

            //Berechnet die Arbeitszeit
            workTime = Difference(lastStartTime, firstStartTime);

            //die afk zeit, wenn die eine bestimmte Zeit erreicht dann wird zu Inaktiv gewechselt
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
                ManageCSV.RemoveCsvLine(path);
                ManageCSV.Save(date, path, firstStartTime, lastStartTime, pauseTime, workTime);
                currentState = State.Create;
            }
            else if (ManageCSV.save)
            {
                Console.WriteLine();
                Console.WriteLine("FirstStartTime: " + firstStartTime);
                Console.WriteLine("LastStartTime: " + lastStartTime);
                Console.WriteLine("WorkTime: " + workTime);
                Console.WriteLine("auto saved while activ");

                ManageCSV.RemoveCsvLine(path);
                ManageCSV.Save(date, path, firstStartTime, lastStartTime, pauseTime, workTime);
                ManageCSV.save = false;
            }
            else if (CursorPosition.currentMouseX != CursorPosition.previousMouseX & CursorPosition.currentMouseY != CursorPosition.previousMouseY)
            {
                currentState = State.Reset;
            }
            else if (TimeSpan.Compare(afkTime, Settings.pausePuffer) == 1)
            {
                Console.WriteLine();
                Console.WriteLine("FirstStartTime: " + firstStartTime);
                Console.WriteLine("LastStartTime: " + lastStartTime);
                Console.WriteLine("WorkTime: " + workTime);
                Console.WriteLine("switch to inactive");

                workTime = Difference(lastStartTime, firstStartTime).Add(-Settings.pausePuffer);
                CursorPosition.previousMouseX = CursorPosition.GetCursorPosition().X;
                CursorPosition.previousMouseY = CursorPosition.GetCursorPosition().Y;
                currentState = State.SetEndTime;
            }
        }

        /// <summary>
        /// Speichert die Endzeit in eine Variable
        /// </summary>
        /// <param name="time"></param>
        public static void SetEndTime(TimeSpan time)
        {
            //wird fürs ausrechnen der Pausenzeit gebraucht
            firstEndTime = time;

            currentState = State.Inactive;
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

            //wird fürs ausrechnen der Pausenzeit gebraucht
            lastEndTime = time;

            pauseTime = Difference(lastEndTime, firstEndTime).Add(Settings.pausePuffer);


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
                ManageCSV.RemoveCsvLine(path);
                ManageCSV.Save(date, path, firstStartTime, lastEndTime, pauseTime, workTime);
                currentState = State.Create;
            }
            else if (CursorPosition.currentMouseX <= CursorPosition.previousMouseX - 50 || CursorPosition.currentMouseX >= CursorPosition.previousMouseX + 50 &
                CursorPosition.currentMouseY <= CursorPosition.previousMouseY - 50 || CursorPosition.currentMouseY >= CursorPosition.previousMouseY + 50)
            {
                Console.WriteLine();
                Console.WriteLine("firstEndTime: " + firstEndTime);
                Console.WriteLine("lastEndTime: " + lastEndTime);
                Console.WriteLine("BreakTime: " + pauseTime);
                Console.WriteLine("switch to active");

                ManageCSV.RemoveCsvLine(path);
                ManageCSV.Save(date, path, firstStartTime, lastEndTime, pauseTime, workTime);
                ManageCSV.WriteCsvLine(path);
                ManageCSV.WriteCsvLine(path);
                pauseTime = TimeSpan.Zero;
                currentState = State.Create;
            }
            else if (ManageCSV.save)
            {
                Console.WriteLine();
                Console.WriteLine("firstEndTime: " + firstEndTime);
                Console.WriteLine("lastEndTime: " + lastEndTime);
                Console.WriteLine("BreakTime: " + pauseTime);
                Console.WriteLine("auto saved while inactiv");

                ManageCSV.RemoveCsvLine(path);
                ManageCSV.Save(date, path, firstStartTime, lastEndTime, pauseTime, workTime);
                ManageCSV.save = false;
            }
        }
    }
}
