using LMRItemTracker.Configs;
using LMRItemTracker.Twitch;
using Microsoft.Extensions.Logging;
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
using System.Windows.Threading;

namespace LMRItemTracker
{
    public partial class TrackerSettingsForm : Form
    {
        private IChatAuthenticationService _chatAuthenticationService;
        private ILogger<TrackerSettingsForm> _logger;

        public TrackerSettingsForm(IChatAuthenticationService chatAuthenticationService, ILogger<TrackerSettingsForm> logger)
        {
            InitializeComponent();
            _chatAuthenticationService = chatAuthenticationService;
            _logger = logger;
    }

        private void TrackerSettingsForm_Load(object sender, EventArgs e)
        {
            numericUpDownRecognitionThreshold.Value = Properties.Settings.Default.RecognitionThreshold;
            numericUpDownExecutionThreshold.Value = Properties.Settings.Default.ExecutionThreshold;
            textBoxRandomizerPath.Text = Properties.Settings.Default.RandomizerPath;
            twitchUsernameTextBox.Text = Properties.Settings.Default.TwitchUserName;
            twitchChannelTextBox.Text = Properties.Settings.Default.TwitchChannel;
            twitchIdTextBox.Text = Properties.Settings.Default.TwitchId;
            checkBoxRespondToChat.Checked = Properties.Settings.Default.EnableTwitchChatResponses;
            checkBoxTwitchPolls.Checked = Properties.Settings.Default.EnableTwitchPolls;
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
            Properties.Settings.Default.TwitchUserName = twitchUsernameTextBox.Text;
            Properties.Settings.Default.TwitchChannel = twitchChannelTextBox.Text;
            Properties.Settings.Default.TwitchId = twitchIdTextBox.Text;
            Properties.Settings.Default.EnableTwitchChatResponses = checkBoxRespondToChat.Checked;
            Properties.Settings.Default.EnableTwitchPolls = checkBoxTwitchPolls.Checked;
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

        private async void buttonTwitchId_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.AppStarting;
            try
            {
                try
                {
                    var token = await _chatAuthenticationService.GetTokenInteractivelyAsync(default);

                    if (token == null)
                    {
                        MessageBox.Show(this, "An unexpected error occurred while trying to log you in with Twitch. " +
                            "Please try again or report this issue with the log file.", "LMR Item Tracker",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    var userData =  await _chatAuthenticationService.GetAuthenticatedUserDataAsync(token, default);
                        

                    if (userData == null)
                    {
                        MessageBox.Show(this, "An unexpected error occurred while trying to log you in with Twitch. " +
                            "Please try again or report this issue with the log file.", "LMR Item Tracker",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    twitchUsernameTextBox.Text = userData.Name;
                    Properties.Settings.Default.TwitchOAuthToken = token;
                    twitchChannelTextBox.Text = string.IsNullOrEmpty(twitchChannelTextBox.Text) ? userData.Name : twitchChannelTextBox.Text;
                    twitchIdTextBox.Text = userData.Id;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An unknown error occurred while logging in with Twitch");
                    MessageBox.Show(this, "An unexpected error occurred while trying to log you in with Twitch. " +
                        "Please try again or report this issue with the log file.", "LMR Item Tracker",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                Cursor = null;
            }
            
        }
    }
}
