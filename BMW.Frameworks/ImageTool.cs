using System;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

namespace BMW.Frameworks
{
    public class ImageTool
    {
        //图片 转为    base64编码的文本
        public static string ImgToBase64String(Bitmap bmp)
        {
            MemoryStream ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Jpeg);
            byte[] arr = new byte[ms.Length];
            ms.Position = 0;
            ms.Read(arr, 0, (int)ms.Length);
            ms.Close();
            String strbaser64 = Convert.ToBase64String(arr);

            return strbaser64;
        }

        public static Bitmap Base64StringToImage(string base64Img)
        {
            //byte[] bytes = Convert.FromBase64String(base64Img);
            byte[] bytes = Convert.FromBase64String(base64Img.Substring("data:image/png;base64,".Length));

            MemoryStream ms = new MemoryStream();
            ms.Write(bytes, 0, bytes.Length);
            Bitmap bmp = new Bitmap(ms);

            return bmp;
        }

        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="base64Img"></param>
        /// <param name="imgPath"></param>
        /// <param name="imgFormat">EX: System.Drawing.Imaging.Jpeg </param>
        public static void SaveFile(string base64Img, string imgPath, ImageFormat imgFormat)
        {
            string dir = Path.GetDirectoryName(imgPath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            var bitmap = Base64StringToImage(base64Img);
            bitmap.Save(imgPath, imgFormat);
        }
    }
}