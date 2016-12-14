﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace AlgoNature.Components
{
    public interface IBitmapGraphicChild
    {
        Bitmap Itself { get; }
        Bitmap GetItselfBitmap();
        bool DrawToGraphics { get; set; }
    }
}
