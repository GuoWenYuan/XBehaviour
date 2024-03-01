using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR

namespace XBehaviour.Runtime
{
    public class Debugger
    {
        private static readonly List<Debugger> _debuggers = new List<Debugger>();
        
        
        /// <summary>
        /// 通过gameObject获取
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public static Debugger Get(GameObject go)
        {
            return _debuggers.Find(p => p.DebuggerGameObject == go);
        }

        /// <summary>
        /// 获取一个root的debugger对象
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public static Debugger Get(Root root)
        {
            Debugger debugger = _debuggers.Find(p => p.BehaviorTree == root);
            if (debugger == null)
            {
                GameObject go = new GameObject(root.Name + "_" + root.InstanceId);
                go.transform.parent = DebuggerParent.transform;
                debugger = new Debugger() { BehaviorTree = root, DebuggerGameObject = go };
                Object.DontDestroyOnLoad(go);
                _debuggers.Add(debugger);
            }

            return debugger;
        }
        
        /// <summary>
        /// 移除debug对象
        /// </summary>
        /// <param name="root"></param>
        public static void Remove(Root root)
        {
            Debugger debugger = _debuggers.Find(p => p.BehaviorTree == root);
            if (debugger != null)
            {
                GameObject go = debugger.DebuggerGameObject;
                Object.Destroy(go);
                _debuggers.Remove(debugger);
            }
        }
        private static GameObject _debuggerParent;

        private static GameObject DebuggerParent
        {
            get
            {
                if (_debuggerParent == null)
                {
                    _debuggerParent = new GameObject("BehaviorTreeDebugger");
                    Object.DontDestroyOnLoad(_debuggerParent);
                }

                return _debuggerParent;
            }
        }

        public Root BehaviorTree;

        public Blackboard Blackboard => BehaviorTree.Board;

        public GameObject DebuggerGameObject;
        
    }
}

#endif