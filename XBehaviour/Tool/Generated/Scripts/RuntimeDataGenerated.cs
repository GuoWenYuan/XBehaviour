#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using XBehaviour.Runtime;
using YamlDotNet.Serialization;
using Utils = XBehaviour.Tool.Utils;

namespace XBehaviour.Tool
{
    /// <summary>
    /// 运行时data数据生成对象
    /// </summary>
    public class RuntimeDataGenerated : IGenerated
    {
        

        public Assembly Assembly => Assembly.GetAssembly(typeof(IParser));
        public string TemplatePath => Utils.TemplatePath.RuntimeNodeData;
        public string GeneratedPath { get; set; }
        
         private Assembly EditorDataAssembly => Assembly.GetAssembly(typeof(IParser));
        
        private List<string> _baseFieldNames;
        
        private List<string> _skipFieldNames = new List<string>()
        {
            "className",
            "Children",
        };
        
        public Type ParserType(ScriptGeneratedData data)
        {
            return Type.GetType("XBehaviour.Runtime." + data.className);
        }

        public string ParserScripts(string template, ScriptGeneratedData data, out string className)
        {
            _baseFieldNames = new List<string>();
            Type editorDataType = EditorDataAssembly.GetType("XBehaviour.Editor." + data.className + "View");
            StringBuilder sb = new StringBuilder();
            if (editorDataType != null)
            {
                GetBaseField(editorDataType, ref _baseFieldNames);
                FieldInfo[] fieldInfos = editorDataType.GetFields();
               
                foreach (var fieldInfo in fieldInfos)
                {
                    if (Tool.Utils.Export(fieldInfo) && !_baseFieldNames.Contains(fieldInfo.Name) && !_skipFieldNames.Contains(fieldInfo.Name))
                    {
                        string line = GetLine();
                        line = line.Replace("类型", GetTypeName(fieldInfo.FieldType));
                        line = line.Replace("变量名称", fieldInfo.Name);
                        sb.AppendLine(Utils.Retract(2) + line);
                    }
                }
            }
            //Debug.Log(sb.ToString());
            className = data.className + "Data";
            template = template.Replace("数据名称", data.className);
            //template = template.Replace("基类", baseClassName);
            template = template.Replace("节点名称", data.className);
            return template.Replace("//变量初始化内容", sb.ToString());
        }

        public bool ParserCondition(Type type)
        {
            return false;
        }


        

        private void GetBaseField(Type type,ref List<string> baseFieldNames)
        {
            if (type.BaseType == null)
            {
                return;
            }

            foreach (var field in type.BaseType.GetFields())
            {
                if (Tool.Utils.Export(field) && !baseFieldNames.Contains(field.Name))
                {
                    baseFieldNames.Add(field.Name);
                }
            }
            GetBaseField(type.BaseType,ref baseFieldNames);
        }

        
        /// <summary>
        /// 获取类型名称
        /// </summary>
        /// <returns></returns>
        private string GetTypeName(Type type)
        {
            if (type == typeof(int))
            {
                return "int";
            }
            else if (type == typeof(string))
            {
                return "string";
            }
            else if (type == typeof(bool))
            {
                return "bool";
            }
            else if (type == typeof(float))
            {
                return "float";
            }
            else if (type == typeof(string[]))
            {
                return "string[]";
            }
            else if (type == typeof(bool[]))
            {
                return "bool[]";
            }
            else if (type == typeof(float[]))
            {
                return "float[]";
            }
            else if (type == typeof(int[]))
            {
                return "int[]";
            }
            else
            {
                return type.Name;
            }
        }

        private string GetLine()
        {
            return @"public 类型 变量名称;";
        }

     

        public void OnBuildEnd()
        {
           // AssetDatabase.Refresh();
        }
    }
}

#endif
