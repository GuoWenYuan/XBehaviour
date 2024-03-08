using System.Collections.Generic;
using System.Linq;
using XNode;

namespace XBehaviour.Editor
{
    public static class Utils
    {
    
        public static List<T> OutputNodes<T>(this XNode.Node node, string outputFieldName) where T : XNode.Node
        {
            return node.Outputs.ToList().Find(p=>p.fieldName == outputFieldName).GetConnections().ConvertAll(p => p.node as T);
        }
    }
}