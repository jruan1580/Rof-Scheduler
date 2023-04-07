using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PetServiceManagement.Domain.BusinessLogic;
using PetServiceManagement.Domain.Models;
using PetServiceManagement.Infrastructure.Persistence.Repositories;
using RofShared.StartupInits;

namespace PetServiceManagement.API
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
            services.AddSingleton<IPetServiceRetrievalRepository, PetServiceRetrievalRepository>();
            services.AddSingleton<IPetServiceUpsertRepository, PetServiceUpsertRepository>();
            services.AddSingleton<IHolidayRetrievalRepository, HolidayRetrievalRepository>();
            services.AddSingleton<IHolidayUpsertRepository, HolidayUpsertRepository>();
            services.AddSingleton<IHolidayRateRetrievalRepository, HolidayRateRetrievalRepository>();
            services.AddSingleton<IHolidayRateUpsertRepository, HolidayRateUpsertRepository>();

            services.AddTransient<IDropdownService<PetService>, PetServiceDropdownService>();
            services.AddTransient<IDropdownService<Holiday>, HolidayDropdownService>();
            services.AddTransient<IPetServiceManagementService, PetServiceManagementService>();
            services.AddTransient<IHolidayService, HolidayService>();
            services.AddTransient<IHolidayRateService, HolidayRateService>();            

            services.AddControllers();
            services.AddJwtAuthentication(Configuration);           
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.AddExceptionHandlerForApi();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.AddCorsForOriginAndAllowAnyMethodAndHeader("http://localhost:3000");            

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
