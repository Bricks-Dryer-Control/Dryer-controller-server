using System;
using System.Collections.Generic;
using Dryer_Server.Interfaces;
using Dryer_Server.Persistance.Model.Historical;
using Dryer_Server.Persistance.Model.Settings;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Dryer_Server.Persistance
{
    public class SqlitePersistanceManager : IDryerHisctoricalValuesPersistance, IDryerConfigurationPersistance
    {
        DbContextOptions<SettingsContext> settingsCtxOptions;
        DbContextOptions<HistoricalContext> historicalCtxOptions;

        HistoricalContext GetHistoricalCtx() => new HistoricalContext(historicalCtxOptions);
        SettingsContext GetSettingsCtx() => new SettingsContext(settingsCtxOptions);

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

        public void Dispose()
        {
            
        }
    }
}