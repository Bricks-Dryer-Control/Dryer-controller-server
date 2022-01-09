﻿using Dryer_Server.Interfaces;

namespace Dryer_Server_Interfaces
{
    public interface ITimeBasedAutoControlPersistance
    {
        void Save(ITimeBasedAutoControl timeBasedAutoControl);
        ITimeBasedAutoControl LoadTimeBasedForChamber(IAutoControlledChamber chamber);
    }
}
