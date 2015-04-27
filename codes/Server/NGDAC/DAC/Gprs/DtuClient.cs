namespace FS.SMIS_Cloud.NGDAC.Gprs
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;

    using FS.SMIS_Cloud.NGDAC.Util;

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
            this.clientSocket.Send(Encoding.ASCII.GetBytes(buff));
        }

        public void Send(byte[] buff)
        {
            this.clientSocket.Send(buff);
        }

        public bool Connect(int dtuCode, string phone, string ipAddr)
        {
            try
            {
                this.clientSocket.Connect(new IPEndPoint(this.ip, this.serverPort)); //配置服务器IP与端口  
                Console.WriteLine("连接服务器成功");
                RegisterPackage reg = new RegisterPackage { code = dtuCode, phone = phone, ip = ipAddr };
                byte[] buff = reg.ToBytes();
                this.clientSocket.Send(buff); // Register buffer
                this.running = true;
                //通过 clientSocket 发送数据  
                byte[] heatBeat = new byte[] { 0xfe };
                this._beatThread = new Thread(this._OnHeatBeat);
                this._beatThread.Start();
                this._recvThread = new Thread(this._OnReceived);
                this._recvThread.Start();
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
            while (this.running)
            {
                try
                {
                    if ((reccnt = this.clientSocket.Receive(this.received)) > 0)
                    {
                        if (this.OnReceived != null) this.OnReceived(this.received, reccnt);
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
            this.running = false;
            this._beatThread.Join();
            this._recvThread.Join();
            this.clientSocket.Close();
        }

        private void _OnHeatBeat()
        {
            byte[] heat = new byte[] { 0xfe };
            while (this.running)
            {
                this.clientSocket.Send(heat);
                Thread.Sleep(1000);
            }
            if (!this.running)
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
            ValueHelper.WriteBCD(buff, 0, this.code); //4
            ValueHelper.WriteAscii(buff, 4, this.phone); //11
            ValueHelper.WriteIP(buff, 16, this.ip);      //4
            buff[20] = 0x00;
            return buff;
        }

    }
    
}
