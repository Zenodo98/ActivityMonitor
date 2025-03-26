
using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Timers;
using static ActivityMonitorProgram.CursorPosition;







// Die Ausgabedateien befinden sich in \ActivityMonitorProgram\ActivityMonitorProgram\bin\Debug\net9.0\Data

namespace ActivityMonitorProgram
{
    public class ActivityMonitor
    {

        //Zustände
        enum State
        {
            Create,
            ActiveIdle,
            Active,
            Reset,
            InactiveIdle,
            Inactive

        }

        static State currentState = State.Create;

        // wenn inaktiv, die Minuten die vergehen müssen, bevor der Zustand sich von aktiv zu inaktiv ändert
        // Standard 10min. Computer gehen in 15min zu standby. 
        static TimeSpan pausePuffer = new TimeSpan(00, 10, 00);

        //automatisches Speichern, standard 5 minuten
        static int autoSaveValue = 5;

        //variablen
        static bool timerRunning = false;
        static bool firstLoop = true;
        static bool firstStart = true;
        static bool save = false;
        static Point currentMouse;
        static TimeSpan firstAfkTime;
        static TimeSpan secondAfkTime;

        //arrays
        static Point[] previousMouse = new Point[1];
        static TimeSpan[] arrayActivity = new TimeSpan[4];




        // gibt true zurück, wenn Mauspositionen ungleich sind
        static bool differentMousePosition(Point pos1, Point pos2)
        {
            if(pos1 == pos2)
            {
                return false;
            }
            return true;
        }




        // Gibt true zurück, wenn eine Taste gedrückt wird
        static bool DetectKeys()
        {
            [DllImport("user32.dll")]
            static extern short GetAsyncKeyState(int vKey);

            int[] keys = new int[31];

            keys[0] = 0x01;
            keys[1] = 0x10;
            keys[2] = 0x11;
            keys[3] = 0x20;
            keys[4] = 0x02;
            keys[5] = 0x41;
            keys[6] = 0x42;
            keys[7] = 0x43;
            keys[8] = 0x44;
            keys[9] = 0x45;
            keys[10] = 0x46;
            keys[11] = 0x47;
            keys[12] = 0x48;
            keys[13] = 0x49;
            keys[14] = 0x4A;
            keys[15] = 0x4B;
            keys[16] = 0x4C;
            keys[17] = 0x4D;
            keys[18] = 0x4E;
            keys[19] = 0x4F;
            keys[20] = 0x50;
            keys[21] = 0x51;
            keys[22] = 0x52;
            keys[23] = 0x53;
            keys[24] = 0x54;
            keys[25] = 0x55;
            keys[26] = 0x56;
            keys[27] = 0x57;
            keys[28] = 0x58;
            keys[29] = 0x59;
            keys[30] = 0x5A;

            foreach (int key in keys)
            {
                if (GetAsyncKeyState(key) != 0)
                {
                    return true;
                }
            }
            return false;
        }




        // die Timer
        //Timer zum speichern der Blöcke
        static void StartTimer()
        {
            System.Timers.Timer timer;
            timer = new System.Timers.Timer(1000 * 60 * autoSaveValue);

            // gibt dem Timer eine Anweisung
            timer.Elapsed += TimerElapsed;
            timer.Enabled = true;
            timer.AutoReset = true;
        }

        //Wenn Timer abläuft werden die Daten in einer CSV-Datei gespeichert
        private static void TimerElapsed(object source, ElapsedEventArgs e)
        {
            Console.WriteLine("Start: " + arrayActivity[0]);
            Console.WriteLine("End: " + arrayActivity[1]);
            Console.WriteLine("BreakTime: " + arrayActivity[2]);
            Console.WriteLine("WorkTime: " + arrayActivity[3]);
            Console.WriteLine();
            save = true;
        }


        //Jede Sekunde wird Mausposition gespeichert
        static void StartMousePosTimer()
        {
            System.Timers.Timer timer;
            timer = new System.Timers.Timer(1000);

            // gibt dem Timer eine Anweisung
            timer.Elapsed += MousePosTimerElapsed;
            timer.Enabled = true;
            timer.AutoReset = true;

        }


        //Wenn Timer abläuft wird diese Methode ausgeführt
        private static void MousePosTimerElapsed(object source, ElapsedEventArgs e)
        {
            previousMouse[0] = GetCursorPosition();
        }




        //Funktion zum speichern der Daten
        static void Save(DateTime date, string path)
        {
            removeCsvLine(path);
            WriteCsv(path, date.ToString("dd-MM-yyyy"));
            WriteCsv(path, arrayActivity[0].ToString());
            WriteCsv(path, arrayActivity[1].ToString());
            WriteCsv(path, arrayActivity[2].ToString());
            WriteCsv(path, arrayActivity[3].ToString());

        }




        //subtrahiert alte Zeit von neuer Zeit
        static TimeSpan difference(TimeSpan time1, TimeSpan time2)
        {
            return time1 - time2;
        }




