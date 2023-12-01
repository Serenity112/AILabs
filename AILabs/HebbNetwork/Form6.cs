using AILabs.HammingNetwork;
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

namespace AILabs.HebbNetwork
{
    public partial class Form6 : Form
    {
        private string _resourcesPath = @"..\..\..\HebbNetwork\Resources\";

        public string[] _lettersFileNames = new string[]
        {
            "С.png",
            "А.png",
            "Н.png",
            "Ч.png",
            "О.png",
            //"К.png",
        };

        private HebbNetwork _hebbNetwork;

        public Form6()
        {
            InitializeComponent();

            List<NumericVector> _inputVectrors = new List<NumericVector>();
            foreach (string file in _lettersFileNames)
            {
                Bitmap bitmap = new Bitmap($"{_resourcesPath}{file}");
                _inputVectrors.Add(ImageUtils.ConvertImageToBinaryVector(bitmap));
            }
            _hebbNetwork = new HebbNetwork(_inputVectrors, textBox1);
        }

        public int Recognize(Bitmap bitmap)
        {
            NumericVector vect = ImageUtils.ConvertImageToBinaryVector(bitmap);
            int group = _hebbNetwork.GetVectorGroup(vect);
            return group;
        }

        public Bitmap GetOriginalImage(int group)
        {
            return new Bitmap($"{_resourcesPath}{_lettersFileNames[group]}");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            Clear();

            string fileSelected = openFileDialog1.FileName;
            Bitmap image = new Bitmap(fileSelected);

            try
            {
                int letter = Recognize(image);
                pictureBox1.Image = ImageUtils.EnlargeImage(image, 16);
                pictureBox2.Image = ImageUtils.EnlargeImage(GetOriginalImage(letter), 16);
            }
            catch (Exception ex)
            {
                pictureBox1.Image = ImageUtils.EnlargeImage(image, 16);
                pictureBox2.Image = null;
                textBox1.Text = ex.Message;
                return;
            }
        }

        private void Clear()
        {
            pictureBox1.Image = null;
            pictureBox2.Image = null;
            textBox1.Text = "";
        }
    }
}
