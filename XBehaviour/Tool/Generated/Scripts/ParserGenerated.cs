#if UNITY_EDITOR


using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;
using XBehaviour.Editor;
using YamlDotNet.Serialization;

namespace XBehaviour.Tool
{
    /// <summary>
    /// 节点的Parser文件生成  
    /// </summary>
    public class ParserGenerated : IGenerated
    {
        public Assembly Assembly => Assembly.GetAssembly(typeof(XBehaviour.Runtime.IParser));
        public string TemplatePath => Utils.TemplatePath.ParserPath;
        public string GeneratedPath { get; set;}

        /// <summary>
        /// Parser模板路径
        /// </summary>
        private string ParserCollectionPath => Utils.TemplatePath.ParserCollectionPath;

        /// <summary>
        /// ParserCollection脚本生成路径
        /// </summary>
        private string CreatorCollectionPath => Utils.GeneratedPath.ParserCollectionPath;
        
        
        string RegisterParserLine => $"{Utils.Retract(3)}parsers.Add(节点名称,new 转换节点());";
        
        private List<string> _skipFieldNames = new List<string>()
        {
            "className",
            "Children",
        };
        
        //向ParserCollection中注册的数据内容
        private StringBuilder RegisterParserScript { get; set; }

        public Type ParserType(ScriptGeneratedData data)
        {
            return Assembly.GetType($"XBehaviour.Editor.{data.className}View");
        }

        public string ParserScripts(string template, ScriptGeneratedData data, out string className)
        {
            Type type = ParserType(data);
            string parserTemplate = template;
            parserTemplate = parserTemplate.Replace("节点名称", data.className);
            parserTemplate = parserTemplate.Replace("节点变量", data.className.ToLower());
            StringBuilder scalarParser = new();
            StringBuilder sequenceParser = new();
            StringBuilder mappingParser = new();
            FieldInfo[] fieldInfos = type.GetFields();
            foreach (var field in fieldInfos)
            {
                if (!Utils.Export(field) || _skipFieldNames.Contains(field.Name))
                {
                    continue;
                }
                
                GetParserLine(field, data, scalarParser, sequenceParser, mappingParser);
            }
            parserTemplate = parserTemplate.Replace("//初始化标量数据", scalarParser.ToString());
            parserTemplate = parserTemplate.Replace("//初始化列表数据", sequenceParser.ToString());
            parserTemplate = parserTemplate.Replace("//初始化映射数据", mappingParser.ToString());
            className = data.className + "Parser";
            return parserTemplate;
        }
        
        private void GetParserLine(FieldInfo fieldInfo, ScriptGeneratedData data, StringBuilder scalarParser, StringBuilder sequenceParser, StringBuilder mappingParser)
        {
            if (fieldInfo.FieldType.IsArray)
            {
                sequenceParser.AppendLine(GetSequenceParserLine(fieldInfo,data));
            }
            else if (fieldInfo.FieldType.IsClass && fieldInfo.FieldType != typeof(string))
            {
                mappingParser.AppendLine(GetMappingParserLine(fieldInfo,data));
            }
            else
            {
                scalarParser.AppendLine(GetScalarParserLine(fieldInfo,data));
            }
        }

        private string GetScalarParserLine(FieldInfo fieldInfo ,ScriptGeneratedData data)
        {
            StringConverterValueType(fieldInfo,out var value);
            string line = $"{Utils.Retract(3)}{data.className.ToLower()}.{fieldInfo.Name} = {value}";
            return line;
        }

        private string GetSequenceParserLine(FieldInfo fieldInfo, ScriptGeneratedData data)
        {
            string line = GeSequenceLine(fieldInfo, data);
            line = line.Replace("节点名称", data.className.ToLower());
            line = line.Replace("变量命名成String", Utils.ToString(fieldInfo.Name));
            line = line.Replace("变量名称", fieldInfo.Name);
            line = line.Replace("类型名称", ConvertSequenceType(fieldInfo));
            StringConverterValueType(fieldInfo, out var value);
            line = line.Replace("类型转换",value );
            return line;
        }
        
        private string GetMappingParserLine(FieldInfo fieldInfo,ScriptGeneratedData data)
        {
            string line = $"{data.className.ToLower()}.{fieldInfo.Name} = ParsersCollection.Parser<{fieldInfo.FieldType.Name}>(mappingValues[\"{fieldInfo.Name}\"]);";
            return line;
        }
        
