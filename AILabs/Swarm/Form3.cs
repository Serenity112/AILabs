using AILabs.DrawingUtils;
using MathLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AILabs.Swarm
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Bitmap bitmap = FunctionPlotDrawer.DrawBitmap(pictureBox1,
                (x, y) => x * x + 3 * y * y + 2 * x * y,
                (0.2558, -0.1163)
            );

            pictureBox1.Image = bitmap;


            SwarmMethod swarmMethod = new SwarmMethod((x, y) => x * x + 3 * y * y + 2 * x * y, 100);

            var result = swarmMethod.FindMinimum();

            textBox1.Text = result.extremum.ToString();
        }
    }
}
