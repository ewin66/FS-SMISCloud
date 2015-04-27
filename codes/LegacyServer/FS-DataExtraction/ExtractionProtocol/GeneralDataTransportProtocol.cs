#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="GeneralDataTransportProtocol.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20140609 by WIN .
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion
namespace ExtractionProtocol
{
    using System;
    using System.Collections.Generic;
    using System.Net.NetworkInformation;

    using FSDE.Commn.CheckMode;
    using FSDE.Model;

    public class GeneralDataTransportProtocol : IMakeDataTransportPacket
    {
        private const int MaxLength = 1024;

        private const int MaxFloatcount = (MaxLength - 33) / 4;

        public byte[][] MakeDataTransportPacket(Data data, int structureId)
        {
            //List<byte> packetbytes = new List<byte>() { 0xfe, 0xef };
            //packetbytes.AddRange(BitConverter.GetBytes((short)data.ProjectCode));
            //packetbytes.Add((byte)data.SafeTypeId);
            //packetbytes.Add(0);
            //packetbytes.AddRange(BitConverter.GetBytes(Convert.ToInt16(data.MoudleNo)));
            //packetbytes.Add((byte)data.ChannelId);
            //packetbytes.AddRange(BitConverter.GetBytes(data.CollectTime.Ticks));
            //foreach (double d in data.DataSet)
            //{
            //    float value = (float)d;
            //    packetbytes.AddRange(BitConverter.GetBytes(value));
            //}
            //packetbytes.AddRange(new byte[] { 0, 0, 0x0D });
            //byte[] packet = packetbytes.ToArray();
            //packet[5] = (byte)packet.Length;
            //byte[] crc16 = CheckModeResult.GetCheckResult(packet, 0, 4, CheckType.CRC16HighByteFirst);
            //packet[packet.Length - 3] = crc16[0];
            //packet[packet.Length - 2] = crc16[1];
            int num = data.DataSet.Count / MaxFloatcount;
            if (data.DataSet.Count % MaxFloatcount != 0)
            {
                num += 1;
            }

            byte[][] packet = new byte[num][];

            int i = 0;
            int j = 0;
            while (i < packet.Length)
            {
                var list = new List<byte> { 0xfe, 0xef };
                list.AddRange(BitConverter.GetBytes((short)data.ProjectCode));
                list.Add((byte)data.SafeTypeId);
                list.Add(00);
                list.Add(00);
                list.AddRange(BitConverter.GetBytes(Convert.ToInt16(data.MoudleNo)));
                list.Add((byte)data.ChannelId);
                list.Add((byte)num);
                list.Add((byte)i);
                //list.AddRange(BitConverter.GetBytes(Convert.ToSingle(data.OFlag)));
                list.AddRange(new byte[] { 0, 0, 0, 0, 0, 0 });
                list.AddRange(BitConverter.GetBytes(structureId));
                list.AddRange(BitConverter.GetBytes(data.CollectTime.Ticks));
                for (; j < data.DataSet.Count; j++)
                {
                    if (list.Count + 4 < 1024)
                    {
                        list.AddRange(BitConverter.GetBytes((float)data.DataSet[j]));
                    }
                    else
                    {
                        break;
                    }
                }
                list.AddRange(new byte[] { 0, 0 });
                list.Add(0x0d);
                packet[i] = list.ToArray();
                Array.Copy(BitConverter.GetBytes((short)packet[i].Length), 0, packet[i], 5, 2);
                byte[] crc16 = CheckModeResult.GetCheckResult(packet[i], 0, 4, CheckType.CRC16HighByteFirst);
                packet[i][packet[i].Length - 3] = crc16[0];
                packet[i][packet[i].Length - 2] = crc16[1];
                i++;
            }
            return packet;
        }
    }
}