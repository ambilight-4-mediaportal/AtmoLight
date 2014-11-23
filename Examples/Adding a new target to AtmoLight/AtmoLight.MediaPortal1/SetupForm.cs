// Please add a new CheckBox in the first tab to the other targets.
// Then add a new tab in which you can add all the configuration you need.
// Don't forget to validate the user input.

using ...;

namespace AtmoLight
{
	public partial class SetupForm : Form
	{
		public SetupForm()
		{
			...
			// Example
			if (Settings.exampleTarget)
			{
				coreObject.AddTarget(Target.Example);
			}
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
			
			// This method has to be triggered when the user leaves the IP textbox.
			// Please add the event handler into SetupForm.Designer.cs.
			private void tbExampleIP_Validating(object sender, System.ComponentModel.CancelEventArgs e)
			{
				if (validatorIPAdress(tbExampleIP.Text) == false)
				{
					MessageBox.Show(LanguageLoader.appStrings.SetupForm_ErrorInvalidIP, LanguageLoader.appStrings.SetupForm_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
			
			// This method has to be triggered when the user leaves the Port textbox.
			// Please add the event handler into SetupForm.Designer.cs.
			private void tbExamplePort_Validating(object sender, System.ComponentModel.CancelEventArgs e)
			{
				int minValue = 1;
				int maxValue = 65535;
				if (validatorInt(tbExamplePort.Text, minValue, maxValue, true) == false)
				{
					MessageBox.Show(LanguageLoader.appStrings.SetupForm_ErrorInvalidIntegerBetween.Replace("[minInteger]", minValue.ToString()).Replace("[maxInteger]", maxValue.ToString()), LanguageLoader.appStrings.SetupForm_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
			
			...
			
			// This method has to be triggered when the user toggles the checkbox for this targets
			// Please add the event handler into SetupForm.Designer.cs.
			private void ckExampleEnabled_CheckedChanged(Object sender, EventArgs e)
			{
				if (ckExampleEnabled.Checked)
				{
					coreObject.AddTarget(Target.Example);
				}
				else
				{
					coreObject.RemoveTarget(Target.Example);
				}
				UpdateComboBoxes();
			}
		}
	}
}