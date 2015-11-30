using System.IO;
using Microsoft.Scripting.Hosting;
using System;

namespace PdfBatchEdit
{
    public class Script
    {
        string path;
        string name;
        ScriptSource source;
        CompiledCode compiledCode;

        public Script(ScriptEngine engine, string path)
        {
            this.path = path;
            name = Path.GetFileNameWithoutExtension(path);
            source = engine.CreateScriptSourceFromFile(path);
            compiledCode = source.Compile();
        }

        public void Execute()
        {
            try
            {
                compiledCode.Execute();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception during execution of the '{0}' script:", name);
                Console.WriteLine("\t{0}", e.Message);
            }
        }

        public string Name
        {
            get { return name; }
        }
    }
}
