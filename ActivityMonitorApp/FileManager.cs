namespace ActivityMonitorApp
{
    class FileManager
    {
        public string FilePath { get; set; }

        public string FileName { get; set; }


        /// <summary>
        /// Hilft beim erstellen und bearbeiten von Dateien.
        /// </summary>
        /// <param name="filePath">Speicherort</param>
        /// <param name="fileName">Dateiname</param>
        public FileManager(string filePath, string fileName)
        { 
            FilePath = filePath;
            FileName = fileName;
        }


        /// <summary>
        /// Die Methode erstellt eine Datei.Optional kann auch Inhalt miterstellt werden.
        /// </summary>
        /// <param name="content">Inhalt</param>
        public void CreateFile(string content = "")
        {

            if (!System.IO.Directory.Exists(FilePath))
            {
                System.IO.Directory.CreateDirectory(FilePath);
            }
                

            if (!System.IO.File.Exists(FilePath + FileName))
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(FilePath + FileName, true))
                {
                    file.WriteLine(content);
                    file.WriteLine("");
                    Console.WriteLine(FileName + " wurde erstellt" );
                    Console.WriteLine();
                }
            }
        }


        /// <summary>
        /// Die Methode schreibt Inhalt in die davor erstellte Datei in die selbe Zeile
        /// </summary>
        /// <param name="content">Inhalt</param>
        public void Write(string content)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(FilePath + FileName, true))
            {
                file.Write(content);
            }
        }


        /// <summary>
        /// Die Methode schreibt Inhalt in die davor erstellte Datei in eine neue Zeile
        /// </summary>
        /// <param name="content">Inhalt</param>
        public void WriteLine(string content = "")
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(FilePath + FileName, true))
            {
                file.WriteLine(content);
            }
        }


        /// <summary>
        /// Entfernt die letzte Zeile
        /// </summary>
        public void RemoveLastLine()
        {
            var lines = System.IO.File.ReadAllLines(FilePath + FileName);
            System.IO.File.WriteAllLines(FilePath + FileName, lines.Take(lines.Length - 1).ToArray());
        }
    }
}
