using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using XBehaviour.Runtime;
using XNode;
using YamlDotNet.RepresentationModel;

namespace XBehaviour.Editor
{
	
	[CreateAssetMenu(fileName = "NewXBehaviour",menuName = "XBehaviour/NewBehaviourTree")]
	public class XBehaviourNodeGraph : NodeGraph {

		[ContextMenu("导出行为树")]
		void Export()
		{
			//TODO:执行导出行为树操作
			CollectNodes();
		}

		void CollectNodes()
		{
			var root = nodes.Find(p => p.GetType() == typeof(RootView));
			CollectNodes(root as NodeView);
			Debug.LogError(new XBehaviourSerialization().Serializer(root));
		}

		void CollectNodes(NodeView nodeView)
		{
			nodeView.className = nodeView.GetType().Name.Replace("View","");
			Debug.LogError(nodeView.className);
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
			var parsedObject = ParseNode(rootNode);
			Debug.LogError(parsedObject);
		}
		
		private static object ParseNode(YamlNode node)
		{
			switch (node)
			{
				case YamlScalarNode scalarNode:
					return ParseScalar(scalarNode);
				case YamlSequenceNode sequenceNode:
					return ParseSequence(sequenceNode);
				case YamlMappingNode mappingNode:
					return ParseMapping(mappingNode);
				default:
					return null;
			}
		}

		private static object ParseScalar(YamlScalarNode scalarNode)
		{
			// 这里可以根据需要处理不同类型的标量值
			if (int.TryParse(scalarNode.Value, out int intValue))
			{
				return intValue;
			}
			else if (bool.TryParse(scalarNode.Value, out bool boolValue))
			{
				return boolValue;
			}
			else
			{
				return scalarNode.Value;
			}
		}

		private static List<object> ParseSequence(YamlSequenceNode sequenceNode)
		{
			var list = new List<object>();
			foreach (var node in sequenceNode.Children)
			{
				list.Add(ParseNode(node));
			}
			return list;
		}

		private static object ParseMapping(YamlMappingNode mappingNode)
		{
			/*
			var result = new Dictionary<string, object>();
			foreach (var entry in mappingNode.Children)
			{
				/*
				result[entry.Key.ToString()] = ParseNode(entry.Value);
				if (entry.Value is YamlSequenceNode sequenceNode)
				{
					foreach (var VARIABLE in sequenceNode.Children)
					{
						Debug.LogError(VARIABLE.NodeType.ToString() + " =>" + VARIABLE);	
					}
					
				}
				
				Debug.LogError(entry.Key);
			}
			// 根据result中的className创建相应的对象
			// 注意：这里需要根据实际情况实现对象的创建和属性赋值
			return result;
		
			*/

			return null;
		}
	}
}
