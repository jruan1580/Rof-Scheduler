using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.Tasks;

namespace RofShared.StartupInits
{
    public static class AuthenticationMiddleware
    {
        public static void AddJwtAuthentication(this IServiceCollection service, IConfiguration configuration)
       {
            service.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration.GetSection("Jwt:Issuer").Value,
                        ValidAudience = configuration.GetSection("Jwt:Audience").Value,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("Jwt:Key").Value))
                    };
                    options.Events = new JwtBearerEvents()
                    {
                        OnMessageReceived = context =>
                        {
                            //no auth header, get it out of cookie, otw, it is in auth header
                            if (!context.Request.Headers.ContainsKey("Authorization"))
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
       }

        public static void AddCorsForOriginAndAllowAnyMethodAndHeader(this IApplicationBuilder app, string origin)
        {
            app.UseCors(x => x
               .WithOrigins(origin)
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials()); // allow credentials            
        }
    }
}