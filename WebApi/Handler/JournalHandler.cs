using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.IHandler;

namespace WebApi.Handler
{
    public class JournalHandler : IJournalHandler
    {
        private readonly DataContext _dbContext;

        public JournalHandler(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<object> GetRangeAsync(int skip, int take, DateTime from, DateTime to)
        {
            var messageList = _dbContext.ExceptionJournals.Where(date => date.CreatedAt >= from && date.CreatedAt <= to);

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
