using LMRItemTracker.Configs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LMRItemTracker
{
    public partial class TrackerSettingsForm : Form
    {
        public TrackerSettingsForm()
        {
            InitializeComponent();
        }

        private void TrackerSettingsForm_Load(object sender, EventArgs e)
        {
            numericUpDownRecognitionThreshold.Value = Properties.Settings.Default.RecognitionThreshold;
            numericUpDownExecutionThreshold.Value = Properties.Settings.Default.ExecutionThreshold;
            textBoxRandomizerPath.Text = Properties.Settings.Default.RandomizerPath;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void buttonOkay_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.RecognitionThreshold = (int)numericUpDownRecognitionThreshold.Value;
            Properties.Settings.Default.ExecutionThreshold = (int)numericUpDownExecutionThreshold.Value;
            Properties.Settings.Default.RandomizerPath = textBoxRandomizerPath.Text;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonRandomizerPath_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.SelectedPath = textBoxRandomizerPath.Text;
                var result = dialog.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    var files = Directory.GetFiles(dialog.SelectedPath);
                    if (files.Any(x => Path.GetExtension(x) == ".jar"))
                    {
                        textBoxRandomizerPath.Text = dialog.SelectedPath;
                    }
                    else
                    {
                        MessageBox.Show("No randomizer jar file found in selected file");
                    }
                }
            }
        }
    }
}
