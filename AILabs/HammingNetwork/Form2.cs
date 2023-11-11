using System.ComponentModel;

namespace AILabs.HammingNetwork
{
    public partial class Form2 : Form
    {
        private LettersRecognizer _lettersRecognizer;

        public Form2()
        {
            InitializeComponent();

            _lettersRecognizer = new LettersRecognizer();
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
                int letter = _lettersRecognizer.Recognize(image);
                pictureBox1.Image = ImageUtils.EnlargeImage(image, 16);
                pictureBox2.Image = ImageUtils.EnlargeImage(_lettersRecognizer.GetOriginalImage(letter), 16);
            }
            catch (Exception ex)
            {
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

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
