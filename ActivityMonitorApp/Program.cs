using ActivityMonitorApp;


class Program
{
    static async Task Main()
    {

        StateMachine stateMachine = new StateMachine();

        while (true)
        {
            await Task.Delay(200);

            stateMachine.CreateState();
            stateMachine.ActiveState();
            stateMachine.InactiveState();
            stateMachine.FullSave();
        }
    }
}
