using EventManagementService.Domain.Services;
using EventManagementService.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.Tasks;

namespace EventManagementService.API
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
            services.AddSingleton<IEventRetrievalRepository, EventRetrievalRepository>();
            services.AddSingleton<IEventUpsertRepository, EventUpsertRepository>();

            services.AddTransient<IEventRetrievalRepository, EventRetrievalRepository>();
            services.AddTransient<IEventUpsertService, EventUpsertService>();

            services.AddControllers();

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
                        //no auth header, get it out of cookie, otw, it is in auth header
                        if (!context.Request.Headers.ContainsKey("Authorization"))
                        {
                            if (context.Request.Cookies.ContainsKey("X-Access-Token-Client"))
                            {
                                context.Token = context.Request.Cookies["X-Access-Token-Client"];
                            }

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

            app.UseCors(x => x
                .WithOrigins("http://localhost:3000", "https://localhost:3000")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
