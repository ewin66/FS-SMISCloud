using Aspose.Cells;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReportGeneratorService.ReportModule;

namespace ReportGeneratorService.TemplateHandle
{
    public class GzlDailyReport:DailyReport
    {
        public GzlDailyReport(TemplateHandlerPrams param) : base(param) 
        {
        }
        public override void WriteFile()
        {
 	        throw new NotImplementedException();
        }
        
    }
}
