using Dryer_Server.Interfaces;

namespace Dryer_Server.Dryer_Simulator
{
    public interface ISimulator: ISerialModbusChamberListener, IModbusControllerCommunicator
    { }
}
