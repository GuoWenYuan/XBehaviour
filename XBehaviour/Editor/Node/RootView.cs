#if UNITY_EDITOR
using UnityEngine;
using XBehaviour.Runtime;

namespace XBehaviour.Editor
{
    [CreateNodeMenu("根节点/基础根节点")]
    [NodeWidth(400)]
    [NodeTint(70, 130, 180)]
    public class RootView : NodeView
    {
        [InspectorName("根节点运行类型")] public RunningType RunningType;

        protected override void Init()
        {
            base.Init();
            name = "基础根节点";
        }
    }
}
#endif