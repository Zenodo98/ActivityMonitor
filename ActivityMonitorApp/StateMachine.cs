﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ActivityMonitorProgram;

namespace ActivityMonitorApp
{
    public enum State
    {
        Active,
        Inactive,
        Create,
        FullSave
    }


    public class StateMachine
    {
        public State CurrentState { get; set; }

        private static string path = "Data\\";
        private static string fileName = DateTime.Now.ToString("dd-MM-yyyy") + ".csv";


        
        ActivityTimes ActivityTimes = new ActivityTimes();
        FileManager CsvActivityFile = new FileManager(@path, fileName);
        Timers OnTimeoutInaktiv = new Timers(1000 * 60 * Settings.pausePufferMinutes);
        Timers PrevMousePosTimer = new Timers(1000);
        Timers AutoSaveTimer = new Timers(1000 * 60 * Settings.autoSaveTime);
        Timers OnTimeoutFullSave = new Timers(1000 * 20);
        Save SaveTimes = new Save();

        public StateMachine() 
        {
            CurrentState = State.Create;
        }

        /// <summary>
        /// In dem Zustand wird die Datei erstellt
        /// </summary>
        public void CreateState()
        {

            CsvActivityFile.CreateFile("Datum,Anfang,Ende,Pause,Gesamtzeit");
            CsvActivityFile.WriteLine();    
            CurrentState = State.Active;
            
        }

        /// <summary>
        /// Solange der PC benutzt wird ist der Zustand aktiv
        /// </summary>
        public void ActiveState()
        {
            //Startzeit, Endzeit, berechnen der Arbeitszeit
            ActivityTimes.SetStartTime();
            ActivityTimes.SetAktivEndTime();
            ActivityTimes.CalculateWorkTime();

            //Die Timer starten
            PrevMousePosTimer.StartPreviousMousePositionTimer();
            AutoSaveTimer.StartAutoSave();
            OnTimeoutInaktiv.OnTimeoutSwitchToInaktiv();

            //automatisches Speichern
            SaveTimes.AutoSave(ActivityTimes.StartTime.ToString(), ActivityTimes.AktivEndTime.ToString(), ActivityTimes.BreakTime.ToString(), ActivityTimes.WorkTime.ToString(), AutoSaveTimer.AutoSave);
            AutoSaveTimer.AutoSave = false;

            //Wenn Datei nicht existiert, wechsel zu Create Zustand
            if (!File.Exists(path + fileName))
            {
                CurrentState = State.Create;
            }
            //Bei einer Mausbewegung wird der Timer gestoppt
            else if (CursorPosition.GetCursorPosition().X <= PrevMousePosTimer.previousMouseX - 50 || CursorPosition.GetCursorPosition().X >= PrevMousePosTimer.previousMouseX + 50 ||
                CursorPosition.GetCursorPosition().Y <= PrevMousePosTimer.previousMouseY - 50 || CursorPosition.GetCursorPosition().Y >= PrevMousePosTimer.previousMouseY + 50)
            {
                OnTimeoutInaktiv.StopTimer();
            }
            //Wenn die Zeit abläuft wird zu Inaktiv gewechselt
            else if (OnTimeoutInaktiv.AwayFromKeyboard)
            {
                Console.WriteLine("");
                Console.WriteLine("Du bist inaktiv");
                ActivityTimes.WorkTime = ActivityTimes.WorkTime.Add(-Settings.pausePuffer);
                AutoSaveTimer.StopTimer();
                CurrentState = State.Inactive;
            }     
        }

        /// <summary>
        /// Solange der PC nicht benutzt wird ist der Zustand inaktiv
        /// </summary>
        public void InactiveState()
        {
            //Endzeit, berechnen der Pausenzeit
            ActivityTimes.SetInaktivEndTime();
            ActivityTimes.CalculateBreakTime();


            ActivityTimes.BreakTime = ActivityTimes.BreakTime.Add(Settings.pausePuffer);

            //automatisches Speichern, funktioniert nicht in standby
            SaveTimes.AutoSave(ActivityTimes.StartTime.ToString(), ActivityTimes.InaktivEndTime.ToString(), ActivityTimes.BreakTime.ToString(), ActivityTimes.WorkTime.ToString(), AutoSaveTimer.AutoSave);
            AutoSaveTimer.AutoSave = false;
                

            //Wenn Datei nicht existiert, wechsel zu Create Zustand
            if (!File.Exists(path+fileName))
            {
                CurrentState = State.Create;
            }

            //Bei Mausbewegung wechsel zu aktiv
            else if (CursorPosition.GetCursorPosition().X <= PrevMousePosTimer.previousMouseX - 50 || CursorPosition.GetCursorPosition().X >= PrevMousePosTimer.previousMouseX + 50 ||
                CursorPosition.GetCursorPosition().Y <= PrevMousePosTimer.previousMouseY - 50 || CursorPosition.GetCursorPosition().Y >= PrevMousePosTimer.previousMouseY + 50)
            {
                OnTimeoutFullSave.SwitchToFullSave();
            }

            if (OnTimeoutFullSave.FullSave)
            {
                CurrentState = State.FullSave;
            }  
        }

        /// <summary>
        /// In dem Zustand wird vollständig gespeichert
        /// </summary>
        public void FullSave()
        {   
            OnTimeoutFullSave.StopTimer();
            OnTimeoutFullSave.FullSave = false;

            SaveTimes.FullSave(ActivityTimes.StartTime.ToString(), ActivityTimes.InaktivEndTime.ToString(), ActivityTimes.BreakTime.ToString(), ActivityTimes.WorkTime.ToString());
            ActivityTimes.ClearDateTimes();


            Console.WriteLine("wurde gespeichert");

            ActivityTimes.ResetStartTime = true;
            OnTimeoutInaktiv.AwayFromKeyboard = false;

            CurrentState = State.Active;
        }
    }
}
