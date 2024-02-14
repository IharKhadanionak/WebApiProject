namespace WebApi.Models.Nodes
{
    public class TreeNode
    {
        public int Id { get; set; }
        public int? SupervisorId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public List<TreeNode> Children { get; set; } = new List<TreeNode>();
    }
}
