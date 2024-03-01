using System;
using System.Collections.Generic;
using YamlDotNet.RepresentationModel;

namespace XBehaviour.Runtime
{
    public class SelectorParser : IParser
    {
         public object Decoding(YamlNode yamlNode)
        {
            Selector selector = new Selector();
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
            DecodingScalar(selector,propertyValues);
            DecodingSequence(selector,sequenceValues);
            DecodingMapping(selector,mappingValues);
            return selector;

        }
        
        private void DecodingScalar(Selector selector, Dictionary<string, string> propertyValues)
        {
            selector.selectionState = Enum.Parse<Selector.SelectionState>(propertyValues["selectionState"]);
            selector.Name = propertyValues["name"];
        }

        private void DecodingSequence(Selector selector,Dictionary<string, YamlSequenceNode> sequenceValues)
        {
            foreach (var entry in sequenceValues)
            {
                switch (entry.Key)
                {
                    case "Children":
                        foreach (var sequenceNode in entry.Value)
                        {
                            var nodeMappingNode = (YamlMappingNode)sequenceNode;
                            selector.AddChild(ParsersCollection.Parser<INode>(nodeMappingNode));
                        }
                        break;
                    
                }   
            }
        }

        private void DecodingMapping(Selector root,Dictionary<string, YamlMappingNode> mappingValues)
        {
            
        }

    }
}