        public bool ParserCondition(Type type)
        {
            return false;
        }

        private string ConvertSequenceType(FieldInfo fieldInfo)
        {
            Type elementType = fieldInfo.FieldType.GetElementType();
            if (elementType == typeof(int))
            {
                return "int";
            }
            else if (elementType == typeof(float))
            {
                return "float";
            }
            else if (elementType == typeof(bool))
            {
                return "bool";
            }
            else if (elementType == typeof(string))
            {
                return "string";
            }
            else if (elementType.IsEnum || elementType.IsClass)
            {
                return $"{elementType.Name}";
            }
            else
            {
                throw new NotSupportedException($"The type {elementType.Name} is not supported.");
            }
        }
        

        /// <summary>
        /// 转换类型->值类型
        /// </summary>
        private bool StringConverterValueType(FieldInfo fieldInfo, out string lineValue)
        {
            var type = fieldInfo.FieldType;
            if (typeof(string) == type || typeof(string[]) == type)
            {
                lineValue = "propertyValues[值名称]";
            }
            else if (typeof(int) == type || typeof(int[]) == type)
            {
                lineValue = "int.Parse(propertyValues[值名称])";
            }
            else if (typeof(float) == type || typeof(float[]) == type)
            {
                lineValue = "float.Parse(propertyValues[值名称])";
            }
            else if (typeof(bool) == type || typeof(bool[]) == type)
            {
                lineValue = "bool.Parse(propertyValues[值名称])";
            }
            else if (type.IsEnum || type.IsArray && type.GetElementType().IsEnum)
            {
                lineValue = GetArrayIsEnum(fieldInfo,out var enumTypeType) ? $"Enum.Parse<{enumTypeType.Name}>(sequenceNode.GetScalarValue())" : $"Enum.Parse<{type.Name}>(propertyValues[值名称])";
            }
            else if (fieldInfo.FieldType.IsClass && !fieldInfo.FieldType.IsArray && !(typeof(string) == type || typeof(string[]) == type))
            {
                lineValue = $"ParsersCollection.Parser<{type.Name}>(mappingValues[值名称])";
            }
            else
            {
                lineValue = null;
                return false;
            }

          
            if (fieldInfo.FieldType.IsArray)
            {
                lineValue = lineValue.Replace("mappingValues[值名称]", "sequenceNode.GetScalarValue()");
                lineValue = lineValue.Replace("propertyValues[值名称]", "sequenceNode.GetScalarValue()");
                return true;
            }
          
            lineValue += ";";
            lineValue = lineValue.Replace("值名称", Utils.ToString(fieldInfo.Name));
            return true;
        }
        
        private bool GetArrayIsEnum(FieldInfo fieldInfo,out Type enumType)
        {
            if (fieldInfo != null)
            {
                Type fieldType = fieldInfo.FieldType;
                if (fieldType.IsArray && fieldType.GetElementType().IsEnum)
                {
                    enumType = fieldType.GetElementType();
                    return true;
                }
            }

            enumType = null;
            return false;
        }

        private string GeSequenceLine(FieldInfo fieldInfo, ScriptGeneratedData data)
        {
            return @"
                case 变量命名成String:
                        节点名称.变量名称 = new 类型名称[entry.Value.Count()];
                        index = 0;
                        foreach (var sequenceNode in entry.Value)
                        {
                            节点名称.变量名称[index] = 类型转换;
                            index++;
                        }
                        break;
                ";
        }

       

        public void OnBuildEnd()
        {
            string parserCollectionText = File.ReadAllText(ParserCollectionPath);
            parserCollectionText = parserCollectionText.Replace("//注册数据", GetParserLine());
            File.WriteAllText(Utils.GeneratedPath.ParserCollectionPath, parserCollectionText);
        }

        private string GetParserLine()
        {
           //获取所有继承自IParser的类,并且注册到ParserCollection中
            StringBuilder parsers = new();
            Type[] types = Assembly.GetTypes();
            foreach (var type in types)
            {
                if (type.IsClass && !type.IsAbstract && type.GetInterface("IParser") != null)
                {
                    string line = RegisterParserLine.Replace("节点名称", Utils.ToString(type.Name.Replace("Parser", "")));
                    line = line.Replace("转换节点", type.Name);
                    parsers.AppendLine(line);

                }
            }
            return parsers.ToString();
        }
    }
}
#endif