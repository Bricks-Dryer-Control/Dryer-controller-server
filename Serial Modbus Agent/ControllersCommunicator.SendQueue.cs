using System;
using System.Collections.Generic;
using System.Linq;
using Dryer_Server.Interfaces;
using NLog.LayoutRenderers.Wrappers;

namespace Dryer_Server.Serial_Modbus_Agent
{
    public partial class ControllersCommunicator
    {
        private class SendQueue
        {
            private object ItemsLock = new object();
            private List<IQueueItem> Items { get; } = new List<IQueueItem>();
            private ControllersCommunicator communicator;

            public SendQueue(ControllersCommunicator communicator)
            {
                this.communicator = communicator;
            }

            internal int SendTimeBased(int id, IFlowInterpolator interpolator)
            {
                var newSend = new InterpolatedActuators(id, interpolator);
                lock (ItemsLock)
                {
                    var item=Items.SingleOrDefault(i=>i.No==id && i is InterpolatedActuators);
                    if (item == null)
                    {
                        Items.Add(newSend);
                    }
                    else
                        (item as InterpolatedActuators).Interpolator = interpolator;

                    return Items.Count;
                }
            }

            internal int SendActuators(int id, int actuator1, int actuator2, int actuator3)
            {
                var newSend = new NormalActuators(id, actuator1, actuator2, actuator3);
                lock (ItemsLock)
                {
                    Items.RemoveAll(i => i.No == id && i is NormalActuators);
                    Items.Add(newSend);
                    return Items.Count;
                }
            }

            internal int SendSpecial(int id, int value)
            {
                var newSpecial = new SpecialActuator(id, value);
                lock (ItemsLock)
                {
                    Items.RemoveAll(i => i.No == id && i is SpecialActuator);
                    Items.Add(newSpecial);
                    return Items.Count;
                }
            }

            internal bool TrySend()
            {
                lock (ItemsLock)
                {
                    if (Items.Any())
                    {
                        var item = Items[0];
                        Items.RemoveAt(0);
                        try
                        {
                            item.SendQueue(communicator);
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine(ex.ToString());
                        }
                        return true;
                    }
                }
                return false;
            }

            internal void Stop(IEnumerable<byte> ids)
            {
                lock (ItemsLock)
                {
                    Items.Clear();
                    Items.AddRange(ids.Select(id => new StopController(id)));
                }
            }

            internal int? GetPosition(byte id)
            {
                int index;
                lock (ItemsLock)
                {
                    index = Items.FindIndex(i => i.No == id);
                }
                
                if (index >= 0)
                    return index + 1;
                return null;
            }

            internal bool IsQueued(byte id)
            {
                return GetPosition(id) != null;
            }
            
            private interface IQueueItem
            {
                int No { get; }
                void SendQueue(ControllersCommunicator communicator);
            }

            private class InterpolatedActuators : IQueueItem
            {
                private readonly byte no;
                public IFlowInterpolator Interpolator { get; set; }

                public InterpolatedActuators(int no,IFlowInterpolator interpolator)
                {
                    this.Interpolator = interpolator;
                    this.no = (byte)no;
                }


                public int No => no;
                public void SendQueue(ControllersCommunicator communicator)
                {
                    var interpolated=Interpolator.InterpolateFlow();
                    ushort[] actuators = { (ushort)interpolated.InFlow, (ushort)interpolated.OutFlow, (ushort)interpolated.ThroughFlow };
                    communicator.WriteActuators(no, actuators);
                }
            }

            private class NormalActuators : IQueueItem
            {
                private byte no;
                private ushort[] actuators;

                public NormalActuators(int no,  int actuator1, int actuator2, int actuator3)
                {
                    this.no = (byte)no;
                    actuators = new ushort[] { (ushort)actuator1, (ushort)actuator2, (ushort)actuator3 };
                }

                public int No => no;

                public void SendQueue(ControllersCommunicator communicator)
                {
                    communicator.WriteActuators(no, actuators);
                }
            }

            private class SpecialActuator : IQueueItem
            {
                private byte no;
                private ushort value;

                public SpecialActuator(int no, int value)
                {
                    this.no = (byte)no;
                    this.value = (ushort)value;
                }

                public int No => no;

                public void SendQueue(ControllersCommunicator communicator)
                {
                    communicator.WriteSpecial(no, value);
                }
            }

            private class StopController : IQueueItem
            {
                private byte no;

                public StopController(byte no)
                {
                    this.no = no;
                }

                public int No => no;

                public void SendQueue(ControllersCommunicator communicator)
                {
                    communicator.WriteStop(no);
                }
            }
        }
    }
}
