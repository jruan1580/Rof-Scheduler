using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using RofShared.Exceptions;
using System;
using System.Net;

namespace RofShared.StartupInits
{
    public static class ExceptionHandlerMiddleware
    {
        public static void AddExceptionHandlerForApi(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    var exception = context.Features.Get<IExceptionHandlerFeature>().Error;

                    if (exception is EntityNotFoundException)
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    }
                    else if (exception is ArgumentException)
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    }
                    else
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    }

                    await context.Response.WriteAsync(exception.Message);
                });
            });
        }
    }
}
