using System;

namespace Dryer_Server.Interfaces
{
    public interface ISerialModbusChamberListener : IDisposable
    {
        void Add(int slaveId, IValueReceiver<ChamberSensors> receiver);
        void Start();
        void Stop();
    }
}
