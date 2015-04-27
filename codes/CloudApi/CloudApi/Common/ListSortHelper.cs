using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using FreeSun.FS_SMISCloud.Server.CloudApi.Areas.Sensor.Controllers;
using log4net;
using Microsoft.CSharp;

namespace FreeSun.FS_SMISCloud.Server.CloudApi.Common
{
    public class ListSortHelper<T> : MarshalByRefObject
    {
        private Assembly _assembly;

        public void LoadSortScript(string sourceFile)
        {
            var codeProvider = new CSharpCodeProvider();
            var cp = new CompilerParameters
            {
                GenerateExecutable = false,
                GenerateInMemory = true,
                IncludeDebugInformation = false
            };
            cp.ReferencedAssemblies.Add(Assembly.GetExecutingAssembly().Location);
            var cr = codeProvider.CompileAssemblyFromFile(cp, sourceFile);
            if (cr.Errors.Count > 0)
            {
                // Display compilation errors.
                Console.WriteLine("\tErrors building {0} into {1}",
                    sourceFile, cr.PathToAssembly);
                foreach (CompilerError ce in cr.Errors)
                {
                    Console.WriteLine("  {0}", ce.ToString());
                    Console.WriteLine();
                }
                _assembly = null;
            }
            else
            {
                Console.WriteLine("\tSource {0} built into {1} successfully.",
                    sourceFile, cr.PathToAssembly);
                Console.WriteLine("\t{0} temporary files created during the compilation.",
                    cp.TempFiles.Count.ToString());
                _assembly = cr.CompiledAssembly;
            }
        }

        public void Sort(ref List<T> list)
        {
            Type t = typeof (IComparer<T>);
            if (!t.IsInterface) throw new ArgumentException("Not an interface.", "interfaceType");
            var temp = _assembly.GetTypes()
                .Where(t.IsAssignableFrom)
                .ToList();
            if (temp.Count >0 )
            {
                var cp = (IComparer<T>)Activator.CreateInstance(temp[0]);
                list.Sort(cp);
            }
        }
    }
}