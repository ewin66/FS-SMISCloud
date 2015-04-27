using FreeSun.FS_SMISCloud.Server.DataCalc.Utility;

namespace DAAS
{
    using System;
    /// <summary>
    /// 快速傅立叶变换(Fast Fourier Transform)。 
    /// </summary>
    public class TWFFT
    {
        private static void bitrp(double[] xreal, double[] ximag, int n)
        {
            // 位反转置换 Bit-reversal Permutation
            int i, j, a, b, p;
            for (i = 1, p = 0; i < n; i *= 2)
            {
                p++;
            }
            for (i = 0; i < n; i++)
            {
                a = i;
                b = 0;
                for (j = 0; j < p; j++)
                {
                    b = b * 2 + a % 2;
                    a = a / 2;
                }
                if (b > i)
                {
                    double t = xreal[i];
                    xreal[i] = xreal[b];
                    xreal[b] = t;
                    t = ximag[i];
                    ximag[i] = ximag[b];
                    ximag[b] = t;
                }
            }
        }

        public static int FFT(double[] xreal, double[] ximag)
        {
            //n值为2的N次方
            int n = 2;
            while (n <= xreal.Length)
            {
                n *= 2;
            }
            n /= 2;
            // 快速傅立叶变换，将复数 x 变换后仍保存在 x 中，xreal, ximag 分别是 x 的实部和虚部
            double[] wreal = new double[n / 2];
            double[] wimag = new double[n / 2];
            double treal, timag, ureal, uimag, arg;
            int m, k, j, t, index1, index2;
            bitrp(xreal, ximag, n);
            // 计算 1 的前 n / 2 个 n 次方根的共轭复数 W'j = wreal [j] + i * wimag [j] , j = 0, 1, ... , n / 2 - 1
            arg = (double)(-2 * Math.PI / n);
            treal = (double)Math.Cos(arg);
            timag = (double)Math.Sin(arg);
            wreal[0] = 1.0f;
            wimag[0] = 0.0f;
            for (j = 1; j < n / 2; j++)
            {
                wreal[j] = wreal[j - 1] * treal - wimag[j - 1] * timag;
                wimag[j] = wreal[j - 1] * timag + wimag[j - 1] * treal;
            }
            for (m = 2; m <= n; m *= 2)
            {
                for (k = 0; k < n; k += m)
                {
                    for (j = 0; j < m / 2; j++)
                    {
                        index1 = k + j;
                        index2 = index1 + m / 2;
                        t = n * j / m;    // 旋转因子 w 的实部在 wreal [] 中的下标为 t
                        treal = wreal[t] * xreal[index2] - wimag[t] * ximag[index2];
                        timag = wreal[t] * ximag[index2] + wimag[t] * xreal[index2];
                        ureal = xreal[index1];
                        uimag = ximag[index1];
                        xreal[index1] = ureal + treal;
                        ximag[index1] = uimag + timag;
                        xreal[index2] = ureal - treal;
                        ximag[index2] = uimag - timag;
                    }
                }
            }
            return n;
        }

        public static int IFFT(double[] xreal, double[] ximag)
        {
            //n值为2的N次方
            int n = 2;
            while (n <= xreal.Length)
            {
                n *= 2;
            }
            n /= 2;
            // 快速傅立叶逆变换
            double[] wreal = new double[n / 2];
            double[] wimag = new double[n / 2];
            double treal, timag, ureal, uimag, arg;
            int m, k, j, t, index1, index2;
            bitrp(xreal, ximag, n);
            // 计算 1 的前 n / 2 个 n 次方根 Wj = wreal [j] + i * wimag [j] , j = 0, 1, ... , n / 2 - 1
            arg = (double)(2 * Math.PI / n);
            treal = (double)(Math.Cos(arg));
            timag = (double)(Math.Sin(arg));
            wreal[0] = 1.0f;
            wimag[0] = 0.0f;
            for (j = 1; j < n / 2; j++)
            {
                wreal[j] = wreal[j - 1] * treal - wimag[j - 1] * timag;
                wimag[j] = wreal[j - 1] * timag + wimag[j - 1] * treal;
            }
            for (m = 2; m <= n; m *= 2)
            {
                for (k = 0; k < n; k += m)
                {
                    for (j = 0; j < m / 2; j++)
                    {
                        index1 = k + j;
                        index2 = index1 + m / 2;
                        t = n * j / m;    // 旋转因子 w 的实部在 wreal [] 中的下标为 t
                        treal = wreal[t] * xreal[index2] - wimag[t] * ximag[index2];
                        timag = wreal[t] * ximag[index2] + wimag[t] * xreal[index2];
                        ureal = xreal[index1];
                        uimag = ximag[index1];
                        xreal[index1] = ureal + treal;
                        ximag[index1] = uimag + timag;
                        xreal[index2] = ureal - treal;
                        ximag[index2] = uimag - timag;
                    }
                }
            }
            for (j = 0; j < n; j++)
            {
                xreal[j] /= n;
                ximag[j] /= n;
            }
            return n;
        }

