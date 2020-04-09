using System;
using CSScriptLibrary;

namespace ECSEngine.Managers
{
    public sealed class ScriptManager : Manager<ScriptManager>
    {
        public ScriptManager()
        {
            var scriptPath = "Content/Scripts/TestScript.cs";
            
            CSScript.EvaluatorConfig.Engine = EvaluatorEngine.Mono;
            CSScript.GlobalSettings.UseAlternativeCompiler = @"%CSSCRIPT_DIR%\Lib\CSSCodeProvider.v4.6.dll"; 

            Debug.Log($"Using engine {CSScript.EvaluatorConfig.Engine}");
            Debug.Log($"Loading script {scriptPath}");

            dynamic testScript = CSScript.Evaluator.LoadFile(scriptPath);

            Debug.Log($"Number default value is {testScript?.Number}");

            testScript?.Run();
        }
    }
}