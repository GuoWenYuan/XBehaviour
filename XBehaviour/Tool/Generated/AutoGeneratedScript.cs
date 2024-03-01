﻿#if UNITY_EDITOR

using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace XBehaviour.Tool
{
    public class AutoGeneratedScript
    {
        private IGenerated _generated;
        private ScriptGeneratedData _data;
        
        public AutoGeneratedScript(IGenerated _generated,string generatedPath,ScriptGeneratedData data)
        {
            this._generated = _generated;
            _generated.GeneratedPath = generatedPath;
            _data = data;
        }
        
        /// <summary>
        /// 开始创建
        /// </summary>
        public void Build()
        {
            //脚本模板
            string template = File.ReadAllText(_generated.TemplatePath);
            Type type = _generated.ParserType(_data);
            if (_generated.ParserCondition(type))
            {
                Debug.LogError($"已存在该节点:{_data.className}");
                return;
            }

            string temp = _generated.ParserScripts(template, _data,out string className);
            string creatorPath = _generated.GeneratedPath;
            File.WriteAllText(creatorPath  + "/" + className + ".cs",temp);
            _generated.OnBuildEnd();
        }
    }
}


#endif
