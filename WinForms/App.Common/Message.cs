using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace App.Common
{
    [Serializable]
    public class Message
    {
        public MessageType MessageType { get; set; }
        public string MessageText { get; set; }
        public Image MessageImage { get; set; }


        #region Serialization

        static BinaryFormatter mSerializer = new BinaryFormatter();

        public byte[] ToBytes()
        {
            byte[] mArray = null;
            using (var mStream = new MemoryStream())
            {
                mSerializer.Serialize(mStream, this);
                mArray = mStream.ToArray();
            }

            return mArray;
        }

        public static Message FromBytes(byte[] mBytes)
        {
            if (mBytes == null || mBytes.Length == 0)
                return new Message();
            else
            {
                using (var mStream = new MemoryStream(mBytes))
                {
                    mStream.Position = 0;
                    return (Message)mSerializer.Deserialize(mStream);
                }
            }
        }

        #endregion

    }

    public enum MessageType
    {
        Text,
        Image
    }
}
