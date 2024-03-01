using System;
using System.IO;
using UnityEngine;
using YamlDotNet.RepresentationModel;
using XBehaviour.Runtime;

namespace Hotfix
{
    public class Test : MonoBehaviour
    {
        private void Start()
        {
            string path = "Assets/Scripts/Hotfix/XBehaviour/test.yaml";
            var input = new StringReader(File.ReadAllText(path));
            YamlStream yaml = new YamlStream();
            yaml.Load(input);
            Debug.LogError(yaml);
            YamlMappingNode mapping = (YamlMappingNode)yaml.Documents[0].RootNode;
            ParsersCollection.Collection(); 
            Root root = ParsersCollection.Parser<Root>(mapping);
            Debug.Log(root);
        }
    }
}