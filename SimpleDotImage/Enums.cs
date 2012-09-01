using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleDotImage
{
    public enum ColorFormat
    {
        Default,
        Gray,
        BlackAndWhite,
        RGB,
        BGR,
        CMYK
    }

    public enum WaterMarkPosition
    {
        TopLeft,
        TopRight,
        BottomRight,
        BottomLeft,
        Center
    }
}
