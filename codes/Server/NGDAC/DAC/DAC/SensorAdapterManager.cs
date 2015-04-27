namespace FS.SMIS_Cloud.NGDAC.DAC
{
    using System;

    using FS.SMIS_Cloud.NGDAC.DAC.CxxAdapter;

    using log4net;

    public class SensorAdapterManager
    {
        private static readonly ILog Log = LogManager.GetLogger("SensorAdapterMgr");

        public static IAdapterManager InitializeManager(string factoryName = "FS.SMIS_Cloud.NGDAC.DAC.CxxAdapter.CxxAdapterManager")
        {
            IAdapterManager _adapterManager = new CxxAdapterManager();
            _adapterManager = null;
            Type type = Type.GetType(factoryName);
            if (type != null)
            {
                _adapterManager = (IAdapterManager) Activator.CreateInstance(type, null);
                _adapterManager.Initialize();
            }
            else
            {
                Log.ErrorFormat("Can't initializing the Adapter manager: {0}", factoryName);
            }
            return _adapterManager;
        }
    }
}
