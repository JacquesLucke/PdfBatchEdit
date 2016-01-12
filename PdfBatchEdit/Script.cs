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
                ScriptScope scope = source.Engine.CreateScope();
                compiledCode.Execute(scope);
                dynamic executeFunction;
                bool executeFunctionExists = scope.TryGetVariable("Execute", out executeFunction);
                if (executeFunctionExists)
                {
                    executeFunction(Utils.GetArgumentsDictionary());
                    Console.WriteLine($"Executed script: '{name}'");
                }
                else
                {
                    Console.WriteLine("The script has to have a 'Execute(args)' function");
                }
                
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception during execution of the '{name}' script:");
                Console.WriteLine("\t{0}", e.Message);
            }
        }

        public string Name
        {
            get { return name; }
        }
    }
}
