using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Media.Imaging;
namespace SimpleDotImage
{
    public interface IImageProcessing
    {
        Stream Process(string imagePath, int resize, string waterMarkPath, string waterMarkText, double waterMarkOpacity, int pictureQuality, bool flipHorizontal,
            bool flipVertical, Rotation rotate, ColorFormat colorFormat);
    }
}
