using System.Collections.Generic;

namespace Dryer_Server.Interfaces
{
    public interface IDryerConfigurationPersistance
    {
        IEnumerable<ChamberConfiguration> GetChamberConfigurations();
        void Save(int id, ChamberConfiguration configuration);
    }
}
