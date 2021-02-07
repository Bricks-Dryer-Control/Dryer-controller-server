using System;
using Dryer_Server.Interfaces;

namespace Dryer_Server.Core
{
    public class Chamber : IValueReceiver<ChamberSensors>
    {  
        public DateTime LastReadingTimeUtc { get; private set; }

        public ChamberSensors LastReadingValue { get; private set; }

     
        public void ValueReceived(ChamberSensors v)
        {
            LastReadingTimeUtc = DateTime.UtcNow;
            LastReadingValue = v;
        }
    }
}
