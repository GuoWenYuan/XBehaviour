#if UNITY_EDITOR

using XNode;

namespace XBehaviour.Editor
{
    [Node.CreateNodeMenu("根节点/基础装饰器")]
    [Node.NodeWidth(400)]
    [Node.NodeTint(75, 20, 75)]
    public class DecoratorView : NodeView
    {
        protected override void Init()
        {
            base.Init();
            name = "装饰节点";
        }
    }
}

#endif