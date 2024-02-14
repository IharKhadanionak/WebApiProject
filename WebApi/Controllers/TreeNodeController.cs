using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Models.Exceptions;
using WebApi.Models.Nodes;
using WebApi.Validators.TreeNodes;

namespace WebApi.Controllers
{
    [ApiController]
    public class TreeNodeController : ControllerBase
    {
        private readonly DataContext _dbContext;

        public TreeNodeController(DataContext dataContext)
        {
            _dbContext = dataContext;
        }

        [HttpPost]
        [Route("api.user.tree.node.create")]
        public async Task<TreeNode> CreateAsync(string treeName, int parentNodeId, string nodeName, CancellationToken token)
        {
            if (string.IsNullOrEmpty(treeName) ||  string.IsNullOrEmpty(nodeName))
                throw new SecureException($"The skip field is required.");

            TreeNode? parentNode = await _dbContext.Nodes.FindAsync(parentNodeId, token);
            if (parentNode == null)
                throw new SecureException($"Node with ID = {parentNodeId} was not found");

            TreeNode? tree = await _dbContext.Nodes.FirstOrDefaultAsync(x => x.SupervisorId == null && x.Name == treeName, token);
            if (tree == null)
                throw new SecureException($"Tree with name = {treeName} was not found");

            if (!parentNode.Path.StartsWith(tree.Path))
                throw new SecureException("Requested node was found, but it doesn't belong your tree");

            int nextId = _dbContext.Nodes.Max(x => (int?)x.Id) + 1 ?? 1;
            string newPath = $"{parentNode.Path}/{nextId}";

            bool isNameDuplicate = await _dbContext.Nodes.AnyAsync(x=> x.Path.StartsWith(tree.Path) && x.Path.Length == newPath.Length && x.Name == nodeName, token);
            if (isNameDuplicate)
                throw new SecureException("Duplicate name");

            TreeNode newNode = new TreeNode
            {
                Name = nodeName,
                SupervisorId = parentNode.Id,
                Path = newPath
            };

            await _dbContext.Nodes.AddAsync(newNode, token);
            await _dbContext.SaveChangesAsync(token);
            return newNode;
        }

        [HttpPost]
        [Route("api.user.tree.node.delete")]
        public async Task DeleteAsync(string treeName, int nodeId, CancellationToken token)
        {
            if(string.IsNullOrEmpty(treeName))
                throw new SecureException($"The skip field is required.");

            TreeNode? node = await _dbContext.Nodes.FindAsync(nodeId, token);
            if (node == null)
                throw new SecureException($"Node with ID = {nodeId} was not found");

            bool hasChildren = await _dbContext.Nodes.AnyAsync(x => x.Path.StartsWith(node.Path) && x.Path.Length > node.Path.Length, token);
            if (hasChildren)
                throw new SecureException($"You have to delete all children nodes first");

            TreeNode? tree = await _dbContext.Nodes.FirstOrDefaultAsync(x => x.SupervisorId == null && x.Name == treeName, token);
            if (tree == null)
                throw new SecureException($"Tree with name = {treeName} was not found");

            if (!node.Path.StartsWith(tree.Path))
                throw new SecureException("Requested node was found, but it doesn't belong your tree");

            _dbContext.Nodes.Remove(node);
            await _dbContext.SaveChangesAsync(token);
        }

        [HttpPost]
        [Route("api.user.tree.node.rename")]
        public async Task RenameAsync(string treeName, int nodeId, string newNodeName, CancellationToken token)
        {
            if (string.IsNullOrEmpty(treeName) || string.IsNullOrEmpty(newNodeName))
                throw new SecureException($"The skip field is required.");

            var node = await _dbContext.Nodes.FindAsync(nodeId, token);
            if (node == null)
                throw new SecureException($"Node with ID = {nodeId} was not found");
            else if (node.SupervisorId == null)
                throw new SecureException($"Couldn't rename root node");

            TreeNode? tree = await _dbContext.Nodes.FirstOrDefaultAsync(x => x.SupervisorId == null && x.Name == treeName, token);
            if (tree == null)
                throw new SecureException($"Tree with name = {treeName} was not found");

            if (!node.Path.StartsWith(tree.Path))
                throw new SecureException("Requested node was found, but it doesn't belong your tree");

            node.Name = newNodeName;
            await _dbContext.SaveChangesAsync(token);
        }
    }
}
