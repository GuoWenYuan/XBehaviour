


namespace XBehaviour.Runtime
{
    /// <summary>
    /// 空白节点，用来占位，执行当前分支的结束事件/开始事件
    /// </summary>
    public class BlankTask : Task
    {
     

        protected override void OnStart()
        {
            base.OnStart();
            Stop(true);
        }
    }
}