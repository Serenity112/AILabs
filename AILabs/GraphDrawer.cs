﻿using MathLib;
using System.IO;
using System.Windows.Forms;

namespace AILabs
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

        private PictureBox _pictureBox;

        public GraphDrawer(PictureBox pictureBox, GraphData data, GraphVisuals visuals)
        {
            _pictureBox = pictureBox;
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
            _backgroundGraph = new Bitmap(_pictureBox.Width, _pictureBox.Height);
            Graphics graphics = Graphics.FromImage(_backgroundGraph);

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

            Vector V_ij = (new Vector(p_i, p_j)).Normalized();
            Vector V_ji = (new Vector(p_j, p_i)).Normalized();

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

        }

        public void DrawPath(List<int> indexes, Color color)
        {
            for (int v = 0; v < indexes.Count - 1; v++)
            {
                int i = indexes[v];
                int j = indexes[v + 1];

                DrawEdge(i, j, color);
            }
        }

        public void Invalidate()
        {
            _pictureBox.Invalidate();
        }

        public void Clear()
        {
            //Image img = pictureBox.BackgroundImage;
            _graphics.Clear(Color.White);
            //pictureBox.BackgroundImage = img;

            //_pictureBox.BackgroundImage = _backgroundGraph;
            //_graphics.DrawImage(_backgroundGraph, Point.Empty);

            //
        }

        private Point[] CalculateVertexCoordinates()
        {
            Point[] vertexes = new Point[_vertexCount];


            return vertexes;
        }

        private void Draw(object sender, EventArgs e)
        {
            _graphics.FillEllipse(new SolidBrush(Color.Red), new Rectangle(30, 30, 50, 50));
            /*foreach (var point in _vertexCoordinates)
            {
                _graphics.FillEllipse(_vertexBrush, new Rectangle(point.X - _vertexRadius / 2, point.Y - _vertexRadius / 2, _vertexRadius, _vertexRadius));
            }*/
        }

        private Point PointByCenter(Point center, double radius, double angle)
        {
            int x = (int)(center.X + radius * Math.Cos(angle));
            int y = (int)(center.Y + radius * Math.Sin(angle));
            return new Point(x, y);
        }
    }
}