using System;
using System.IO.Ports;
using System.Threading;

namespace Com.Simulator
{
    internal class ComSimulator
    {
        private class ComPair
        {
            public int dtu { get; set; }
            public string Dest { get; set; }
            public string Source { get; set; }
            public SerialPort Port { get; set; }

            const int IDX_SOURCE =7;
            // (data[IDX_SOURCE] << 8) | (data[1 + IDX_SOURCE])
            private byte[] resp =
                StrToToHexByte("fe 46 41 53 17 00 00 1b 76 01 01 0a 07 b4 00 00 00 00 00 00 00 00 19 69 ef");


            public void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
            {
                int count = Port.BytesToRead;
                if (count > 0)
                {

                    byte[] buff = new byte[count];
                    Port.Read(buff, 0, count);

                    short mno = GetShort(buff,5 );
                    byte cno = buff[12];
                    Console.WriteLine("[{0}] Msg from [{1}]: size={2}, buff={3}, m={4}", Source, Dest, count, BytesToHexStr(buff), mno);
                    // ack
                    WriteShort(resp, 7, mno);
                    resp[11] = cno;
                    resp[resp.Length-2] = CheckXor(resp, 0, resp.Length - 2);

                    Port.Write(resp, 0, resp.Length);
                }
            }
        }
 
        public static int Main(string[] args)
        {
            ComPair[] pairs = new ComPair[]
            {
                new ComPair{dtu=2, Dest="COM6",Source = "COM16"},
                new ComPair{dtu=3, Dest="COM14",Source = "COM4"},
                new ComPair{dtu=4, Dest="COM1",Source = "COM11"},
                new ComPair{dtu=5, Dest="COM8",Source = "COM18"},
                new ComPair{dtu=7, Dest="COM7",Source = "COM17"},
                new ComPair{dtu=8, Dest="COM13",Source = "COM3"},
                new ComPair{dtu=9, Dest="COM38",Source = "COM28"},
                new ComPair{dtu=10, Dest="COM9",Source = "COM19"}
            };
            Thread[] ts = new Thread[pairs.Length];
            foreach (ComPair cp in pairs)
            {
                cp.Port = new SerialPort
                {
                    PortName = cp.Source,
                    BaudRate = 9600,
                    DataBits =  8,
                    StopBits = StopBits.One,
                    Parity =  Parity.None
                };
                cp.Port.DataReceived += cp.OnDataReceived;
                cp.Port.Open();
                Console.WriteLine("Port {0} opened for {1}...", cp.Source, cp.Dest);
               new Thread(() =>
                {
                    while (true)
                    {
                         Thread.Sleep(10);
                    }
                }).Start();
            }
            return 1;
        }

        public static string BytesToHexStr(byte[] da, string separator = " ")
        {
            return BytesToHexStr(da, 0, da.Length, separator);
        }

        public static string BytesToHexStr(byte[] da, int start, int length, string separator = " ")
        {
            string str = "";
            if (separator == null)
            {
                separator = " ";
            }
            for (int i = start; i < (start + length); i++)
            {
                str = str + Convert.ToString(da[i], 0x10).PadLeft(2, '0') + separator;
            }
            if (length > 0 && separator.Length > 0)
            {
                str = str.Remove(str.Length - separator.Length);
            }
            return str;
        }

        public static byte[] StrToToHexByte(string hexString)
        {
            string str = hexString;
            str = str.Replace(" ", "").Replace("\n", "").Replace("\r", "");
            int num = str.Length / 2;
            byte[] buffer = new byte[num];
            for (int i = 0; i < num; i++)
            {
                buffer[i] = Convert.ToByte(str.Substring(i * 2, 2), 0x10);
            }
            return buffer;
        }

        public static byte CheckXor(byte[] data, int start, int end)
        {
            byte result = 0;
            for (int i = start; i < end; i++)
            {
                result = (byte)((((int)result) ^ ((int)data[i])) & 0x000000ff);
            }
            return result;
        }

        public static void WriteShort(byte[] buff, int offset, short value)
        {
            byte[] b2 = BitConverter.GetBytes(value);
            Array.Reverse(b2);
            Array.Copy(b2, 0, buff, offset, 2);
        }

        public static short GetShort(byte[] data, int index)
        {
            var by = new byte[2];
            Array.Copy(data, index, by, 0, 2);
            Array.Reverse(by);
            return BitConverter.ToInt16(by, 0);
        }

    }


}
