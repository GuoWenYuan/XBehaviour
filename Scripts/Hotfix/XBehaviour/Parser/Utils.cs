using YamlDotNet.RepresentationModel;

namespace XBehaviour.Runtime
{
    public static class Utils
    {
        public static string GetScalarValue(this YamlNode node)
        {
            return ((YamlScalarNode)node).Value;
        }

        public static YamlNode GetChildNode(this YamlNode node,string key)
        {
            return node[key];
        }

        public static string GetValueFromNode(this YamlNode node, string key)
        {
            return node.GetChildNode(key).GetScalarValue();
        }

        public static YamlSequenceNode GetSequenceNode(this YamlNode node, string key)
        {
            return (YamlSequenceNode)node.GetChildNode(key);
        }
    }
}