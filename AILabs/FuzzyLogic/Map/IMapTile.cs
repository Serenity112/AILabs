using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILabs.FuzzyLogic.Map
{
    public abstract class IMapTile
    {
        public readonly int Width;

        public readonly int Height;

        public readonly PointF LeftTop;

        public IMapTile(PointF leftTop, int width, int height)
        {
            LeftTop = leftTop;
            Height = height;
            Width = width;
        }

        public abstract Bitmap Draw();

        public abstract bool CollidesWithPoint(PointF point);
    }
}
