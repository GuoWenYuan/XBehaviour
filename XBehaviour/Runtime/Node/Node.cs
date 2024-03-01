using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace XBehaviour.Runtime
{
    /// <summary>
    /// 行为树根节点
    /// </summary>
    public abstract class Node : INode
    {
        public long InstanceId { get; set; }
        public string Name { get; set; }
        public Action<ResultState> StopHandler { get; set; } // 当行为树完成时
        public Action StartHandler { get; set; } // 当行为树节点开始运行时
        public bool IsActive => CurrentState == State.Active; // 当前节点是否为激活状态
        public State CurrentState { get; private set; }
        public ResultState CurrentResultState { get; private set; } // 当前结果状态
        public INode Parent { get; set; }
        public INode Root { get; set; }
        public virtual Blackboard Board
        {
            get => Root?.Board;
            set { }
        }

        public List<INode> Children { get; private set; }

       

        public void Create()
        {
            
        }

        public void Destroy()
        {
            StartHandler = null;
            StopHandler = null;
        }

        public void Start()
        {
            Assert.IsFalse(CurrentState == State.Active,"node is already active");
            CurrentState = State.Active;
            CurrentResultState = ResultState.Running;
            StartHandler?.Invoke();
            //处理节点信息相关
#if UNITY_EDITOR
            Root.TotalNumStartCalls ++;
            DebugNumStartCalls ++;
#endif
            OnStart();
        }

        public void Stop(bool succeed = false, bool complete = true)
        {
            if (CurrentState != State.Active) return;
#if UNITY_EDITOR
            Root.TotalNumStopCalls++;
            this.DebugLastStopRequestAt = UnityEngine.Time.time;
            this.DebugNumStopCalls++;
            DebugLastResult = succeed;
#endif
            CurrentState = State.Inactive;
            CurrentResultState = succeed ? ResultState.Success : ResultState.Failure;
            OnStop(succeed, complete);
            StopHandler?.Invoke(CurrentResultState);
            Parent?.ChildStopped(this, succeed);
        }

        protected virtual void OnCreate(){ }
        protected virtual void OnDestroy(){ }

        protected virtual void OnStart() {}
        protected virtual void OnStop(bool succeed,bool complete) {}

        public virtual void ChildStopped(INode child, bool succeeded){ }
      
        

        public void AddChild(INode child)
        {
            Children ??= new List<INode>();
            Children.Add(child);
            child.Parent = this;
            child.Root = Root;
        }

        public void AddChild(List<INode> children)
        {
            foreach (var child in children)
            {
                AddChild(child);
            }
        }

        public void RemoveChild(INode child)
        {
            Children.Remove(child);
        }

       

        #region EditorInspection
        
#if UNITY_EDITOR
        public float DebugLastStopRequestAt { get; set; }
        public int DebugNumStartCalls { get; set; }
        public int DebugNumStopCalls { get; set; }
        public bool DebugLastResult { get; set; }
        public int TotalNumStartCalls { get; set; }
        public int TotalNumStopCalls { get; set; }
        public int TotalNumStoppedCalls { get; set; }
        public List<INode> DebugChildren => Children;
        public List<INode> DebugActiveChild => DebugChildren.FindAll(p => p.IsActive);
        
        public bool Collapse { get; set; }
#endif


        #endregion



    }

}

