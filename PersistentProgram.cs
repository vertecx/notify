using System;
using System.Windows.Forms;

namespace Petr.Notify
{
	internal static class PersistentProgram
	{
		[STAThread]
		private static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
		}
	}
}
