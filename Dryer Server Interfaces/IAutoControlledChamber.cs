using System;

namespace Dryer_Server.Interfaces
{
    public interface IAutoControlledChamber
    {
        ChamberConvertedStatus ConvertedStatus { get; }
        ChamberSensors ChamberSensors { get; }
        int OutFlowOffset { get; }
        bool? DirSensor { get; }
        int[] SetValuesGetActuators(int inFlow, int outFlow, int troughtFlow);
        void EnqueueAutoControl(IAutoValueGetter valueGetter);
        void AutoControlChanged();
    }
}