// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Bll.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the Bll type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using log4net;

namespace DataCenter.Accessor
{
    /// <summary>
    /// The bll.
    /// </summary>
    public class Bll
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Bll));
        /// <summary>
        /// The insert data values.
        /// </summary>
        /// <param name="tableName">
        /// The table name.
        /// </param>
        /// <param name="datavalues">
        /// The datavalues.
        /// </param>
        /// <param name="sensorId">
        /// The sensor id.
        /// </param>
        /// <param name="safetyFactorTypeID">
        /// The safety factor type id.
        /// </param>
        /// <param name="acqTime">
        /// The acq time.
        /// </param>
        public void InsertDataValues(string tableName, float[] datavalues, int sensorId, int safetyFactorTypeID, DateTime acqTime)
        {
            try
            {
                SqlDal.InsertDataValues(tableName, datavalues, sensorId, safetyFactorTypeID, acqTime);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
        }

        /// <summary>
        /// The get table name.
        /// </summary>
        /// <param name="safetyFactorTypeID">
        /// The safety factor type id.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetTableName(int safetyFactorTypeID)
         {
            try
            {
                return SqlDal.GetTableName(safetyFactorTypeID);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw ex;
            }
         }

        /// <summary>
        /// The get sensor id.
        /// </summary>
        /// <param name="moduleNo">
        /// The module no.
        /// </param>
        /// <param name="channelNo">
        /// The channel no.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int GetSensorID(int projectId,int safeTypeId,string dtuId,string moduleNo, int channelNo)
        { // { structureId, safeTypeId, dtuId, moduleNo, channelNo }
            try
            {
                return SqlDal.GetSensorID(projectId, safeTypeId, dtuId, moduleNo, channelNo);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw ex;
            }
        }

        public void InsertOrigalData(int sensorId, DateTime acqTime, float[] values)
        {
            SqlDal.InsertOrigalData(sensorId,acqTime,values);
        }


    }
}