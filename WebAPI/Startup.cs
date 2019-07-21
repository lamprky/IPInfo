using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebAPI.Data;
using WebAPI.Repositories;
using WebAPI.Repositories.Interfaces;
using WebAPI.Services;
using WebAPI.Services.Interfaces;

namespace WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public static string ConnectionString { get; private set; }
        public static int CacheExpirationMins { get; private set; }
        public static int BatchRecords { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            GetLocalSetting();

            services.AddDbContext<ApplicationDbContext>(options =>
               options.UseSqlServer(ConnectionString, b => b.MigrationsAssembly("WebAPI")));

            services.AddMemoryCache();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            RegisterServices(services);
            RegisterRepositories(services);
        }

        private void GetLocalSetting()
        {
            ConnectionString = Configuration.GetConnectionString("DefaultConnection");

            var localSetting = Configuration.GetSection("ApplicationSettings");
            CacheExpirationMins = int.Parse(localSetting.GetSection("CacheExpirationMins").Value);
            BatchRecords = int.Parse(localSetting.GetSection("BatchRecords").Value);
        }

        private static void RegisterServices(IServiceCollection services)
        {
            var servicesRegistrations =
                (from type in typeof(IPInfoService).Assembly.GetExportedTypes()
                 where type.Namespace == typeof(IPInfoService).Namespace && type.Name.EndsWith("Service")
                 select new
                 {
                     Service = type.GetInterfaces().Single(),
                     Implementation = type
                 }).ToList();

            foreach (var reg in servicesRegistrations)
            {
                services.AddTransient(reg.Service, reg.Implementation);
            }
        }

        private static void RegisterRepositories(IServiceCollection services)
        {
            var repositoriesRegistrations =
                           (from type in typeof(IPDetailsRepository).Assembly.GetExportedTypes()
                            where (type.Namespace == typeof(IPDetailsRepository).Namespace) &&
                            type.Name.EndsWith("Repository") &&
                            type != typeof(GenericRepository<>) &&
                            type.GetInterfaces().Where(
                                i => i.Name.EndsWith("Repository") &&
                                i != typeof(IGenericRepository<>)
                                ).SingleOrDefault() != null
                            select new
                            {
                                Service = type.GetInterfaces().Where(
                                    i => i.Name.EndsWith("Repository") &&
                                    i != typeof(IGenericRepository<>)
                                    ).SingleOrDefault(),
                                Implementation = type
                            }).ToList();

            foreach (var reg in repositoriesRegistrations)
            {
                services.AddTransient(reg.Service, reg.Implementation);
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
