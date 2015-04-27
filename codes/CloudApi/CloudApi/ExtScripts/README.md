[TOC]


##C#脚本扩展
实现一些定制化的，易变的需求。

###传感器排序接口
- 实现*IComparer*接口, 供 *List* 排序使用
- *FactorSensorController* 调用

####实例

``` csharp
using System.Collections;
using System.Collections.Generic;
using FreeSun.FS_SMISCloud.Server.CloudApi.Areas.Sensor.Controllers;

namespace FreeSun.FS_SMISCloud.Server.CloudApi.Scripts
{
	//82结构的41监测因素（坪汗净空收敛）
    public class Sort_82_41 : IComparer<SimpleSensor>
    {
        public int Compare(SimpleSensor x, SimpleSensor y)
        {
            if (x != null && y != null)
                return x.sensorid.CompareTo(y.sensorid) * -1;//按ID大小倒序
            else
                return Comparer.Default.Compare(x, y);
        }
    }
}
``` 
####调用

``` csharp
private static void Sort(List<SimpleSensor> toList, int structId, int factorId)
{
    const string directory = @".\ExtScripts";
    var name = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
        string.Format(directory + @"\Sort_{0}_{1}", structId, factorId));
    var assembly = CompileHelper.CompileAssembly(name + ".cs");
    var scriptTypes = CompileHelper.GetTypesImplementingInterface(assembly, typeof (IComparer<SimpleSensor>));
    if (scriptTypes != null && scriptTypes.Count == 1)
    {
        var comparer = (IComparer<SimpleSensor>) Activator.CreateInstance(scriptTypes[0]);
        toList.Sort(comparer);
    }
}
``` 

####CompileHelper

``` csharp
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using Microsoft.CSharp;

namespace FreeSun.FS_SMISCloud.Server.CloudApi.Common
{
    public class CompileHelper
    {
        public static Assembly CompileAssembly(string sortScript)
        {
            var codeProvider = new CSharpCodeProvider();
            var compilerParameters = new CompilerParameters
            {
                GenerateExecutable = false,
                GenerateInMemory = true,
                IncludeDebugInformation = true
            };
            compilerParameters.ReferencedAssemblies.Add(Assembly.GetExecutingAssembly().Location);
            var result = codeProvider.CompileAssemblyFromFile(compilerParameters, sortScript);
            if (result.Errors.HasErrors) throw new Exception("Assembly compilation failed.");
            return result.CompiledAssembly;
        }

        public static List<Type> GetTypesImplementingInterface(Assembly assembly, Type interfaceType)
        {
            if (!interfaceType.IsInterface) throw new ArgumentException("Not an interface.", "interfaceType");
            return assembly.GetTypes()
                .Where(interfaceType.IsAssignableFrom)
                .ToList();
        }
    }
}
``` 