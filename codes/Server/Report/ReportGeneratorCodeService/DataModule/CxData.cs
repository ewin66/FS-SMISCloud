using System;
using System.Collections.Generic;

namespace ReportGeneratorService.DataModule
{
    public class CxData : MarshalByRefObject
    {
        public DateTime? DateTime { get; set; }        

        public List<CxInernalData> Data { get; set; }
    }

    public class CxInernalData
    {
        public decimal? XValue { get; set; }

        public decimal? YValue { get; set; }

        public decimal? Depth { get; set; }
    }
}
