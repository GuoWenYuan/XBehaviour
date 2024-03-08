using UnityEditor;
using UnityEngine;
using XBehaviour.Runtime;
using XNodeEditor;

namespace XBehaviour.Tool
{
    [NodeGraphEditor.CustomNodeGraphEditor(typeof(XNode.NodeGraph))]
    public class XBehaviourNBodeGraphEditor : NodeGraphEditor
    {
        public override void OnGUI()
        {
            base.OnGUI();
            if (GUI.Button(new Rect(0, 0, 100, 30), "导出行为树"))
            {
                ExportBehaviourTree();
            }
        }
        
        private void ExportBehaviourTree()
        {
            var graph = (XBehaviourNodeGraph)this.target;
            var filePath = EditorUtility.SaveFilePanel("保存行为树配置文件", XBehaviourPath.ConfigPath.RootPath, graph.name, "yaml");
            if (!string.IsNullOrEmpty(filePath))
            {
                graph.Export(filePath);
            }
            
        }
    }
}