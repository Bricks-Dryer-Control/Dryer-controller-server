using System;
using Dryer_Server.Interfaces;
using Dryer_Server.Persistance;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Dryer_Server.WebApi
{
    public class Startup
    {
        static readonly UiDataKeeper ui = new UiDataKeeper();
        private static Dryer_Server.Core.Main main;
        static readonly SqlitePersistanceManager persistanceManager = new SqlitePersistanceManager();

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            main = new Dryer_Server.Core.Main(ui, TimeSpan.FromSeconds(configuration.GetSection("ApplicationParameters").GetValue<int>("CheckingDelayInSeconds")));
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var ui = new UiDataKeeper();
            services.AddSingleton(typeof(IMainController), main);
            services.AddSingleton(typeof(IUiDataKeeper), ui);
            services.AddSingleton(typeof(IAutoControlPersistance), persistanceManager);
            services.AddMvc().AddNewtonsoftJson();
            services.AddCors((options =>
            {
                options.AddPolicy("NoCors",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                    });
            }));
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "Dryer_Server.WebApi", Version = "v1"});
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var initTask = main.InitializeAsync();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Dryer_Server.WebApi v1"));
            }

            app.UseCors(options => options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

            app.UseRouting();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            initTask.Wait();
            main.Start();
        }
    }
}