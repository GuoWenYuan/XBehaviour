using System;
using System.Collections.Generic;
using System.IO;
using Framework.Resource;
using UnityEngine;
using YamlDotNet.RepresentationModel;
using YooAsset;

namespace XBehaviour.Runtime
{
    public static class ParsersCollection
    {
        static readonly Dictionary<string,IParser> parsers = new ();
        static bool loaded = false;
        /// <summary>
        /// 转换标记
        /// </summary>
        private static readonly string parserFlag = "className";

        public static void Collection()
        {
            if (loaded) return;
			parsers.Add("Parallel",new ParallelParser());
			parsers.Add("Root",new RootParser());
			parsers.Add("Selector",new SelectorParser());
			parsers.Add("Task",new TaskParser());
			parsers.Add("Vector",new VectorParser());
			parsers.Add("CharacterMoveTask",new CharacterMoveTaskParser());
			parsers.Add("CharacterRoot",new CharacterRootParser());
			parsers.Add("CharacterRotationTask",new CharacterRotationTaskParser());
			parsers.Add("ConditionDamageTarget",new ConditionDamageTargetParser());
			parsers.Add("Cooldown",new CooldownParser());
			parsers.Add("Repeat",new RepeatParser());
			parsers.Add("SkillExerciseTask",new SkillExerciseTaskParser());
			parsers.Add("SkillStudyTask",new SkillStudyTaskParser());
			parsers.Add("SkillStudyUseSkill",new SkillStudyUseSkillParser());
			parsers.Add("SkillStudyWait",new SkillStudyWaitParser());
			parsers.Add("WaitActorCreator",new WaitActorCreatorParser());
			parsers.Add("WaitPlayerUseSkill",new WaitPlayerUseSkillParser());

            
            loaded = true;
        }

    

        public static IParser GetParser(string name)
        {
            if (parsers.TryGetValue(name,out var parser))
            {
                return parser;
            }
            
            return null;
        }
        
        public static T Parser<T>(string configPath) where T : Root
		{
			Collection();
			var loadAsset = ResourceManager.Instance.LoadAssetSync<TextAsset>(configPath, ResourceManager.ResourceType.eConfig);
			if (loadAsset.Status == EOperationStatus.Failed)
			{
				return default(T);
			}
			var input = loadAsset.GetAssetObject<TextAsset>().text;
			YamlStream yaml = new YamlStream();
			yaml.Load(new StringReader(input));
			YamlMappingNode mapping = (YamlMappingNode)yaml.Documents[0].RootNode;
			T root = Parser<T>(mapping);
			//在这里统一赋值Root 与 Creat
			//遍历树的所有子节点
			XBehaviourFactory.RecursionTraversal(root, (node) =>
			{
				node.Root = root;
				node.Create();
			});
			return root;
		}
      
        
        /// <summary>
        /// 开始转换
        /// </summary>
        public static T Parser<T>(YamlNode rootNode)
        {
            string name = rootNode.GetValueFromNode(parserFlag);
            return (T)GetParser(name).Decoding(rootNode);
        }

        public static void Dispose()
        {
            parsers.Clear();
            loaded = false; // Clean
        }

    }
}