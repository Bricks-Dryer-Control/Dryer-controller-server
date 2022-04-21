using System;

namespace Dryer_Server.Interfaces
{
    public interface IAutoValueGetter
    {
        bool ShouldSend { get; }

        int[] GetValues();
    }
}