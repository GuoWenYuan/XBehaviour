#if UNITY_EDITOR

namespace XBehaviour.Editor
{
    [CreateNodeMenu("组合节点/顺序节点")]
    [NodeWidth(400)]
    [NodeTint(46, 139, 87)]
    public class SequenceView : NodeView
    {
        protected override void Init()
        {
            base.Init();
            name = "顺序节点";
        }
    }
}

#endif