using System;
using System.Windows.Forms;
using Microsoft.VisualBasic.ApplicationServices;

namespace Petr.Notify
{
	internal static class PersistentProgram
	{
		[STAThread]
		private static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			new SingleInstanceController().Run(Environment.GetCommandLineArgs());
		}

		private class SingleInstanceController : WindowsFormsApplicationBase
		{
			public SingleInstanceController()
			{
				IsSingleInstance = true;

				StartupNextInstance += SingleInstanceController_StartupNextInstance;
			}

			protected override void OnCreateMainForm()
			{
				MainForm = new NotifyForm(true);
			}

			private void SingleInstanceController_StartupNextInstance(object sender, StartupNextInstanceEventArgs e)
			{
				(MainForm as NotifyForm).DisplayNotification(e.CommandLine);
			}
		}
	}
}
