using Dryer_Server.Interfaces;
using System;

namespace Dryer_Server.Serial_Modbus_Agent
{
    public class DirSensor
    {
        private readonly DirSensorSettings settings;

        public DirSensor(DirSensorSettings settings)
        {
            this.settings = settings;
        }

        public bool? Status { get; set; }
        public byte ControllerId => settings.ControllerId;
        public ushort InputNumber => settings.InputNumber;
    }
}