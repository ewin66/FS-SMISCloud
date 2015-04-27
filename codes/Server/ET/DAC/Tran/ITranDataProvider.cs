using System.Collections.Generic;

namespace FS.SMIS_Cloud.DAC.Tran
{
    public interface ITranDataProvider
    {
        void Init(Dictionary<string, string> args);
        int Remainder { get; }
        bool HasMoreData();
        TranMsg[] NextPackages(out int len);
        void OnPackageSent();
    }
}
