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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AtmolightSetupForm));
      this.lblPathInfo = new System.Windows.Forms.Label();
      this.edFile = new System.Windows.Forms.TextBox();
      this.btnSelectFile = new System.Windows.Forms.Button();
      this.btnSave = new System.Windows.Forms.Button();
      this.btnCancel = new System.Windows.Forms.Button();
      this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
      this.grpMPClose = new System.Windows.Forms.GroupBox();
      this.rbDisableLEDs = new System.Windows.Forms.RadioButton();
      this.rbSwitchToLiveView = new System.Windows.Forms.RadioButton();
      this.grpMode = new System.Windows.Forms.GroupBox();
      this.grpStaticColor = new System.Windows.Forms.GroupBox();
      this.lblRed = new System.Windows.Forms.Label();
      this.tbGreen = new System.Windows.Forms.TextBox();
      this.tbBlue = new System.Windows.Forms.TextBox();
      this.tbRed = new System.Windows.Forms.TextBox();
      this.lblGreen = new System.Windows.Forms.Label();
      this.lblBlue = new System.Windows.Forms.Label();
      this.lblMenu = new System.Windows.Forms.Label();
      this.cbMenu = new System.Windows.Forms.ComboBox();
      this.cbRadio = new System.Windows.Forms.ComboBox();
      this.lblRadio = new System.Windows.Forms.Label();
      this.cbMusic = new System.Windows.Forms.ComboBox();
      this.lblMusic = new System.Windows.Forms.Label();
      this.cbVideo = new System.Windows.Forms.ComboBox();
      this.lblVidTvRec = new System.Windows.Forms.Label();
      this.grpDeactivate = new System.Windows.Forms.GroupBox();
      this.edExcludeEnd = new System.Windows.Forms.TextBox();
      this.lblEnd = new System.Windows.Forms.Label();
      this.edExcludeStart = new System.Windows.Forms.TextBox();
      this.lblStart = new System.Windows.Forms.Label();
      this.grpPluginOption = new System.Windows.Forms.GroupBox();
      this.lblRefreshRate = new System.Windows.Forms.Label();
      this.tbRefreshRate = new System.Windows.Forms.TextBox();
      this.ckRestartOnError = new System.Windows.Forms.CheckBox();
      this.lblDelay = new System.Windows.Forms.Label();
      this.tbDelay = new System.Windows.Forms.TextBox();
      this.ckDelay = new System.Windows.Forms.CheckBox();
      this.lblMenuButton = new System.Windows.Forms.Label();
      this.cbMenuButton = new System.Windows.Forms.ComboBox();
      this.ckExitAtmoWin = new System.Windows.Forms.CheckBox();
      this.ckStartAtmoWin = new System.Windows.Forms.CheckBox();
      this.lblProfile = new System.Windows.Forms.Label();
      this.comboBox2 = new System.Windows.Forms.ComboBox();
      this.lblFrames = new System.Windows.Forms.Label();
      this.lowCpuTime = new System.Windows.Forms.TextBox();
      this.ckLowCpu = new System.Windows.Forms.CheckBox();
      this.ckOnMediaStart = new System.Windows.Forms.CheckBox();
      this.comboBox1 = new System.Windows.Forms.ComboBox();
      this.lblLedsOnOff = new System.Windows.Forms.Label();
      this.lblVersion = new System.Windows.Forms.Label();
      this.lblVersionVal = new System.Windows.Forms.Label();
      this.btnLanguage = new System.Windows.Forms.Button();
      this.openFileDialog2 = new System.Windows.Forms.OpenFileDialog();
      this.lblHint = new System.Windows.Forms.Label();
      this.grpMPClose.SuspendLayout();
      this.grpMode.SuspendLayout();
      this.grpStaticColor.SuspendLayout();
      this.grpDeactivate.SuspendLayout();
      this.grpPluginOption.SuspendLayout();
      this.SuspendLayout();
      // 
      // lblPathInfo
      // 
      this.lblPathInfo.AutoSize = true;
      this.lblPathInfo.Location = new System.Drawing.Point(6, 13);
      this.lblPathInfo.Name = "lblPathInfo";
      this.lblPathInfo.Size = new System.Drawing.Size(162, 13);
      this.lblPathInfo.TabIndex = 0;
      this.lblPathInfo.Text = "Path+Filename of AtmoWinA.exe";
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
      this.btnSave.Location = new System.Drawing.Point(9, 570);
      this.btnSave.Name = "btnSave";
      this.btnSave.Size = new System.Drawing.Size(89, 23);
      this.btnSave.TabIndex = 4;
      this.btnSave.Text = "Save";
      this.btnSave.UseVisualStyleBackColor = true;
      this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
      // 
      // btnCancel
      // 
      this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
      this.btnCancel.Location = new System.Drawing.Point(104, 570);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new System.Drawing.Size(86, 23);
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
      // grpMPClose
      // 
      this.grpMPClose.Controls.Add(this.rbDisableLEDs);
      this.grpMPClose.Controls.Add(this.rbSwitchToLiveView);
      this.grpMPClose.Location = new System.Drawing.Point(9, 490);
      this.grpMPClose.Name = "grpMPClose";
      this.grpMPClose.Size = new System.Drawing.Size(436, 70);
      this.grpMPClose.TabIndex = 6;
      this.grpMPClose.TabStop = false;
      this.grpMPClose.Text = "On MP close...";
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
      // grpMode
      // 
      this.grpMode.Controls.Add(this.grpStaticColor);
      this.grpMode.Controls.Add(this.lblMenu);
      this.grpMode.Controls.Add(this.cbMenu);
      this.grpMode.Controls.Add(this.cbRadio);
      this.grpMode.Controls.Add(this.lblRadio);
      this.grpMode.Controls.Add(this.cbMusic);
      this.grpMode.Controls.Add(this.lblMusic);
      this.grpMode.Controls.Add(this.cbVideo);
      this.grpMode.Controls.Add(this.lblVidTvRec);
      this.grpMode.Location = new System.Drawing.Point(9, 56);
      this.grpMode.Name = "grpMode";
      this.grpMode.Size = new System.Drawing.Size(291, 187);
      this.grpMode.TabIndex = 7;
      this.grpMode.TabStop = false;
      this.grpMode.Text = "Atmolight Mode per content type";
      // 
      // grpStaticColor
      // 
      this.grpStaticColor.Controls.Add(this.lblRed);
      this.grpStaticColor.Controls.Add(this.tbGreen);
      this.grpStaticColor.Controls.Add(this.tbBlue);
      this.grpStaticColor.Controls.Add(this.tbRed);
      this.grpStaticColor.Controls.Add(this.lblGreen);
      this.grpStaticColor.Controls.Add(this.lblBlue);
      this.grpStaticColor.Location = new System.Drawing.Point(7, 131);
      this.grpStaticColor.Name = "grpStaticColor";
      this.grpStaticColor.Size = new System.Drawing.Size(276, 49);
      this.grpStaticColor.TabIndex = 25;
      this.grpStaticColor.TabStop = false;
      this.grpStaticColor.Text = "Static Color";
      // 
      // lblRed
      // 
      this.lblRed.AutoSize = true;
      this.lblRed.Location = new System.Drawing.Point(23, 22);
      this.lblRed.Name = "lblRed";
      this.lblRed.Size = new System.Drawing.Size(30, 13);
      this.lblRed.TabIndex = 19;
      this.lblRed.Text = "Red:";
      // 
      // tbGreen
      // 
      this.tbGreen.Location = new System.Drawing.Point(144, 19);
      this.tbGreen.Name = "tbGreen";
      this.tbGreen.Size = new System.Drawing.Size(40, 20);
      this.tbGreen.TabIndex = 23;
      this.tbGreen.Text = "0";
      // 
      // tbBlue
      // 
      this.tbBlue.Location = new System.Drawing.Point(230, 19);
      this.tbBlue.Name = "tbBlue";
      this.tbBlue.Size = new System.Drawing.Size(40, 20);
      this.tbBlue.TabIndex = 24;
      this.tbBlue.Text = "0";
      // 
      // tbRed
      // 
      this.tbRed.Location = new System.Drawing.Point(59, 19);
      this.tbRed.Name = "tbRed";
      this.tbRed.Size = new System.Drawing.Size(40, 20);
      this.tbRed.TabIndex = 22;
      this.tbRed.Text = "0";
      // 
      // lblGreen
      // 
      this.lblGreen.AutoSize = true;
      this.lblGreen.Location = new System.Drawing.Point(105, 22);
      this.lblGreen.Name = "lblGreen";
      this.lblGreen.Size = new System.Drawing.Size(39, 13);
      this.lblGreen.TabIndex = 20;
      this.lblGreen.Text = "Green:";
      // 
      // lblBlue
      // 
      this.lblBlue.AutoSize = true;
      this.lblBlue.Location = new System.Drawing.Point(196, 22);
      this.lblBlue.Name = "lblBlue";
      this.lblBlue.Size = new System.Drawing.Size(31, 13);
      this.lblBlue.TabIndex = 21;
      this.lblBlue.Text = "Blue:";
      // 
      // lblMenu
      // 
      this.lblMenu.AutoSize = true;
      this.lblMenu.Location = new System.Drawing.Point(10, 107);
      this.lblMenu.Name = "lblMenu";
      this.lblMenu.Size = new System.Drawing.Size(61, 13);
      this.lblMenu.TabIndex = 18;
      this.lblMenu.Text = "Menu/GUI:";
      // 
      // cbMenu
      // 
      this.cbMenu.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cbMenu.FormattingEnabled = true;
      this.cbMenu.Items.AddRange(new object[] {
            "LEDs disabled",
            "AtmoWin Live Mode",
            "Colorchanger",
            "Colorchanger LR",
            "Static Color"});
      this.cbMenu.Location = new System.Drawing.Point(145, 104);
      this.cbMenu.Name = "cbMenu";
      this.cbMenu.Size = new System.Drawing.Size(140, 21);
      this.cbMenu.TabIndex = 17;
      // 
      // cbRadio
      // 
      this.cbRadio.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cbRadio.FormattingEnabled = true;
      this.cbRadio.Items.AddRange(new object[] {
            "LEDs disabled",
            "AtmoWin Live Mode",
            "Colorchanger",
            "Colorchanger LR",
            "Static Color"});
      this.cbRadio.Location = new System.Drawing.Point(145, 77);
      this.cbRadio.Name = "cbRadio";
      this.cbRadio.Size = new System.Drawing.Size(140, 21);
      this.cbRadio.TabIndex = 5;
      // 
      // lblRadio
      // 
      this.lblRadio.AutoSize = true;
      this.lblRadio.Location = new System.Drawing.Point(10, 80);
      this.lblRadio.Name = "lblRadio";
      this.lblRadio.Size = new System.Drawing.Size(38, 13);
      this.lblRadio.TabIndex = 4;
      this.lblRadio.Text = "Radio:";
      // 
      // cbMusic
      // 
      this.cbMusic.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cbMusic.FormattingEnabled = true;
      this.cbMusic.Items.AddRange(new object[] {
            "LEDs disabled",
            "AtmoWin Live Mode",
            "Colorchanger",
            "Colorchanger LR",
            "Static Color"});
      this.cbMusic.Location = new System.Drawing.Point(145, 50);
      this.cbMusic.Name = "cbMusic";
      this.cbMusic.Size = new System.Drawing.Size(140, 21);
      this.cbMusic.TabIndex = 3;
      // 
      // lblMusic
      // 
      this.lblMusic.AutoSize = true;
      this.lblMusic.Location = new System.Drawing.Point(10, 53);
      this.lblMusic.Name = "lblMusic";
      this.lblMusic.Size = new System.Drawing.Size(38, 13);
      this.lblMusic.TabIndex = 2;
      this.lblMusic.Text = "Music:";
      // 
      // cbVideo
      // 
      this.cbVideo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cbVideo.FormattingEnabled = true;
      this.cbVideo.Items.AddRange(new object[] {
            "LEDs disabled",
            "AtmoWin Live Mode",
            "Colorchanger",
            "Colorchanger LR",
            "MediaPortal Live Mode",
            "Static Color"});
      this.cbVideo.Location = new System.Drawing.Point(145, 23);
      this.cbVideo.Name = "cbVideo";
      this.cbVideo.Size = new System.Drawing.Size(140, 21);
      this.cbVideo.TabIndex = 1;
      // 
      // lblVidTvRec
      // 
      this.lblVidTvRec.AutoSize = true;
      this.lblVidTvRec.Location = new System.Drawing.Point(10, 26);
      this.lblVidTvRec.Name = "lblVidTvRec";
      this.lblVidTvRec.Size = new System.Drawing.Size(115, 13);
      this.lblVidTvRec.TabIndex = 0;
      this.lblVidTvRec.Text = "Video/TV/Recordings:";
      // 
      // grpDeactivate
      // 
      this.grpDeactivate.Controls.Add(this.edExcludeEnd);
      this.grpDeactivate.Controls.Add(this.lblEnd);
      this.grpDeactivate.Controls.Add(this.edExcludeStart);
      this.grpDeactivate.Controls.Add(this.lblStart);
      this.grpDeactivate.Location = new System.Drawing.Point(307, 57);
      this.grpDeactivate.Name = "grpDeactivate";
      this.grpDeactivate.Size = new System.Drawing.Size(138, 80);
      this.grpDeactivate.TabIndex = 8;
      this.grpDeactivate.TabStop = false;
      this.grpDeactivate.Text = "Deactive between...";
      // 
      // edExcludeEnd
      // 
      this.edExcludeEnd.Location = new System.Drawing.Point(75, 47);
      this.edExcludeEnd.Name = "edExcludeEnd";
      this.edExcludeEnd.Size = new System.Drawing.Size(50, 20);
      this.edExcludeEnd.TabIndex = 3;
      // 
      // lblEnd
      // 
      this.lblEnd.AutoSize = true;
      this.lblEnd.Location = new System.Drawing.Point(7, 50);
      this.lblEnd.Name = "lblEnd";
      this.lblEnd.Size = new System.Drawing.Size(29, 13);
      this.lblEnd.TabIndex = 2;
      this.lblEnd.Text = "End:";
      // 
      // edExcludeStart
      // 
      this.edExcludeStart.Location = new System.Drawing.Point(75, 21);
      this.edExcludeStart.Name = "edExcludeStart";
      this.edExcludeStart.Size = new System.Drawing.Size(50, 20);
      this.edExcludeStart.TabIndex = 1;
      // 
      // lblStart
      // 
      this.lblStart.AutoSize = true;
      this.lblStart.Location = new System.Drawing.Point(7, 24);
      this.lblStart.Name = "lblStart";
      this.lblStart.Size = new System.Drawing.Size(32, 13);
      this.lblStart.TabIndex = 0;
      this.lblStart.Text = "Start:";
      // 
      // grpPluginOption
      // 
      this.grpPluginOption.Controls.Add(this.lblRefreshRate);
      this.grpPluginOption.Controls.Add(this.tbRefreshRate);
      this.grpPluginOption.Controls.Add(this.ckRestartOnError);
      this.grpPluginOption.Controls.Add(this.lblDelay);
      this.grpPluginOption.Controls.Add(this.tbDelay);
      this.grpPluginOption.Controls.Add(this.ckDelay);
      this.grpPluginOption.Controls.Add(this.lblMenuButton);
      this.grpPluginOption.Controls.Add(this.cbMenuButton);
      this.grpPluginOption.Controls.Add(this.ckExitAtmoWin);
      this.grpPluginOption.Controls.Add(this.ckStartAtmoWin);
      this.grpPluginOption.Controls.Add(this.lblProfile);
      this.grpPluginOption.Controls.Add(this.comboBox2);
      this.grpPluginOption.Controls.Add(this.lblFrames);
      this.grpPluginOption.Controls.Add(this.lowCpuTime);
      this.grpPluginOption.Controls.Add(this.ckLowCpu);
      this.grpPluginOption.Controls.Add(this.ckOnMediaStart);
      this.grpPluginOption.Controls.Add(this.comboBox1);
      this.grpPluginOption.Controls.Add(this.lblLedsOnOff);
      this.grpPluginOption.Location = new System.Drawing.Point(10, 249);
      this.grpPluginOption.Name = "grpPluginOption";
      this.grpPluginOption.Size = new System.Drawing.Size(435, 235);
      this.grpPluginOption.TabIndex = 11;
      this.grpPluginOption.TabStop = false;
      this.grpPluginOption.Text = "Plugin options";
      // 
      // lblRefreshRate
      // 
      this.lblRefreshRate.AutoSize = true;
      this.lblRefreshRate.Location = new System.Drawing.Point(315, 139);
      this.lblRefreshRate.Name = "lblRefreshRate";
      this.lblRefreshRate.Size = new System.Drawing.Size(20, 13);
      this.lblRefreshRate.TabIndex = 27;
      this.lblRefreshRate.Text = "Hz";
      // 
      // tbRefreshRate
      // 
      this.tbRefreshRate.Location = new System.Drawing.Point(270, 136);
      this.tbRefreshRate.Name = "tbRefreshRate";
      this.tbRefreshRate.Size = new System.Drawing.Size(41, 20);
      this.tbRefreshRate.TabIndex = 26;
      this.tbRefreshRate.Text = "50";
      // 
      // ckRestartOnError
      // 
      this.ckRestartOnError.AutoSize = true;
      this.ckRestartOnError.Checked = true;
      this.ckRestartOnError.CheckState = System.Windows.Forms.CheckState.Checked;
      this.ckRestartOnError.Location = new System.Drawing.Point(13, 208);
      this.ckRestartOnError.Name = "ckRestartOnError";
      this.ckRestartOnError.Size = new System.Drawing.Size(224, 17);
      this.ckRestartOnError.TabIndex = 25;
      this.ckRestartOnError.Text = "Restart AtmoWin and Connection on Error";
      this.ckRestartOnError.UseVisualStyleBackColor = true;
      // 
      // lblDelay
      // 
      this.lblDelay.AutoSize = true;
      this.lblDelay.Location = new System.Drawing.Point(165, 139);
      this.lblDelay.Name = "lblDelay";
      this.lblDelay.Size = new System.Drawing.Size(62, 13);
      this.lblDelay.TabIndex = 24;
      this.lblDelay.Text = "ms Delay at";
      // 
      // tbDelay
      // 
      this.tbDelay.Location = new System.Drawing.Point(120, 136);
      this.tbDelay.Name = "tbDelay";
      this.tbDelay.Size = new System.Drawing.Size(41, 20);
      this.tbDelay.TabIndex = 23;
      this.tbDelay.Text = "0";
      // 
      // ckDelay
      // 
      this.ckDelay.AutoSize = true;
      this.ckDelay.Location = new System.Drawing.Point(13, 138);
      this.ckDelay.Name = "ckDelay";
      this.ckDelay.Size = new System.Drawing.Size(77, 17);
      this.ckDelay.TabIndex = 22;
      this.ckDelay.Text = "LED Delay";
      this.ckDelay.UseVisualStyleBackColor = true;
      // 
      // lblMenuButton
      // 
      this.lblMenuButton.AutoSize = true;
      this.lblMenuButton.Location = new System.Drawing.Point(10, 18);
      this.lblMenuButton.Name = "lblMenuButton";
      this.lblMenuButton.Size = new System.Drawing.Size(95, 13);
      this.lblMenuButton.TabIndex = 21;
      this.lblMenuButton.Text = "Menu RemoteKey:";
      // 
      // cbMenuButton
      // 
      this.cbMenuButton.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cbMenuButton.FormattingEnabled = true;
      this.cbMenuButton.Items.AddRange(new object[] {
            "Red",
            "Green",
            "Yellow",
            "Blue",
            "None"});
      this.cbMenuButton.Location = new System.Drawing.Point(188, 15);
      this.cbMenuButton.Name = "cbMenuButton";
      this.cbMenuButton.Size = new System.Drawing.Size(96, 21);
      this.cbMenuButton.TabIndex = 20;
      // 
      // ckExitAtmoWin
      // 
      this.ckExitAtmoWin.AutoSize = true;
      this.ckExitAtmoWin.Checked = true;
      this.ckExitAtmoWin.CheckState = System.Windows.Forms.CheckState.Checked;
      this.ckExitAtmoWin.Location = new System.Drawing.Point(13, 184);
      this.ckExitAtmoWin.Name = "ckExitAtmoWin";
      this.ckExitAtmoWin.Size = new System.Drawing.Size(173, 17);
      this.ckExitAtmoWin.TabIndex = 19;
      this.ckExitAtmoWin.Text = "Exit AtmoWin with MediaPortal ";
      this.ckExitAtmoWin.UseVisualStyleBackColor = true;
      // 
      // ckStartAtmoWin
      // 
      this.ckStartAtmoWin.AutoSize = true;
      this.ckStartAtmoWin.Checked = true;
      this.ckStartAtmoWin.CheckState = System.Windows.Forms.CheckState.Checked;
      this.ckStartAtmoWin.Location = new System.Drawing.Point(13, 161);
      this.ckStartAtmoWin.Name = "ckStartAtmoWin";
      this.ckStartAtmoWin.Size = new System.Drawing.Size(175, 17);
      this.ckStartAtmoWin.TabIndex = 18;
      this.ckStartAtmoWin.Text = "Start AtmoWin with MediaPortal";
      this.ckStartAtmoWin.UseVisualStyleBackColor = true;
      // 
      // lblProfile
      // 
      this.lblProfile.AutoSize = true;
      this.lblProfile.Location = new System.Drawing.Point(10, 68);
      this.lblProfile.Name = "lblProfile";
      this.lblProfile.Size = new System.Drawing.Size(97, 13);
      this.lblProfile.TabIndex = 17;
      this.lblProfile.Text = "Profile RemoteKey:";
      // 
      // comboBox2
      // 
      this.comboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.comboBox2.FormattingEnabled = true;
      this.comboBox2.Items.AddRange(new object[] {
            "Red",
            "Green",
            "Yellow",
            "Blue",
            "None"});
      this.comboBox2.Location = new System.Drawing.Point(188, 65);
      this.comboBox2.Name = "comboBox2";
      this.comboBox2.Size = new System.Drawing.Size(96, 21);
      this.comboBox2.TabIndex = 16;
      // 
      // lblFrames
      // 
      this.lblFrames.AutoSize = true;
      this.lblFrames.Location = new System.Drawing.Point(165, 116);
      this.lblFrames.Name = "lblFrames";
      this.lblFrames.Size = new System.Drawing.Size(101, 13);
      this.lblFrames.TabIndex = 15;
      this.lblFrames.Text = "ms between Frames";
      // 
      // lowCpuTime
      // 
      this.lowCpuTime.Location = new System.Drawing.Point(120, 113);
      this.lowCpuTime.MaxLength = 4;
      this.lowCpuTime.Name = "lowCpuTime";
      this.lowCpuTime.Size = new System.Drawing.Size(41, 20);
      this.lowCpuTime.TabIndex = 14;
      this.lowCpuTime.Text = "0";
      // 
      // ckLowCpu
      // 
      this.ckLowCpu.AutoSize = true;
      this.ckLowCpu.Location = new System.Drawing.Point(13, 115);
      this.ckLowCpu.Name = "ckLowCpu";
      this.ckLowCpu.Size = new System.Drawing.Size(71, 17);
      this.ckLowCpu.TabIndex = 13;
      this.ckLowCpu.Text = "Low CPU";
      this.ckLowCpu.UseVisualStyleBackColor = true;
      // 
      // ckOnMediaStart
      // 
      this.ckOnMediaStart.AutoSize = true;
      this.ckOnMediaStart.Location = new System.Drawing.Point(13, 92);
      this.ckOnMediaStart.Name = "ckOnMediaStart";
      this.ckOnMediaStart.Size = new System.Drawing.Size(208, 17);
      this.ckOnMediaStart.TabIndex = 12;
      this.ckOnMediaStart.Text = "LEDs off on media start (manual mode)";
      this.ckOnMediaStart.UseVisualStyleBackColor = true;
      // 
      // comboBox1
      // 
      this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.comboBox1.FormattingEnabled = true;
      this.comboBox1.Items.AddRange(new object[] {
            "Red",
            "Green",
            "Yellow",
            "Blue",
            "None"});
      this.comboBox1.Location = new System.Drawing.Point(188, 40);
      this.comboBox1.Name = "comboBox1";
      this.comboBox1.Size = new System.Drawing.Size(96, 21);
      this.comboBox1.TabIndex = 9;
      // 
      // lblLedsOnOff
      // 
      this.lblLedsOnOff.AutoSize = true;
      this.lblLedsOnOff.Location = new System.Drawing.Point(10, 43);
      this.lblLedsOnOff.Name = "lblLedsOnOff";
      this.lblLedsOnOff.Size = new System.Drawing.Size(125, 13);
      this.lblLedsOnOff.TabIndex = 8;
      this.lblLedsOnOff.Text = "LEDs OnOff RemoteKey:";
      // 
      // lblVersion
      // 
      this.lblVersion.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
      this.lblVersion.Location = new System.Drawing.Point(350, 570);
      this.lblVersion.Name = "lblVersion";
      this.lblVersion.Size = new System.Drawing.Size(95, 26);
      this.lblVersion.TabIndex = 13;
      this.lblVersion.Text = "AtmoLight Plugin Version:";
      // 
      // lblVersionVal
      // 
      this.lblVersionVal.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
      this.lblVersionVal.AutoSize = true;
      this.lblVersionVal.Location = new System.Drawing.Point(392, 583);
      this.lblVersionVal.Name = "lblVersionVal";
      this.lblVersionVal.Size = new System.Drawing.Size(40, 13);
      this.lblVersionVal.TabIndex = 14;
      this.lblVersionVal.Text = "0.0.0.0";
      // 
      // btnLanguage
      // 
      this.btnLanguage.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
      this.btnLanguage.Location = new System.Drawing.Point(196, 570);
      this.btnLanguage.Name = "btnLanguage";
      this.btnLanguage.Size = new System.Drawing.Size(104, 23);
      this.btnLanguage.TabIndex = 16;
      this.btnLanguage.Text = "Load Language";
      this.btnLanguage.UseVisualStyleBackColor = true;
      this.btnLanguage.Click += new System.EventHandler(this.btnLanguage_Click);
      // 
      // openFileDialog2
      // 
      this.openFileDialog2.Filter = "files|*.lng";
      // 
      // lblHint
      // 
      this.lblHint.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblHint.Location = new System.Drawing.Point(307, 140);
      this.lblHint.Name = "lblHint";
      this.lblHint.Size = new System.Drawing.Size(138, 103);
      this.lblHint.TabIndex = 18;
      this.lblHint.Text = "Hint: Use the context menu to switch effects, enable/disable the LEDs or switch 3" +
    "D-SBS mode.";
      // 
      // AtmolightSetupForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(452, 602);
      this.Controls.Add(this.lblHint);
      this.Controls.Add(this.btnLanguage);
      this.Controls.Add(this.btnCancel);
      this.Controls.Add(this.lblVersionVal);
      this.Controls.Add(this.lblVersion);
      this.Controls.Add(this.grpPluginOption);
      this.Controls.Add(this.grpDeactivate);
      this.Controls.Add(this.grpMode);
      this.Controls.Add(this.grpMPClose);
      this.Controls.Add(this.btnSave);
      this.Controls.Add(this.btnSelectFile);
      this.Controls.Add(this.edFile);
      this.Controls.Add(this.lblPathInfo);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MaximumSize = new System.Drawing.Size(468, 640);
      this.MinimumSize = new System.Drawing.Size(468, 640);
      this.Name = "AtmolightSetupForm";
      this.Text = "AtmoLight Setup";
      this.grpMPClose.ResumeLayout(false);
      this.grpMPClose.PerformLayout();
      this.grpMode.ResumeLayout(false);
      this.grpMode.PerformLayout();
      this.grpStaticColor.ResumeLayout(false);
      this.grpStaticColor.PerformLayout();
      this.grpDeactivate.ResumeLayout(false);
      this.grpDeactivate.PerformLayout();
      this.grpPluginOption.ResumeLayout(false);
      this.grpPluginOption.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label lblPathInfo;
    private System.Windows.Forms.TextBox edFile;
    private System.Windows.Forms.Button btnSelectFile;
    private System.Windows.Forms.Button btnSave;
    private System.Windows.Forms.Button btnCancel;
    private System.Windows.Forms.OpenFileDialog openFileDialog1;
    private System.Windows.Forms.GroupBox grpMPClose;
    private System.Windows.Forms.RadioButton rbDisableLEDs;
    private System.Windows.Forms.RadioButton rbSwitchToLiveView;
    private System.Windows.Forms.GroupBox grpMode;
    private System.Windows.Forms.ComboBox cbRadio;
    private System.Windows.Forms.Label lblRadio;
    private System.Windows.Forms.ComboBox cbMusic;
    private System.Windows.Forms.Label lblMusic;
    private System.Windows.Forms.ComboBox cbVideo;
    private System.Windows.Forms.Label lblVidTvRec;
    private System.Windows.Forms.GroupBox grpDeactivate;
    private System.Windows.Forms.TextBox edExcludeEnd;
    private System.Windows.Forms.Label lblEnd;
    private System.Windows.Forms.TextBox edExcludeStart;
    private System.Windows.Forms.Label lblStart;
    private System.Windows.Forms.GroupBox grpPluginOption;
    private System.Windows.Forms.ComboBox comboBox1;
    private System.Windows.Forms.Label lblLedsOnOff;
    private System.Windows.Forms.CheckBox ckOnMediaStart;
    private System.Windows.Forms.CheckBox ckLowCpu;
    private System.Windows.Forms.Label lblFrames;
    private System.Windows.Forms.TextBox lowCpuTime;
    private System.Windows.Forms.ComboBox comboBox2;
    private System.Windows.Forms.Label lblProfile;
    private System.Windows.Forms.CheckBox ckExitAtmoWin;
    private System.Windows.Forms.CheckBox ckStartAtmoWin;
    private System.Windows.Forms.Label lblVersion;
    private System.Windows.Forms.Label lblVersionVal;
    private System.Windows.Forms.Button btnLanguage;
    private System.Windows.Forms.OpenFileDialog openFileDialog2;
    private System.Windows.Forms.Label lblMenu;
    private System.Windows.Forms.ComboBox cbMenu;
    private System.Windows.Forms.GroupBox grpStaticColor;
    private System.Windows.Forms.Label lblRed;
    private System.Windows.Forms.TextBox tbBlue;
    private System.Windows.Forms.TextBox tbGreen;
    private System.Windows.Forms.TextBox tbRed;
    private System.Windows.Forms.Label lblBlue;
    private System.Windows.Forms.Label lblGreen;
    private System.Windows.Forms.Label lblMenuButton;
    private System.Windows.Forms.ComboBox cbMenuButton;
    private System.Windows.Forms.Label lblDelay;
    private System.Windows.Forms.TextBox tbDelay;
    private System.Windows.Forms.CheckBox ckDelay;
    private System.Windows.Forms.CheckBox ckRestartOnError;
    private System.Windows.Forms.Label lblRefreshRate;
    private System.Windows.Forms.TextBox tbRefreshRate;
    private System.Windows.Forms.Label lblHint;
  }
}