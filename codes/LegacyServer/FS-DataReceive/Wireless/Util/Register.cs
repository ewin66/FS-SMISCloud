using System;
using Microsoft.Win32;

namespace ET.Common
{
    /// <summary>
    /// 操作注册表
    /// </summary>
    public class Register
    {

        /// <summary>
        /// 实例构造函数
        /// </summary>
        public Register()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }

        public static string PathKey = "";

        #region 公共方法

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subKey"></param>
        /// <param name="keytype"></param>
        public virtual void CreateSubKey(string subKey, KeyType keytype)
        {
            //判断注册表项名称是否为空，如果为空，返回false
            if (string.IsNullOrEmpty(subKey))
            {
                return;
            }
            //创建基于注册表基项的节点
            var key = (RegistryKey)getRegistryKey(keytype);

            //要创建的注册表项的节点
            //RegistryKey sKey;
            if (!IsExist(keytype, subKey))
            {
                 key.CreateSubKey(subKey);
            }
            //sKey.Close();
            //关闭对注册表项的更改
            key.Close();
        }

        /// <summary>
        /// 写入注册表,如果指定项已经存在,则修改指定项的值
        /// </summary>
        /// <param name="keytype">注册表基项枚举</param>
        /// <param name="key">注册表项,不包括基项</param>
        /// <param name="name">值名称</param>
        /// <param name="values">值</param>
        /// <returns>返回布尔值,指定操作是否成功</returns>
        public bool SetValue(KeyType keytype, string key, string name, string values)
        {
            try
            {
                var rk = (RegistryKey) getRegistryKey(keytype);
                RegistryKey rkt = rk.CreateSubKey(key);
                if (rkt != null)
                {
                    rkt.SetValue(name, values);
                    rkt.Close();
                    rk.Close();
                    return true;
                }
                    throw (new Exception("要写入的项不存在"));
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 读取注册表
        /// </summary>
        /// <param name="keytype">注册表基项枚举</param>
        /// <param name="key">注册表项,不包括基项</param>
        /// <param name="name">值名称</param>
        /// <returns>返回字符串</returns>
        public string GetValue(KeyType keytype, string key, string name)
        {
            try
            {
                var rk = (RegistryKey) getRegistryKey(keytype);

                RegistryKey rkt = rk.OpenSubKey(key);

                if (rkt != null)
                    return rkt.GetValue(name).ToString();
                throw (new Exception("无法找到指定项"));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 删除注册表中的值
        /// </summary>
        /// <param name="keytype">注册表基项枚举</param>
        /// <param name="key">注册表项名称,不包括基项</param>
        /// <param name="name">值名称</param>
        /// <returns>返回布尔值,指定操作是否成功</returns>
        public bool DeleteValue(KeyType keytype, string key, string name)
        {
            try
            {
                var rk = (RegistryKey) getRegistryKey(keytype);
                RegistryKey rkt = rk.OpenSubKey(key, true);

                if (rkt != null)
                {
                    rkt.DeleteValue(name, true);
                    rkt.Close();
                    rk.Close();
                    return true;
                }
                    throw (new Exception("无法找到指定项"));
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 删除注册表中的指定项
        /// </summary>
        /// <param name="keytype">注册表基项枚举</param>
        /// <param name="key">注册表中的项,不包括基项</param>
        /// <returns>返回布尔值,指定操作是否成功</returns>
        public bool DeleteSubKey(KeyType keytype, string key)
        {
            try
            {
                var rk = (RegistryKey) getRegistryKey(keytype);
                rk.DeleteSubKeyTree(key);
                rk.Close();
                return true;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 判断指定项是否存在
        /// </summary>
        /// <param name="keytype">基项枚举</param>
        /// <param name="key">指定项字符串</param>
        /// <returns>返回布尔值,说明指定项是否存在</returns>
        public bool IsExist(KeyType keytype, string key)
        {
            var rk = (RegistryKey) getRegistryKey(keytype);

            if (rk.OpenSubKey(key) == null)
                return false;
            return true;
        }


        /// <summary>
        /// 检索指定项关联的所有值
        /// </summary>
        /// <param name="keytype">基项枚举</param>
        /// <param name="key">指定项字符串</param>
        /// <returns>返回指定项关联的所有值的字符串数组</returns>
        public string[] GetValues(KeyType keytype, string key)
        {
            var rk = (RegistryKey) getRegistryKey(keytype);

            RegistryKey rkt = rk.OpenSubKey(key);

            if (rkt != null)
            {
                string[] names = rkt.GetValueNames();

                if (names.Length == 0)
                    return names;
                var values = new string[names.Length];

                int i = 0;
                foreach (string name in names)
                {
                    values[i] = rkt.GetValue(name).ToString();
                    i++;
                }
                return values;
            }
            throw (new Exception("指定项不存在"));
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 返回RegistryKey对象
        /// </summary>
        /// <param name="keytype">注册表基项枚举</param>
        /// <returns></returns>
        private object getRegistryKey(KeyType keytype)
        {
            RegistryKey rk = null;

            switch (keytype)
            {
                case KeyType.HKEY_CLASS_ROOT:
                    rk = Registry.ClassesRoot;
                    break;
                case KeyType.HKEY_CURRENT_USER:
                    rk = Registry.CurrentUser;
                    break;
                case KeyType.HKEY_LOCAL_MACHINE:
                    rk = Registry.LocalMachine;
                    break;
                case KeyType.HKEY_USERS:
                    rk = Registry.Users;
                    break;
                case KeyType.HKEY_CURRENT_CONFIG:
                    rk = Registry.CurrentConfig;
                    break;
            }

            return rk;
        }

        #endregion

        #region 枚举

        /// <summary>
        /// 注册表基项枚举
        /// </summary>
        public enum KeyType
        {
            /// <summary>
            /// 注册表基项 HKEY_CLASSES_ROOT
            /// </summary>
            HKEY_CLASS_ROOT,

            /// <summary>
            /// 注册表基项 HKEY_CURRENT_USER
            /// </summary>
            HKEY_CURRENT_USER,

            /// <summary>
            /// 注册表基项 HKEY_LOCAL_MACHINE
            /// </summary>
            HKEY_LOCAL_MACHINE,

            /// <summary>
            /// 注册表基项 HKEY_USERS
            /// </summary>
            HKEY_USERS,

            /// <summary>
            /// 注册表基项 HKEY_CURRENT_CONFIG
            /// </summary>
            HKEY_CURRENT_CONFIG
        }

        #endregion




        //以下从‘读’‘写’‘删除’‘判断’四个事例实现对注册表的简单操作 

        /// <summary>
        /// 读取指定名称的注册表的值
        /// </summary>
        /// <param name="keytype"></param>
        /// <param name="pathKey"></param>
        /// <param name="chaildKey"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetRegistData(KeyType keytype,string pathKey,string chaildKey,string name)
        {
            if(string.IsNullOrEmpty(pathKey)||string.IsNullOrEmpty(chaildKey))
                throw new Exception("入参不能为空");
            string registData;
            var hkml = (RegistryKey)getRegistryKey(keytype);
            RegistryKey software = hkml.OpenSubKey(pathKey, true);
            if (software != null)
            {
                RegistryKey aimdir = software.OpenSubKey(chaildKey, true);
                if (aimdir != null)
                {
                    registData = aimdir.GetValue(name).ToString();
                    return registData;
                }
            }
            return null;
            //throw new Exception("该注册表项不存在");
        }

        
        /// <summary>
        /// 向注册表中写数据
        /// </summary>
        /// <param name="keytype"></param>
        /// <param name="pathKey"></param>
        /// <param name="chaildKey"></param>
        /// <param name="name"></param>
        /// <param name="tovalue"></param>
        /// <returns></returns>
        public bool WriteRegedit(KeyType keytype, string pathKey, string chaildKey, string name, string tovalue)
        {
            if (string.IsNullOrEmpty(pathKey) || string.IsNullOrEmpty(chaildKey) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(tovalue))
                throw new Exception("入参不能为空");
            var hklm = (RegistryKey) getRegistryKey(keytype);
            RegistryKey software = hklm.OpenSubKey(pathKey, true);
            if (software != null)
            {
                RegistryKey aimdir = software.CreateSubKey(chaildKey);
                if (aimdir != null)
                {
                    aimdir.SetValue(name, tovalue);
                    software.Close();
                    return true;
                }
                throw new Exception("该注册表项子项不存在");
            }
            throw new Exception("该注册表项不存在");
        }

 

        //删除注册表中指定的注册表项
        public void DeleteRegist(KeyType keytype, string pathKey, string chaildKey, string name)
        {
            if (string.IsNullOrEmpty(pathKey) || string.IsNullOrEmpty(chaildKey) || string.IsNullOrEmpty(name))
                throw new Exception("入参不能为空");
            string[] aimnames;
            var hkml = (RegistryKey)getRegistryKey(keytype);
            RegistryKey software = hkml.OpenSubKey(pathKey, true);
            RegistryKey aimdir = software.OpenSubKey(chaildKey, true);
            aimnames = aimdir.GetSubKeyNames();
            foreach (string aimKey in aimnames)
            {
                if (aimKey == name)
                    aimdir.DeleteSubKeyTree(name);
            }
            software.Close();
        }

        //判断指定注册表项是否存在
        public bool IsRegeditExit(KeyType keytype, string pathKey, string chaildKey, string name)
        {
            if (string.IsNullOrEmpty(pathKey) || string.IsNullOrEmpty(chaildKey) || string.IsNullOrEmpty(name))
                throw new Exception("入参不能为空");
            //bool exit = false;
            string[] subkeyNames;
            var hkml = (RegistryKey)getRegistryKey(keytype);
            RegistryKey software = hkml.OpenSubKey(pathKey, true);
            RegistryKey aimdir = software.OpenSubKey(chaildKey, true);
            subkeyNames = aimdir.GetSubKeyNames();
            foreach (string keyName in subkeyNames)
            {
                if (keyName == name)
                {
                    //exit = true;
                    return true;
                }
            }
            return false;
        }


    }
}


