﻿using System;
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
        private Dictionary<int, IValueReceiver<ChamberSensors>> chambers = new Dictionary<int, IValueReceiver<ChamberSensors>>();

        private const int msgSize = 25;

        private static StopBits GetStopBits(int stopBits) => stopBits switch
        {
            0 => StopBits.None,
            1 => StopBits.One,
            2 => StopBits.Two,
            _ => throw new ArgumentException($"Wrong value: { stopBits }", nameof(stopBits))
        };

        private static Parity GetParity(char parity) => parity switch
        {
            'N' => Parity.None,
            'O' => Parity.Odd,
            'E' => Parity.Even,
            _ => throw new ArgumentException($"Wrong value { parity }", nameof(parity))
        };

        public SerialModbusChamberListener(string port, int baud = 9600, int dataBits = 8, char parity = 'N', int stopBits = 2)
        {
            this.serialPort = new SerialPort(port, baud, GetParity(parity), dataBits, GetStopBits(stopBits));
            modbusBuffer = new ModbusBuffer(msgSize, DetectReadTempHumm); 
        }

        protected SerialModbusChamberListener()
        {
            modbusBuffer = new ModbusBuffer(msgSize, DetectReadTempHumm); 
        }

        public void Add(int slaveId, IValueReceiver<ChamberSensors> receiver)
            => chambers.Add(slaveId, receiver);

        public void Start()
        {
            if (listenerTask != null && listenerTask.Status == TaskStatus.Running)
                Stop();

            stoper = new CancellationTokenSource();
            serialPort.Open();
            serialPort.DataReceived += SerialPort_DataReceived;
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

            if (chambers.TryGetValue(slaveId, out var receiver))
            {
                var buff = new byte[4];
                for (int i = 0; i < 5; i++) responseEnum.MoveNext();
                
                responseEnum.MoveNext();
                buff[1] = responseEnum.Current;
                responseEnum.MoveNext();
                buff[0] = responseEnum.Current;
                responseEnum.MoveNext();
                buff[3] = responseEnum.Current;
                responseEnum.MoveNext();
                buff[2] = responseEnum.Current;

                double h = BitConverter.ToSingle(buff);
                
                responseEnum.MoveNext();
                buff[1] = responseEnum.Current;
                responseEnum.MoveNext();
                buff[0] = responseEnum.Current;
                responseEnum.MoveNext();
                buff[3] = responseEnum.Current;
                responseEnum.MoveNext();
                buff[2] = responseEnum.Current;
                
                double t = BitConverter.ToSingle(buff);

                receiver.ValueReceived(new ChamberSensors { Humidity = h, Temperature = t });
            }
        }

        public void Stop()
        {
            stoper.Cancel();
            listenerTask.Wait();
            serialPort.Close();
        }

        private static byte[] readTempHummStartSequence = new byte[] { 0x03, 0x01, 0x00, 0x00, 0x05 };
        public static bool DetectReadTempHumm(IEnumerable<byte> data)
        {
            if (!readTempHummStartSequence.SequenceEqual(data.Skip(1).Take(5)))
                return false;

            return CrcModbus.Get(data.Take(msgSize - 2)).SequenceEqual(data.Skip(msgSize - 2));
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
