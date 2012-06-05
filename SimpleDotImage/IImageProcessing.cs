using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SimpleDotImage
{
    public interface IImageProcessing
    {
        Stream Process(string imagePath, int resize, string waterMarkPath, string waterMarkText, double waterMarkOpacity, int rotate, int pictureQuality);
    }
}
