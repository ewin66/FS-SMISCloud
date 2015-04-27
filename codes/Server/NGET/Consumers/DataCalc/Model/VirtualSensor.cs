namespace FS.SMIS_Cloud.NGET.DataCalc.Model
{
    using System.Collections.Generic;

    using FS.SMIS_Cloud.NGET.Model;

    public class VirtualSensor : SensorData
    {
        public VirtualSensor(int nullPhysicValsCount, IEnumerable<double?> themeValues)
            : base(null, new double[nullPhysicValsCount], null)
        {
            IsSaveDataOriginal = true;
            _themsValues = new List<double?>();
            foreach (var d in themeValues)
            {
                _themsValues.Add(d);
            }
        }
    }
}