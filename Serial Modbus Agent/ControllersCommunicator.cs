using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dryer_Server.Interfaces;
using NModbus;
using NModbus.Serial;

namespace Dryer_Server.Serial_Modbus_Agent
{
    public partial class ControllersCommunicator : IModbusControllerCommunicator
    {
        private const ushort StartAddress = 4597;
        private const ushort ReadPositionsStartAddress = 4599;
        private const ushort WriteActuatorsAddress = 4603;
        private const ushort WriteSpecialAddress = 4606;
        private static ushort[] WriteActuatorsStartCommand { get; } = new ushort[] { 1, 0 };
        private static ushort[] WriteSpecialStartCommand { get; } = new ushort[] { 5, 0 };
        private static ushort[] WriteStopCommand { get; } = new ushort[] { 0, 0 };

        IModbusSerialMaster rtu;
        SendQueue queue;
        List<Chamber> chambers = new List<Chamber>();
        List<Chamber> inMotion = new List<Chamber>();
        CancellationTokenSource stoper = new();
        SerialPort sp;

        public ControllersCommunicator(string port, int baud = 9600, int dataBits = 8, char parity = 'N', int stopBits = 1)
        {
            if (!string.IsNullOrEmpty(port))
            {
                sp = SerialPortCreator.Create(port, baud, dataBits, parity, stopBits);
                var adapter = new SerialPortAdapter(sp);
                adapter.ReadTimeout = 150;
                adapter.WriteTimeout = 150;
                var factory = new ModbusFactory();
                rtu = factory.CreateRtuMaster(adapter);
                rtu.Transport.Retries = 1;
                rtu.Transport.ReadTimeout = 150;
                rtu.Transport.WriteTimeout = 150;
                rtu.Transport.WaitToRetryMilliseconds = 0;
            }
            queue = new SendQueue(this);
        }

        public ControllersCommunicator(PortSettings controllersPort)
            :this(controllersPort.Port, controllersPort.Baud, controllersPort.DataBits, controllersPort.Parity, controllersPort.StopBits)
        { }

        protected ControllersCommunicator(IModbusSerialMaster rtu)
        {
            this.rtu = rtu;
            queue = new SendQueue(this);
        }

        public void Register(ChamberConfiguration configuration, bool active, IValueReceiver<ChamberControllerStatus> receiver)
        {
            chambers.Add(new Chamber
            {
                Id = (byte)configuration.Id,
                Active = active,
                Receiver = receiver,
            });
        }

        public int SendActuators(int id, int actuator1, int actuator2, int actuator3)
        {
            return queue.SendActuators(id, actuator1, actuator2, actuator3);
        }

        public int SendSpecial(int id, int value)
        {
            return queue.SendSpecial(id, value);
        }

        public int SendAuto(int id, IAutoValueGetter autoValueGetter)
        {
            return queue.SendAuto(id, autoValueGetter);
        }

        public void Start()
        {
            chambers.Sort((x, y) => x.Id.CompareTo(y.Id));
            if (sp != null)
            {
                sp.Open();
                Task.Run(Worker);
            }
        }

        private void Worker()
        {
            var inMotionEnumerator = inMotion.ToList().GetEnumerator();
            var chamberEnumerator = chambers.Where(c => c.Active).GetEnumerator();

            while (!stoper.IsCancellationRequested)
            {
                if (!queue.TrySend())
                {
                    if (inMotionEnumerator.MoveNext())
                    {
                        Read(inMotionEnumerator.Current);
                        continue;
                    }
                    inMotionEnumerator = inMotion.ToList().GetEnumerator();

                    if (!chamberEnumerator.MoveNext())
                    {
                        chamberEnumerator = chambers.Where(c => c.Active).GetEnumerator();
                        if (!chamberEnumerator.MoveNext())
                            continue;
                    }
                    Read(chamberEnumerator.Current);
                }
            }
        }

        private void Read(Chamber chamber)
        {
            try
            {
                var statusRaw = rtu.ReadHoldingRegisters(chamber.Id, StartAddress, 2);
                var positionsRaw = rtu.ReadHoldingRegisters(chamber.Id, ReadPositionsStartAddress, 4);
                
                var status = new ChamberControllerStatus
                {
                    ActualActuator = statusRaw[0],
                    Current1 = positionsRaw[0],
                    Current2 = positionsRaw[1],
                    Current3 = positionsRaw[2],
                    Current4 = positionsRaw[3],
                    QueuePosition = GetQueuePosition(chamber),
                    workingStatus = GetWorkingStatus(statusRaw, chamber),
                };

                Task.Run(() => chamber.Receiver.ValueReceived(status));
                
                if (!chamber.Active && status.QueuePosition == null)
                    inMotion.Remove(chamber);
            }
            catch (Exception e)
            {
                var errorStatus = new ChamberControllerStatus {
                    workingStatus = ChamberControllerStatus.WorkingStatus.Error,
                };
                Task.Run(() => chamber.Receiver.ValueReceived(errorStatus));
                System.Diagnostics.Debug.WriteLine($"Read Error on chamber {chamber.Id}:\n{e.ToString()}");
                if (!sp.IsOpen)
                {
                    try
                    {
                        sp.Open();
                    }
                    catch (Exception)
                    {}
                }
                Task.Delay(200).Wait();
            }
        }

        private int? GetQueuePosition(Chamber chamber)
        {
            return queue.GetPosition(chamber.Id);
        }

        private ChamberControllerStatus.WorkingStatus GetWorkingStatus(ushort[] statusRaw, Chamber chamber)
        {
            if (statusRaw[1] != 0)
                return (ChamberControllerStatus.WorkingStatus)statusRaw[1];

            if (chamber.Active)
                return ChamberControllerStatus.WorkingStatus.NoOperation;
            
            return ChamberControllerStatus.WorkingStatus.Off;
        }

        private void WriteActuators(byte id, ushort[] actuators)
        {
            try
            {
                rtu.WriteMultipleRegisters(id, WriteActuatorsAddress, actuators);
                rtu.WriteMultipleRegisters(id, StartAddress, WriteActuatorsStartCommand);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
            }
        }

        private void WriteSpecial(byte id, ushort value)
        {
            try
            {
                rtu.WriteSingleRegister(id, WriteSpecialAddress, value);
                rtu.WriteMultipleRegisters(id, StartAddress, WriteSpecialStartCommand);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
            }
        }
        
        private void WriteStop(byte id)
        {
            try
            {
                rtu.WriteMultipleRegisters(id, StartAddress, WriteStopCommand);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
            }
        }

        public void Stop()
        {
            stoper.Cancel();
            sp.Close();
        }

        public void StopAllActuators()
        {
            var primary = inMotion.Select(c => c.Id).ToList();
            var secondary = chambers.Select(c => c.Id).Except(primary).ToList();
            queue.Stop(primary.Concat(secondary));
        }

        public void Dispose()
        {
            rtu?.Dispose();
        }

        public bool isChamberListen(int id)
        {
            return chambers[id - 1].Active;
        }

        public void setChamberListen(int id, bool value)
        {
            chambers[id - 1].Active = value;
        }

        public bool IsChamberQueued(int id)
        {
            return queue.IsQueued(Convert.ToByte(id));
        }
    }
}
