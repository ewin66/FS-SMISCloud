namespace FS.SMIS_Cloud.NGDAC.Tran
{
    using System.Collections.Generic;

    public interface ITranDataProvider
    {
        void Init(Dictionary<string, string> args);
        int Remainder { get; }
        bool HasMoreData();
        TranMsg[] NextPackages(out int len);
        void OnPackageSent();
    }
}
