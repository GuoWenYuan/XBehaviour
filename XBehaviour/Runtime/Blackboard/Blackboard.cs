using System;
using System.Collections.Generic;
using UnityEngine;

namespace XBehaviour.Runtime
{
    public abstract partial class Blackboard
    {
      
        #region Notification
        /// <summary>
        /// 数值变化类型
        /// </summary>
        public enum Type
        {
            /// <summary>
            /// 新增
            /// </summary>
            ADD,

            /// <summary>
            /// 移除
            /// </summary>
            REMOVE,

            /// <summary>
            /// 修改
            /// </summary>
            CHANGE,
            
            /// <summary>
            /// 重置值 = 写入相同的值
            /// </summary>
            RESET
        }

        /// <summary>
        /// 通知消息
        /// </summary>
        private struct Notification
        {
            public string key;
            public Type type;
            public object value;

            public Notification(string key, Type type, object value)
            {
                this.key = key;
                this.type = type;
                this.value = value;
            }
        }

        //所有的数据源信息
        private Dictionary<string, object> _datas = new Dictionary<string, object>();

        //所有的消息内容
        private List<Notification> _notifications = new List<Notification>();

        //所有要广播的消息内容
        private List<Notification> _notificationsDispatch = new List<Notification>();

        //所有的观察消息
        private Dictionary<string, List<Action<Type, object>>> _observers =
            new Dictionary<string, List<Action<Type, object>>>();

        //所有移除列表的观察消息
        private Dictionary<string, List<Action<Type, object>>> _removeObservers =
            new Dictionary<string, List<Action<Type, object>>>();

        //所有新增消息列表
        private Dictionary<string, List<Action<Type, object>>> _addObservers =
            new Dictionary<string, List<Action<Type, object>>>();

        /// <summary>
        /// 是否在消息广播期间
        /// </summary>
        private bool _isNotifiyng = false;

        private bool _enable = false;
#if UNITY_EDITOR
        public List<string> Keys =>  new List<string>(_datas.Keys);
#endif
        /// <summary>
        /// 启动黑板
        /// </summary>
        public void Enable()
        {
            _enable = true;
        }

        /// <summary>
        /// 取消黑板
        /// </summary>
        public void Disable()
        {
            _enable = false;
        }

        #region Value Set And Get

        public object this[string key]
        {
            get { return Get(key); }
            set { Set(key, value); }
        }

        /// <summary>
        /// 写入黑板数值
        /// </summary>
        /// <param name="key">数值名称</param>
        /// <param name="value">值</param>
        public void Set(string key, object value)
        {

            if (!_datas.ContainsKey(key))
            {
                _datas[key] = value;
                SetNotification(key, Type.ADD, value);
            }
            else
            {
                if (value == null)
                {
                    _datas.Remove(key);
                    SetNotification(key, Type.REMOVE, null);
                    return;
                }

                if (!_datas[key].Equals(value))
                {
                    _datas[key] = value;
                    SetNotification(key, Type.CHANGE, value);
                }
                else
                {
                    SetNotification(key,Type.RESET,value);
                }
            }
        }

        public bool GetBool(string key)
        {
            return Get<bool>(key);
        }

        public float GetFloat(string key)
        {
            object result = Get(key);
            if (result == null)
            {
                return float.NaN;
            }

            return (float)Get(key);
        }

        public Vector3 GetVector3(string key)
        {
            return Get<Vector3>(key);
        }

        public int GetInt(string key)
        {
            return Get<int>(key);
        }
        

        public T Get<T>(string key)
        {
            object result = Get(key);
            if (result == null)
            {
                return default(T);
            }

            return (T)result;
        }

        public object Get(string key)
        {
            _datas.TryGetValue(key, out var data);
            return data;
        }

        /// <summary>
        /// 取消写入
        /// </summary>
        /// <param name="key">取消的Key</param>
        public void UnSet(string key)
        {
            Set(key, null);
        }

