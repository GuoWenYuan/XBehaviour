namespace XBehaviour.Runtime
{
    /// <summary>
    /// 装饰器节点
    /// </summary>
    public class Decorator : Composite
    {
        protected override void DoChildStopped(INode child, bool succeeded)
        {
            Stop(succeeded);
        }

        protected override void ProcessChildren()
        {
            Children[0].Start();
        }
    }
}