using WebApi.Models.Nodes;

namespace WebApi.IHandler
{
    public interface ITreeNodeHandler
    {
        Task<TreeNode> Create(string treeName, int parentNodeId, string nodeName, CancellationToken token);
        Task Delete(string treeName, int nodeId, CancellationToken token);
        Task Rename(string treeName, int nodeId, string newNodeName, CancellationToken token);
    }
}
