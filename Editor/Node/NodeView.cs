#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

using XNode;
using YamlDotNet.Serialization;

namespace XBehaviour.Editor
{
    
   
    public class NodeView : XNode.Node
    {
        [HideInInspector]
        public string className; // 类名

        
        
        [Input(connectionType = ConnectionType.Override,backingValue = ShowBackingValue.Never),YamlIgnore]
        public int parent;

        [Output(connectionType = ConnectionType.Multiple,backingValue = ShowBackingValue.Never),YamlIgnore]
        public int child;
        
        [HideInInspector]
        public List<NodeView> Children { get; set; } = new List<NodeView>();

        protected override void Init()
        {
           
        }

        public override object GetValue(NodePort port)
        {
            return this;
        }
    }

}
#endif