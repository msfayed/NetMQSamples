using System;
using System.IO;
using NetMQ;
using NetMQ.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace ServerApp
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Log("ServerApp Started ..");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            StartSocket(() => new RouterSocket());

            Log("ServerApp Ended.");

        }


        static void StartSocket<T>(Func<T> mFunction) where T : NetMQSocket
        {
            using (var mSocket = mFunction())
            {
                Log("Socket Type: " + mSocket.GetType().Name);
                Log("Socket binding to tcp://*:12345");

                //mSocket.Options.SendHighWatermark = 1000;

                mSocket.Bind("tcp://*:12345");

                //var msg = "TopicA msg- 1";
                //Log("Sending message : {0}", msg);
                ////mSocket.SendMoreFrame("TopicA").SendFrame(msg);
                //mSocket.SendFrame(msg);

                //msg = "TopicB msg- 2";
                //Log("Sending message : {0}", msg);

                //mSocket.SendMoreFrame("TopicB").SendFrame(msg);

                Application.Run(new frmMain(mSocket));
            }
        }



        public static void Log(string mStr, params object[] mParam)
        {
            var mMsg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff \t") +
                        (mParam == null ? mStr : string.Format(mStr, mParam)) +
                        Environment.NewLine;

            if (Debugger.IsAttached)
                Debug.Write(mMsg);
            else

                File.AppendAllText("./App.log.txt", mMsg);
        }
    }
}
