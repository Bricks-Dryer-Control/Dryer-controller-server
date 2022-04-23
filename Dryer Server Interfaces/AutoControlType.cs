using System;

namespace Dryer_Server.Interfaces
{
    [Flags]
    public enum AutoControlType
    {
        PedefinedSettings = 1, 
        StrightFitting = 2, 
        PI = 4, 
        WithPercent = 2^8,
        WithFilter = 2^9,
    }
}
