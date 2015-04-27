using System;
using System.IO;
using System.Runtime.InteropServices;

namespace FreeSun.FS_SMISCloud.Server.DataCalc.Utility
{
    public class StructConvert
    {
        public static Byte[] StructToBytes(Object structure)
        {
            Int32 size = Marshal.SizeOf(structure);
            Console.WriteLine(size);
            IntPtr buffer = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.StructureToPtr(structure, buffer, false);
                Byte[] bytes = new Byte[size];
                Marshal.Copy(buffer, bytes, 0, size);
                return bytes;
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

        public static Object BytesToStruct(Byte[] bytes, Type strcutType)
        {
            Int32 size = Marshal.SizeOf(strcutType);
            IntPtr buffer = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.Copy(bytes, 0, buffer, size);
                return Marshal.PtrToStructure(buffer, strcutType);
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }
    }

    public class FileUtils
    {
        public static void StorageDatum(string file, double[] real, double[] imag)
        {
            using (var fs = new FileStream(file, FileMode.Create, FileAccess.ReadWrite))
            {
                var sw = new StreamWriter(fs);
                if (imag == null)
                {
                    for (int i = 0; i < real.Length; i++)
                    {
                        sw.WriteLine(real[i]);
                    }
                }
                else
                {
                    for (int i = 0; i < real.Length; i++)
                    {
                        sw.WriteLine(real[i] + "\t" + imag[i]);
                    }
                }
                fs.Close();
            }
        }
    }
}
