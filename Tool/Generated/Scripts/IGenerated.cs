#if UNITY_EDITOR

using System;
using System.Reflection;

namespace XBehaviour.Tool
{
    /// <summary>
    /// 脚本生成器接口
    /// </summary>
    public interface IGenerated
    {
        /// <summary>
        /// 脚本所在程序集
        /// </summary>
        Assembly Assembly { get; }

        /// <summary>
        /// 脚本模板路径
        /// </summary>
        string TemplatePath { get; }
        /// <summary>
        /// 生成路径
        /// </summary>
        string GeneratedPath { get; set; }


        /// <summary>
        /// 转换类型
        /// </summary>
        Type ParserType(ScriptGeneratedData data);

        /// <summary>
        /// 处理每一个ScriptGeneratedData数据源转换为脚本内容
        /// </summary>
        /// <returns></returns>
        string ParserScripts(string template,ScriptGeneratedData data,out string className);


        /// <summary>
        /// 转换为脚本的条件
        /// </summary>
        bool ParserCondition(Type type);

        /// <summary>
        /// 当创建结束
        /// </summary>
        void OnBuildEnd();
    }
}

#endif