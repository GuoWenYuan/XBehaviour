
namespace XBehaviour.Runtime
{
    public abstract class Composite : Node
    {
        public int ChildrenCount => Children.Count;
        public int RunningIndex { get; protected set; }

        protected override void OnStart()
        {
            RunningIndex = 0;
            ProcessChildren();
        }

        protected abstract void DoChildStopped(INode child, bool succeeded);

        protected abstract void ProcessChildren();
        public override void ChildStopped(INode child, bool succeeded)
        {
            DoChildStopped(child, succeeded);
        }
    }
}