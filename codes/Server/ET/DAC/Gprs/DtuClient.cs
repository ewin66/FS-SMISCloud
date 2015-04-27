using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using FS.SMIS_Cloud.DAC.Util;

namespace FS.SMIS_Cloud.DAC.Gprs
{
    public class DtuClient
    {
        public delegate void OnMessageReceived(byte[] buff, int len);
        private IPAddress ip;
        private Socket clientSocket;
        string serverip;
        int serverPort;
        public OnMessageReceived OnReceived;
        private bool running = false;

        byte[] received = new byte[1024];
        private Thread _recvThread;
        private Thread _beatThread;

        public DtuClient(string serverip, int serverport)
        {
            this.serverip = serverip;
            this.serverPort = serverport;
            this.ip = IPAddress.Parse(serverip);
            this.clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
        }
        public void Send(string buff)
        {
            clientSocket.Send(Encoding.ASCII.GetBytes(buff));
        }

        public void Send(byte[] buff)
        {
            clientSocket.Send(buff);
        }

        public bool Connect(int dtuCode, string phone, string ipAddr)
        {
            try
            {
                clientSocket.Connect(new IPEndPoint(ip, serverPort)); //配置服务器IP与端口  
                Console.WriteLine("连接服务器成功");
                RegisterPackage reg = new RegisterPackage { code = dtuCode, phone = phone, ip = ipAddr };
                byte[] buff = reg.ToBytes();
                clientSocket.Send(buff); // Register buffer
                running = true;
                //通过 clientSocket 发送数据  
                byte[] heatBeat = new byte[] { 0xfe };
                _beatThread = new Thread(_OnHeatBeat);
                _beatThread.Start();
                _recvThread = new Thread(_OnReceived);
                _recvThread.Start();
                return true;
            }
            catch
            {
                Console.WriteLine("连接服务器失败，请按回车键退出！");
                return false;
            }
        }

        private void _OnReceived()
        {
            int reccnt = 0;
            while (running)
            {
                try
                {
                    if ((reccnt = clientSocket.Receive(received)) > 0)
                    {
                        if (OnReceived != null) OnReceived(received, reccnt);
                    }
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine("Error: {0}", e.Message);
                    Thread.Sleep(2000);
                }
            }
            Console.WriteLine("Node Client receiver closed.");
        }

        public void Close()
        {
            Console.WriteLine("Closing Client");
            //clientSocket.Close();
            running = false;
            _beatThread.Join();
            _recvThread.Join();
            clientSocket.Close();
        }

        private void _OnHeatBeat()
        {
            byte[] heat = new byte[] { 0xfe };
            while (running)
            {
                clientSocket.Send(heat);
                Thread.Sleep(1000);
            }
            if (!running)
            {
                Console.WriteLine("Node Client heartbeat closed.");
            }
        }

    }


    class RegisterPackage
    {
        /*
           ID：               8byte:   8位HEX ID编号
           PHONE_NUMBER：      11 byte: 11位手机电话号码的ASCII码
           0                   1byte: 
           IP_ADD：            4byte: 动态IP地址（HEX）
           ETX：		       0x00表明数据的结束。
       */
        public int code { get; set; }
        public string phone { get; set; }
        public string ip { get; set; }
        public byte[] ToBytes()
        {
            byte[] buff = new byte[21];
            ValueHelper.WriteBCD(buff, 0, code); //4
            ValueHelper.WriteAscii(buff, 4, phone); //11
            ValueHelper.WriteIP(buff, 16, ip);      //4
            buff[20] = 0x00;
            return buff;
        }

    }
    
}
