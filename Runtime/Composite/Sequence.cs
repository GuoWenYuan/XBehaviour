
namespace XBehaviour.Runtime
{
    /// <summary>
    /// 顺序节点
    /// </summary>
    public class Sequence : Composite
    {
        protected override void DoChildStopped(INode child, bool succeeded)
        {
            if(succeeded) ProcessChildren();
            else Stop(false);
        }

        protected override void ProcessChildren()
        {
            if (RunningIndex == ChildrenCount)
            {
                Stop(true); return;
            }

            Children[RunningIndex].Start();
            RunningIndex++;
        }
    }
}