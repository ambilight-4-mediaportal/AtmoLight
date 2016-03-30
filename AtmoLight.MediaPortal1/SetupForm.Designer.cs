namespace AtmoLight
{
  partial class SetupForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetupForm));
      this.btnSave = new System.Windows.Forms.Button();
      this.btnCancel = new System.Windows.Forms.Button();
      this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
      this.lblVersion = new System.Windows.Forms.Label();
      this.lblVersionVal = new System.Windows.Forms.Label();
      this.btnLanguage = new System.Windows.Forms.Button();
      this.openFileDialog2 = new System.Windows.Forms.OpenFileDialog();
      this.openFileDialog3 = new System.Windows.Forms.OpenFileDialog();
      this.tabPageHyperion = new System.Windows.Forms.TabPage();
      this.grpHyperionPrioritySettings = new System.Windows.Forms.GroupBox();
      this.lblHyperionPriorityStaticColor = new System.Windows.Forms.Label();
      this.tbHyperionPriorityStaticColor = new System.Windows.Forms.TextBox();
      this.lblHyperionPriority = new System.Windows.Forms.Label();
      this.tbHyperionPriority = new System.Windows.Forms.TextBox();
      this.grpHyperionNetworkSettings = new System.Windows.Forms.GroupBox();
      this.ckHyperionLiveReconnect = new System.Windows.Forms.CheckBox();
      this.tbHyperionReconnectAttempts = new System.Windows.Forms.TextBox();
      this.lblHyperionReconnectAttempts = new System.Windows.Forms.Label();
      this.tbHyperionReconnectDelay = new System.Windows.Forms.TextBox();
      this.lblHyperionReconnectDelay = new System.Windows.Forms.Label();
      this.tbHyperionIP = new System.Windows.Forms.TextBox();
      this.lblHyperionIP = new System.Windows.Forms.Label();
      this.tbHyperionPort = new System.Windows.Forms.TextBox();
      this.lblHyperionPort = new System.Windows.Forms.Label();
      this.tabPageAtmowin = new System.Windows.Forms.TabPage();
      this.grpAtmowinSettings = new System.Windows.Forms.GroupBox();
      this.lblPathInfoAtmoWin = new System.Windows.Forms.Label();
      this.btnSelectFileAtmoWin = new System.Windows.Forms.Button();
      this.edFileAtmoWin = new System.Windows.Forms.TextBox();
      this.ckExitAtmoWin = new System.Windows.Forms.CheckBox();
      this.ckStartAtmoWin = new System.Windows.Forms.CheckBox();
      this.grpAtmowinWakeHelper = new System.Windows.Forms.GroupBox();
      this.lblAtmoWakeHelperReinitializationDelay = new System.Windows.Forms.Label();
      this.tbAtmoWakeHelperReinitializationDelay = new System.Windows.Forms.TextBox();
      this.lblAtmoWakeHelperConnectDelay = new System.Windows.Forms.Label();
      this.tbAtmoWakeHelperConnectDelay = new System.Windows.Forms.TextBox();
      this.lblAtmoWakeHelperDisconnectDelay = new System.Windows.Forms.Label();
      this.tbAtmoWakeHelperDisconnectDelay = new System.Windows.Forms.TextBox();
      this.lblAtmoWakeHelperResumeDelay = new System.Windows.Forms.Label();
      this.tbAtmoWakeHelperResumeDelay = new System.Windows.Forms.TextBox();
      this.lblAtmoWakeHelperComPort = new System.Windows.Forms.Label();
      this.cbAtmoWakeHelperComPort = new System.Windows.Forms.ComboBox();
      this.ckAtmoWakeHelperEnabled = new System.Windows.Forms.CheckBox();
      this.tabPageGeneric = new System.Windows.Forms.TabPage();
      this.grpTargets = new System.Windows.Forms.GroupBox();
      this.ckAtmoOrbEnabled = new System.Windows.Forms.CheckBox();
      this.ckAmbiBoxEnabled = new System.Windows.Forms.CheckBox();
      this.ckBoblightEnabled = new System.Windows.Forms.CheckBox();
      this.ckHueEnabled = new System.Windows.Forms.CheckBox();
      this.lblHintHardware = new System.Windows.Forms.Label();
      this.ckAtmowinEnabled = new System.Windows.Forms.CheckBox();
      this.ckHyperionEnabled = new System.Windows.Forms.CheckBox();
      this.grpMode = new System.Windows.Forms.GroupBox();
      this.grpAdvancedOptions = new System.Windows.Forms.GroupBox();
      this.cbRemoteApiServer = new System.Windows.Forms.CheckBox();
      this.ckTrueGrabbing = new System.Windows.Forms.CheckBox();
      this.grpVUMeter = new System.Windows.Forms.GroupBox();
      this.tbVUMeterMinHue = new System.Windows.Forms.TextBox();
      this.tbVUMeterMaxHue = new System.Windows.Forms.TextBox();
      this.lblVUMeterMaxHue = new System.Windows.Forms.Label();
      this.lblVUMeterMinHue = new System.Windows.Forms.Label();
      this.tbVUMeterMindB = new System.Windows.Forms.TextBox();
      this.lblVUMeterMindB = new System.Windows.Forms.Label();
      this.cbMPExit = new System.Windows.Forms.ComboBox();
      this.lblMPExit = new System.Windows.Forms.Label();
      this.grpGIF = new System.Windows.Forms.GroupBox();
      this.btnSelectGIF = new System.Windows.Forms.Button();
      this.tbGIF = new System.Windows.Forms.TextBox();
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
      this.grpPluginOption = new System.Windows.Forms.GroupBox();
      this.ckMonitorWindowState = new System.Windows.Forms.CheckBox();
      this.cbBlackbarDetectionVertical = new System.Windows.Forms.CheckBox();
      this.cbBlackbarDetectionHorizontal = new System.Windows.Forms.CheckBox();
      this.cbBlackbarDetectionLinkAreas = new System.Windows.Forms.CheckBox();
      this.ckMonitorScreensaverState = new System.Windows.Forms.CheckBox();
      this.lblpowerModeChangedDelayMS = new System.Windows.Forms.Label();
      this.lblpowerModeChangedDelay = new System.Windows.Forms.Label();
      this.tbBlackbarDetectionThreshold = new System.Windows.Forms.TextBox();
      this.tbpowerModeChangedDelay = new System.Windows.Forms.TextBox();
      this.ckRestartOnError = new System.Windows.Forms.CheckBox();
      this.grpCaptureDimensions = new System.Windows.Forms.GroupBox();
      this.lblHintCaptureDimensions = new System.Windows.Forms.Label();
      this.lblCaptureHeight = new System.Windows.Forms.Label();
      this.lblCaptureWidth = new System.Windows.Forms.Label();
      this.tbCaptureWidth = new System.Windows.Forms.TextBox();
      this.tbCaptureHeight = new System.Windows.Forms.TextBox();
      this.lblBlackarDetectionMS = new System.Windows.Forms.Label();
      this.grpDeactivate = new System.Windows.Forms.GroupBox();
      this.edExcludeEnd = new System.Windows.Forms.TextBox();
      this.lblEnd = new System.Windows.Forms.Label();
      this.edExcludeStart = new System.Windows.Forms.TextBox();
      this.lblStart = new System.Windows.Forms.Label();
      this.tbBlackbarDetectionTime = new System.Windows.Forms.TextBox();
      this.lblHintMenuButtons = new System.Windows.Forms.Label();
      this.ckBlackbarDetection = new System.Windows.Forms.CheckBox();
      this.lblRefreshRate = new System.Windows.Forms.Label();
      this.tbRefreshRate = new System.Windows.Forms.TextBox();
      this.lblDelay = new System.Windows.Forms.Label();
      this.tbDelay = new System.Windows.Forms.TextBox();
      this.ckDelay = new System.Windows.Forms.CheckBox();
      this.lblMenuButton = new System.Windows.Forms.Label();
      this.cbMenuButton = new System.Windows.Forms.ComboBox();
      this.lblProfile = new System.Windows.Forms.Label();
      this.comboBox2 = new System.Windows.Forms.ComboBox();
      this.lblFrames = new System.Windows.Forms.Label();
      this.lowCpuTime = new System.Windows.Forms.TextBox();
      this.ckLowCpu = new System.Windows.Forms.CheckBox();
      this.ckOnMediaStart = new System.Windows.Forms.CheckBox();
      this.comboBox1 = new System.Windows.Forms.ComboBox();
      this.lblLedsOnOff = new System.Windows.Forms.Label();
      this.tabMenu = new System.Windows.Forms.TabControl();
      this.tabPageAmbiBox = new System.Windows.Forms.TabPage();
      this.grpAmbiBoxNetwork = new System.Windows.Forms.GroupBox();
      this.tbAmbiboxChangeImageDelay = new System.Windows.Forms.TextBox();
      this.lblAmbiboxChangeImageDelay = new System.Windows.Forms.Label();
      this.tbAmbiBoxExternalProfile = new System.Windows.Forms.TextBox();
      this.tbAmbiBoxMediaPortalProfile = new System.Windows.Forms.TextBox();
      this.tbAmbiBoxReconnectDelay = new System.Windows.Forms.TextBox();
      this.tbAmbiBoxMaxReconnectAttempts = new System.Windows.Forms.TextBox();
      this.tbAmbiBoxPort = new System.Windows.Forms.TextBox();
      this.tbAmbiBoxIP = new System.Windows.Forms.TextBox();
      this.lblAmbiBoxExternalProfile = new System.Windows.Forms.Label();
      this.lblAmbiBoxMediaPortalProfile = new System.Windows.Forms.Label();
      this.lblAmbiBoxReconnectDelay = new System.Windows.Forms.Label();
      this.lblAmbiBoxMaxReconnectAttempts = new System.Windows.Forms.Label();
      this.lblAmbiBoxPort = new System.Windows.Forms.Label();
      this.lblAmbiBoxIP = new System.Windows.Forms.Label();
      this.grpAmbiBoxLocal = new System.Windows.Forms.GroupBox();
      this.cbAmbiBoxAutoStop = new System.Windows.Forms.CheckBox();
      this.cbAmbiBoxAutoStart = new System.Windows.Forms.CheckBox();
      this.lblAmbiBoxPath = new System.Windows.Forms.Label();
      this.tbAmbiBoxPath = new System.Windows.Forms.TextBox();
      this.btnSelectFileAmbiBox = new System.Windows.Forms.Button();
      this.tabPageAtmoOrb = new System.Windows.Forms.TabPage();
      this.grpAtmoOrbLamps = new System.Windows.Forms.GroupBox();
      this.tbAtmoOrbLedCount = new System.Windows.Forms.TextBox();
      this.lblAtmoOrbLedCount = new System.Windows.Forms.Label();
      this.lblAtmoOrbProtocol = new System.Windows.Forms.Label();
      this.cbAtmoOrbProtocol = new System.Windows.Forms.ComboBox();
      this.lblAtmoOrbVScanTo = new System.Windows.Forms.Label();
      this.lblAtmoOrbHScanTo = new System.Windows.Forms.Label();
      this.lblAtmoOrbConnection = new System.Windows.Forms.Label();
      this.cbAtmoOrbInvertZone = new System.Windows.Forms.CheckBox();
      this.tbAtmoOrbVScanEnd = new System.Windows.Forms.TextBox();
      this.tbAtmoOrbVScanStart = new System.Windows.Forms.TextBox();
      this.lblAtmoOrbVScan = new System.Windows.Forms.Label();
      this.tbAtmoOrbHScanEnd = new System.Windows.Forms.TextBox();
      this.tbAtmoOrbHScanStart = new System.Windows.Forms.TextBox();
      this.lblAtmoOrbHScan = new System.Windows.Forms.Label();
      this.tbAtmoOrbPort = new System.Windows.Forms.TextBox();
      this.lblAtmoOrbPort = new System.Windows.Forms.Label();
      this.tbAtmoOrbIP = new System.Windows.Forms.TextBox();
      this.lblAtmoOrbIP = new System.Windows.Forms.Label();
      this.rbAtmoOrbUDP = new System.Windows.Forms.RadioButton();
      this.rbAtmoOrbTCP = new System.Windows.Forms.RadioButton();
      this.tbAtmoOrbID = new System.Windows.Forms.TextBox();
      this.lblAtmoOrbID = new System.Windows.Forms.Label();
      this.btnAtmoOrbRemove = new System.Windows.Forms.Button();
      this.btnAtmoOrbUpdate = new System.Windows.Forms.Button();
      this.btnAtmoOrbAdd = new System.Windows.Forms.Button();
      this.lbAtmoOrbLamps = new System.Windows.Forms.ListBox();
      this.grpAtmoOrbBasicSettings = new System.Windows.Forms.GroupBox();
      this.cbAtmoOrbUseSmoothing = new System.Windows.Forms.CheckBox();
      this.tbAtmoOrbGamma = new System.Windows.Forms.TextBox();
      this.tbAtmoOrbSaturation = new System.Windows.Forms.TextBox();
      this.lblAtmoOrbGamma = new System.Windows.Forms.Label();
      this.lblAtmoOrbSaturation = new System.Windows.Forms.Label();
      this.lblAtmoOrbBlackThreshold = new System.Windows.Forms.Label();
      this.lblAtmoOrbThreshold = new System.Windows.Forms.Label();
      this.lblAtmoOrbMinDiversion = new System.Windows.Forms.Label();
      this.lblAtmoOrbBroadcastPort = new System.Windows.Forms.Label();
      this.tbAtmoOrbBlackThreshold = new System.Windows.Forms.TextBox();
      this.tbAtmoOrbThreshold = new System.Windows.Forms.TextBox();
      this.tbAtmoOrbMinDiversion = new System.Windows.Forms.TextBox();
      this.tbAtmoOrbBroadcastPort = new System.Windows.Forms.TextBox();
      this.cbAtmoOrbUseOverallLightness = new System.Windows.Forms.CheckBox();
      this.tabPageBoblight = new System.Windows.Forms.TabPage();
      this.grpBoblightSettings = new System.Windows.Forms.GroupBox();
      this.tbBoblightGamma = new System.Windows.Forms.TextBox();
      this.tbarBoblightGamma = new System.Windows.Forms.TrackBar();
      this.lblBoblightGamma = new System.Windows.Forms.Label();
      this.tbBoblightThreshold = new System.Windows.Forms.TextBox();
      this.tbBoblightValue = new System.Windows.Forms.TextBox();
      this.tbBoblightSaturation = new System.Windows.Forms.TextBox();
      this.tbBoblightAutospeed = new System.Windows.Forms.TextBox();
      this.tbBoblightSpeed = new System.Windows.Forms.TextBox();
      this.ckBoblightInterpolation = new System.Windows.Forms.CheckBox();
      this.lblBoblightThreshold = new System.Windows.Forms.Label();
      this.lblBoblightValue = new System.Windows.Forms.Label();
      this.lblBoblightSaturation = new System.Windows.Forms.Label();
      this.lblBoblightAutospeed = new System.Windows.Forms.Label();
      this.lblBoblightSpeed = new System.Windows.Forms.Label();
      this.tbarBoblightThreshold = new System.Windows.Forms.TrackBar();
      this.tbarBoblightValue = new System.Windows.Forms.TrackBar();
      this.tbarBoblightSaturation = new System.Windows.Forms.TrackBar();
      this.tbarBoblightAutospeed = new System.Windows.Forms.TrackBar();
      this.tbarBoblightSpeed = new System.Windows.Forms.TrackBar();
      this.grpBoblightGeneral = new System.Windows.Forms.GroupBox();
      this.tbBoblightMaxFPS = new System.Windows.Forms.TextBox();
      this.tbBoblightReconnectDelay = new System.Windows.Forms.TextBox();
      this.tbBoblightMaxReconnectAttempts = new System.Windows.Forms.TextBox();
      this.tbBoblightPort = new System.Windows.Forms.TextBox();
      this.tbBoblightIP = new System.Windows.Forms.TextBox();
      this.lblBoblightMaxFPS = new System.Windows.Forms.Label();
      this.lblBoblightReconnectDelay = new System.Windows.Forms.Label();
      this.lblBoblightMaxReconnectAttempts = new System.Windows.Forms.Label();
      this.lblBoblightPort = new System.Windows.Forms.Label();
      this.lblBoblightIP = new System.Windows.Forms.Label();
      this.tabPageHue = new System.Windows.Forms.TabPage();
      this.grpHueTheaterMode = new System.Windows.Forms.GroupBox();
      this.lblHintHueTheaterMode = new System.Windows.Forms.Label();
      this.ckHueTheaterRestoreLights = new System.Windows.Forms.CheckBox();
      this.ckHueTheaterEnabled = new System.Windows.Forms.CheckBox();
      this.grpHueAverageColor = new System.Windows.Forms.GroupBox();
      this.cbHueOverallLightness = new System.Windows.Forms.CheckBox();
      this.tbHueSaturation = new System.Windows.Forms.TextBox();
      this.lblHueSaturation = new System.Windows.Forms.Label();
      this.lblHueBlackThreshold = new System.Windows.Forms.Label();
      this.lblHueThreshold = new System.Windows.Forms.Label();
      this.lblHueMinDiversion = new System.Windows.Forms.Label();
      this.tbHueBlackThreshold = new System.Windows.Forms.TextBox();
      this.tbHueThreshold = new System.Windows.Forms.TextBox();
      this.tbHueMinDiversion = new System.Windows.Forms.TextBox();
      this.grpHueGeneralSettings = new System.Windows.Forms.GroupBox();
      this.ckHueBridgeDisableOnSuspend = new System.Windows.Forms.CheckBox();
      this.ckHueBridgeEnableOnResume = new System.Windows.Forms.CheckBox();
      this.ckhueIsRemoteMachine = new System.Windows.Forms.CheckBox();
      this.lblPathInfoHue = new System.Windows.Forms.Label();
      this.btnSelectFileHue = new System.Windows.Forms.Button();
      this.edFileHue = new System.Windows.Forms.TextBox();
      this.ckStartHue = new System.Windows.Forms.CheckBox();
      this.grpHueNetworkSettings = new System.Windows.Forms.GroupBox();
      this.tbHueReconnectAttempts = new System.Windows.Forms.TextBox();
      this.lblHueReconnectAttempts = new System.Windows.Forms.Label();
      this.tbHueReconnectDelay = new System.Windows.Forms.TextBox();
      this.lblHueReconnectDelay = new System.Windows.Forms.Label();
      this.lblHintHue = new System.Windows.Forms.Label();
      this.tbHuePort = new System.Windows.Forms.TextBox();
      this.lblHuePort = new System.Windows.Forms.Label();
      this.tbHueIP = new System.Windows.Forms.TextBox();
      this.lblHueIP = new System.Windows.Forms.Label();
      this.openFileDialog4 = new System.Windows.Forms.OpenFileDialog();
      this.openFileDialog5 = new System.Windows.Forms.OpenFileDialog();
      this.lblAtmoOrbSmoothingThreshold = new System.Windows.Forms.Label();
      this.tbAtmoOrbSmoothingThreshold = new System.Windows.Forms.TextBox();
      this.tabPageHyperion.SuspendLayout();
      this.grpHyperionPrioritySettings.SuspendLayout();
      this.grpHyperionNetworkSettings.SuspendLayout();
      this.tabPageAtmowin.SuspendLayout();
      this.grpAtmowinSettings.SuspendLayout();
      this.grpAtmowinWakeHelper.SuspendLayout();
      this.tabPageGeneric.SuspendLayout();
      this.grpTargets.SuspendLayout();
      this.grpMode.SuspendLayout();
      this.grpAdvancedOptions.SuspendLayout();
      this.grpVUMeter.SuspendLayout();
      this.grpGIF.SuspendLayout();
      this.grpStaticColor.SuspendLayout();
      this.grpPluginOption.SuspendLayout();
      this.grpCaptureDimensions.SuspendLayout();
      this.grpDeactivate.SuspendLayout();
      this.tabMenu.SuspendLayout();
      this.tabPageAmbiBox.SuspendLayout();
      this.grpAmbiBoxNetwork.SuspendLayout();
      this.grpAmbiBoxLocal.SuspendLayout();
      this.tabPageAtmoOrb.SuspendLayout();
      this.grpAtmoOrbLamps.SuspendLayout();
      this.grpAtmoOrbBasicSettings.SuspendLayout();
      this.tabPageBoblight.SuspendLayout();
      this.grpBoblightSettings.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.tbarBoblightGamma)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.tbarBoblightThreshold)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.tbarBoblightValue)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.tbarBoblightSaturation)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.tbarBoblightAutospeed)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.tbarBoblightSpeed)).BeginInit();
      this.grpBoblightGeneral.SuspendLayout();
      this.tabPageHue.SuspendLayout();
      this.grpHueTheaterMode.SuspendLayout();
      this.grpHueAverageColor.SuspendLayout();
      this.grpHueGeneralSettings.SuspendLayout();
      this.grpHueNetworkSettings.SuspendLayout();
      this.SuspendLayout();
      // 
      // btnSave
      // 
      this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.btnSave.Location = new System.Drawing.Point(16, 630);
      this.btnSave.Name = "btnSave";
      this.btnSave.Size = new System.Drawing.Size(105, 23);
      this.btnSave.TabIndex = 100;
      this.btnSave.Text = "Save";
      this.btnSave.UseVisualStyleBackColor = true;
      this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
      // 
      // btnCancel
      // 
      this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.btnCancel.Location = new System.Drawing.Point(141, 630);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new System.Drawing.Size(105, 23);
      this.btnCancel.TabIndex = 101;
      this.btnCancel.Text = "Cancel";
      this.btnCancel.UseVisualStyleBackColor = true;
      this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
      // 
      // openFileDialog1
      // 
      this.openFileDialog1.Filter = "AtmoWinA.exe|*.exe";
      this.openFileDialog1.RestoreDirectory = true;
      // 
      // lblVersion
      // 
      this.lblVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.lblVersion.Location = new System.Drawing.Point(754, 629);
      this.lblVersion.Name = "lblVersion";
      this.lblVersion.Size = new System.Drawing.Size(95, 26);
      this.lblVersion.TabIndex = 13;
      this.lblVersion.Text = "AtmoLight Plugin Version:";
      // 
      // lblVersionVal
      // 
      this.lblVersionVal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.lblVersionVal.AutoSize = true;
      this.lblVersionVal.Location = new System.Drawing.Point(799, 642);
      this.lblVersionVal.Name = "lblVersionVal";
      this.lblVersionVal.Size = new System.Drawing.Size(40, 13);
      this.lblVersionVal.TabIndex = 14;
      this.lblVersionVal.Text = "0.0.0.0";
      // 
      // btnLanguage
      // 
      this.btnLanguage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.btnLanguage.Location = new System.Drawing.Point(266, 630);
      this.btnLanguage.Name = "btnLanguage";
      this.btnLanguage.Size = new System.Drawing.Size(105, 23);
      this.btnLanguage.TabIndex = 102;
      this.btnLanguage.Text = "Load Language";
      this.btnLanguage.UseVisualStyleBackColor = true;
      this.btnLanguage.Click += new System.EventHandler(this.btnLanguage_Click);
      // 
      // openFileDialog2
      // 
      this.openFileDialog2.Filter = "files|*.xml";
      // 
      // openFileDialog3
      // 
      this.openFileDialog3.Filter = "files|*.gif";
      // 
      // tabPageHyperion
      // 
      this.tabPageHyperion.BackColor = System.Drawing.SystemColors.Control;
      this.tabPageHyperion.Controls.Add(this.grpHyperionPrioritySettings);
      this.tabPageHyperion.Controls.Add(this.grpHyperionNetworkSettings);
      this.tabPageHyperion.Location = new System.Drawing.Point(4, 22);
      this.tabPageHyperion.Name = "tabPageHyperion";
      this.tabPageHyperion.Size = new System.Drawing.Size(842, 584);
      this.tabPageHyperion.TabIndex = 2;
      this.tabPageHyperion.Text = "Hyperion";
      // 
      // grpHyperionPrioritySettings
      // 
      this.grpHyperionPrioritySettings.Controls.Add(this.lblHyperionPriorityStaticColor);
      this.grpHyperionPrioritySettings.Controls.Add(this.tbHyperionPriorityStaticColor);
      this.grpHyperionPrioritySettings.Controls.Add(this.lblHyperionPriority);
      this.grpHyperionPrioritySettings.Controls.Add(this.tbHyperionPriority);
      this.grpHyperionPrioritySettings.Location = new System.Drawing.Point(10, 175);
      this.grpHyperionPrioritySettings.Name = "grpHyperionPrioritySettings";
      this.grpHyperionPrioritySettings.Size = new System.Drawing.Size(820, 75);
      this.grpHyperionPrioritySettings.TabIndex = 10;
      this.grpHyperionPrioritySettings.TabStop = false;
      this.grpHyperionPrioritySettings.Text = "Priority";
      // 
      // lblHyperionPriorityStaticColor
      // 
      this.lblHyperionPriorityStaticColor.AutoSize = true;
      this.lblHyperionPriorityStaticColor.Location = new System.Drawing.Point(10, 50);
      this.lblHyperionPriorityStaticColor.Name = "lblHyperionPriorityStaticColor";
      this.lblHyperionPriorityStaticColor.Size = new System.Drawing.Size(95, 13);
      this.lblHyperionPriorityStaticColor.TabIndex = 8;
      this.lblHyperionPriorityStaticColor.Text = "Priority static color:";
      // 
      // tbHyperionPriorityStaticColor
      // 
      this.tbHyperionPriorityStaticColor.Location = new System.Drawing.Point(220, 47);
      this.tbHyperionPriorityStaticColor.Name = "tbHyperionPriorityStaticColor";
      this.tbHyperionPriorityStaticColor.Size = new System.Drawing.Size(93, 20);
      this.tbHyperionPriorityStaticColor.TabIndex = 6;
      this.tbHyperionPriorityStaticColor.Validating += new System.ComponentModel.CancelEventHandler(this.tbHyperionPriorityStaticColor_Validating);
      // 
      // lblHyperionPriority
      // 
      this.lblHyperionPriority.AutoSize = true;
      this.lblHyperionPriority.Location = new System.Drawing.Point(10, 25);
      this.lblHyperionPriority.Name = "lblHyperionPriority";
      this.lblHyperionPriority.Size = new System.Drawing.Size(60, 13);
      this.lblHyperionPriority.TabIndex = 5;
      this.lblHyperionPriority.Text = "Priority live:";
      // 
      // tbHyperionPriority
      // 
      this.tbHyperionPriority.Location = new System.Drawing.Point(220, 22);
      this.tbHyperionPriority.Name = "tbHyperionPriority";
      this.tbHyperionPriority.Size = new System.Drawing.Size(93, 20);
      this.tbHyperionPriority.TabIndex = 5;
      this.tbHyperionPriority.Validating += new System.ComponentModel.CancelEventHandler(this.tbHyperionPriority_Validating);
      // 
      // grpHyperionNetworkSettings
      // 
      this.grpHyperionNetworkSettings.Controls.Add(this.ckHyperionLiveReconnect);
      this.grpHyperionNetworkSettings.Controls.Add(this.tbHyperionReconnectAttempts);
      this.grpHyperionNetworkSettings.Controls.Add(this.lblHyperionReconnectAttempts);
      this.grpHyperionNetworkSettings.Controls.Add(this.tbHyperionReconnectDelay);
      this.grpHyperionNetworkSettings.Controls.Add(this.lblHyperionReconnectDelay);
      this.grpHyperionNetworkSettings.Controls.Add(this.tbHyperionIP);
      this.grpHyperionNetworkSettings.Controls.Add(this.lblHyperionIP);
      this.grpHyperionNetworkSettings.Controls.Add(this.tbHyperionPort);
      this.grpHyperionNetworkSettings.Controls.Add(this.lblHyperionPort);
      this.grpHyperionNetworkSettings.Location = new System.Drawing.Point(10, 10);
      this.grpHyperionNetworkSettings.Name = "grpHyperionNetworkSettings";
      this.grpHyperionNetworkSettings.Size = new System.Drawing.Size(820, 160);
      this.grpHyperionNetworkSettings.TabIndex = 7;
      this.grpHyperionNetworkSettings.TabStop = false;
      this.grpHyperionNetworkSettings.Text = "Network";
      // 
      // ckHyperionLiveReconnect
      // 
      this.ckHyperionLiveReconnect.Location = new System.Drawing.Point(12, 125);
      this.ckHyperionLiveReconnect.Name = "ckHyperionLiveReconnect";
      this.ckHyperionLiveReconnect.Size = new System.Drawing.Size(518, 30);
      this.ckHyperionLiveReconnect.TabIndex = 9;
      this.ckHyperionLiveReconnect.Text = "Live reconnect (NOT recommended)\r\nWill keep reconnecting until device is reachabl" +
    "e and only useful for live testing Hyperion config changes.";
      this.ckHyperionLiveReconnect.UseVisualStyleBackColor = true;
      // 
      // tbHyperionReconnectAttempts
      // 
      this.tbHyperionReconnectAttempts.Location = new System.Drawing.Point(220, 97);
      this.tbHyperionReconnectAttempts.Name = "tbHyperionReconnectAttempts";
      this.tbHyperionReconnectAttempts.Size = new System.Drawing.Size(93, 20);
      this.tbHyperionReconnectAttempts.TabIndex = 7;
      this.tbHyperionReconnectAttempts.Validating += new System.ComponentModel.CancelEventHandler(this.tbHyperionReconnectAttempts_Validating);
      // 
      // lblHyperionReconnectAttempts
      // 
      this.lblHyperionReconnectAttempts.AutoSize = true;
      this.lblHyperionReconnectAttempts.Location = new System.Drawing.Point(10, 100);
      this.lblHyperionReconnectAttempts.Name = "lblHyperionReconnectAttempts";
      this.lblHyperionReconnectAttempts.Size = new System.Drawing.Size(106, 13);
      this.lblHyperionReconnectAttempts.TabIndex = 8;
      this.lblHyperionReconnectAttempts.Text = "Reconnect attempts:";
      // 
      // tbHyperionReconnectDelay
      // 
      this.tbHyperionReconnectDelay.Location = new System.Drawing.Point(220, 72);
      this.tbHyperionReconnectDelay.Name = "tbHyperionReconnectDelay";
      this.tbHyperionReconnectDelay.Size = new System.Drawing.Size(93, 20);
      this.tbHyperionReconnectDelay.TabIndex = 5;
      this.tbHyperionReconnectDelay.Validating += new System.ComponentModel.CancelEventHandler(this.tbHyperionReconnectDelay_Validating);
      // 
      // lblHyperionReconnectDelay
      // 
      this.lblHyperionReconnectDelay.AutoSize = true;
      this.lblHyperionReconnectDelay.Location = new System.Drawing.Point(10, 75);
      this.lblHyperionReconnectDelay.Name = "lblHyperionReconnectDelay";
      this.lblHyperionReconnectDelay.Size = new System.Drawing.Size(113, 13);
      this.lblHyperionReconnectDelay.TabIndex = 6;
      this.lblHyperionReconnectDelay.Text = "Reconnect delay (ms):";
      // 
      // tbHyperionIP
      // 
      this.tbHyperionIP.Location = new System.Drawing.Point(220, 22);
      this.tbHyperionIP.Name = "tbHyperionIP";
      this.tbHyperionIP.Size = new System.Drawing.Size(93, 20);
      this.tbHyperionIP.TabIndex = 1;
      this.tbHyperionIP.Validating += new System.ComponentModel.CancelEventHandler(this.tbHyperionIP_Validating);
      // 
      // lblHyperionIP
      // 
      this.lblHyperionIP.AutoSize = true;
      this.lblHyperionIP.Location = new System.Drawing.Point(10, 25);
      this.lblHyperionIP.Name = "lblHyperionIP";
      this.lblHyperionIP.Size = new System.Drawing.Size(20, 13);
      this.lblHyperionIP.TabIndex = 3;
      this.lblHyperionIP.Text = "IP:";
      // 
      // tbHyperionPort
      // 
      this.tbHyperionPort.Location = new System.Drawing.Point(220, 47);
      this.tbHyperionPort.Name = "tbHyperionPort";
      this.tbHyperionPort.Size = new System.Drawing.Size(93, 20);
      this.tbHyperionPort.TabIndex = 2;
      this.tbHyperionPort.Validating += new System.ComponentModel.CancelEventHandler(this.tbHyperionPort_Validating);
      // 
      // lblHyperionPort
      // 
      this.lblHyperionPort.AutoSize = true;
      this.lblHyperionPort.Location = new System.Drawing.Point(10, 50);
      this.lblHyperionPort.Name = "lblHyperionPort";
      this.lblHyperionPort.Size = new System.Drawing.Size(29, 13);
      this.lblHyperionPort.TabIndex = 4;
      this.lblHyperionPort.Text = "Port:";
      // 
      // tabPageAtmowin
      // 
      this.tabPageAtmowin.BackColor = System.Drawing.SystemColors.Control;
      this.tabPageAtmowin.Controls.Add(this.grpAtmowinSettings);
      this.tabPageAtmowin.Controls.Add(this.grpAtmowinWakeHelper);
      this.tabPageAtmowin.Location = new System.Drawing.Point(4, 22);
      this.tabPageAtmowin.Name = "tabPageAtmowin";
      this.tabPageAtmowin.Padding = new System.Windows.Forms.Padding(3);
      this.tabPageAtmowin.Size = new System.Drawing.Size(842, 584);
      this.tabPageAtmowin.TabIndex = 1;
      this.tabPageAtmowin.Text = "Atmowin";
      // 
      // grpAtmowinSettings
      // 
      this.grpAtmowinSettings.Controls.Add(this.lblPathInfoAtmoWin);
      this.grpAtmowinSettings.Controls.Add(this.btnSelectFileAtmoWin);
      this.grpAtmowinSettings.Controls.Add(this.edFileAtmoWin);
      this.grpAtmowinSettings.Controls.Add(this.ckExitAtmoWin);
      this.grpAtmowinSettings.Controls.Add(this.ckStartAtmoWin);
      this.grpAtmowinSettings.Location = new System.Drawing.Point(10, 10);
      this.grpAtmowinSettings.Name = "grpAtmowinSettings";
      this.grpAtmowinSettings.Size = new System.Drawing.Size(820, 125);
      this.grpAtmowinSettings.TabIndex = 26;
      this.grpAtmowinSettings.TabStop = false;
      this.grpAtmowinSettings.Text = "Settings";
      // 
      // lblPathInfoAtmoWin
      // 
      this.lblPathInfoAtmoWin.AutoSize = true;
      this.lblPathInfoAtmoWin.Location = new System.Drawing.Point(10, 25);
      this.lblPathInfoAtmoWin.Name = "lblPathInfoAtmoWin";
      this.lblPathInfoAtmoWin.Size = new System.Drawing.Size(162, 13);
      this.lblPathInfoAtmoWin.TabIndex = 0;
      this.lblPathInfoAtmoWin.Text = "Path+Filename of AtmoWinA.exe";
      // 
      // btnSelectFileAtmoWin
      // 
      this.btnSelectFileAtmoWin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnSelectFileAtmoWin.Location = new System.Drawing.Point(725, 41);
      this.btnSelectFileAtmoWin.Name = "btnSelectFileAtmoWin";
      this.btnSelectFileAtmoWin.Size = new System.Drawing.Size(85, 23);
      this.btnSelectFileAtmoWin.TabIndex = 2;
      this.btnSelectFileAtmoWin.Text = "...";
      this.btnSelectFileAtmoWin.UseVisualStyleBackColor = true;
      this.btnSelectFileAtmoWin.Click += new System.EventHandler(this.btnSelectFile_Click);
      // 
      // edFileAtmoWin
      // 
      this.edFileAtmoWin.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.edFileAtmoWin.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
      this.edFileAtmoWin.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystem;
      this.edFileAtmoWin.Location = new System.Drawing.Point(12, 43);
      this.edFileAtmoWin.Name = "edFileAtmoWin";
      this.edFileAtmoWin.Size = new System.Drawing.Size(700, 20);
      this.edFileAtmoWin.TabIndex = 1;
      this.edFileAtmoWin.Validating += new System.ComponentModel.CancelEventHandler(this.edFile_Validating);
      // 
      // ckExitAtmoWin
      // 
      this.ckExitAtmoWin.AutoSize = true;
      this.ckExitAtmoWin.Checked = true;
      this.ckExitAtmoWin.CheckState = System.Windows.Forms.CheckState.Checked;
      this.ckExitAtmoWin.Location = new System.Drawing.Point(12, 100);
      this.ckExitAtmoWin.Name = "ckExitAtmoWin";
      this.ckExitAtmoWin.Size = new System.Drawing.Size(173, 17);
      this.ckExitAtmoWin.TabIndex = 4;
      this.ckExitAtmoWin.Text = "Exit AtmoWin with MediaPortal ";
      this.ckExitAtmoWin.UseVisualStyleBackColor = true;
      // 
      // ckStartAtmoWin
      // 
      this.ckStartAtmoWin.AutoSize = true;
      this.ckStartAtmoWin.Checked = true;
      this.ckStartAtmoWin.CheckState = System.Windows.Forms.CheckState.Checked;
      this.ckStartAtmoWin.Location = new System.Drawing.Point(12, 75);
      this.ckStartAtmoWin.Name = "ckStartAtmoWin";
      this.ckStartAtmoWin.Size = new System.Drawing.Size(175, 17);
      this.ckStartAtmoWin.TabIndex = 3;
      this.ckStartAtmoWin.Text = "Start AtmoWin with MediaPortal";
      this.ckStartAtmoWin.UseVisualStyleBackColor = true;
      // 
      // grpAtmowinWakeHelper
      // 
      this.grpAtmowinWakeHelper.Controls.Add(this.lblAtmoWakeHelperReinitializationDelay);
      this.grpAtmowinWakeHelper.Controls.Add(this.tbAtmoWakeHelperReinitializationDelay);
      this.grpAtmowinWakeHelper.Controls.Add(this.lblAtmoWakeHelperConnectDelay);
      this.grpAtmowinWakeHelper.Controls.Add(this.tbAtmoWakeHelperConnectDelay);
      this.grpAtmowinWakeHelper.Controls.Add(this.lblAtmoWakeHelperDisconnectDelay);
      this.grpAtmowinWakeHelper.Controls.Add(this.tbAtmoWakeHelperDisconnectDelay);
      this.grpAtmowinWakeHelper.Controls.Add(this.lblAtmoWakeHelperResumeDelay);
      this.grpAtmowinWakeHelper.Controls.Add(this.tbAtmoWakeHelperResumeDelay);
      this.grpAtmowinWakeHelper.Controls.Add(this.lblAtmoWakeHelperComPort);
      this.grpAtmowinWakeHelper.Controls.Add(this.cbAtmoWakeHelperComPort);
      this.grpAtmowinWakeHelper.Controls.Add(this.ckAtmoWakeHelperEnabled);
      this.grpAtmowinWakeHelper.Location = new System.Drawing.Point(10, 140);
      this.grpAtmowinWakeHelper.Name = "grpAtmowinWakeHelper";
      this.grpAtmowinWakeHelper.Size = new System.Drawing.Size(820, 180);
      this.grpAtmowinWakeHelper.TabIndex = 27;
      this.grpAtmowinWakeHelper.TabStop = false;
      this.grpAtmowinWakeHelper.Text = "COM port wake helper (optional)";
      // 
      // lblAtmoWakeHelperReinitializationDelay
      // 
      this.lblAtmoWakeHelperReinitializationDelay.AutoSize = true;
      this.lblAtmoWakeHelperReinitializationDelay.Location = new System.Drawing.Point(10, 155);
      this.lblAtmoWakeHelperReinitializationDelay.Name = "lblAtmoWakeHelperReinitializationDelay";
      this.lblAtmoWakeHelperReinitializationDelay.Size = new System.Drawing.Size(124, 13);
      this.lblAtmoWakeHelperReinitializationDelay.TabIndex = 10;
      this.lblAtmoWakeHelperReinitializationDelay.Text = "Reinitialization delay (ms)";
      // 
      // tbAtmoWakeHelperReinitializationDelay
      // 
      this.tbAtmoWakeHelperReinitializationDelay.Location = new System.Drawing.Point(200, 152);
      this.tbAtmoWakeHelperReinitializationDelay.Name = "tbAtmoWakeHelperReinitializationDelay";
      this.tbAtmoWakeHelperReinitializationDelay.Size = new System.Drawing.Size(70, 20);
      this.tbAtmoWakeHelperReinitializationDelay.TabIndex = 9;
      this.tbAtmoWakeHelperReinitializationDelay.Validating += new System.ComponentModel.CancelEventHandler(this.tbAtmoWakeHelperReinitializationDelay_Validating);
      // 
      // lblAtmoWakeHelperConnectDelay
      // 
      this.lblAtmoWakeHelperConnectDelay.AutoSize = true;
      this.lblAtmoWakeHelperConnectDelay.Location = new System.Drawing.Point(10, 130);
      this.lblAtmoWakeHelperConnectDelay.Name = "lblAtmoWakeHelperConnectDelay";
      this.lblAtmoWakeHelperConnectDelay.Size = new System.Drawing.Size(97, 13);
      this.lblAtmoWakeHelperConnectDelay.TabIndex = 8;
      this.lblAtmoWakeHelperConnectDelay.Text = "Connect delay (ms)";
      // 
      // tbAtmoWakeHelperConnectDelay
      // 
      this.tbAtmoWakeHelperConnectDelay.Location = new System.Drawing.Point(200, 127);
      this.tbAtmoWakeHelperConnectDelay.Name = "tbAtmoWakeHelperConnectDelay";
      this.tbAtmoWakeHelperConnectDelay.Size = new System.Drawing.Size(70, 20);
      this.tbAtmoWakeHelperConnectDelay.TabIndex = 7;
      this.tbAtmoWakeHelperConnectDelay.Validating += new System.ComponentModel.CancelEventHandler(this.tbAtmoWakeHelperConnectDelay_Validating);
      // 
      // lblAtmoWakeHelperDisconnectDelay
      // 
      this.lblAtmoWakeHelperDisconnectDelay.AutoSize = true;
      this.lblAtmoWakeHelperDisconnectDelay.Location = new System.Drawing.Point(10, 105);
      this.lblAtmoWakeHelperDisconnectDelay.Name = "lblAtmoWakeHelperDisconnectDelay";
      this.lblAtmoWakeHelperDisconnectDelay.Size = new System.Drawing.Size(111, 13);
      this.lblAtmoWakeHelperDisconnectDelay.TabIndex = 6;
      this.lblAtmoWakeHelperDisconnectDelay.Text = "Disconnect delay (ms)";
      // 
      // tbAtmoWakeHelperDisconnectDelay
      // 
      this.tbAtmoWakeHelperDisconnectDelay.Location = new System.Drawing.Point(200, 102);
      this.tbAtmoWakeHelperDisconnectDelay.Name = "tbAtmoWakeHelperDisconnectDelay";
      this.tbAtmoWakeHelperDisconnectDelay.Size = new System.Drawing.Size(70, 20);
      this.tbAtmoWakeHelperDisconnectDelay.TabIndex = 5;
      this.tbAtmoWakeHelperDisconnectDelay.Validating += new System.ComponentModel.CancelEventHandler(this.tbAtmoWakeHelperDisconnectDelay_Validating);
      // 
      // lblAtmoWakeHelperResumeDelay
      // 
      this.lblAtmoWakeHelperResumeDelay.AutoSize = true;
      this.lblAtmoWakeHelperResumeDelay.Location = new System.Drawing.Point(10, 80);
      this.lblAtmoWakeHelperResumeDelay.Name = "lblAtmoWakeHelperResumeDelay";
      this.lblAtmoWakeHelperResumeDelay.Size = new System.Drawing.Size(96, 13);
      this.lblAtmoWakeHelperResumeDelay.TabIndex = 4;
      this.lblAtmoWakeHelperResumeDelay.Text = "Resume delay (ms)";
      // 
      // tbAtmoWakeHelperResumeDelay
      // 
      this.tbAtmoWakeHelperResumeDelay.Location = new System.Drawing.Point(200, 77);
      this.tbAtmoWakeHelperResumeDelay.Name = "tbAtmoWakeHelperResumeDelay";
      this.tbAtmoWakeHelperResumeDelay.Size = new System.Drawing.Size(70, 20);
      this.tbAtmoWakeHelperResumeDelay.TabIndex = 3;
      this.tbAtmoWakeHelperResumeDelay.Validating += new System.ComponentModel.CancelEventHandler(this.tbAtmoWakeHelperResumeDelay_Validating);
      // 
      // lblAtmoWakeHelperComPort
      // 
      this.lblAtmoWakeHelperComPort.AutoSize = true;
      this.lblAtmoWakeHelperComPort.Location = new System.Drawing.Point(10, 55);
      this.lblAtmoWakeHelperComPort.Name = "lblAtmoWakeHelperComPort";
      this.lblAtmoWakeHelperComPort.Size = new System.Drawing.Size(98, 13);
      this.lblAtmoWakeHelperComPort.TabIndex = 2;
      this.lblAtmoWakeHelperComPort.Text = "AtmoWin COM port";
      // 
      // cbAtmoWakeHelperComPort
      // 
      this.cbAtmoWakeHelperComPort.FormattingEnabled = true;
      this.cbAtmoWakeHelperComPort.Items.AddRange(new object[] {
            "COM1",
            "COM2",
            "COM3",
            "COM4",
            "COM5",
            "COM6",
            "COM7",
            "COM8",
            "COM9",
            "COM10",
            "COM11",
            "COM12",
            "COM13",
            "COM14",
            "COM15",
            "COM16",
            "COM17",
            "COM18",
            "COM19",
            "COM20",
            "COM21",
            "COM22",
            "COM23",
            "COM24",
            "COM25",
            "COM26",
            "COM27",
            "COM28",
            "COM29",
            "COM30",
            "COM31",
            "COM32",
            "COM33",
            "COM34",
            "COM35",
            "COM36",
            "COM37",
            "COM38",
            "COM39",
            "COM40"});
      this.cbAtmoWakeHelperComPort.Location = new System.Drawing.Point(200, 52);
      this.cbAtmoWakeHelperComPort.Name = "cbAtmoWakeHelperComPort";
      this.cbAtmoWakeHelperComPort.Size = new System.Drawing.Size(70, 21);
      this.cbAtmoWakeHelperComPort.TabIndex = 1;
      // 
      // ckAtmoWakeHelperEnabled
      // 
      this.ckAtmoWakeHelperEnabled.AutoSize = true;
      this.ckAtmoWakeHelperEnabled.Location = new System.Drawing.Point(12, 25);
      this.ckAtmoWakeHelperEnabled.Name = "ckAtmoWakeHelperEnabled";
      this.ckAtmoWakeHelperEnabled.Size = new System.Drawing.Size(65, 17);
      this.ckAtmoWakeHelperEnabled.TabIndex = 0;
      this.ckAtmoWakeHelperEnabled.Text = "Enabled";
      this.ckAtmoWakeHelperEnabled.UseVisualStyleBackColor = true;
      // 
      // tabPageGeneric
      // 
      this.tabPageGeneric.BackColor = System.Drawing.SystemColors.Control;
      this.tabPageGeneric.Controls.Add(this.grpTargets);
      this.tabPageGeneric.Controls.Add(this.grpMode);
      this.tabPageGeneric.Controls.Add(this.grpPluginOption);
      this.tabPageGeneric.Location = new System.Drawing.Point(4, 22);
      this.tabPageGeneric.Name = "tabPageGeneric";
      this.tabPageGeneric.Padding = new System.Windows.Forms.Padding(3);
      this.tabPageGeneric.Size = new System.Drawing.Size(842, 584);
      this.tabPageGeneric.TabIndex = 0;
      this.tabPageGeneric.Text = "Generic settings";
      // 
      // grpTargets
      // 
      this.grpTargets.Controls.Add(this.ckAtmoOrbEnabled);
      this.grpTargets.Controls.Add(this.ckAmbiBoxEnabled);
      this.grpTargets.Controls.Add(this.ckBoblightEnabled);
      this.grpTargets.Controls.Add(this.ckHueEnabled);
      this.grpTargets.Controls.Add(this.lblHintHardware);
      this.grpTargets.Controls.Add(this.ckAtmowinEnabled);
      this.grpTargets.Controls.Add(this.ckHyperionEnabled);
      this.grpTargets.Location = new System.Drawing.Point(10, 10);
      this.grpTargets.Name = "grpTargets";
      this.grpTargets.Size = new System.Drawing.Size(350, 103);
      this.grpTargets.TabIndex = 19;
      this.grpTargets.TabStop = false;
      this.grpTargets.Text = "Hardware selection";
      // 
      // ckAtmoOrbEnabled
      // 
      this.ckAtmoOrbEnabled.AutoSize = true;
      this.ckAtmoOrbEnabled.Location = new System.Drawing.Point(12, 50);
      this.ckAtmoOrbEnabled.Name = "ckAtmoOrbEnabled";
      this.ckAtmoOrbEnabled.Size = new System.Drawing.Size(67, 17);
      this.ckAtmoOrbEnabled.TabIndex = 24;
      this.ckAtmoOrbEnabled.Text = "AtmoOrb";
      this.ckAtmoOrbEnabled.UseVisualStyleBackColor = true;
      this.ckAtmoOrbEnabled.CheckedChanged += new System.EventHandler(this.ckAtmoOrbEnabled_CheckedChanged);
      // 
      // ckAmbiBoxEnabled
      // 
      this.ckAmbiBoxEnabled.AutoSize = true;
      this.ckAmbiBoxEnabled.Location = new System.Drawing.Point(12, 25);
      this.ckAmbiBoxEnabled.Name = "ckAmbiBoxEnabled";
      this.ckAmbiBoxEnabled.Size = new System.Drawing.Size(67, 17);
      this.ckAmbiBoxEnabled.TabIndex = 23;
      this.ckAmbiBoxEnabled.Text = "AmbiBox";
      this.ckAmbiBoxEnabled.UseVisualStyleBackColor = true;
      this.ckAmbiBoxEnabled.CheckedChanged += new System.EventHandler(this.ckAmbiBoxEnabled_CheckedChanged);
      // 
      // ckBoblightEnabled
      // 
      this.ckBoblightEnabled.AutoSize = true;
      this.ckBoblightEnabled.Location = new System.Drawing.Point(94, 25);
      this.ckBoblightEnabled.Name = "ckBoblightEnabled";
      this.ckBoblightEnabled.Size = new System.Drawing.Size(64, 17);
      this.ckBoblightEnabled.TabIndex = 22;
      this.ckBoblightEnabled.Text = "Boblight";
      this.ckBoblightEnabled.UseVisualStyleBackColor = true;
      this.ckBoblightEnabled.CheckedChanged += new System.EventHandler(this.ckBoblightEnabled_CheckedChanged);
      // 
      // ckHueEnabled
      // 
      this.ckHueEnabled.AutoSize = true;
      this.ckHueEnabled.Location = new System.Drawing.Point(94, 50);
      this.ckHueEnabled.Name = "ckHueEnabled";
      this.ckHueEnabled.Size = new System.Drawing.Size(46, 17);
      this.ckHueEnabled.TabIndex = 21;
      this.ckHueEnabled.Text = "Hue";
      this.ckHueEnabled.UseVisualStyleBackColor = true;
      this.ckHueEnabled.CheckedChanged += new System.EventHandler(this.ckHueEnabled_CheckedChanged);
      // 
      // lblHintHardware
      // 
      this.lblHintHardware.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblHintHardware.Location = new System.Drawing.Point(176, 25);
      this.lblHintHardware.Name = "lblHintHardware";
      this.lblHintHardware.Size = new System.Drawing.Size(164, 67);
      this.lblHintHardware.TabIndex = 20;
      this.lblHintHardware.Text = "Hint: Select the devices you want to use with AtmoLight and configure it on its d" +
    "esignated tab";
      // 
      // ckAtmowinEnabled
      // 
      this.ckAtmowinEnabled.AutoSize = true;
      this.ckAtmowinEnabled.Checked = true;
      this.ckAtmowinEnabled.CheckState = System.Windows.Forms.CheckState.Checked;
      this.ckAtmowinEnabled.Location = new System.Drawing.Point(12, 75);
      this.ckAtmowinEnabled.Name = "ckAtmowinEnabled";
      this.ckAtmowinEnabled.Size = new System.Drawing.Size(69, 17);
      this.ckAtmowinEnabled.TabIndex = 1;
      this.ckAtmowinEnabled.Text = "AtmoWin";
      this.ckAtmowinEnabled.UseVisualStyleBackColor = true;
      this.ckAtmowinEnabled.CheckedChanged += new System.EventHandler(this.ckAtmowinEnabled_CheckedChanged);
      // 
      // ckHyperionEnabled
      // 
      this.ckHyperionEnabled.AutoSize = true;
      this.ckHyperionEnabled.Location = new System.Drawing.Point(94, 75);
      this.ckHyperionEnabled.Name = "ckHyperionEnabled";
      this.ckHyperionEnabled.Size = new System.Drawing.Size(68, 17);
      this.ckHyperionEnabled.TabIndex = 2;
      this.ckHyperionEnabled.Text = "Hyperion";
      this.ckHyperionEnabled.UseVisualStyleBackColor = true;
      this.ckHyperionEnabled.CheckedChanged += new System.EventHandler(this.ckHyperionEnabled_CheckedChanged);
      // 
      // grpMode
      // 
      this.grpMode.Controls.Add(this.grpAdvancedOptions);
      this.grpMode.Controls.Add(this.ckTrueGrabbing);
      this.grpMode.Controls.Add(this.grpVUMeter);
      this.grpMode.Controls.Add(this.cbMPExit);
      this.grpMode.Controls.Add(this.lblMPExit);
      this.grpMode.Controls.Add(this.grpGIF);
      this.grpMode.Controls.Add(this.grpStaticColor);
      this.grpMode.Controls.Add(this.lblMenu);
      this.grpMode.Controls.Add(this.cbMenu);
      this.grpMode.Controls.Add(this.cbRadio);
      this.grpMode.Controls.Add(this.lblRadio);
      this.grpMode.Controls.Add(this.cbMusic);
      this.grpMode.Controls.Add(this.lblMusic);
      this.grpMode.Controls.Add(this.cbVideo);
      this.grpMode.Controls.Add(this.lblVidTvRec);
      this.grpMode.Location = new System.Drawing.Point(10, 119);
      this.grpMode.Name = "grpMode";
      this.grpMode.Size = new System.Drawing.Size(350, 456);
      this.grpMode.TabIndex = 7;
      this.grpMode.TabStop = false;
      this.grpMode.Text = "Effect Settings";
      // 
      // grpAdvancedOptions
      // 
      this.grpAdvancedOptions.Controls.Add(this.cbRemoteApiServer);
      this.grpAdvancedOptions.Location = new System.Drawing.Point(4, 350);
      this.grpAdvancedOptions.Name = "grpAdvancedOptions";
      this.grpAdvancedOptions.Size = new System.Drawing.Size(340, 50);
      this.grpAdvancedOptions.TabIndex = 34;
      this.grpAdvancedOptions.TabStop = false;
      this.grpAdvancedOptions.Text = "Advanced options";
      // 
      // cbRemoteApiServer
      // 
      this.cbRemoteApiServer.AutoSize = true;
      this.cbRemoteApiServer.Location = new System.Drawing.Point(12, 25);
      this.cbRemoteApiServer.Name = "cbRemoteApiServer";
      this.cbRemoteApiServer.Size = new System.Drawing.Size(146, 17);
      this.cbRemoteApiServer.TabIndex = 0;
      this.cbRemoteApiServer.Text = "Enable remote API server";
      this.cbRemoteApiServer.UseVisualStyleBackColor = true;
      // 
      // ckTrueGrabbing
      // 
      this.ckTrueGrabbing.Checked = true;
      this.ckTrueGrabbing.CheckState = System.Windows.Forms.CheckState.Checked;
      this.ckTrueGrabbing.Location = new System.Drawing.Point(12, 150);
      this.ckTrueGrabbing.Name = "ckTrueGrabbing";
      this.ckTrueGrabbing.Size = new System.Drawing.Size(329, 17);
      this.ckTrueGrabbing.TabIndex = 33;
      this.ckTrueGrabbing.Text = "Use GUI data when video is minimized in MediaPortal Live Mode";
      this.ckTrueGrabbing.UseVisualStyleBackColor = true;
      // 
      // grpVUMeter
      // 
      this.grpVUMeter.Controls.Add(this.tbVUMeterMinHue);
      this.grpVUMeter.Controls.Add(this.tbVUMeterMaxHue);
      this.grpVUMeter.Controls.Add(this.lblVUMeterMaxHue);
      this.grpVUMeter.Controls.Add(this.lblVUMeterMinHue);
      this.grpVUMeter.Controls.Add(this.tbVUMeterMindB);
      this.grpVUMeter.Controls.Add(this.lblVUMeterMindB);
      this.grpVUMeter.Location = new System.Drawing.Point(4, 295);
      this.grpVUMeter.Name = "grpVUMeter";
      this.grpVUMeter.Size = new System.Drawing.Size(340, 50);
      this.grpVUMeter.TabIndex = 29;
      this.grpVUMeter.TabStop = false;
      this.grpVUMeter.Text = "VUMeter";
      // 
      // tbVUMeterMinHue
      // 
      this.tbVUMeterMinHue.Location = new System.Drawing.Point(162, 22);
      this.tbVUMeterMinHue.Name = "tbVUMeterMinHue";
      this.tbVUMeterMinHue.Size = new System.Drawing.Size(45, 20);
      this.tbVUMeterMinHue.TabIndex = 5;
      this.tbVUMeterMinHue.Validating += new System.ComponentModel.CancelEventHandler(this.tbVUMeterMinHue_Validating);
      // 
      // tbVUMeterMaxHue
      // 
      this.tbVUMeterMaxHue.Location = new System.Drawing.Point(277, 22);
      this.tbVUMeterMaxHue.Name = "tbVUMeterMaxHue";
      this.tbVUMeterMaxHue.Size = new System.Drawing.Size(45, 20);
      this.tbVUMeterMaxHue.TabIndex = 4;
      this.tbVUMeterMaxHue.Validating += new System.ComponentModel.CancelEventHandler(this.tbVUMeterMaxHue_Validating);
      // 
      // lblVUMeterMaxHue
      // 
      this.lblVUMeterMaxHue.AutoSize = true;
      this.lblVUMeterMaxHue.Location = new System.Drawing.Point(219, 25);
      this.lblVUMeterMaxHue.Name = "lblVUMeterMaxHue";
      this.lblVUMeterMaxHue.Size = new System.Drawing.Size(53, 13);
      this.lblVUMeterMaxHue.TabIndex = 3;
      this.lblVUMeterMaxHue.Text = "Max Hue:";
      // 
      // lblVUMeterMinHue
      // 
      this.lblVUMeterMinHue.AutoSize = true;
      this.lblVUMeterMinHue.Location = new System.Drawing.Point(104, 25);
      this.lblVUMeterMinHue.Name = "lblVUMeterMinHue";
      this.lblVUMeterMinHue.Size = new System.Drawing.Size(53, 13);
      this.lblVUMeterMinHue.TabIndex = 2;
      this.lblVUMeterMinHue.Text = "Min. Hue:";
      // 
      // tbVUMeterMindB
      // 
      this.tbVUMeterMindB.Location = new System.Drawing.Point(61, 22);
      this.tbVUMeterMindB.Name = "tbVUMeterMindB";
      this.tbVUMeterMindB.Size = new System.Drawing.Size(30, 20);
      this.tbVUMeterMindB.TabIndex = 1;
      this.tbVUMeterMindB.Validating += new System.ComponentModel.CancelEventHandler(this.tbVUMeterMindB_Validating);
      // 
      // lblVUMeterMindB
      // 
      this.lblVUMeterMindB.AutoSize = true;
      this.lblVUMeterMindB.Location = new System.Drawing.Point(10, 25);
      this.lblVUMeterMindB.Name = "lblVUMeterMindB";
      this.lblVUMeterMindB.Size = new System.Drawing.Size(46, 13);
      this.lblVUMeterMindB.TabIndex = 0;
      this.lblVUMeterMindB.Text = "Min. dB:";
      // 
      // cbMPExit
      // 
      this.cbMPExit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cbMPExit.FormattingEnabled = true;
      this.cbMPExit.Items.AddRange(new object[] {
            "LEDs disabled",
            "External Live Mode",
            "Colorchanger",
            "Colorchanger LR",
            "Static Color"});
      this.cbMPExit.Location = new System.Drawing.Point(175, 121);
      this.cbMPExit.Name = "cbMPExit";
      this.cbMPExit.Size = new System.Drawing.Size(160, 21);
      this.cbMPExit.TabIndex = 28;
      this.cbMPExit.SelectedIndexChanged += new System.EventHandler(this.cbMPExit_SelectedIndexChanged);
      // 
      // lblMPExit
      // 
      this.lblMPExit.AutoSize = true;
      this.lblMPExit.Location = new System.Drawing.Point(10, 125);
      this.lblMPExit.Name = "lblMPExit";
      this.lblMPExit.Size = new System.Drawing.Size(86, 13);
      this.lblMPExit.TabIndex = 27;
      this.lblMPExit.Text = "MediaPortal Exit:";
      // 
      // grpGIF
      // 
      this.grpGIF.Controls.Add(this.btnSelectGIF);
      this.grpGIF.Controls.Add(this.tbGIF);
      this.grpGIF.Location = new System.Drawing.Point(4, 240);
      this.grpGIF.Name = "grpGIF";
      this.grpGIF.Size = new System.Drawing.Size(340, 50);
      this.grpGIF.TabIndex = 26;
      this.grpGIF.TabStop = false;
      this.grpGIF.Text = "GIF Reader";
      // 
      // btnSelectGIF
      // 
      this.btnSelectGIF.Location = new System.Drawing.Point(280, 17);
      this.btnSelectGIF.Name = "btnSelectGIF";
      this.btnSelectGIF.Size = new System.Drawing.Size(50, 23);
      this.btnSelectGIF.TabIndex = 24;
      this.btnSelectGIF.Text = "...";
      this.btnSelectGIF.UseVisualStyleBackColor = true;
      this.btnSelectGIF.Click += new System.EventHandler(this.btnSelectGIF_Click);
      // 
      // tbGIF
      // 
      this.tbGIF.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
      this.tbGIF.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystem;
      this.tbGIF.Location = new System.Drawing.Point(12, 19);
      this.tbGIF.Name = "tbGIF";
      this.tbGIF.Size = new System.Drawing.Size(260, 20);
      this.tbGIF.TabIndex = 23;
      this.tbGIF.Validating += new System.ComponentModel.CancelEventHandler(this.tbGIF_Validating);
      // 
      // grpStaticColor
      // 
      this.grpStaticColor.Controls.Add(this.lblRed);
      this.grpStaticColor.Controls.Add(this.tbGreen);
      this.grpStaticColor.Controls.Add(this.tbBlue);
      this.grpStaticColor.Controls.Add(this.tbRed);
      this.grpStaticColor.Controls.Add(this.lblGreen);
      this.grpStaticColor.Controls.Add(this.lblBlue);
      this.grpStaticColor.Location = new System.Drawing.Point(4, 185);
      this.grpStaticColor.Name = "grpStaticColor";
      this.grpStaticColor.Size = new System.Drawing.Size(340, 50);
      this.grpStaticColor.TabIndex = 25;
      this.grpStaticColor.TabStop = false;
      this.grpStaticColor.Text = "Static Color";
      // 
      // lblRed
      // 
      this.lblRed.AutoSize = true;
      this.lblRed.Location = new System.Drawing.Point(10, 25);
      this.lblRed.Name = "lblRed";
      this.lblRed.Size = new System.Drawing.Size(30, 13);
      this.lblRed.TabIndex = 19;
      this.lblRed.Text = "Red:";
      // 
      // tbGreen
      // 
      this.tbGreen.Location = new System.Drawing.Point(171, 22);
      this.tbGreen.Name = "tbGreen";
      this.tbGreen.Size = new System.Drawing.Size(40, 20);
      this.tbGreen.TabIndex = 21;
      this.tbGreen.Text = "0";
      this.tbGreen.Validating += new System.ComponentModel.CancelEventHandler(this.tbGreen_Validating);
      // 
      // tbBlue
      // 
      this.tbBlue.Location = new System.Drawing.Point(282, 22);
      this.tbBlue.Name = "tbBlue";
      this.tbBlue.Size = new System.Drawing.Size(40, 20);
      this.tbBlue.TabIndex = 22;
      this.tbBlue.Text = "0";
      this.tbBlue.Validating += new System.ComponentModel.CancelEventHandler(this.tbBlue_Validating);
      // 
      // tbRed
      // 
      this.tbRed.Location = new System.Drawing.Point(55, 22);
      this.tbRed.Name = "tbRed";
      this.tbRed.Size = new System.Drawing.Size(40, 20);
      this.tbRed.TabIndex = 20;
      this.tbRed.Text = "0";
      this.tbRed.Validating += new System.ComponentModel.CancelEventHandler(this.tbRed_Validating);
      // 
      // lblGreen
      // 
      this.lblGreen.AutoSize = true;
      this.lblGreen.Location = new System.Drawing.Point(121, 25);
      this.lblGreen.Name = "lblGreen";
      this.lblGreen.Size = new System.Drawing.Size(39, 13);
      this.lblGreen.TabIndex = 20;
      this.lblGreen.Text = "Green:";
      // 
      // lblBlue
      // 
      this.lblBlue.AutoSize = true;
      this.lblBlue.Location = new System.Drawing.Point(240, 25);
      this.lblBlue.Name = "lblBlue";
      this.lblBlue.Size = new System.Drawing.Size(31, 13);
      this.lblBlue.TabIndex = 21;
      this.lblBlue.Text = "Blue:";
      // 
      // lblMenu
      // 
      this.lblMenu.AutoSize = true;
      this.lblMenu.Location = new System.Drawing.Point(10, 100);
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
            "External Live Mode",
            "Colorchanger",
            "Colorchanger LR",
            "MediaPortal Live Mode",
            "Static Color",
            "GIF Reader"});
      this.cbMenu.Location = new System.Drawing.Point(175, 96);
      this.cbMenu.Name = "cbMenu";
      this.cbMenu.Size = new System.Drawing.Size(160, 21);
      this.cbMenu.TabIndex = 19;
      this.cbMenu.SelectedIndexChanged += new System.EventHandler(this.cbMenu_SelectedIndexChanged);
      // 
      // cbRadio
      // 
      this.cbRadio.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cbRadio.FormattingEnabled = true;
      this.cbRadio.Items.AddRange(new object[] {
            "LEDs disabled",
            "External Live Mode",
            "Colorchanger",
            "Colorchanger LR",
            "MediaPortal Live Mode",
            "Static Color",
            "GIF Reader",
            "VU Meter",
            "VU Meter Rainbow"});
      this.cbRadio.Location = new System.Drawing.Point(175, 71);
      this.cbRadio.Name = "cbRadio";
      this.cbRadio.Size = new System.Drawing.Size(160, 21);
      this.cbRadio.TabIndex = 18;
      this.cbRadio.SelectedIndexChanged += new System.EventHandler(this.cbRadio_SelectedIndexChanged);
      // 
      // lblRadio
      // 
      this.lblRadio.AutoSize = true;
      this.lblRadio.Location = new System.Drawing.Point(10, 75);
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
            "External Live Mode",
            "Colorchanger",
            "Colorchanger LR",
            "MediaPortal Live Mode",
            "Static Color",
            "GIF Reader",
            "VU Meter",
            "VU Meter Rainbow"});
      this.cbMusic.Location = new System.Drawing.Point(175, 46);
      this.cbMusic.Name = "cbMusic";
      this.cbMusic.Size = new System.Drawing.Size(160, 21);
      this.cbMusic.TabIndex = 17;
      this.cbMusic.SelectedIndexChanged += new System.EventHandler(this.cbMusic_SelectedIndexChanged);
      // 
      // lblMusic
      // 
      this.lblMusic.AutoSize = true;
      this.lblMusic.Location = new System.Drawing.Point(10, 50);
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
            "External Live Mode",
            "Colorchanger",
            "Colorchanger LR",
            "MediaPortal Live Mode",
            "Static Color",
            "GIF Reader"});
      this.cbVideo.Location = new System.Drawing.Point(175, 21);
      this.cbVideo.Name = "cbVideo";
      this.cbVideo.Size = new System.Drawing.Size(160, 21);
      this.cbVideo.TabIndex = 16;
      this.cbVideo.SelectedIndexChanged += new System.EventHandler(this.cbVideo_SelectedIndexChanged);
      // 
      // lblVidTvRec
      // 
      this.lblVidTvRec.AutoSize = true;
      this.lblVidTvRec.Location = new System.Drawing.Point(10, 25);
      this.lblVidTvRec.Name = "lblVidTvRec";
      this.lblVidTvRec.Size = new System.Drawing.Size(115, 13);
      this.lblVidTvRec.TabIndex = 0;
      this.lblVidTvRec.Text = "Video/TV/Recordings:";
      // 
      // grpPluginOption
      // 
      this.grpPluginOption.Controls.Add(this.ckMonitorWindowState);
      this.grpPluginOption.Controls.Add(this.cbBlackbarDetectionVertical);
      this.grpPluginOption.Controls.Add(this.cbBlackbarDetectionHorizontal);
      this.grpPluginOption.Controls.Add(this.cbBlackbarDetectionLinkAreas);
      this.grpPluginOption.Controls.Add(this.ckMonitorScreensaverState);
      this.grpPluginOption.Controls.Add(this.lblpowerModeChangedDelayMS);
      this.grpPluginOption.Controls.Add(this.lblpowerModeChangedDelay);
      this.grpPluginOption.Controls.Add(this.tbBlackbarDetectionThreshold);
      this.grpPluginOption.Controls.Add(this.tbpowerModeChangedDelay);
      this.grpPluginOption.Controls.Add(this.ckRestartOnError);
      this.grpPluginOption.Controls.Add(this.grpCaptureDimensions);
      this.grpPluginOption.Controls.Add(this.lblBlackarDetectionMS);
      this.grpPluginOption.Controls.Add(this.grpDeactivate);
      this.grpPluginOption.Controls.Add(this.tbBlackbarDetectionTime);
      this.grpPluginOption.Controls.Add(this.lblHintMenuButtons);
      this.grpPluginOption.Controls.Add(this.ckBlackbarDetection);
      this.grpPluginOption.Controls.Add(this.lblRefreshRate);
      this.grpPluginOption.Controls.Add(this.tbRefreshRate);
      this.grpPluginOption.Controls.Add(this.lblDelay);
      this.grpPluginOption.Controls.Add(this.tbDelay);
      this.grpPluginOption.Controls.Add(this.ckDelay);
      this.grpPluginOption.Controls.Add(this.lblMenuButton);
      this.grpPluginOption.Controls.Add(this.cbMenuButton);
      this.grpPluginOption.Controls.Add(this.lblProfile);
      this.grpPluginOption.Controls.Add(this.comboBox2);
      this.grpPluginOption.Controls.Add(this.lblFrames);
      this.grpPluginOption.Controls.Add(this.lowCpuTime);
      this.grpPluginOption.Controls.Add(this.ckLowCpu);
      this.grpPluginOption.Controls.Add(this.ckOnMediaStart);
      this.grpPluginOption.Controls.Add(this.comboBox1);
      this.grpPluginOption.Controls.Add(this.lblLedsOnOff);
      this.grpPluginOption.Location = new System.Drawing.Point(370, 10);
      this.grpPluginOption.Name = "grpPluginOption";
      this.grpPluginOption.Size = new System.Drawing.Size(460, 565);
      this.grpPluginOption.TabIndex = 11;
      this.grpPluginOption.TabStop = false;
      this.grpPluginOption.Text = "Plugin options";
      // 
      // ckMonitorWindowState
      // 
      this.ckMonitorWindowState.AutoSize = true;
      this.ckMonitorWindowState.Location = new System.Drawing.Point(12, 325);
      this.ckMonitorWindowState.Name = "ckMonitorWindowState";
      this.ckMonitorWindowState.Size = new System.Drawing.Size(295, 17);
      this.ckMonitorWindowState.TabIndex = 42;
      this.ckMonitorWindowState.Text = "Turn off leds while Mediaportal is minimized or suspended";
      this.ckMonitorWindowState.UseVisualStyleBackColor = true;
      // 
      // cbBlackbarDetectionVertical
      // 
      this.cbBlackbarDetectionVertical.AutoSize = true;
      this.cbBlackbarDetectionVertical.Location = new System.Drawing.Point(12, 250);
      this.cbBlackbarDetectionVertical.Name = "cbBlackbarDetectionVertical";
      this.cbBlackbarDetectionVertical.Size = new System.Drawing.Size(180, 17);
      this.cbBlackbarDetectionVertical.TabIndex = 41;
      this.cbBlackbarDetectionVertical.Text = "Blackbar Detection vertical scan";
      this.cbBlackbarDetectionVertical.UseVisualStyleBackColor = true;
      // 
      // cbBlackbarDetectionHorizontal
      // 
      this.cbBlackbarDetectionHorizontal.AutoSize = true;
      this.cbBlackbarDetectionHorizontal.Location = new System.Drawing.Point(12, 225);
      this.cbBlackbarDetectionHorizontal.Name = "cbBlackbarDetectionHorizontal";
      this.cbBlackbarDetectionHorizontal.Size = new System.Drawing.Size(191, 17);
      this.cbBlackbarDetectionHorizontal.TabIndex = 40;
      this.cbBlackbarDetectionHorizontal.Text = "Blackbar Detection horizontal scan";
      this.cbBlackbarDetectionHorizontal.UseVisualStyleBackColor = true;
      // 
      // cbBlackbarDetectionLinkAreas
      // 
      this.cbBlackbarDetectionLinkAreas.AutoSize = true;
      this.cbBlackbarDetectionLinkAreas.Location = new System.Drawing.Point(12, 200);
      this.cbBlackbarDetectionLinkAreas.Name = "cbBlackbarDetectionLinkAreas";
      this.cbBlackbarDetectionLinkAreas.Size = new System.Drawing.Size(309, 17);
      this.cbBlackbarDetectionLinkAreas.TabIndex = 39;
      this.cbBlackbarDetectionLinkAreas.Text = "Blackbar Detection link areas (top to bottom and left to right)";
      this.cbBlackbarDetectionLinkAreas.UseVisualStyleBackColor = true;
      // 
      // ckMonitorScreensaverState
      // 
      this.ckMonitorScreensaverState.AutoSize = true;
      this.ckMonitorScreensaverState.Location = new System.Drawing.Point(12, 300);
      this.ckMonitorScreensaverState.Name = "ckMonitorScreensaverState";
      this.ckMonitorScreensaverState.Size = new System.Drawing.Size(273, 17);
      this.ckMonitorScreensaverState.TabIndex = 38;
      this.ckMonitorScreensaverState.Text = "Turn off leds while Mediaportal screensaver is active";
      this.ckMonitorScreensaverState.UseVisualStyleBackColor = true;
      // 
      // lblpowerModeChangedDelayMS
      // 
      this.lblpowerModeChangedDelayMS.AutoSize = true;
      this.lblpowerModeChangedDelayMS.Location = new System.Drawing.Point(255, 353);
      this.lblpowerModeChangedDelayMS.Name = "lblpowerModeChangedDelayMS";
      this.lblpowerModeChangedDelayMS.Size = new System.Drawing.Size(20, 13);
      this.lblpowerModeChangedDelayMS.TabIndex = 35;
      this.lblpowerModeChangedDelayMS.Text = "ms";
      // 
      // lblpowerModeChangedDelay
      // 
      this.lblpowerModeChangedDelay.AutoSize = true;
      this.lblpowerModeChangedDelay.Location = new System.Drawing.Point(9, 350);
      this.lblpowerModeChangedDelay.Name = "lblpowerModeChangedDelay";
      this.lblpowerModeChangedDelay.Size = new System.Drawing.Size(101, 13);
      this.lblpowerModeChangedDelay.TabIndex = 34;
      this.lblpowerModeChangedDelay.Text = "Delay after standby:";
      // 
      // tbBlackbarDetectionThreshold
      // 
      this.tbBlackbarDetectionThreshold.Location = new System.Drawing.Point(373, 172);
      this.tbBlackbarDetectionThreshold.Name = "tbBlackbarDetectionThreshold";
      this.tbBlackbarDetectionThreshold.Size = new System.Drawing.Size(41, 20);
      this.tbBlackbarDetectionThreshold.TabIndex = 33;
      this.tbBlackbarDetectionThreshold.Text = "20";
      this.tbBlackbarDetectionThreshold.Validating += new System.ComponentModel.CancelEventHandler(this.tbBlackbarDetectionThreshold_Validating);
      // 
      // tbpowerModeChangedDelay
      // 
      this.tbpowerModeChangedDelay.Location = new System.Drawing.Point(208, 350);
      this.tbpowerModeChangedDelay.Name = "tbpowerModeChangedDelay";
      this.tbpowerModeChangedDelay.Size = new System.Drawing.Size(41, 20);
      this.tbpowerModeChangedDelay.TabIndex = 33;
      this.tbpowerModeChangedDelay.Text = "0";
      this.tbpowerModeChangedDelay.Validating += new System.ComponentModel.CancelEventHandler(this.tbpowerModeChangedDelay_Validating);
      // 
      // ckRestartOnError
      // 
      this.ckRestartOnError.AutoSize = true;
      this.ckRestartOnError.Checked = true;
      this.ckRestartOnError.CheckState = System.Windows.Forms.CheckState.Checked;
      this.ckRestartOnError.Location = new System.Drawing.Point(12, 275);
      this.ckRestartOnError.Name = "ckRestartOnError";
      this.ckRestartOnError.Size = new System.Drawing.Size(144, 17);
      this.ckRestartOnError.TabIndex = 32;
      this.ckRestartOnError.Text = "Auto-Reconnect on Error";
      this.ckRestartOnError.UseVisualStyleBackColor = true;
      // 
      // grpCaptureDimensions
      // 
      this.grpCaptureDimensions.Controls.Add(this.lblHintCaptureDimensions);
      this.grpCaptureDimensions.Controls.Add(this.lblCaptureHeight);
      this.grpCaptureDimensions.Controls.Add(this.lblCaptureWidth);
      this.grpCaptureDimensions.Controls.Add(this.tbCaptureWidth);
      this.grpCaptureDimensions.Controls.Add(this.tbCaptureHeight);
      this.grpCaptureDimensions.Location = new System.Drawing.Point(5, 380);
      this.grpCaptureDimensions.Name = "grpCaptureDimensions";
      this.grpCaptureDimensions.Size = new System.Drawing.Size(450, 80);
      this.grpCaptureDimensions.TabIndex = 31;
      this.grpCaptureDimensions.TabStop = false;
      this.grpCaptureDimensions.Text = "Capture dimensions";
      // 
      // lblHintCaptureDimensions
      // 
      this.lblHintCaptureDimensions.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblHintCaptureDimensions.Location = new System.Drawing.Point(195, 25);
      this.lblHintCaptureDimensions.Name = "lblHintCaptureDimensions";
      this.lblHintCaptureDimensions.Size = new System.Drawing.Size(248, 38);
      this.lblHintCaptureDimensions.TabIndex = 32;
      this.lblHintCaptureDimensions.Text = "Hint: Only used when Atmowin is disabled";
      // 
      // lblCaptureHeight
      // 
      this.lblCaptureHeight.AutoSize = true;
      this.lblCaptureHeight.Location = new System.Drawing.Point(10, 50);
      this.lblCaptureHeight.Name = "lblCaptureHeight";
      this.lblCaptureHeight.Size = new System.Drawing.Size(41, 13);
      this.lblCaptureHeight.TabIndex = 10;
      this.lblCaptureHeight.Text = "Height:";
      // 
      // lblCaptureWidth
      // 
      this.lblCaptureWidth.AutoSize = true;
      this.lblCaptureWidth.Location = new System.Drawing.Point(10, 25);
      this.lblCaptureWidth.Name = "lblCaptureWidth";
      this.lblCaptureWidth.Size = new System.Drawing.Size(38, 13);
      this.lblCaptureWidth.TabIndex = 9;
      this.lblCaptureWidth.Text = "Width:";
      // 
      // tbCaptureWidth
      // 
      this.tbCaptureWidth.Location = new System.Drawing.Point(75, 22);
      this.tbCaptureWidth.Name = "tbCaptureWidth";
      this.tbCaptureWidth.Size = new System.Drawing.Size(50, 20);
      this.tbCaptureWidth.TabIndex = 3;
      this.tbCaptureWidth.Text = "64";
      this.tbCaptureWidth.Validating += new System.ComponentModel.CancelEventHandler(this.tbCaptureWidth_Validating);
      // 
      // tbCaptureHeight
      // 
      this.tbCaptureHeight.Location = new System.Drawing.Point(75, 47);
      this.tbCaptureHeight.Name = "tbCaptureHeight";
      this.tbCaptureHeight.Size = new System.Drawing.Size(50, 20);
      this.tbCaptureHeight.TabIndex = 4;
      this.tbCaptureHeight.Text = "64";
      this.tbCaptureHeight.Validating += new System.ComponentModel.CancelEventHandler(this.tbCaptureHeight_Validating);
      // 
      // lblBlackarDetectionMS
      // 
      this.lblBlackarDetectionMS.AutoSize = true;
      this.lblBlackarDetectionMS.Location = new System.Drawing.Point(256, 175);
      this.lblBlackarDetectionMS.Name = "lblBlackarDetectionMS";
      this.lblBlackarDetectionMS.Size = new System.Drawing.Size(95, 13);
      this.lblBlackarDetectionMS.TabIndex = 30;
      this.lblBlackarDetectionMS.Text = "ms, with Threshold";
      // 
      // grpDeactivate
      // 
      this.grpDeactivate.Controls.Add(this.edExcludeEnd);
      this.grpDeactivate.Controls.Add(this.lblEnd);
      this.grpDeactivate.Controls.Add(this.edExcludeStart);
      this.grpDeactivate.Controls.Add(this.lblStart);
      this.grpDeactivate.Location = new System.Drawing.Point(5, 465);
      this.grpDeactivate.Name = "grpDeactivate";
      this.grpDeactivate.Size = new System.Drawing.Size(450, 80);
      this.grpDeactivate.TabIndex = 8;
      this.grpDeactivate.TabStop = false;
      this.grpDeactivate.Text = "Disable LEDs between";
      // 
      // edExcludeEnd
      // 
      this.edExcludeEnd.Location = new System.Drawing.Point(75, 47);
      this.edExcludeEnd.Name = "edExcludeEnd";
      this.edExcludeEnd.Size = new System.Drawing.Size(50, 20);
      this.edExcludeEnd.TabIndex = 15;
      this.edExcludeEnd.Text = "21:00";
      this.edExcludeEnd.Validating += new System.ComponentModel.CancelEventHandler(this.edExcludeEnd_Validating);
      // 
      // lblEnd
      // 
      this.lblEnd.AutoSize = true;
      this.lblEnd.Location = new System.Drawing.Point(10, 50);
      this.lblEnd.Name = "lblEnd";
      this.lblEnd.Size = new System.Drawing.Size(29, 13);
      this.lblEnd.TabIndex = 2;
      this.lblEnd.Text = "End:";
      // 
      // edExcludeStart
      // 
      this.edExcludeStart.Location = new System.Drawing.Point(75, 22);
      this.edExcludeStart.Name = "edExcludeStart";
      this.edExcludeStart.Size = new System.Drawing.Size(50, 20);
      this.edExcludeStart.TabIndex = 14;
      this.edExcludeStart.Text = "8:00";
      this.edExcludeStart.Validating += new System.ComponentModel.CancelEventHandler(this.edExcludeStart_Validating);
      // 
      // lblStart
      // 
      this.lblStart.AutoSize = true;
      this.lblStart.Location = new System.Drawing.Point(10, 25);
      this.lblStart.Name = "lblStart";
      this.lblStart.Size = new System.Drawing.Size(32, 13);
      this.lblStart.TabIndex = 0;
      this.lblStart.Text = "Start:";
      // 
      // tbBlackbarDetectionTime
      // 
      this.tbBlackbarDetectionTime.Location = new System.Drawing.Point(209, 172);
      this.tbBlackbarDetectionTime.Name = "tbBlackbarDetectionTime";
      this.tbBlackbarDetectionTime.Size = new System.Drawing.Size(41, 20);
      this.tbBlackbarDetectionTime.TabIndex = 13;
      this.tbBlackbarDetectionTime.Text = "0";
      this.tbBlackbarDetectionTime.Validating += new System.ComponentModel.CancelEventHandler(this.tbBlackbarDetectionTime_Validating);
      // 
      // lblHintMenuButtons
      // 
      this.lblHintMenuButtons.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblHintMenuButtons.Location = new System.Drawing.Point(312, 25);
      this.lblHintMenuButtons.Name = "lblHintMenuButtons";
      this.lblHintMenuButtons.Size = new System.Drawing.Size(141, 92);
      this.lblHintMenuButtons.TabIndex = 18;
      this.lblHintMenuButtons.Text = "Hint: Use the context menu to switch effects, enable/disable the LEDs or switch 3" +
    "D-SBS mode.";
      // 
      // ckBlackbarDetection
      // 
      this.ckBlackbarDetection.AutoSize = true;
      this.ckBlackbarDetection.Location = new System.Drawing.Point(12, 175);
      this.ckBlackbarDetection.Name = "ckBlackbarDetection";
      this.ckBlackbarDetection.Size = new System.Drawing.Size(146, 17);
      this.ckBlackbarDetection.TabIndex = 12;
      this.ckBlackbarDetection.Text = "Blackbar Detection every";
      this.ckBlackbarDetection.UseVisualStyleBackColor = true;
      // 
      // lblRefreshRate
      // 
      this.lblRefreshRate.AutoSize = true;
      this.lblRefreshRate.Location = new System.Drawing.Point(415, 150);
      this.lblRefreshRate.Name = "lblRefreshRate";
      this.lblRefreshRate.Size = new System.Drawing.Size(20, 13);
      this.lblRefreshRate.TabIndex = 27;
      this.lblRefreshRate.Text = "Hz";
      // 
      // tbRefreshRate
      // 
      this.tbRefreshRate.Location = new System.Drawing.Point(373, 147);
      this.tbRefreshRate.Name = "tbRefreshRate";
      this.tbRefreshRate.Size = new System.Drawing.Size(41, 20);
      this.tbRefreshRate.TabIndex = 11;
      this.tbRefreshRate.Text = "50";
      this.tbRefreshRate.Validating += new System.ComponentModel.CancelEventHandler(this.tbRefreshRate_Validating);
      // 
      // lblDelay
      // 
      this.lblDelay.AutoSize = true;
      this.lblDelay.Location = new System.Drawing.Point(256, 150);
      this.lblDelay.Name = "lblDelay";
      this.lblDelay.Size = new System.Drawing.Size(62, 13);
      this.lblDelay.TabIndex = 24;
      this.lblDelay.Text = "ms Delay at";
      // 
      // tbDelay
      // 
      this.tbDelay.Location = new System.Drawing.Point(209, 147);
      this.tbDelay.Name = "tbDelay";
      this.tbDelay.Size = new System.Drawing.Size(41, 20);
      this.tbDelay.TabIndex = 10;
      this.tbDelay.Text = "0";
      this.tbDelay.Validating += new System.ComponentModel.CancelEventHandler(this.tbDelay_Validating);
      // 
      // ckDelay
      // 
      this.ckDelay.AutoSize = true;
      this.ckDelay.Location = new System.Drawing.Point(12, 150);
      this.ckDelay.Name = "ckDelay";
      this.ckDelay.Size = new System.Drawing.Size(77, 17);
      this.ckDelay.TabIndex = 9;
      this.ckDelay.Text = "LED Delay";
      this.ckDelay.UseVisualStyleBackColor = true;
      // 
      // lblMenuButton
      // 
      this.lblMenuButton.AutoSize = true;
      this.lblMenuButton.Location = new System.Drawing.Point(10, 25);
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
      this.cbMenuButton.Location = new System.Drawing.Point(210, 21);
      this.cbMenuButton.Name = "cbMenuButton";
      this.cbMenuButton.Size = new System.Drawing.Size(96, 21);
      this.cbMenuButton.TabIndex = 3;
      this.cbMenuButton.Validating += new System.ComponentModel.CancelEventHandler(this.cbMenuButton_Validating);
      // 
      // lblProfile
      // 
      this.lblProfile.AutoSize = true;
      this.lblProfile.Location = new System.Drawing.Point(10, 75);
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
      this.comboBox2.Location = new System.Drawing.Point(210, 71);
      this.comboBox2.Name = "comboBox2";
      this.comboBox2.Size = new System.Drawing.Size(96, 21);
      this.comboBox2.TabIndex = 5;
      this.comboBox2.Validating += new System.ComponentModel.CancelEventHandler(this.comboBox2_Validating);
      // 
      // lblFrames
      // 
      this.lblFrames.AutoSize = true;
      this.lblFrames.Location = new System.Drawing.Point(256, 125);
      this.lblFrames.Name = "lblFrames";
      this.lblFrames.Size = new System.Drawing.Size(101, 13);
      this.lblFrames.TabIndex = 15;
      this.lblFrames.Text = "ms between Frames";
      // 
      // lowCpuTime
      // 
      this.lowCpuTime.Location = new System.Drawing.Point(209, 122);
      this.lowCpuTime.MaxLength = 4;
      this.lowCpuTime.Name = "lowCpuTime";
      this.lowCpuTime.Size = new System.Drawing.Size(41, 20);
      this.lowCpuTime.TabIndex = 8;
      this.lowCpuTime.TabStop = false;
      this.lowCpuTime.Text = "0";
      this.lowCpuTime.Validating += new System.ComponentModel.CancelEventHandler(this.lowCpuTime_Validating);
      // 
      // ckLowCpu
      // 
      this.ckLowCpu.AutoSize = true;
      this.ckLowCpu.Location = new System.Drawing.Point(12, 125);
      this.ckLowCpu.Name = "ckLowCpu";
      this.ckLowCpu.Size = new System.Drawing.Size(71, 17);
      this.ckLowCpu.TabIndex = 7;
      this.ckLowCpu.Text = "Low CPU";
      this.ckLowCpu.UseVisualStyleBackColor = true;
      // 
      // ckOnMediaStart
      // 
      this.ckOnMediaStart.AutoSize = true;
      this.ckOnMediaStart.Location = new System.Drawing.Point(12, 100);
      this.ckOnMediaStart.Name = "ckOnMediaStart";
      this.ckOnMediaStart.Size = new System.Drawing.Size(208, 17);
      this.ckOnMediaStart.TabIndex = 6;
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
      this.comboBox1.Location = new System.Drawing.Point(210, 46);
      this.comboBox1.Name = "comboBox1";
      this.comboBox1.Size = new System.Drawing.Size(96, 21);
      this.comboBox1.TabIndex = 4;
      this.comboBox1.Validating += new System.ComponentModel.CancelEventHandler(this.comboBox1_Validating);
      // 
      // lblLedsOnOff
      // 
      this.lblLedsOnOff.AutoSize = true;
      this.lblLedsOnOff.Location = new System.Drawing.Point(10, 50);
      this.lblLedsOnOff.Name = "lblLedsOnOff";
      this.lblLedsOnOff.Size = new System.Drawing.Size(125, 13);
      this.lblLedsOnOff.TabIndex = 8;
      this.lblLedsOnOff.Text = "LEDs OnOff RemoteKey:";
      // 
      // tabMenu
      // 
      this.tabMenu.Controls.Add(this.tabPageGeneric);
      this.tabMenu.Controls.Add(this.tabPageAmbiBox);
      this.tabMenu.Controls.Add(this.tabPageAtmoOrb);
      this.tabMenu.Controls.Add(this.tabPageAtmowin);
      this.tabMenu.Controls.Add(this.tabPageBoblight);
      this.tabMenu.Controls.Add(this.tabPageHue);
      this.tabMenu.Controls.Add(this.tabPageHyperion);
      this.tabMenu.Location = new System.Drawing.Point(12, 12);
      this.tabMenu.Name = "tabMenu";
      this.tabMenu.SelectedIndex = 0;
      this.tabMenu.Size = new System.Drawing.Size(850, 610);
      this.tabMenu.TabIndex = 20;
      // 
      // tabPageAmbiBox
      // 
      this.tabPageAmbiBox.BackColor = System.Drawing.SystemColors.Control;
      this.tabPageAmbiBox.Controls.Add(this.grpAmbiBoxNetwork);
      this.tabPageAmbiBox.Controls.Add(this.grpAmbiBoxLocal);
      this.tabPageAmbiBox.Location = new System.Drawing.Point(4, 22);
      this.tabPageAmbiBox.Name = "tabPageAmbiBox";
      this.tabPageAmbiBox.Padding = new System.Windows.Forms.Padding(3);
      this.tabPageAmbiBox.Size = new System.Drawing.Size(842, 584);
      this.tabPageAmbiBox.TabIndex = 5;
      this.tabPageAmbiBox.Text = "AmbiBox";
      // 
      // grpAmbiBoxNetwork
      // 
      this.grpAmbiBoxNetwork.Controls.Add(this.tbAmbiboxChangeImageDelay);
      this.grpAmbiBoxNetwork.Controls.Add(this.lblAmbiboxChangeImageDelay);
      this.grpAmbiBoxNetwork.Controls.Add(this.tbAmbiBoxExternalProfile);
      this.grpAmbiBoxNetwork.Controls.Add(this.tbAmbiBoxMediaPortalProfile);
      this.grpAmbiBoxNetwork.Controls.Add(this.tbAmbiBoxReconnectDelay);
      this.grpAmbiBoxNetwork.Controls.Add(this.tbAmbiBoxMaxReconnectAttempts);
      this.grpAmbiBoxNetwork.Controls.Add(this.tbAmbiBoxPort);
      this.grpAmbiBoxNetwork.Controls.Add(this.tbAmbiBoxIP);
      this.grpAmbiBoxNetwork.Controls.Add(this.lblAmbiBoxExternalProfile);
      this.grpAmbiBoxNetwork.Controls.Add(this.lblAmbiBoxMediaPortalProfile);
      this.grpAmbiBoxNetwork.Controls.Add(this.lblAmbiBoxReconnectDelay);
      this.grpAmbiBoxNetwork.Controls.Add(this.lblAmbiBoxMaxReconnectAttempts);
      this.grpAmbiBoxNetwork.Controls.Add(this.lblAmbiBoxPort);
      this.grpAmbiBoxNetwork.Controls.Add(this.lblAmbiBoxIP);
      this.grpAmbiBoxNetwork.Location = new System.Drawing.Point(10, 140);
      this.grpAmbiBoxNetwork.Name = "grpAmbiBoxNetwork";
      this.grpAmbiBoxNetwork.Size = new System.Drawing.Size(820, 200);
      this.grpAmbiBoxNetwork.TabIndex = 1;
      this.grpAmbiBoxNetwork.TabStop = false;
      this.grpAmbiBoxNetwork.Text = "AmbiBox API";
      // 
      // tbAmbiboxChangeImageDelay
      // 
      this.tbAmbiboxChangeImageDelay.Location = new System.Drawing.Point(230, 122);
      this.tbAmbiboxChangeImageDelay.Name = "tbAmbiboxChangeImageDelay";
      this.tbAmbiboxChangeImageDelay.Size = new System.Drawing.Size(100, 20);
      this.tbAmbiboxChangeImageDelay.TabIndex = 15;
      // 
      // lblAmbiboxChangeImageDelay
      // 
      this.lblAmbiboxChangeImageDelay.AutoSize = true;
      this.lblAmbiboxChangeImageDelay.Location = new System.Drawing.Point(10, 125);
      this.lblAmbiboxChangeImageDelay.Name = "lblAmbiboxChangeImageDelay";
      this.lblAmbiboxChangeImageDelay.Size = new System.Drawing.Size(107, 13);
      this.lblAmbiboxChangeImageDelay.TabIndex = 14;
      this.lblAmbiboxChangeImageDelay.Text = "Change Image delay:";
      // 
      // tbAmbiBoxExternalProfile
      // 
      this.tbAmbiBoxExternalProfile.Location = new System.Drawing.Point(230, 172);
      this.tbAmbiBoxExternalProfile.Name = "tbAmbiBoxExternalProfile";
      this.tbAmbiBoxExternalProfile.Size = new System.Drawing.Size(100, 20);
      this.tbAmbiBoxExternalProfile.TabIndex = 13;
      // 
      // tbAmbiBoxMediaPortalProfile
      // 
      this.tbAmbiBoxMediaPortalProfile.Location = new System.Drawing.Point(230, 147);
      this.tbAmbiBoxMediaPortalProfile.Name = "tbAmbiBoxMediaPortalProfile";
      this.tbAmbiBoxMediaPortalProfile.Size = new System.Drawing.Size(100, 20);
      this.tbAmbiBoxMediaPortalProfile.TabIndex = 12;
      // 
      // tbAmbiBoxReconnectDelay
      // 
      this.tbAmbiBoxReconnectDelay.Location = new System.Drawing.Point(230, 97);
      this.tbAmbiBoxReconnectDelay.Name = "tbAmbiBoxReconnectDelay";
      this.tbAmbiBoxReconnectDelay.Size = new System.Drawing.Size(100, 20);
      this.tbAmbiBoxReconnectDelay.TabIndex = 10;
      this.tbAmbiBoxReconnectDelay.Validating += new System.ComponentModel.CancelEventHandler(this.tbAmbiBoxReconnectDelay_Validating);
      // 
      // tbAmbiBoxMaxReconnectAttempts
      // 
      this.tbAmbiBoxMaxReconnectAttempts.Location = new System.Drawing.Point(230, 72);
      this.tbAmbiBoxMaxReconnectAttempts.Name = "tbAmbiBoxMaxReconnectAttempts";
      this.tbAmbiBoxMaxReconnectAttempts.Size = new System.Drawing.Size(100, 20);
      this.tbAmbiBoxMaxReconnectAttempts.TabIndex = 9;
      this.tbAmbiBoxMaxReconnectAttempts.Validating += new System.ComponentModel.CancelEventHandler(this.tbAmbiBoxMaxReconnectAttempts_Validating);
      // 
      // tbAmbiBoxPort
      // 
      this.tbAmbiBoxPort.Location = new System.Drawing.Point(230, 47);
      this.tbAmbiBoxPort.Name = "tbAmbiBoxPort";
      this.tbAmbiBoxPort.Size = new System.Drawing.Size(100, 20);
      this.tbAmbiBoxPort.TabIndex = 8;
      this.tbAmbiBoxPort.Validating += new System.ComponentModel.CancelEventHandler(this.tbAmbiBoxPort_Validating);
      // 
      // tbAmbiBoxIP
      // 
      this.tbAmbiBoxIP.Location = new System.Drawing.Point(230, 22);
      this.tbAmbiBoxIP.Name = "tbAmbiBoxIP";
      this.tbAmbiBoxIP.Size = new System.Drawing.Size(100, 20);
      this.tbAmbiBoxIP.TabIndex = 7;
      this.tbAmbiBoxIP.Validating += new System.ComponentModel.CancelEventHandler(this.tbAmbiBoxIP_Validating);
      // 
      // lblAmbiBoxExternalProfile
      // 
      this.lblAmbiBoxExternalProfile.AutoSize = true;
      this.lblAmbiBoxExternalProfile.Location = new System.Drawing.Point(10, 175);
      this.lblAmbiBoxExternalProfile.Name = "lblAmbiBoxExternalProfile";
      this.lblAmbiBoxExternalProfile.Size = new System.Drawing.Size(118, 13);
      this.lblAmbiBoxExternalProfile.TabIndex = 6;
      this.lblAmbiBoxExternalProfile.Text = "External capture profile:";
      // 
      // lblAmbiBoxMediaPortalProfile
      // 
      this.lblAmbiBoxMediaPortalProfile.AutoSize = true;
      this.lblAmbiBoxMediaPortalProfile.Location = new System.Drawing.Point(10, 150);
      this.lblAmbiBoxMediaPortalProfile.Name = "lblAmbiBoxMediaPortalProfile";
      this.lblAmbiBoxMediaPortalProfile.Size = new System.Drawing.Size(136, 13);
      this.lblAmbiBoxMediaPortalProfile.TabIndex = 5;
      this.lblAmbiBoxMediaPortalProfile.Text = "MediaPortal capture profile:";
      // 
      // lblAmbiBoxReconnectDelay
      // 
      this.lblAmbiBoxReconnectDelay.AutoSize = true;
      this.lblAmbiBoxReconnectDelay.Location = new System.Drawing.Point(10, 100);
      this.lblAmbiBoxReconnectDelay.Name = "lblAmbiBoxReconnectDelay";
      this.lblAmbiBoxReconnectDelay.Size = new System.Drawing.Size(91, 13);
      this.lblAmbiBoxReconnectDelay.TabIndex = 3;
      this.lblAmbiBoxReconnectDelay.Text = "Reconnect delay:";
      // 
      // lblAmbiBoxMaxReconnectAttempts
      // 
      this.lblAmbiBoxMaxReconnectAttempts.AutoSize = true;
      this.lblAmbiBoxMaxReconnectAttempts.Location = new System.Drawing.Point(10, 75);
      this.lblAmbiBoxMaxReconnectAttempts.Name = "lblAmbiBoxMaxReconnectAttempts";
      this.lblAmbiBoxMaxReconnectAttempts.Size = new System.Drawing.Size(106, 13);
      this.lblAmbiBoxMaxReconnectAttempts.TabIndex = 2;
      this.lblAmbiBoxMaxReconnectAttempts.Text = "Reconnect attempts:";
      // 
      // lblAmbiBoxPort
      // 
      this.lblAmbiBoxPort.AutoSize = true;
      this.lblAmbiBoxPort.Location = new System.Drawing.Point(10, 50);
      this.lblAmbiBoxPort.Name = "lblAmbiBoxPort";
      this.lblAmbiBoxPort.Size = new System.Drawing.Size(29, 13);
      this.lblAmbiBoxPort.TabIndex = 1;
      this.lblAmbiBoxPort.Text = "Port:";
      // 
      // lblAmbiBoxIP
      // 
      this.lblAmbiBoxIP.AutoSize = true;
      this.lblAmbiBoxIP.Location = new System.Drawing.Point(10, 25);
      this.lblAmbiBoxIP.Name = "lblAmbiBoxIP";
      this.lblAmbiBoxIP.Size = new System.Drawing.Size(20, 13);
      this.lblAmbiBoxIP.TabIndex = 0;
      this.lblAmbiBoxIP.Text = "IP:";
      // 
      // grpAmbiBoxLocal
      // 
      this.grpAmbiBoxLocal.Controls.Add(this.cbAmbiBoxAutoStop);
      this.grpAmbiBoxLocal.Controls.Add(this.cbAmbiBoxAutoStart);
      this.grpAmbiBoxLocal.Controls.Add(this.lblAmbiBoxPath);
      this.grpAmbiBoxLocal.Controls.Add(this.tbAmbiBoxPath);
      this.grpAmbiBoxLocal.Controls.Add(this.btnSelectFileAmbiBox);
      this.grpAmbiBoxLocal.Location = new System.Drawing.Point(10, 10);
      this.grpAmbiBoxLocal.Name = "grpAmbiBoxLocal";
      this.grpAmbiBoxLocal.Size = new System.Drawing.Size(820, 125);
      this.grpAmbiBoxLocal.TabIndex = 0;
      this.grpAmbiBoxLocal.TabStop = false;
      this.grpAmbiBoxLocal.Text = "AmbiBox.exe";
      // 
      // cbAmbiBoxAutoStop
      // 
      this.cbAmbiBoxAutoStop.AutoSize = true;
      this.cbAmbiBoxAutoStop.Location = new System.Drawing.Point(12, 100);
      this.cbAmbiBoxAutoStop.Name = "cbAmbiBoxAutoStop";
      this.cbAmbiBoxAutoStop.Size = new System.Drawing.Size(173, 17);
      this.cbAmbiBoxAutoStop.TabIndex = 7;
      this.cbAmbiBoxAutoStop.Text = "Stop AmbiBox with MediaPortal";
      this.cbAmbiBoxAutoStop.UseVisualStyleBackColor = true;
      // 
      // cbAmbiBoxAutoStart
      // 
      this.cbAmbiBoxAutoStart.AutoSize = true;
      this.cbAmbiBoxAutoStart.Location = new System.Drawing.Point(12, 75);
      this.cbAmbiBoxAutoStart.Name = "cbAmbiBoxAutoStart";
      this.cbAmbiBoxAutoStart.Size = new System.Drawing.Size(173, 17);
      this.cbAmbiBoxAutoStart.TabIndex = 6;
      this.cbAmbiBoxAutoStart.Text = "Start AmbiBox with MediaPortal";
      this.cbAmbiBoxAutoStart.UseVisualStyleBackColor = true;
      // 
      // lblAmbiBoxPath
      // 
      this.lblAmbiBoxPath.AutoSize = true;
      this.lblAmbiBoxPath.Location = new System.Drawing.Point(10, 25);
      this.lblAmbiBoxPath.Name = "lblAmbiBoxPath";
      this.lblAmbiBoxPath.Size = new System.Drawing.Size(153, 13);
      this.lblAmbiBoxPath.TabIndex = 5;
      this.lblAmbiBoxPath.Text = "Path+Filename to AmbiBox.exe";
      // 
      // tbAmbiBoxPath
      // 
      this.tbAmbiBoxPath.Location = new System.Drawing.Point(12, 43);
      this.tbAmbiBoxPath.Name = "tbAmbiBoxPath";
      this.tbAmbiBoxPath.Size = new System.Drawing.Size(700, 20);
      this.tbAmbiBoxPath.TabIndex = 4;
      // 
      // btnSelectFileAmbiBox
      // 
      this.btnSelectFileAmbiBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnSelectFileAmbiBox.Location = new System.Drawing.Point(725, 41);
      this.btnSelectFileAmbiBox.Name = "btnSelectFileAmbiBox";
      this.btnSelectFileAmbiBox.Size = new System.Drawing.Size(85, 23);
      this.btnSelectFileAmbiBox.TabIndex = 3;
      this.btnSelectFileAmbiBox.Text = "...";
      this.btnSelectFileAmbiBox.UseVisualStyleBackColor = true;
      this.btnSelectFileAmbiBox.Click += new System.EventHandler(this.btnSelectFileAmbiBox_Click);
      // 
      // tabPageAtmoOrb
      // 
      this.tabPageAtmoOrb.BackColor = System.Drawing.SystemColors.Control;
      this.tabPageAtmoOrb.Controls.Add(this.grpAtmoOrbLamps);
      this.tabPageAtmoOrb.Controls.Add(this.grpAtmoOrbBasicSettings);
      this.tabPageAtmoOrb.Location = new System.Drawing.Point(4, 22);
      this.tabPageAtmoOrb.Name = "tabPageAtmoOrb";
      this.tabPageAtmoOrb.Padding = new System.Windows.Forms.Padding(3);
      this.tabPageAtmoOrb.Size = new System.Drawing.Size(842, 584);
      this.tabPageAtmoOrb.TabIndex = 6;
      this.tabPageAtmoOrb.Text = "AtmoOrb";
      // 
      // grpAtmoOrbLamps
      // 
      this.grpAtmoOrbLamps.Controls.Add(this.tbAtmoOrbLedCount);
      this.grpAtmoOrbLamps.Controls.Add(this.lblAtmoOrbLedCount);
      this.grpAtmoOrbLamps.Controls.Add(this.lblAtmoOrbProtocol);
      this.grpAtmoOrbLamps.Controls.Add(this.cbAtmoOrbProtocol);
      this.grpAtmoOrbLamps.Controls.Add(this.lblAtmoOrbVScanTo);
      this.grpAtmoOrbLamps.Controls.Add(this.lblAtmoOrbHScanTo);
      this.grpAtmoOrbLamps.Controls.Add(this.lblAtmoOrbConnection);
      this.grpAtmoOrbLamps.Controls.Add(this.cbAtmoOrbInvertZone);
      this.grpAtmoOrbLamps.Controls.Add(this.tbAtmoOrbVScanEnd);
      this.grpAtmoOrbLamps.Controls.Add(this.tbAtmoOrbVScanStart);
      this.grpAtmoOrbLamps.Controls.Add(this.lblAtmoOrbVScan);
      this.grpAtmoOrbLamps.Controls.Add(this.tbAtmoOrbHScanEnd);
      this.grpAtmoOrbLamps.Controls.Add(this.tbAtmoOrbHScanStart);
      this.grpAtmoOrbLamps.Controls.Add(this.lblAtmoOrbHScan);
      this.grpAtmoOrbLamps.Controls.Add(this.tbAtmoOrbPort);
      this.grpAtmoOrbLamps.Controls.Add(this.lblAtmoOrbPort);
      this.grpAtmoOrbLamps.Controls.Add(this.tbAtmoOrbIP);
      this.grpAtmoOrbLamps.Controls.Add(this.lblAtmoOrbIP);
      this.grpAtmoOrbLamps.Controls.Add(this.rbAtmoOrbUDP);
      this.grpAtmoOrbLamps.Controls.Add(this.rbAtmoOrbTCP);
      this.grpAtmoOrbLamps.Controls.Add(this.tbAtmoOrbID);
      this.grpAtmoOrbLamps.Controls.Add(this.lblAtmoOrbID);
      this.grpAtmoOrbLamps.Controls.Add(this.btnAtmoOrbRemove);
      this.grpAtmoOrbLamps.Controls.Add(this.btnAtmoOrbUpdate);
      this.grpAtmoOrbLamps.Controls.Add(this.btnAtmoOrbAdd);
      this.grpAtmoOrbLamps.Controls.Add(this.lbAtmoOrbLamps);
      this.grpAtmoOrbLamps.Location = new System.Drawing.Point(10, 165);
      this.grpAtmoOrbLamps.Name = "grpAtmoOrbLamps";
      this.grpAtmoOrbLamps.Size = new System.Drawing.Size(820, 280);
      this.grpAtmoOrbLamps.TabIndex = 1;
      this.grpAtmoOrbLamps.TabStop = false;
      this.grpAtmoOrbLamps.Text = "Lamp settings";
      // 
      // tbAtmoOrbLedCount
      // 
      this.tbAtmoOrbLedCount.Location = new System.Drawing.Point(490, 150);
      this.tbAtmoOrbLedCount.Name = "tbAtmoOrbLedCount";
      this.tbAtmoOrbLedCount.Size = new System.Drawing.Size(100, 20);
      this.tbAtmoOrbLedCount.TabIndex = 25;
      // 
      // lblAtmoOrbLedCount
      // 
      this.lblAtmoOrbLedCount.AutoSize = true;
      this.lblAtmoOrbLedCount.Location = new System.Drawing.Point(350, 150);
      this.lblAtmoOrbLedCount.Name = "lblAtmoOrbLedCount";
      this.lblAtmoOrbLedCount.Size = new System.Drawing.Size(61, 13);
      this.lblAtmoOrbLedCount.TabIndex = 24;
      this.lblAtmoOrbLedCount.Text = "LED count:";
      // 
      // lblAtmoOrbProtocol
      // 
      this.lblAtmoOrbProtocol.AutoSize = true;
      this.lblAtmoOrbProtocol.Location = new System.Drawing.Point(350, 75);
      this.lblAtmoOrbProtocol.Name = "lblAtmoOrbProtocol";
      this.lblAtmoOrbProtocol.Size = new System.Drawing.Size(49, 13);
      this.lblAtmoOrbProtocol.TabIndex = 23;
      this.lblAtmoOrbProtocol.Text = "Protocol:";
      // 
      // cbAtmoOrbProtocol
      // 
      this.cbAtmoOrbProtocol.FormattingEnabled = true;
      this.cbAtmoOrbProtocol.Items.AddRange(new object[] {
            "IP",
            "Broadcast",
            "Multicast"});
      this.cbAtmoOrbProtocol.Location = new System.Drawing.Point(490, 75);
      this.cbAtmoOrbProtocol.Name = "cbAtmoOrbProtocol";
      this.cbAtmoOrbProtocol.Size = new System.Drawing.Size(100, 21);
      this.cbAtmoOrbProtocol.TabIndex = 22;
      this.cbAtmoOrbProtocol.SelectedIndexChanged += new System.EventHandler(this.cbAtmoOrbProtocolType_SelectedIndexChanged);
      this.cbAtmoOrbProtocol.Validating += new System.ComponentModel.CancelEventHandler(this.cbAtmoOrbProtocolType_Validating);
      // 
      // lblAtmoOrbVScanTo
      // 
      this.lblAtmoOrbVScanTo.AutoSize = true;
      this.lblAtmoOrbVScanTo.Location = new System.Drawing.Point(599, 203);
      this.lblAtmoOrbVScanTo.Name = "lblAtmoOrbVScanTo";
      this.lblAtmoOrbVScanTo.Size = new System.Drawing.Size(16, 13);
      this.lblAtmoOrbVScanTo.TabIndex = 21;
      this.lblAtmoOrbVScanTo.Text = "to";
      // 
      // lblAtmoOrbHScanTo
      // 
      this.lblAtmoOrbHScanTo.AutoSize = true;
      this.lblAtmoOrbHScanTo.Location = new System.Drawing.Point(599, 178);
      this.lblAtmoOrbHScanTo.Name = "lblAtmoOrbHScanTo";
      this.lblAtmoOrbHScanTo.Size = new System.Drawing.Size(16, 13);
      this.lblAtmoOrbHScanTo.TabIndex = 20;
      this.lblAtmoOrbHScanTo.Text = "to";
      // 
      // lblAtmoOrbConnection
      // 
      this.lblAtmoOrbConnection.AutoSize = true;
      this.lblAtmoOrbConnection.Location = new System.Drawing.Point(350, 50);
      this.lblAtmoOrbConnection.Name = "lblAtmoOrbConnection";
      this.lblAtmoOrbConnection.Size = new System.Drawing.Size(87, 13);
      this.lblAtmoOrbConnection.TabIndex = 19;
      this.lblAtmoOrbConnection.Text = "Connection type:";
      // 
      // cbAtmoOrbInvertZone
      // 
      this.cbAtmoOrbInvertZone.AutoSize = true;
      this.cbAtmoOrbInvertZone.Location = new System.Drawing.Point(350, 225);
      this.cbAtmoOrbInvertZone.Name = "cbAtmoOrbInvertZone";
      this.cbAtmoOrbInvertZone.Size = new System.Drawing.Size(81, 17);
      this.cbAtmoOrbInvertZone.TabIndex = 18;
      this.cbAtmoOrbInvertZone.Text = "Invert Zone";
      this.cbAtmoOrbInvertZone.UseVisualStyleBackColor = true;
      // 
      // tbAtmoOrbVScanEnd
      // 
      this.tbAtmoOrbVScanEnd.Location = new System.Drawing.Point(621, 201);
      this.tbAtmoOrbVScanEnd.Name = "tbAtmoOrbVScanEnd";
      this.tbAtmoOrbVScanEnd.Size = new System.Drawing.Size(100, 20);
      this.tbAtmoOrbVScanEnd.TabIndex = 17;
      this.tbAtmoOrbVScanEnd.Validating += new System.ComponentModel.CancelEventHandler(this.tbAtmoOrbVScanEnd_Validating);
      // 
      // tbAtmoOrbVScanStart
      // 
      this.tbAtmoOrbVScanStart.Location = new System.Drawing.Point(490, 200);
      this.tbAtmoOrbVScanStart.Name = "tbAtmoOrbVScanStart";
      this.tbAtmoOrbVScanStart.Size = new System.Drawing.Size(100, 20);
      this.tbAtmoOrbVScanStart.TabIndex = 16;
      this.tbAtmoOrbVScanStart.Validating += new System.ComponentModel.CancelEventHandler(this.tbAtmoOrbVScanStart_Validating);
      // 
      // lblAtmoOrbVScan
      // 
      this.lblAtmoOrbVScan.AutoSize = true;
      this.lblAtmoOrbVScan.Location = new System.Drawing.Point(350, 200);
      this.lblAtmoOrbVScan.Name = "lblAtmoOrbVScan";
      this.lblAtmoOrbVScan.Size = new System.Drawing.Size(62, 13);
      this.lblAtmoOrbVScan.TabIndex = 15;
      this.lblAtmoOrbVScan.Text = "VScan from";
      // 
      // tbAtmoOrbHScanEnd
      // 
      this.tbAtmoOrbHScanEnd.Location = new System.Drawing.Point(621, 175);
      this.tbAtmoOrbHScanEnd.Name = "tbAtmoOrbHScanEnd";
      this.tbAtmoOrbHScanEnd.Size = new System.Drawing.Size(100, 20);
      this.tbAtmoOrbHScanEnd.TabIndex = 14;
      this.tbAtmoOrbHScanEnd.Validating += new System.ComponentModel.CancelEventHandler(this.tbAtmoOrbHScanEnd_Validating);
      // 
      // tbAtmoOrbHScanStart
      // 
      this.tbAtmoOrbHScanStart.Location = new System.Drawing.Point(490, 175);
      this.tbAtmoOrbHScanStart.Name = "tbAtmoOrbHScanStart";
      this.tbAtmoOrbHScanStart.Size = new System.Drawing.Size(100, 20);
      this.tbAtmoOrbHScanStart.TabIndex = 13;
      this.tbAtmoOrbHScanStart.Validating += new System.ComponentModel.CancelEventHandler(this.tbAtmoOrbHScanStart_Validating);
      // 
      // lblAtmoOrbHScan
      // 
      this.lblAtmoOrbHScan.AutoSize = true;
      this.lblAtmoOrbHScan.Location = new System.Drawing.Point(350, 175);
      this.lblAtmoOrbHScan.Name = "lblAtmoOrbHScan";
      this.lblAtmoOrbHScan.Size = new System.Drawing.Size(63, 13);
      this.lblAtmoOrbHScan.TabIndex = 12;
      this.lblAtmoOrbHScan.Text = "HScan from";
      // 
      // tbAtmoOrbPort
      // 
      this.tbAtmoOrbPort.Location = new System.Drawing.Point(490, 125);
      this.tbAtmoOrbPort.Name = "tbAtmoOrbPort";
      this.tbAtmoOrbPort.Size = new System.Drawing.Size(100, 20);
      this.tbAtmoOrbPort.TabIndex = 11;
      this.tbAtmoOrbPort.Validating += new System.ComponentModel.CancelEventHandler(this.tbAtmoOrbPort_Validating);
      // 
      // lblAtmoOrbPort
      // 
      this.lblAtmoOrbPort.AutoSize = true;
      this.lblAtmoOrbPort.Location = new System.Drawing.Point(350, 125);
      this.lblAtmoOrbPort.Name = "lblAtmoOrbPort";
      this.lblAtmoOrbPort.Size = new System.Drawing.Size(29, 13);
      this.lblAtmoOrbPort.TabIndex = 10;
      this.lblAtmoOrbPort.Text = "Port:";
      // 
      // tbAtmoOrbIP
      // 
      this.tbAtmoOrbIP.Location = new System.Drawing.Point(490, 100);
      this.tbAtmoOrbIP.Name = "tbAtmoOrbIP";
      this.tbAtmoOrbIP.Size = new System.Drawing.Size(100, 20);
      this.tbAtmoOrbIP.TabIndex = 9;
      this.tbAtmoOrbIP.Validating += new System.ComponentModel.CancelEventHandler(this.tbAtmoOrbIP_Validating);
      // 
      // lblAtmoOrbIP
      // 
      this.lblAtmoOrbIP.AutoSize = true;
      this.lblAtmoOrbIP.Location = new System.Drawing.Point(350, 100);
      this.lblAtmoOrbIP.Name = "lblAtmoOrbIP";
      this.lblAtmoOrbIP.Size = new System.Drawing.Size(20, 13);
      this.lblAtmoOrbIP.TabIndex = 8;
      this.lblAtmoOrbIP.Text = "IP:";
      // 
      // rbAtmoOrbUDP
      // 
      this.rbAtmoOrbUDP.AutoSize = true;
      this.rbAtmoOrbUDP.Location = new System.Drawing.Point(565, 50);
      this.rbAtmoOrbUDP.Name = "rbAtmoOrbUDP";
      this.rbAtmoOrbUDP.Size = new System.Drawing.Size(48, 17);
      this.rbAtmoOrbUDP.TabIndex = 7;
      this.rbAtmoOrbUDP.TabStop = true;
      this.rbAtmoOrbUDP.Text = "UDP";
      this.rbAtmoOrbUDP.UseVisualStyleBackColor = true;
      this.rbAtmoOrbUDP.CheckedChanged += new System.EventHandler(this.rbAtmoOrbUDPTCP_CheckedChanged);
      // 
      // rbAtmoOrbTCP
      // 
      this.rbAtmoOrbTCP.AutoSize = true;
      this.rbAtmoOrbTCP.Location = new System.Drawing.Point(490, 50);
      this.rbAtmoOrbTCP.Name = "rbAtmoOrbTCP";
      this.rbAtmoOrbTCP.Size = new System.Drawing.Size(46, 17);
      this.rbAtmoOrbTCP.TabIndex = 6;
      this.rbAtmoOrbTCP.TabStop = true;
      this.rbAtmoOrbTCP.Text = "TCP";
      this.rbAtmoOrbTCP.UseVisualStyleBackColor = true;
      this.rbAtmoOrbTCP.CheckedChanged += new System.EventHandler(this.rbAtmoOrbUDPTCP_CheckedChanged);
      // 
      // tbAtmoOrbID
      // 
      this.tbAtmoOrbID.Location = new System.Drawing.Point(490, 25);
      this.tbAtmoOrbID.Name = "tbAtmoOrbID";
      this.tbAtmoOrbID.Size = new System.Drawing.Size(100, 20);
      this.tbAtmoOrbID.TabIndex = 5;
      // 
      // lblAtmoOrbID
      // 
      this.lblAtmoOrbID.AutoSize = true;
      this.lblAtmoOrbID.Location = new System.Drawing.Point(350, 25);
      this.lblAtmoOrbID.Name = "lblAtmoOrbID";
      this.lblAtmoOrbID.Size = new System.Drawing.Size(21, 13);
      this.lblAtmoOrbID.TabIndex = 4;
      this.lblAtmoOrbID.Text = "ID:";
      // 
      // btnAtmoOrbRemove
      // 
      this.btnAtmoOrbRemove.Location = new System.Drawing.Point(621, 250);
      this.btnAtmoOrbRemove.Name = "btnAtmoOrbRemove";
      this.btnAtmoOrbRemove.Size = new System.Drawing.Size(75, 23);
      this.btnAtmoOrbRemove.TabIndex = 3;
      this.btnAtmoOrbRemove.Text = "Remove";
      this.btnAtmoOrbRemove.UseVisualStyleBackColor = true;
      this.btnAtmoOrbRemove.Click += new System.EventHandler(this.btnAtmoOrbRemove_Click);
      // 
      // btnAtmoOrbUpdate
      // 
      this.btnAtmoOrbUpdate.Location = new System.Drawing.Point(490, 250);
      this.btnAtmoOrbUpdate.Name = "btnAtmoOrbUpdate";
      this.btnAtmoOrbUpdate.Size = new System.Drawing.Size(75, 23);
      this.btnAtmoOrbUpdate.TabIndex = 2;
      this.btnAtmoOrbUpdate.Text = "Update";
      this.btnAtmoOrbUpdate.UseVisualStyleBackColor = true;
      this.btnAtmoOrbUpdate.Click += new System.EventHandler(this.btnAtmoOrbUpdate_Click);
      // 
      // btnAtmoOrbAdd
      // 
      this.btnAtmoOrbAdd.Location = new System.Drawing.Point(350, 250);
      this.btnAtmoOrbAdd.Name = "btnAtmoOrbAdd";
      this.btnAtmoOrbAdd.Size = new System.Drawing.Size(75, 23);
      this.btnAtmoOrbAdd.TabIndex = 1;
      this.btnAtmoOrbAdd.Text = "Add";
      this.btnAtmoOrbAdd.UseVisualStyleBackColor = true;
      this.btnAtmoOrbAdd.Click += new System.EventHandler(this.btnAtmoOrbAdd_Click);
      // 
      // lbAtmoOrbLamps
      // 
      this.lbAtmoOrbLamps.FormattingEnabled = true;
      this.lbAtmoOrbLamps.Location = new System.Drawing.Point(12, 25);
      this.lbAtmoOrbLamps.Name = "lbAtmoOrbLamps";
      this.lbAtmoOrbLamps.Size = new System.Drawing.Size(248, 186);
      this.lbAtmoOrbLamps.TabIndex = 0;
      this.lbAtmoOrbLamps.SelectedIndexChanged += new System.EventHandler(this.lbAtmoOrbLamps_SelectedIndexChanged);
      // 
      // grpAtmoOrbBasicSettings
      // 
      this.grpAtmoOrbBasicSettings.Controls.Add(this.tbAtmoOrbSmoothingThreshold);
      this.grpAtmoOrbBasicSettings.Controls.Add(this.lblAtmoOrbSmoothingThreshold);
      this.grpAtmoOrbBasicSettings.Controls.Add(this.cbAtmoOrbUseSmoothing);
      this.grpAtmoOrbBasicSettings.Controls.Add(this.tbAtmoOrbGamma);
      this.grpAtmoOrbBasicSettings.Controls.Add(this.tbAtmoOrbSaturation);
      this.grpAtmoOrbBasicSettings.Controls.Add(this.lblAtmoOrbGamma);
      this.grpAtmoOrbBasicSettings.Controls.Add(this.lblAtmoOrbSaturation);
      this.grpAtmoOrbBasicSettings.Controls.Add(this.lblAtmoOrbBlackThreshold);
      this.grpAtmoOrbBasicSettings.Controls.Add(this.lblAtmoOrbThreshold);
      this.grpAtmoOrbBasicSettings.Controls.Add(this.lblAtmoOrbMinDiversion);
      this.grpAtmoOrbBasicSettings.Controls.Add(this.lblAtmoOrbBroadcastPort);
      this.grpAtmoOrbBasicSettings.Controls.Add(this.tbAtmoOrbBlackThreshold);
      this.grpAtmoOrbBasicSettings.Controls.Add(this.tbAtmoOrbThreshold);
      this.grpAtmoOrbBasicSettings.Controls.Add(this.tbAtmoOrbMinDiversion);
      this.grpAtmoOrbBasicSettings.Controls.Add(this.tbAtmoOrbBroadcastPort);
      this.grpAtmoOrbBasicSettings.Controls.Add(this.cbAtmoOrbUseOverallLightness);
      this.grpAtmoOrbBasicSettings.Location = new System.Drawing.Point(10, 10);
      this.grpAtmoOrbBasicSettings.Name = "grpAtmoOrbBasicSettings";
      this.grpAtmoOrbBasicSettings.Size = new System.Drawing.Size(820, 150);
      this.grpAtmoOrbBasicSettings.TabIndex = 0;
      this.grpAtmoOrbBasicSettings.TabStop = false;
      this.grpAtmoOrbBasicSettings.Text = "Basic settings";
      // 
      // cbAtmoOrbUseSmoothing
      // 
      this.cbAtmoOrbUseSmoothing.AutoSize = true;
      this.cbAtmoOrbUseSmoothing.Location = new System.Drawing.Point(350, 75);
      this.cbAtmoOrbUseSmoothing.Name = "cbAtmoOrbUseSmoothing";
      this.cbAtmoOrbUseSmoothing.Size = new System.Drawing.Size(96, 17);
      this.cbAtmoOrbUseSmoothing.TabIndex = 21;
      this.cbAtmoOrbUseSmoothing.Text = "Use smoothing";
      this.cbAtmoOrbUseSmoothing.UseVisualStyleBackColor = true;
      // 
      // tbAtmoOrbGamma
      // 
      this.tbAtmoOrbGamma.Location = new System.Drawing.Point(500, 22);
      this.tbAtmoOrbGamma.Name = "tbAtmoOrbGamma";
      this.tbAtmoOrbGamma.Size = new System.Drawing.Size(100, 20);
      this.tbAtmoOrbGamma.TabIndex = 12;
      this.tbAtmoOrbGamma.Validating += new System.ComponentModel.CancelEventHandler(this.tbAtmoOrbGamma_Validating);
      // 
      // tbAtmoOrbSaturation
      // 
      this.tbAtmoOrbSaturation.Location = new System.Drawing.Point(160, 122);
      this.tbAtmoOrbSaturation.Name = "tbAtmoOrbSaturation";
      this.tbAtmoOrbSaturation.Size = new System.Drawing.Size(100, 20);
      this.tbAtmoOrbSaturation.TabIndex = 11;
      this.tbAtmoOrbSaturation.Validating += new System.ComponentModel.CancelEventHandler(this.tbAtmoOrbSaturation_Validating);
      // 
      // lblAtmoOrbGamma
      // 
      this.lblAtmoOrbGamma.AutoSize = true;
      this.lblAtmoOrbGamma.Location = new System.Drawing.Point(350, 25);
      this.lblAtmoOrbGamma.Name = "lblAtmoOrbGamma";
      this.lblAtmoOrbGamma.Size = new System.Drawing.Size(96, 13);
      this.lblAtmoOrbGamma.TabIndex = 10;
      this.lblAtmoOrbGamma.Text = "Gamma correction:";
      // 
      // lblAtmoOrbSaturation
      // 
      this.lblAtmoOrbSaturation.AutoSize = true;
      this.lblAtmoOrbSaturation.Location = new System.Drawing.Point(10, 125);
      this.lblAtmoOrbSaturation.Name = "lblAtmoOrbSaturation";
      this.lblAtmoOrbSaturation.Size = new System.Drawing.Size(108, 13);
      this.lblAtmoOrbSaturation.TabIndex = 9;
      this.lblAtmoOrbSaturation.Text = "Saturation correction:";
      // 
      // lblAtmoOrbBlackThreshold
      // 
      this.lblAtmoOrbBlackThreshold.AutoSize = true;
      this.lblAtmoOrbBlackThreshold.Location = new System.Drawing.Point(10, 100);
      this.lblAtmoOrbBlackThreshold.Name = "lblAtmoOrbBlackThreshold";
      this.lblAtmoOrbBlackThreshold.Size = new System.Drawing.Size(83, 13);
      this.lblAtmoOrbBlackThreshold.TabIndex = 8;
      this.lblAtmoOrbBlackThreshold.Text = "Black threshold:";
      // 
      // lblAtmoOrbThreshold
      // 
      this.lblAtmoOrbThreshold.AutoSize = true;
      this.lblAtmoOrbThreshold.Location = new System.Drawing.Point(10, 75);
      this.lblAtmoOrbThreshold.Name = "lblAtmoOrbThreshold";
      this.lblAtmoOrbThreshold.Size = new System.Drawing.Size(54, 13);
      this.lblAtmoOrbThreshold.TabIndex = 7;
      this.lblAtmoOrbThreshold.Text = "Threshold";
      // 
      // lblAtmoOrbMinDiversion
      // 
      this.lblAtmoOrbMinDiversion.AutoSize = true;
      this.lblAtmoOrbMinDiversion.Location = new System.Drawing.Point(10, 50);
      this.lblAtmoOrbMinDiversion.Name = "lblAtmoOrbMinDiversion";
      this.lblAtmoOrbMinDiversion.Size = new System.Drawing.Size(101, 13);
      this.lblAtmoOrbMinDiversion.TabIndex = 6;
      this.lblAtmoOrbMinDiversion.Text = "Min. color diversion:";
      // 
      // lblAtmoOrbBroadcastPort
      // 
      this.lblAtmoOrbBroadcastPort.AutoSize = true;
      this.lblAtmoOrbBroadcastPort.Location = new System.Drawing.Point(10, 25);
      this.lblAtmoOrbBroadcastPort.Name = "lblAtmoOrbBroadcastPort";
      this.lblAtmoOrbBroadcastPort.Size = new System.Drawing.Size(79, 13);
      this.lblAtmoOrbBroadcastPort.TabIndex = 5;
      this.lblAtmoOrbBroadcastPort.Text = "Broadcast port:";
      // 
      // tbAtmoOrbBlackThreshold
      // 
      this.tbAtmoOrbBlackThreshold.Location = new System.Drawing.Point(160, 97);
      this.tbAtmoOrbBlackThreshold.Name = "tbAtmoOrbBlackThreshold";
      this.tbAtmoOrbBlackThreshold.Size = new System.Drawing.Size(100, 20);
      this.tbAtmoOrbBlackThreshold.TabIndex = 4;
      this.tbAtmoOrbBlackThreshold.Validating += new System.ComponentModel.CancelEventHandler(this.tbAtmoOrbBlackThreshold_Validating);
      // 
      // tbAtmoOrbThreshold
      // 
      this.tbAtmoOrbThreshold.Location = new System.Drawing.Point(160, 72);
      this.tbAtmoOrbThreshold.Name = "tbAtmoOrbThreshold";
      this.tbAtmoOrbThreshold.Size = new System.Drawing.Size(100, 20);
      this.tbAtmoOrbThreshold.TabIndex = 3;
      this.tbAtmoOrbThreshold.Validating += new System.ComponentModel.CancelEventHandler(this.tbAtmoOrbThreshold_Validating);
      // 
      // tbAtmoOrbMinDiversion
      // 
      this.tbAtmoOrbMinDiversion.Location = new System.Drawing.Point(160, 47);
      this.tbAtmoOrbMinDiversion.Name = "tbAtmoOrbMinDiversion";
      this.tbAtmoOrbMinDiversion.Size = new System.Drawing.Size(100, 20);
      this.tbAtmoOrbMinDiversion.TabIndex = 2;
      this.tbAtmoOrbMinDiversion.Validating += new System.ComponentModel.CancelEventHandler(this.tbAtmoOrbMinDiversion_Validating);
      // 
      // tbAtmoOrbBroadcastPort
      // 
      this.tbAtmoOrbBroadcastPort.Location = new System.Drawing.Point(160, 22);
      this.tbAtmoOrbBroadcastPort.Name = "tbAtmoOrbBroadcastPort";
      this.tbAtmoOrbBroadcastPort.Size = new System.Drawing.Size(100, 20);
      this.tbAtmoOrbBroadcastPort.TabIndex = 1;
      this.tbAtmoOrbBroadcastPort.Validating += new System.ComponentModel.CancelEventHandler(this.tbAtmoOrbBroadcastPort_Validating);
      // 
      // cbAtmoOrbUseOverallLightness
      // 
      this.cbAtmoOrbUseOverallLightness.AutoSize = true;
      this.cbAtmoOrbUseOverallLightness.Location = new System.Drawing.Point(350, 50);
      this.cbAtmoOrbUseOverallLightness.Name = "cbAtmoOrbUseOverallLightness";
      this.cbAtmoOrbUseOverallLightness.Size = new System.Drawing.Size(123, 17);
      this.cbAtmoOrbUseOverallLightness.TabIndex = 0;
      this.cbAtmoOrbUseOverallLightness.Text = "Use overall lightness";
      this.cbAtmoOrbUseOverallLightness.UseVisualStyleBackColor = true;
      // 
      // tabPageBoblight
      // 
      this.tabPageBoblight.BackColor = System.Drawing.SystemColors.Control;
      this.tabPageBoblight.Controls.Add(this.grpBoblightSettings);
      this.tabPageBoblight.Controls.Add(this.grpBoblightGeneral);
      this.tabPageBoblight.Location = new System.Drawing.Point(4, 22);
      this.tabPageBoblight.Name = "tabPageBoblight";
      this.tabPageBoblight.Padding = new System.Windows.Forms.Padding(3);
      this.tabPageBoblight.Size = new System.Drawing.Size(842, 584);
      this.tabPageBoblight.TabIndex = 4;
      this.tabPageBoblight.Text = "Boblight";
      // 
      // grpBoblightSettings
      // 
      this.grpBoblightSettings.Controls.Add(this.tbBoblightGamma);
      this.grpBoblightSettings.Controls.Add(this.tbarBoblightGamma);
      this.grpBoblightSettings.Controls.Add(this.lblBoblightGamma);
      this.grpBoblightSettings.Controls.Add(this.tbBoblightThreshold);
      this.grpBoblightSettings.Controls.Add(this.tbBoblightValue);
      this.grpBoblightSettings.Controls.Add(this.tbBoblightSaturation);
      this.grpBoblightSettings.Controls.Add(this.tbBoblightAutospeed);
      this.grpBoblightSettings.Controls.Add(this.tbBoblightSpeed);
      this.grpBoblightSettings.Controls.Add(this.ckBoblightInterpolation);
      this.grpBoblightSettings.Controls.Add(this.lblBoblightThreshold);
      this.grpBoblightSettings.Controls.Add(this.lblBoblightValue);
      this.grpBoblightSettings.Controls.Add(this.lblBoblightSaturation);
      this.grpBoblightSettings.Controls.Add(this.lblBoblightAutospeed);
      this.grpBoblightSettings.Controls.Add(this.lblBoblightSpeed);
      this.grpBoblightSettings.Controls.Add(this.tbarBoblightThreshold);
      this.grpBoblightSettings.Controls.Add(this.tbarBoblightValue);
      this.grpBoblightSettings.Controls.Add(this.tbarBoblightSaturation);
      this.grpBoblightSettings.Controls.Add(this.tbarBoblightAutospeed);
      this.grpBoblightSettings.Controls.Add(this.tbarBoblightSpeed);
      this.grpBoblightSettings.Location = new System.Drawing.Point(10, 165);
      this.grpBoblightSettings.Name = "grpBoblightSettings";
      this.grpBoblightSettings.Size = new System.Drawing.Size(820, 260);
      this.grpBoblightSettings.TabIndex = 1;
      this.grpBoblightSettings.TabStop = false;
      this.grpBoblightSettings.Text = "Boblight settings";
      // 
      // tbBoblightGamma
      // 
      this.tbBoblightGamma.Location = new System.Drawing.Point(520, 197);
      this.tbBoblightGamma.Name = "tbBoblightGamma";
      this.tbBoblightGamma.ReadOnly = true;
      this.tbBoblightGamma.Size = new System.Drawing.Size(50, 20);
      this.tbBoblightGamma.TabIndex = 19;
      // 
      // tbarBoblightGamma
      // 
      this.tbarBoblightGamma.Location = new System.Drawing.Point(150, 195);
      this.tbarBoblightGamma.Maximum = 100;
      this.tbarBoblightGamma.Name = "tbarBoblightGamma";
      this.tbarBoblightGamma.Size = new System.Drawing.Size(350, 45);
      this.tbarBoblightGamma.TabIndex = 18;
      this.tbarBoblightGamma.ValueChanged += new System.EventHandler(this.tbarBoblightGamma_ValueChanged);
      // 
      // lblBoblightGamma
      // 
      this.lblBoblightGamma.AutoSize = true;
      this.lblBoblightGamma.Location = new System.Drawing.Point(10, 200);
      this.lblBoblightGamma.Name = "lblBoblightGamma";
      this.lblBoblightGamma.Size = new System.Drawing.Size(46, 13);
      this.lblBoblightGamma.TabIndex = 17;
      this.lblBoblightGamma.Text = "Gamma:";
      // 
      // tbBoblightThreshold
      // 
      this.tbBoblightThreshold.Location = new System.Drawing.Point(520, 162);
      this.tbBoblightThreshold.Name = "tbBoblightThreshold";
      this.tbBoblightThreshold.ReadOnly = true;
      this.tbBoblightThreshold.Size = new System.Drawing.Size(50, 20);
      this.tbBoblightThreshold.TabIndex = 16;
      // 
      // tbBoblightValue
      // 
      this.tbBoblightValue.Location = new System.Drawing.Point(520, 127);
      this.tbBoblightValue.Name = "tbBoblightValue";
      this.tbBoblightValue.ReadOnly = true;
      this.tbBoblightValue.Size = new System.Drawing.Size(50, 20);
      this.tbBoblightValue.TabIndex = 15;
      // 
      // tbBoblightSaturation
      // 
      this.tbBoblightSaturation.Location = new System.Drawing.Point(520, 92);
      this.tbBoblightSaturation.Name = "tbBoblightSaturation";
      this.tbBoblightSaturation.ReadOnly = true;
      this.tbBoblightSaturation.Size = new System.Drawing.Size(50, 20);
      this.tbBoblightSaturation.TabIndex = 14;
      // 
      // tbBoblightAutospeed
      // 
      this.tbBoblightAutospeed.Location = new System.Drawing.Point(520, 57);
      this.tbBoblightAutospeed.Name = "tbBoblightAutospeed";
      this.tbBoblightAutospeed.ReadOnly = true;
      this.tbBoblightAutospeed.Size = new System.Drawing.Size(50, 20);
      this.tbBoblightAutospeed.TabIndex = 13;
      // 
      // tbBoblightSpeed
      // 
      this.tbBoblightSpeed.Location = new System.Drawing.Point(520, 22);
      this.tbBoblightSpeed.Name = "tbBoblightSpeed";
      this.tbBoblightSpeed.ReadOnly = true;
      this.tbBoblightSpeed.Size = new System.Drawing.Size(50, 20);
      this.tbBoblightSpeed.TabIndex = 12;
      // 
      // ckBoblightInterpolation
      // 
      this.ckBoblightInterpolation.AutoSize = true;
      this.ckBoblightInterpolation.Location = new System.Drawing.Point(12, 235);
      this.ckBoblightInterpolation.Name = "ckBoblightInterpolation";
      this.ckBoblightInterpolation.Size = new System.Drawing.Size(84, 17);
      this.ckBoblightInterpolation.TabIndex = 10;
      this.ckBoblightInterpolation.Text = "Interpolation";
      this.ckBoblightInterpolation.UseVisualStyleBackColor = true;
      // 
      // lblBoblightThreshold
      // 
      this.lblBoblightThreshold.AutoSize = true;
      this.lblBoblightThreshold.Location = new System.Drawing.Point(10, 165);
      this.lblBoblightThreshold.Name = "lblBoblightThreshold";
      this.lblBoblightThreshold.Size = new System.Drawing.Size(57, 13);
      this.lblBoblightThreshold.TabIndex = 9;
      this.lblBoblightThreshold.Text = "Threshold:";
      // 
      // lblBoblightValue
      // 
      this.lblBoblightValue.AutoSize = true;
      this.lblBoblightValue.Location = new System.Drawing.Point(10, 130);
      this.lblBoblightValue.Name = "lblBoblightValue";
      this.lblBoblightValue.Size = new System.Drawing.Size(37, 13);
      this.lblBoblightValue.TabIndex = 8;
      this.lblBoblightValue.Text = "Value:";
      // 
      // lblBoblightSaturation
      // 
      this.lblBoblightSaturation.AutoSize = true;
      this.lblBoblightSaturation.Location = new System.Drawing.Point(10, 95);
      this.lblBoblightSaturation.Name = "lblBoblightSaturation";
      this.lblBoblightSaturation.Size = new System.Drawing.Size(58, 13);
      this.lblBoblightSaturation.TabIndex = 7;
      this.lblBoblightSaturation.Text = "Saturation:";
      // 
      // lblBoblightAutospeed
      // 
      this.lblBoblightAutospeed.AutoSize = true;
      this.lblBoblightAutospeed.Location = new System.Drawing.Point(10, 60);
      this.lblBoblightAutospeed.Name = "lblBoblightAutospeed";
      this.lblBoblightAutospeed.Size = new System.Drawing.Size(61, 13);
      this.lblBoblightAutospeed.TabIndex = 6;
      this.lblBoblightAutospeed.Text = "Autospeed:";
      // 
      // lblBoblightSpeed
      // 
      this.lblBoblightSpeed.AutoSize = true;
      this.lblBoblightSpeed.Location = new System.Drawing.Point(10, 25);
      this.lblBoblightSpeed.Name = "lblBoblightSpeed";
      this.lblBoblightSpeed.Size = new System.Drawing.Size(41, 13);
      this.lblBoblightSpeed.TabIndex = 5;
      this.lblBoblightSpeed.Text = "Speed:";
      // 
      // tbarBoblightThreshold
      // 
      this.tbarBoblightThreshold.Location = new System.Drawing.Point(150, 160);
      this.tbarBoblightThreshold.Maximum = 255;
      this.tbarBoblightThreshold.Name = "tbarBoblightThreshold";
      this.tbarBoblightThreshold.Size = new System.Drawing.Size(350, 45);
      this.tbarBoblightThreshold.TabIndex = 4;
      this.tbarBoblightThreshold.ValueChanged += new System.EventHandler(this.tbarBoblightThreshold_ValueChanged);
      // 
      // tbarBoblightValue
      // 
      this.tbarBoblightValue.Location = new System.Drawing.Point(150, 125);
      this.tbarBoblightValue.Maximum = 20;
      this.tbarBoblightValue.Name = "tbarBoblightValue";
      this.tbarBoblightValue.Size = new System.Drawing.Size(350, 45);
      this.tbarBoblightValue.TabIndex = 3;
      this.tbarBoblightValue.ValueChanged += new System.EventHandler(this.tbarBoblightValue_ValueChanged);
      // 
      // tbarBoblightSaturation
      // 
      this.tbarBoblightSaturation.Location = new System.Drawing.Point(150, 90);
      this.tbarBoblightSaturation.Maximum = 20;
      this.tbarBoblightSaturation.Name = "tbarBoblightSaturation";
      this.tbarBoblightSaturation.Size = new System.Drawing.Size(350, 45);
      this.tbarBoblightSaturation.TabIndex = 2;
      this.tbarBoblightSaturation.ValueChanged += new System.EventHandler(this.tbarBoblightSaturation_ValueChanged);
      // 
      // tbarBoblightAutospeed
      // 
      this.tbarBoblightAutospeed.Location = new System.Drawing.Point(150, 55);
      this.tbarBoblightAutospeed.Maximum = 100;
      this.tbarBoblightAutospeed.Name = "tbarBoblightAutospeed";
      this.tbarBoblightAutospeed.Size = new System.Drawing.Size(350, 45);
      this.tbarBoblightAutospeed.TabIndex = 1;
      this.tbarBoblightAutospeed.ValueChanged += new System.EventHandler(this.tbarBoblightAutospeed_ValueChanged);
      // 
      // tbarBoblightSpeed
      // 
      this.tbarBoblightSpeed.Location = new System.Drawing.Point(150, 20);
      this.tbarBoblightSpeed.Maximum = 100;
      this.tbarBoblightSpeed.Name = "tbarBoblightSpeed";
      this.tbarBoblightSpeed.Size = new System.Drawing.Size(350, 45);
      this.tbarBoblightSpeed.TabIndex = 0;
      this.tbarBoblightSpeed.ValueChanged += new System.EventHandler(this.tbarBoblightSpeed_ValueChanged);
      // 
      // grpBoblightGeneral
      // 
      this.grpBoblightGeneral.Controls.Add(this.tbBoblightMaxFPS);
      this.grpBoblightGeneral.Controls.Add(this.tbBoblightReconnectDelay);
      this.grpBoblightGeneral.Controls.Add(this.tbBoblightMaxReconnectAttempts);
      this.grpBoblightGeneral.Controls.Add(this.tbBoblightPort);
      this.grpBoblightGeneral.Controls.Add(this.tbBoblightIP);
      this.grpBoblightGeneral.Controls.Add(this.lblBoblightMaxFPS);
      this.grpBoblightGeneral.Controls.Add(this.lblBoblightReconnectDelay);
      this.grpBoblightGeneral.Controls.Add(this.lblBoblightMaxReconnectAttempts);
      this.grpBoblightGeneral.Controls.Add(this.lblBoblightPort);
      this.grpBoblightGeneral.Controls.Add(this.lblBoblightIP);
      this.grpBoblightGeneral.Location = new System.Drawing.Point(10, 10);
      this.grpBoblightGeneral.Name = "grpBoblightGeneral";
      this.grpBoblightGeneral.Size = new System.Drawing.Size(820, 150);
      this.grpBoblightGeneral.TabIndex = 0;
      this.grpBoblightGeneral.TabStop = false;
      this.grpBoblightGeneral.Text = "General settings";
      // 
      // tbBoblightMaxFPS
      // 
      this.tbBoblightMaxFPS.Location = new System.Drawing.Point(230, 122);
      this.tbBoblightMaxFPS.Name = "tbBoblightMaxFPS";
      this.tbBoblightMaxFPS.Size = new System.Drawing.Size(100, 20);
      this.tbBoblightMaxFPS.TabIndex = 9;
      this.tbBoblightMaxFPS.Validating += new System.ComponentModel.CancelEventHandler(this.tbBoblightMaxFPS_Validating);
      // 
      // tbBoblightReconnectDelay
      // 
      this.tbBoblightReconnectDelay.Location = new System.Drawing.Point(230, 97);
      this.tbBoblightReconnectDelay.Name = "tbBoblightReconnectDelay";
      this.tbBoblightReconnectDelay.Size = new System.Drawing.Size(100, 20);
      this.tbBoblightReconnectDelay.TabIndex = 8;
      this.tbBoblightReconnectDelay.Validating += new System.ComponentModel.CancelEventHandler(this.tbBoblightReconnectDelay_Validating);
      // 
      // tbBoblightMaxReconnectAttempts
      // 
      this.tbBoblightMaxReconnectAttempts.Location = new System.Drawing.Point(230, 72);
      this.tbBoblightMaxReconnectAttempts.Name = "tbBoblightMaxReconnectAttempts";
      this.tbBoblightMaxReconnectAttempts.Size = new System.Drawing.Size(100, 20);
      this.tbBoblightMaxReconnectAttempts.TabIndex = 7;
      this.tbBoblightMaxReconnectAttempts.Validating += new System.ComponentModel.CancelEventHandler(this.tbBoblightMaxReconnectAttempts_Validating);
      // 
      // tbBoblightPort
      // 
      this.tbBoblightPort.Location = new System.Drawing.Point(230, 47);
      this.tbBoblightPort.Name = "tbBoblightPort";
      this.tbBoblightPort.Size = new System.Drawing.Size(100, 20);
      this.tbBoblightPort.TabIndex = 6;
      this.tbBoblightPort.Validating += new System.ComponentModel.CancelEventHandler(this.tbBoblightPort_Validating);
      // 
      // tbBoblightIP
      // 
      this.tbBoblightIP.Location = new System.Drawing.Point(230, 22);
      this.tbBoblightIP.Name = "tbBoblightIP";
      this.tbBoblightIP.Size = new System.Drawing.Size(100, 20);
      this.tbBoblightIP.TabIndex = 5;
      this.tbBoblightIP.Validating += new System.ComponentModel.CancelEventHandler(this.tbBoblightIP_Validating);
      // 
      // lblBoblightMaxFPS
      // 
      this.lblBoblightMaxFPS.AutoSize = true;
      this.lblBoblightMaxFPS.Location = new System.Drawing.Point(10, 125);
      this.lblBoblightMaxFPS.Name = "lblBoblightMaxFPS";
      this.lblBoblightMaxFPS.Size = new System.Drawing.Size(105, 13);
      this.lblBoblightMaxFPS.TabIndex = 4;
      this.lblBoblightMaxFPS.Text = "Limit capture FPS to:";
      // 
      // lblBoblightReconnectDelay
      // 
      this.lblBoblightReconnectDelay.AutoSize = true;
      this.lblBoblightReconnectDelay.Location = new System.Drawing.Point(10, 100);
      this.lblBoblightReconnectDelay.Name = "lblBoblightReconnectDelay";
      this.lblBoblightReconnectDelay.Size = new System.Drawing.Size(91, 13);
      this.lblBoblightReconnectDelay.TabIndex = 3;
      this.lblBoblightReconnectDelay.Text = "Reconnect delay:";
      // 
      // lblBoblightMaxReconnectAttempts
      // 
      this.lblBoblightMaxReconnectAttempts.AutoSize = true;
      this.lblBoblightMaxReconnectAttempts.Location = new System.Drawing.Point(10, 75);
      this.lblBoblightMaxReconnectAttempts.Name = "lblBoblightMaxReconnectAttempts";
      this.lblBoblightMaxReconnectAttempts.Size = new System.Drawing.Size(106, 13);
      this.lblBoblightMaxReconnectAttempts.TabIndex = 2;
      this.lblBoblightMaxReconnectAttempts.Text = "Reconnect attempts:";
      // 
      // lblBoblightPort
      // 
      this.lblBoblightPort.AutoSize = true;
      this.lblBoblightPort.Location = new System.Drawing.Point(10, 50);
      this.lblBoblightPort.Name = "lblBoblightPort";
      this.lblBoblightPort.Size = new System.Drawing.Size(29, 13);
      this.lblBoblightPort.TabIndex = 1;
      this.lblBoblightPort.Text = "Port:";
      // 
      // lblBoblightIP
      // 
      this.lblBoblightIP.AutoSize = true;
      this.lblBoblightIP.Location = new System.Drawing.Point(10, 25);
      this.lblBoblightIP.Name = "lblBoblightIP";
      this.lblBoblightIP.Size = new System.Drawing.Size(20, 13);
      this.lblBoblightIP.TabIndex = 0;
      this.lblBoblightIP.Text = "IP:";
      // 
      // tabPageHue
      // 
      this.tabPageHue.BackColor = System.Drawing.SystemColors.Control;
      this.tabPageHue.Controls.Add(this.grpHueTheaterMode);
      this.tabPageHue.Controls.Add(this.grpHueAverageColor);
      this.tabPageHue.Controls.Add(this.grpHueGeneralSettings);
      this.tabPageHue.Controls.Add(this.grpHueNetworkSettings);
      this.tabPageHue.Location = new System.Drawing.Point(4, 22);
      this.tabPageHue.Name = "tabPageHue";
      this.tabPageHue.Size = new System.Drawing.Size(842, 584);
      this.tabPageHue.TabIndex = 3;
      this.tabPageHue.Text = "Hue";
      // 
      // grpHueTheaterMode
      // 
      this.grpHueTheaterMode.Controls.Add(this.lblHintHueTheaterMode);
      this.grpHueTheaterMode.Controls.Add(this.ckHueTheaterRestoreLights);
      this.grpHueTheaterMode.Controls.Add(this.ckHueTheaterEnabled);
      this.grpHueTheaterMode.Location = new System.Drawing.Point(10, 450);
      this.grpHueTheaterMode.Name = "grpHueTheaterMode";
      this.grpHueTheaterMode.Size = new System.Drawing.Size(820, 100);
      this.grpHueTheaterMode.TabIndex = 29;
      this.grpHueTheaterMode.TabStop = false;
      this.grpHueTheaterMode.Text = "Theater mode";
      // 
      // lblHintHueTheaterMode
      // 
      this.lblHintHueTheaterMode.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblHintHueTheaterMode.Location = new System.Drawing.Point(9, 25);
      this.lblHintHueTheaterMode.Name = "lblHintHueTheaterMode";
      this.lblHintHueTheaterMode.Size = new System.Drawing.Size(796, 13);
      this.lblHintHueTheaterMode.TabIndex = 22;
      this.lblHintHueTheaterMode.Text = "Hint: when theater mode is enabled the lights will turn off during movie playback" +
    " and back on after playback if that option is enabled";
      // 
      // ckHueTheaterRestoreLights
      // 
      this.ckHueTheaterRestoreLights.AutoSize = true;
      this.ckHueTheaterRestoreLights.Location = new System.Drawing.Point(12, 75);
      this.ckHueTheaterRestoreLights.Name = "ckHueTheaterRestoreLights";
      this.ckHueTheaterRestoreLights.Size = new System.Drawing.Size(201, 17);
      this.ckHueTheaterRestoreLights.TabIndex = 8;
      this.ckHueTheaterRestoreLights.Text = "Restore lights after playback stopped";
      this.ckHueTheaterRestoreLights.UseVisualStyleBackColor = true;
      // 
      // ckHueTheaterEnabled
      // 
      this.ckHueTheaterEnabled.AutoSize = true;
      this.ckHueTheaterEnabled.Checked = true;
      this.ckHueTheaterEnabled.CheckState = System.Windows.Forms.CheckState.Checked;
      this.ckHueTheaterEnabled.Location = new System.Drawing.Point(12, 50);
      this.ckHueTheaterEnabled.Name = "ckHueTheaterEnabled";
      this.ckHueTheaterEnabled.Size = new System.Drawing.Size(124, 17);
      this.ckHueTheaterEnabled.TabIndex = 7;
      this.ckHueTheaterEnabled.Text = "Enable theater mode";
      this.ckHueTheaterEnabled.UseVisualStyleBackColor = true;
      // 
      // grpHueAverageColor
      // 
      this.grpHueAverageColor.Controls.Add(this.cbHueOverallLightness);
      this.grpHueAverageColor.Controls.Add(this.tbHueSaturation);
      this.grpHueAverageColor.Controls.Add(this.lblHueSaturation);
      this.grpHueAverageColor.Controls.Add(this.lblHueBlackThreshold);
      this.grpHueAverageColor.Controls.Add(this.lblHueThreshold);
      this.grpHueAverageColor.Controls.Add(this.lblHueMinDiversion);
      this.grpHueAverageColor.Controls.Add(this.tbHueBlackThreshold);
      this.grpHueAverageColor.Controls.Add(this.tbHueThreshold);
      this.grpHueAverageColor.Controls.Add(this.tbHueMinDiversion);
      this.grpHueAverageColor.Location = new System.Drawing.Point(10, 345);
      this.grpHueAverageColor.Name = "grpHueAverageColor";
      this.grpHueAverageColor.Size = new System.Drawing.Size(820, 100);
      this.grpHueAverageColor.TabIndex = 28;
      this.grpHueAverageColor.TabStop = false;
      this.grpHueAverageColor.Text = "Average color settings";
      // 
      // cbHueOverallLightness
      // 
      this.cbHueOverallLightness.AutoSize = true;
      this.cbHueOverallLightness.Location = new System.Drawing.Point(352, 50);
      this.cbHueOverallLightness.Name = "cbHueOverallLightness";
      this.cbHueOverallLightness.Size = new System.Drawing.Size(123, 17);
      this.cbHueOverallLightness.TabIndex = 17;
      this.cbHueOverallLightness.Text = "Use overall lightness";
      this.cbHueOverallLightness.UseVisualStyleBackColor = true;
      // 
      // tbHueSaturation
      // 
      this.tbHueSaturation.Location = new System.Drawing.Point(505, 22);
      this.tbHueSaturation.Name = "tbHueSaturation";
      this.tbHueSaturation.Size = new System.Drawing.Size(100, 20);
      this.tbHueSaturation.TabIndex = 16;
      this.tbHueSaturation.Validating += new System.ComponentModel.CancelEventHandler(this.tbHueSaturation_Validating);
      // 
      // lblHueSaturation
      // 
      this.lblHueSaturation.AutoSize = true;
      this.lblHueSaturation.Location = new System.Drawing.Point(350, 25);
      this.lblHueSaturation.Name = "lblHueSaturation";
      this.lblHueSaturation.Size = new System.Drawing.Size(108, 13);
      this.lblHueSaturation.TabIndex = 15;
      this.lblHueSaturation.Text = "Saturation correction:";
      // 
      // lblHueBlackThreshold
      // 
      this.lblHueBlackThreshold.AutoSize = true;
      this.lblHueBlackThreshold.Location = new System.Drawing.Point(10, 75);
      this.lblHueBlackThreshold.Name = "lblHueBlackThreshold";
      this.lblHueBlackThreshold.Size = new System.Drawing.Size(83, 13);
      this.lblHueBlackThreshold.TabIndex = 14;
      this.lblHueBlackThreshold.Text = "Black threshold:";
      // 
      // lblHueThreshold
      // 
      this.lblHueThreshold.AutoSize = true;
      this.lblHueThreshold.Location = new System.Drawing.Point(10, 50);
      this.lblHueThreshold.Name = "lblHueThreshold";
      this.lblHueThreshold.Size = new System.Drawing.Size(54, 13);
      this.lblHueThreshold.TabIndex = 13;
      this.lblHueThreshold.Text = "Threshold";
      // 
      // lblHueMinDiversion
      // 
      this.lblHueMinDiversion.AutoSize = true;
      this.lblHueMinDiversion.Location = new System.Drawing.Point(10, 25);
      this.lblHueMinDiversion.Name = "lblHueMinDiversion";
      this.lblHueMinDiversion.Size = new System.Drawing.Size(101, 13);
      this.lblHueMinDiversion.TabIndex = 12;
      this.lblHueMinDiversion.Text = "Min. color diversion:";
      // 
      // tbHueBlackThreshold
      // 
      this.tbHueBlackThreshold.Location = new System.Drawing.Point(160, 72);
      this.tbHueBlackThreshold.Name = "tbHueBlackThreshold";
      this.tbHueBlackThreshold.Size = new System.Drawing.Size(100, 20);
      this.tbHueBlackThreshold.TabIndex = 11;
      this.tbHueBlackThreshold.Validating += new System.ComponentModel.CancelEventHandler(this.tbHueBlackThreshold_Validating);
      // 
      // tbHueThreshold
      // 
      this.tbHueThreshold.Location = new System.Drawing.Point(160, 47);
      this.tbHueThreshold.Name = "tbHueThreshold";
      this.tbHueThreshold.Size = new System.Drawing.Size(100, 20);
      this.tbHueThreshold.TabIndex = 10;
      this.tbHueThreshold.Validating += new System.ComponentModel.CancelEventHandler(this.tbHueThreshold_Validating);
      // 
      // tbHueMinDiversion
      // 
      this.tbHueMinDiversion.Location = new System.Drawing.Point(160, 22);
      this.tbHueMinDiversion.Name = "tbHueMinDiversion";
      this.tbHueMinDiversion.Size = new System.Drawing.Size(100, 20);
      this.tbHueMinDiversion.TabIndex = 9;
      this.tbHueMinDiversion.Validating += new System.ComponentModel.CancelEventHandler(this.tbHueMinDiversion_Validating);
      // 
      // grpHueGeneralSettings
      // 
      this.grpHueGeneralSettings.Controls.Add(this.ckHueBridgeDisableOnSuspend);
      this.grpHueGeneralSettings.Controls.Add(this.ckHueBridgeEnableOnResume);
      this.grpHueGeneralSettings.Controls.Add(this.ckhueIsRemoteMachine);
      this.grpHueGeneralSettings.Controls.Add(this.lblPathInfoHue);
      this.grpHueGeneralSettings.Controls.Add(this.btnSelectFileHue);
      this.grpHueGeneralSettings.Controls.Add(this.edFileHue);
      this.grpHueGeneralSettings.Controls.Add(this.ckStartHue);
      this.grpHueGeneralSettings.Location = new System.Drawing.Point(10, 10);
      this.grpHueGeneralSettings.Name = "grpHueGeneralSettings";
      this.grpHueGeneralSettings.Size = new System.Drawing.Size(820, 175);
      this.grpHueGeneralSettings.TabIndex = 27;
      this.grpHueGeneralSettings.TabStop = false;
      this.grpHueGeneralSettings.Text = "Settings";
      // 
      // ckHueBridgeDisableOnSuspend
      // 
      this.ckHueBridgeDisableOnSuspend.AutoSize = true;
      this.ckHueBridgeDisableOnSuspend.Location = new System.Drawing.Point(12, 125);
      this.ckHueBridgeDisableOnSuspend.Name = "ckHueBridgeDisableOnSuspend";
      this.ckHueBridgeDisableOnSuspend.Size = new System.Drawing.Size(177, 17);
      this.ckHueBridgeDisableOnSuspend.TabIndex = 6;
      this.ckHueBridgeDisableOnSuspend.Text = "Turn off Hue Bridge on suspend\r\n";
      this.ckHueBridgeDisableOnSuspend.UseVisualStyleBackColor = true;
      // 
      // ckHueBridgeEnableOnResume
      // 
      this.ckHueBridgeEnableOnResume.AutoSize = true;
      this.ckHueBridgeEnableOnResume.Location = new System.Drawing.Point(12, 100);
      this.ckHueBridgeEnableOnResume.Name = "ckHueBridgeEnableOnResume";
      this.ckHueBridgeEnableOnResume.Size = new System.Drawing.Size(246, 17);
      this.ckHueBridgeEnableOnResume.TabIndex = 5;
      this.ckHueBridgeEnableOnResume.Text = "Turn on Hue Bridge upon resume from standby";
      this.ckHueBridgeEnableOnResume.UseVisualStyleBackColor = true;
      // 
      // ckhueIsRemoteMachine
      // 
      this.ckhueIsRemoteMachine.AutoSize = true;
      this.ckhueIsRemoteMachine.Location = new System.Drawing.Point(12, 150);
      this.ckhueIsRemoteMachine.Name = "ckhueIsRemoteMachine";
      this.ckhueIsRemoteMachine.Size = new System.Drawing.Size(281, 17);
      this.ckhueIsRemoteMachine.TabIndex = 4;
      this.ckhueIsRemoteMachine.Text = "AtmoHue is on remote machine (skips file/path check)\r\n";
      this.ckhueIsRemoteMachine.UseVisualStyleBackColor = true;
      // 
      // lblPathInfoHue
      // 
      this.lblPathInfoHue.AutoSize = true;
      this.lblPathInfoHue.Location = new System.Drawing.Point(10, 25);
      this.lblPathInfoHue.Name = "lblPathInfoHue";
      this.lblPathInfoHue.Size = new System.Drawing.Size(156, 13);
      this.lblPathInfoHue.TabIndex = 0;
      this.lblPathInfoHue.Text = "Path+Filename of AtmoHue.exe\r\n";
      // 
      // btnSelectFileHue
      // 
      this.btnSelectFileHue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnSelectFileHue.Location = new System.Drawing.Point(723, 41);
      this.btnSelectFileHue.Name = "btnSelectFileHue";
      this.btnSelectFileHue.Size = new System.Drawing.Size(85, 23);
      this.btnSelectFileHue.TabIndex = 2;
      this.btnSelectFileHue.Text = "...";
      this.btnSelectFileHue.UseVisualStyleBackColor = true;
      this.btnSelectFileHue.Click += new System.EventHandler(this.btnSelectFileHue_Click);
      // 
      // edFileHue
      // 
      this.edFileHue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.edFileHue.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
      this.edFileHue.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystem;
      this.edFileHue.Location = new System.Drawing.Point(12, 43);
      this.edFileHue.Name = "edFileHue";
      this.edFileHue.Size = new System.Drawing.Size(698, 20);
      this.edFileHue.TabIndex = 1;
      // 
      // ckStartHue
      // 
      this.ckStartHue.AutoSize = true;
      this.ckStartHue.Checked = true;
      this.ckStartHue.CheckState = System.Windows.Forms.CheckState.Checked;
      this.ckStartHue.Location = new System.Drawing.Point(12, 75);
      this.ckStartHue.Name = "ckStartHue";
      this.ckStartHue.Size = new System.Drawing.Size(176, 17);
      this.ckStartHue.TabIndex = 3;
      this.ckStartHue.Text = "Start AtmoHue with MediaPortal";
      this.ckStartHue.UseVisualStyleBackColor = true;
      // 
      // grpHueNetworkSettings
      // 
      this.grpHueNetworkSettings.Controls.Add(this.tbHueReconnectAttempts);
      this.grpHueNetworkSettings.Controls.Add(this.lblHueReconnectAttempts);
      this.grpHueNetworkSettings.Controls.Add(this.tbHueReconnectDelay);
      this.grpHueNetworkSettings.Controls.Add(this.lblHueReconnectDelay);
      this.grpHueNetworkSettings.Controls.Add(this.lblHintHue);
      this.grpHueNetworkSettings.Controls.Add(this.tbHuePort);
      this.grpHueNetworkSettings.Controls.Add(this.lblHuePort);
      this.grpHueNetworkSettings.Controls.Add(this.tbHueIP);
      this.grpHueNetworkSettings.Controls.Add(this.lblHueIP);
      this.grpHueNetworkSettings.Location = new System.Drawing.Point(10, 190);
      this.grpHueNetworkSettings.Name = "grpHueNetworkSettings";
      this.grpHueNetworkSettings.Size = new System.Drawing.Size(820, 150);
      this.grpHueNetworkSettings.TabIndex = 0;
      this.grpHueNetworkSettings.TabStop = false;
      this.grpHueNetworkSettings.Text = "Network";
      // 
      // tbHueReconnectAttempts
      // 
      this.tbHueReconnectAttempts.Location = new System.Drawing.Point(230, 97);
      this.tbHueReconnectAttempts.Name = "tbHueReconnectAttempts";
      this.tbHueReconnectAttempts.Size = new System.Drawing.Size(93, 20);
      this.tbHueReconnectAttempts.TabIndex = 26;
      this.tbHueReconnectAttempts.Validating += new System.ComponentModel.CancelEventHandler(this.tbHueReconnectAttempts_Validating);
      // 
      // lblHueReconnectAttempts
      // 
      this.lblHueReconnectAttempts.AutoSize = true;
      this.lblHueReconnectAttempts.Location = new System.Drawing.Point(10, 100);
      this.lblHueReconnectAttempts.Name = "lblHueReconnectAttempts";
      this.lblHueReconnectAttempts.Size = new System.Drawing.Size(106, 13);
      this.lblHueReconnectAttempts.TabIndex = 27;
      this.lblHueReconnectAttempts.Text = "Reconnect attempts:";
      // 
      // tbHueReconnectDelay
      // 
      this.tbHueReconnectDelay.Location = new System.Drawing.Point(230, 72);
      this.tbHueReconnectDelay.Name = "tbHueReconnectDelay";
      this.tbHueReconnectDelay.Size = new System.Drawing.Size(93, 20);
      this.tbHueReconnectDelay.TabIndex = 24;
      this.tbHueReconnectDelay.Validating += new System.ComponentModel.CancelEventHandler(this.tbHueReconnectDelay_Validating);
      // 
      // lblHueReconnectDelay
      // 
      this.lblHueReconnectDelay.AutoSize = true;
      this.lblHueReconnectDelay.Location = new System.Drawing.Point(10, 75);
      this.lblHueReconnectDelay.Name = "lblHueReconnectDelay";
      this.lblHueReconnectDelay.Size = new System.Drawing.Size(113, 13);
      this.lblHueReconnectDelay.TabIndex = 25;
      this.lblHueReconnectDelay.Text = "Reconnect delay (ms):";
      // 
      // lblHintHue
      // 
      this.lblHintHue.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblHintHue.Location = new System.Drawing.Point(10, 125);
      this.lblHintHue.Name = "lblHintHue";
      this.lblHintHue.Size = new System.Drawing.Size(796, 13);
      this.lblHintHue.TabIndex = 21;
      this.lblHintHue.Text = "Hint: requires AtmoHue to be running on the above IP and Port to function, do not" +
    " enter your Hue bridge information here\r\n";
      // 
      // tbHuePort
      // 
      this.tbHuePort.Location = new System.Drawing.Point(230, 47);
      this.tbHuePort.Name = "tbHuePort";
      this.tbHuePort.Size = new System.Drawing.Size(93, 20);
      this.tbHuePort.TabIndex = 6;
      this.tbHuePort.Validating += new System.ComponentModel.CancelEventHandler(this.tbHuePort_Validating);
      // 
      // lblHuePort
      // 
      this.lblHuePort.AutoSize = true;
      this.lblHuePort.Location = new System.Drawing.Point(10, 50);
      this.lblHuePort.Name = "lblHuePort";
      this.lblHuePort.Size = new System.Drawing.Size(29, 13);
      this.lblHuePort.TabIndex = 7;
      this.lblHuePort.Text = "Port:";
      // 
      // tbHueIP
      // 
      this.tbHueIP.Location = new System.Drawing.Point(230, 22);
      this.tbHueIP.Name = "tbHueIP";
      this.tbHueIP.Size = new System.Drawing.Size(93, 20);
      this.tbHueIP.TabIndex = 4;
      this.tbHueIP.Validating += new System.ComponentModel.CancelEventHandler(this.tbHueIP_Validating);
      // 
      // lblHueIP
      // 
      this.lblHueIP.AutoSize = true;
      this.lblHueIP.Location = new System.Drawing.Point(10, 25);
      this.lblHueIP.Name = "lblHueIP";
      this.lblHueIP.Size = new System.Drawing.Size(20, 13);
      this.lblHueIP.TabIndex = 5;
      this.lblHueIP.Text = "IP:";
      // 
      // openFileDialog4
      // 
      this.openFileDialog4.Filter = "AtmoHue.exe|*.exe";
      // 
      // openFileDialog5
      // 
      this.openFileDialog5.Filter = "AmbiBox.exe|*.exe";
      // 
      // lblAtmoOrbSmoothingThreshold
      // 
      this.lblAtmoOrbSmoothingThreshold.AutoSize = true;
      this.lblAtmoOrbSmoothingThreshold.Location = new System.Drawing.Point(350, 100);
      this.lblAtmoOrbSmoothingThreshold.Name = "lblAtmoOrbSmoothingThreshold";
      this.lblAtmoOrbSmoothingThreshold.Size = new System.Drawing.Size(110, 13);
      this.lblAtmoOrbSmoothingThreshold.TabIndex = 22;
      this.lblAtmoOrbSmoothingThreshold.Text = "Smoothing Threshold:";
      // 
      // tbAtmoOrbSmoothingThreshold
      // 
      this.tbAtmoOrbSmoothingThreshold.Location = new System.Drawing.Point(500, 97);
      this.tbAtmoOrbSmoothingThreshold.Name = "tbAtmoOrbSmoothingThreshold";
      this.tbAtmoOrbSmoothingThreshold.Size = new System.Drawing.Size(100, 20);
      this.tbAtmoOrbSmoothingThreshold.TabIndex = 23;
      // 
      // SetupForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(872, 662);
      this.Controls.Add(this.tabMenu);
      this.Controls.Add(this.btnLanguage);
      this.Controls.Add(this.btnCancel);
      this.Controls.Add(this.lblVersionVal);
      this.Controls.Add(this.lblVersion);
      this.Controls.Add(this.btnSave);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MaximizeBox = false;
      this.MaximumSize = new System.Drawing.Size(888, 700);
      this.MinimumSize = new System.Drawing.Size(888, 700);
      this.Name = "SetupForm";
      this.Text = "AtmoLight Setup";
      this.tabPageHyperion.ResumeLayout(false);
      this.grpHyperionPrioritySettings.ResumeLayout(false);
      this.grpHyperionPrioritySettings.PerformLayout();
      this.grpHyperionNetworkSettings.ResumeLayout(false);
      this.grpHyperionNetworkSettings.PerformLayout();
      this.tabPageAtmowin.ResumeLayout(false);
      this.grpAtmowinSettings.ResumeLayout(false);
      this.grpAtmowinSettings.PerformLayout();
      this.grpAtmowinWakeHelper.ResumeLayout(false);
      this.grpAtmowinWakeHelper.PerformLayout();
      this.tabPageGeneric.ResumeLayout(false);
      this.grpTargets.ResumeLayout(false);
      this.grpTargets.PerformLayout();
      this.grpMode.ResumeLayout(false);
      this.grpMode.PerformLayout();
      this.grpAdvancedOptions.ResumeLayout(false);
      this.grpAdvancedOptions.PerformLayout();
      this.grpVUMeter.ResumeLayout(false);
      this.grpVUMeter.PerformLayout();
      this.grpGIF.ResumeLayout(false);
      this.grpGIF.PerformLayout();
      this.grpStaticColor.ResumeLayout(false);
      this.grpStaticColor.PerformLayout();
      this.grpPluginOption.ResumeLayout(false);
      this.grpPluginOption.PerformLayout();
      this.grpCaptureDimensions.ResumeLayout(false);
      this.grpCaptureDimensions.PerformLayout();
      this.grpDeactivate.ResumeLayout(false);
      this.grpDeactivate.PerformLayout();
      this.tabMenu.ResumeLayout(false);
      this.tabPageAmbiBox.ResumeLayout(false);
      this.grpAmbiBoxNetwork.ResumeLayout(false);
      this.grpAmbiBoxNetwork.PerformLayout();
      this.grpAmbiBoxLocal.ResumeLayout(false);
      this.grpAmbiBoxLocal.PerformLayout();
      this.tabPageAtmoOrb.ResumeLayout(false);
      this.grpAtmoOrbLamps.ResumeLayout(false);
      this.grpAtmoOrbLamps.PerformLayout();
      this.grpAtmoOrbBasicSettings.ResumeLayout(false);
      this.grpAtmoOrbBasicSettings.PerformLayout();
      this.tabPageBoblight.ResumeLayout(false);
      this.grpBoblightSettings.ResumeLayout(false);
      this.grpBoblightSettings.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.tbarBoblightGamma)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.tbarBoblightThreshold)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.tbarBoblightValue)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.tbarBoblightSaturation)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.tbarBoblightAutospeed)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.tbarBoblightSpeed)).EndInit();
      this.grpBoblightGeneral.ResumeLayout(false);
      this.grpBoblightGeneral.PerformLayout();
      this.tabPageHue.ResumeLayout(false);
      this.grpHueTheaterMode.ResumeLayout(false);
      this.grpHueTheaterMode.PerformLayout();
      this.grpHueAverageColor.ResumeLayout(false);
      this.grpHueAverageColor.PerformLayout();
      this.grpHueGeneralSettings.ResumeLayout(false);
      this.grpHueGeneralSettings.PerformLayout();
      this.grpHueNetworkSettings.ResumeLayout(false);
      this.grpHueNetworkSettings.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button btnSave;
    private System.Windows.Forms.Button btnCancel;
    private System.Windows.Forms.OpenFileDialog openFileDialog1;
    private System.Windows.Forms.Label lblVersion;
    private System.Windows.Forms.Label lblVersionVal;
    private System.Windows.Forms.Button btnLanguage;
    private System.Windows.Forms.OpenFileDialog openFileDialog2;
    private System.Windows.Forms.OpenFileDialog openFileDialog3;
    private System.Windows.Forms.TabPage tabPageHyperion;
    private System.Windows.Forms.GroupBox grpHyperionPrioritySettings;
    private System.Windows.Forms.Label lblHyperionPriorityStaticColor;
    private System.Windows.Forms.TextBox tbHyperionPriorityStaticColor;
    private System.Windows.Forms.Label lblHyperionPriority;
    private System.Windows.Forms.TextBox tbHyperionPriority;
    private System.Windows.Forms.GroupBox grpHyperionNetworkSettings;
    private System.Windows.Forms.CheckBox ckHyperionLiveReconnect;
    private System.Windows.Forms.TextBox tbHyperionReconnectAttempts;
    private System.Windows.Forms.Label lblHyperionReconnectAttempts;
    private System.Windows.Forms.TextBox tbHyperionReconnectDelay;
    private System.Windows.Forms.Label lblHyperionReconnectDelay;
    private System.Windows.Forms.TextBox tbHyperionIP;
    private System.Windows.Forms.Label lblHyperionIP;
    private System.Windows.Forms.TextBox tbHyperionPort;
    private System.Windows.Forms.Label lblHyperionPort;
    private System.Windows.Forms.TabPage tabPageAtmowin;
    private System.Windows.Forms.GroupBox grpAtmowinSettings;
    private System.Windows.Forms.Label lblPathInfoAtmoWin;
    private System.Windows.Forms.Button btnSelectFileAtmoWin;
    private System.Windows.Forms.TextBox edFileAtmoWin;
    private System.Windows.Forms.CheckBox ckExitAtmoWin;
    private System.Windows.Forms.CheckBox ckStartAtmoWin;
    private System.Windows.Forms.TabPage tabPageGeneric;
    private System.Windows.Forms.GroupBox grpTargets;
    private System.Windows.Forms.CheckBox ckHueEnabled;
    private System.Windows.Forms.Label lblHintHardware;
    private System.Windows.Forms.CheckBox ckAtmowinEnabled;
    private System.Windows.Forms.CheckBox ckHyperionEnabled;
    private System.Windows.Forms.GroupBox grpMode;
    private System.Windows.Forms.ComboBox cbMPExit;
    private System.Windows.Forms.Label lblMPExit;
    private System.Windows.Forms.GroupBox grpGIF;
    private System.Windows.Forms.Button btnSelectGIF;
    private System.Windows.Forms.TextBox tbGIF;
    private System.Windows.Forms.GroupBox grpStaticColor;
    private System.Windows.Forms.Label lblRed;
    private System.Windows.Forms.TextBox tbGreen;
    private System.Windows.Forms.TextBox tbBlue;
    private System.Windows.Forms.TextBox tbRed;
    private System.Windows.Forms.Label lblGreen;
    private System.Windows.Forms.Label lblBlue;
    private System.Windows.Forms.Label lblMenu;
    private System.Windows.Forms.ComboBox cbMenu;
    private System.Windows.Forms.ComboBox cbRadio;
    private System.Windows.Forms.Label lblRadio;
    private System.Windows.Forms.ComboBox cbMusic;
    private System.Windows.Forms.Label lblMusic;
    private System.Windows.Forms.ComboBox cbVideo;
    private System.Windows.Forms.Label lblVidTvRec;
    private System.Windows.Forms.GroupBox grpPluginOption;
    private System.Windows.Forms.CheckBox ckRestartOnError;
    private System.Windows.Forms.CheckBox ckTrueGrabbing;
    private System.Windows.Forms.GroupBox grpCaptureDimensions;
    private System.Windows.Forms.Label lblHintCaptureDimensions;
    private System.Windows.Forms.Label lblCaptureHeight;
    private System.Windows.Forms.Label lblCaptureWidth;
    private System.Windows.Forms.TextBox tbCaptureWidth;
    private System.Windows.Forms.TextBox tbCaptureHeight;
    private System.Windows.Forms.Label lblBlackarDetectionMS;
    private System.Windows.Forms.GroupBox grpDeactivate;
    private System.Windows.Forms.TextBox edExcludeEnd;
    private System.Windows.Forms.Label lblEnd;
    private System.Windows.Forms.TextBox edExcludeStart;
    private System.Windows.Forms.Label lblStart;
    private System.Windows.Forms.TextBox tbBlackbarDetectionTime;
    private System.Windows.Forms.Label lblHintMenuButtons;
    private System.Windows.Forms.CheckBox ckBlackbarDetection;
    private System.Windows.Forms.Label lblRefreshRate;
    private System.Windows.Forms.TextBox tbRefreshRate;
    private System.Windows.Forms.Label lblDelay;
    private System.Windows.Forms.TextBox tbDelay;
    private System.Windows.Forms.CheckBox ckDelay;
    private System.Windows.Forms.Label lblMenuButton;
    private System.Windows.Forms.ComboBox cbMenuButton;
    private System.Windows.Forms.Label lblProfile;
    private System.Windows.Forms.ComboBox comboBox2;
    private System.Windows.Forms.Label lblFrames;
    private System.Windows.Forms.TextBox lowCpuTime;
    private System.Windows.Forms.CheckBox ckLowCpu;
    private System.Windows.Forms.CheckBox ckOnMediaStart;
    private System.Windows.Forms.ComboBox comboBox1;
    private System.Windows.Forms.Label lblLedsOnOff;
    private System.Windows.Forms.TabControl tabMenu;
    private System.Windows.Forms.TabPage tabPageHue;
    private System.Windows.Forms.GroupBox grpHueNetworkSettings;
    private System.Windows.Forms.TextBox tbHueIP;
    private System.Windows.Forms.Label lblHueIP;
    private System.Windows.Forms.TextBox tbHuePort;
    private System.Windows.Forms.Label lblHuePort;
    private System.Windows.Forms.Label lblHintHue;
    private System.Windows.Forms.TabPage tabPageBoblight;
    private System.Windows.Forms.CheckBox ckBoblightEnabled;
    private System.Windows.Forms.GroupBox grpBoblightGeneral;
    private System.Windows.Forms.TextBox tbBoblightMaxFPS;
    private System.Windows.Forms.TextBox tbBoblightReconnectDelay;
    private System.Windows.Forms.TextBox tbBoblightMaxReconnectAttempts;
    private System.Windows.Forms.TextBox tbBoblightPort;
    private System.Windows.Forms.TextBox tbBoblightIP;
    private System.Windows.Forms.Label lblBoblightMaxFPS;
    private System.Windows.Forms.Label lblBoblightReconnectDelay;
    private System.Windows.Forms.Label lblBoblightMaxReconnectAttempts;
    private System.Windows.Forms.Label lblBoblightPort;
    private System.Windows.Forms.Label lblBoblightIP;
    private System.Windows.Forms.GroupBox grpHueGeneralSettings;
    private System.Windows.Forms.Label lblPathInfoHue;
    private System.Windows.Forms.Button btnSelectFileHue;
    private System.Windows.Forms.TextBox edFileHue;
    private System.Windows.Forms.CheckBox ckStartHue;
    private System.Windows.Forms.CheckBox ckhueIsRemoteMachine;
    private System.Windows.Forms.OpenFileDialog openFileDialog4;
    private System.Windows.Forms.GroupBox grpBoblightSettings;
    private System.Windows.Forms.TextBox tbBoblightThreshold;
    private System.Windows.Forms.TextBox tbBoblightValue;
    private System.Windows.Forms.TextBox tbBoblightSaturation;
    private System.Windows.Forms.TextBox tbBoblightAutospeed;
    private System.Windows.Forms.TextBox tbBoblightSpeed;
    private System.Windows.Forms.CheckBox ckBoblightInterpolation;
    private System.Windows.Forms.Label lblBoblightThreshold;
    private System.Windows.Forms.Label lblBoblightValue;
    private System.Windows.Forms.Label lblBoblightSaturation;
    private System.Windows.Forms.Label lblBoblightAutospeed;
    private System.Windows.Forms.Label lblBoblightSpeed;
    private System.Windows.Forms.TrackBar tbarBoblightThreshold;
    private System.Windows.Forms.TrackBar tbarBoblightValue;
    private System.Windows.Forms.TrackBar tbarBoblightSaturation;
    private System.Windows.Forms.TrackBar tbarBoblightAutospeed;
    private System.Windows.Forms.TrackBar tbarBoblightSpeed;
    private System.Windows.Forms.TextBox tbBoblightGamma;
    private System.Windows.Forms.TrackBar tbarBoblightGamma;
    private System.Windows.Forms.Label lblBoblightGamma;
    private System.Windows.Forms.TextBox tbHueReconnectAttempts;
    private System.Windows.Forms.Label lblHueReconnectAttempts;
    private System.Windows.Forms.TextBox tbHueReconnectDelay;
    private System.Windows.Forms.Label lblHueReconnectDelay;
    private System.Windows.Forms.TextBox tbBlackbarDetectionThreshold;
    private System.Windows.Forms.CheckBox ckHueBridgeEnableOnResume;
    private System.Windows.Forms.CheckBox ckHueBridgeDisableOnSuspend;
    private System.Windows.Forms.Label lblpowerModeChangedDelayMS;
    private System.Windows.Forms.Label lblpowerModeChangedDelay;
    private System.Windows.Forms.TextBox tbpowerModeChangedDelay;
    private System.Windows.Forms.CheckBox ckAmbiBoxEnabled;
    private System.Windows.Forms.TabPage tabPageAmbiBox;
    private System.Windows.Forms.GroupBox grpAmbiBoxNetwork;
    private System.Windows.Forms.GroupBox grpAmbiBoxLocal;
    private System.Windows.Forms.CheckBox cbAmbiBoxAutoStop;
    private System.Windows.Forms.CheckBox cbAmbiBoxAutoStart;
    private System.Windows.Forms.Label lblAmbiBoxPath;
    private System.Windows.Forms.TextBox tbAmbiBoxPath;
    private System.Windows.Forms.Button btnSelectFileAmbiBox;
    private System.Windows.Forms.OpenFileDialog openFileDialog5;
    private System.Windows.Forms.Label lblAmbiBoxReconnectDelay;
    private System.Windows.Forms.Label lblAmbiBoxMaxReconnectAttempts;
    private System.Windows.Forms.Label lblAmbiBoxPort;
    private System.Windows.Forms.Label lblAmbiBoxIP;
    private System.Windows.Forms.TextBox tbAmbiBoxExternalProfile;
    private System.Windows.Forms.TextBox tbAmbiBoxMediaPortalProfile;
    private System.Windows.Forms.TextBox tbAmbiBoxReconnectDelay;
    private System.Windows.Forms.TextBox tbAmbiBoxMaxReconnectAttempts;
    private System.Windows.Forms.TextBox tbAmbiBoxPort;
    private System.Windows.Forms.TextBox tbAmbiBoxIP;
    private System.Windows.Forms.Label lblAmbiBoxExternalProfile;
    private System.Windows.Forms.TabPage tabPageAtmoOrb;
    private System.Windows.Forms.GroupBox grpAtmoOrbLamps;
    private System.Windows.Forms.ListBox lbAtmoOrbLamps;
    private System.Windows.Forms.GroupBox grpAtmoOrbBasicSettings;
    private System.Windows.Forms.CheckBox cbAtmoOrbUseOverallLightness;
    private System.Windows.Forms.Label lblAtmoOrbBlackThreshold;
    private System.Windows.Forms.Label lblAtmoOrbThreshold;
    private System.Windows.Forms.Label lblAtmoOrbMinDiversion;
    private System.Windows.Forms.Label lblAtmoOrbBroadcastPort;
    private System.Windows.Forms.TextBox tbAtmoOrbBlackThreshold;
    private System.Windows.Forms.TextBox tbAtmoOrbThreshold;
    private System.Windows.Forms.TextBox tbAtmoOrbMinDiversion;
    private System.Windows.Forms.TextBox tbAtmoOrbBroadcastPort;
    private System.Windows.Forms.TextBox tbAtmoOrbGamma;
    private System.Windows.Forms.TextBox tbAtmoOrbSaturation;
    private System.Windows.Forms.Label lblAtmoOrbGamma;
    private System.Windows.Forms.Label lblAtmoOrbSaturation;
    private System.Windows.Forms.TextBox tbAtmoOrbID;
    private System.Windows.Forms.Label lblAtmoOrbID;
    private System.Windows.Forms.Button btnAtmoOrbRemove;
    private System.Windows.Forms.Button btnAtmoOrbUpdate;
    private System.Windows.Forms.Button btnAtmoOrbAdd;
    private System.Windows.Forms.Label lblAtmoOrbVScanTo;
    private System.Windows.Forms.Label lblAtmoOrbHScanTo;
    private System.Windows.Forms.Label lblAtmoOrbConnection;
    private System.Windows.Forms.CheckBox cbAtmoOrbInvertZone;
    private System.Windows.Forms.TextBox tbAtmoOrbVScanEnd;
    private System.Windows.Forms.TextBox tbAtmoOrbVScanStart;
    private System.Windows.Forms.Label lblAtmoOrbVScan;
    private System.Windows.Forms.TextBox tbAtmoOrbHScanEnd;
    private System.Windows.Forms.TextBox tbAtmoOrbHScanStart;
    private System.Windows.Forms.Label lblAtmoOrbHScan;
    private System.Windows.Forms.TextBox tbAtmoOrbPort;
    private System.Windows.Forms.Label lblAtmoOrbPort;
    private System.Windows.Forms.TextBox tbAtmoOrbIP;
    private System.Windows.Forms.Label lblAtmoOrbIP;
    private System.Windows.Forms.RadioButton rbAtmoOrbUDP;
    private System.Windows.Forms.RadioButton rbAtmoOrbTCP;
    private System.Windows.Forms.CheckBox ckAtmoOrbEnabled;
    private System.Windows.Forms.GroupBox grpVUMeter;
    private System.Windows.Forms.Label lblVUMeterMaxHue;
    private System.Windows.Forms.Label lblVUMeterMinHue;
    private System.Windows.Forms.TextBox tbVUMeterMindB;
    private System.Windows.Forms.Label lblVUMeterMindB;
    private System.Windows.Forms.TextBox tbVUMeterMaxHue;
    private System.Windows.Forms.TextBox tbVUMeterMinHue;
    private System.Windows.Forms.GroupBox grpHueAverageColor;
    private System.Windows.Forms.Label lblHueBlackThreshold;
    private System.Windows.Forms.Label lblHueThreshold;
    private System.Windows.Forms.Label lblHueMinDiversion;
    private System.Windows.Forms.TextBox tbHueBlackThreshold;
    private System.Windows.Forms.TextBox tbHueThreshold;
    private System.Windows.Forms.TextBox tbHueMinDiversion;
    private System.Windows.Forms.CheckBox cbHueOverallLightness;
    private System.Windows.Forms.TextBox tbHueSaturation;
    private System.Windows.Forms.Label lblHueSaturation;
    private System.Windows.Forms.CheckBox ckMonitorScreensaverState;
    private System.Windows.Forms.CheckBox cbBlackbarDetectionVertical;
    private System.Windows.Forms.CheckBox cbBlackbarDetectionHorizontal;
    private System.Windows.Forms.CheckBox cbBlackbarDetectionLinkAreas;
    private System.Windows.Forms.CheckBox ckMonitorWindowState;
    private System.Windows.Forms.Label lblAtmoOrbProtocol;
    private System.Windows.Forms.ComboBox cbAtmoOrbProtocol;
    private System.Windows.Forms.GroupBox grpHueTheaterMode;
    private System.Windows.Forms.CheckBox ckHueTheaterRestoreLights;
    private System.Windows.Forms.CheckBox ckHueTheaterEnabled;
    private System.Windows.Forms.Label lblHintHueTheaterMode;
    private System.Windows.Forms.GroupBox grpAtmowinWakeHelper;
    private System.Windows.Forms.ComboBox cbAtmoWakeHelperComPort;
    private System.Windows.Forms.CheckBox ckAtmoWakeHelperEnabled;
    private System.Windows.Forms.Label lblAtmoWakeHelperComPort;
    private System.Windows.Forms.TextBox tbAtmoWakeHelperResumeDelay;
    private System.Windows.Forms.Label lblAtmoWakeHelperResumeDelay;
    private System.Windows.Forms.Label lblAtmoWakeHelperReinitializationDelay;
    private System.Windows.Forms.TextBox tbAtmoWakeHelperReinitializationDelay;
    private System.Windows.Forms.Label lblAtmoWakeHelperConnectDelay;
    private System.Windows.Forms.TextBox tbAtmoWakeHelperConnectDelay;
    private System.Windows.Forms.Label lblAtmoWakeHelperDisconnectDelay;
    private System.Windows.Forms.TextBox tbAtmoWakeHelperDisconnectDelay;
    private System.Windows.Forms.TextBox tbAtmoOrbLedCount;
    private System.Windows.Forms.Label lblAtmoOrbLedCount;
    private System.Windows.Forms.GroupBox grpAdvancedOptions;
    private System.Windows.Forms.CheckBox cbRemoteApiServer;
    private System.Windows.Forms.CheckBox cbAtmoOrbUseSmoothing;
    private System.Windows.Forms.TextBox tbAmbiboxChangeImageDelay;
    private System.Windows.Forms.Label lblAmbiboxChangeImageDelay;
    private System.Windows.Forms.Label lblAmbiBoxMediaPortalProfile;
    private System.Windows.Forms.TextBox tbAtmoOrbSmoothingThreshold;
    private System.Windows.Forms.Label lblAtmoOrbSmoothingThreshold;
  }
}