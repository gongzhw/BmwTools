using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace BMW.Frameworks.QRCodeHelper
{
    public class QRCodeHelper  
    {
        /// <summary>
        /// 生成二维码
        /// </summary>
        /// <param name="strContent">内容 如果是网址要加http 才能在微信中跳转。其他内容随意</param>
        /// <param name="ms">内存流</param>
        /// <returns></returns>
        public static bool GetQRCode(string strContent, MemoryStream ms)
        {
            ErrorCorrectionLevel Ecl = ErrorCorrectionLevel.M; //误差校正水平 
            string Content = strContent;//待编码内容
            QuietZoneModules QuietZones = QuietZoneModules.Two;  //空白区域 
            int ModuleSize = 12;//大小
            var encoder = new QrEncoder(Ecl);
            QrCode qr;
            if (encoder.TryEncode(Content, out qr))//对内容进行编码，并保存生成的矩阵
            {
                var render = new GraphicsRenderer(new FixedModuleSize(ModuleSize, QuietZones));
                render.WriteToStream(qr.Matrix, ImageFormat.Png, ms);
            }
            else
            {
                return false;
            }
            return true;
        }

        public static void CreateImage(string name, int fontsize, string filePath, int wdith = 100, int higeht = 100)
        {
            Font font = new Font("Arial", fontsize, FontStyle.Bold);
            //绘笔颜色
            SolidBrush brush = new SolidBrush(Color.Red);

            Bitmap image = new Bitmap(wdith, higeht);
            Graphics g = Graphics.FromImage(image);
            g.Clear(ColorTranslator.FromHtml("#f0f0f0"));
            RectangleF rect = new RectangleF(5, 5, wdith, higeht);
            //绘制图片
            g.DrawString(name, font, brush, rect);
            //保存图片
            image.Save(filePath, ImageFormat.Jpeg);
            //释放对象
            g.Dispose();
            image.Dispose();
        }
    }
}
