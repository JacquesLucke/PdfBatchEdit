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
            ScriptScope apiScope = engine.CreateModule("PdfBatchEditAPI");
            CompiledCode compiled = CompileFile(ApiFilePath);
            apiScope.SetVariable("currentData", data);
            compiled.Execute(apiScope);
        }

        public void LoadScripts()
        {
            scripts = new Dictionary<string, Script>();
            foreach (string path in Directory.GetFiles(ScriptsDirectoryPath))
            {
                try
                {
                    Script script = new Script(engine, path);
                    scripts[script.Name] = script;
                    Console.WriteLine("Loaded addon: {0}", script.Name);
                }
                catch
                {
                    Console.WriteLine("Couldn't load addon: {0}", Path.GetFileName(path));
                }
            }
        }

        public void ExecuteScripts()
        {
            foreach (Script addon in scripts.Values)
            {
                addon.Execute();
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
