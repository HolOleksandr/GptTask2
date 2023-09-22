using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using BookStore.BLL.Exceptions;

namespace BookStore.API.Configurations
{
    public class ApiExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            if (context.Exception is BookStoreNotFoundException)
            {
                context.HttpContext.Response.StatusCode = 404;
                context.Result = new JsonResult(context.Exception.Message);
            }
            else
            {
                context.HttpContext.Response.StatusCode = 500;
                context.Result = new JsonResult("An unexpected error occurred.");
            }
        }
    }
}
