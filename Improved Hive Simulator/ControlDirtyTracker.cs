using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Improved_Hive_Simulator
{
    class ControlDirtyTracker : Dictionary<string, string>
    {
        public delegate void RedrawLabels();

        private RedrawLabels redrawLabelsCallback;
        private Form _form;
        private bool _isDirty;

        public bool IsDirty { get { return _isDirty; } set { _isDirty = value; } }

        public ControlDirtyTracker(Form _form, RedrawLabels redrawLabelsCallback)
        {
            this._form = _form;
            this.redrawLabelsCallback = redrawLabelsCallback;
            AssignHandlersForControls(_form.Controls);
        }

        private void DirtyTracker_TextChanged(object sender, EventArgs e)
        {
            _form.Text = "ControlPanel*";
            _isDirty = true;
        }

        private void DirtyTracker_ValueChanged(object sender, EventArgs e)
        {
            _form.Text = "ControlPanel*";
            _isDirty = true;
            redrawLabelsCallback();
        }

        private void DirtyTracker_CheckedChanged(object sender, EventArgs e)
        {
            _form.Text = "ControlPanel*";
            _isDirty = true;
        }

        private void AssignHandlersForControls(Control.ControlCollection controls)
        {
            foreach(Control ctrl in controls)
            {
                if (ctrl is TextBox)
                {
                    (ctrl as TextBox).TextChanged += new EventHandler(DirtyTracker_TextChanged);
                    this.Add(ctrl.Name, ctrl.Text);
                }
                if(ctrl is UserTrackBar)
                {
                    (ctrl as UserTrackBar).ValueChanged += new EventHandler(DirtyTracker_ValueChanged);
                    this.Add(ctrl.Name, String.Format("{0:f1}", (ctrl as UserTrackBar).Value));
                }
                else if (ctrl is TrackBar)
                {
                    (ctrl as TrackBar).ValueChanged += new EventHandler(DirtyTracker_ValueChanged);
                    this.Add(ctrl.Name, String.Format("{0:f1}",(ctrl as TrackBar).Value));
                }
                if (ctrl is CheckBox)
                {
                    (ctrl as CheckBox).CheckedChanged += new EventHandler(DirtyTracker_CheckedChanged);
                    this.Add(ctrl.Name, (ctrl as CheckBox).Checked.ToString());
                }

                if (ctrl.HasChildren)
                    AssignHandlersForControls(ctrl.Controls);
            }
        } 
    }
}
