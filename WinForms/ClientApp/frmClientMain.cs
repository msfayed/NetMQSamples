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
using System.IO;
using System.Drawing.Imaging;
using App.Common;


namespace ClientApp
{
    public partial class frmClientMain : Form
    {
        NetMQSocket mSocket = null;
        NetMQPoller poller = null;

        public frmClientMain(NetMQSocket mSocket)
        {
            this.mSocket = mSocket;
            InitializeComponent();

            poller = new NetMQPoller { mSocket };
            mSocket.ReceiveReady += MSocket_ReceiveReady;

            // start polling (on this thread)
            poller.RunAsync();

            if (mSocket.HasOut)
            {
                mSocket.SendFrame((new App.Common.Message() { MessageType = App.Common.MessageType.Image, MessageImage = Image.FromFile(@"C:\Users\Public\Pictures\Sample Pictures\Penguins.jpg") }).ToBytes());
                mSocket.SendFrame((new App.Common.Message() { MessageType = App.Common.MessageType.Text, MessageText = "Hiii" }).ToBytes());

            }
            else
                Program.Log("ClientApp mSocket.HasOut == null !");
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            poller.Stop();
            base.OnClosing(e);
        }

        private void MSocket_ReceiveReady(object sender, NetMQSocketEventArgs e)
        {
            //var mData = e.Socket.ReceiveFrameString();
            var mData = e.Socket.ReceiveFrameBytes();
            var mMsg = App.Common.Message.FromBytes(mData);

            Program.Log("ClientApp Received : " + mMsg.MessageType.ToString() + " - " + mMsg.MessageText);

            Action mAction = delegate
            {
                if (mMsg.MessageType == App.Common.MessageType.Text)
                    richTextBox1.AppendText(mMsg.MessageType.ToString() + " - " + mMsg.MessageText + Environment.NewLine);
                else if (mMsg.MessageType == App.Common.MessageType.Image)
                {
                    //Clipboard.SetImage(mMsg.MessageImage);
                    //richTextBox1.Paste();
                    //richTextBox1.AppendText(Environment.NewLine);
                    richTextBox1.AppendImage(mMsg.MessageImage);
                }

                richTextBox1.Refresh();
            };

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

            var mMsg = new App.Common.Message();
            mMsg.MessageType = App.Common.MessageType.Text;
            mMsg.MessageText = textBox1.Text;

            mSocket.SendFrame(mMsg.ToBytes());

            textBox1.SelectAll();
        }
    }
}
