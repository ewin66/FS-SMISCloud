using System;
using System.Threading;

namespace DataCenter.Communication.comm.Comm
{
    class Program
    {
        static void Main2(string[] args)
        {


            //Hashtable has =new Hashtable();

            //has.Add(1,2);
            //has.Add(1,23);

            //has.Add(1,34);

            ServerManager.Start();
            Thread.Sleep(5000);
            DtuConsumer c1 = new DtuConsumer("C11", "12345678");
            DtuConsumer2 c2 = new DtuConsumer2("C12", "22345678");
            DtuConsumer c3 = new DtuConsumer("C13", "12345678");

            DtuConsumer c21 = new DtuConsumer("C21", "22345678");
            //while (true)
            {
                Thread t1 = new Thread(c1.DoWork);
                Thread t2 = new Thread(c2.DoWork);
                Thread t3 = new Thread(c3.DoWork);
                Thread t21 = new Thread(c21.DoWork);

                t1.Start(); 
                //t2.Start(); 
                //t3.Start();  
                t1.Join(); 
                //t2.Join(); 
                //t3.Join(); 

                Thread.Sleep(2000);

                t1 = new Thread(c1.DoWork2);
                t1.Start();
            }

            Console.ReadLine();
        }

        static void Main1(string[] args)
        {
            ServerManager.Start();
            while (true)
            {
                byte[] toSend = System.Text.Encoding.UTF8.GetBytes("Hello2"); 
                Connection c = ServerManager.GetConnection("12345678");
                if (c != null) {
                    c.registerDataHandler(new Myhandler());
                    if (c.IsAvaliable())
                    {
                       // c.Send(toSend);
                    }
                }
                Thread.Sleep(1000);
            }
        }
    }
    class DtuConsumer
    {
        string dtuid;
        string name;
        public DtuConsumer(string name, string id)
        {
            this.name = name;
            this.dtuid = id;
        }
        private Myhandler handle = new Myhandler();
        private Connection c1;

        public void DoWork()
        {
            this.c1 = ServerManager.GetConnection(this.dtuid);
            // sync.
            byte[] toSend = System.Text.Encoding.UTF8.GetBytes(string.Format("Hello from {0}", this.name)); 
            Console.Write("[{0}]: synchronized sending...", this.name);
            if (this.c1 != null && this.c1.IsAvaliable())
            {
                DtuMsg received = this.c1.Ssend(toSend, 2);
                Console.WriteLine(" Received: {0}", received == null ? "null" : System.Text.Encoding.Default.GetString(received.Databuffer));
            }
            else
            {
                Console.WriteLine("DTU Not Ready");
            }
        }


        public void DoWork2()
        {
            this.c1 = ServerManager.GetConnection(this.dtuid);
           
            // c.registerDataHandler(new Myhandler());
            if (this.c1 != null)
            {
                this.c1.registerDataHandler(this.handle);
                if (this.c1.IsAvaliable())
                {
                    this.c1.Send(System.Text.Encoding.Default.GetBytes("Hello"));
                }
            }
            Thread.Sleep(1000);
        }
    }


    class DtuConsumer2
    {
        string dtuid;
        string name;
        public DtuConsumer2(string name, string id)
        {
            this.name = name;
            this.dtuid = id;
        }
        private Connection c;

        private Myhandler handle = new Myhandler();
        

        public void DoWork()
        {
            while (true)
            {
                this.c = ServerManager.GetConnection(this.dtuid);
                // c.registerDataHandler(new Myhandler());
                if (this.c != null)
                {
                    this.c.registerDataHandler(this.handle);
                    if (this.c.IsAvaliable())
                    {
                        // c.Send(toSend);
                    }
                }
                Thread.Sleep(1000);
            }
            
        }

    }


    class Myhandler : IDtuDataHandler
    {
        public void OnDataReceived(DtuMsg buffer)
        {
            Console.WriteLine("Received: {0}", buffer.DtuId);
        }
    }
}
