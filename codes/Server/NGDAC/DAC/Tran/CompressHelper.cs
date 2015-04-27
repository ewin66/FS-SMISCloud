namespace FS.SMIS_Cloud.NGDAC.Tran
{
    using System;
    using System.IO;
    using System.IO.Compression;

    public class CompressHelper
    {
        /// <summary>
        /// 读取文件在内存压缩
        /// </summary>
        /// <param name="path"></param>
        /// <returns>
        /// 压缩后的字节流
        /// </returns>
        public static byte[] Compress(string path)
        {
            using (FileStream streamToZip = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                using (MemoryStream outStream = new MemoryStream())
                {
                    byte[] buffer = new byte[streamToZip.Length];
                    streamToZip.Read(buffer, 0, Convert.ToInt32(streamToZip.Length));
                    using (MemoryStream intStream = new MemoryStream(buffer))
                    {
                        using (GZipStream compress = new GZipStream(outStream, CompressionMode.Compress))
                        {
                            intStream.CopyTo(compress);
                        }
                    }
                    return outStream.ToArray();
                }
            }
        }

        /// <summary>
        /// 压缩字节流
        /// </summary>
        /// <param name="dataBytes"></param>
        /// <returns>
        /// 压缩后的字节流
        /// </returns>
        public static byte[] Compress(byte[] dataBytes)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                using (MemoryStream intStream = new MemoryStream(dataBytes))
                {
                    using (GZipStream compress = new GZipStream(outStream, CompressionMode.Compress))
                    {
                        intStream.CopyTo(compress);
                    }
                }
                return outStream.ToArray();
            }
        }

        public static byte[] Decompress(byte[] bytes)
        {
            return Decompress(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// 压缩后的字节流
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns>
        /// 解压后的字节流
        /// </returns>
        public static byte[] Decompress(byte[] bytes, int offset, int count)
        {
            using (MemoryStream compressedStream = new MemoryStream(bytes, offset, count))
            {
                using (MemoryStream outStream = new MemoryStream())
                {
                    using (GZipStream decompress = new GZipStream(compressedStream, CompressionMode.Decompress))
                    {
                        decompress.CopyTo(outStream);
                        return outStream.ToArray();
                    }
                }
            }
        }
        

    }
}