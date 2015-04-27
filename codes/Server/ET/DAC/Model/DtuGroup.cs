#region File Header
//  --------------------------------------------------------------------------------------------
//  <copyright file="DtuGroup.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2015 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20150408 by LINGWENLONG .
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Linq;

namespace FS.SMIS_Cloud.DAC.Model
{
    public class DtuGroup
    {
        private readonly List<SensorGroup> _groups = new List<SensorGroup>();
        private DateTime _lastGetTime = DateTime.MinValue;

        public uint DacInterval { get { return this._groups.Max(g => g.DacInterval); } }

        public void Add(SensorGroup sengroup)
        {
            lock (this)
            {
                this._groups.Add(sengroup);  
            }
        }

        /// <summary>
        /// DTU编号
        /// </summary>
        /// <param name="dtucode"></param>
        /// <returns></returns>
        public bool Exists(string dtucode)
        {
            return this._groups.Exists(d => d.DtuCode == dtucode);
        }

        /// <summary>
        /// DTUID
        /// </summary>
        /// <param name="dtuId"></param>
        /// <returns></returns>
        public bool Exists(uint dtuId)
        {
            return this._groups.Exists(d => d.DtuId == dtuId);
        }

        /// <summary>
        /// 传感器分组ID
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public bool Exists(int groupId)
        {
            return this._groups.Exists(d => d.GroupId == groupId);
        }
        
        private string _guid = Guid.NewGuid().ToString();

        public string GetGuid(string dtucode, DateTime time)
        {
            lock (this)
            {
                if (((time - this._lastGetTime).TotalSeconds >= this.DacInterval - 50))
                {
                    this._guid = Guid.NewGuid().ToString();
                    this._lastGetTime = time;
                }
            }

            return this._guid;
        }

        
    }
}