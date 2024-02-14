using Microsoft.AspNetCore.Mvc;
using WebApi.Data;
using WebApi.IHandler;
using WebApi.Models.Exceptions;
using WebApi.Models.Nodes;

namespace WebApi.Controllers
{
    [ApiController]
    public class TreeNodeController : ControllerBase
    {
        private readonly ITreeNodeHandler _treeNodeHandler;

        public TreeNodeController(DataContext dataContext, ITreeNodeHandler treeNodeHandler)
        {
            _treeNodeHandler = treeNodeHandler;
        }

        [HttpPost]
        [Route("api.user.tree.node.create")]
        public async Task<TreeNode> Create(string treeName, int parentNodeId, string nodeName, CancellationToken token)
        {
            if (string.IsNullOrEmpty(treeName) ||  string.IsNullOrEmpty(nodeName))
                throw new SecureException($"The skip field is required.");

            var newNode = await _treeNodeHandler.Create(treeName, parentNodeId, nodeName, token);
            return newNode;
        }

        [HttpPost]
        [Route("api.user.tree.node.delete")]
        public async Task Delete(string treeName, int nodeId, CancellationToken token)
        {
            if(string.IsNullOrEmpty(treeName))
                throw new SecureException($"The skip field is required.");

            await _treeNodeHandler.Delete(treeName, nodeId, token);
        }

        [HttpPost]
        [Route("api.user.tree.node.rename")]
        public async Task Rename(string treeName, int nodeId, string newNodeName, CancellationToken token)
        {
            if (string.IsNullOrEmpty(treeName) || string.IsNullOrEmpty(newNodeName))
                throw new SecureException($"The skip field is required.");

            await _treeNodeHandler.Rename(treeName, nodeId, newNodeName, token);
        }
    }
}
