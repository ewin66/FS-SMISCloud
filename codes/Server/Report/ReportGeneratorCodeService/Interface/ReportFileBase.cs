using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportGeneratorService.Interface
{
    using ReportGeneratorService.ReportModule;

    using ReportGeneratorService.DataModule;

    public abstract class ReportFileBase
    {
        public Func<ReportTaskResult> CreateNewFile;

        public abstract ReportInfo ReportInfo { get; }

        protected ReportGroup ReportGroup { get; set; }

        public ReportFileBase(ReportGroup reportGroup)
        {
            this.CreateNewFile = this.Create;
            this.ReportGroup = reportGroup;
        }

        protected abstract ReportTaskResult Create();
    }
}
