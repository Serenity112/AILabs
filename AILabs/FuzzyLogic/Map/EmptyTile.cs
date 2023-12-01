using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILabs.FuzzyLogic.Map
{
    public class EmptyTile : IMapTile
    {
        public EmptyTile(PointF leftTop, int width, int height) : base(leftTop, width, height)
        {
        }

        public override bool CollidesWithPoint(PointF point)
        {
            return false;
        }

        public override Bitmap Draw()
        {
            Bitmap bitmap = new Bitmap(Width, Height);
            return bitmap;
        }
    }
}
