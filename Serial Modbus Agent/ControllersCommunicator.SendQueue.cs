using System;
using System.Collections.Generic;

namespace Dryer_Server.Serial_Modbus_Agent
{
    public partial class ControllersCommunicator
    {
        private class SendQueue
        {
            private ControllersCommunicator communicator;

            public SendQueue(ControllersCommunicator communicator)
            {
                this.communicator = communicator;
            }

            internal int SendActuators(int id, int actuator1, int actuator2, int actuator3)
            {
                throw new NotImplementedException();
            }

            internal int SendSpecial(int id, int value)
            {
                throw new NotImplementedException();
            }

            internal void Stop(IEnumerable<int> enumerable)
            {
                throw new NotImplementedException();
            }

            internal bool TrySend()
            {
                throw new NotImplementedException();
            }

            internal void Stop(IEnumerable<byte> enumerable)
            {
                throw new NotImplementedException();
            }
        }
    }
}
