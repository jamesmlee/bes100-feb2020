using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
using Microsoft.OpenApi.Models;

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

            // Swashbuckle.AspNetCore
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Books API",
                    Version = "1.0",
                    Contact = new OpenApiContact
                    {
                        Name = "James Lee"
                    },
                    Description = "This is the API for BES 100"
                });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Library API");
                
                // allows just going to localhost:1337 instead of needing a specific url
                c.RoutePrefix = string.Empty;
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
