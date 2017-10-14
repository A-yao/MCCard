using System;
using System.Drawing;
using XBitmap;

namespace MCCard.Common
{
    public class Captcha : CommonFun
    {
        public byte[] GetCaptcha(string root = null)
        {
            byte[] CaptchaImage = null;
            using (Font font = new Font("Courier New", 21, FontStyle.Bold))
            {
                string password = RandomPassword.Generate(5, 5);
                if (root == "Login")
                {
                    Session[SessionName.CaptchaLogin.ToString()] = password;
                }
                else
                {
                    Session[SessionName.Captcha.ToString()] = password;
                }


                Size size = this.GetTextSize(font, password.ToString());

                Bitmap bmp = new Bitmap(size.Width + 20, (int)(size.Height * 2.1));

                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.White);

                    g.DrawString(Convert.ToString(password), font, Brushes.Blue, 3, 3);

                    CaptchaImage = bmp.ConvertByteArray();
                }
            }
            return CaptchaImage;
        }
        #region RepositionPoint
        /// <summary>
        /// 
        /// </summary>
        internal class RepositionPoint
        {
            public int StartY = -1;
            public int EndY = -1;
            public int StartX = -1;
            public int EndX = -1;
        }
        #endregion

        #region GetTextSize()
        /// <summary>
        /// 
        /// </summary>
        /// <param name="TextFont"></param>
        /// <param name="Text"></param>
        /// <returns></returns>
        internal Size GetTextSize(Font TextFont, string Text)
        {
            Bitmap bmp = null;
            try
            {
                int ih = TextFont.Height;

                int iw = ih * Text.Length;

                bmp = new Bitmap(iw, ih);

                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.White);
                    g.DrawString(Text,
                        TextFont,
                        new SolidBrush(Color.Blue),
                        0,
                        0);
                    g.Dispose();
                }
                RepositionPoint newPoints = this.Reposition(ref bmp);

                return new Size(newPoints.EndX - newPoints.StartX, newPoints.EndY - newPoints.StartY);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                bmp.DisposeBitmap();
            }
        }
        #endregion

        #region Reposition()
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Bmp"></param>
        /// <returns></returns>
        internal RepositionPoint Reposition(ref Bitmap Bmp)
        {
            return this.Reposition(ref Bmp, 254);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Bmp"></param>
        /// <param name="Threshold"></param>
        /// <returns></returns>
        internal RepositionPoint Reposition(ref Bitmap Bmp, byte Threshold)
        {
            int nstartY = 0, nendY = Bmp.Height, nstartX = 0, nendX = Bmp.Width;

            XBitmap.ProjectionInfo hp = Bmp.HorizontalProjection(Threshold);

            for (int pi = 0; pi < hp.Result.Length; pi++)
            {
                if (!hp.Result[pi].Equals(0))
                {
                    nstartY = pi; break;
                }
            }
            for (int pi = hp.Result.Length - 1; pi >= 0; pi--)
            {
                if (!hp.Result[pi].Equals(0))
                {
                    nendY = pi; break;
                }
            }
            hp = null;

            XBitmap.ProjectionInfo vp = Bmp.VerticalProjection(Threshold);

            for (int pi = 0; pi < vp.Result.Length; pi++)
            {
                if (!vp.Result[pi].Equals(0))
                {
                    nstartX = pi; break;
                }
            }
            for (int pi = vp.Result.Length - 1; pi >= 0; pi--)
            {
                if (!vp.Result[pi].Equals(0))
                {
                    nendX = pi; break;
                }
            }
            vp = null;

            return new RepositionPoint() { StartY = nstartY, EndY = nendY, StartX = nstartX, EndX = nendX };
        }
        #endregion
    }
}