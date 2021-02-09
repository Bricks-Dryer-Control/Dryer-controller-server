using System;

namespace Dryer_Server.Interfaces
{
    public enum AutoControlType
    {
        PedefinedSettings, StrightFitting, PI, PIFF, PID, PIDFF
    }

    public static class AutoControlTypeHelper
    {
        public static bool IsTemperatureBased(this AutoControlType type)
        {
            return type != AutoControlType.PedefinedSettings;
        }
    }
}
