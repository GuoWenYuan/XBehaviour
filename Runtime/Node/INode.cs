using System.Collections.Generic;

namespace XBehaviour.Runtime
{
    
    public enum State
    {
        //未激活
        Inactive,
        //激活
        Active,
        //停止请求
        StopRequested,
    }
        
    //行为树结果状态
    public enum ResultState
    {
        //运行中
        Running = -1,
        //失败
        Failure = 0,
        //成功
        Success = 1,
    }
    /// <summary>
    /// 行为树 所有节点基类
    /// </summary>
    public interface INode
    {
        long InstanceId { get; set; }
        string Name { get; set; }
        /// <summary>
        /// 当行为树完成时
        /// </summary>
        System.Action<ResultState> StopHandler { get; set; }
        /// <summary>
        /// 当行为树节点开始运行时
        /// </summary>
        System.Action StartHandler { get; set; }
        /// <summary>
        /// 当前节点是否为激活状态
        /// </summary>
        bool IsActive { get; }
        /// <summary>
        /// 当前状态
        /// </summary>
        State CurrentState { get; }
        /// <summary>
        /// 当前结果状态
        /// </summary>
        ResultState CurrentResultState { get; }
       
        INode Parent { get; set; }
        INode Root { get; set; }

        Blackboard Board { get; set; }

        List<INode> Children { get; }
        
#if UNITY_EDITOR
        public float DebugLastStopRequestAt { get; set; }
        public int DebugNumStartCalls { get; set; }
        public int DebugNumStopCalls { get; set; }
        public bool DebugLastResult { get; set; }
        public int TotalNumStartCalls { get; set;}
        public int TotalNumStopCalls { get; set; }
        public int TotalNumStoppedCalls { get; set;}
        public List<INode> DebugChildren { get; }
        public List<INode> DebugActiveChild { get; }
        
        /// <summary>
        /// 在Editor模式下显示的label
        /// </summary>
        public string Label => this.ToString();


        /// <summary>
        /// 在editor模式下折叠
        /// </summary>
        public bool Collapse { get; set; }


#endif
        
        /// <summary>
        /// 创建节点
        /// </summary>
        void Create();
        
        
        /// <summary>
        /// 销毁节点
        /// </summary>
        void Destroy();
        
        /// <summary>
        /// 节点开始运行
        /// </summary>
        void Start(); 
        
        /// <summary>
        /// 停止节点运行
        /// </summary>
        /// <param name="succeed">节点运行是否成功</param>
        void Stop(bool succeed = false);
        
        
        /// <summary>
        /// 添加子节点
        /// </summary>
        void AddChild(INode child); 
        
        /// <summary>
        /// 添加子节点
        /// </summary>
        void AddChild(List<INode> children);
        
        /// <summary>
        /// 移除子节点
        /// </summary>
        void RemoveChild(INode child);
        
        /// <summary>
        /// 当子节点停止
        /// </summary>
        /// <param name="child"></param>
        /// <param name="succeeded"></param>
        void ChildStopped(INode child, bool succeeded);
        
        
        void ClearEvents(); //清除自定义事件
    }
}