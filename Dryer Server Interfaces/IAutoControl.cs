using System;

namespace Dryer_Server.Interfaces
{
    public interface IAutoControl: IDisposable, IAutoValueGetter
    {
        string Name { get; }
        DateTime StartDateUtc { get; }
        bool Active { get; }

        void Start();
        void Stop();
        void SetChamber(IAutoControlledChamber chamber);
    }
}