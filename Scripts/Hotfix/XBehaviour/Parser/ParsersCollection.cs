using System;
using System.Collections.Generic;
using YamlDotNet.RepresentationModel;

namespace XBehaviour.Runtime
{
    public static class ParsersCollection
    {
        static readonly Dictionary<string,IParser> parsers = new ();
        static bool loaded = false;
        /// <summary>
        /// 转换标记
        /// </summary>
        private static readonly string parserFlag = "className";

        public static void Collection()
        {
            if (loaded) return;
			parsers.Add("Parallel",new ParallelParser());
			parsers.Add("Root",new RootParser());
			parsers.Add("Selector",new SelectorParser());
			parsers.Add("Task",new TaskParser());
			parsers.Add("Vector",new VectorParser());

            
            loaded = true;
        }
        
        public static IParser GetParser(string name)
        {
            if (parsers.TryGetValue(name,out var parser))
            {
                return parser;
            }
            
            return null;
        }
        

        /// <summary>
        /// 开始转换
        /// </summary>
        public static T Parser<T>(YamlNode rootNode)
        {
            string name = rootNode.GetValueFromNode(parserFlag);
            return (T)GetParser(name).Decoding(rootNode);
        }

        public static void Dispose()
        {
            parsers.Clear();
            loaded = false; // Clean
        }

    }
}