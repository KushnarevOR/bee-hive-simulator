using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Improved_Hive_Simulator
{
    public partial class ControlPanel : Form
    {
        private ControlDirtyTracker _dirtyTracker;

        public ControlPanel(World world)
        {
            InitializeComponent();

            userTrackBar1.Value = world.NectarHarvestedPerNewFlower;
            trackBar1.Value = world.FieldMinX;
            trackBar2.Value = world.FieldMinY;
            trackBar3.Value = world.FieldMaxX;
            trackBar4.Value = world.FieldMaxY;
            trackBar5.Value = world.Hive.InitialBees;
            userTrackBar7.Value = world.Hive.InitialHoney;
            userTrackBar8.Value = world.Hive.MaximumHoney;
            userTrackBar9.Value = world.Hive.NectarHoneyRatio;
            trackBar6.Value = world.Hive.MaximumBees;
            userTrackBar11.Value = world.Hive.MinimumHoneyForCreatingBees;

            _dirtyTracker = new ControlDirtyTracker(this, new ControlDirtyTracker.RedrawLabels(RedrawLabels));

            RedrawLabels();
        }

        private void RedrawLabels()
        {
            label12.Text = String.Format("{0:f1}", userTrackBar1.Value);
            label13.Text = String.Format("{0}", trackBar1.Value);
            label14.Text = String.Format("{0}", trackBar2.Value);
            label15.Text = String.Format("{0}", trackBar3.Value);
            label16.Text = String.Format("{0}", trackBar4.Value);
            label17.Text = String.Format("{0}", trackBar5.Value);
            label18.Text = String.Format("{0:f1}", userTrackBar7.Value);
            label19.Text = String.Format("{0:f1}", userTrackBar8.Value);
            label20.Text = String.Format("{0:f1}", userTrackBar9.Value);
            label21.Text = String.Format("{0}", trackBar6.Value);
            label22.Text = String.Format("{0:f1}", userTrackBar11.Value);
        }

        private void cancel_button_Click(object sender, EventArgs e)
        {

        }

        private void ControlPanel_Load(object sender, EventArgs e)
        {

        }
    }
}
