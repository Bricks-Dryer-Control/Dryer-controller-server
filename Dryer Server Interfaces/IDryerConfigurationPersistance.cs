using System;
using System.Collections.Generic;

namespace Dryer_Server.Interfaces
{
    public interface IDryerConfigurationPersistance: IDisposable
    {
        IEnumerable<ChamberConfiguration> GetChamberConfigurations();
        void Save(int id, ChamberConfiguration configuration);
    }
}
