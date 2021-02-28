using System;
using System.IO.Ports;

namespace Dryer_Server.Serial_Modbus_Agent
{
    public static class SerialPortCreator
    {
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

        public static SerialPort Create(string port, int baud = 9600, int dataBits = 8, char parity = 'N', int stopBits = 1)
        {
            return new SerialPort(port, baud, GetParity(parity), dataBits, GetStopBits(stopBits));
        }
    }
}
