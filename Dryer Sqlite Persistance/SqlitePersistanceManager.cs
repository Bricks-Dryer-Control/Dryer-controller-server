using Dryer_Server.Interfaces;
using Dryer_Server.Persistance.Model.AutoControl;
using Dryer_Server.Persistance.Model.Historical;
using Dryer_Server.Persistance.Model.Settings;
using Dryer_Sqlite.Persistance.Model.AutoControl;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dryer_Server.Persistance
{
    public class SqlitePersistanceManager : IDryerHisctoricalValuesPersistance, IDryerConfigurationPersistance,
        IAutoControlPersistance, ITimeBasedAutoControlPersistance
    {
        readonly DbContextOptions<SettingsContext> settingsCtxOptions;
        readonly DbContextOptions<HistoricalContext> historicalCtxOptions;
        readonly DbContextOptions<AutoControlContext> autocontrolCtxOptions;
        private readonly DbContextOptions<TimeBasedAutoControlContext> timeBasedAutoControlOptions;

        HistoricalContext GetHistoricalCtx() => new HistoricalContext(historicalCtxOptions);
        SettingsContext GetSettingsCtx() => new SettingsContext(settingsCtxOptions);
        AutoControlContext GetAutoControlCtx() => new AutoControlContext(autocontrolCtxOptions);
        TimeBasedAutoControlContext GetTimeBasedAutoControlContext() => new TimeBasedAutoControlContext(timeBasedAutoControlOptions);

        public SqlitePersistanceManager()
        {
            var settingsBuilder = new DbContextOptionsBuilder<SettingsContext>();
            var settingsConnectionString = Startup.GetSettingsConnectionString();
            settingsBuilder.UseSqlite(settingsConnectionString);
            settingsCtxOptions = settingsBuilder.Options;

            var historicalBuilder = new DbContextOptionsBuilder<HistoricalContext>();
            var historicalConnectionString = Startup.GetHistoricalConnectionString();
            historicalBuilder.UseSqlite(historicalConnectionString);
            historicalCtxOptions = historicalBuilder.Options;

            var autocontrolBuilder = new DbContextOptionsBuilder<AutoControlContext>();
            var autocontrolConnectionString = Startup.GetAutoControlConnectionString();
            autocontrolBuilder.UseSqlite(autocontrolConnectionString);
            autocontrolCtxOptions = autocontrolBuilder.Options;

            var timeBasedAutoControlBuilder= new DbContextOptionsBuilder<TimeBasedAutoControlContext>();
            var timeBasedAutocontrolConnectionString = Startup.GetAutoControlConnectionString();
            timeBasedAutoControlBuilder.UseSqlite(timeBasedAutocontrolConnectionString);
            timeBasedAutoControlOptions = timeBasedAutoControlBuilder.Options;

        }

        public SqlitePersistanceManager(DbContextOptions<AutoControlContext> autoControlContextOptions,
            DbContextOptions<HistoricalContext> historicalContextOptions,
            DbContextOptions<SettingsContext> settingsContextOptions,
            DbContextOptions<TimeBasedAutoControlContext> timeBasedAutoControlContextOptions
            )
        {
            settingsCtxOptions = settingsContextOptions;
            historicalCtxOptions = historicalContextOptions;
            autocontrolCtxOptions = autoControlContextOptions;
            timeBasedAutoControlOptions = timeBasedAutoControlContextOptions;
        }

        public IEnumerable<IChamberSensorHistoricValue> GetSensorsHistory(int id, DateTime startUtc, DateTime finishUtc)
        {
            using var ctx = GetHistoricalCtx();
            
            return ctx.Sensors
                .Where(s => s.ChamberId == id)
                .Where(s => s.TimestampUtc >= startUtc && s.TimestampUtc < finishUtc)
                .OrderBy(s => s.TimestampUtc)
                .Select(s => (IChamberSensorHistoricValue)s)
                .ToList();
        }

        public IEnumerable<IChamberStatusHistoricValue> GetStatusHistory(int id, DateTime startUtc, DateTime finishUtc)
        {
            using var ctx = GetHistoricalCtx();
            
            return ctx.States
                .Where(s => s.ChamberId == id)
                .Where(s => s.TimestampUtc >= startUtc && s.TimestampUtc < finishUtc)
                .OrderBy(s => s.TimestampUtc)
                .Select(s => (IChamberStatusHistoricValue)s)
                .ToList();
        }

        public void Save(int id, ChamberConvertedStatus status)
        {
            using var ctx = GetHistoricalCtx();
            
            ctx.States.Add(new ChamberConvertedState(status, id));
        }

        public void Save(int id, ChamberSensors sensors)
        {
            using var ctx = GetHistoricalCtx();
            
            ctx.Sensors.Add(new ChamberSensorValue(sensors, id));
        }

        public void Save(int id, ChamberConfiguration configuration)
        {
            using var ctx = GetSettingsCtx();

            ctx.Chamber.Add(new ChamberSetting(configuration));
        }

        public IEnumerable<ChamberConfiguration> GetChamberConfigurations()
        {
            using var ctx = GetSettingsCtx();
            
            var ids = ctx.Chamber.Select(c => c.Id)
                .Distinct()
                .ToList();
            
            foreach (var id in ids)
                yield return ctx.Chamber
                    .Where(c => c.Id == id)
                    .OrderByDescending(c => c.CreationTimeUtc)
                    .FirstOrDefault();
        }

        public IEnumerable<(int id, ChamberConvertedStatus status, ChamberSensors sensors)> GetLastValues(IEnumerable<int> ids)
        {
            using var ctx = GetHistoricalCtx();
            foreach (var id in ids)
            {
                ChamberConvertedStatus status = ctx.States
                    .Where(s => s.ChamberId == id)
                    .OrderByDescending(s => s.TimestampUtc)
                    .FirstOrDefault();
                ChamberSensors sensors = ctx.Sensors
                    .Where(s => s.ChamberId == id)
                    .OrderByDescending(s => s.TimestampUtc)
                    .FirstOrDefault();
                yield return (id, status, sensors);
            }
        }

        public IEnumerable<string> GetAutoControls()
        {
            using var ctx = GetAutoControlCtx();
            return ctx.Definitions
                .Where(d => !d.Deleted)
                .Select(d => d.Name)
                .Distinct();
        }

        void IAutoControlPersistance.SaveDeactivateLatest(AutoControl autoControl)
        {
            Deactivate(autoControl.Name);
            SaveAutoControl(autoControl);
        }
        public void SaveAutoControl(AutoControl autoControl)
        {
            using var ctx = GetAutoControlCtx();
            ctx.Definitions.Add(new DbAutoControl(autoControl));
            ctx.SaveChanges();
        }

        AutoControl IAutoControlPersistance.Load(string name) => LoadAutoControl(name);
        public AutoControl LoadAutoControl(string name)
        {
            using var ctx = GetAutoControlCtx();
            return ctx.Definitions
                .Include(d => d.Sets)
                .Where(d => !d.Deleted && d.Name == name)
                .OrderByDescending(d => d.Id)
                .First()
                .ToAutoControl();
        }

        public void Dispose()
        {
            
        }

        AutoControl IAutoControlPersistance.GetControlWithItems(string name)
        {
            using var ctx = GetAutoControlCtx();
            return ctx.Definitions.Include(d => d.Sets)
                .Where(d => !d.Deleted && d.Name == name)
                .Select(d => d.ToAutoControl()).Single();
        }

        IEnumerable<AutoControl> IAutoControlPersistance.GetControls()
        {
            using var ctx = GetAutoControlCtx();
            return ctx.Definitions
                  .Where(d => !d.Deleted)
                  .OrderByDescending(d => d.Id)
                  .Select(d => d.ToAutoControl()).ToList();
        }

        void IAutoControlPersistance.Delete(string name)
        {
            Deactivate(name);
        }

        private void Deactivate(string name)
        {
            using var ctx = GetAutoControlCtx();
            foreach (var autoControl in ctx.Definitions.Where(d => !d.Deleted && d.Name == name))
                autoControl.Deleted = true;
            ctx.SaveChanges();
        }

        void ITimeBasedAutoControlPersistance.Save(ITimeBasedAutoControl timeBasedAutoControl)
        {
            using var ctx = GetTimeBasedAutoControlContext();
            ctx.Add(new DbTimeBasedAutoControl(timeBasedAutoControl));
            ctx.SaveChanges();
        }


        ITimeBasedAutoControl ITimeBasedAutoControlPersistance.LoadTimeBasedForChamber(IAutoControlledChamber chamber, TimeSpan checkingDelay)
        {
            using var timeBasedAutoControlContext = GetTimeBasedAutoControlContext();
            var timeBasedAutoControlDb= timeBasedAutoControlContext.TimeBasedAutoControls.SingleOrDefault(t => t.ChamberId == chamber.Id);
            if (timeBasedAutoControlDb == null) return null;
            return new TimeBasedAutoControl(checkingDelay, timeBasedAutoControlDb.StartMoment, timeBasedAutoControlDb.AutoControl.ToAutoControl(), chamber);
        }
    }
}