using Aspose.Cells;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReportGeneratorService.ReportModule;

namespace ReportGeneratorService.TemplateHandle
{
    public class GzlMonthReport:MonthReport
    {
        public GzlMonthReport(TemplateHandlerPrams param)
            : base(param)
        { 
        }
        public override void WriteFile()
        {
            throw new NotImplementedException();
        }
    }
}
