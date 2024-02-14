namespace WebApi.IHandler
{
    public interface IJournalHandler
    {
        Task<object> GetRangeAsync(int skip, int take, DateTime from, DateTime to);
        Task<object> GetSignleAsync(int id);
    }
}
