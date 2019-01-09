using System;
using System.IO;
using NetMQ;
using NetMQ.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication2
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

            using (var pubSocket = new DealerSocket())
            {
                Log("DealerSocket socket binding...");
                pubSocket.Options.SendHighWatermark = 1000;
                pubSocket.Bind("tcp://*:12345");

                var msg = "TopicA msg- 1";
                Log("Sending message : {0}", msg);
                pubSocket.SendMoreFrame("TopicA").SendFrame(msg);

                msg = "TopicB msg- 2";
                Log("Sending message : {0}", msg);
                pubSocket.SendMoreFrame("TopicB").SendFrame(msg);

                Application.Run(new frmMain(pubSocket));

                Log("Application Ended.");
            }

        }

        public static void Log(string mStr, params object[] mParam)
        {
            File.AppendAllText("./PupApp.log.txt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff \t") + (mParam == null ? mStr : string.Format(mStr, mParam)) + Environment.NewLine);
        }
    }
}
