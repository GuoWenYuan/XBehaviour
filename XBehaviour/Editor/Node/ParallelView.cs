#if UNITY_EDITOR
using XBehaviour.Runtime;

namespace XBehaviour.Editor
{
    [CreateNodeMenu("组合节点/并行节点")]
    [NodeWidth(400)]
    [NodeTint(100, 140, 0)]
    public class ParallelView : NodeView
    {
        
        public Parallel.ParallelState state;
        
        public Parallel.ChildReturnStatue returnStatue;
        
        protected override void Init()
        {
            base.Init();
            name = "并行节点";
        }
    }
}
#endif