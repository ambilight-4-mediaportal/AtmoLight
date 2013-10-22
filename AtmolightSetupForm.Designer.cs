namespace MediaPortal.ProcessPlugins.Atmolight
{
  partial class AtmolightSetupForm
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
        this.label1 = new System.Windows.Forms.Label();
        this.edFile = new System.Windows.Forms.TextBox();
        this.btnSelectFile = new System.Windows.Forms.Button();
        this.btnSave = new System.Windows.Forms.Button();
        this.btnCancel = new System.Windows.Forms.Button();
        this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
        this.groupBox2 = new System.Windows.Forms.GroupBox();
        this.rbDisableLEDs = new System.Windows.Forms.RadioButton();
        this.rbSwitchToLiveView = new System.Windows.Forms.RadioButton();
        this.groupBox1 = new System.Windows.Forms.GroupBox();
        this.cbRadio = new System.Windows.Forms.ComboBox();
        this.label4 = new System.Windows.Forms.Label();
        this.cbMusic = new System.Windows.Forms.ComboBox();
        this.label3 = new System.Windows.Forms.Label();
        this.cbVideo = new System.Windows.Forms.ComboBox();
        this.label2 = new System.Windows.Forms.Label();
        this.groupBox3 = new System.Windows.Forms.GroupBox();
        this.edExcludeEnd = new System.Windows.Forms.TextBox();
        this.label6 = new System.Windows.Forms.Label();
        this.edExcludeStart = new System.Windows.Forms.TextBox();
        this.label5 = new System.Windows.Forms.Label();
        this.label7 = new System.Windows.Forms.Label();
        this.groupBox4 = new System.Windows.Forms.GroupBox();
        this.checkBox2 = new System.Windows.Forms.CheckBox();
        this.checkBox1 = new System.Windows.Forms.CheckBox();
        this.comboBox1 = new System.Windows.Forms.ComboBox();
        this.label8 = new System.Windows.Forms.Label();
        this.checkBox3 = new System.Windows.Forms.CheckBox();
        this.groupBox2.SuspendLayout();
        this.groupBox1.SuspendLayout();
        this.groupBox3.SuspendLayout();
        this.groupBox4.SuspendLayout();
        this.SuspendLayout();
        // 
        // label1
        // 
        this.label1.AutoSize = true;
        this.label1.Location = new System.Drawing.Point(6, 13);
        this.label1.Name = "label1";
        this.label1.Size = new System.Drawing.Size(162, 13);
        this.label1.TabIndex = 0;
        this.label1.Text = "Path+Filename of AtmoWinA.exe";
        // 
        // edFile
        // 
        this.edFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                    | System.Windows.Forms.AnchorStyles.Right)));
        this.edFile.Location = new System.Drawing.Point(9, 30);
        this.edFile.Name = "edFile";
        this.edFile.Size = new System.Drawing.Size(393, 20);
        this.edFile.TabIndex = 1;
        // 
        // btnSelectFile
        // 
        this.btnSelectFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
        this.btnSelectFile.Location = new System.Drawing.Point(409, 29);
        this.btnSelectFile.Name = "btnSelectFile";
        this.btnSelectFile.Size = new System.Drawing.Size(36, 23);
        this.btnSelectFile.TabIndex = 2;
        this.btnSelectFile.Text = "...";
        this.btnSelectFile.UseVisualStyleBackColor = true;
        this.btnSelectFile.Click += new System.EventHandler(this.btnSelectFile_Click);
        // 
        // btnSave
        // 
        this.btnSave.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
        this.btnSave.Location = new System.Drawing.Point(82, 382);
        this.btnSave.Name = "btnSave";
        this.btnSave.Size = new System.Drawing.Size(75, 23);
        this.btnSave.TabIndex = 4;
        this.btnSave.Text = "Save";
        this.btnSave.UseVisualStyleBackColor = true;
        this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
        // 
        // btnCancel
        // 
        this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
        this.btnCancel.Location = new System.Drawing.Point(293, 382);
        this.btnCancel.Name = "btnCancel";
        this.btnCancel.Size = new System.Drawing.Size(75, 23);
        this.btnCancel.TabIndex = 5;
        this.btnCancel.Text = "Cancel";
        this.btnCancel.UseVisualStyleBackColor = true;
        this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
        // 
        // openFileDialog1
        // 
        this.openFileDialog1.FileName = "openFileDialog1";
        this.openFileDialog1.Filter = "AtmoWinA.exe|*.exe";
        this.openFileDialog1.RestoreDirectory = true;
        // 
        // groupBox2
        // 
        this.groupBox2.Controls.Add(this.rbDisableLEDs);
        this.groupBox2.Controls.Add(this.rbSwitchToLiveView);
        this.groupBox2.Location = new System.Drawing.Point(9, 291);
        this.groupBox2.Name = "groupBox2";
        this.groupBox2.Size = new System.Drawing.Size(291, 75);
        this.groupBox2.TabIndex = 6;
        this.groupBox2.TabStop = false;
        this.groupBox2.Text = "On MP close...";
        // 
        // rbDisableLEDs
        // 
        this.rbDisableLEDs.AutoSize = true;
        this.rbDisableLEDs.Location = new System.Drawing.Point(7, 43);
        this.rbDisableLEDs.Name = "rbDisableLEDs";
        this.rbDisableLEDs.Size = new System.Drawing.Size(114, 17);
        this.rbDisableLEDs.TabIndex = 1;
        this.rbDisableLEDs.TabStop = true;
        this.rbDisableLEDs.Text = "Switch all LEDs off";
        this.rbDisableLEDs.UseVisualStyleBackColor = true;
        // 
        // rbSwitchToLiveView
        // 
        this.rbSwitchToLiveView.AutoSize = true;
        this.rbSwitchToLiveView.Location = new System.Drawing.Point(7, 20);
        this.rbSwitchToLiveView.Name = "rbSwitchToLiveView";
        this.rbSwitchToLiveView.Size = new System.Drawing.Size(232, 17);
        this.rbSwitchToLiveView.TabIndex = 0;
        this.rbSwitchToLiveView.TabStop = true;
        this.rbSwitchToLiveView.Text = "Switch to AtmoWin\'s internal live view mode";
        this.rbSwitchToLiveView.UseVisualStyleBackColor = true;
        // 
        // groupBox1
        // 
        this.groupBox1.Controls.Add(this.cbRadio);
        this.groupBox1.Controls.Add(this.label4);
        this.groupBox1.Controls.Add(this.cbMusic);
        this.groupBox1.Controls.Add(this.label3);
        this.groupBox1.Controls.Add(this.cbVideo);
        this.groupBox1.Controls.Add(this.label2);
        this.groupBox1.Location = new System.Drawing.Point(9, 56);
        this.groupBox1.Name = "groupBox1";
        this.groupBox1.Size = new System.Drawing.Size(291, 106);
        this.groupBox1.TabIndex = 7;
        this.groupBox1.TabStop = false;
        this.groupBox1.Text = "Atmolight Mode per content type";
        // 
        // cbRadio
        // 
        this.cbRadio.FormattingEnabled = true;
        this.cbRadio.Items.AddRange(new object[] {
            "LEDs disabled",
            "AtmoWin GDI Live-view",
            "Colorchanger",
            "Colorchanger LR"});
        this.cbRadio.Location = new System.Drawing.Point(131, 77);
        this.cbRadio.Name = "cbRadio";
        this.cbRadio.Size = new System.Drawing.Size(154, 21);
        this.cbRadio.TabIndex = 5;
        // 
        // label4
        // 
        this.label4.AutoSize = true;
        this.label4.Location = new System.Drawing.Point(10, 80);
        this.label4.Name = "label4";
        this.label4.Size = new System.Drawing.Size(38, 13);
        this.label4.TabIndex = 4;
        this.label4.Text = "Radio:";
        // 
        // cbMusic
        // 
        this.cbMusic.FormattingEnabled = true;
        this.cbMusic.Items.AddRange(new object[] {
            "LEDs disabled",
            "AtmoWin GDI Live-view",
            "Colorchanger",
            "Colorchanger LR"});
        this.cbMusic.Location = new System.Drawing.Point(131, 50);
        this.cbMusic.Name = "cbMusic";
        this.cbMusic.Size = new System.Drawing.Size(154, 21);
        this.cbMusic.TabIndex = 3;
        // 
        // label3
        // 
        this.label3.AutoSize = true;
        this.label3.Location = new System.Drawing.Point(10, 53);
        this.label3.Name = "label3";
        this.label3.Size = new System.Drawing.Size(38, 13);
        this.label3.TabIndex = 2;
        this.label3.Text = "Music:";
        // 
        // cbVideo
        // 
        this.cbVideo.FormattingEnabled = true;
        this.cbVideo.Items.AddRange(new object[] {
            "LEDs disabled",
            "AtmoWin GDI Live-view",
            "Colorchanger",
            "Colorchanger LR",
            "MP Live-view"});
        this.cbVideo.Location = new System.Drawing.Point(131, 23);
        this.cbVideo.Name = "cbVideo";
        this.cbVideo.Size = new System.Drawing.Size(154, 21);
        this.cbVideo.TabIndex = 1;
        // 
        // label2
        // 
        this.label2.AutoSize = true;
        this.label2.Location = new System.Drawing.Point(10, 26);
        this.label2.Name = "label2";
        this.label2.Size = new System.Drawing.Size(114, 13);
        this.label2.TabIndex = 0;
        this.label2.Text = "Video/Tv/Recordings:";
        // 
        // groupBox3
        // 
        this.groupBox3.Controls.Add(this.edExcludeEnd);
        this.groupBox3.Controls.Add(this.label6);
        this.groupBox3.Controls.Add(this.edExcludeStart);
        this.groupBox3.Controls.Add(this.label5);
        this.groupBox3.Location = new System.Drawing.Point(307, 57);
        this.groupBox3.Name = "groupBox3";
        this.groupBox3.Size = new System.Drawing.Size(125, 80);
        this.groupBox3.TabIndex = 8;
        this.groupBox3.TabStop = false;
        this.groupBox3.Text = "Deactive between...";
        // 
        // edExcludeEnd
        // 
        this.edExcludeEnd.Location = new System.Drawing.Point(45, 47);
        this.edExcludeEnd.Name = "edExcludeEnd";
        this.edExcludeEnd.Size = new System.Drawing.Size(67, 20);
        this.edExcludeEnd.TabIndex = 3;
        // 
        // label6
        // 
        this.label6.AutoSize = true;
        this.label6.Location = new System.Drawing.Point(7, 50);
        this.label6.Name = "label6";
        this.label6.Size = new System.Drawing.Size(29, 13);
        this.label6.TabIndex = 2;
        this.label6.Text = "End:";
        // 
        // edExcludeStart
        // 
        this.edExcludeStart.Location = new System.Drawing.Point(45, 21);
        this.edExcludeStart.Name = "edExcludeStart";
        this.edExcludeStart.Size = new System.Drawing.Size(67, 20);
        this.edExcludeStart.TabIndex = 1;
        // 
        // label5
        // 
        this.label5.AutoSize = true;
        this.label5.Location = new System.Drawing.Point(7, 24);
        this.label5.Name = "label5";
        this.label5.Size = new System.Drawing.Size(32, 13);
        this.label5.TabIndex = 0;
        this.label5.Text = "Start:";
        // 
        // label7
        // 
        this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.label7.Location = new System.Drawing.Point(307, 153);
        this.label7.Name = "label7";
        this.label7.Size = new System.Drawing.Size(138, 81);
        this.label7.TabIndex = 9;
        this.label7.Text = "Hint: You can use the \"STOP\" button when nothing is playing to enable GDI Live-Vi" +
            "ew, disable the LEDs or switch 3D-SBS Mode.";
        // 
        // groupBox4
        // 
        this.groupBox4.Controls.Add(this.checkBox3);
        this.groupBox4.Controls.Add(this.checkBox2);
        this.groupBox4.Controls.Add(this.checkBox1);
        this.groupBox4.Controls.Add(this.comboBox1);
        this.groupBox4.Controls.Add(this.label8);
        this.groupBox4.Location = new System.Drawing.Point(9, 170);
        this.groupBox4.Name = "groupBox4";
        this.groupBox4.Size = new System.Drawing.Size(290, 115);
        this.groupBox4.TabIndex = 11;
        this.groupBox4.TabStop = false;
        this.groupBox4.Text = "Plugin options";
        // 
        // checkBox2
        // 
        this.checkBox2.AutoSize = true;
        this.checkBox2.Location = new System.Drawing.Point(13, 40);
        this.checkBox2.Name = "checkBox2";
        this.checkBox2.Size = new System.Drawing.Size(208, 17);
        this.checkBox2.TabIndex = 12;
        this.checkBox2.Text = "LEDs off on media start (manual mode)";
        this.checkBox2.UseVisualStyleBackColor = true;
        // 
        // checkBox1
        // 
        this.checkBox1.AutoSize = true;
        this.checkBox1.Location = new System.Drawing.Point(13, 63);
        this.checkBox1.Name = "checkBox1";
        this.checkBox1.Size = new System.Drawing.Size(120, 17);
        this.checkBox1.TabIndex = 11;
        this.checkBox1.Text = "disable STOP menu";
        this.checkBox1.UseVisualStyleBackColor = true;
        // 
        // comboBox1
        // 
        this.comboBox1.FormattingEnabled = true;
        this.comboBox1.Items.AddRange(new object[] {
            "red",
            "green",
            "yellow",
            "blue"});
        this.comboBox1.Location = new System.Drawing.Point(140, 13);
        this.comboBox1.Name = "comboBox1";
        this.comboBox1.Size = new System.Drawing.Size(144, 21);
        this.comboBox1.TabIndex = 9;
        // 
        // label8
        // 
        this.label8.AutoSize = true;
        this.label8.Location = new System.Drawing.Point(9, 16);
        this.label8.Name = "label8";
        this.label8.Size = new System.Drawing.Size(125, 13);
        this.label8.TabIndex = 8;
        this.label8.Text = "LEDs OnOff RemoteKey:";
        // 
        // checkBox3
        // 
        this.checkBox3.AutoSize = true;
        this.checkBox3.Location = new System.Drawing.Point(13, 86);
        this.checkBox3.Name = "checkBox3";
        this.checkBox3.Size = new System.Drawing.Size(67, 17);
        this.checkBox3.TabIndex = 13;
        this.checkBox3.Text = "low CPU";
        this.checkBox3.UseVisualStyleBackColor = true;
        // 
        // AtmolightSetupForm
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(452, 414);
        this.Controls.Add(this.groupBox4);
        this.Controls.Add(this.label7);
        this.Controls.Add(this.groupBox3);
        this.Controls.Add(this.groupBox1);
        this.Controls.Add(this.groupBox2);
        this.Controls.Add(this.btnCancel);
        this.Controls.Add(this.btnSave);
        this.Controls.Add(this.btnSelectFile);
        this.Controls.Add(this.edFile);
        this.Controls.Add(this.label1);
        this.Name = "AtmolightSetupForm";
        this.Text = "Atmolight Setup";
        this.groupBox2.ResumeLayout(false);
        this.groupBox2.PerformLayout();
        this.groupBox1.ResumeLayout(false);
        this.groupBox1.PerformLayout();
        this.groupBox3.ResumeLayout(false);
        this.groupBox3.PerformLayout();
        this.groupBox4.ResumeLayout(false);
        this.groupBox4.PerformLayout();
        this.ResumeLayout(false);
        this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox edFile;
    private System.Windows.Forms.Button btnSelectFile;
    private System.Windows.Forms.Button btnSave;
    private System.Windows.Forms.Button btnCancel;
    private System.Windows.Forms.OpenFileDialog openFileDialog1;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.RadioButton rbDisableLEDs;
    private System.Windows.Forms.RadioButton rbSwitchToLiveView;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.ComboBox cbRadio;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.ComboBox cbMusic;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.ComboBox cbVideo;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.GroupBox groupBox3;
    private System.Windows.Forms.TextBox edExcludeEnd;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.TextBox edExcludeStart;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.GroupBox groupBox4;
    private System.Windows.Forms.ComboBox comboBox1;
    private System.Windows.Forms.Label label8;
    private System.Windows.Forms.CheckBox checkBox2;
    private System.Windows.Forms.CheckBox checkBox1;
    private System.Windows.Forms.CheckBox checkBox3;
  }
}