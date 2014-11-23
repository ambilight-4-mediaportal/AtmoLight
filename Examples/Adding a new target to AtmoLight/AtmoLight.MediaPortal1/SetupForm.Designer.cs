// Edit this file AFTER you have added the options to the setup form.

namespace AtmoLight
{
  partial class SetupForm
  {
  ...
		private void InitializeComponent()
		{
			...
			// 
			// ckExampleEnabled
			// 
			...
			this.ckExampleEnabled.CheckedChanged += new System.EventHandler(this.ckExampleEnabled_CheckedChanged);
			
			// 
			// tbExampleIP
			// 
			...
			this.tbExampleIP.Validating += new System.ComponentModel.CancelEventHandler(this.tbExampleIP_Validating);

			...
			// 
			// tbExamplePort
			// 
			...
			this.tbExamplePort.Validating += new System.ComponentModel.CancelEventHandler(this.tbExamplePort_Validating);
			
			...
		}
	}
}