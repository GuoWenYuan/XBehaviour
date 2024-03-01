using System;
using YamlDotNet.RepresentationModel;

namespace XBehaviour.Runtime
{
    public interface IParser
    {
        object Decoding(YamlNode yamlNode);
    }
}