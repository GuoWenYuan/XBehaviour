using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XBehaviour.Editor;
using XBehaviour.Runtime;
using XNode;
using YamlDotNet.RepresentationModel;

namespace XBehaviour.Tool
{

	public static class XBehaviourGraphics
	{
		
		[MenuItem("Assets/Create/行为树编辑器/新建Graph编辑图", false, 101)]
		public static void CreateMapAreaGraphProcessor()
		{
			var graph = ScriptableObject.CreateInstance<XBehaviourNodeGraph>();
			ProjectWindowUtil.CreateAsset(graph, "BehaviorTree.asset");
		}
	}

	//[CreateAssetMenu(fileName = "NewXBehaviour",menuName = "Assets/Create/行为树",order = 15)]
	public class XBehaviourNodeGraph : NodeGraph {

		//[ContextMenu("导出行为树")]
		public void Export(string exportPath)
		{
			//TODO:执行导出行为树操作
			var serializerStr = CollectNodes();
			File.WriteAllText(exportPath,serializerStr);
			Debug.Log("导出成功，路径:" + exportPath);
			
		}

		string CollectNodes()
		{
			var root = nodes.Find(p => (p is RootView));
			CollectNodes(root as NodeView);
			return new XBehaviourSerialization().Serializer(root);
		}

		void CollectNodes(NodeView nodeView)
		{
			nodeView.className = nodeView.GetType().Name.Replace("View","");
			//只找到子节点的链接对象
			var children = nodeView.OutputNodes<NodeView>("child");
			if (children == null || children.Count == 0) return;
			
			nodeView.Children.Clear();
			children = children.OrderBy(p => p.position.y).ToList();
			foreach (var child in children)
			{
				nodeView.Children.Add(child);
				CollectNodes(child);
				
			}
		}
		[ContextMenu("测试读取")]
		void TestReader()
		{
			string path = "C:/Users/GuoWY/Desktop/BT/Assets/Scripts/Hotfix/XBehaviour/test.yaml";
			var input = new StringReader(File.ReadAllText(path));
			YamlStream yaml = new YamlStream();
			yaml.Load(input);
			Debug.LogError(yaml);
			YamlMappingNode mapping = (YamlMappingNode)yaml.Documents[0].RootNode;
			var rootNode = yaml.Documents[0].RootNode;
			//var parsedObject = ParseNode(rootNode);
			//Debug.LogError(parsedObject);
		}
		
		
	}
}
