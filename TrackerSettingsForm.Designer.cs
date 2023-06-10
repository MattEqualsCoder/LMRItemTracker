namespace LMRItemTracker
{
    partial class TrackerSettingsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.panelMain = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.labelRecognitionThreshold = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDownRecognitionThreshold = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownExecutionThreshold = new System.Windows.Forms.NumericUpDown();
            this.labelRandomizerPath = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.textBoxRandomizerPath = new System.Windows.Forms.TextBox();
            this.buttonRandomizerPath = new System.Windows.Forms.Button();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.buttonOkay = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panelMain.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRecognitionThreshold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownExecutionThreshold)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.panelBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panelMain);
            this.panel1.Controls.Add(this.panelBottom);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(10);
            this.panel1.Size = new System.Drawing.Size(415, 177);
            this.panel1.TabIndex = 0;
            // 
            // panelMain
            // 
            this.panelMain.Controls.Add(this.tableLayoutPanel1);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(10, 10);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(395, 112);
            this.panelMain.TabIndex = 2;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 34.68354F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 65.31645F));
            this.tableLayoutPanel1.Controls.Add(this.labelRecognitionThreshold, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.numericUpDownRecognitionThreshold, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.numericUpDownExecutionThreshold, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelRandomizerPath, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 1, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 57F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(395, 112);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // labelRecognitionThreshold
            // 
            this.labelRecognitionThreshold.AutoSize = true;
            this.labelRecognitionThreshold.Location = new System.Drawing.Point(3, 6);
            this.labelRecognitionThreshold.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.labelRecognitionThreshold.Name = "labelRecognitionThreshold";
            this.labelRecognitionThreshold.Size = new System.Drawing.Size(114, 13);
            this.labelRecognitionThreshold.TabIndex = 0;
            this.labelRecognitionThreshold.Text = "Recognition Threshold";
            this.labelRecognitionThreshold.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 33);
            this.label1.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(104, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Execution Threshold";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // numericUpDownRecognitionThreshold
            // 
            this.numericUpDownRecognitionThreshold.Location = new System.Drawing.Point(139, 3);
            this.numericUpDownRecognitionThreshold.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numericUpDownRecognitionThreshold.Name = "numericUpDownRecognitionThreshold";
            this.numericUpDownRecognitionThreshold.Size = new System.Drawing.Size(120, 20);
            this.numericUpDownRecognitionThreshold.TabIndex = 2;
            this.numericUpDownRecognitionThreshold.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // numericUpDownExecutionThreshold
            // 
            this.numericUpDownExecutionThreshold.Dock = System.Windows.Forms.DockStyle.Left;
            this.numericUpDownExecutionThreshold.Location = new System.Drawing.Point(139, 30);
            this.numericUpDownExecutionThreshold.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numericUpDownExecutionThreshold.Name = "numericUpDownExecutionThreshold";
            this.numericUpDownExecutionThreshold.Size = new System.Drawing.Size(120, 20);
            this.numericUpDownExecutionThreshold.TabIndex = 3;
            this.numericUpDownExecutionThreshold.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // labelRandomizerPath
            // 
            this.labelRandomizerPath.AutoSize = true;
            this.labelRandomizerPath.Location = new System.Drawing.Point(3, 60);
            this.labelRandomizerPath.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.labelRandomizerPath.Name = "labelRandomizerPath";
            this.labelRandomizerPath.Size = new System.Drawing.Size(88, 13);
            this.labelRandomizerPath.TabIndex = 4;
            this.labelRandomizerPath.Text = "Randomizer Path";
            this.labelRandomizerPath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.textBoxRandomizerPath);
            this.flowLayoutPanel1.Controls.Add(this.buttonRandomizerPath);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(136, 54);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(258, 58);
            this.flowLayoutPanel1.TabIndex = 5;
            // 
            // textBoxRandomizerPath
            // 
            this.textBoxRandomizerPath.Location = new System.Drawing.Point(3, 3);
            this.textBoxRandomizerPath.Name = "textBoxRandomizerPath";
            this.textBoxRandomizerPath.ReadOnly = true;
            this.textBoxRandomizerPath.Size = new System.Drawing.Size(171, 20);
            this.textBoxRandomizerPath.TabIndex = 0;
            // 
            // buttonRandomizerPath
            // 
            this.buttonRandomizerPath.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonRandomizerPath.Location = new System.Drawing.Point(180, 2);
            this.buttonRandomizerPath.Margin = new System.Windows.Forms.Padding(3, 2, 3, 3);
            this.buttonRandomizerPath.Name = "buttonRandomizerPath";
            this.buttonRandomizerPath.Size = new System.Drawing.Size(75, 23);
            this.buttonRandomizerPath.TabIndex = 1;
            this.buttonRandomizerPath.Text = "Browse";
            this.buttonRandomizerPath.UseVisualStyleBackColor = true;
            this.buttonRandomizerPath.Click += new System.EventHandler(this.buttonRandomizerPath_Click);
            // 
            // panelBottom
            // 
            this.panelBottom.Controls.Add(this.buttonOkay);
            this.panelBottom.Controls.Add(this.buttonCancel);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(10, 122);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(395, 45);
            this.panelBottom.TabIndex = 1;
            // 
            // buttonOkay
            // 
            this.buttonOkay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOkay.Location = new System.Drawing.Point(236, 19);
            this.buttonOkay.Name = "buttonOkay";
            this.buttonOkay.Size = new System.Drawing.Size(75, 23);
            this.buttonOkay.TabIndex = 1;
            this.buttonOkay.Text = "OK";
            this.buttonOkay.UseVisualStyleBackColor = true;
            this.buttonOkay.Click += new System.EventHandler(this.buttonOkay_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.Location = new System.Drawing.Point(317, 19);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 0;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // TrackerSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(415, 177);
            this.Controls.Add(this.panel1);
            this.Name = "TrackerSettingsForm";
            this.Text = "cke";
            this.Load += new System.EventHandler(this.TrackerSettingsForm_Load);
            this.panel1.ResumeLayout(false);
            this.panelMain.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRecognitionThreshold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownExecutionThreshold)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.panelBottom.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.Button buttonOkay;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label labelRecognitionThreshold;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numericUpDownRecognitionThreshold;
        private System.Windows.Forms.NumericUpDown numericUpDownExecutionThreshold;
        private System.Windows.Forms.Label labelRandomizerPath;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.TextBox textBoxRandomizerPath;
        private System.Windows.Forms.Button buttonRandomizerPath;
    }
}