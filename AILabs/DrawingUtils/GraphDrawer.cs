using MathLib;
using System.IO;
using System.Windows.Forms;

namespace AILabs.DrawingUtils
{
    public struct GraphVisuals
    {
        public GraphVisuals(int vertexRadius, Color vertexColor,
            float edgeSize, Color edgeColor)
        {
            VertexRadius = vertexRadius;
            GraphRadiusCoefficient = 0.5;
            VertexColor = vertexColor;
            EdgeColor = edgeColor;
            EdgeSize = edgeSize;
        }

        public static GraphVisuals DefaultVisuals()
        {
            return new GraphVisuals(30, Color.Black, 2, Color.Green);
        }

        public int VertexRadius { get; private set; }

        public double GraphRadiusCoefficient { get; private set; }

        public Color VertexColor { get; private set; }

        public Color EdgeColor { get; private set; }

        public float EdgeSize { get; private set; }
    }

    public class GraphDrawer
    {
        private class Vertex
        {
            public Vertex(string name, Point coordinates, Point nameCoordinates)
            {
                Name = name;
                Coordinates = coordinates;
                NameCoordinates = nameCoordinates;
            }

            public string Name { get; private set; }

            public Point Coordinates { get; private set; }

            public Point NameCoordinates { get; private set; }
        }

        private Point _center;
        private Vertex[] _Vertexes;

        private GraphData _graphData;
        private GraphVisuals _graphVisuals;

        private int _vertexCount { get { return _graphData.Size; } }

        private int _vrtxRad;

        private Bitmap _backgroundGraph;

        // private Pen _edgePen = new Pen(Color.FromArgb(128, 128, 128), 2);

        private Graphics _graphics;

        public GraphDrawer(PictureBox pictureBox, GraphData data, GraphVisuals visuals)
        {
            _graphics = pictureBox.CreateGraphics();

            _graphData = data;
            _graphVisuals = visuals;

            _center = new Point(pictureBox.Width / 2, pictureBox.Height / 2);
            _vrtxRad = visuals.VertexRadius;

            _Vertexes = new Vertex[_vertexCount];
            CalculateVertexes();
        }

        private void CalculateVertexes()
        {
            double radius = _center.X * 0.5;
            double angleStep = 2 * Math.PI / _vertexCount;
            for (int i = 0; i < _vertexCount; i++)
            {
                double angle = i * angleStep;
                _Vertexes[i] = new Vertex(Convert.ToChar(i + 65).ToString(),
                    PointByCenter(_center, radius, angle),
                    PointByCenter(_center, radius + _graphVisuals.VertexRadius * 1.3, angle));
            }
        }

        public void RedrawGraphBase()
        {
            //_backgroundGraph = new Bitmap(_pictureBox.Width, _pictureBox.Height);
            //Graphics graphics = Graphics.FromImage(_backgroundGraph);

            // Отрисовка рёбер
            Pen edgePen = new Pen(_graphVisuals.EdgeColor, _graphVisuals.EdgeSize);
            for (int i = 0; i < _vertexCount; i++)
            {
                for (int j = 0; j < _vertexCount; j++)
                {
                    Point p_i = _Vertexes[i].Coordinates;
                    Point p_j = _Vertexes[j].Coordinates;
                    _graphics.DrawLine(edgePen, p_i, p_j);
                }
            }

            // Отрисовка вершин
            Brush vertexBrush = new SolidBrush(_graphVisuals.VertexColor);
            foreach (Vertex vertex in _Vertexes)
            {
                Point p = vertex.Coordinates;
                _graphics.FillEllipse(vertexBrush, new Rectangle(p.X - _vrtxRad / 2, p.Y - _vrtxRad / 2, _vrtxRad, _vrtxRad));
                _graphics.DrawString(vertex.Name, new Font("Arial", 12f), vertexBrush, vertex.NameCoordinates);
            }

            //_pictureBox.BackgroundImage = _backgroundGraph;
            // _graphics.Clear(Color.White);
        }

