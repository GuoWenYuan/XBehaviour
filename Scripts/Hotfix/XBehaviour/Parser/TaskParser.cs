using System;
using System.Collections.Generic;
using UnityEngine;
using XBehaviour.Runtime;
using YamlDotNet.RepresentationModel;

namespace XBehaviour.Runtime
{
    public class TaskParser : IParser
    {
        public object Decoding(YamlNode yamlNode)
        {
            Task task = new Task();
            Dictionary<string, string> propertyValues = new();
            Dictionary<string, YamlMappingNode> mappingValues = new();
            Dictionary<string, YamlSequenceNode> sequenceValues = new();
            foreach (var entry in (YamlMappingNode)yamlNode)
            {
                //Debug.LogError($"Key:{entry.Key} Value:{entry.Value}  => KeyType:{entry.Value.NodeType} => ValueType:{entry.Value.NodeType}");
                switch (entry.Value.NodeType)
                {
                    case YamlNodeType.Mapping:
                        mappingValues.Add(entry.Key.GetScalarValue(),(YamlMappingNode)entry.Value);
                        break;
                    case YamlNodeType.Scalar:
                        propertyValues.Add(entry.Key.GetScalarValue(), entry.Value.GetScalarValue());
                        break;
                    case YamlNodeType.Sequence:
                        sequenceValues.Add(entry.Key.GetScalarValue(),(YamlSequenceNode)entry.Value);
                        break;
                }
            }
            
            
            DecodingScalar(task,propertyValues);
            DecodingSequence(task,sequenceValues);
            DecodingMapping(task,mappingValues);
            return task;

        }
        
        private void DecodingScalar(Task task, Dictionary<string, string> propertyValues)
        {
            task.value = propertyValues["value"];
            
        }

        private void DecodingSequence(Task task,Dictionary<string, YamlSequenceNode> sequenceValues)
        {
           
        }

        private void DecodingMapping(Task task,Dictionary<string, YamlMappingNode> mappingValues)
        {
            task.vector = ParsersCollection.Parser<Vector3>(mappingValues["vector"]);
        }

    }
}