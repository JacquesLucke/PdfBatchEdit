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
    public class AddonManager
    {
        private ScriptEngine engine;
        public bool isSetup { get; }
        Dictionary<string, Addon> addons = new Dictionary<string, Addon>();

        public AddonManager(PdfBatchEditData data)
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

        public void LoadAddons()
        {
            addons = new Dictionary<string, Addon>();
            foreach (string path in Directory.GetFiles(AddonsDirectoryPath))
            {
                try
                {
                    Addon addon = new Addon(engine, path);
                    addons[addon.Name] = addon;
                    Console.WriteLine("Loaded addon: {0}", addon.Name);
                }
                catch
                {
                    Console.WriteLine("Couldn't load addon: {0}", Path.GetFileName(path));
                }
            }
        }

        public void ExecuteAddons()
        {
            foreach (Addon addon in addons.Values)
            {
                addon.Execute();
            }
        }

        public string AddonsDirectoryPath
        {
            get { return Path.Combine(Utils.MainDirectory, "addons"); }
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