        public void ColorVertex(int vertexNum, Color color)
        {
            Point point = _Vertexes[vertexNum].Coordinates;
            Brush brush = new SolidBrush(color);
            _graphics.FillEllipse(brush, new Rectangle(point.X - _vrtxRad / 2, point.Y - _vrtxRad / 2, _vrtxRad, _vrtxRad));
        }

        public void DrawEdge(int i, int j, Color color)
        {
            Pen edgePen = new Pen(color, _graphVisuals.EdgeSize);
            Point p_i = _Vertexes[i].Coordinates;
            Point p_j = _Vertexes[j].Coordinates;

            Vector V_ij = new Vector(p_i, p_j).Normalized();
            Vector V_ji = new Vector(p_j, p_i).Normalized();

            Point p_i_new = p_i + V_ij * (_graphVisuals.VertexRadius / 2);
            Point p_j_new = p_j + V_ji * (_graphVisuals.VertexRadius / 2);

            _graphics.DrawLine(edgePen, p_i_new, p_j_new);
        }

        public void DrawEdgeAll(Color color)
        {
            for (int i = 0; i < _vertexCount; i++)
            {
                for (int j = 0; j < _vertexCount; j++)
                {
                    DrawEdge(i, j, color);
                }
            }
        }

        public void DrawArrow(int i, int j, Color color)
        {
            double arrowLen = 10;
            double arrAngle1 = Math.PI / 6;
            double arrAngle2 = -Math.PI / 6;

            Pen edgePen = new Pen(color, _graphVisuals.EdgeSize);
            Point p_i = _Vertexes[i].Coordinates;
            Point p_j = _Vertexes[j].Coordinates;

            Vector V_ij = new Vector(p_i, p_j).Normalized();
            Vector V_ji = new Vector(p_j, p_i).Normalized();

            Point p_i_new = p_i + V_ij * (_graphVisuals.VertexRadius / 2);
            Point p_j_new = p_j + V_ji * (_graphVisuals.VertexRadius / 2);

            Point p_j_arrow = p_j_new + V_ji * arrowLen;

            Point p0 = p_j_new;
            Point p_rot = p_j_arrow;

            double x1 = p0.X + (p_rot.X - p0.X) * Math.Cos(arrAngle1) - (p_rot.Y - p0.Y) * Math.Sin(arrAngle1);
            double y1 = p0.Y + (p_rot.X - p0.X) * Math.Sin(arrAngle1) + (p_rot.Y - p0.Y) * Math.Cos(arrAngle1);
            Point arrOffset1 = new Point((int)x1, (int)y1);

            double x2 = p0.X + (p_rot.X - p0.X) * Math.Cos(arrAngle2) - (p_rot.Y - p0.Y) * Math.Sin(arrAngle2);
            double y2 = p0.Y + (p_rot.X - p0.X) * Math.Sin(arrAngle2) + (p_rot.Y - p0.Y) * Math.Cos(arrAngle2);
            Point arrOffset2 = new Point((int)x2, (int)y2);

            _graphics.DrawLine(edgePen, p_i_new, p_j_new);
            _graphics.DrawLine(edgePen, p0, arrOffset1);
            _graphics.DrawLine(edgePen, p0, arrOffset2);
        }

        public void DrawPath(List<int> indexes, Color color)
        {
            for (int v = 0; v < indexes.Count - 1; v++)
            {
                int i = indexes[v];
                int j = indexes[v + 1];

                DrawArrow(i, j, color);
            }
        }

        public void Clear()
        {
            _graphics.Clear(Color.White);
        }

        private Point PointByCenter(Point center, double radius, double angle)
        {
            int x = (int)(center.X + radius * Math.Cos(angle));
            int y = (int)(center.Y + radius * Math.Sin(angle));
            return new Point(x, y);
        }
    }
}
