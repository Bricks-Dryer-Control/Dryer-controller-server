using Dryer_Server.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dryer_Server.Dryer_Simulator
{
    public abstract class Simulator: ISimulator
    {
        public static ISimulator Get(string config)
        {
            return new StaticSimulator();
        }

        protected List<ValueReceiver> valueReceivers = new();
        protected List<StatusReceiver> statusReceivers = new();
        protected Queue<int> queue = new();

        public virtual void Add(int slaveId, IValueReceiver<ChamberSensors> receiver)
        {
            valueReceivers.Add(new ValueReceiver(slaveId, receiver));
        }

        public virtual void Dispose()
        { }

        public virtual bool isChamberListen(int id)
        {
            return statusReceivers.FirstOrDefault(x => x.chamber.Id == id)?.active ?? false;
        }

        public virtual bool IsChamberQueued(int id)
        {
            return queue.Contains(id);
        }

        public virtual void Register(ChamberConfiguration configuration, bool active, IValueReceiver<ChamberControllerStatus> receiver)
        {
            statusReceivers.Add(new StatusReceiver(configuration, active, receiver));
        }

        public virtual int SendActuators(int id, int actuator1, int actuator2, int actuator3)
        {
            Console.WriteLine($"Chamber {id} queued: {actuator1}, {actuator2}, {actuator3}");
            queue.Enqueue(id);
            return queue.Count;
        }

        public virtual int SendSpecial(int id, int value)
        {
            Console.WriteLine($"Chamber {id} queued special: {value}");
            queue.Enqueue(id);
            return queue.Count;
        }

        public int SendAuto(int id, IAutoValueGetter autoValueGetter)
        {
            Console.WriteLine($"Send Auto {id} should {autoValueGetter.ShouldSend}: {string.Join(' ',autoValueGetter.GetValues()) }");
            queue.Enqueue(id);
            return queue.Count;
        }

        public virtual void setChamberListen(int id, bool value)
        {
            Console.WriteLine($"Chamber {id} active {value}");
            foreach (var t in statusReceivers.Where(x => x.chamber.Id == id))
                t.active = value;
        }

        public abstract void Start();

        public abstract void Stop();

        public virtual void StopAllActuators()
        {
            Console.WriteLine("Stop all");
        }

        public CommonStatus GetCommonStatus()
        {
            return new CommonStatus
            {
                Direction = isChamberListen(1),
                InQueue = 1,
                TurnedOn = 2,
                WorkingNow = 3,
            };
        }

        protected record StatusReceiver
        {
            public ChamberConfiguration chamber;
            public bool active;
            public IValueReceiver<ChamberControllerStatus> receiver;

            public StatusReceiver(ChamberConfiguration chamber, bool active, IValueReceiver<ChamberControllerStatus> receiver)
            {
                this.chamber = chamber;
                this.active = active;
                this.receiver = receiver;
            }
        }

        protected record ValueReceiver
        {
            public int id;
            public IValueReceiver<ChamberSensors> receiver;

            public ValueReceiver(int id, IValueReceiver<ChamberSensors> receiver)
            {
                this.id = id;
                this.receiver = receiver;
            }
        }
    }
}
