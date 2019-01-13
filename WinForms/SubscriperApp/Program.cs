
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using NetMQ;
using NetMQ.Sockets;
using System.Diagnostics;


namespace ClientApp
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Log("ClientApp Started ..");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            StartSocket(()=> new DealerSocket());

            Log("ClientApp Ended.");


        }

        static void StartSocket<T>(Func<T> mFunction) where T : NetMQSocket
        {
            using (var mSocket = mFunction())
            {
                Log("Socket Type: " + mSocket.GetType().Name);
                Log("Connecting Socket to tcp://localhost:12345");

                //mSocket.Options.ReceiveHighWatermark = 1000;
                mSocket.Connect("tcp://localhost:12345");

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
