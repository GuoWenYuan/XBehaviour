namespace XBehaviour.Runtime
{
    public enum RunningType
    {
        Begin,
        SucceedIndex,
        FailIndex,
    }
    
    /// <summary>
    /// 树的根节点
    /// </summary>
    public class Root : Composite
    {
        public RunningType RunningType { get; set; }
        
        private Blackboard _board;
        /// <summary>
        /// 黑板
        /// </summary>
        public override Blackboard Board 
        { 
            get => _board;
            set
            {
                if (value == null)
                {
                    //取消黑板的消息机制
                    _board.Disable();
                    _board.Destroy();
                }
                else
                {
                    //激活黑板的消息机制
                    _board = value;
                    _board.Create();
                    _board.Enable();
                }
            }
        }
        
        /// <summary>
        /// 获取黑板
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetBoard<T>()  where T : Blackboard
        {
            return Board as T;
        }
      

        protected override void OnStop(bool succeed, bool complete)
        {
            Board?.Disable();
        }

        protected override void OnDestroy()
        {
            Board = null;
        }

        protected override void DoChildStopped(INode child, bool succeeded)
        {
            if (succeeded) ProcessChildren();
            else Stop(false);
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