using System.IO;
using System.Text;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace XBehaviour.Runtime
{
    /// <summary>
    /// 行为树序列化相关
    /// </summary>
    public class XBehaviourSerialization : ISerialization
    {
        public string Serializer<T>(T data)
        {
            StringBuilder stringBuilder = new StringBuilder();
            StringWriter stringWriter = new StringWriter(stringBuilder);
    
            Serializer serializer = new Serializer();
            serializer.Serialize(stringWriter, data);
            return stringBuilder.ToString();
        }

        public T Deserializer<T>(string configuration)
        {
            return new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build().Deserialize<T>(configuration);
        }
        
        public T Deserializer<T>(string configuration,IYamlTypeConverter yamlTypeConverter)
        {
            return new DeserializerBuilder().WithTypeConverter(yamlTypeConverter).Build().Deserialize<T>(configuration);
        }
    }
}