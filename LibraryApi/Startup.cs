using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryApi.Domain;
using LibraryApi.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LibraryApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // AddTransient, AddScoped, AddSingleton
            services.AddTransient<IGenerateEmployeeIds, EmployeeIdGenerator>();
            services.AddControllers();
            services.AddDbContext<LibraryDataContext>(options =>
            // do NOT hardcode connection strings like this ... just for example
                //options.UseSqlServer(@"server=.\sqlexpress;database=library;integrated security=true")
                options.UseSqlServer(Configuration.GetConnectionString("LibraryDatabase"))
            // move to appSettings.json, or if running in Docker, as an environment variable where 
            // name: ConnectionStrings__LibraryDatabase ... value: server=.\sqlexpress;database=library;integrated security=true
            );
            // in Package Manager Console: add-migration initial
            // in Package Manager Console: update-database
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
