using CSScriptLibrary;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace ECSEngine.Managers
{
    public sealed class ScriptManager : Manager<ScriptManager>
    {
        public Dictionary<string, dynamic> ScriptList { get; } = new Dictionary<string, dynamic>();

        public ScriptManager()
        {
            Init();

            dynamic testScript = LoadScript("Content/Scripts/TestScript.cs");

            testScript?.Run();
        }

        private void Init()
        {
            CSScript.GlobalSettings.SearchDirs += Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Content\\Scripts;";
            Debug.Log($"CSScript search: {CSScript.GlobalSettings.SearchDirs}");
            CSScript.CacheEnabled = false;
            CSScript.EvaluatorConfig.Engine = EvaluatorEngine.Mono;
            CSScript.GlobalSettings.UseAlternativeCompiler = @"Content/CSScript/CSSRoslynProvider.dll";

            Debug.Log($"Using engine {CSScript.EvaluatorConfig.Engine}");
        }

        public void ReloadScript(string path)
        {
            ScriptList.Remove(path);
            LoadScript(path);
        }

        public dynamic LoadScript(string path)
        {
            Debug.Log($"Loading script {path}");

            dynamic instance = CSScript.Evaluator.LoadFile(path);
            ScriptList.Add(path, instance);

            return instance;
        }
    }
}