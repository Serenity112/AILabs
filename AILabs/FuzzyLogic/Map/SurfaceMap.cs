using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILabs.FuzzyLogic.Map
{
    public class SurfaceMap
    {
        private int _mapWidth;
        private int _mapHeight;
        private IMapTile[,] _currentMap;

        public SurfaceMap(int mapHeight, int mapWidth)
        {
            _mapWidth = mapWidth;
            _mapHeight = mapHeight;
        }

        public bool IfPointInsideMap(PointF p)
        {
            return (p.X >= 0 && p.X <= _mapWidth && p.Y <= _mapHeight && p.Y >= 0);
        }

        public void GenerateNewMap(int tileSize, int solidTilesCount)
        {
            _currentMap = CreateNewMap(tileSize, solidTilesCount);
        }

        public IEnumerable<IMapTile> GetNextTile()
        {
            for (int h = 0; h < _currentMap.GetLength(0); h++)
            {
                for (int w = 0; w < _currentMap.GetLength(1); w++)
                {
                    yield return _currentMap[h, w];
                }
            }
        }

        private IMapTile[,] CreateNewMap(int tileSize, int solidTilesCount)
        {
            int tilesCountW = (int)(_mapWidth / tileSize);
            int tilesCountH = (int)(_mapHeight / tileSize);

            IMapTile[,] map = new IMapTile[tilesCountW, tilesCountH];

            HashSet<(int, int)> solidTiles = new HashSet<(int, int)>();
            Random rnd = new Random();
            for (int i = 0; i < solidTilesCount; i++)
            {
                if (i > tilesCountW * tilesCountH)
                {
                    break;
                }

                (int, int) tile = (rnd.Next(tilesCountW), rnd.Next(tilesCountH));
                while (solidTiles.Contains(tile))
                {
                    tile = (rnd.Next(tilesCountW), rnd.Next(tilesCountH));
                }

                solidTiles.Add(tile);
            }

            for (int w = 0; w < tilesCountH; w++)
            {
                for (int h = 0; h < tilesCountW; h++)
                {
                    PointF leftTop = new PointF()
                    {
                        X = tileSize * w,
                        Y = tileSize * h,
                    };

                    if (solidTiles.Contains((w, h)))
                    {
                        map[w, h] = new SolidTile(leftTop, tileSize, tileSize);
                    }
                    else
                    {
                        map[w, h] = new EmptyTile(leftTop, tileSize, tileSize);
                    }
                }
            }

            return map;
        }
    }
}
