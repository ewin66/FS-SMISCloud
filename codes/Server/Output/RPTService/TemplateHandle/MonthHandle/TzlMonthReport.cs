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
    public class TzlMonthReport:MonthReport
    {
        public TzlMonthReport(TemplateHandlerPrams param)
            : base(param)
        { 
        }
        public override void WriteFile()
        {
            throw new NotImplementedException();
        }
    }
}
