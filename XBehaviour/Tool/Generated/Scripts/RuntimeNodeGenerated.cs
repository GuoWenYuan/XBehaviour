#if UNITY_EDITOR


using System;
using System.Reflection;
using XBehaviour.Runtime;
using XBehaviour.Tool;
using Utils = XBehaviour.Tool.Utils;

namespace XBehaviour.Tool
{
    /// <summary>
    /// 运行时节点脚本   Root<->TreeNode
    /// </summary>
    public class RuntimeNodeGenerated : IGenerated
    {
        public Assembly Assembly => Assembly.GetAssembly(typeof(IParser));
        public string TemplatePath => Utils.TemplatePath.RuntimeNode;
        public string GeneratedPath { get; set; }
        public Type ParserType(ScriptGeneratedData data)
        {
            return Assembly.GetType("XBehaviour.Runtime" + data.className);
        }

        public string ParserScripts(string template, ScriptGeneratedData data, out string className)
        {
            className = data.className;
            Type editorDataType = EditorDataAssembly.GetType("XBehaviour.Editor." + data.className + "View");
            string parentName = editorDataType.BaseType.Name.Replace("View", "");
            parentName = parentName.Replace("Data", "");
            template = template.Replace("父节点名称", parentName);
            string temp = template.Replace("节点数据名称", data.className);
            temp = temp.Replace("节点名称", data.className);
            return temp.Replace("节点Name", data.displayName);
        }

        public bool ParserCondition(Type type)
        {
            return type != null;
        }


        private Assembly EditorDataAssembly => Assembly.GetAssembly(typeof(IParser));
    

        public void OnBuildEnd()
        {
            
        }
    }
}
#endif