        /// <summary>
        /// 对于时域a[]转换为频率Y(f),参数b为0
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int FFTData1(double[] a, double[] b, double Fs, out double[] Y, out double[] f)
        {
            try
            {
                int L = a.Length;//采样个数
                int NFFT = TWFFT.funTwo(L);//最靠近大于2的幂数NFFT，有点问题
                Y = new double[NFFT / 2 + 1];
                f = new double[NFFT / 2 + 1];
                double[] ax = new double[NFFT];
                double[] bx = new double[NFFT];
                Array.Copy(a, 0, ax, 0, L);
                if (b != null)
                {
                    Array.Copy(b, 0, bx, 0, L);
                }
                TWFFT.FFT(ax, bx);

                for (int i = 0; i < NFFT / 2 + 1; i++)
                {
                    double j = (double)i / (NFFT / 2);
                    f[i] = Fs / 2 * j;
                }

                for (int i = 0; i < NFFT / 2 + 1; i++)
                {
                    Y[i] = 2 * (double)Math.Sqrt(Math.Pow(ax[i], 2) + Math.Pow(bx[i], 2)) / L;
                }
                return NFFT / 2 + 1;
            }
            catch (Exception ee)
            {
                throw ee;
            }
        }

        /// <summary>
        /// 对于时域a[]转换为频率Y(f),参数b为0
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int FFTData0(double[] a, double[] b, double Fs, out double[] Y, out double[] f)
        {
            try
            {
                int L = a.Length;//采样个数
                if (b == null)
                {
                    b = new double[L];
                }
                int NFFT = TWFFT.funTwo(L);//最靠近大于2的幂数NFFT，有点问题
                Y = new double[NFFT / 2 + 1];
                f = new double[NFFT / 2 + 1];
                TWFFT.FFT(a, b);
#if DEBUG
                FileUtils.StorageDatum(string.Format(@"FFTResult{0}.txt", DateTime.Now.ToString("HHmmss")), a, b);
#endif

                for (int i = 0; i < NFFT / 2 + 1; i++)
                {
                    double j = (double)i / (NFFT / 2);
                    f[i] = Fs / 2 * j;
                }

                for (int i = 0; i < NFFT / 2 + 1; i++)
                {
                    Y[i] = 2 * (double)Math.Sqrt(Math.Pow(a[i], 2) + Math.Pow(b[i], 2)) / L;
                }
                return NFFT / 2 + 1;
            }
            catch (Exception ee)
            {
                throw ee;
            }
        }

        public static int funTwo(int v)
        {
            int res = 1;
            while (res < v)
                res <<= 1;
            return res;
        }

        /// <summary>
        /// 对于时域a[]转换为频率Y(f),参数b为0
        /// 数据总量可大于每次FFT计算的nCount，多次FFT计算求平均
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool FFTData(double[] a, double fs, int nCount, out double[] Y, out double[] f)
        {
            try
            {
                int len = a.Length;
                int TotalCnt = 0;
                int index = 0;
                double[] tmp = new double[nCount];
                Y = new double[nCount / 2 + 1];
                while (index < len)
                {
                    int n = 0;
                    for (int i = index; i < index + nCount; i++)
                    {
                        if (i < len)
                        {
                            tmp[n] = a[i];
                        }
                        else
                        {
                            tmp[n] = 0f;
                        }
                        n++;
                    }

                    var b = new double[nCount];
                    TWFFT.FFT(tmp, b);

                    //直流分量
                    Y[0] += (double)Math.Sqrt(Math.Pow(tmp[0], 2) + Math.Pow(b[0], 2)) / nCount;
                    for (int i = 1; i < nCount / 2 + 1; i++)
                    {
                        Y[i] += 2 * (double)Math.Sqrt(Math.Pow(tmp[i], 2) + Math.Pow(b[i], 2)) / nCount;
                    }
                    index += nCount;
                    TotalCnt++;
                }

                for (int i = 0; i < nCount / 2 + 1; i++)
                {
                    Y[i] = Y[i] / TotalCnt;
                }

                f = new double[nCount / 2 + 1];
                for (int i = 0; i < nCount / 2 + 1; i++)
                {
                    double j = (double)i / (nCount / 2);
                    f[i] = fs / 2 * j;
                }
                return true;
            }
            catch (Exception ee)
            {
                throw ee;
            }
        }
    }
}

