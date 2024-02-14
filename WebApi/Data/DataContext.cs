using Microsoft.EntityFrameworkCore;
using WebApi.Models.Exceptions;
using WebApi.Models.Nodes;

namespace WebApi.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            
        }

        public DbSet<TreeNode> Nodes { get; set; }
        public DbSet<ExceptionJournal> ExceptionJournals { get; set; }
    }
}
