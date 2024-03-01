#if UNITY_EDITOR

using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using XNode;

namespace XBehaviour.Editor
{
    [Node.CreateNodeMenu("根节点/基础任务节点")]
    [Node.NodeWidth(400)]
    [Node.NodeTint(30, 30, 30)]
    public class TaskView : NodeView
    {
        public string value;
       
        [InspectorName("测试用向量")]
        public VectorView vector;
        protected override void Init()
        {
            base.Init();
            name = "基础任务节点";
        }
    }
}

#endif