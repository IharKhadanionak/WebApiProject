using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using WebApi.Data;
using WebApi.Models;
using WebApi.Models.Exceptions;
using WebApi.Models.Nodes;

namespace WebApi.Controllers
{
    [ApiController]
    public class JournalController : ControllerBase
    {
        private readonly DataContext _dbContext;

        public JournalController(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        [Route("api.user.journal.getRange")]
        public async Task<object> GetRangeAsync(int skip, int take, [FromBody] Filter dates)
        {
            var messageList = _dbContext.ExceptionJournals.Where(date => date.CreatedAt >= dates.From && date.CreatedAt <= dates.To);

            var count = await messageList.CountAsync();

            var items = await messageList
                .Skip(skip)
                .Take(take)
                .Select(x => new { id = x.Id, eventId = x.EventId, createdAt = x.CreatedAt })
                .ToListAsync();

            var response = new
            {
                skip,
                count,
                items
            };
            return response;
        }

        [HttpPost]
        [Route("api.user.journal.getSingle")]
        public async Task<object> GetSignleAsync(int id)
        {
            var message = await _dbContext.ExceptionJournals
                .Where(x => x.Id == id)
                .Select(x => new 
                {
                    x.StackTrace,
                    x.BodyParameters,
                    x.QueryParameters,
                    x.Id,
                    x.EventId,
                    x.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (message == null)
                throw new Exception();

            var response = new
            {
                text = message.QueryParameters + message.BodyParameters + message.StackTrace,
                message.Id,
                message.EventId,
                message.CreatedAt
            };
            return response;
        }
    }
}
