using AILabs.FuzzyLogic.Map;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace AILabs.FuzzyLogic
{
    public partial class FormFL : Form
    {
        Graphics _graphics;
        private int width;
        private int height;

        private SurfaceMap _surfaceMap;

        private Bitmap _mapPlot;

        private Random _random;

        private FuzzyRobot _fuzzyRobot;

        private CancellationTokenSource _robotToken;

        private bool _robotActive = false;

        private int _tileSize;

        public FormFL()
        {
            InitializeComponent();

            width = pictureBox1.Width;
            height = pictureBox1.Height;
            _graphics = pictureBox1.CreateGraphics();

            _robotToken = new CancellationTokenSource();
            _random = new Random();
        }

        // Создание поля
        private void button1_Click(object sender, EventArgs e)
        {
            _mapPlot = new Bitmap(width, height);
            Graphics graphics = Graphics.FromImage(_mapPlot);
            graphics.Clear(Color.White);

            _surfaceMap = new SurfaceMap(height, width);

            _tileSize = (int)numericUpDown1.Value;
            int solidTiles = (int)numericUpDown2.Value;

            _surfaceMap.GenerateNewMap(_tileSize, solidTiles);
            foreach (var tile in _surfaceMap.GetNextTile())
            {
                graphics.DrawImage(tile.Draw(), tile.LeftTop);
            }

            pictureBox1.Image = _mapPlot;
        }

        private void IterateRobot(CancellationTokenSource token)
        {
            Task.Run(() =>
            {
                for (int i = 0; i < 1000; i++)
                {
                    if (token.IsCancellationRequested)
                    {
                        break;
                    }

                    var directions = _fuzzyRobot.RayTraceDirections(_surfaceMap);
                    var fuzzyValues = _fuzzyRobot.Fuzzify(directions);
                    var rulesOutput = _fuzzyRobot.FuzzyRules(fuzzyValues);
                    double deffuzed = _fuzzyRobot.GetDeffusedValue(rulesOutput);

                    _fuzzyRobot.Rotate(deffuzed);
                    _fuzzyRobot.Move();


                    _graphics.Clear(Color.White);
                    pictureBox1.Image = _mapPlot;
                    Task.Delay(1).Wait();

                    _graphics.DrawImage(_fuzzyRobot.DrawRobot(), _fuzzyRobot.LeftTopGlobalPosition);
                    Task.Delay(1).Wait();

                    Task.Delay(3).Wait();
                }


                //_graphics.DrawLine(new Pen(Color.Black, 2), directions[0], _fuzzyRobot.CentroidGlobalPosition);
                //_graphics.DrawLine(new Pen(Color.Black, 2), directions[1], _fuzzyRobot.CentroidGlobalPosition);
                //_graphics.DrawLine(new Pen(Color.Black, 2), directions[2], _fuzzyRobot.CentroidGlobalPosition);



            });
        }

        // Создание нового робота на текущем поле
        private void button2_Click(object sender, EventArgs e)
        {
            int raysAngle = 45;
            float robotSize = _tileSize / 2;

            _fuzzyRobot = new FuzzyRobot(raysAngle, robotSize, _tileSize);

            _fuzzyRobot.SetRandomStartPosition(_surfaceMap);

            Bitmap robotImg = _fuzzyRobot.DrawRobot();
            _graphics.DrawImage(robotImg, _fuzzyRobot.LeftTopGlobalPosition);



        }

        // Пауза / Запуск робота
        private void button3_Click(object sender, EventArgs e)
        {
            ChangeRobotState();
        }

        private void ChangeRobotState()
        {
            //IterateRobot(_robotToken);
            //return;

            if (_robotActive)
            {
                _robotToken.Cancel();
            }
            else
            {
                _robotToken.Dispose();
                _robotToken = new CancellationTokenSource();
                IterateRobot(_robotToken);
            }

            var directions = _fuzzyRobot.RayTraceDirections(_surfaceMap);
            var fuzzyValues = _fuzzyRobot.Fuzzify(directions);
            var rulesOutput = _fuzzyRobot.FuzzyRules(fuzzyValues);


            textBox1.Text = rulesOutput[FuzzyRobot.RotAction.RotNone].ToString();
            textBox2.Text = rulesOutput[FuzzyRobot.RotAction.RotLeft].ToString();
            textBox3.Text = rulesOutput[FuzzyRobot.RotAction.RotRight].ToString();



            _robotActive = !_robotActive;
        }
    }
}
