using System;
using System.Collections.Generic;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILabs.FuzzyLogic.Map
{
    public class SolidTile : IMapTile
    {
        private PointF RightBottom;

        public SolidTile(PointF leftTop, int width, int height) : base(leftTop, width, height)
        {
            RightBottom = new PointF(LeftTop.X + Width, LeftTop.Y + Height);
        }

        public override bool CollidesWithPoint(PointF p)
        {
            return (p.X >= LeftTop.X && p.X <= RightBottom.X && p.Y <= RightBottom.Y && p.Y >= LeftTop.Y);
        }

        public override Bitmap Draw()
        {
            Bitmap bitmap = new Bitmap(Width, Height);
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    bitmap.SetPixel(i, j, Color.Black);
                }
            }
            return bitmap;
        }
    }
}
