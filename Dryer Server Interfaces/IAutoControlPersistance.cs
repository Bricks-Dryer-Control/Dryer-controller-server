using System;
using System.Collections.Generic;

namespace Dryer_Server.Interfaces
{
    public interface IAutoControlPersistance: IDisposable
    {
        IEnumerable<string> GetAutoControls();
        void SaveDeactivateLatest(AutoControl autoControl);
        AutoControl Load(string name);
        AutoControl GetControlWithItems(string name);
        IEnumerable<AutoControl> GetControls();
        void Delete(string name);
    }
}
