using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActivityMonitorProgram
{
    internal class ManageCSV
    {
        public static bool save = false;
        /// <summary>
        /// Die Methode erstellt eine CSV-Datei und schreibt "Datum,Anfang,Ende,Pause,Gesamtzeit" darein.
        /// </summary>
        /// <param name="path">Das Pfadverzeichnis, wo die Datei erstellt werden soll. </param>
        public static void CreateFile(string path)
        {
            //Wenn Dateiname nicht existiert, dann wird die Bedienung ausgeführt
            if (!File.Exists(@path))
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@path, true))
                {
                    file.Write("Datum,Anfang,Ende,Pause,Gesamtzeit");
                    file.WriteLine("");
                    file.WriteLine("");
                }
            }
        }


        /// <summary>
        /// Die Methode schreibt in die Datei.
        /// </summary>
        /// <param name="path">Das Pfadverzeichnis der Datei.</param>
        /// <param name="content">Der Inhalt der in die Datei geschrieben werden soll.</param>
        public static void WriteCsv(string path, string content)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@path, true))
            {
                file.Write(content + ",");
            }
        }


        /// <summary>
        /// Die Methode schreibt eine Leere Zeile.
        /// </summary>
        /// <param name="path">Das Pfadverzeichnis der Datei.</param>
        public static void WriteCsvLine(string path)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@path, true))
            {
                file.WriteLine("");
            }
        }


        /// <summary>
        /// Die Methode entfernt die letzte Zeile.
        /// </summary>
        /// <param name="path">Das Pfadverzeichnis der Datei.</param>
        public static void RemoveCsvLine(string path)
        {
            var lines = System.IO.File.ReadAllLines(path);
            System.IO.File.WriteAllLines(path, lines.Take(lines.Length - 1).ToArray());
        }


        /// <summary>
        /// Die Methode schreibt die Daten in die Datei.
        /// </summary>
        /// <param name="date">heutige Datum</param>
        /// <param name="path">Das Pfadverzeichnis der Datei</param>
        public static void Save(DateTime date, string path)
        {
            ManageCSV.RemoveCsvLine(path);
            ManageCSV.WriteCsv(path, date.ToString("dd-MM-yyyy"));
            ManageCSV.WriteCsv(path, States.firstStartTime.ToString());
            ManageCSV.WriteCsv(path, States.firstEndTime.ToString());
            ManageCSV.WriteCsv(path, States.pauseTime.ToString());
            ManageCSV.WriteCsv(path, States.workTime.ToString());
        }
    }
}
