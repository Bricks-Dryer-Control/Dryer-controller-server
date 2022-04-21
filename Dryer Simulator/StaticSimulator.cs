using Dryer_Server.Interfaces;
using System;
using System.Collections.Generic;
using System.Timers;

namespace Dryer_Server.Dryer_Simulator
{
    internal class StaticSimulator : Simulator
    {
        Timer timer = new()
        {
            AutoReset = true,
            Interval = 5e3,
            Enabled = false,
        };

        public StaticSimulator()
        {
            timer.Elapsed += Timer_Elapsed;
        }

        public override void Start()
        {
            timer.Start();
        }

        public override void Stop()
        {
            timer.Stop();
        }
        public override void Dispose()
        {
            timer?.Dispose();
            base.Dispose();
        }

        readonly List<ChamberSensors> sensors = new()
        {
            new ChamberSensors { Humidity = 10F, Temperature = 90F },
            new ChamberSensors { Humidity = 20F, Temperature = 60F },
            new ChamberSensors { Humidity = 0F, Temperature = 30F },
            new ChamberSensors { Humidity = 40F, Temperature = -10F },
            new ChamberSensors { Humidity = 50F, Temperature = 20F },
            new ChamberSensors { Humidity = 60F, Temperature = 100F },
            new ChamberSensors { Humidity = 70F, Temperature = 0F },
        };

        readonly List<ChamberControllerStatus> statuses = new()
        {
            new ChamberControllerStatus { 
                ActualActuator = 0, 
                QueuePosition = null, 
                Current1 = 100, 
                Current2 = 200, 
                Current3 = 100, 
                Current4 = 50, 
                workingStatus = ChamberControllerStatus.WorkingStatus.Off},
            new ChamberControllerStatus
            {
                ActualActuator = 0,
                QueuePosition = null,
                Current1 = 110,
                Current2 = 210,
                Current3 = 110,
                Current4 = 150,
                workingStatus = ChamberControllerStatus.WorkingStatus.NoOperation
            },
            new ChamberControllerStatus
            {
                ActualActuator = 1,
                QueuePosition = 0,
                Current1 = 120,
                Current2 = 220,
                Current3 = 120,
                Current4 = 250,
                workingStatus = ChamberControllerStatus.WorkingStatus.ActuatorStarted,
            },
            new ChamberControllerStatus
            {
                ActualActuator = 0,
                QueuePosition = null,
                Current1 = 0,
                Current2 = 0,
                Current3 = 0,
                Current4 = 0,
                workingStatus = ChamberControllerStatus.WorkingStatus.Error
            }
        };

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine($"Queue: {string.Join(',', queue)}");
            queue.Clear();

            foreach (var r in valueReceivers)
                r.receiver.ValueReceived(sensors[r.id % sensors.Count]);

            foreach (var r in statusReceivers)
                r.receiver.ValueReceived(statuses[r.chamber.Id % statuses.Count]);
        }
    }
}