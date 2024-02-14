using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Text;
using WebApi.Data;
using WebApi.Models.Exceptions;

namespace WebApi.Middlewares
{
    public class ExceptionHandlerMiddleware : IMiddleware
    {
        private readonly DataContext _dbContext;

        public ExceptionHandlerMiddleware(DataContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (SecureException ex)
            {
                await HandleExceptionAsync(context, ex, "Secure");
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex, "Exception");
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception, string exceptionType)
        {
            var journalEntry = new ExceptionJournal
            {
                EventId = Guid.NewGuid(),
                CreatedAt = DateTime.Now,
                StackTrace = exception.StackTrace,
                QueryParameters = context.Request.Query.ToString(),
                BodyParameters = string.Empty,
            };

            StringBuilder stringBuilder = new StringBuilder();
            foreach(var item in context.Request.Query)
            {
                stringBuilder.Append(item);
            }
            journalEntry.QueryParameters = stringBuilder.ToString();

            using (var streamReader = new StreamReader(context.Request.Body))
            {
                journalEntry.BodyParameters = await streamReader.ReadToEndAsync();
            }
            
            await _dbContext.ExceptionJournals.AddAsync(journalEntry);
            await _dbContext.SaveChangesAsync();

            var response = new
            {
                type = exceptionType,
                id = journalEntry.EventId,
                data = new
                {
                    message = exceptionType == "Secure" ? exception.Message : $"Internal server error ID = {journalEntry.EventId}"
                }
            };

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
        }
    }
}
