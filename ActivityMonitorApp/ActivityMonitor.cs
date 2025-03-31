using System.IO;
using ActivityMonitorApp;
using static System.Runtime.InteropServices.JavaScript.JSType;


class ActivityMonitor
{
    static async Task Main()
    {

        StateMachine stateMachine = new StateMachine();

        while (true)
        {
            await Task.Delay(200);

            //Switch-Statement um zwischen den Zuständen zu wechseln
            switch (stateMachine.CurrentState)
            {
                case State.Create:
                    stateMachine.CreateState();
                    break;
                case State.Active:
                    stateMachine.ActiveState();
                    break;
                case State.Inactive:
                    stateMachine.InactiveState();
                    break;
                case State.FullSave:
                    stateMachine.FullSave();
                    break;
            }
        }
    }
}
