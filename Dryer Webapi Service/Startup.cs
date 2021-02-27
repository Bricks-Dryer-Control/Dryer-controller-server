using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dryer_Server.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace Dryer_Server.WebApi
{
    public class Startup
    {
        static readonly UiDataKeeper ui = new UiDataKeeper();
        static readonly Dryer_Server.Core.Main main = new Dryer_Server.Core.Main(ui);

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var ui = new UiDataKeeper();
            services.AddSingleton(typeof(IMainController), main);
            services.AddSingleton(typeof(IUiDataKeeper), ui);

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
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Dryer_Server.WebApi", Version = "v1" });
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            initTask.Wait();
            main.Start();
        }
    }
}
