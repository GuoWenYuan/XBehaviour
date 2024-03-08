using UnityEngine;

namespace XBehaviour.Runtime
{
   
    
    /// <summary>
    /// 行为树选择节点
    /// </summary>
    public class Selector : Composite
    {
        
        public enum SelectionState
        {
            [InspectorName("某一子节点成功返回成功")]
            SuccessOne,
            [InspectorName("某一子节点失败返回成功")]
            FailureOne,
        }
        
        public SelectionState selectionState;
        

        protected override void DoChildStopped(INode child, bool succeeded)
        {
            if (succeeded && selectionState == SelectionState.SuccessOne)
            {
                Stop(true);
                return;   
            }

            if (!succeeded && selectionState == SelectionState.FailureOne)
            {
                Stop(true);
                return;   
            }
            ProcessChildren();
        }

        protected override void ProcessChildren()
        {
            if (RunningIndex == ChildrenCount)
            {
                Stop(true); return;
            }

            Children[RunningIndex].Start();
            RunningIndex++;
        }
    }
}