using EmployeeManagementService.API.Authentication;
using EmployeeManagementService.Domain.Services;
using EmployeeManagementService.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagementService.API
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
            services.AddSingleton<IEmployeeRepository, EmployeeRepository>();
            services.AddTransient<IEmployeeService, EmployeeService>();
            services.AddTransient<IPasswordService, PasswordService>();
            services.AddTransient<ITokenHandler, Authentication.TokenHandler>();

            services.AddMvc();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer(options =>
               {
                   options.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateIssuer = true,
                       ValidateAudience = true,
                       ValidateLifetime = true,
                       ValidateIssuerSigningKey = true,
                       ValidIssuer = Configuration.GetSection("Jwt:Issuer").Value,
                       ValidAudience = Configuration.GetSection("Jwt:Audience").Value,
                       IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetSection("Jwt:Key").Value))
                   };
                   options.Events = new JwtBearerEvents()
                   {
                       OnMessageReceived = context =>
                       {
                           //it is postman, token is in headers.
                           if (context.Request.Headers.TryGetValue("User-Agent", out var agent) && !agent.ToString().Contains("Postman"))
                           {
                               if (context.Request.Cookies.ContainsKey("X-Access-Token-Admin"))
                               {
                                   context.Token = context.Request.Cookies["X-Access-Token-Admin"];
                               }

                               if (context.Request.Cookies.ContainsKey("X-Access-Token-Employee"))
                               {
                                   context.Token = context.Request.Cookies["X-Access-Token-Employee"];
                               }
                           }                     

                           return Task.CompletedTask;
                       }
                   };                 
               });

            services.AddControllers();
            services.AddCors();
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

            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
