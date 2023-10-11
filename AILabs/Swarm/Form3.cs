using AILabs.DrawingUtils;
using MathLib;
using static AILabs.Swarm.SwarmMethod;

namespace AILabs.Swarm
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();

            _graphics = pictureBox1.CreateGraphics();

            _allowedFunctions = new List<Func<double, double, double>>()
            {
               (x, y) => x * x + 3 * y * y + 2 * x * y,
               (x, y) => 100 * Math.Pow(y-x*x, 2) + Math.Pow(1-x, 2),
               (x, y) => -12 * y + 4 * x * x + 4 * y * y - 4 * x * y,
               (x, y) => Math.Pow(x-2, 4) + Math.Pow(x-2*y, 2),
               (x, y) => 2*x*x*x + 4*x*y*y*y -10*x*y + y*y,
            };

            listBox1.SelectedIndex = _pickedFunction;
            label5.Text = listBox1.Text;
            _swarmMethod = new SwarmMethod(_allowedFunctions[_pickedFunction], 500);
        }

        private List<Func<double, double, double>> _allowedFunctions;

        private Graphics _graphics;

        private SwarmMethod _swarmMethod;

        private Bitmap _plot;

        private double _interval = 10;

        private int _pickedFunction = 1;

        private bool _plotReady = false;

        // Полный алгоритм
        private void button1_Click(object sender, EventArgs e)
        {
            int maxCount = 1000;
            int countdown = 25;

            Vector extremum;

            var result = _swarmMethod.SingleIteration();
            extremum = result.extremum;

            _plot = FunctionPlotDrawer.ContourPlotter(pictureBox1, _allowedFunctions[_pickedFunction], (0, 0), _interval, (DrawingMode)Convert.ToInt32(checkBox1.Checked));

            for (int i = 1; i < maxCount; i++)
            {
                Bitmap particlesMap = DrawParticles(result.data);
                _graphics.Clear(Color.White);
                _graphics.DrawImage(_plot, new Rectangle(0, 0, 512, 512));
                _graphics.DrawImage(particlesMap, new Rectangle(0, 0, 512, 512));

                result = _swarmMethod.SingleIteration();
                Vector newExtremum = result.extremum;

                if (Math.Abs((newExtremum - extremum).Length()) <= 0.01)
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

                Thread.Sleep(20);
            }

            textBox1.Text = extremum.ToString();
        }

        // Один шаг
        private void button2_Click(object sender, EventArgs e)
        {
            var result = _swarmMethod.SingleIteration();

            textBox1.Text = result.extremum.ToString();

            _graphics.Clear(Color.White);

            if (!_plotReady)
            {
                _plotReady = true;
                _plot = FunctionPlotDrawer.ContourPlotter(pictureBox1, _allowedFunctions[_pickedFunction], (0, 0), _interval, (DrawingMode)Convert.ToInt32(checkBox1.Checked));
            }

            Bitmap particlesMap = DrawParticles(result.data);

            _graphics.DrawImage(_plot, new Rectangle(0, 0, pictureBox1.Width, pictureBox1.Height));

            _graphics.DrawImage(particlesMap, new Rectangle(0, 0, pictureBox1.Width, pictureBox1.Height));

        }

        private Bitmap DrawParticles(List<Vector> data)
        {
            Bitmap particles = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            particles.MakeTransparent();
            Graphics g = Graphics.FromImage(particles);


            //g.ScaleTransform(-1, 1);


            int width = 2;
            Pen pen = new Pen(Color.White, width);

            foreach (var vect in data)
            {
                if (vect.Dx < -_interval || vect.Dy > _interval)
                {
                    continue;
                }

                Vector normVal = vect + new Vector(_interval, _interval);
                Vector trVal = ((normVal / (2.0 * _interval)) * pictureBox1.Width);

                g.DrawEllipse(pen, new Rectangle((int)(trVal.Dx - width / 2), (int)(trVal.Dy - width / 2), width, width));
            }

            return particles;
        }


        private void listBox1_MouseClick(object sender, MouseEventArgs e)
        {
            _pickedFunction = listBox1.SelectedIndex;

            CreateNewSwarm();

            label5.Text = listBox1.Text;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label3.Text = trackBar1.Value.ToString();

            CreateNewSwarm();
        }

        private void CreateNewSwarm()
        {
            _swarmMethod = new SwarmMethod(_allowedFunctions[listBox1.SelectedIndex], trackBar1.Value);
            textBox1.Text = "";
            _plotReady = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            CreateNewSwarm();
        }
    }
}
