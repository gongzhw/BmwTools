using System;
using System.Text;
using System.Web.Mvc;
using System.Drawing;
using System.Drawing.Imaging;

namespace BMW.Frameworks.CustomActionResult
{
    public class VerificationCodeActionResult : ActionResult
    {
        public Color BackGroundColor {get;set;}
        public Color RandomTextColor { get; set; }
        public string RandomWord
        {
            get;
            set;
        }

        /// <summary>
        /// 验证码的最大长度
        /// </summary>
        public int MaxLength
        {
            get;
            set;
        }

        public int ImgWidth { get; set; }
        public int ImgHeight { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            RenderCAPTCHAImage(context);
        }

        private void RenderCAPTCHAImage(ControllerContext context)
        {
            ImgHeight = ImgHeight <= 0 ? 150 : ImgHeight;
            ImgHeight = ImgHeight <= 0 ? 40 : ImgHeight;

            Bitmap objBMP = new System.Drawing.Bitmap(ImgWidth, ImgHeight);
            Graphics objGraphics = System.Drawing.Graphics.FromImage(objBMP);

            objGraphics.Clear(BackGroundColor);

            // Instantiate object of brush with black color
            SolidBrush objBrush = new SolidBrush(RandomTextColor);

            Font objFont = null;
            int a;
            String myFont, str;

            //Creating an array for most readable yet cryptic fonts for OCR's
            // This is entirely up to developer's discretion
            String[] crypticFonts = new String[11];

            crypticFonts[0] = "Arial";
            crypticFonts[1] = "Verdana";
            crypticFonts[2] = "Comic Sans MS";
            crypticFonts[3] = "Impact";
            crypticFonts[4] = "Haettenschweiler";
            crypticFonts[5] = "Lucida Sans Unicode";
            crypticFonts[6] = "Garamond";
            crypticFonts[7] = "Courier New";
            crypticFonts[8] = "Book Antiqua";
            crypticFonts[9] = "Arial Narrow";
            crypticFonts[10] = "Estrangelo Edessa";

            //Loop to write the characters on image  
            // with different fonts.

            if (string.IsNullOrEmpty(RandomWord))
            {
                RandomWord = SelectRandomWord();
            }
            context.HttpContext.Session["vcode"] = RandomWord;
            for (a = 0; a <= RandomWord.Length - 1; a++)
            {
                myFont = crypticFonts[new Random().Next(a)];
                objFont = new Font(myFont, 18, FontStyle.Bold | FontStyle.Italic | FontStyle.Strikeout);
                str = RandomWord.Substring(a, 1);
                objGraphics.DrawString(str, objFont, objBrush, a * 20, 5);
                objGraphics.Flush();
            }
            context.HttpContext.Response.ContentType = "image/GF";
            objBMP.Save(context.HttpContext.Response.OutputStream, ImageFormat.Gif);
            objFont.Dispose();
            objGraphics.Dispose();
            objBMP.Dispose();
        }

        private string SelectRandomWord()
        {
            if (MaxLength > 36)
            {
                throw new InvalidOperationException("Random Word Charecters can not be greater than 36.");
            }
            // Creating an array of 26 characters  and 0-9 numbers
            char[] columns = new char[36];

            for (int charPos = 65; charPos < 65 + 26; charPos++)
                columns[charPos - 65] = (char)charPos;

            for (int intPos = 48; intPos <= 57; intPos++)
                columns[26 + (intPos - 48)] = (char)intPos;

            StringBuilder randomBuilder = new StringBuilder();
            // pick charecters from random position
            Random randomSeed = new Random();
            for (int incr = 0; incr < MaxLength; incr++)
            {
                randomBuilder.Append(columns[randomSeed.Next(36)].ToString());
            }
            return randomBuilder.ToString();
        }

    }
}
