using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WIC = System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;

namespace SimpleDotImage
{
    public class ImageHelper
    {
        public static void SaveStream(Stream imageStream, string fileName)
        {
            imageStream.Position = 0;
            using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                int Length = 256;
                Byte[] buffer = new Byte[Length];
                int bytesRead = imageStream.Read(buffer, 0, Length);

                // write the required bytes
                while (bytesRead > 0)
                {
                    fs.Write(buffer, 0, bytesRead);
                    bytesRead = imageStream.Read(buffer, 0, Length);
                }
            }
        }
       
        public static void Deallocate(WIC.ImageBrush iBrush)
        {
            iBrush = null;
        }

        public static void Deallocate(WIC.DrawingBrush dBrush)
        {
            dBrush = null;
        }

        public static void Deallocate(WIC.DrawingImage gImg)
        {
            gImg = null;
        }

        public static void Deallocate(WIC.GeometryDrawing geo)
        {
            geo = null;
        }

        public static void Deallocate(WIC.RectangleGeometry rect)
        {
            rect = null;
        }

        public static void Deallocate(FormatConvertedBitmap jpg)
        {
            jpg = null;
        }

        public static void Deallocate(WIC.DrawingGroup jpg)
        {
            jpg = null;
        }

        public static void Deallocate(JpegBitmapEncoder encoder)
        {
            encoder = null;
        }

        public static void Deallocate(RenderTargetBitmap jpg)
        {
            jpg = null;
        }

        public static void Deallocate(BitmapMetadata metadata)
        {
            metadata = null;
        }

        public static void Deallocate(BitmapFrame frame)
        {
            frame = null;
        }

        public static void Deallocate(BitmapDecoder decoder)
        {
            decoder = null;
        }

        public static void Deallocate(TransformedBitmap tBitmap)
        {
            tBitmap = null;
        }

        public static void Deallocate(WIC.DrawingVisual jpg)
        {
            jpg = null;
        }
    }
}
