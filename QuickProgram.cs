using System;
using System.Windows.Forms;

namespace Petr.Notify
{
	internal static class QuickProgram
	{
		[STAThread]
		private static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			using (var nc = new QuickContext())
			{
				Application.Run(nc);
			}
		}
	}
}
