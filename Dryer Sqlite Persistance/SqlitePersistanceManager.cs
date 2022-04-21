using Dryer_Server.Interfaces;
using Dryer_Server.Persistance.Model.AutoControl;
using Dryer_Server.Persistance.Model.Historical;
using Dryer_Server.Persistance.Model.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dryer_Server.Persistance
{
    public class SqlitePersistanceManager : IDryerHisctoricalValuesPersistance, 
        IDryerConfigurationPersistance, IAutoControlPersistance
    {
        readonly DbContextOptions<SettingsContext> settingsCtxOptions;
        readonly DbContextOptions<HistoricalContext> historicalCtxOptions;
        readonly DbContextOptions<AutoControlContext> autocontrolCtxOptions;

        HistoricalContext GetHistoricalCtx() => new HistoricalContext(historicalCtxOptions);
        SettingsContext GetSettingsCtx() => new SettingsContext(settingsCtxOptions);
        AutoControlContext GetAutoControlCtx() => new AutoControlContext(autocontrolCtxOptions);

        public SqlitePersistanceManager(IConfiguration config)
        {
            var settingsBuilder = new DbContextOptionsBuilder<SettingsContext>();
            settingsBuilder.UseSqlite(config.GetConnectionString("SettingsContext"));
            settingsCtxOptions = settingsBuilder.Options;

            var historicalBuilder = new DbContextOptionsBuilder<HistoricalContext>();
            historicalBuilder.UseSqlite(config.GetConnectionString("HistoricalContext"));
            historicalCtxOptions = historicalBuilder.Options;

            var autocontrolBuilder = new DbContextOptionsBuilder<AutoControlContext>();
            autocontrolBuilder.UseSqlite(config.GetConnectionString("AutoControlContext"));
            autocontrolCtxOptions = autocontrolBuilder.Options;
        }

        public SqlitePersistanceManager(DbContextOptions<AutoControlContext> autoControlContextOptions,
            DbContextOptions<HistoricalContext> historicalContextOptions,
            DbContextOptions<SettingsContext> settingsContextOptions)
        {
            settingsCtxOptions = settingsContextOptions;
            historicalCtxOptions = historicalContextOptions;
            autocontrolCtxOptions = autoControlContextOptions;
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

        IEnumerable<AutoControl> IAutoControlPersistance.GetControls()
        {
            using var ctx = GetAutoControlCtx();
            return ctx.Definitions
                  .Where(d => !d.Deleted)
                  .Select(d => d.ToAutoControl())
                  .ToList();
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

        public void SaveState(int chamberId, IAutoControl autoControl)
        {
            using var ctx = GetAutoControlCtx();
            var dbAutoControl = ctx.Definitions
                .Where(d => !d.Deleted && d.Name == autoControl.Name)
                .OrderByDescending(d => d.Id)
                .First();
            var newState = new DbAutoControlState(chamberId, autoControl, dbAutoControl);
            ctx.Add(newState);
            ctx.SaveChanges();
        }

        public IEnumerable<(int chamberId, DateTime startUtc, AutoControl autoControl)> LoadStates()
        {
            using var ctx = GetAutoControlCtx();

            var ids = ctx.States
                .Select(d => d.ChamberId)
                .Distinct()
                .ToList();

            foreach (var id in ids)
            {
                var ac = ctx.States
                    .Include(s => s.AutoControl)
                    .Include(s => s.AutoControl.Sets)
                    .Where(s => s.ChamberId == id)
                    .OrderByDescending(s => s.Id)
                    .First();
                yield return (id, ac.StartUtc, ac.AutoControl.ToAutoControl());
            }
        }
    }
}