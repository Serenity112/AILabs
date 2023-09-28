using MathLib;
using System.Windows.Forms;

namespace AILabs.LabAnts
{
    public partial class Form1 : Form
    {
        private const decimal TrackerScale = 100.0M;

        private AntColony _antColony;

        private GraphDrawer _graphDrawer;

        private TrackBar[] _trackBars;
        private NumericUpDown[] _numerics;

        public Form1()
        {
            InitializeComponent();

            SubsribeEvents();

            Start();
        }

        private void SubsribeEvents()
        {
            Scroll1.ValueChanged += TrackNumericEvent!;
            Numeric1.ValueChanged += TrackNumericEvent!;

            Scroll2.ValueChanged += TrackNumericEvent!;
            Numeric2.ValueChanged += TrackNumericEvent!;

            Scroll3.ValueChanged += TrackNumericEvent!;
            Numeric3.ValueChanged += TrackNumericEvent!;

            Scroll4.ValueChanged += TrackNumericEvent!;
            Numeric4.ValueChanged += TrackNumericEvent!;

            button5.Click += UpdateColony!;
            button4.Click += SingleIteration!;
            button6.Click += FullACO_Search!;
            button1.Click += DefaultSettings!;

            _trackBars = new TrackBar[4] { Scroll1, Scroll2, Scroll3, Scroll4 };
            _numerics = new NumericUpDown[5] { Numeric1, Numeric2, Numeric3, Numeric4, Numeric5 };

        }

        private void Start()
        {
            _antColony = new AntColony(AntColonyParameters.DefaultParameters());
            SetFormsInitialParameters();
            UpdateColony(this, EventArgs.Empty);
        }

        private void UpdateColony(object sender, EventArgs e)
        {
            AntColonyParameters antColonyParameters = new AntColonyParameters(
                (double)Numeric1.Value, (double)Numeric2.Value, (double)Numeric3.Value, (double)Numeric4.Value, (int)Numeric5.Value);

            _antColony = new AntColony(antColonyParameters);
            _graphDrawer = new GraphDrawer(pictureBox1, _antColony.DistancesGraph, GraphVisuals.DefaultVisuals());

            RedrawGraph();
        }

        private void SingleIteration(object sender, EventArgs e)
        {
            RedrawGraph();
            PathData data = _antColony.SingleIteration();
            _graphDrawer.DrawPath(data.PathIndexes, Color.Red);
        }

        private void FullACO_Search(object sender, EventArgs e)
        {
            RedrawGraph();
            PathData data = _antColony.ACO_Full();
            _graphDrawer.DrawPath(data.PathIndexes, Color.Red);
        }

        private void DefaultSettings(object sender, EventArgs e)
        {
            SetFormsInitialParameters();
        }

        private void RedrawGraph()
        {
            _graphDrawer.Clear();
            _graphDrawer.RedrawGraphBase();
            _graphDrawer.ColorVertex(_antColony.StartingCityIndex, Color.Red);
        }

        // Контролируемые параметры
        private void SetFormsInitialParameters()
        {
            AntColonyParameters parameters = AntColonyParameters.DefaultParameters();
            _trackBars[0].Value = (int)(parameters.PhInfl * (double)TrackerScale);
            _numerics[0].Value = (decimal)(parameters.PhInfl);

            _trackBars[1].Value = (int)(parameters.DistInfl * (double)TrackerScale);
            _numerics[1].Value = (decimal)(parameters.DistInfl);

            _trackBars[2].Value = (int)(parameters.PhEvapRate * (double)TrackerScale);
            _numerics[2].Value = (decimal)(parameters.PhEvapRate);

            _trackBars[3].Value = (int)(parameters.PhStr * (double)TrackerScale);
            _numerics[3].Value = (decimal)(parameters.PhStr);

            _numerics[4].Value = parameters.PathCopyCount;
        }

        private void TrackNumericEvent(object sender, EventArgs e)
        {
            Control control = (Control)sender;

            if (control is TrackBar)
            {
                int num = int.Parse(control.Name[control.Name.Length - 1].ToString()) - 1;

                TrackBar trackBar = _trackBars[num];
                NumericUpDown numericUpDown = _numerics[num];

                decimal value = (trackBar.Value / TrackerScale);
                numericUpDown.Value = value;
            }

            if (control is NumericUpDown)
            {
                int num = int.Parse(control.Name[control.Name.Length - 1].ToString()) - 1;

                if (num < 5)
                {
                    TrackBar trackBar = _trackBars[num];
                    NumericUpDown numericUpDown = _numerics[num];

                    decimal value = (numericUpDown.Value * TrackerScale);
                    trackBar.Value = (int)value;
                }
            }
        }
    }
}
