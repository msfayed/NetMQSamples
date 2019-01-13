using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NetMQ;
using NetMQ.Sockets;


namespace ClientApp
{
    public partial class frmMain : Form
    {
        NetMQSocket mSocket = null;
        NetMQPoller poller = null;

        public frmMain(NetMQSocket mSocket)
        {
            this.mSocket = mSocket;
            InitializeComponent();

            poller = new NetMQPoller { mSocket };
            mSocket.ReceiveReady += MSocket_ReceiveReady;

            // start polling (on this thread)
            poller.RunAsync();

            mSocket.SendFrame("ClientApp is ready.");

        }

        protected override void OnClosing(CancelEventArgs e)
        {
            poller.Stop();
            base.OnClosing(e);
        }

        private void MSocket_ReceiveReady(object sender, NetMQSocketEventArgs e)
        {
            var mData = e.Socket.ReceiveFrameString();

            Program.Log("ClientApp Received : " + mData);

            Action mAction = delegate { richTextBox1.AppendText(mData + Environment.NewLine); };

            if (richTextBox1.InvokeRequired)
                richTextBox1.Invoke(mAction);
            else
                mAction();

        }

        private void frmMain_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Program.Log("ClientApp Sending " + textBox1.Text);

            mSocket.SendFrame(textBox1.Text);

            textBox1.SelectAll();
        }
    }
}
