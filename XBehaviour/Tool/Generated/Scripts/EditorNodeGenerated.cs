#if UNITY_EDITOR

using System;
using System.Reflection;
using XBehaviour.Editor;
using XBehaviour.Runtime;

namespace XBehaviour.Tool
{
    /// <summary>
    /// 节点数据生成器  TreeNodeData
    /// </summary>
    public class EditorNodeGenerated : IGenerated
    {
        public Assembly Assembly => Assembly.GetAssembly(typeof(RootView));
        public string TemplatePath => Utils.TemplatePath.EditorNodePath;
        public string GeneratedPath { get; set; }
        public Type ParserType(ScriptGeneratedData data)
        {
            return Assembly.GetType("XBehaviour.Editor" + data.className);
        }

        public string ParserScripts(string template, ScriptGeneratedData data, out string className)
        {
            template = template.Replace("父节点名称", data.parentName);
            template = template.Replace("节点名称", data.className);
            template = template.Replace("编辑器界面创建层级", data.editorPanelShowName);
            template = template.Replace("节点显示名称", data.displayName);
            className = data.className + "View";
            return template;

        }

        public bool ParserCondition(Type type)
        {
            return type != null;
        }
        

        public void OnBuildEnd()
        {
            
        }
    }
}

#endif