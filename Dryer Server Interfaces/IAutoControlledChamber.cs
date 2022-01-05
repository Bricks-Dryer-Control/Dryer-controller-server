﻿namespace Dryer_Server.Interfaces
{
    public interface IAutoControlledChamber 
    {
        bool IsAutoControl { get; }
        bool IsQueued { get; }
        int CurrentInFlow { get; }
        int CurrentOutFlow { get; }
        int CurrentThroughFlow { get; }
        void AddToQueue(IFlowInterpolator flowInterpolator);
    }
}