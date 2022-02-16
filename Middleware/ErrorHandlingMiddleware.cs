using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RestaurantApi.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantApi.Middleware
{
    public class ErrorHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger)
        {
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next.Invoke(context);
            }
            catch(ForbidException e)
            {
                context.Response.StatusCode = 403;
            }
            catch(BadRequestEception e)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync(e.Message);
            }
            catch (NotFoundException e)
            {
                context.Response.StatusCode = 404;

                await context.Response.WriteAsync(e.Message); //write async jakby tu wysylamy chyba response do klienta
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);

                context.Response.StatusCode = 500; //kontekst http, dostp do response i inncyh wlasicwosci http
                await context.Response.WriteAsync("Something went wrong");
            }
        }
    }
}
