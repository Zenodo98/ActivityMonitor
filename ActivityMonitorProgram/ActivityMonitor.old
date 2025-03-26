
using System;
using System.Runtime.InteropServices;
using System.Timers;
using Microsoft.VisualBasic;



// Die Ausgabedateien befinden sich in \ActivityMonitorProgram\ActivityMonitorProgram\bin\Debug\net9.0\Data

namespace ActivityMonitorProgram
{
    public class ActivityMonitor
    {

        //Zustände
        enum State
        {
            Active,
            Inactive,
            Save
        }

        static State currentState = State.Save;

        // wenn inaktiv, die Minuten die vergehen müssen, bevor der Zustand sich von aktiv zu inaktiv ändert
        // Standart 10min. Computer gehen in 15min zu standby. 
        static int pausePuffer = 10;


        static int counter = 0;
        static bool timerRunning = false;
        static bool aktiv = true;
        static bool firstStart = true;
        static TimeSpan firstTime;
        static TimeSpan secondTime;
        static TimeSpan thirdTime;

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

        // der Timer
        static void StartTimer()
        {

            
            System.Timers.Timer timer;
            timer = new System.Timers.Timer(1000);
           

            // gibt dem Timer eine Anweisung
            timer.Elapsed += TimerElapsed;
            timer.Enabled = true;
            timer.AutoReset = true;
          
            
        }


        //Wenn Timer abläuft wird diese Methode ausgeführt
        private static void TimerElapsed(object source, ElapsedEventArgs e)
        {
            counter++;
        }

        //Da der Dateiname das Datum benutzt, wird automatisch jeden Tag eine neue csv-Datei erstellt
        //csv-dateien können von jeder datenbank geöffnet werden
        public static void WriteCsv(DateTime date, string path) {

            

            

            string strDate = date.ToString("dd-MM-yyyy");
            string strTime = date.ToString("HH:mm:ss");

            

            //Wenn Dateiname nicht existiert, dann wird die Bedienung ausgeführt
            if (!File.Exists(@path))
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@path, true))
                {
                    file.Write("Datum,Anfang,Ende,Pause,Gesamtzeit");
                    file.WriteLine("");


                }

            }

            if (firstStart) 
             {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@path, true))
                {
                   
                    file.WriteLine("");
                    firstStart = false;
                }
            }


       

            if (aktiv)
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@path, true))
                {

                    firstTime = DateTime.Now.TimeOfDay;

                  



                    file.Write(strDate + ",");
                    file.Write(strTime + ",");

                    


                }

                
                
                thirdTime = DateTime.Now.TimeOfDay;
                TimeSpan pauseTime = thirdTime - secondTime;
                pauseTime = pauseTime.Add(TimeSpan.FromMinutes(pausePuffer));
                pauseTime = TimeSpan.FromSeconds(Math.Round(pauseTime.TotalSeconds));
                File.WriteAllText(@path, File.ReadAllText(@path).Replace("EMPTY,", pauseTime + ","));

            }
            else

            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@path, true))
                {

                    secondTime = DateTime.Now.TimeOfDay;

                    TimeSpan workTime = secondTime - firstTime;
                    workTime = workTime.Add(TimeSpan.FromMinutes(-pausePuffer));
                    workTime = TimeSpan.FromSeconds(Math.Round(workTime.TotalSeconds));


                    file.Write(strTime + ",");
                    file.Write("EMPTY" + ",");
                    file.Write(workTime + ",");

                    file.WriteLine();
                }
            }


        }


        static void Active() 
        {
            if (!timerRunning)
            {
                StartTimer();
                timerRunning = true;
            }

            if (DetectKeys())
            {
                counter = 0;
            }

            if (!DetectKeys() & counter >= pausePuffer*60)
            {   
                aktiv = false;
                Console.WriteLine("saved to file");
                Console.WriteLine(" ");
                currentState = State.Save;
            }
        }

        static void Inactive()
        {
            counter = 0;
            if (DetectKeys())
            {
                aktiv = true;
                Console.WriteLine("saved to file");
                Console.WriteLine(" ");
                currentState = State.Save;
            }
        }

        static void Save()
        {

            DateTime date = DateTime.Now;

            string path = "Data" + "\\" + date.ToString("dd-MM-yyyy") + ".csv";


            

            WriteCsv(date, path);
            if (aktiv)
            {
                Console.WriteLine("Active");
                Console.WriteLine(" ");
                currentState = State.Active;
            }
            else {
                Console.WriteLine("Inactive");
                Console.WriteLine(" ");
                currentState = State.Inactive;
            }
        }



        static async Task Main(string[] args)
        {

            //Die Hauptschleife
            while (true)
            {
                await Task.Delay(300);

                //Switch-Statement um zwischen den Zuständen zu wechseln
                switch (currentState)
                {
                    case State.Active:
                        Active();
                        break;
                    case State.Inactive:
                        Inactive();
                        break;
                    case State.Save:
                        Save();
                        break;
                }

            }
        }
    }    
}
