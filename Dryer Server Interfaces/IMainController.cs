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
        void ChangeChamberReading(int no, bool value);
        HistoryResponse GetHistory(int no, DateTime from, DateTime to);
        void StartAutoControl(int chamberId, string autoControlName, TimeSpan startingPoint);
    }
}
