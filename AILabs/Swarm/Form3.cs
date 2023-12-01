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
            label5.Text = listBox1.Text;

            CreateNewSwarm();
        }

        private List<Func<double, double, double>> _allowedFunctions;

        private Graphics _graphics;

        private SwarmMethod _swarmMethod;

        private Bitmap _plot;

        private double _interval = 10;

        private int _pickedFunction = 1;

        private RectangleF _bounds;

        // Полный алгоритм
        private void button1_Click(object sender, EventArgs e)
        {
            CreateNewSwarm();

            int maxCount = 1000;
            int countdown = 25;

            Vector extremum;

            var result = _swarmMethod.SingleIteration();
            extremum = result.ExtremumCoords;

            for (int i = 1; i < maxCount; i++)
            {
                Bitmap particlesMap = DrawParticles(result.Particles);
                _graphics.Clear(Color.White);
                _graphics.DrawImage(_plot, new Rectangle(-pictureBox1.Width / 2, -pictureBox1.Height / 2, pictureBox1.Width, pictureBox1.Height));
                _graphics.DrawImage(particlesMap, new Rectangle(-pictureBox1.Width / 2, -pictureBox1.Height / 2, pictureBox1.Width, pictureBox1.Height));

                result = _swarmMethod.SingleIteration();
                Vector newExtremum = result.ExtremumCoords;

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
                textBox1.Text = Math.Round(result.ExtremumValue, 10).ToString() + " В координатах" + result.ExtremumCoords;
            }
        }

        // Один шаг
        private void button2_Click(object sender, EventArgs e)
        {
            var result = _swarmMethod.SingleIteration();

            textBox1.Text = result.ExtremumValue.ToString();

            _graphics.Clear(Color.White);

            Bitmap particlesMap = DrawParticles(result.Particles);

            _graphics.DrawImage(_plot, new Rectangle(-pictureBox1.Width / 2, -pictureBox1.Height / 2, pictureBox1.Width, pictureBox1.Height));

            _graphics.DrawImage(particlesMap, new Rectangle(-pictureBox1.Width / 2, -pictureBox1.Height / 2, pictureBox1.Width, pictureBox1.Height));
        }

        private Bitmap DrawParticles(List<ParticleData> data)
        {
            Bitmap particles = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            particles.MakeTransparent();
            Graphics g = Graphics.FromImage(particles);

            int width = 2;
            Pen pen = new Pen(Color.White, width);

            foreach (var particle in data)
            {
                Vector vect = particle.CurrentPoint;
                if (vect.Dx < -_interval || vect.Dy > _interval)
                {
                    continue;
                }

                Vector normVal = vect + new Vector(_interval, _interval);
                Vector trVal = ((normVal / (2.0 * _interval)) * pictureBox1.Width);
                g.DrawEllipse(pen, new Rectangle((int)(trVal.Dx - width / 2), (int)(trVal.Dy - width / 2), width, width));

                if (checkBox2.Checked)
                {
                    Vector arrow = particle.Speed;
                    Vector normArrow = arrow + new Vector(_interval, _interval);
                    Vector trArrow = ((normArrow / (2.0 * _interval)) * pictureBox1.Width);

                    Vector normalizedArrow = (trArrow - trVal).Normalized() * 10;
                    Vector finalArrow = trVal + normalizedArrow;

                    g.DrawLine(new Pen(Color.White, 1), (int)trVal.Dx, (int)trVal.Dy, (int)finalArrow.Dx, (int)finalArrow.Dy);
                }
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
            (float x, float y) p1 = (float.Parse(textBox2.Text), float.Parse(textBox3.Text));
            (float x, float y) p2 = (float.Parse(textBox4.Text), float.Parse(textBox5.Text));
            float interval_x = Math.Abs(p1.x - p2.x);
            float interval_y = Math.Abs(p1.y - p2.y);
            (float x0, float x1) x_asc = p1.x < p2.x ? (p1.x, p2.x) : (p2.x, p1.x);
            (float y0, float y1) y_asc = p1.y < p2.y ? (p1.y, p2.y) : (p2.y, p1.y);
            label11.Text = x_asc.x1.ToString();
            label12.Text = x_asc.x0.ToString();
            label10.Text = y_asc.y0.ToString();
            label19.Text = y_asc.y1.ToString();
            (float x, float y) left_bottom = (x_asc.x0, y_asc.y0);
            (float x, float y) right_top = (x_asc.x1, y_asc.y1);

            _plot = FunctionPlotDrawer.ContourPlotter(pictureBox1, _allowedFunctions[_pickedFunction],
                left_bottom, right_top, (DrawingMode)Convert.ToInt32(checkBox1.Checked));
            _graphics.DrawImage(_plot, new Rectangle(-pictureBox1.Width / 2, -pictureBox1.Height / 2, pictureBox1.Width, pictureBox1.Height));

            _bounds = new RectangleF(left_bottom.x, left_bottom.y, interval_x, interval_y);

            _swarmMethod = new SwarmMethod(_allowedFunctions[listBox1.SelectedIndex], trackBar1.Value, _bounds);
            textBox1.Text = "";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            CreateNewSwarm();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Form3_Load(object sender, EventArgs e)
        {

        }
    }
}
