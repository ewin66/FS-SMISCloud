﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成
//     如果重新生成代码，将丢失对此文件所做的更改。
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aggregation
{
    using Agg.DataPool;
    using Agg.Process;

    public class AggTask
    {
        private IDataPool dataPool;

        private IAggProcess process;

        public AggTask(IDataPool dataPool, IAggProcess aggProcess)
        {
            this.dataPool = dataPool;
            this.process = aggProcess;
        }

        /// <summary>
        ///  调用GetData
        /// </summary>
        public virtual void Run()
        {
            
        }

    }

}


