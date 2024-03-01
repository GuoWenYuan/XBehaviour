using UnityEngine;

namespace XBehaviour.Runtime
{
    /// <summary>
    /// 并行节点
    /// </summary>
    public class Parallel : Composite
    {
        /// <summary>
        /// 并行节点类型
        /// </summary>
        public enum ParallelState
        {
            [InspectorName("执行完成所有子节点返回，无论成功失败")]
            Define = 0,
            [InspectorName("某一子节点失败返回")]
            FailureOne = 1,
            [InspectorName("某一子节点成功返回")]
            SuccessOne = 2
        }
        
        /// <summary>
        /// 并行节点子节点返回父节点类型
        /// </summary>
        public enum ChildReturnStatue
        {
            [InspectorName("根据子节点返回的值")]
            FollowChild = 0,
            [InspectorName("正确")]
            True = 1,
            [InspectorName("错误")]
            False = 2
        }


        public ParallelState state;
        public ChildReturnStatue returnStatue;
        //已经运行过的子节点个数
        private int _childRunningCount;
        
        protected override void DoChildStopped(INode child, bool succeeded)
        {
            if (state == ParallelState.FailureOne && !succeeded)
            {
                Stop(GetReturnStatue(false));
                return;
            }
            if (state == ParallelState.SuccessOne && succeeded)
            {
                Stop(GetReturnStatue(true));
                return;
            }

            _childRunningCount++;
            if (_childRunningCount == ChildrenCount)
            {
                Stop(GetReturnStatue(succeeded));
            }
        }

        protected override void ProcessChildren()
        {
            _childRunningCount = 0;
            for (var i = 0; i < ChildrenCount; i++)
            {
                Children[i].Start();
            }
        }
        
        private bool GetReturnStatue(bool childSucceed)
        {
            switch (returnStatue)
            {
                case ChildReturnStatue.FollowChild:
                    return childSucceed;
                case ChildReturnStatue.True:
                    return true;
                case ChildReturnStatue.False:
                    return false;
                default:
                    return false;
            }
        }

        protected override void OnStop(bool succeed, bool complete)
        {
            this.StopChildren();
        }
    }
}