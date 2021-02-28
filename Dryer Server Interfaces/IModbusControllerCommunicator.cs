using System;

namespace Dryer_Server.Interfaces
{
    public interface IModbusControllerCommunicator: IDisposable
    {
        void Register(ChamberConfiguration configuration, bool active, IValueReceiver<ChamberControllerStatus> receiver);
        void Start();
        void Stop();
        int SendActuators(int id, int actuator1, int actuator2, int actuator3);
        int SendSpecial(int id, int value);
        void StopAllActuators();
    }
}