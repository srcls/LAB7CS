using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Options;

namespace LAB7CS
{
    class Program
    {
        static void Main(string[] args)
        {
            bool show_help = false;
            List<string> names = new List<string>();
            string res = "300x300";
            string currDir = Directory.GetCurrentDirectory();
            DirectoryInfo inputDir = null;
            DirectoryInfo outputDir = Directory.CreateDirectory(currDir + "\\output\\");

            var p = new OptionSet() {
                { "r|res=", "Width x Height resolution.",
                    v => res = v },
                { "id|inputdir=",
                    "Source directory with files.\n",
                    v =>
                    {
                        if (v != null)
                        {
                            inputDir = Directory.CreateDirectory(v+"\\");
                        }
                        else
                        {
                            inputDir = Directory.CreateDirectory(currDir + "\\input\\");
                        }
                    }},
                { "od|outputdir=", "Output directory for files",
                    v => { if (v != null)
                        {
                            outputDir = Directory.CreateDirectory(v+"\\");
                        }

                    } },
                { "h|help",  "show this message and exit",
                    v => show_help = v != null },
            };

            List<string> extra;
            try
            {
                extra = p.Parse(args);
            }
            catch (OptionException e)
            {
                Console.Write("greet: ");
                Console.WriteLine(e.Message);
                Console.WriteLine("Try `greet --help' for more information.");
                return;
            }



            int width = 0;
            int height = 0;
            width = int.Parse(res.Split('x')[0]);
            height = int.Parse(res.Split('x')[1]);
            if (inputDir == null)
            {
                Console.WriteLine("Set input dir!");
                return;
            }
            var files = Directory.EnumerateFiles(inputDir.FullName, "*.*", SearchOption.TopDirectoryOnly)
                .Where(s => s.EndsWith(".jpg") || s.EndsWith(".jpeg") || s.EndsWith(".png") || s.EndsWith(".gif") || s.EndsWith(".bmp") || s.EndsWith(".svg"));


            
            foreach (var file in files)
            {
                bool flag = false;
                int count = 1;
                using (Bitmap bitmap = (Bitmap)Image.FromFile(file))
                {
                    using (Bitmap newBitmap = ResizeImage(bitmap, width, height))
                    {
                        string outFileName = outputDir.FullName + Path.GetFileNameWithoutExtension(file);
                        while (flag == false)
                        {
                            if (File.Exists(outFileName + Path.GetExtension(file)))
                            {
                                if (outFileName.EndsWith("_" + count.ToString()))
                                {
                                    outFileName = outFileName.Remove(outFileName.Length - 1, 1);
                                    count++;
                                    outFileName += count.ToString();
                                }
                                else
                                {
                                    outFileName += "_" + count.ToString();
                                }
                            }
                            else
                            {
                                flag = true;
                            }
                        }
                        newBitmap.Save(outFileName + Path.GetExtension(file));
                    }
                }
            }
        }

        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
    }
}