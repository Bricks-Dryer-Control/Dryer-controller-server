using System;
using System.Linq;
using System.Timers;
using System.Collections.Generic;
using Dryer_Server.Interfaces;

namespace Dryer_Server.AutomaticControl
{
    public class TimeBasedAutoControl: IDisposable
    {
        IChamber Chamber { get; }
        DateTime StartMoment { get; }
        AutoControlItem Last { get; set; } = null;
        AutoControlItem Next { get; set; } = null;
        IEnumerator<AutoControlItem> ControlEnumerator { get; set; } = null;
        AutoControl AutoControl { get; }
        Timer Timer { get; }

        public TimeBasedAutoControl(TimeSpan checkingDelay, TimeSpan currentMoment, AutoControl autoControl, IChamber chamber)
        {
            StartMoment = DateTime.UtcNow - currentMoment;
            Chamber = chamber;
            AutoControl = autoControl;
            
            ControlEnumerator = autoControl.Sets.OrderBy(s => s.Time).GetEnumerator();
            if (ControlEnumerator.MoveNext())
                Next = ControlEnumerator.Current;

            Timer = new Timer {
                AutoReset = true,
                Enabled = false,
                Interval = checkingDelay.TotalMilliseconds,
            };
            Timer.Elapsed += ElapsedEventHandler;
            Timer.Start();
        }

        void ElapsedEventHandler(object sender, ElapsedEventArgs eventArgs)
        {
            if (!Chamber.IsAutoControl) return;
            if (Chamber.IsQueued) return;

            var moment = SetLastNextGetMoment();
            if (Last == null) return;
            var (inFlow, outFlow, throughFlow) = GetSettedValues(moment);
            if (AutoControl.ControlDifference <= Math.Abs(Chamber.CurrentInFlow - inFlow)
                || AutoControl.ControlDifference <= Math.Abs(Chamber.CurrentOutFlow - outFlow)
                || AutoControl.ControlDifference <= Math.Abs(Chamber.CurrentThroughFlow - throughFlow))
            {
                Chamber.AddToQueue();
            }
        }

        public (int inFlow, int outFlow, int throughFlow) GetCurrentSettedValues()
        {
            var moment = SetLastNextGetMoment();
            return  GetSettedValues(moment);
        }

        TimeSpan SetLastNextGetMoment() {
            var moment = DateTime.UtcNow - StartMoment;
            while (Next != null && Next.Time < moment)
            {
                Last = Next;
                if (ControlEnumerator.MoveNext())
                    Next = ControlEnumerator.Current;
                else
                    Next = null;
            }
            return moment;
        }

        (int inFlow, int outFlow, int throughFlow) GetSettedValues(TimeSpan moment)
        {
            if (Next == null)
                return (Last.InFlow, Last.OutFlow, Last.ThroughFlow);
                
            var p = (moment - Last.Time) / (Next.Time - Last.Time);
            var inFlow = Last.InFlow + p * (Next.InFlow - Last.InFlow);
            var outFlow = Last.OutFlow + p * (Next.OutFlow - Last.OutFlow);
            var throughFlow = Last.ThroughFlow + p * (Next.ThroughFlow - Last.ThroughFlow);
            return ((int)inFlow, (int)outFlow, (int)throughFlow);
        }

        public void Dispose() 
        {
            Timer.Stop();
            Timer.Dispose();
        }
    }
}
