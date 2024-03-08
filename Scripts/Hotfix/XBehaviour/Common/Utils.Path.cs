using UnityEngine;

namespace XBehaviour.Runtime
{
    public static class XBehaviourPath
    {
        public static class ConfigPath
        {
            public static string RootPath = "Assets/GameConfig/Config/XBehaviour/";
            public static string SkillTeachingPath = RootPath + "SkillTeaching/";
        }

        public static string ToYamlPath(this string path)
        {
            return path + ".yaml";
        }
    }
}