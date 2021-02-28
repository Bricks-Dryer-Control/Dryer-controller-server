using Dryer_Server.Interfaces;

namespace Dryer_Server.Serial_Modbus_Agent
{
    public partial class ControllersCommunicator
    {
        class Chamber
        {
            public byte Id {get;set;}
            public bool Active {get;set;}
            public IValueReceiver<ChamberControllerStatus> Receiver {get;set;}
        }
    }
}
