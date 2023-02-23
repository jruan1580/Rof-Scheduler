using AuthenticationService.Domain.Services;
using AuthenticationService.Infrastructure.ClientManagement;
using AuthenticationService.Infrastructure.EmployeeManagement;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RofShared.StartupInits;

namespace AuthenticationService.API
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
            services.AddHttpClient();

            services.AddTransient<ITokenHandler, TokenHandler>();
            services.AddTransient<IEmployeeManagementAccessor, EmployeeManagementAccessor>();
            services.AddTransient<IClientManagementAccessor, ClientManagementAccessor>();
            services.AddTransient<IAuthService, AuthService>();

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
