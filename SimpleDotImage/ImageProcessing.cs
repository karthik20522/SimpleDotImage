using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Threading.Tasks;
using System.Globalization;

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

                if (waterMarkImageStream != null)
                    waterMarkImageStream.Dispose();

                thumbnail = null;
                metadata = null;
            }
            // do unmanaged cleanup here,
        }
        #endregion       
         
        /// <summary>
        /// WIC ImageProcessing
        /// </summary>
        /// <param name="imagePath">original file path</param>
        /// <param name="resize">resize size. this resize preserves aspect ratio</param>
        /// <param name="waterMarkPath">watermark file path (optional)</param>
        /// <param name="waterMarkText">watermark text to be printed</param>
        /// <param name="waterMarkOpacity">opacity of the watermark</param>
        /// <param name="waterMarkPosition">position of watermark relative to image</param>
        /// <param name="waterMarkTextSize">watermark text size</param>
        /// <param name="pictureQuality">quality of the processed image. higher the value, better the quality. default is 85</param>
        /// <param name="flipHorizontal">flip an image horizontally</param>
        /// <param name="flipVertical">flip an image vertically</param>
        /// <param name="rotate">rotate an image</param>
        /// <param name="colorFormat">image color - gray scale, back & white, rgb, bgr, cmyk</param>
        /// <returns></returns>
        public Stream Process(string imagePath, int resize = 0, string waterMarkPath = "", string waterMarkText = "",
            double waterMarkOpacity = 0.4, WaterMarkPosition waterMarkPosition = WaterMarkPosition.Center, int waterMarkTextSize = 54, 
            int pictureQuality = 85, bool flipHorizontal = false, bool flipVertical = false, 
            Rotation rotate = Rotation.Rotate0, ColorFormat colorFormat = ColorFormat.Default)
        {
            var backGroundDispatcher = new BackgroundDispatcher(Guid.NewGuid().ToString("N"));
            var imageProcessingOutput = new ImageProcessingOutput();

            backGroundDispatcher.Invoke((Action)delegate
            {
                PerformWork(imageProcessingOutput, imagePath, resize, waterMarkPath, waterMarkText, waterMarkOpacity, waterMarkPosition, waterMarkTextSize, 
                pictureQuality, flipHorizontal, flipVertical, rotate, colorFormat);
            });

            return imageProcessingOutput.OutputStream;
        }

        private void PerformWork(ImageProcessingOutput outputStream, string imagePath, int resize = 0, string waterMarkPath = "", string waterMarkText = "",
            double waterMarkOpacity = 0.4, WaterMarkPosition waterMarkPosition = WaterMarkPosition.Center, int waterMarkTextSize = 54,
            int pictureQuality = 85, bool flipHorizontal = false, bool flipVertical = false,
            Rotation rotate = Rotation.Rotate0, ColorFormat colorFormat = ColorFormat.Default)
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
                    metadata = imageFrame.Metadata == null ? null : imageFrame.Metadata.Clone() as BitmapMetadata;
                    colorContexts = imageFrame.ColorContexts == null ? null : imageFrame.ColorContexts.ToList();
                }
                catch //corrupted image metadata
                {
                    metadata = new BitmapMetadata("jpg");
                    metadata.SetQuery("/app1/ifd/exif/subifd:{uint=40961}", (ushort)1); //sRGB color space by default
                }

                //resize image
                var resizeTask = Task<BitmapFrame>.Factory.StartNew(() =>
                {
                    imageFrame = ResizeImage(imageFrame, resize);

                    if (imageFrame != null)
                        imageFrame.Freeze(); //no more modifiction to the resized image

                    return imageFrame;
                });

                //build watermark brush - provided watermark image
                var waterMarkTask = Task<ImageBrush>.Factory.StartNew(() =>
                {
                    var waterMarkImage = BuildWaterMark(waterMarkImageStream, waterMarkOpacity, waterMarkPosition);

                    if (waterMarkImage != null)
                        waterMarkImage.Freeze(); //no more modification to the watermark image

                    return waterMarkImage;
                });

                //build watermark brush - provided watermark text
                var waterMarkTextTask = Task<DrawingBrush>.Factory.StartNew(() =>
                {
                    var waterMarkImage = BuildWaterMark(waterMarkText ?? string.Empty, waterMarkOpacity, waterMarkTextSize, waterMarkPosition);

                    if (waterMarkImage != null)
                        waterMarkImage.Freeze(); //no more modification to the watermark image

                    return waterMarkImage;
                });

                //These operations can be processed in parallel and stiched back together
                Task.WaitAll(resizeTask, waterMarkTask, waterMarkTextTask);

                imageFrame = MergeLayers(resizeTask.Result, waterMarkTask.Result, waterMarkTextTask.Result);

                outputStream.OutputStream = GenerateNewJPEGImage(imageFrame, pictureQuality, flipHorizontal, flipVertical, rotate, colorFormat);
            }
        }

        #region PrivateMethods
        private Stream GenerateNewJPEGImage(BitmapFrame targetFrame, int pictureQuality, bool flipHorizontal, bool flipVertical, Rotation rotate, ColorFormat imageFormat)
        {
            if (targetFrame == null)
                return null;

            if (imageFormat != ColorFormat.Default)
            {
                var formatBitmap = new FormatConvertedBitmap();
                formatBitmap.BeginInit();
                formatBitmap.Source = targetFrame;
                formatBitmap.DestinationFormat = GetPixelFormat(imageFormat);
                formatBitmap.EndInit();
                formatBitmap.Freeze();

                ImageHelper.Deallocate(targetFrame);
                targetFrame = BitmapFrame.Create(formatBitmap);
                ImageHelper.Deallocate(formatBitmap);
            }

            var memoryStream = new MemoryStream();

            var targetEncoder = new JpegBitmapEncoder();
            targetEncoder.QualityLevel = pictureQuality;
            targetEncoder.FlipHorizontal = flipHorizontal;
            targetEncoder.FlipVertical = flipVertical;
            targetEncoder.Rotation = rotate;

            targetEncoder.Frames.Add(targetFrame);
            targetEncoder.Save(memoryStream);

            ImageHelper.Deallocate(targetEncoder);
            ImageHelper.Deallocate(targetFrame);
            ImageHelper.Deallocate(thumbnail);

            return memoryStream;
        }

        private DrawingBrush BuildWaterMark(string waterMarkText, double waterMarkOpacity, int textSize, WaterMarkPosition position)
        {
            if (string.IsNullOrEmpty(waterMarkText))
                return null;

            var visualCaptionText = new FormattedText(waterMarkText, CultureInfo.CurrentCulture, System.Windows.FlowDirection.LeftToRight,
                new Typeface("Georgia"), textSize, new SolidColorBrush(Colors.White));

            var drawing = new DrawingGroup();
            using (var context = drawing.Open())
            {
                context.DrawText(visualCaptionText, new System.Windows.Point(0, 0));
            }

            var brush = new DrawingBrush(drawing);
            brush.Stretch = Stretch.None;
            brush.TileMode = TileMode.None;
            brush.AlignmentX = XAlignment(position);
            brush.AlignmentY = YAlignment(position);
            brush.Opacity = waterMarkOpacity;
            brush.Freeze();

            ImageHelper.Deallocate(drawing);

            return brush;
        }

        private ImageBrush BuildWaterMark(Stream waterMarkImage, double waterMarkOpacity, WaterMarkPosition position)
        {
            if (waterMarkImage == null)
                return null;

            var wmDecoder = BitmapDecoder.Create(waterMarkImage, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.None);
            var wmFrame = wmDecoder.Frames[0] as BitmapFrame; //get the watermark frame
            wmFrame.Freeze();

            ImageBrush brush = new ImageBrush(wmFrame);
            brush.Stretch = Stretch.None;
            brush.TileMode = TileMode.None;
            brush.AlignmentX = XAlignment(position);
            brush.AlignmentY = YAlignment(position);
            brush.Opacity = waterMarkOpacity;

            brush.Freeze(); //no more modifiction to watermark image

            ImageHelper.Deallocate(wmDecoder);
            ImageHelper.Deallocate(wmFrame);

            return brush;
        }

        private BitmapFrame ResizeImage(BitmapFrame originalFrame, int newImageSize)
        {
            if (newImageSize < 1 || newImageSize == originalFrame.PixelWidth)
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

        private BitmapFrame MergeLayers(BitmapFrame originalPhotoFrame, ImageBrush waterMarkBrush, DrawingBrush waterMarkTextBrush)
        {
            //GetPixelFormat(imageFormat)
            var rtbDpi = new RenderTargetBitmap(originalPhotoFrame.PixelWidth, originalPhotoFrame.PixelHeight, (double)96, (double)96, PixelFormats.Default);
            var drawVisual = new DrawingVisual();

            using (var dc = drawVisual.RenderOpen())
            {
                dc.DrawImage(originalPhotoFrame, new System.Windows.Rect(0, 0, rtbDpi.Width, rtbDpi.Height));
                dc.DrawRectangle(waterMarkBrush, null, new System.Windows.Rect(0, 0, rtbDpi.Width, rtbDpi.Height));
                dc.DrawRectangle(waterMarkTextBrush, null, new System.Windows.Rect(0, 0, rtbDpi.Width, rtbDpi.Height));
            }

            rtbDpi.Render(drawVisual);

            ImageHelper.Deallocate(originalPhotoFrame);
            originalPhotoFrame = BitmapFrame.Create(rtbDpi, thumbnail, metadata, colorContexts == null ? null : colorContexts.AsReadOnly());

            ImageHelper.Deallocate(drawVisual);
            ImageHelper.Deallocate(rtbDpi);
            ImageHelper.Deallocate(waterMarkBrush);
            ImageHelper.Deallocate(waterMarkTextBrush);

            return originalPhotoFrame;
        }

        private AlignmentY YAlignment(WaterMarkPosition position)
        {
            switch (position)
            {
                case WaterMarkPosition.BottomLeft:
                case WaterMarkPosition.BottomRight:
                    return AlignmentY.Bottom;

                case WaterMarkPosition.TopLeft:
                case WaterMarkPosition.TopRight:
                    return AlignmentY.Top;

                default:
                    return AlignmentY.Center;
            }
        }

        private AlignmentX XAlignment(WaterMarkPosition position)
        {
            switch (position)
            {
                case WaterMarkPosition.BottomLeft:
                case WaterMarkPosition.TopLeft:
                    return AlignmentX.Left;

                case WaterMarkPosition.BottomRight:
                case WaterMarkPosition.TopRight:
                    return AlignmentX.Right;

                default:
                    return AlignmentX.Center;
            }
        }

        private PixelFormat GetPixelFormat(ColorFormat format)
        {
            switch (format)
            {
                case ColorFormat.BGR:
                    return PixelFormats.Bgr24;

                case ColorFormat.BlackAndWhite:
                    return PixelFormats.BlackWhite;

                case ColorFormat.CMYK:
                    return PixelFormats.Cmyk32;

                case ColorFormat.Gray:
                    return PixelFormats.Gray32Float;

                case ColorFormat.RGB:
                    return PixelFormats.Rgb24;

                default:
                    return PixelFormats.Default;
            }
        }
        #endregion
    }

    public class ImageProcessingOutput
    {
        public Stream OutputStream { get; set; }
    }
}
