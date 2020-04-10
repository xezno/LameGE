using CSScriptLibrary;
using System.IO;
using System.Reflection;

namespace ECSEngine.Managers
{
    public sealed class ScriptManager : Manager<ScriptManager>
    {
        public ScriptManager()
        {
            var scriptPath = "Content/Scripts/TestScript.cs";
            CSScript.GlobalSettings.SearchDirs += Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Content\\Scripts;";
            Debug.Log($"CSScript search: {CSScript.GlobalSettings.SearchDirs}");
            CSScript.EvaluatorConfig.Engine = EvaluatorEngine.Mono;
            CSScript.GlobalSettings.UseAlternativeCompiler = @"Content/CSScript/CSSRoslynProvider.dll";

            Debug.Log($"Using engine {CSScript.EvaluatorConfig.Engine}");
            Debug.Log($"Loading script {scriptPath}");

            dynamic testScript = CSScript.Evaluator.LoadFile(scriptPath);

            Debug.Log($"Number default value is {testScript?.Number}");

            testScript?.Run();
        }
    }
}