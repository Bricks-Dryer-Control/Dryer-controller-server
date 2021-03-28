using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dryer_Server.Interfaces
{
    public interface IProgramImporter
    {
        void Import(string filePath, string name);
    }
}
