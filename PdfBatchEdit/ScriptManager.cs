using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;

namespace PdfBatchEdit
{
    public class ScriptManager
    {
        private ScriptEngine engine;
        public bool isSetup { get; }
        Dictionary<string, Script> scripts = new Dictionary<string, Script>();

        public ScriptManager(PdfBatchEditData data)
        {
            try
            {
                engine = Python.CreateEngine();
                SetupAPI(data);
                isSetup = true;
            }
            catch
            {
                isSetup = false;
            }
        }

        private void SetupAPI(PdfBatchEditData data)
        {
            ScriptScope apiScope = engine.CreateModule("pdfedit");
            CompiledCode compiled = CompileFile(ApiFilePath);
            apiScope.SetVariable("currentData", data);
            compiled.Execute(apiScope);
        }

        public void LoadScripts()
        {
            scripts = new Dictionary<string, Script>();
            EnsureScriptsDirectory();
            foreach (string path in Directory.GetFiles(ScriptsDirectoryPath))
            {
                try
                {
                    Script script = new Script(engine, path);
                    scripts[script.Name] = script;
                    Console.WriteLine("Loaded script: {0}", script.Name);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Couldn't load script: {0}", Path.GetFileName(path));
                    Console.WriteLine("\t{0}", e.Message);
                }
            }
        }

        public void ExecuteScripts()
        {
            foreach (Script script in scripts.Values)
            {
                script.Execute();
            }
        }

        public void ExecuteScript(string name)
        {
            if (scripts.ContainsKey(name))
            {
                scripts[name].Execute();
            }
            else
            {
                Console.WriteLine($"Script '{name}' not found");
            }
        }

        public void EnsureScriptsDirectory()
        {
            if (!Directory.Exists(ScriptsDirectoryPath))
            {
                Directory.CreateDirectory(ScriptsDirectoryPath);
            }
        }

        public string ScriptsDirectoryPath
        {
            get { return Path.Combine(Utils.MainDirectory, "scripts"); }
        }

        private string ApiFilePath
        {
            get { return Path.Combine(Utils.MainDirectory, "PythonAPI.py"); }
        }

        private CompiledCode CompileFile(string path)
        {
            ScriptSource source = engine.CreateScriptSourceFromFile(path);
            CompiledCode compiledCode = source.Compile();
            return compiledCode;
        }
    }
}
