namespace XBehaviour.Runtime
{
    public static class Utils
    {
        /// <summary>
        /// 停止所有子节点，递归
        /// </summary>
        /// <param name="node">要停止的节点</param>
        /// <param name="cancelHandler">是否要取消事件</param>
        public static void StopChildren(this INode node,bool cancelHandler = false)
        {
            if (node.Children.Count == 0) return;
            foreach (var child in node.Children)
            {
                if (cancelHandler)
                {
                    child.StartHandler = null;
                    child.StopHandler = null;
                }

                child.Stop();
                child.StopChildren(cancelHandler);
            }
        }
    }
}