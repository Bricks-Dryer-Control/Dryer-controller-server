using System;
using Dryer_Server.Persistance.Model.Historical;
using Dryer_Server.Persistance.Model.Settings;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Dryer_Server.Persistance
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<HistoricalContext>(
                options => options.UseSqlite(GetHistoricalConnectionString())
            );

            services.AddDbContext<SettingsContext>(
                options => options.UseSqlite(GetSettingsConnectionString())
            );
        }

        public static string GetHistoricalConnectionString()
        {
            var historicalConnectionStringBuilder = new SqliteConnectionStringBuilder
            {
                DataSource = "./historic.db",
                ForeignKeys = true,
                Mode = SqliteOpenMode.ReadWriteCreate,
                Cache = SqliteCacheMode.Default
            };
            return historicalConnectionStringBuilder.ToString();
        }

        public static string GetSettingsConnectionString()
        {
            var settingsConectionStringBuilder = new SqliteConnectionStringBuilder
            {
                DataSource = "./settings.db",
                ForeignKeys = true,
                Mode = SqliteOpenMode.ReadWriteCreate,
                Cache = SqliteCacheMode.Default
            };
            return settingsConectionStringBuilder.ToString();
        }
    }
}