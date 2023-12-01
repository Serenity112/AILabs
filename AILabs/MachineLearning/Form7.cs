using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace AILabs.MachineLearning
{
    public partial class Form7 : Form
    {
        private string _currentFilePath = string.Empty;

        private enum SampleType
        {
            Normal,
            Anomaly,
        }

        private IsolationForest _isolationForest;

        public Form7()
        {
            InitializeComponent();
        }

        // Анализ файла
        private void button1_Click(object sender, EventArgs e)
        {
            ClearFlowPanel();

            if (_currentFilePath == string.Empty)
            {
                return;
            }

            int numTrees = (int)numericUpDown1.Value;
            int subSampleSize = (int)numericUpDown2.Value;
            double anomalyThreshold = 0.5;

            List<DataPoint> sample = new List<DataPoint>();
            var dataset = MLParser.ParseFile(_currentFilePath);

            foreach (var item in dataset)
            {
                DataPoint point = new DataPoint(item.Item1);
                sample.Add(point);
            }

            int dimentions = dataset[0].dimentionalData.Length;

            _isolationForest = new IsolationForest(numTrees, dimentions, subSampleSize);

            _isolationForest.Train(sample);

            Dictionary<DataPoint, double> anomalyScores = _isolationForest.GetAnomalyScores();

            int anomalyCount = 0;

            int TruePositive = 0;
            int FalsePositive = 0;
            int FalseNegative = 0;
            int TrueNegative = 0;

            for (int i = 0; i < anomalyScores.Count; i++)
            {
                var score = anomalyScores.ElementAt(i);

                // Аномалия
                if (score.Value >= anomalyThreshold)
                {
                    anomalyCount++;
                    CreateEntry(SampleType.Anomaly, $"{score.Key}: {score.Value}");

                    if (dataset[i].isAnomaly)
                    {
                        TruePositive++;
                    }
                    else
                    {
                        FalsePositive++;
                    }
                }
                // Норма
                else
                {
                    CreateEntry(SampleType.Normal, $"{score.Key}: {score.Value}");

                    if (dataset[i].isAnomaly)
                    {
                        FalseNegative++;
                    }
                    else
                    {
                        TrueNegative++;
                    }
                }
            }

            label8.Text = Math.Round(((anomalyCount * 1.0) / (anomalyScores.Count)), 7).ToString();

            //TPR
            label5.Text = Math.Round(((TruePositive * 1.0) / (TruePositive + FalseNegative)), 7).ToString();
            //FPR
            label14.Text = Math.Round(((FalsePositive * 1.0) / (FalsePositive + TrueNegative)), 7).ToString();

            label10.Text = (sample.Count - anomalyCount).ToString();
            label11.Text = anomalyCount.ToString();
        }

        private void CreateEntry(SampleType sample, string text)
        {
            FlowLayoutPanel tragetPanel = sample == SampleType.Normal ? flowLayoutPanel1 : flowLayoutPanel2;

            tragetPanel.Controls.Add(new Label()
            {
                Text = text,
                BackColor = Color.White,
                Size = new Size(300, 50),
                Font = new Font(FontFamily.GenericSansSerif, 13, FontStyle.Regular),
            });
        }

        private void ClearFlowPanel()
        {
            flowLayoutPanel1.Controls.Clear();
            flowLayoutPanel2.Controls.Clear();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            _currentFilePath = openFileDialog1.FileName;
            label16.Text = openFileDialog1.SafeFileName;
        }
    }
}
