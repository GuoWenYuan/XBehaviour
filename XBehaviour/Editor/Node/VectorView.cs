#if UNITY_EDITOR
using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using XNode;
using YamlDotNet.Serialization;

namespace XBehaviour.Editor
{
    
    public class VectorView : XNode.Node
    {
      
        [HideInInspector]
        public string className;
        
        public float x;
        public float y;
        public float z;
        protected override void Init()
        {
            base.Init();
            className = "Vector3";
        }
    }
}
#endif