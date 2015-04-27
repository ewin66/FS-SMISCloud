using System;
using System.CodeDom.Compiler;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using log4net;
using Microsoft.CSharp;
using Newtonsoft.Json;

namespace FS.DynamicScript
{
    internal class DepConfig
    {
        public List<string> DependList { get; set; }
    }

    public class DynamicScriptConfig
    {
        public AppDomain Domain { get; set; }
        public CrossDomainCompiler Compiler { get; set; }
        public FileSystemWatcher ChangeWatcher { get; set; }
        public string DepConfigFile { get; set; }
        public string Directory { get; set; }
    }

    public class CrossDomainCompiler : MarshalByRefObject
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (CrossDomainCompiler));

        private static readonly Dictionary<string, DynamicScriptConfig> DynamicScriptConfigs =
            new Dictionary<string, DynamicScriptConfig>();

        private static readonly object Lock = new object();
        private Assembly _assembly;


        public override object InitializeLifetimeService()
        {
            //Remoting对象 无限生存期
            return null;
        }

        private bool CompileAssembly(string depDescFile, params string[] sourceFile)
        {
            _assembly = null;
            if (!sourceFile.Any())
            {
                throw new FileNotFoundException("No file to compile");
            }
            var codeProvider = new CSharpCodeProvider();
            var cp = new CompilerParameters
            {
                GenerateExecutable = false,
                GenerateInMemory = true,
                IncludeDebugInformation = false
            };
            cp.ReferencedAssemblies.Add(Assembly.GetExecutingAssembly().Location);
            var deps = JsonConvert.DeserializeObject<DepConfig>(File.ReadAllText(depDescFile));
            foreach (var dep in deps.DependList)
            {
                cp.ReferencedAssemblies.Add(dep);
            }
            var cr = codeProvider.CompileAssemblyFromFile(cp, sourceFile);

            if (cr.Errors.Count > 0)
            {
                Logger.ErrorFormat("Errors building {0} into {1}",
                    sourceFile, cr.PathToAssembly);
                foreach (CompilerError ce in cr.Errors)
                {
                    Logger.ErrorFormat("  {0}", ce);
                }
                return false;
            }
            
            Logger.InfoFormat("Source's files {0} built into {1} successfully.",
                sourceFile, (new FileInfo(depDescFile)).DirectoryName);
            Logger.DebugFormat("{0} temporary files created during the compilation.",
                cp.TempFiles.Count);
            Logger.InfoFormat("{0} Sensor files compilated sucessed.", sourceFile.Length);
            _assembly = cr.CompiledAssembly;
            return true;
        }

        private void RawCall(Type interfaceType, string method, ref object[] constructorParas, ref object[] methodParas)
        {
            if (!interfaceType.IsInterface && !interfaceType.IsAbstract)
            {
                throw new ArgumentException("Not an abstract or interface.", "interfaceType");
            }
            if (_assembly == null)
                throw new NullReferenceException("Assembly is null");
            var temp = _assembly.GetTypes()
                .Where(interfaceType.IsAssignableFrom)
                .ToList();
            //var temp = _assembly.GetTypes().ToList();
            if (temp.Count <= 0)
                throw new NotImplementedException(string.Format("No implements of {0}", interfaceType.FullName));
            //
            var impl = Activator.CreateInstance(temp[0], constructorParas);
            var magicMethod = interfaceType.GetMethod(method);
            magicMethod.Invoke(impl, methodParas);
        }

        private void RawCall(Type interfaceType, string scriptclass, string method, ref object[] constructorParas,
            ref object[] methodParas, string[] propertynames, ref object[] magicperInfos)
        {
            if (!interfaceType.IsInterface && !interfaceType.IsAbstract)
            {
                throw new ArgumentException("Not an abstract or interface.", "interfaceType");
            }
            if (_assembly == null)
                throw new NullReferenceException("Assembly is null");
            var temp = _assembly.GetTypes()
                .Where(interfaceType.IsAssignableFrom)
                .ToList();
            //var temp = _assembly.GetTypes().ToList();
            if (temp.Count <= 0)
                throw new NotImplementedException(string.Format("No implements of {0}", interfaceType.FullName));

            Type type = temp.Find(t => t.Name == scriptclass);
            if (type != null)
            {
                var impl = Activator.CreateInstance(type, constructorParas);
                var magicMethod = interfaceType.GetMethod(method);
                PropertyInfo[] magicpropertyInfos = interfaceType.GetProperties();

                if (magicpropertyInfos != null && magicpropertyInfos.Length > 0 && propertynames != null)
                {
                    magicperInfos = new object[propertynames.Length];
                    for (int i = 0; i < propertynames.Length; i++)
                    {
                        try
                        {
                            PropertyInfo magicperInfo =
                                magicpropertyInfos.FirstOrDefault(p => p.Name == propertynames[i]);
                            if (magicperInfo != null)
                            {
                                var v = magicperInfo.GetValue(impl, null);
                                magicperInfos[i] = v;
                            }
                            else
                            {
                                magicperInfos[i] = null;
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.InfoFormat(ex.Message);
                            magicperInfos[i] = null;
                        }
                    }
                }
                else
                {
                    magicperInfos = null;
                }
                magicMethod.Invoke(impl, methodParas);
            }
        }



        #region 每次重新编译的方式，性能相对较低

        public static void Call(string scriptFile, string depDescFile, Type interfaceType, string method)
        {
            var cp = new object[] {};
            var mp = new object[] {};
            Call(scriptFile, depDescFile, interfaceType, method, ref cp, ref mp);
        }

        public static void Call(string scriptFile, string depDescFile, Type interfaceType, string method,
            ref object[] constructorParas)
        {
            var mp = new object[] {};
            Call(scriptFile, depDescFile, interfaceType, method, ref constructorParas, ref mp);
        }

        public static void Call(string scriptFile, string depDescFile, Type interfaceType, string method,
            ref object[] constructorParas,
            ref object[] methodParas)
        {
            var domain = "CrossDomainCompilerDomain-" + Guid.NewGuid();
            var appDomain = AppDomain.CreateDomain(domain, new Evidence(), new AppDomainSetup
            {
                // !! 作为动态库使用时，需要指定自身所在目录
                ApplicationBase = AppDomain.CurrentDomain.BaseDirectory
            });
            appDomain.SetData("APP_CONFIG_FILE", AppDomain.CurrentDomain.GetData("APP_CONFIG_FILE"));
            try
            {
                Logger.DebugFormat("Ceating AppDomain: {0}", domain);
                var compiler =
                    (CrossDomainCompiler)
                        appDomain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName,
                            typeof (CrossDomainCompiler).FullName);
                if (compiler.CompileAssembly(depDescFile, scriptFile))
                {
                    compiler.RawCall(interfaceType, method, ref constructorParas, ref methodParas);
                }
                else
                {
                    var error = string.Format("Compiler {0} with {1} failed. See the detail above", scriptFile,
                        depDescFile);
                    Logger.Error(error);
                    throw new CompileErrorException(error);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Logger.DebugFormat("Unloading AppDomain: {0}", domain);
                AppDomain.Unload(appDomain);
            }
        }

        #endregion

        #region 重用方式，一次性编译整个目录，目录更新时自动刷新

        public static void LoadScript(string depDescFile, string directory)
        {
            if (!Directory.Exists(directory))
                throw new FileNotFoundException(string.Format("Directory {0} not exist.", directory));
            directory = Path.GetFullPath(directory);
            lock (Lock)
            {
                if (DynamicScriptConfigs.ContainsKey(directory))
                {
                    Cleanup(directory);
                }
                var dc = new DynamicScriptConfig
                {
                    ChangeWatcher = new FileSystemWatcher
                    {
                        Path = directory,
                        IncludeSubdirectories = false,
                        Filter = "*.cs"
                    },
                    Directory = directory,
                    DepConfigFile = depDescFile
                };

                var domain = "CrossDomainCompilerDomain-" + Guid.NewGuid();
                var appDomain = AppDomain.CreateDomain(domain, new Evidence(), new AppDomainSetup
                {
                    // !! 作为动态库使用时，需要指定自身所在目录
                    ApplicationBase = AppDomain.CurrentDomain.BaseDirectory
                });
                appDomain.SetData("APP_CONFIG_FILE", AppDomain.CurrentDomain.GetData("APP_CONFIG_FILE"));
                try
                {
                    Logger.DebugFormat("Ceating AppDomain: {0}", domain);
                    var compiler =
                        (CrossDomainCompiler)
                            appDomain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName,
                                typeof (CrossDomainCompiler).FullName);
                    var scriptFiles = Directory.GetFiles(directory, "*.cs");
                    DynamicScriptConfigs.Add(directory, dc);
                    dc.ChangeWatcher.Changed += ChangeWatcher_Changed;
                    dc.ChangeWatcher.Created += ChangeWatcher_Changed;
                    dc.ChangeWatcher.Deleted += ChangeWatcher_Changed;

                    dc.ChangeWatcher.EnableRaisingEvents = true;
                    if (!compiler.CompileAssembly(depDescFile, scriptFiles))
                    {
                        var error = string.Format("Compiler {0} with {1} failed. See the detail above", directory,
                            depDescFile);
                        Logger.Error(error);
                        throw new CompileErrorException(error);
                    }
                    dc.Domain = appDomain;
                    dc.Compiler = compiler;
                }
                catch (Exception ex)
                {
                    Logger.DebugFormat("Unloading AppDomain: {0}", domain);
                    AppDomain.Unload(appDomain);
                    throw ex;
                }
            }
        }

        private static void ChangeWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            var watcher = sender as FileSystemWatcher;
            if (watcher == null) return;

            var path = watcher.Path;
            DynamicScriptConfig dc = null;
            try
            {
                watcher.EnableRaisingEvents = false;
                lock (Lock)
                {
                    if (DynamicScriptConfigs.ContainsKey(path))
                    {
                        dc = DynamicScriptConfigs[path];
                        LoadScript(dc.DepConfigFile, dc.Directory);
                        Logger.InfoFormat("成功更新传感器适配器");
                    }
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                //reset dc
                dc = DynamicScriptConfigs[path];
                watcher = dc != null ? dc.ChangeWatcher : null;
                if (watcher != null)
                {
                    watcher.EnableRaisingEvents = true;
                }
            }
        }

        public static void Cleanup()
        {
            lock (Lock)
            {
                var keys = DynamicScriptConfigs.Keys;
                foreach (var key in keys)
                {
                    var dc = DynamicScriptConfigs[key];
                    dc.ChangeWatcher.Changed -= ChangeWatcher_Changed;
                    dc.ChangeWatcher.Created -= ChangeWatcher_Changed;
                    dc.ChangeWatcher.Deleted -= ChangeWatcher_Changed;
                    dc.ChangeWatcher.EnableRaisingEvents = false;
                    dc.ChangeWatcher.Dispose();
                    if (dc.Domain != null)
                    {
                        AppDomain.Unload(dc.Domain);
                    }
                }
                DynamicScriptConfigs.Clear();
            }
        }

        private static void Cleanup(string directory)
        {
            if (!Directory.Exists(directory)) return;
            lock (Lock)
            {
                if (DynamicScriptConfigs.ContainsKey(directory))
                {
                    var dc = DynamicScriptConfigs[directory];
                    dc.ChangeWatcher.Changed -= ChangeWatcher_Changed;
                    dc.ChangeWatcher.Created -= ChangeWatcher_Changed;
                    dc.ChangeWatcher.Deleted -= ChangeWatcher_Changed;
                    dc.ChangeWatcher.EnableRaisingEvents = false;
                    dc.ChangeWatcher.Dispose();
                    if (dc.Domain != null)
                    {
                        AppDomain.Unload(dc.Domain);
                    }
                    DynamicScriptConfigs.Remove(directory);
                }
            }
        }

        public static void Call(string directory, Type interfaceType, string scriptclass, string method)
        {
            var cp = new object[] {};
            var mp = new object[] {};
            object[] obj = null;
            Call(directory, interfaceType, scriptclass, method, ref cp, ref mp, null, ref obj);
        }

        public static void Call(string directory, Type interfaceType, string scriptclass, string method,
            ref object[] constructorParas)
        {
            var mp = new object[] {};
            object[] objs = null;
            Call(directory, interfaceType, scriptclass, method, ref constructorParas, ref mp, null, ref objs);
        }

        public static void Call(string directory, Type interfaceType, string scriptclass, string method,
            ref object[] constructorParas, ref object[] methodParas)
        {
            object[] objs = null;
            Call(directory, interfaceType, scriptclass, method, ref constructorParas, ref methodParas, null, ref objs);
        }


        public static void Call(string directory, Type interfaceType, string scriptclass, string method,
            ref object[] constructorParas,
            ref object[] methodParas, string[] propertynames, ref object[] magicperInfos)
        {
            if (!Directory.Exists(directory))
                throw new CompilerNotFoundException(string.Format("Directory {0} not exist.", directory));
            directory = Path.GetFullPath(directory);
            lock (Lock)
            {
                try
                {
                    if (DynamicScriptConfigs.ContainsKey(directory))
                    {
                        var dc = DynamicScriptConfigs[directory];
                        if (dc.Compiler != null && dc.Domain != null)
                        {
                            dc.Compiler.RawCall(interfaceType, scriptclass, method, ref constructorParas,
                                ref methodParas,
                                propertynames, ref magicperInfos);
                        }
                        else
                        {
                            throw new CompilerNotFoundException(string.Format("Compiler of {0} not exist.", directory));
                        }
                    }
                    else
                    {
                        throw new CompilerNotFoundException(string.Format("Directory {0} not exist.", directory));
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
        }



        #endregion
    }

    public class CompilerNotFoundException : Exception
    {
        public CompilerNotFoundException(string error) : base(error)
        {
        }
    }

    public class CompileErrorException : Exception
    {
        public CompileErrorException(string error) : base(error)
        {
        }
    }
}