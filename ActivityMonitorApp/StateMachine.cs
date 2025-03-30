using System;
using System.Collections.Generic;
using System.Linq;
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
        private State CurrentState { get; set; }

        private static string path = "Data\\";
        private static string fileName = DateTime.Now.ToString("dd-MM-yyyy") + ".csv";


        
        ActivityTimes ActivityTimes = new ActivityTimes();
        FileHandler CsvActivityFile = new FileHandler(@path, fileName);
        TimerHandler OnTimeoutInaktiv = new TimerHandler(1000 * 60 * Settings.pausePufferMinutes);
        TimerHandler PrevMousePosTimer = new TimerHandler(1000);
        TimerHandler AutoSaveTimer = new TimerHandler(1000 * 60 * Settings.autoSaveTime);
        Save SaveTimes = new Save();

        public StateMachine() 
        {
            CurrentState = State.Create;
        }

        public void SwitchState(State state) 
        {
            CurrentState = state;
        }

        /// <summary>
        /// In dem Zustand wird die Datei erstellt
        /// </summary>
        public void CreateState()
        {
            if (CurrentState == State.Create)
            {
                CsvActivityFile.CreateFile("Datum,Anfang,Ende,Pause,Gesamtzeit");
                CsvActivityFile.WriteLine();    
                CurrentState = State.Active;
            }
        }

        /// <summary>
        /// Solange der PC benutzt wird ist der Zustand aktiv
        /// </summary>
        public void ActiveState()
        {
            if (CurrentState == State.Active)
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
                //Bei einer Muasbewegung wird der Timer gestoppt
                else if (CursorPosition.GetCursorPosition().X <= PrevMousePosTimer.previousMouseX - 50 || CursorPosition.GetCursorPosition().X >= PrevMousePosTimer.previousMouseX + 50 ||
                    CursorPosition.GetCursorPosition().Y <= PrevMousePosTimer.previousMouseY - 50 || CursorPosition.GetCursorPosition().Y >= PrevMousePosTimer.previousMouseY + 50)
                {
                    OnTimeoutInaktiv.StopTimer();

                }
                //Wenn die Zeit abläuft wird zu Inaktiv gewechselt
                else if (OnTimeoutInaktiv.AwayFromKeyboard)
                {
                    Console.WriteLine("Du bist inaktiv");
                    ActivityTimes.WorkTime = ActivityTimes.WorkTime.Add(-Settings.pausePuffer);
                    CurrentState = State.Inactive;
                }
            }
        }

        /// <summary>
        /// Solange der PC nicht benutzt wird ist der Zustand inaktiv
        /// </summary>
        public void InactiveState()
        {
            if (CurrentState == State.Inactive)
            {
                //Endzeit, berechnen der Pausenzeit
                ActivityTimes.SetInaktivEndTime();
                ActivityTimes.CalculateBreakTime();


                ActivityTimes.BreakTime = ActivityTimes.BreakTime.Add(Settings.pausePuffer);

                //automatisches Speichern
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
                    Console.WriteLine("Du bist aktiv");
                    OnTimeoutInaktiv.AwayFromKeyboard = false;
                    CurrentState = State.FullSave;
                }
            }
        }

        /// <summary>
        /// In dem Zustand wird vollständig gespeichert
        /// </summary>
        public void FullSave()
        {
            
            if (CurrentState == State.FullSave) 
            {
                SaveTimes.FullSave(ActivityTimes.StartTime.ToString(), ActivityTimes.InaktivEndTime.ToString(), ActivityTimes.BreakTime.ToString(), ActivityTimes.WorkTime.ToString());
                ActivityTimes.ClearDateTimes();
                ActivityTimes.ResetStartTime = true;
                CurrentState = State.Active;
            }
        }
    }
}
