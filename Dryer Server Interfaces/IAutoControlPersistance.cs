using System;
using System.Collections.Generic;

namespace Dryer_Server.Interfaces
{
    public interface IAutoControlPersistance: IDisposable
    {
        IEnumerable<string> GetAutoControls();
        void Save(AutoControl autoControl);
        AutoControl Load(string name);
    }
}
