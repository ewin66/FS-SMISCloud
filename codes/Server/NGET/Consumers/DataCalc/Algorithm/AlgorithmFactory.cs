namespace FS.SMIS_Cloud.NGET.DataCalc.Algorithm
{
    using FS.SMIS_Cloud.NGET.DataCalc.Model;

    class AlgorithmFactory
    {
        public static IAlgorithm CreateAlgorithm(SensorGroup group)
        {
            switch (group.GroupType)
            {
                case GroupType.Inclination:
                    return new DeepDisplacementAlgorithm(group);
                    break;
                case GroupType.SaturationLine:
                    return new SaturationLineAlgorithm(group);
                    break;
                case GroupType.Settlement:
                    return new SettlementAlgorithm(group);
                    break;
                case GroupType.VirtualSensor:
                    return new VirtualSensorAlgorithm(group);
                    break;
                default:
                    break;
            }
            return null;
        }
    }
}
