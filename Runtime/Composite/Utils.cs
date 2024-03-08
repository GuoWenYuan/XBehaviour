using System;

namespace XBehaviour.Runtime
{
    public static class Utils
    {
        
        /// <summary>
        /// 停止所有子节点，递归
        /// </summary>
        /// <param name="node">要停止的节点</param>
        /// <param name="cancelHandler">是否要取消事件</param>
        public static void StopChildren(this INode node,bool clearEvents = false)
        {
            if (node.Children == null || node.Children.Count == 0) return;
            foreach (var child in node.Children)
            {
                if (clearEvents)
                {
                   child.ClearEvents();
                }

                child.Stop();
                child.StopChildren(clearEvents);
            }
        }
        
        /// <summary>
        /// 黑板转换
        /// </summary>
        /// <param name="board">黑板</param>
        /// <typeparam name="T">目标类型</typeparam>
        public static T Parser<T>(this Blackboard board) where T : Blackboard
        {
            return board as T;  
        }
    }
    
 
    
    
}