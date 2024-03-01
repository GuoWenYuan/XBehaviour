using System.Collections.Generic;
using UnityEngine;
using YamlDotNet.RepresentationModel;

namespace XBehaviour.Runtime
{
    public class VectorParser : IParser
    {
        public object Decoding(YamlNode yamlNode)
        {
            Vector3 vector3 = new Vector3();
            Dictionary<string, string> propertyValues = new();
            Dictionary<string, YamlMappingNode> mappingValues = new();
            Dictionary<string, YamlSequenceNode> sequenceValues = new();
            foreach (var entry in (YamlMappingNode)yamlNode)
            {
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
            DecodingScalar(ref vector3,propertyValues);
            DecodingSequence(vector3,sequenceValues);
            DecodingMapping(vector3,mappingValues);
            return vector3;

        }
        
        private void DecodingScalar(ref Vector3 vector3, Dictionary<string, string> propertyValues)
        {
            vector3.x = float.Parse(propertyValues["x"]);
            vector3.y = float.Parse(propertyValues["y"]);
            vector3.z = float.Parse(propertyValues["z"]);

        }

        private void DecodingSequence(Vector3 vector3,Dictionary<string, YamlSequenceNode> sequenceValues)
        {
           
        }

        private void DecodingMapping(Vector3 vector3,Dictionary<string, YamlMappingNode> mappingValues)
        {
           
        }

    }
}