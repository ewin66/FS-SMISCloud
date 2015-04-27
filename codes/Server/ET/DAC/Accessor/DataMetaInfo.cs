using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FS.SMIS_Cloud.DAC.Accessor
{
    public class DataMetaInfo
    {
        public Type DataType {get; set; }
        public string TableName { get; set; }
        public string[] ThemeColums { get; set; } 
        public int OriginalDataOffset { get; set; }
        public int OriginalDataCount { get; set; }
        public int ThemesDataOffset { get; set; }

        public DataMetaInfo()
        {
            OriginalDataOffset = 0;
            ThemesDataOffset = 0;
        }
    }
}
