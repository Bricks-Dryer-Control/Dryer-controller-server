using System;
using System.Collections.Generic;

namespace Dryer_Server.Interfaces
{
    public interface IAutoControlPersistance: IDisposable
    {
        IEnumerable<string> GetAutoControls();
        void SaveDeactivateLatest(AutoControl autoControl);
        AutoControl Load(string name);
        IEnumerable<AutoControl> GetControls();
        void SaveState(int chamberId, IAutoControl autoControl);
        IEnumerable<(int chamberId, DateTime startUtc, AutoControl autoControl)> LoadStates();
        void Delete(string name);
    }
}
