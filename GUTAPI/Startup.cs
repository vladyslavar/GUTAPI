using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GUTAPI.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Viber.Bot;
using System.IO;
using Viber.Bot.NetCore.Middleware;

namespace GUTAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            using (var client = new CustomDBContext())
            {
                client.Database.EnsureCreated();
            }
        }

        public IConfiguration Configuration { get; }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string connection = Configuration.GetConnectionString("DefaultConnection");
            services.AddSingleton(typeof(CustomDBContext));
            services.AddControllers();

            services.AddViberBotApi(opt =>
            {
                opt.Token = "4d3ce6e62427d2ce-74fcf7f7176d0e49-3bb15a44e9781f70";
                opt.Webhook = "https://gutapi.ml/api/viberhook/hook";
            });
        }
        /*
        public void Init()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            _authToken = config["4d3ce6e62427d2ce-74fcf7f7176d0e49-3bb15a44e9781f70"];
            _webhookUrl = config["https://gutapi.ml/api/viber/Hook"];
        }
        */
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
