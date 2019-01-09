
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using NetMQ;
using NetMQ.Sockets;


namespace SubscriperApp
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Log("Application Started ..");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (var subSocket = new DealerSocket())
            {
                subSocket.Options.ReceiveHighWatermark = 1000;
                subSocket.Connect("tcp://localhost:12345");
                
                Log("Subscriber socket connecting...");

                Application.Run(new frmMain(subSocket));
            }

            Log("Application Ended.");


        }

        public static void Log(string mStr, params object[] mParam)
        {
            File.AppendAllText("./SubApp.log.txt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff \t") + (mParam == null ? mStr : string.Format(mStr, mParam)) + Environment.NewLine);
        }
    }
}
