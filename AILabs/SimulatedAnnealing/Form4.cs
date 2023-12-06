using AILabs.DrawingUtils;
using AILabs.LabAnts;
using MathLib;
using System.Runtime.Intrinsics.Arm;

namespace AILabs.SimulatedAnnealing
{
    public partial class Form4 : Form
    {
        private SimulatedAnnealing _annealing;
        private GraphDrawer _graphDrawer;
        private GraphData _inputGraph;
        private string _cityDataPath = @"..\..\..\SimulatedAnnealing\Input\input2.txt";

        public Form4()
        {
            InitializeComponent();

            SubsribeEvents();

            Start();
        }

        private void SubsribeEvents()
        {
            button5.Click += Restart!;
            button4.Click += SingleIteration!;
            button6.Click += FullSearch!;
            button1.Click += DefaultSettings!;
        }

        private void Start()
        {
            _inputGraph = GraphParser.ReadGraphData(_cityDataPath);
            _annealing = new SimulatedAnnealing(_inputGraph, AnnealingParameters.DefaultParameters());
            DefaultSettings(this, EventArgs.Empty);
        }

        private void Restart(object sender, EventArgs e)
        {
            AnnealingParameters ap = new AnnealingParameters(
                (double)Numeric1.Value, (double)Numeric2.Value, (double)Numeric3.Value);

            _inputGraph = GraphParser.ReadGraphData(_cityDataPath);

            _annealing = new SimulatedAnnealing(_inputGraph, ap);
            _graphDrawer = new GraphDrawer(pictureBox1, _annealing.GetGraph(), GraphVisuals.DefaultVisuals());
            label5.Text = (Math.Round((double)Numeric1.Value, 3)).ToString();

            textBox1.Text = "";
            RedrawGraph();
        }

        private void SingleIteration(object sender, EventArgs e)
        {
            RedrawGraph();
            PathData data = _annealing.SingleIteration();
            _graphDrawer.DrawPath(data.PathIndexes, Color.Red);
            textBox1.Text = $"Путь: {data}, L: {data.Length}";
            label5.Text = _annealing.GetTemperature().ToString();
        }

        private void FullSearch(object sender, EventArgs e)
        {
            Restart(this, EventArgs.Empty);
            RedrawGraph();
            PathData data = _annealing.FullSearch();
            _graphDrawer.DrawPath(data.PathIndexes, Color.Red);
            textBox1.Text = $"Путь: {data}, L: {data.Length}";
            label5.Text = _annealing.GetTemperature().ToString();
        }

        private void DefaultSettings(object sender, EventArgs e)
        {
            AnnealingParameters dp = AnnealingParameters.DefaultParameters();
            Numeric1.Value = (decimal)dp.InitialTemperature;
            Numeric2.Value = (decimal)dp.FinalTemperature;
            Numeric3.Value = (decimal)dp.TemperatureStep;
            label5.Text = (Math.Round((double)dp.InitialTemperature, 3)).ToString();
            Restart(this, EventArgs.Empty);
        }

        private void RedrawGraph()
        {
            _graphDrawer.Clear();
            _graphDrawer.RedrawGraphBase();
            _graphDrawer.ColorVertex(0, Color.Red);
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {

        }
    }
}
