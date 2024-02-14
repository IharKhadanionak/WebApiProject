using WebApi.Models.Nodes;

namespace WebApi.IHandler
{
    public interface ITreeHandler
    {
        Task<TreeNode> GetCreateTreeAsync(string treeName, CancellationToken token);
    }
}
