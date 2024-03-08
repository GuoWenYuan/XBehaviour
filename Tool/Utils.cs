using System.Reflection;
using System.Text;
using XBehaviour.Editor;
using YamlDotNet.Serialization;

namespace XBehaviour.Tool
{
    public static class Utils
    {
        public static class TemplatePath
        {
            private static readonly string RootPath = "Assets/Tools/XBehaviour/Tool/Generated/Template/"; //模板路径
            public static readonly string  RuntimeNodeData = RootPath + "RuntimeNodeData.txt"; //运行时节点数据
            public static readonly string RuntimeNode =  RootPath + "RuntimeNode.txt"; //运行时节点
            public static readonly string ParserPath = RootPath + "Parser.txt"; //解析器
            public static readonly string ParserCollectionPath = RootPath + "ParserCollection.txt"; //解析器集合
            public static readonly string EditorNodePath = RootPath + "EditorNode.txt"; //编辑器节点
        }
        
        public static class GeneratedPath
        {
            public static readonly string RootPath = "Assets/Scripts/Hotfix/XBehaviour/"; //生成路径
            public static readonly string RuntimeNodeData = RootPath + "RuntimeNodeData.txt"; //运行时节点数据
            public static readonly string RuntimeNode =  RootPath + "RuntimeNode.txt"; //运行时节点
            public static readonly string ParserPath = RootPath + "Parser.txt"; //解析器5
            public static readonly string ParserCollectionPath = RootPath + "Parser/ParsersCollection.cs"; //解析器集合
            public static readonly string EditorNodePath = RootPath + "EditorNode.txt"; //编辑器节点
        }
        
        
        
        /// <summary>
        /// 是否为需要导出的字段
        /// </summary>
        public static bool Export(FieldInfo fieldInfo)
        {
            return !YamlIgnore(fieldInfo) && !MustExport(fieldInfo);
        }
        
        
       
        private static bool YamlIgnore(FieldInfo fieldInfo)
        {
            return fieldInfo.GetCustomAttribute(typeof(YamlIgnoreAttribute)) != null;
        }
        
        /// <summary>
        /// 是否为必须要导出的字段
        /// </summary>
        private static bool MustExport(FieldInfo fieldInfo)
        {
            return fieldInfo.GetCustomAttribute(typeof(MustExportAttribute)) != null;
        }
        
        public static string ToString(string constant)
        {
            return $"\"{constant}\"";
        }

        /// <summary>
        /// 增加缩进次数 1缩进 = 4 空格
        /// </summary>
        public static string Retract(int count)
        {
            StringBuilder stringBuilder = new StringBuilder();
            //count = count * 4;
            for (int i = 0; i < count; i++)
            {
                stringBuilder.Append("\t");
            }

            return stringBuilder.ToString();
        }

        
            

    }
}