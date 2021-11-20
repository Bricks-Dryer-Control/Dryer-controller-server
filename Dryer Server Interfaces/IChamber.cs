namespace Dryer_Server.Interfaces
{
    public interface IChamber 
    {
        bool IsAutoControl { get; }
        bool IsQueued { get; }
        int CurrentInFlow { get; }
        int CurrentOutFlow { get; }
        int CurrentThroughFlow { get; }
        void AddToQueue();
    }
}