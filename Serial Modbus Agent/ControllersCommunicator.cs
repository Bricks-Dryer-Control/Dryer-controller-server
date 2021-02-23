using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dryer_Server.Interfaces;

namespace Dryer_Server.Serial_Modbus_Agent
{
    public class ControllersCommunicator : IModbusControllerCommunicator
    {
        public void Dispose()
        {
        }

        public void Register(ChamberConfiguration configuration, IValueReceiver<ChamberControllerStatus> receiver)
        {
        }

        public int SendActuators(int id, int actuator1, int actuator2, int actuator3)
        {
            return 0;
        }

        public int SendSpecial(int id, int value)
        {
            return 0;
        }

        public void Start()
        {
        }

        public void Stop()
        {
        }

        public void StopAllActuators()
        {
        }
    }
}
