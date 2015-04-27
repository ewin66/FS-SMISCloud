using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportGeneratorService.DataModule
{
    using ReportGeneratorService.ReportModule;

    public class ReportInfo : MarshalByRefObject
    {
        public DateTime CreatedDate { get; set; }

        public int? OrgId { get; set; }

        public int? StructureId { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }

        public string FullName { get; set; }

        public string ExtName { get; set; }

        public bool Statue { get; set; }

        public DateType DateType { get; set; }
    }
}
