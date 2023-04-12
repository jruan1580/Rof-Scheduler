using ClientManagementService.Domain.Services;
using ClientManagementService.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RofShared.Services;
using RofShared.StartupInits;

namespace ClientManagementService.API
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
            services.AddSingleton<IClientRepository, ClientRepository>();
            services.AddSingleton<IPetRepository, PetRepository>();
            services.AddSingleton<IPetToVaccinesRepository, PetToVaccinesRepository>();

            services.AddTransient<IPasswordService, PasswordService>();
            services.AddTransient<IClientService, ClientService>();
            services.AddTransient<IPetService, PetService>();
            services.AddTransient<IDropdownService, DropdownService>();

            services.AddMvc();

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
