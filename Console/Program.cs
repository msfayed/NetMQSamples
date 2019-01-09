using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Core;
using NetMQ.Sockets;
using System.Threading;
using System.Reflection;
using System.IO;

namespace Project2
{
    class Program
    {
        private const int mDelayTime = 1000;

        static void Main(string[] args)
        {
            if (args != null && args.Length > 0)
            {
                if (args[0].ToLower().StartsWith("s")) // server 
                {
                    Console.WriteLine("Startting The Server .. ");

                    using (var socket = new ResponseSocket("tcp://*:5555"))
                    {
                        Console.WriteLine("Server Started.");
                        while (true)
                        {
                            //Thread.Sleep(mDelayTime);
                            var rcvdMsg = socket.ReceiveFrameString(Encoding.UTF8);
                            Console.WriteLine("Received: " + rcvdMsg);
                            var replyMsg = "Message Received : " + rcvdMsg;
                            Console.WriteLine("Sending : " + replyMsg + Environment.NewLine);
                            socket.SendFrame(replyMsg);
                        }
                    }
                }
                else // Client
                {
                    var mID = args[0].Trim(); //DateTime.Now.ToString("HH-mm-ss-fff");
                    Console.WriteLine("Startting The Client .. " + mID);


                    using (var socket = new RequestSocket("tcp://127.0.0.1:5555"))
                    {
                        Console.WriteLine("Client Started.");
                        while (true)
                        {
                            Thread.Sleep(mDelayTime);

                            var replyMsg = "Hello from Client [" + mID + "] " + DateTime.Now.ToString("HH:mm:ss.fff");
                            Console.WriteLine("Sending : " + replyMsg + Environment.NewLine);

                            socket.SendFrame(replyMsg);
                            var rcvdMsg = socket.ReceiveFrameString(Encoding.UTF8);
                            Console.WriteLine("Received: " + rcvdMsg);
                        }
                    }



                }

            }
            else
            {
                var mAppPath = new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath;

                System.Diagnostics.Process.Start(mAppPath, "s");
                System.Diagnostics.Process.Start(mAppPath, "c1");
                System.Diagnostics.Process.Start(mAppPath, "c2");
            }

        }
    }
}
