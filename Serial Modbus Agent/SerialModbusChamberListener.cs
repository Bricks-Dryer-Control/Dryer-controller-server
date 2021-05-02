using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dryer_Server.Interfaces;

namespace Dryer_Server.Serial_Modbus_Agent
{
    public class SerialModbusChamberListener : ISerialModbusChamberListener
    {
        private SerialPort serialPort = null;
        private CancellationTokenSource stoper = null;
        private Task listenerTask = null;
        private ModbusBuffer modbusBuffer;
        private Dictionary<int, List<IValueReceiver<ChamberSensors>>> chambers = new();

        private const int requestSize = 8;
        private const int msgSize = 23;

        public SerialModbusChamberListener(string port, int baud = 9600, int dataBits = 8, char parity = 'N', int stopBits = 2)
        {
            this.serialPort = SerialPortCreator.Create(port, baud, dataBits, parity, stopBits);
            modbusBuffer = new ModbusBuffer(msgSize, DetectReadTempHumm); 
        }

        protected SerialModbusChamberListener()
        {
            modbusBuffer = new ModbusBuffer(msgSize, DetectReadTempHumm); 
        }

        public void Add(int slaveId, IValueReceiver<ChamberSensors> receiver)
        {
            if (chambers.TryGetValue(slaveId, out var receivers))
                receivers.Add(receiver);
            else
                chambers.Add(slaveId, new List<IValueReceiver<ChamberSensors>>{ receiver });
        }

        public void Start()
        {
            if (listenerTask != null && listenerTask.Status == TaskStatus.Running)
                Stop();

            stoper = new CancellationTokenSource();
            serialPort.DataReceived += SerialPort_DataReceived;
            serialPort.Open();
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int readed;

            while (serialPort.BytesToRead > 0 && (readed = serialPort.ReadByte()) >= 0)
            {
                if (modbusBuffer.InsertAndCheck((byte)readed))
                    ReadValues(modbusBuffer.Current);
            }
        }

        public void ReadValues(IEnumerable<byte> current)
        {
            using var responseEnum = current.GetEnumerator();
            responseEnum.MoveNext();
            var slaveId = responseEnum.Current;

            if (chambers.TryGetValue(slaveId, out var receivers))
            {
                var buff = new byte[4];
                for (int i = 0; i < requestSize + 2; i++) responseEnum.MoveNext();
                
                responseEnum.MoveNext();
                buff[1] = responseEnum.Current;
                responseEnum.MoveNext();
                buff[0] = responseEnum.Current;
                responseEnum.MoveNext();
                buff[3] = responseEnum.Current;
                responseEnum.MoveNext();
                buff[2] = responseEnum.Current;

                var h = BitConverter.ToSingle(buff);
                
                responseEnum.MoveNext();
                buff[1] = responseEnum.Current;
                responseEnum.MoveNext();
                buff[0] = responseEnum.Current;
                responseEnum.MoveNext();
                buff[3] = responseEnum.Current;
                responseEnum.MoveNext();
                buff[2] = responseEnum.Current;
                
                var t = BitConverter.ToSingle(buff);

                foreach (var receiver in receivers)
                    receiver.ValueReceived(new ChamberSensors { Humidity = h, Temperature = t });
            }
        }

        public void Stop()
        {
            stoper.Cancel();
            listenerTask.Wait();
            serialPort.Close();
        }

        private readonly static byte[] readTempHummStartSequence = new byte[] { 0x03, 0x01, 0x00, 0x00, 0x05 };
        private readonly static byte[] zeros = new byte[] { 0x00, 0x00 };

        public static bool DetectReadTempHumm(IEnumerable<byte> data)
        {
            return readTempHummStartSequence.SequenceEqual(data.Skip(1).Take(5))
                && data.Skip(msgSize - 4).Take(2).SequenceEqual(zeros)
                && CrcModbus.Get(data.Take(requestSize - 2)).SequenceEqual(data.Skip(requestSize - 2).Take(2))
                && data.Take(2).SequenceEqual(data.Skip(requestSize).Take(2))
                && data.ElementAt(requestSize + 2) == 0x0A
                && CrcModbus.Get(data.Skip(requestSize).Take(13)).SequenceEqual(data.Skip(msgSize - 2));
        }

        private bool _disposed = false;
        protected virtual void Dispose(bool disposing)
            {
                if (_disposed)
                    return;

                if (disposing)
                {
                    serialPort?.Dispose();
                }

                _disposed = true;
            }

        public void Dispose() => Dispose(true);
    }
}
