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
        Stream Process(string imagePath, int resize = 0, string waterMarkPath = "", string waterMarkText = "",
            double waterMarkOpacity = 0.4, WaterMarkPosition waterMarkPosition = WaterMarkPosition.Center, int waterMarkTextSize = 54,
            int pictureQuality = 85, bool flipHorizontal = false, bool flipVertical = false,
            Rotation rotate = Rotation.Rotate0, ColorFormat colorFormat = ColorFormat.Default);
    }
}
