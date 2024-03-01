using System;
using System.Collections.Generic;
using System.Linq;
using XBehaviour.Runtime;
using YamlDotNet.RepresentationModel;

namespace XBehaviour.Runtime
{
    public class ParallelParser : IParser
    {
         public object Decoding(YamlNode yamlNode)
        {
            Parallel parallel = new Parallel();
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
            DecodingScalar(parallel,propertyValues);
            DecodingSequence(parallel,sequenceValues);
            DecodingMapping(parallel,mappingValues);
            return parallel;

        }
        
        private void DecodingScalar(Parallel parallel, Dictionary<string, string> propertyValues)
        {
            parallel.state = Enum.Parse<Parallel.ParallelState>(propertyValues["state"]);
            parallel.returnStatue = Enum.Parse<Parallel.ChildReturnStatue>(propertyValues["returnStatue"]);
            parallel.Name = propertyValues["name"];
            
           
        }

        private void DecodingSequence(Parallel parallel,Dictionary<string, YamlSequenceNode> sequenceValues)
        {
            foreach (var entry in sequenceValues)
            {
                switch (entry.Key)
                {
                    case "Children":
                        
                        foreach (var sequenceNode in entry.Value)
                        {
                            var nodeMappingNode = (YamlMappingNode)sequenceNode;
                            parallel.AddChild(ParsersCollection.Parser<INode>(nodeMappingNode));
                        }
                        break;
                    
                }   
            }
        }

        private void DecodingMapping(Parallel parallel,Dictionary<string, YamlMappingNode> mappingValues)
        {
            
        }

    }
}