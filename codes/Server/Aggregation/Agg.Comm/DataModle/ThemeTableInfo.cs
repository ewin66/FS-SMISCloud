namespace Agg.Comm.DataModle
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class ThemeTableInfo
    {
        private int factorId;
        public ThemeTableInfo(int factorId)
        {
            this.factorId = factorId;
        }

        public int ColNum { get; set; }
        public string ThemeTableName { get; set; }
    }
}