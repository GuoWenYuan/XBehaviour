using System;
using XBehaviour.Runtime;

namespace XBehaviour.Runtime
{
    public static class XBehaviourFactory
    {
            
        public static T Get<T>() where T : INode, new()
        {
            return GenerationEmpty<T>();
        }
            
            
        public static T Get<T>(INode parent,Root root) where T : INode, new()
        {
            T t = GenerationEmpty<T>(null,null);
            //t.Parent = parent;
            t.Root = root;
            parent.AddChild(t);
            t.Create();
            return t;
        }
            
            
        private static T GenerationEmpty<T>(Action startHandler = null,Action<ResultState> stopHandler = null) where T : INode, new()
        {
            T t = new T();
            if (t is Root root)
            {
                t.Root = root;
            }
            t.StartHandler = startHandler;
            t.StopHandler = stopHandler;
            return t;
        }
        
        
        //递归遍历行为树中所有节点
        public static void RecursionTraversal(INode node,Action<INode> action)
        {
            action(node);
            if (node.Children != null)
            {
                foreach (var child in node.Children)
                {
                    RecursionTraversal(child,action);
                }
            }
        }
    }
}