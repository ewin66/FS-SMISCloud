#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="IniFileHelper.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20140813 by WIN .
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion

namespace FSDE.Commn 
{
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Text;

    /// <summary>    
    /// 操作ini文件     
    /// </summary> 
    public class IniFileHelper     
    { 
        /// <summary> 
        /// 获取所有段落名：先获得字节数组，然后将字节数组转换为字符串         
        /// </summary> 
        /// <param name="buffer">请使用字节数组代替StringBuilder</param>         
        /// <param name="iLen"></param> 
        /// <param name="fileName"></param>        
        /// <returns></returns> 
        [DllImport("kernel32.dll")] 
        public extern static int GetPrivateProfileSectionNamesA(byte[] buffer, int iLen, string fileName);  

        /// <summary> 
        /// 获取指定小节所有项名和值的一个列表         
        /// /// </summary> 
        /// <param name="section">节段，欲获取的小节。注意这个字串不区分大小写</param> 
        /// <param name="buffer">缓冲区返回的是一个二进制的串，字符串之间是用"\0"分隔的</param> 
        /// <param name="nSize">缓冲区的大小</param> 
        /// <param name="filePath">初始化文件的名字。如没有指定完整路径名，windows就在Windows目录中查找文件</param>
        /// <returns></returns> 
        [DllImport("kernel32")] 
        public static extern int GetPrivateProfileSection(string section, byte[] buffer, int nSize, string filePath);  

        /// <summary> 
        /// 读操作读取字符串
        /// </summary> 
        /// <param name="section">要读取的段落名</param>
        /// <param name="key">要读取的键</param> 
        /// <param name="defVal">读取异常的情况下的缺省值；如果Key值没有找到，则返回缺省的字符串的地址</param> 
        /// <param name="retVal">key所对应的值，如果该key不存在则返回空值</param>
        /// <param name="size">返回值允许的大小</param> 
        /// <param name="filePath">INI文件的完整路径和文件名</param>
        /// <returns></returns>         
        [DllImport("kernel32")] 
        private static extern int GetPrivateProfileString(string section, string key, string defVal, StringBuilder retVal, int size, string filePath);      
    
        /// <summary> 
        /// 读操作读取整数         
        /// </summary> 
        /// <param name="lpAppName">指向包含Section 名称的字符串地址</param>
        /// <param name="lpKeyName">指向包含Key 名称的字符串地址</param>
        /// <param name="nDefault">如果Key 值没有找到，则返回缺省的值是多少</param>
        /// <param name="lpFileName">INI文件的完整路径和文件名</param>
        /// <returns>返回获得的整数值</returns>
        [DllImport("kernel32")] 
        private static extern int GetPrivateProfileInt(string lpAppName, string lpKeyName, int nDefault, string lpFileName); 
 
        /// <summary>         
        /// 写操作 
        /// </summary> 
        /// <param name="section">要写入的段落名</param> 
        /// <param name="key">要写入的键，如果该key存在则覆盖写入</param>
        /// <param name="val">key所对应的值</param> 
        /// <param name="filePath">INI文件的完整路径和文件名</param>
        /// <returns></returns> 
        [DllImport("kernel32")] 
        private static extern bool WritePrivateProfileString(string section, string key, string val, string filePath);
  
        /// <summary>
        /// 获得整数值
        /// </summary> 
        /// <param name="section">要读取的段落名</param> 
        /// <param name="key">要读取的键</param> 
        /// <param name="def">如果Key 值没有找到，则返回缺省的值是多少</param>
        /// <param name="fileName">INI文件的完整路径和文件名</param>
        /// <returns></returns>
        public static int GetInt(string section, string key, int def, string fileName)  
        { 
            return GetPrivateProfileInt(section, key, def, fileName);         
        }  

        /// <summary> 
        /// 获得字符串值,默认返回长度为
        /// </summary> 
        /// <param name="section">要读取的段落名</param>
        /// <param name="key">要读取的键</param> 
        /// <param name="def">如果Key 值没有找到，返回的默认值</param> 
        /// <param name="fileName">INI文件的完整路径和文件名</param>
        /// <returns></returns> 
        public static string GetString(string section, string key, string def, string fileName)
        { 
            StringBuilder temp = new StringBuilder(1024); 
            GetPrivateProfileString(section, key, def, temp, 1024, fileName); 
            return temp.ToString(); 
        }  

