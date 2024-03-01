using System;
using System.Collections.Generic;
using XBehaviour.Runtime;
using YamlDotNet.RepresentationModel;

namespace XBehaviour.Runtime
{
    public class RootParser : IParser
    {
        public object Decoding(YamlNode yamlNode)
        {
            Root root = new Root();
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
            DecodingScalar(root,propertyValues);
            DecodingSequence(root,sequenceValues);
            DecodingMapping(root,mappingValues);
            return root;

        }
        
        private void DecodingScalar(Root root, Dictionary<string, string> propertyValues)
        {
            root.RunningType = Enum.Parse<RunningType>(propertyValues["RunningType"]);
            root.Name = propertyValues["name"];
        }

        private void DecodingSequence(Root root,Dictionary<string, YamlSequenceNode> sequenceValues)
        {
            foreach (var entry in sequenceValues)
            {
                switch (entry.Key)
                {
                    case "Children":
                        foreach (var sequenceNode in entry.Value)
                        {
                            var nodeMappingNode = (YamlMappingNode)sequenceNode;
                            root.AddChild(ParsersCollection.Parser<INode>(nodeMappingNode));
                        }
                        break;
                    
                }   
            }
        }

        private void DecodingMapping(Root root,Dictionary<string, YamlMappingNode> mappingValues)
        {
            
        }




    }
}