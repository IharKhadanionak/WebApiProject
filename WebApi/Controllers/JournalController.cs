using Microsoft.AspNetCore.Mvc;
using WebApi.IHandler;
using WebApi.Models;

namespace WebApi.Controllers
{
    [ApiController]
    public class JournalController : ControllerBase
    {
        private readonly IJournalHandler _journalHandler;

        public JournalController(IJournalHandler journalHandler)
        {
            _journalHandler = journalHandler;
        }

        [HttpPost]
        [Route("api.user.journal.getRange")]
        public async Task<object> GetRange(int skip, int take, [FromBody] Filter dates)
        {
            var response = await _journalHandler.GetRangeAsync(skip, take, dates.From, dates.To);
            return response;
        }

        [HttpPost]
        [Route("api.user.journal.getSingle")]
        public async Task<object> GetSignle(int id)
        {
            var response = await _journalHandler.GetSignleAsync(id);
            return response;
        }
    }
}
