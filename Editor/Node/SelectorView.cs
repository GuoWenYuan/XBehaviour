#if UNITY_EDITOR
using UnityEngine;
using XBehaviour.Runtime;

namespace XBehaviour.Editor
{
    [CreateNodeMenu("组合节点/选择节点")]
    [NodeWidth(400)]
    [NodeTint(65, 105, 100)]
    public class SelectorView : NodeView
    {
        [InspectorName("选择节点状态")]
        public Selector.SelectionState selectionState;
        protected override void Init()
        {
            base.Init();
            name = "选择节点";
        }
    }
}
#endif