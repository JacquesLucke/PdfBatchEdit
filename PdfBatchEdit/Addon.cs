using System.IO;
using Microsoft.Scripting.Hosting;

namespace PdfBatchEdit
{
    public class Addon
    {
        string path;
        string name;
        ScriptSource source;
        CompiledCode compiledCode;

        public Addon(ScriptEngine engine, string path)
        {
            this.path = path;
            name = Path.GetFileNameWithoutExtension(path);
            source = engine.CreateScriptSourceFromFile(path);
            compiledCode = source.Compile();
        }

        public void Execute()
        {
            compiledCode.Execute();
        }

        public string Name
        {
            get { return name; }
        }
    }
}
