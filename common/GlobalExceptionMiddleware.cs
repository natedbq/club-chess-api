using chess.api.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace chess.api.common
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleException(ex, httpContext);
            }
        }

        private async Task HandleException(Exception ex, HttpContext httpContext)
        {


            if (ex is InvalidOperationException)
            {
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                httpContext.Response.StatusCode = 400; //HTTP status code
                                                        //httpContext.Response.WriteAsync("Invalid operation");
                                                        //httpContext.Response.WriteAsync("Invalid operation");             
                await httpContext.Response.WriteAsJsonAsync(new ResponseModel
                {
                    Message = "Invalid operation",
                    StatusCode = 400,
                    Success = false
                });
            }
            else if (ex is ArgumentException)
            {
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                await httpContext.Response.WriteAsJsonAsync(new ResponseModel
                {
                    Message = ex.Message,
                    StatusCode = 400,
                    Success = false
                });
            }
            else if(ex is BusinessRuleException)
            {
                httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
                await httpContext.Response.WriteAsJsonAsync(new ResponseModel
                {
                    Message = ex.Message,
                    StatusCode = 403,
                    Success = false
                });
            }
            else
            {
                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await httpContext.Response.WriteAsJsonAsync(new ResponseModel
                {
                    Message = ex.Message,
                    StatusCode = 500,
                    Success = false
                });
            }


        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ExceptionHandleMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandleMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GlobalExceptionMiddleware>();
        }
    }
    public class ResponseModel
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }
    }
}
