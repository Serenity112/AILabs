using AILabs.DrawingUtils;
using AILabs.Swarm;
using MathLib;

namespace AILabs.Genetic
{
    public partial class Form5 : Form
    {
        private List<Func<double, double, double>> _allowedFunctions;

        private Graphics _graphics;

        private GeneticAlgorithm _genetic;

        private Bitmap _plot;

        private int _pickedFunction = 1;

        private bool _plotReady = false;

        private int _encodingMode = 0;

        private RectangleF _bounds;

        public Form5()
        {
            InitializeComponent();

            _graphics = pictureBox1.CreateGraphics();
            _graphics.TranslateTransform(pictureBox1.Width / 2, pictureBox1.Height / 2);
            _graphics.ScaleTransform(1, -1);

            _allowedFunctions = new List<Func<double, double, double>>()
            {
               (x, y) => x * x + 3 * y * y + 2 * x * y,
               (x, y) => 100 * Math.Pow(y-x*x, 2) + Math.Pow(1-x, 2),
               (x, y) => -12 * y + 4 * x * x + 4 * y * y - 4 * x * y,
               (x, y) => Math.Pow(x-2, 4) + Math.Pow(x-2*y, 2),
               (x, y) => 2*x*x*x + 4*x*y*y*y -10*x*y + y*y,
            };

            listBox1.SelectedIndex = _pickedFunction;
            listBox2.SelectedIndex = _encodingMode;
            NewGeneration();
        }

        // Кнопка обновить
        private void button3_Click(object sender, EventArgs e)
        {
            NewGeneration();
            textBox1.Text = "Обновлено";
        }

        // Кнопка одной итерации
        private void button2_Click(object sender, EventArgs e)
        {
            var result = _genetic.SingleIteration();

            _graphics.Clear(Color.White);

            Bitmap particlesMap = DrawPoints(result.Vectors);
            _graphics.DrawImage(_plot, new Rectangle(-pictureBox1.Width / 2, -pictureBox1.Height / 2, pictureBox1.Width, pictureBox1.Height));
            _graphics.DrawImage(particlesMap, new Rectangle(-pictureBox1.Width / 2, -pictureBox1.Height / 2, pictureBox1.Width, pictureBox1.Height));

            textBox1.Text = $"{result.ExtremumValue} {result.ExtremumCoords}";
        }

        // Полный поиск
        private void button1_Click(object sender, EventArgs e)
        {
            NewGeneration();
            textBox1.Text = "";

            int maxCount = 150;
            int countdown = 15;
            int counter = 0;

            var result = _genetic.SingleIteration();
            double extremum = result.ExtremumValue;

            for (int i = 1; i < maxCount; i++)
            {
                Bitmap particlesMap = DrawPoints(result.Vectors);
                _graphics.Clear(Color.White);
                _graphics.DrawImage(_plot, new Rectangle(-pictureBox1.Width / 2, -pictureBox1.Height / 2, pictureBox1.Width, pictureBox1.Height));
                _graphics.DrawImage(particlesMap, new Rectangle(-pictureBox1.Width / 2, -pictureBox1.Height / 2, pictureBox1.Width, pictureBox1.Height));

                result = _genetic.SingleIteration();
                double newExtremum = result.ExtremumValue;

                if (Math.Abs(newExtremum - extremum) <= 0.001)
                {
                    countdown--;
                    if (countdown == 0)
                    {
                        break;
                    }
                }
                else
                {
                    countdown = 15;
                }

                extremum = newExtremum;

                counter++;

                Thread.Sleep(20);
            }

            textBox1.Text = $"Значение: {Math.Round(result.ExtremumValue, 5)}, Координаты: {result.ExtremumCoords}, Итераций: {counter}";
        }

        // Считать данные с окна и создать новое поколение
        private void NewGeneration()
        {
            var data = new GeneticAlgorithmData(
                trackBar1.Value,
                (double)numericUpDown1.Value,
                (EncodingMode)_encodingMode,
                (double)numericUpDown3.Value,
                (double)numericUpDown2.Value);

            (float x, float y) p1 = (float.Parse(textBox2.Text), float.Parse(textBox3.Text));
            (float x, float y) p2 = (float.Parse(textBox4.Text), float.Parse(textBox5.Text));

            float interval_x = Math.Abs(p1.x - p2.x);
            float interval_y = Math.Abs(p1.y - p2.y);

            (float x0, float x1) x_asc = p1.x < p2.x ? (p1.x, p2.x) : (p2.x, p1.x);
            (float y0, float y1) y_asc = p1.y < p2.y ? (p1.y, p2.y) : (p2.y, p1.y);

            label5.Text = x_asc.x0.ToString();
            label6.Text = x_asc.x1.ToString();
            label7.Text = y_asc.y0.ToString();
            label19.Text = y_asc.y1.ToString();

            (float x, float y) left_bottom = (x_asc.x0, y_asc.y0);
            (float x, float y) right_top = (x_asc.x1, y_asc.y1);

            _plot = FunctionPlotDrawer.ContourPlotter(pictureBox1, _allowedFunctions[_pickedFunction],
                left_bottom, right_top, (DrawingMode)Convert.ToInt32(checkBox1.Checked));
            _graphics.DrawImage(_plot, new Rectangle(-pictureBox1.Width / 2, -pictureBox1.Height / 2, pictureBox1.Width, pictureBox1.Height));

            _plotReady = false;

            _bounds = new RectangleF(left_bottom.x, left_bottom.y, interval_x, interval_y);

            _genetic = new GeneticAlgorithm(_allowedFunctions[listBox1.SelectedIndex], data, _bounds);
        }

        private void listBox1_MouseClick(object sender, MouseEventArgs e)
        {
            _pickedFunction = listBox1.SelectedIndex;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label3.Text = trackBar1.Value.ToString();
        }


        private void listBox2_MouseClick(object sender, MouseEventArgs e)
        {
            _encodingMode = listBox2.SelectedIndex;
        }

        private Bitmap DrawPoints(List<Vector> data)
        {
            Bitmap particles = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            particles.MakeTransparent();
            Graphics g = Graphics.FromImage(particles);

            int width = 2;
            Pen pen = new Pen(Color.White, width);

            foreach (Vector point in data)
            {
                int pbX = (int)(((point.Dx - _bounds.X) / (_bounds.Width)) * pictureBox1.Width);
                int pbY = (int)(((point.Dy - _bounds.Y) / (_bounds.Height)) * pictureBox1.Height);

                if ((pbX < 0 || pbX > pictureBox1.Width) || (pbY < 0 || pbY > pictureBox1.Height))
                {
                    continue;
                }

                g.DrawEllipse(pen, new Rectangle((int)(pbX - width / 2), (int)(pbY - width / 2), width, width));
            }

            return particles;
        }
    }
}
