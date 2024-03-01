using System.Collections.Generic;

namespace XBehaviour.Runtime
{
    public interface ISerialization
    {
        public enum Type
        {
            XBehaviour,
        }
        
         /// <summary>
         /// 单独序列化一个对象
         /// </summary>
         /// <param name="data"></param>
         /// <typeparam name="T"></typeparam>
         /// <returns></returns>
         string Serializer<T>(T data);
         
         /// <summary>
         /// 反序列化单独对象
         /// </summary>
         T Deserializer<T>(string configuration);
    }
}