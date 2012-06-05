using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Threading.Tasks;

namespace SimpleDotImage
{
    public class ImageProcessing : IImageProcessing, IDisposable
    {
        private BitmapMetadata metadata;
        private List<ColorContext> colorContexts;
        private BitmapFrame thumbnail;
        private Stream waterMarkImageStream = null;

        #region Dispose
        ~ImageProcessing()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            System.GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // do managed cleanup here, primarily by calling other objects' Dispose method.
                ImageHelper.Deallocate(thumbnail);
                ImageHelper.Deallocate(metadata);

                if(waterMarkImageStream != null)
                    waterMarkImageStream.Dispose();
                
                thumbnail = null;
                metadata = null;
            }
            // do unmanaged cleanup here,
        }
        #endregion

        public Stream Process(string imagePath, int resize = 0, string waterMarkPath = "", double waterMarkOpacity = 0.4, int rotate = 0, int pictureQuality = 85)
        {
            if (!File.Exists(imagePath)) 
                throw new FileNotFoundException("image file not found");

            if (!string.IsNullOrEmpty(waterMarkPath) && File.Exists(waterMarkPath))
                waterMarkImageStream = File.Open(waterMarkPath, FileMode.Open);

            using (var file = File.Open(imagePath, FileMode.Open))
            {
                var photoDecoder = BitmapDecoder.Create(file, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.None);
                var imageFrame = photoDecoder.Frames[0]; //get the image frame
              
                try
                {
                    thumbnail = imageFrame.Thumbnail == null ? null : imageFrame.Thumbnail.Clone() as BitmapFrame;
                    metadata = imageFrame.Metadata as BitmapMetadata ?? imageFrame.Metadata.Clone() as BitmapMetadata;
                    colorContexts = imageFrame.ColorContexts == null ? null : imageFrame.ColorContexts.ToList();
                }
                catch //corrupted image metadata
                {   
                    metadata = new BitmapMetadata("jpg");
                    metadata.SetQuery("/app1/ifd/exif/subifd:{uint=40961}", (ushort)1); //sRGB color space by default
                }

                var resizeTask = Task<BitmapFrame>.Factory.StartNew(() =>{
                    imageFrame = ResizeImage(imageFrame, resize);
                    
                    if(imageFrame != null)
                        imageFrame.Freeze(); //no more modifiction to the resized image

                    return imageFrame;
                });

                var waterMarkTask = Task<ImageBrush>.Factory.StartNew(() =>
                {
                    var waterMarkImage = BuildWaterMark(waterMarkImageStream, waterMarkOpacity);

                    if(waterMarkImage != null)
                        waterMarkImage.Freeze(); //no more modification to the watermark image

                    return waterMarkImage;
                });

                //These operations can be processed in parallel and stiched back together
                Task.WaitAll(resizeTask, waterMarkTask);

                imageFrame = MergeLayers(resizeTask.Result, waterMarkTask.Result);
                return GenerateNewJPEGImage(imageFrame, pictureQuality);
            }
        }

        private Stream GenerateNewJPEGImage(BitmapFrame targetFrame, int pictureQuality)
        {
            if (targetFrame == null)
                return null;
            
            var memoryStream = new MemoryStream();
            
            var targetEncoder = new JpegBitmapEncoder();
            targetEncoder.QualityLevel = pictureQuality;
            targetEncoder.Frames.Add(targetFrame);
            targetEncoder.Save(memoryStream);

            ImageHelper.Deallocate(targetEncoder);
            ImageHelper.Deallocate(targetFrame);
            ImageHelper.Deallocate(thumbnail);

            return memoryStream;
        }

        private ImageBrush BuildWaterMark(Stream waterMarkImage, double waterMarkOpacity)
        {
            if (waterMarkImage == null)
                return null;
            
            var wmDecoder = BitmapDecoder.Create(waterMarkImage, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.None);
            var wmFrame = wmDecoder.Frames[0] as BitmapFrame; //get the watermark frame
            wmFrame.Freeze();

            ImageBrush brush = new ImageBrush(wmFrame);
            brush.Stretch = Stretch.Uniform;
            brush.TileMode = TileMode.None;
            brush.AlignmentX = AlignmentX.Center;
            brush.AlignmentY = AlignmentY.Center;
            brush.Opacity = waterMarkOpacity;

            brush.Freeze(); //no more modifiction to watermark image

            ImageHelper.Deallocate(wmDecoder);
            ImageHelper.Deallocate(wmFrame);

            return brush;
        }

        private BitmapFrame ResizeImage(BitmapFrame originalFrame, int newImageSize)
        {
            if (newImageSize < 1)
                return originalFrame;

            //resize calculation
            var width = (originalFrame.Width > originalFrame.Height ? newImageSize : (int)(originalFrame.Width * newImageSize / originalFrame.Height));
            var height = (originalFrame.Width < originalFrame.Height ? newImageSize : (int)(originalFrame.Height * newImageSize / originalFrame.Width));

            var target = new TransformedBitmap(originalFrame,
                new ScaleTransform(width / originalFrame.Width * 96 / originalFrame.DpiX, height / originalFrame.Height * 96 / originalFrame.DpiY, 0, 0));

            var newResizedFrame = BitmapFrame.Create(target, thumbnail, null, colorContexts == null ? null : colorContexts.AsReadOnly());

            ImageHelper.Deallocate(target);
            ImageHelper.Deallocate(originalFrame);

            return newResizedFrame;
        }

        private BitmapFrame MergeLayers(BitmapFrame originalPhotoFrame, ImageBrush waterMarkBrush, double paddingHeight = 0)
        {
            var rtbDpi = new RenderTargetBitmap(originalPhotoFrame.PixelWidth, originalPhotoFrame.PixelHeight + (int)paddingHeight, 
                (double)96, (double)96, PixelFormats.Default);
            var drawVisual = new DrawingVisual();

            using (var dc = drawVisual.RenderOpen())
            {
                dc.DrawImage(originalPhotoFrame, new System.Windows.Rect(0, 0, rtbDpi.Width, rtbDpi.Height));
                dc.DrawRectangle(waterMarkBrush, null, new System.Windows.Rect(0, 0, rtbDpi.Width, rtbDpi.Height));                
            }

            rtbDpi.Render(drawVisual);
            originalPhotoFrame = BitmapFrame.Create(rtbDpi, thumbnail, metadata, colorContexts == null ? null : colorContexts.AsReadOnly());

            ImageHelper.Deallocate(drawVisual);
            ImageHelper.Deallocate(rtbDpi);

            return originalPhotoFrame;
        }
    }
}
