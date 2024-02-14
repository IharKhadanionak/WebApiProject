using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.IHandler;
using WebApi.Models.Nodes;

namespace WebApi.Handler
{
    public class TreeHandler : ITreeHandler
    {
        private readonly DataContext _dbContext;

        public TreeHandler(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TreeNode> GetCreateTreeAsync(string treeName, CancellationToken token)
        {
            TreeNode? tree = await _dbContext.Nodes
                .Where(x => x.SupervisorId == null && x.Name == treeName)
                .FirstOrDefaultAsync();

            if (tree == null)
            {
                int nextId = _dbContext.Nodes.Max(x => (int?)x.Id) + 1 ?? 1;
                string path = $"/{nextId}";
                tree = new TreeNode
                {
                    Name = treeName,
                    SupervisorId = null,
                    Path = path
                };

                await _dbContext.Nodes.AddAsync(tree, token);
                await _dbContext.SaveChangesAsync(token);
            }
            else
            {
                var entireTree = _dbContext.Nodes
                    .AsNoTracking()
                    .Where(x => x.Path.StartsWith(tree.Path))
                    .ToDictionary(x => x.Id);

                TreeBuilder builder = new TreeBuilder();
                tree = builder.BuildTree(entireTree);
            }
            return tree;
        }
    }
}
