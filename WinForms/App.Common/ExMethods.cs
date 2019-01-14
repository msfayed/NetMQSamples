using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace App.Common
{
    public static class ExMethods
    {
        public static byte[] ToBytes(this Image mImage, ImageFormat mFormat = null)
        {
            byte[] mRet = null;
            if (mImage != null)
            {
                using (MemoryStream mStream = new MemoryStream())
                {
                    mImage.Save(mStream, mFormat == null ? mImage.RawFormat : mFormat);
                    mRet = mStream.ToArray();
                }
            }
            return mRet;

        }

        public static string ToHexString(this byte[] mInput)
        {
            var result = new StringBuilder();

            if (mInput != null && mInput.Length > 0)
                for (int i = 0; i < mInput.Length; i++)
                {
                    if (i != 0 && i % 60 == 0)
                    {
                        result.AppendLine();
                    }
                    result.AppendFormat("{0:x2}", mInput[i]);
                }

            return result.ToString();
        }


        public static void AppendLine(this RichTextBox mTextBox)
        {
            mTextBox.AppendText("==Line==");

            mTextBox.Rtf = mTextBox.Rtf.Replace("==Line==", @"{\pict\wmetafile8\picw26\pich26\picwgoal20000\pichgoal15 
            0100090000035000000000002700000000000400000003010800050000000b0200000000050000
            000c0202000200030000001e000400000007010400040000000701040027000000410b2000cc00
            010001000000000001000100000000002800000001000000010000000100010000000000000000
            000000000000000000000000000000000000000000ffffff00000000ff040000002701ffff0300
            00000000
            }");

        }
        public static void AppendImage(this RichTextBox mTextBox, Image mImage)
        {
            /*
             In RTF, a picture is defined like this:

             '{' \pict (brdr? & shading? & picttype & pictsize & metafileinfo?) data '}' 
             
             A question mark indicates the control word is optional. 
             "data" is simply the content of the file in hex format. 
             If you want to use binary, use the \bin control word.
             
             \pict = starts a picture group, 
             \pngblip = png picture 
             \picwX = width of the picture (X is the pixel value) 
             \pichX = height of the picture 
             \picwgoalX = desired width of the picture in twips
             
            */

            var _width = (mImage.Width / mImage.HorizontalResolution) * 72;
            var _height = (mImage.Height / mImage.VerticalResolution) * 72;

            var result = new StringBuilder();
            result.Append(@"{\pict\wmetafile8");
            result.Append(@"\picw" + _width);
            result.Append(@"\pich" + _height);
            result.Append(@"\picwgoal" + pt2Twip(_width));
            result.Append(@"\pichgoal" + pt2Twip(_height));
            result.AppendLine(@" ");

            //result.Append(BitConverter.ToString(mImage.ToBytes(ImageFormat.Jpeg)).Replace("-", string.Empty).ToLower());
            result.Append(MakeMetafileStream(mImage).ToArray().ToHexString());
            
            result.Append(@"}");

            mTextBox.AppendText("==Image==");
            mTextBox.Rtf = mTextBox.Rtf.Replace("==Image==", result.ToString());
            

            //// another method , that I didn't like :)
            //Clipboard.SetImage(mImage);
            //mTextBox.Paste();
            //mTextBox.AppendText(Environment.NewLine);

        }

        private static int pt2Twip(float pt)
        {
            return !float.IsNaN(pt) ? Convert.ToInt32(pt * 20) : 0;
        }


        [Flags]
        private enum EmfToWmfBitsFlags
        {
            EmfToWmfBitsFlagsDefault = 0x00000000,
            EmfToWmfBitsFlagsEmbedEmf = 0x00000001,
            EmfToWmfBitsFlagsIncludePlaceable = 0x00000002,
            EmfToWmfBitsFlagsNoXORClip = 0x00000004
        }

        private static int MM_ISOTROPIC = 7;
        private static int MM_ANISOTROPIC = 8;

        [DllImport("gdiplus.dll")]
        private static extern uint GdipEmfToWmfBits(IntPtr _hEmf, uint _bufferSize, byte[] _buffer, int _mappingMode, EmfToWmfBitsFlags _flags);
        [DllImport("gdi32.dll")]
        private static extern IntPtr SetMetaFileBitsEx(uint _bufferSize, byte[] _buffer);
        [DllImport("gdi32.dll")]
        private static extern IntPtr CopyMetaFile(IntPtr hWmf, string filename);
        [DllImport("gdi32.dll")]
        private static extern bool DeleteMetaFile(IntPtr hWmf);
        [DllImport("gdi32.dll")]
        private static extern bool DeleteEnhMetaFile(IntPtr hEmf);

        public static MemoryStream MakeMetafileStream(Image image)
        {
            Metafile metafile = null;
            using (Graphics g = Graphics.FromImage(image))
            {
                IntPtr hDC = g.GetHdc();
                metafile = new Metafile(hDC, EmfType.EmfOnly);
                g.ReleaseHdc(hDC);
            }

            using (Graphics g = Graphics.FromImage(metafile))
            {
                g.DrawImage(image, 0, 0);
            }
            IntPtr _hEmf = metafile.GetHenhmetafile();
            uint _bufferSize = GdipEmfToWmfBits(_hEmf, 0, null, MM_ANISOTROPIC, EmfToWmfBitsFlags.EmfToWmfBitsFlagsDefault);
            byte[] _buffer = new byte[_bufferSize];
            GdipEmfToWmfBits(_hEmf, _bufferSize, _buffer, MM_ANISOTROPIC, EmfToWmfBitsFlags.EmfToWmfBitsFlagsDefault);
            DeleteEnhMetaFile(_hEmf);

            var stream = new MemoryStream();
            stream.Write(_buffer, 0, (int)_bufferSize);
            stream.Seek(0, 0);

            return stream;
        }
    }
}
