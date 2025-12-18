using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace ClassifiedAds.Common.Exceptions
{
    public class DbUpdateExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is DbUpdateException dbEx
                && dbEx.InnerException is PostgresException pg
                && pg.SqlState == "23505")
            {
                context.Result = new ConflictObjectResult(new { error = "That record already exists." });
                context.ExceptionHandled = true;
            }
        }
    }

}