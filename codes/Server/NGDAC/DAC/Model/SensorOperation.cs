namespace FS.SMIS_Cloud.NGDAC.Model
{
    using System;

    [Serializable]
    public class SensorOperation
    {
        public Sensor Sensor { get; set; }

        public uint OldSensorId { get; set; }

        public uint OldDtuId { get; set; }

        public Operations Action { get; set; }

    }

    [Serializable]
    public class OpearationTemp<T>
    {
        public T OperatorObj { get; set; }

        public uint OldOperatorObjId { get; set; }

        public uint OldParentId { get; set; }

        public T OldOperatorObj { get; set; }

        public Operations Action { get; set; }
    }

}