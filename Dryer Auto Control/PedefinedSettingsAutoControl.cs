using Dryer_Server.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace Dryer_Server.Dryer_Auto_Control
{
    internal class PedefinedSettingsAutoControl : AutoControl
    {
        TimeSpan lastSet = TimeSpan.MinValue;
        bool finished = false;
        Timer timer = new Timer
        {
            AutoReset = true,
            Interval = 10e3,
            Enabled = false,
        };

        private AutoControlItem last;
        private bool active = false;
        private readonly IEnumerator<AutoControlItem> enumerator;

        public bool IsOnQueue { get; private set; } = false;

        public override bool Active => active;

        public override bool ShouldSend => ShouldSendToController();

        public PedefinedSettingsAutoControl(Interfaces.AutoControl autoControlData, DateTime startUtc, IAutoControlledChamber chamber) : base(autoControlData, startUtc, chamber)
        {
            timer.Elapsed += Timer_Elapsed;
            enumerator = autoControlData.Sets.GetEnumerator();
            enumerator.MoveNext();
            last = enumerator.Current;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Execute();
        }

        public override void Start()
        {
            active = true;
            Execute();
            timer.Start();
            chamber.AutoControlChanged();
        }

        public override void Stop()
        {
            active = false;
            timer.Stop();
            chamber.AutoControlChanged();
        }

        public override void Dispose()
        {
            timer.Dispose();
        }

        private void Execute()
        {
            if (finished)
            {
                timer.Stop();
                return;
            }

            var now = DateTime.UtcNow - StartDateUtc;
            if (now < TimeSpan.Zero || now < lastSet + autoControlData.TimeToSet || IsOnQueue)
                return;

            GetCurrentEstimates(now, out var inFlow, out var outFlow, out var throughFlow);

            if (CheckSetValues(inFlow, outFlow, throughFlow))
                PutOnQueue();
        }

        private void GetCurrentEstimates(TimeSpan now, out int inFlow, out int outFlow, out int throughFlow)
        {
            while (!finished && now > enumerator.Current.Time)
            {
                last = enumerator.Current;
                if (!enumerator.MoveNext())
                {
                    finished = true;
                    timer.Stop();
                }
            }

            if (finished)
            {
                last = autoControlData.Sets.Last();
                inFlow = last.InFlow;
                outFlow = last.OutFlow;
                throughFlow = last.ThroughFlow;
                return;
            }

            var next = enumerator.Current;
            var numerator = now - last.Time;
            var denumerator = next.Time - last.Time;
            var proportion = numerator / denumerator;

            inFlow = GetEstimate(last, next, x => x.InFlow, proportion);
            outFlow = GetEstimate(last, next, x => x.OutFlow, proportion);
            throughFlow = GetEstimate(last, next, x => x.ThroughFlow, proportion);
        }

        private int GetEstimate(AutoControlItem last, AutoControlItem next, Func<AutoControlItem, int> field, double proportion)
        {
            return field(last) + (int)(proportion * (field(next) - field(last)));
        }

        private bool CheckSetValues(int inFlow, int outFlow, int throughFlow)
        {
            var d = autoControlData.ControlDifference;
            var status = chamber.ConvertedStatus;

            if (inFlow < autoControlData.MinInFlow)
                inFlow = autoControlData.MinInFlow;
            else if (inFlow > autoControlData.MaxInFlow)
                inFlow = autoControlData.MaxInFlow;
            if (inFlow + d < status.InFlowPosition || inFlow - d > status.InFlowPosition)
                return true;
            
            if (outFlow < autoControlData.MinOutFlow)
                outFlow = autoControlData.MinOutFlow;
            else if (outFlow > autoControlData.MaxOutFlow)
                outFlow = autoControlData.MaxOutFlow;
            if (outFlow + d < status.OutFlowPosition || outFlow - d > status.OutFlowPosition)
                return true;

            if (throughFlow + d < status.ThroughFlowPosition || throughFlow - d > status.ThroughFlowPosition)
                return true;

            return false;
        }

        private void PutOnQueue()
        {
            IsOnQueue = true;
            chamber.EnqueueAutoControl(this);
        }

        private bool ShouldSendToController()
        {
            IsOnQueue = false;
            return Active;
        }

        public override int[] GetValues()
        {
            var now = DateTime.UtcNow - StartDateUtc;
            GetCurrentEstimates(now, out var inFlow, out var outFlow, out var throughFlow);
            return chamber.SetValuesGetActuators(inFlow, outFlow, throughFlow);
        }
    }
}