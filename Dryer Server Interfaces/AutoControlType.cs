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

    public static class AutoControlTypeHelper
    {
        public static bool IsTemperatureBased(this AutoControlType type)
        {
            return type != AutoControlType.PedefinedSettings;
        }
    }
}
