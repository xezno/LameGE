using CSScriptLibrary;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Engine.Managers.Scripting
{
    public sealed class ScriptManager : Manager<ScriptManager>
    {
        public Dictionary<string, dynamic> ScriptList { get; } = new Dictionary<string, dynamic>();

        public ScriptManager()
        {
            Init();
        }

        private void Init()
        {
            CSScript.GlobalSettings.SearchDirs += Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Content\\Scripts;";
            DebugUtils.Logging.Log($"CSScript search dirs: {CSScript.GlobalSettings.SearchDirs}");
            CSScript.CacheEnabled = false;
            CSScript.EvaluatorConfig.Engine = EvaluatorEngine.Mono;

            DebugUtils.Logging.Log($"Using engine {CSScript.EvaluatorConfig.Engine}");
            CSScript.GlobalSettings.UseAlternativeCompiler = @"Content/CSScript/CSSRoslynProvider.dll";

            using var streamReader = new StreamReader("Content/Scripts/scripts.json");

            var config = JsonConvert.DeserializeObject<ScriptConfig>(streamReader.ReadToEnd());

            foreach (var script in config.ScriptList)
            {
                DebugUtils.Logging.Log($"Loaded script {script.Path}");
                if (script.EntryPoint == null)
                {
                    LoadScript("Content/Scripts/" + script.Path);
                }
                else
                {
                    dynamic scriptInstance = LoadScript("Content/Scripts/" + script.Path);
                    Type scriptInstanceType = scriptInstance.GetType();
                    var entryMethod = scriptInstanceType.GetMethod(script.EntryPoint);

                    if (entryMethod == null)
                    {
                        DebugUtils.Logging.Log($"Entry point {script.EntryPoint} not found!", DebugUtils.Logging.Severity.High);
                        return;
                    }
                    entryMethod.Invoke((object)scriptInstance, new object[] { });

                    DebugUtils.Logging.Log($"Entry point {script.EntryPoint}");
                }
            }
        }

        public void ReloadScript(string path)
        {
            ScriptList.Remove(path);
            LoadScript(path);
        }

        public dynamic LoadScript(string path)
        {
            DebugUtils.Logging.Log($"Loading script {path}");

            dynamic instance = CSScript.Evaluator.LoadFile(path);
            ScriptList.Add(path, instance);

            return instance;
        }
    }
}