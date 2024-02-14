using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Models.Exceptions;
using WebApi.Models.Nodes;

namespace WebApi.Controllers
{
    [ApiController]
    public class TreeController : ControllerBase
    {
        private readonly DataContext _dbContext;
        private IValidator<string> _validator;

        public TreeController(DataContext dbContext, IValidator<string> validator)
        {
            _dbContext = dbContext;
            _validator = validator;
        }

        [HttpPost]
        [Route("api.user.tree.get")]
        public async Task<TreeNode> Get(string treeName, CancellationToken token)
        {
            //this is an example that i know about validation issues. better to use here if statement
            var validationResult = _validator.Validate(treeName);
            if (!validationResult.IsValid)
                throw new SecureException($"The skip field is required.");

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
