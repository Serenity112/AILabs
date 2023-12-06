using AILabs.FuzzyLogic.Map;

namespace AILabs.FuzzyLogic
{
    public partial class FormFL : Form
    {
        Graphics _graphics;
        private int _width;
        private int _height;

        private SurfaceMap _surfaceMap;

        private Bitmap _mapPlot;

        private FuzzyRobot? _fuzzyRobot = null;

        private CancellationTokenSource _robotToken;

        private bool _robotActive = false;

        private int _tileSize;

        public FormFL()
        {
            InitializeComponent();

            _width = pictureBox1.Width;
            _height = pictureBox1.Height;
            _graphics = pictureBox1.CreateGraphics();

            _robotToken = new CancellationTokenSource();
        }

        // Создание поля
        private void button1_Click(object sender, EventArgs e)
        {
            _mapPlot = new Bitmap(_width, _height);
            Graphics graphics = Graphics.FromImage(_mapPlot);
            graphics.Clear(Color.White);

            _surfaceMap = new SurfaceMap(_height, _width);

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
                double curr_angle = _fuzzyRobot.GetCurrentVisionAngle();

                while (true)
                {
                    if (token.IsCancellationRequested)
                    {
                        break;
                    }

                    var rayResult = _fuzzyRobot.RayTraceDirections(_surfaceMap);

                    if (checkBox1.Checked)
                    {
                        _graphics.DrawLine(new Pen(Color.Green, 2), rayResult.Points[0], _fuzzyRobot.CentroidGlobalPosition);
                        _graphics.DrawLine(new Pen(Color.Green, 2), rayResult.Points[1], _fuzzyRobot.CentroidGlobalPosition);
                        _graphics.DrawLine(new Pen(Color.Green, 2), rayResult.Points[2], _fuzzyRobot.CentroidGlobalPosition);
                        Task.Delay(1).Wait();
                    }

                    var fuzzyValues = _fuzzyRobot.Fuzzify(rayResult.Distances);
                    var rulesOutput = _fuzzyRobot.FuzzyRules(fuzzyValues);
                    double deffuzed = _fuzzyRobot.GetDeffusedValue(rulesOutput);

                    _fuzzyRobot.Rotate(deffuzed);
                    double newAngle = _fuzzyRobot.GetCurrentVisionAngle();

                    if (Math.Round(curr_angle, 3) == Math.Round(newAngle, 3))
                    {
                        _fuzzyRobot.Move();
                    }

                    curr_angle = newAngle;

                    _graphics.Clear(Color.White);
                    pictureBox1.Image = _mapPlot;
                    Task.Delay(1).Wait();

                    _graphics.DrawImage(_fuzzyRobot.DrawRobot(), _fuzzyRobot.LeftTopGlobalPosition);
                    Task.Delay(4).Wait();
                }
            });
        }

        // Создание нового робота на текущем поле
        private void button2_Click(object sender, EventArgs e)
        {
            double visionAngles = (double)(numericUpDown3.Value) * Math.PI / 180;
            float robotSize = _tileSize / 4;
            double speed = (double)(numericUpDown4.Value);

            _fuzzyRobot = new FuzzyRobot(visionAngles, robotSize, _tileSize, speed);
            _fuzzyRobot.SetRandomStartPosition(_surfaceMap);

            Bitmap robotImg = _fuzzyRobot.DrawRobot();
            _graphics.DrawImage(robotImg, _fuzzyRobot.LeftTopGlobalPosition);
        }

        // Пауза / Запуск робота
        private void button3_Click(object sender, EventArgs e)
        {
            if (_fuzzyRobot != null)
            {
                ChangeRobotState();
            }
        }

        private void ChangeRobotState()
        {
            if (_fuzzyRobot != null)
            {
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

                _robotActive = !_robotActive;
            }
        }
    }
}
