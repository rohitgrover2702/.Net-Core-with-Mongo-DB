using Kobe.Common.ViewModels;
using Kobe.Data.Repository;
using Kobe.Domain.Collections;
using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Threading.Tasks;
using static Kobe.Common.Enums;

namespace Kobe.Common.Utilities
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMongoDBRepository<Error> _errorRepository;        

        public ExceptionMiddleware(RequestDelegate next, IMongoDBRepository<Error> errorRepository)
        {            
            _next = next;
            _errorRepository = errorRepository;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {                
                Error error = new Error();
                error.Message = ex.Message;
                error.StackTrace = ex.StackTrace;
                error.UserId = httpContext.User.Identity.Name;
                await _errorRepository.Add(error);
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            return context.Response.WriteAsync(new ResponseViewModel()
            {                
                Status = (int)Numbers.Zero,
                Message = Constants.Error
            }.ToString());
        }
    }
}
