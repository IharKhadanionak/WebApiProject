namespace WebApi.Models.Nodes
{
    public class TreeBuilder
    {
        public TreeNode BuildTree(Dictionary<int, TreeNode> dict)
        {
            foreach (var item in dict)
            {
                if (item.Value.SupervisorId != null)
                {
                    TreeNode parent = dict[item.Value.SupervisorId ?? default(int)];
                    TreeNode child = dict[item.Value.Id];
                    parent.Children.Add(child);
                }
            }
            return dict.First().Value;
        }
    }
}
