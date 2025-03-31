using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActivityMonitorApp
{
    internal class Save
    {
        public string Date { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string BreakTime { get; set; }
        public string WorkTime { get; set; }

        private static string path = "Data\\";
        private static string fileName = DateTime.Now.ToString("dd-MM-yyyy") + ".csv";
        FileManager CsvActivityFile = new FileManager(@path, fileName);

        /// <summary>
        /// Speichern von Aktivitätszeiten
        /// </summary>
        public Save() 
        {
            Date = DateTime.Now.ToString("dd-MM-yyyy");
            StartTime = TimeSpan.Zero.ToString();
            EndTime = TimeSpan.Zero.ToString();
            BreakTime = TimeSpan.Zero.ToString();
            WorkTime = TimeSpan.Zero.ToString();
        }

        /// <summary>
        /// automatisches speichern
        /// </summary>
        /// <param name="startTime">Startzeit</param>
        /// <param name="endTime">Endzeit</param>
        /// <param name="breakTime">Pausenzeit</param>
        /// <param name="workTime">Arbeitszeit</param>
        /// <param name="save">boolean, bei true wird gespeichert</param>
        public void AutoSave(string startTime, string endTime, string breakTime, string workTime, bool save) 
        {
            if (save)
            {
                Date = DateTime.Now.ToString("dd-MM-yyyy");
                StartTime = startTime;
                EndTime = endTime;
                BreakTime = breakTime;
                WorkTime = workTime;

                CsvActivityFile.RemoveLastLine();
                CsvActivityFile.Write(Date + ",");
                CsvActivityFile.Write(StartTime + ",");
                CsvActivityFile.Write(EndTime + ",");
                CsvActivityFile.Write(BreakTime + ",");
                CsvActivityFile.Write(WorkTime + ",");
                CsvActivityFile.WriteLine();

                Console.WriteLine("-------------------------");
                Console.WriteLine("automatisch gespeichert");
                Console.WriteLine("-------------------------");
            }
        }

        /// <summary>
        /// vollständiges speichern
        /// </summary>
        /// <param name="startTime">Startzeit</param>
        /// <param name="endTime">Endzeit</param>
        /// <param name="breakTime">Pausenzeit</param>
        /// <param name="workTime">Arbeitszeit</param>
        public void FullSave(string startTime, string endTime, string breakTime, string workTime)
        {
            Date = DateTime.Now.ToString("dd-MM-yyyy");
            StartTime = startTime;
            EndTime = endTime;
            BreakTime = breakTime;
            WorkTime = workTime;

            CsvActivityFile.RemoveLastLine();
            CsvActivityFile.Write(Date + ",");
            CsvActivityFile.Write(StartTime + ",");
            CsvActivityFile.Write(EndTime + ",");
            CsvActivityFile.Write(BreakTime + ",");
            CsvActivityFile.Write(WorkTime + ",");
            CsvActivityFile.WriteLine();
            CsvActivityFile.WriteLine();
        }
    }
}
