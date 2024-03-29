﻿#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Hotfix;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace XBehaviour.Tool
{
    /// <summary>
    /// 刷新节点配置文件
    /// </summary>
    public static class XBehaviourEditor
    {

        [MenuItem("Assets/程序-行为树工具/刷新模板", false, 101)]
        public static void Create()
        {
            var selection = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets);
            if (selection.Length == 0)
            {
                Debug.LogError("未选择脚本");
                return;
            }
            if (selection.Length > 1)
            {
                Debug.LogError("请勿框选");
                return;
            }
        
            Object obj = selection[0];
            var selectPath = AssetDatabase.GetAssetPath(obj);
            string scriptName = selectPath.Split('/')[selectPath.Split('/').Length - 1];
            if (!scriptName.EndsWith(".cs"))
            {
                Debug.LogError("请勿选择脚本外的内容");
                return;
            }

            selectPath = selectPath.Replace(scriptName, "");
            scriptName = scriptName.Replace(".cs", "");
            scriptName = scriptName.Replace("View", "");
            scriptName = scriptName.Replace("Data", "");
            var data = new ScriptGeneratedData()
            {
                className = scriptName,
            };
            new AutoGeneratedScript(new RuntimeDataGenerated(),selectPath,data).Build();
            new AutoGeneratedScript(new RuntimeNodeGenerated(),selectPath,data).Build();
            new AutoGeneratedScript(new ParserGenerated(),selectPath,data).Build();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            

        }




    
    }

    /// <summary>
    /// 编辑器node data模板生成
    /// </summary>
    public class EditorDataWindows : OdinEditorWindow
    {
        private static string _createPath;
        private static Dictionary<string, string> _nodeMap = new Dictionary<string, string>();
        private static string[] _parentPopups;
        private static int _selectParentIndex;
        private static string _editorSelectName = "编辑器界面创建层级,例:行为树/二级目录/创建时节点名称";
        [MenuItem("Assets/程序-行为树工具/新建EditorNode模板", false, 101)]
        public static void Open()
        {
            var obj = Selection.activeObject;

            if (obj == null) _createPath = "Assets";
            else _createPath = AssetDatabase.GetAssetPath(obj.GetInstanceID());
            
            CreateWindow<EditorDataWindows>().Show();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _nodeMap.Clear();
            _nodeMap.Add("默认","NodeView");
            _nodeMap.Add("根节点","RootView");
            _nodeMap.Add("任务节点","TaskView");
            _nodeMap.Add("装饰器节点","DecoratorView");
            _parentPopups = new string[_nodeMap.Count];
            int index = 0;
            foreach (var data in _nodeMap)
            {
                _parentPopups[index++] = data.Key;
            }
            _editorSelectName = "编辑器界面创建层级,例:行为树/二级目录/创建时节点名称";
        }

        private static string _nodeName;
        private static string _showName;
        protected override void OnGUI()
        {
            //base.OnGUI();
            GUILayout.BeginHorizontal();
            GUILayout.Label("节点名称:");
            _nodeName = GUILayout.TextField(_nodeName);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("显示在编辑器中的名称:");
            _showName = GUILayout.TextField(_showName);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("编辑器面板中选择层级:");
            _editorSelectName = GUILayout.TextField(_editorSelectName);
            GUILayout.EndHorizontal();
            _selectParentIndex = EditorGUILayout.Popup("父节点",_selectParentIndex,_parentPopups);
            if (GUILayout.Button("确认生成"))
            {
                ExportEditorNode();
            }
        }

        private void ExportEditorNode()
        {
            if (string.IsNullOrEmpty(_nodeName) || string.IsNullOrEmpty(_showName))
            {
                Debug.LogError("节点名称与显示名称不能为空！");
                return;
            }

            _createPath = Utils.GeneratedPath.RootPath + $"/{_nodeName}";
            /*
            if (Directory.Exists(_createPath))
            {
                Debug.LogError("创建失败，BT/Node下存在同名文件夹");
                return;
            }
            */

            Directory.CreateDirectory(_createPath);
            new AutoGeneratedScript(new EditorNodeGenerated(), _createPath, new ()
            {
                className = _nodeName,
                displayName = _showName,
                parentName = _nodeMap[_parentPopups[_selectParentIndex]],
                editorPanelShowName = _editorSelectName
            }).Build();
            Debug.Log($"导出成功,生成路径为:{_createPath}");
            AssetDatabase.Refresh();
            Close();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _createPath = null;
            _nodeName = null;
            _showName = null;
            _selectParentIndex = 0;
        }
    }

}

#endif
