// Please add a new CheckBox in the first tab to the other targets.
// Then add a new tab in which you can add all the configuration you need.

using ...;

namespace AtmoLight
{
	public partial class SetupForm : Form
	{
		public SetupForm()
		{
			...
			tbExampleIP.Text = Settings.exampleIP;
			tbExamplePort.Text = Settings.examplePort.ToString();
			...
			ckExampleEnabled.Checked = Settings.exampleTarget;
			...
			
			private void UpdateLanguageOnControls()
			{
				...
				lblExampleIP.Text = LanguageLoader.appStrings.SetupForm_lblExampleIP;
				lblExamplePort.Text = LanguageLoader.appStrings.SetupForm_lblExamplePort;
				...
			}
			
			 private void btnSave_Click(object sender, EventArgs e)
			{
				...
				// Example IP
				if (validatorIPAdress(tbExampleIP.Text) == false)
				{	
					MessageBox.Show(LanguageLoader.appStrings.SetupForm_ErrorInvalidIP + " - [" + lblExampleIP.Text + "]", LanguageLoader.appStrings.SetupForm_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
				...
				// Example port
				minValue = 1;
				maxValue = 65535;
				if (validatorInt(tbExamplePort.Text, minValue, maxValue, true) == false)
				{
					MessageBox.Show(LanguageLoader.appStrings.SetupForm_ErrorInvalidIntegerBetween.Replace("[minInteger]", minValue.ToString()).Replace("[maxInteger]", maxValue.ToString()) + " - [" + lblExamplePort.Text + "]", LanguageLoader.appStrings.SetupForm_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
				...
				Settings.exampleTarget = ckExampleEnabled.Checked;
				Settings.exampleIP = tbExampleIP.Text;
				Settings.examplePort = int.Parse(tbExamplePort.Text);
				...
			}
			
			...
		}
	}
}