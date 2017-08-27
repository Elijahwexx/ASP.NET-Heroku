using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace backend
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSwagger();

            // Use SQLite database
            services.AddDbContext<AppDbContext>(options => 
                options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));

            services.AddTransient<IItemRepository, ItemRepository>();
            // Add MVC services to the services container (only the necessary services)
            services.AddMvcCore()
                // Add ApiExplorer (needed by Swagger)
                .AddApiExplorer()
                // Add Authorization handler for MVC actions
                .AddAuthorization()
                // Add and configure JSON formatters
                .AddJsonFormatters(options =>
                {
                    // Use CamelCase because it is easier with Java based clients e.g. JavaScript
                    options.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
                    // Configure time correctly so that other clients will be comfortable. UTC is most common/standard
                    options.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;
                    options.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat;
                    // Ignore null values to reduce the data sent after serialization
                    options.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                    // Do not indent content to reduce data usage
                    options.Formatting = Newtonsoft.Json.Formatting.None;
                });
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, AppDbContext context)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            // Add swagger schema docs
            app.UseSwagger(options => options.PreSerializeFilters.Add((swaggerDoc, httpRequest) => swaggerDoc.Host = httpRequest.Host.Value));

            // Add API documentation UI via ReDoc
            app.UseReDoc(path: "api/docs", title: "API Test 1.0");
            
            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            // Seed the database
            DatabaseInitializer.Initialize(context);
        }
    }
}