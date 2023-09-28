using MathLib;
using System.Windows.Forms;

namespace AILabs.LabAnts
{
    public partial class Form1 : Form
    {
        private const decimal TrackerScale = 100.0M;

        private AntColony _antColony;

        private GraphDrawer _graphDrawer;

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
        }

        private void Start()
        {
            _antColony = new AntColony();

            _graphDrawer = new GraphDrawer(pictureBox1, _antColony._distGraph, GraphVisuals.DefaultVisuals());
        }

        private void IterateACO()
        {
            _graphDrawer.Clear();
            _graphDrawer.RedrawGraphBase();
            _graphDrawer.ColorVertex(_antColony.StartingCityIndex, Color.Red);

            PathData data = _antColony.ACO_Start();



            _antColony.UpdatePheromones(data);
            _graphDrawer.DrawPath(data.PathIndexes, Color.Red);



            // _graphDrawer.DrawPath(new List<int>() { 0, 1, 2, 3, 4, 2, 0 }, Color.Red);
        }


        // Кнопки
        private void button1_Click(object sender, EventArgs e)
        {
            _graphDrawer.RedrawGraphBase();


        }

        private void button2_Click(object sender, EventArgs e)
        {

            _graphDrawer.ColorVertex(_antColony.StartingCityIndex, Color.Red);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            IterateACO();
        }

        # region ScrollBars

        // Влияние феромонов

        private void TrackNumericEvent(object sender, EventArgs e)
        {
            Control ctr = (Control)sender;

            if (ctr is TrackBar)
            {
                switch (ctr.Name)
                {
                    case "Scroll1":
                        var value = (Scroll1.Value / TrackerScale);
                        Numeric1.Value = value;
                        break;
                }
            }

        }

        private void PhInflTrack_Scroll(object sender, EventArgs e)
        {
            var value = (Scroll1.Value / TrackerScale);
            Numeric1.Value = value;
            _antColony.PhInfl = (double)value;
        }

        private void PhInflNum_ValueChanged(object sender, EventArgs e)
        {
            Scroll1.Value = (int)(Numeric1.Value * TrackerScale);
        }

        // Влияние весов рёбер
        private void DistInflTrack_Scroll(object sender, EventArgs e)
        {
            Numeric2.Value = (decimal)(Scroll2.Value / TrackerScale);
        }

        private void DistInflNum_ValueChanged(object sender, EventArgs e)
        {
            Scroll2.Value = (int)(Numeric2.Value * TrackerScale);
        }

        // Исчезание феромонов
        private void PhEvapTrack_Scroll(object sender, EventArgs e)
        {
            Numeric3.Value = (decimal)(Scroll3.Value / TrackerScale);
        }

        private void PhEvapNum_ValueChanged(object sender, EventArgs e)
        {
            Scroll3.Value = (int)(Numeric3.Value * TrackerScale);
        }

        // Сила феромонов
        private void PhStrTrack_Scroll(object sender, EventArgs e)
        {
            Numeric4.Value = (decimal)(Scroll4.Value / TrackerScale);
        }

        private void PhStrNum_ValueChanged(object sender, EventArgs e)
        {
            Scroll4.Value = (int)(Numeric4.Value * TrackerScale);
        }

        #endregion
    }
}