        //Da der Dateiname das Datum benutzt, wird automatisch jeden Tag eine neue csv-Datei erstellt
        //csv-dateien können von jeder datenbank geöffnet werden
        static void createFile(string path)
        {
            //Wenn Dateiname nicht existiert, dann wird die Bedienung ausgeführt
            if (!File.Exists(@path))
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@path, true))
                {
                    file.Write("Datum,Anfang,Ende,Pause,Gesamtzeit");
                }
            }
            if (firstLoop)
            {
                firstLoop = true;
            }
        }




        //Funktion zum schreiben der CSV-Datei
        static void WriteCsv(string path, string content)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@path, true))
            {
                file.Write(content + ",");
            }
        }




        //Funktion zum erstellen einer leeren Zeile
        static void WriteCsvLine(string path)
        {

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@path, true))
            {
                file.WriteLine("");
            }
        }




        //funktion zum entfernen einer Zeile
        static void removeCsvLine(string path)
        {
            var lines = System.IO.File.ReadAllLines(path);
            System.IO.File.WriteAllLines(path, lines.Take(lines.Length - 1).ToArray());
        }




        //create -> Active -> reset -> activeidle -> inactiv -> inactividle ->create

        //erstellt neue Datei
        static void Create(DateTime date, string path, TimeSpan time)
        {
            createFile(path);
            WriteCsvLine(path);

            if (firstStart)
            {
                firstStart = false;
                WriteCsvLine(path);
            }


            arrayActivity[1] = time;

            currentState = State.Active;

        }



        //timer reset
        static void Reset(TimeSpan time)
        {
            secondAfkTime = time;
            currentState = State.ActiveIdle;
        }




        static void Active(string path, TimeSpan time)
        {
            arrayActivity[0] = time;
            currentState = State.Reset;
        }




        static void Inactive(TimeSpan time)
        {
            arrayActivity[1] = time;
            currentState = State.InactiveIdle;
        }




        
        static void ActiveIdle(DateTime date, string path, TimeSpan time)
        {
            
            if (!File.Exists(@path))
            {
                currentState = State.Create;
            }

            arrayActivity[0] = time;
            arrayActivity[3] = difference(arrayActivity[0], arrayActivity[1]);


            if (save)
            {
                Save(date, path);
            }

            firstAfkTime = time;

            TimeSpan afkTime = difference(firstAfkTime, secondAfkTime);
            TimeSpan.Compare(afkTime, pausePuffer);
            currentMouse = GetCursorPosition();

            //Wenn Taste gedrückt wird, dann reset
            if (DetectKeys() || differentMousePosition(previousMouse[0], currentMouse))
            {
                currentState = State.Reset;
            }

            if (TimeSpan.Compare(afkTime, pausePuffer) == 1)
            {
                Console.WriteLine("switch to inactive");
                arrayActivity[3] = difference(arrayActivity[0], arrayActivity[1]).Add(-pausePuffer);
                currentState = State.Inactive;   
            }  
        }




        static void InactiveIdle(DateTime date, string path, TimeSpan time)
        {
            currentMouse = GetCursorPosition();
            firstLoop = false;

            arrayActivity[1] = time;
            arrayActivity[2] = difference(arrayActivity[1], arrayActivity[0]).Add(pausePuffer);

            if (save)
            {
                Save(date, path);
            }

            if (!File.Exists(@path))
            {
                currentState = State.Create;
            }

            if (DetectKeys() || differentMousePosition(previousMouse[0], currentMouse))
            {
                Console.WriteLine("switch to active");
                Save(date, path);
                WriteCsvLine(path);
                arrayActivity[2] = TimeSpan.Zero;
                currentState = State.Create;
            }
        }




        static async Task Main(string[] args)
        {
            Console.WriteLine("running");
            //Die Hauptschleife
            while (true)
            {
                await Task.Delay(300);

                TimeSpan time = DateTime.Now.TimeOfDay;
                time = TimeSpan.FromSeconds(Math.Round(time.TotalSeconds));

                DateTime date = DateTime.Now;

                string path = "Data" + "\\" + date.ToString("dd-MM-yyyy") + ".csv";


                //startet die Timer

                if (!timerRunning)
                {
                    StartTimer();
                    StartMousePosTimer();
                    timerRunning = true;
                }



                //Switch-Statement um zwischen den Zuständen zu wechseln
                switch (currentState)
                {
                    case State.Create:
                        Create(date, path, time);
                        break;
                    case State.ActiveIdle:
                        ActiveIdle(date, path, time);
                        break;
                    case State.Reset:
                        Reset(time);
                        break;
                    case State.InactiveIdle:
                        InactiveIdle(date, path, time);
                        break;
                    case State.Active:
                        Active(path, time);
                        break;
                    case State.Inactive:
                        Inactive(time);
                        break;

                }

            }
        }
    }

   
    internal static class CursorPosition
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct PointInter
        {
            public int X;
            public int Y;
            public static explicit operator Point(PointInter point) => new Point(point.X, point.Y);
        }

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out PointInter lpPoint);

        // die Methode gibt die Maus Position zurück
        public static Point GetCursorPosition()
        {
            PointInter lpPoint;
            GetCursorPos(out lpPoint);
            return (Point)lpPoint;
        }
    }
}