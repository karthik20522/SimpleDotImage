using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WIC = System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;

namespace SimpleDotImage
{
    //You need to clean up the thread manually as they will still reside in memory if they are not flagged for termination. 
    //Thread count will go through the roof on the server if you dont invoke the dipatcher calls. 
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

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        public static void Deallocate(WIC.ImageBrush iBrush)
        {
            if (iBrush != null && iBrush.Dispatcher != null && iBrush.Dispatcher.Thread != null && iBrush.Dispatcher.Thread.IsAlive)
                iBrush.Dispatcher.InvokeShutdown();
            iBrush = null;
        }

        public static void Deallocate(WIC.DrawingBrush dBrush)
        {
            if (dBrush != null && dBrush.Dispatcher != null && dBrush.Dispatcher.Thread != null && dBrush.Dispatcher.Thread.IsAlive)
                dBrush.Dispatcher.InvokeShutdown();
            dBrush = null;
        }

        public static void Deallocate(WIC.DrawingImage gImg)
        {
            if (gImg != null && gImg.Dispatcher != null && gImg.Dispatcher.Thread != null && gImg.Dispatcher.Thread.IsAlive)
                gImg.Dispatcher.InvokeShutdown();
            gImg = null;
        }

        public static void Deallocate(WIC.GeometryDrawing geo)
        {
            if (geo != null && geo.Dispatcher != null && geo.Dispatcher.Thread != null && geo.Dispatcher.Thread.IsAlive)
                geo.Dispatcher.InvokeShutdown();
            geo = null;
        }

        public static void Deallocate(WIC.RectangleGeometry rect)
        {
            if (rect != null && rect.Dispatcher != null && rect.Dispatcher.Thread != null && rect.Dispatcher.Thread.IsAlive)
                rect.Dispatcher.InvokeShutdown();
            rect = null;
        }

        public static void Deallocate(FormatConvertedBitmap jpg)
        {
            if (jpg != null && jpg.Dispatcher != null && jpg.Dispatcher.Thread != null && jpg.Dispatcher.Thread.IsAlive)
                jpg.Dispatcher.InvokeShutdown();
            jpg = null;
        }

        public static void Deallocate(WIC.DrawingGroup jpg)
        {
            if (jpg != null && jpg.Dispatcher != null && jpg.Dispatcher.Thread != null && jpg.Dispatcher.Thread.IsAlive)
                jpg.Dispatcher.InvokeShutdown();
            jpg = null;
        }

        public static void Deallocate(JpegBitmapEncoder encoder)
        {
            if (encoder != null && encoder.Dispatcher != null && encoder.Dispatcher.Thread.IsAlive)
                encoder.Dispatcher.InvokeShutdown();

            encoder = null;
        }

        public static void Deallocate(RenderTargetBitmap jpg)
        {
            if (jpg != null && jpg.Dispatcher != null && jpg.Dispatcher.Thread != null && jpg.Dispatcher.Thread.IsAlive)
                jpg.Dispatcher.InvokeShutdown();
            jpg = null;
        }

        public static void Deallocate(BitmapMetadata metadata)
        {
            if (metadata != null && metadata.Dispatcher != null && metadata.Dispatcher.Thread.IsAlive)
                metadata.Dispatcher.InvokeShutdown();

            metadata = null;
        }

        public static void Deallocate(BitmapFrame frame)
        {
            if (frame != null && frame.Dispatcher != null && frame.Dispatcher.Thread.IsAlive)
                frame.Dispatcher.InvokeShutdown();

            frame = null;
        }

        public static void Deallocate(BitmapDecoder decoder)
        {
            if (decoder != null && decoder.Dispatcher != null && decoder.Dispatcher.Thread.IsAlive)
                decoder.Dispatcher.InvokeShutdown();

            decoder = null;
        }

        public static void Deallocate(TransformedBitmap tBitmap)
        {
            if (tBitmap != null && tBitmap.Dispatcher != null && tBitmap.Dispatcher.Thread.IsAlive)
                tBitmap.Dispatcher.InvokeShutdown();

            tBitmap = null;
        }

        public static void Deallocate(WIC.DrawingVisual jpg)
        {
            if (jpg != null && jpg.Dispatcher != null && jpg.Dispatcher.Thread != null && jpg.Dispatcher.Thread.IsAlive)
                jpg.Dispatcher.InvokeShutdown();

            jpg = null;
        }
    }
}