        /// <summary>
        /// 写入消息广播信息
        /// </summary>
        private void SetNotification(string key, Type type, object value)
        {
            //在非启用状态下不进行广播
            if (!_enable)
            {
                return;
            }

            this._notifications.Add(new Notification(key, type, value));
            //写入新数值进行消息广播
            NotifiyObservers();
        }


        #endregion

        #region Observers Add And Remove

        /// <summary>
        /// 添加数值观察
        /// </summary>
        public void AddObservers(string key, System.Action<Type, object> observer)
        {
            List<System.Action<Type, object>> observers = GetObserverList(this._observers, key);
            if (!_isNotifiyng)
            {
                if (!observers.Contains(observer))
                {
                    observers.Add(observer);
                }
            }
            else
            {
                if (!observers.Contains(observer))
                {
                    List<System.Action<Type, object>> addObservers = GetObserverList(this._addObservers, key);
                    if (!addObservers.Contains(observer))
                    {
                        addObservers.Add(observer);
                    }
                }

                List<System.Action<Type, object>> removeObservers = GetObserverList(this._removeObservers, key);
                if (removeObservers.Contains(observer))
                {
                    removeObservers.Remove(observer);
                }
            }
        }

        /// <summary>
        /// 移除数值观察
        /// </summary>
        /// <param name="key"></param>
        /// <param name="observer"></param>
        public void RemoveObserver(string key, System.Action<Type, object> observer)
        {
            List<System.Action<Type, object>> observers = GetObserverList(this._observers, key);
            if (!_isNotifiyng)
            {
                if (observers.Contains(observer))
                {
                    observers.Remove(observer);
                }
            }
            else
            {
                List<System.Action<Type, object>> removeObservers = GetObserverList(this._removeObservers, key);
                if (!removeObservers.Contains(observer))
                {
                    if (observers.Contains(observer))
                    {
                        removeObservers.Add(observer);
                    }
                }

                List<System.Action<Type, object>> addObservers = GetObserverList(this._addObservers, key);
                if (addObservers.Contains(observer))
                {
                    addObservers.Remove(observer);
                }
            }
        }


        private List<System.Action<Type, object>> GetObserverList(
            Dictionary<string, List<System.Action<Type, object>>> target, string key)
        {
            List<System.Action<Type, object>> observers;
            if (target.ContainsKey(key))
            {
                observers = target[key];
            }
            else
            {
                observers = new List<System.Action<Type, object>>();
                target[key] = observers;
            }

            return observers;
        }

        #endregion

        /// <summary>
        /// 信息广播
        /// </summary>
        private void NotifiyObservers()
        {
            if (_notifications.Count == 0)
            {
                return;
            }

            _notificationsDispatch.Clear();
            _notificationsDispatch.AddRange(_notifications);
            _notifications.Clear();

            _isNotifiyng = true;
            foreach (Notification notification in _notificationsDispatch)
            {
                if (!this._observers.ContainsKey(notification.key))
                {
                    continue;
                }

                List<System.Action<Type, object>> observers = GetObserverList(this._observers, notification.key);
                foreach (System.Action<Type, object> observer in observers)
                {
                    if (this._removeObservers.ContainsKey(notification.key) &&
                        this._removeObservers[notification.key].Contains(observer))
                    {
                        continue;
                    }

                    observer(notification.type, notification.value);
                }
            }

            foreach (string key in this._addObservers.Keys)
            {
                GetObserverList(this._observers, key).AddRange(this._addObservers[key]);
            }

            foreach (string key in this._removeObservers.Keys)
            {
                foreach (System.Action<Type, object> action in _removeObservers[key])
                {
                    GetObserverList(this._observers, key).Remove(action);
                }
            }

            this._addObservers.Clear();
            this._removeObservers.Clear();

            _isNotifiyng = false;

        }
        

        #endregion        
       
    }
}