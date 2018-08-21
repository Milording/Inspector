using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;

namespace Inspector
{
    public class JsonExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        
        public JsonExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next.Invoke(httpContext);
            }
            catch (Exception ex)
            {
                var result = new JsonResult(new {ex.Message}) { StatusCode = (int?) HttpStatusCode.BadRequest};
                
                var routeData = httpContext.GetRouteData();
                var actionDescriptor = new ActionDescriptor();
                var actionContext = new ActionContext(httpContext, routeData, actionDescriptor);
                await result.ExecuteResultAsync(actionContext);
            }
        }
    }

    public static class JsonExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseJsonException(this IApplicationBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            return builder.UseMiddleware<JsonExceptionMiddleware>();
        }
    }
}
