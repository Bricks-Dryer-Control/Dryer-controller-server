using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dryer_Server.Interfaces
{
    public interface IMainController
    {
        Task InitializeAsync();
        void Start();
        void Stop();
        void ChangeActuators(int no, int inFlow, int outFlow, int throughFlow);
        void ChangeWent(int no, int value);
        void ChangeRoof(int no, bool isRoof);
        void StopAll();
        CommonStatus GetCommon();
    }
}
