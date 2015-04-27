using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using NUnit.Framework;

namespace DAC.Test.Tran
{

    public interface IAdapterCap
    {
        int Protocol { get; }
        string Name { get; }
    }


    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class Adapter : ExportAttribute
    {
        public Adapter() : base(typeof(IAdapter)) { }
        public int Protocol { get; set; }
        public string Name { get; set; }
    }

    [TestFixture]
    class MEFTester
    {
 //       [Import]
 //       public IAdapter myadapter;

        [ImportMany] 
        public List<IAdapter> adapters1;
        
        [ImportMany]
        public IEnumerable<IAdapter> adapters2 { get; set; }

        // 强约束, 自定义元数据
        [ImportMany]
        public Lazy<IAdapter, IAdapterCap>[] adapters3 { get; set; }

        // 弱约束: 自定义元数据 // ExportTypeIdentify = Class
        [ImportMany]
        public Lazy<IAdapter, IDictionary<string, object>>[] adapters4 { get; set; }


        [Test]
        public void Test()
        {
            var catalog = new AssemblyCatalog(typeof(MEFTester).Assembly);
            CompositionContainer cc = new CompositionContainer(catalog);
            cc.ComposeParts(this);
            foreach (IAdapter a in adapters2)
            {
            }
            foreach (Lazy<IAdapter, IAdapterCap> lazy in adapters3)
            {
                Console.WriteLine("Name={0} protocol={1}",lazy.Metadata.Name, lazy.Metadata.Protocol);
            }
            Assert.AreEqual(2, adapters3.Count());
            Assert.AreEqual(3, adapters1.Count());
            Assert.AreEqual(3, adapters2.Count());
            Assert.AreEqual(3, adapters4.Count());
        }
    }


    [Export(typeof(IAdapter))]
    [ExportMetadata("Protocol", 200)]
    [ExportMetadata("Name", "Adapter1")]
    class AdapterImpl1 : IAdapter
    {
    }

    [Adapter(Name = "Adapter2", Protocol = 300)]
    class AdapterImpl2 : IAdapter
    {
    }

    [Export(typeof(IAdapter))]
    class AdapterImpl3 : IAdapter
    {
    }

    interface IAdapter
    {
    }
}
