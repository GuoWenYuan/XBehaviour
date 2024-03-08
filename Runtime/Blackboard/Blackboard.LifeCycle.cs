using System;
using System.Collections.Generic;

namespace XBehaviour.Runtime
{
    public partial class Blackboard
    {
        public void Create()
        {
            OnCreate();
        }

        public void Destroy()
        {
            OnDestroy();
        }
        
        protected abstract void OnCreate();
        protected abstract void OnDestroy();

    }
}