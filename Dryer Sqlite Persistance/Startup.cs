using System;
using Dryer_Server.Persistance.Model;
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
            var historicalConnectionStringBuilder = new SqliteConnectionStringBuilder();
            historicalConnectionStringBuilder.DataSource = "./historic.db";
            historicalConnectionStringBuilder.ForeignKeys = false;
            historicalConnectionStringBuilder.Mode = SqliteOpenMode.ReadWriteCreate;
            historicalConnectionStringBuilder.Cache = SqliteCacheMode.Default;
            return historicalConnectionStringBuilder.ToString();
        }

        public static string GetSettingsConnectionString()
        {
            var settingsConectionStringBuilder = new SqliteConnectionStringBuilder();
            settingsConectionStringBuilder.DataSource = "./settings.db";
            settingsConectionStringBuilder.ForeignKeys = true;
            settingsConectionStringBuilder.Mode = SqliteOpenMode.ReadWriteCreate;
            settingsConectionStringBuilder.Cache = SqliteCacheMode.Default;
            return settingsConectionStringBuilder.ToString();
        }
    }
}