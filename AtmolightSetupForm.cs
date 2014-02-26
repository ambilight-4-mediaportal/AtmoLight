using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace MediaPortal.ProcessPlugins.Atmolight
{
  public partial class AtmolightSetupForm : Form
  {
    public AtmolightSetupForm()
    {
      InitializeComponent();

      lblVersionVal.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString();
      edFile.Text = AtmolightSettings.atmowinExe;
      cbVideo.SelectedIndex = (int)AtmolightSettings.effectVideo;
      cbMusic.SelectedIndex = (int)AtmolightSettings.effectMusic;
      cbRadio.SelectedIndex = (int)AtmolightSettings.effectRadio;
      comboBox1.SelectedIndex = (int)AtmolightSettings.killbutton;
      comboBox2.SelectedIndex = (int)AtmolightSettings.cmbutton;
      edExcludeStart.Text = AtmolightSettings.excludeTimeStart.ToString("HH:mm");
      edExcludeEnd.Text = AtmolightSettings.excludeTimeEnd.ToString("HH:mm");
      lowCpuTime.Text = AtmolightSettings.lowCPUTime.ToString();
      
      if (AtmolightSettings.HateTheStopThing)
        checkBox1.Checked = true;
      else
        this.checkBox1.Checked = false;

      if (AtmolightSettings.OffOnStart)
        checkBox2.Checked = true;
      else
        this.checkBox2.Checked = false;
     
      if (AtmolightSettings.lowCPU)
        checkBox3.Checked = true;
      else
        this.checkBox3.Checked = false;

      if (AtmolightSettings.startAtmoWin)
        ckStartAtmoWin.Checked = true;
      else
        ckStartAtmoWin.Checked = false;

      if (AtmolightSettings.exitAtmoWin)
        ckExitAtmoWin.Checked = true;
      else
        ckExitAtmoWin.Checked = false;
      
      if (AtmolightSettings.disableOnShutdown)
        rbDisableLEDs.Checked = true;
      else
        rbSwitchToLiveView.Checked = true;
    }

    private void btnSelectFile_Click(object sender, EventArgs e)
    {
      if (openFileDialog1.ShowDialog() == DialogResult.OK)
      {
        string filenameNoExtension = Path.GetFileNameWithoutExtension(openFileDialog1.FileName);
        string filename = filenameNoExtension.ToLower();
        if (filename == "atmowina")
        {
          edFile.Text = openFileDialog1.FileName;
        }
        else
        {
          MessageBox.Show("You have to enter a invalid File, should be AtmoWinA", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
          edFile.Text = "";
          return;
        }
      }
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
      this.DialogResult = DialogResult.Cancel;
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      DateTime dt;
      if (!DateTime.TryParse(edExcludeStart.Text, out dt))
      {
        MessageBox.Show("You have to enter a valid start time", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }
      
      AtmolightSettings.excludeTimeStart = dt;
      DateTime dt2;
      if (!DateTime.TryParse(edExcludeEnd.Text, out dt2))
      {
        MessageBox.Show("You have to enter a valid end time", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      int cTime;
      if (!int.TryParse(lowCpuTime.Text, out cTime))
      {
        MessageBox.Show("You have to enter a valid number of ms", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      AtmolightSettings.lowCPUTime = cTime;

      AtmolightSettings.excludeTimeEnd = dt2;
      AtmolightSettings.atmowinExe = edFile.Text;
      AtmolightSettings.effectVideo = (ContentEffect)cbVideo.SelectedIndex;
      AtmolightSettings.effectMusic = (ContentEffect)cbMusic.SelectedIndex;
      AtmolightSettings.effectRadio = (ContentEffect)cbRadio.SelectedIndex;
      AtmolightSettings.killbutton = comboBox1.SelectedIndex;
      AtmolightSettings.cmbutton = comboBox2.SelectedIndex;
      AtmolightSettings.disableOnShutdown = rbDisableLEDs.Checked;
      AtmolightSettings.enableInternalLiveView = rbSwitchToLiveView.Checked;
      AtmolightSettings.HateTheStopThing = checkBox1.Checked;
      AtmolightSettings.OffOnStart = checkBox2.Checked;
      AtmolightSettings.lowCPU = checkBox3.Checked;
      AtmolightSettings.startAtmoWin = ckStartAtmoWin.Checked;
      AtmolightSettings.exitAtmoWin = ckExitAtmoWin.Checked;
      AtmolightSettings.SaveSettings();
      this.DialogResult = DialogResult.OK;
    }

  }
}
