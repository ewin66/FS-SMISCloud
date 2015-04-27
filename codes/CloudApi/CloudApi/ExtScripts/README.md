[TOC]


##C#�ű���չ
ʵ��һЩ���ƻ��ģ��ױ������

###����������ӿ�
- ʵ��*IComparer*�ӿ�, �� *List* ����ʹ��
- *FactorSensorController* ����

####ʵ��

``` csharp
using System.Collections;
using System.Collections.Generic;
using FreeSun.FS_SMISCloud.Server.CloudApi.Areas.Sensor.Controllers;

namespace FreeSun.FS_SMISCloud.Server.CloudApi.Scripts
{
	//82�ṹ��41������أ�ƺ������������
    public class Sort_82_41 : IComparer<SimpleSensor>
    {
        public int Compare(SimpleSensor x, SimpleSensor y)
        {
            if (x != null && y != null)
                return x.sensorid.CompareTo(y.sensorid) * -1;//��ID��С����
            else
                return Comparer.Default.Compare(x, y);
        }
    }
}
``` 
####����

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