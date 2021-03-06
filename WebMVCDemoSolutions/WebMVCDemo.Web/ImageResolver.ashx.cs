﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using System.IO;
using System.Configuration;

namespace WebMVCDemo.Web
{
    /// <summary>
    /// Summary description for ImageResolver
    /// </summary>
    public class ImageResolver : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            string imageName = context.Request.QueryString["imagename"];
            string size = context.Request.QueryString["size"];

            int W = 75;
            int H = 75;

            if (size == "l")
            {
                using (MemoryStream str = new MemoryStream())
                {
                    using (Image img = Image.FromFile(ConfigurationManager.AppSettings["PhotoGalleryPath"] + imageName))
                    {
                        img.Save(str, System.Drawing.Imaging.ImageFormat.Png);
                        img.Dispose();
                        str.WriteTo(context.Response.OutputStream);
                        str.Dispose();
                        str.Close();
                    }
                }
            }

            else
            {
                Image _img = null;

                using (FileStream fs = new FileStream(ConfigurationManager.AppSettings["PhotoGalleryPath"] + imageName, FileMode.Open, FileAccess.Read))
                {
                    using (Image img = Image.FromStream(fs))
                    {
                        if (size == "m")
                        {
                            W = img.Width / 2;
                            H = img.Height / 2;
                        }
                        else if (size == "500")
                        {
                            W = 500;
                            H = 500;
                        }

                        _img = new Bitmap(W, H);

                        using (Graphics g = Graphics.FromImage(_img))
                        {
                            g.DrawImage(img, 0, 0, W, H);
                            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                            g.Dispose();
                        }
                        img.Dispose();
                    }
                }


                //using (Image img = Image.FromFile(ConfigurationManager.AppSettings["PhotoGalleryPath"] + imageName))
                //{
                //    if (size == "m")
                //    {
                //        W = img.Width / 2;
                //        H = img.Height / 2;
                //    }
                //    else if (size == "500")
                //    {
                //        W = 500;
                //        H = 500;
                //    }

                //    _img = new Bitmap(W, H);

                //    using (Graphics g = Graphics.FromImage(_img))
                //    {
                //        g.DrawImage(img, 0, 0, W, H);
                //        g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                //        g.Dispose();
                //    }
                //    img.Dispose();
                //}

                using (MemoryStream str = new MemoryStream())
                {
                    _img = _img.GetThumbnailImage(W, H, null, IntPtr.Zero);
                    _img.Save(str, System.Drawing.Imaging.ImageFormat.Png);
                    _img.Dispose();

                    str.WriteTo(context.Response.OutputStream);

                    str.Close();
                    str.Dispose();
                }
            }

            GC.Collect(1, GCCollectionMode.Forced, false);

            context.Response.ContentType = ".png";
            context.Response.End();
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}