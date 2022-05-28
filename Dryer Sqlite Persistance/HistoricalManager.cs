using Dryer_Server.Interfaces;
using Dryer_Server.Persistance.Model.Historical;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace Dryer_Server.Persistance
{
    internal partial class HistoricalManager: IDisposable
    {
        private readonly HistorySettings historicalSettings = new HistorySettings();
        private readonly DbContextOptions<HistoricalContext> historicalCtxOptions;

        private readonly Timer windowTimer;

        public HistoricalManager(IConfiguration config)
        {
            config.GetSection("Historical").Bind(historicalSettings);

            var historicalBuilder = new DbContextOptionsBuilder<HistoricalContext>();
            historicalBuilder.UseSqlite(config.GetConnectionString("HistoricalContext"));
            historicalCtxOptions = historicalBuilder.Options;

            if (historicalSettings.LogTimeWindow > TimeSpan.Zero)
            {
                windowTimer = new Timer(historicalSettings.LogTimeWindow.TotalMilliseconds);
                windowTimer.Elapsed += WindowTimer_Elapsed;
                windowTimer.Start();
            }
        }

        internal void Add(ChamberConvertedState status)
        {
            var item = new Item<ChamberConvertedState>(status);
            if (item.last == null)
            {
                Save(item);
                return;
            }

            if (historicalSettings.LogTimeWindow == TimeSpan.Zero
                && (!historicalSettings.LogDifferencesOnly 
                    || item.IsDifferent()))
            {
                if (item.last?.saved == false)
                    Save(item, item.last);
                else
                    Save(item);
            }
        }

        internal void Add(ChamberSensorValue sensor)
        {
            var item = new Item<ChamberSensorValue>(sensor);
            if (item.last == null)
            {
                Save(item);
                return;
            }

            if (historicalSettings.LogTimeWindow == TimeSpan.Zero
                && (!historicalSettings.LogDifferencesOnly
                    || item.IsDifferent()))
            {
                if (item.last?.saved == false)
                    Save(item, item.last);
                else
                    Save(item);
            }
        }

        private void WindowTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var savedStatuses = Item<ChamberConvertedState>.Items;
            var savedSensors = Item<ChamberSensorValue>.Items;

            var dateLimit = historicalSettings.MaxTimeWithoutLog > TimeSpan.Zero
                ? DateTime.UtcNow - historicalSettings.MaxTimeWithoutLog
                : DateTime.MaxValue;

            lock (savedStatuses)
            lock (savedSensors)
            {
                var statuses = savedStatuses.Values
                    .Where(s => !s.saved);
                var sensors = savedSensors.Values
                    .Where(s => !s.saved);

                if (historicalSettings.LogDifferencesOnly)
                {
                    statuses = statuses
                        .Where(s => s.IsDifferent() || s.lastSaved < dateLimit);
                    sensors = sensors
                        .Where(s => s.IsDifferent() || s.lastSaved < dateLimit);
                }

                Save(statuses, sensors);
            }
        }

        private void Save(params Item<ChamberConvertedState>[] statuses)
        {
            try
            {
                using (var ctx = new HistoricalContext(historicalCtxOptions))
                {
                    ctx.States.AddRange(statuses.Select(s => s.value));
                    ctx.SaveChanges();
                }

                foreach (var item in statuses)
                {
                    item.saved = true;
                    item.lastSaved = DateTime.UtcNow;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void Save(params Item<ChamberSensorValue>[] sensorsValues)
        {
            try
            {
                using (var ctx = new HistoricalContext(historicalCtxOptions))
                {
                    ctx.Sensors.AddRange(sensorsValues.Select(s => s.value));
                    ctx.SaveChanges();
                }

                foreach (var item in sensorsValues)
                {
                    item.saved = true;
                    item.lastSaved = DateTime.UtcNow;
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
            }
        }

        private void Save(IEnumerable<Item<ChamberConvertedState>> statuses, IEnumerable<Item<ChamberSensorValue>> sensorsValues)
        {
            try
            {
                using (var ctx = new HistoricalContext(historicalCtxOptions))
                {
                    ctx.States.AddRange(statuses.Select(s => s.value));
                    ctx.Sensors.AddRange(sensorsValues.Select(s => s.value));
                    ctx.SaveChanges();
                }

                foreach (var item in statuses)
                {
                    item.saved = true;
                    item.lastSaved = DateTime.UtcNow;
                }
                foreach (var item in sensorsValues)
                {
                    item.saved = true;
                    item.lastSaved = DateTime.UtcNow;
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
            }
        }

        public void Dispose()
        {
            if (windowTimer != null)
            {
                windowTimer.Stop();
                windowTimer.Dispose();
                OnShoutdown();
            }
        }

        private void OnShoutdown()
        {
            var savedStatuses = Item<ChamberConvertedState>.Items;
            var savedSensors = Item<ChamberSensorValue>.Items;

            lock (savedStatuses)
            lock (savedSensors)
            {
                var statuses = savedStatuses.Values
                    .Where(s => !s.saved);
                var sensors = savedSensors.Values
                    .Where(s => !s.saved);

                Save(statuses, sensors);
            }
        }
    }
}