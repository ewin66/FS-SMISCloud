// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ThemesDataBase.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the ThemesDataBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


using System;
using System.Data.Linq.Mapping;

namespace DataCenter.Model
{
    [Serializable]
    public abstract class ThemesDataBase
    {
        public ThemesDataBase()
        {
            
        }

        public ThemesDataBase(int sensorId, int safetyFactorTypeID,DateTime acqtime)
        {
            this.SensorID = sensorId;
            this.SafetyFactorTypeID = safetyFactorTypeID;
            this.AcqTime = acqtime;
        }

        // [Column(Name = "ID", DbType = "INTEGER PRIMARY KEY AUTOINCREMENT", IsPrimaryKey = true)]
        // public Int64 ID { get; set; }

        /// <summary>
        /// Gets or sets the sensor id.
        /// </summary>
        [Column(Name = "SENSOR_ID", DbType = "int")]
        public Int32 SensorID { get; set; }

        [Column(Name = "SAFETY_FACTOR_TYPE_ID", DbType = "int")]
        public Int32 SafetyFactorTypeID { get; set; }

        [Column(Name = "ACQUISITION_DATETIME",  DbType = "DateTime")]
        public DateTime AcqTime { get; set; }

        // [Column(Name = "ORDERBY_COLUMN",DbType = "int")]
        // public Int32 OrderbyColumn { get; set; }

        // [Column(Name = "DESCRITIPOIN", DbType = "nvarchar(100)")]
        // public string Description { get; set; }
    }
}
