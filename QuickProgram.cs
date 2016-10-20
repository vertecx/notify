using System;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Petr.Notify
{
	internal static class Program
	{
		private static bool Wellbehaved()
		{
#if PERSISTENT
			var release = int.Parse(Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion","ReleaseId", "0").ToString());

			if (release >= 1511)
			{
				return false;
			}
#endif

			return true;
		}

		[STAThread]
		private static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			using (var nc = new NotificationContext())
			{
				// HACK: To show persistent notifications that must be manually dismissed in Windows 10,
				// the application must misbehave by not running the message loop.
				if (Wellbehaved())
				{
					Application.Run(nc);
				}
			}
		}
	}
}
