﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Dryer_Server.Serial_Modbus_Agent
{
    public class ModbusBuffer
    {
        readonly byte[] buff;
        int pos = 0;
        Func<IEnumerable<byte>, bool> check;
        IEnumerable<byte> Collection => buff.Skip(pos).Concat(buff.Take(pos));

        public ModbusBuffer(int size, Func<IEnumerable<byte>, bool> check)
        {
            buff = new byte[size];
            this.check = check;
        }

        public IEnumerable<byte> Current => Collection;

        public bool InsertAndCheck(byte val)
        {
            buff[pos++] = val;
            if (pos >= buff.Length)
                pos = 0;

            return check(Collection);
        }
    }
}
