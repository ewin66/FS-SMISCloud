namespace FS.SMIS_Cloud.NGDAC.Util
{
    using System;
    using System.ComponentModel;
    using System.Reflection;

    /// <summary>
    /// 枚举帮助类
    /// </summary>
    public class EnumHelper
    {
        /// <summary>
        /// 得到一个枚举值的Description
        /// </summary>
        /// <param name="en">枚举值</param>
        /// <returns>枚举值的Description信息</returns>
        public static string GetDescription(Enum en)
        {
            Type type = en.GetType();

            MemberInfo[] memInfo = type.GetMember(en.ToString());

            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)
                {
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }

            return en.ToString();
        }

        /// <summary>
        /// 根据枚举描述获取一个枚举信息类
        /// </summary>
        /// <param name="type"></param>
        /// <param name="desc"></param>
        /// <returns></returns>
        public static object GetItemFromDesc(Type type, string desc)
        {
            foreach (MemberInfo mInfo in type.GetMembers())
            {
                foreach (Attribute attr in Attribute.GetCustomAttributes(mInfo))
                {
                    if (attr.GetType() == typeof(DescriptionAttribute))
                    {
                        if(((DescriptionAttribute)attr).Description==desc.Trim())
                        {
                            return Enum.Parse(type,mInfo.Name);
                        }
                    }
                }
            }
            return null;
        }
    }
}
