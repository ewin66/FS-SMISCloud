#region File Header
//  --------------------------------------------------------------------------------------------
//  <copyright file="StructureNode.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2015 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20150226 by LINGWENLONG .
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
using System.Collections.Concurrent;
using System.Linq;

namespace FS.SMIS_Cloud.DAC.Model
{
    public class StructureNode
    {
        public uint StructureId { get; set; }
        public int DacInterval { get; set; }

        private readonly ConcurrentBag<string> _dtunodes = new ConcurrentBag<string>();   
        private DateTime _lastGetTime = DateTime.MinValue;

        public void AddNewDtu(string dtucode)
        {
            if (!_dtunodes.Contains(dtucode))
                _dtunodes.Add(dtucode);
        }

        private string _guid = Guid.NewGuid().ToString();

        public string GetGuid(string dtucode, DateTime time)
        {
            lock (this)
            {
                if (!_dtunodes.Contains(dtucode))
                {
                    AddNewDtu(dtucode);
                }
                if (((time - _lastGetTime).TotalSeconds >= DacInterval - 60))
                    {
                        _guid = Guid.NewGuid().ToString();
                        _lastGetTime = time;
                    }  
            }

            return _guid;
        }

        public void DeleteDtu(string dtucode)
        {
            if (_dtunodes.Contains(dtucode))
            {
                _dtunodes.TryTake(out dtucode);
            }
        }

        public void UpdateDtucodes(OpearationTemp<string> dtu)
        {
            switch (dtu.Action)
            {
                case Operations.Add:
                    AddNewDtu(dtu.OperatorObj);
                    break;
                case Operations.Delete:
                    DeleteDtu(dtu.OperatorObj);
                    break;
                case Operations.Update:
                    DeleteDtu(dtu.OldOperatorObj);
                    AddNewDtu(dtu.OperatorObj);
                    break;
            }
        }
    }
}