        /// <summary> 
        /// 获得字符串值，返回长度用户自定义
        /// </summary> 
        /// <param name="section">要读取的段落名</param>
        /// <param name="key">要读取的键</param> 
        /// <param name="def">如果Key 值没有找到，返回的默认值</param>
        /// <param name="fileName">INI文件的完整路径和文件名</param> 
        /// <param name="size">用户自定义返回的字符串长度</param>
        /// <returns></returns> 
        public static string GetString(string section, string key, string def, string fileName, int size)
        { 
            StringBuilder temp = new StringBuilder(); 
            GetPrivateProfileString(section, key, def, temp, size, fileName); 
            return temp.ToString();
        }  

        /// <summary>
        /// 写整数值 
        /// </summary> 
        /// <param name="section">要写入的段落名</param> 
        /// <param name="key">要写入的键，如果该key存在则覆盖写入</param>
        /// <param name="iVal">key所对应的值</param> 
        /// <param name="fileName">INI文件的完整路径和文件名</param> 
        public static void WriteInt(string section, string key, int iVal, string fileName)
        { 
            WritePrivateProfileString(section, key, iVal.ToString(), fileName);
        }  

        /// <summary>         
        /// 写字符串的值 
        /// </summary> 
        /// <param name="section">要写入的段落名</param> 
        /// <param name="key">要写入的键，如果该key存在则覆盖写入</param>
        /// <param name="strVal">key所对应的值</param> 
        /// <param name="fileName">INI文件的完整路径和文件名</param> 
        public static void WriteString(string section, string key, string strVal, string fileName)
        { 
            WritePrivateProfileString(section, key, strVal, fileName);
        }  

        /// <summary> 
        /// 删除指定的key
        /// </summary> 
        /// <param name="section">要写入的段落名</param>
        /// <param name="key">要删除的键</param> 
        /// <param name="fileName">INI文件的完整路径和文件名</param> 
        public static void DelKey(string section, string key, string fileName)
        { 
            WritePrivateProfileString(section, key, null, fileName);
        }  

        /// <summary> 
        /// 删除指定的段落
        /// </summary> 
        /// <param name="section">要删除的段落名</param> 
        /// <param name="fileName">INI文件的完整路径和文件名</param>
        public static void DelSection(string section, string fileName)
        { 
            WritePrivateProfileString(section, null, null, fileName);
        }

        /// <summary> 
        /// 获取ini文件中所有的段名(节名)，保存在列表中
        /// </summary> 
        /// <param name="filePath">ini文件的绝对路径</param>
        /// <returns></returns> 
        public static List<string> ReadSections(string filePath)
        {
            byte[] buffer = new byte[65535];
            int rel = GetPrivateProfileSectionNamesA(buffer, buffer.GetUpperBound(0), filePath);
            int iCnt, iPos; 
            List<string> arrayList = new List<string>();
            string tmp;             
            if (rel > 0) 
            {
                iPos = 0; 
                for (iCnt = 0; iCnt < rel; iCnt++)
                { 
                    if (buffer[iCnt] == 0x00) 
                    { 
                        tmp = ASCIIEncoding.Default.GetString(buffer, iPos, iCnt - iPos).Trim(); 
                        iPos = iCnt + 1;
                        if (tmp != "")  
                        { 
                            arrayList.Add(tmp); 
                        }   
                    }      
                }        
            } 
            return arrayList; 
        }

         /// <summary> 
         /// 获取指定段section下的所有键值对，返回集合的每一个元素形如"key=value"
         /// </summary> 
         /// <param name="section">指定的段落</param> 
         /// <param name="filePath">ini文件的绝对路径</param>
         /// <returns></returns> 
        public static List<string> ReadKeyValues(string section, string filePath)
        { 
            byte[] buffer = new byte[32767]; 
            List<string> list = new List<string>(); 
            int length = GetPrivateProfileSection(section, buffer, buffer.GetUpperBound(0), filePath);
            string temp; 
            int postion = 0; 
            for (int i = 0; i < length; i++)
            { 
                if (buffer[i] == 0x00) //以'\0'来作为分隔
                { 
                    temp = ASCIIEncoding.Default.GetString(buffer, postion, i - postion).Trim(); 
                    postion = i + 1; 
                    if (temp.Length > 0)
                    { 
                        list.Add(temp); 
                    }   
                }         
            } 
            return list;
        }
   
    } 
} 




 


