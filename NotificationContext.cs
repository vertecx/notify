using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Petr.Notify
{
	internal class QuickContext : ApplicationContext
	{
		// The minimum and maximum number of arguments supported.
		private const int MinNumArgs = 1;
		private const int MaxNumArgs = 4;
		// Notification display time is controlled by a Windows setting since Vista. 5 seconds is the default value.
		private const int DisplayTime = 5000;

		// The second or third argument will be compared against the following strings to decide which icon to show.
		private static readonly string[] NoIconStrings = { "0", "N", "NONE" };
		private static readonly string[] InfoIconStrings = { "1", "I", "INFO", "INFORMATION" };
		private static readonly string[] WarningIconStrings = { "2", "W", "WARN", "WARNING" };
		private static readonly string[] ErrorIconStrings = { "3", "E", "ERR", "ERROR" };

		private NotifyIcon ni = new NotifyIcon();
		private string runOnClick = null;

		internal QuickContext()
		{
			var args = Environment.GetCommandLineArgs();
			var numArgs = args.Length - 1;

			// The first argument, args[0], is always the name of the executable. It is of no use to this program.
			// The second argument, args[1], is the first passed by the user.
			switch (numArgs)
			{
				case 1:
					// One argument can only be text.
					ni.BalloonTipText = UnescapeNewline(args[1]);
					break;
				case 2:
					// Two arguments can be either text + icon or title + text.
					if (IsIcon(args[2]))
					{
						ni.BalloonTipText = UnescapeNewline(args[1]);
						ni.BalloonTipIcon = GetIcon(args[2]);
					}
					else
					{
						ni.BalloonTipTitle = args[1];
						ni.BalloonTipText = UnescapeNewline(args[2]);
					}
					break;
				case 3:
					// Three arguments can be combined as text + icon + run, title + text + icon or title + text + run.
					if (IsIcon(args[2]))
					{
						ni.BalloonTipText = UnescapeNewline(args[1]);
						ni.BalloonTipIcon = GetIcon(args[2]);
						runOnClick = args[3].Trim();
					}
					else if (IsIcon(args[3]))
					{
						ni.BalloonTipTitle = args[1];
						ni.BalloonTipText = UnescapeNewline(args[2]);
						ni.BalloonTipIcon = GetIcon(args[3]);
					}
					else
					{
						ni.BalloonTipTitle = args[1];
						ni.BalloonTipText = UnescapeNewline(args[2]);
						runOnClick = args[3].Trim();
					}
					break;
				case 4:
					// Four arguments can only be title + text + icon + run on click.
					ni.BalloonTipTitle = args[1];
					ni.BalloonTipText = UnescapeNewline(args[2]);
					ni.BalloonTipIcon = GetIcon(args[3]);
					runOnClick = args[4].Trim();
					break;
				default:
					// Display a localized error message if the number of arguments is out of range.
					ni.BalloonTipTitle = Localization.GetString(Strings.BadArgumentsTitle);
					ni.BalloonTipText = string.Format(CultureInfo.CurrentCulture, Localization.GetString(Strings.BadArgumentsText), MinNumArgs, MaxNumArgs, numArgs);
					ni.BalloonTipIcon = ToolTipIcon.Error;
					break;
			}

			// Use the icon already embedded in the exe file as the notification area icon.
			// We'll save a few kilobytes by not including it again as a managed resource.
			ni.Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);

			// Hook up event handlers and display the notification.
			ni.BalloonTipClicked += OnBalloonTipClicked;
			ni.BalloonTipClosed += OnBalloonTipClosed;

			ni.Visible = true;
			ni.ShowBalloonTip(DisplayTime);
		}

		protected override void Dispose(bool disposing)
		{
			// Hiding the icon before exiting prevents "dead" icons that disappear once moused over in the notification area.
			ni.Visible = false;
			ni.Dispose();

			base.Dispose(disposing);
		}

		private void OnBalloonTipClicked(object sender, EventArgs e)
		{
			const string SingleQuote = "'";
			const string Space = " ";

			if (!string.IsNullOrEmpty(runOnClick))
			{
				try
				{
					string filename = runOnClick;
					string arguments = string.Empty;

					// Unless the entire run on click argument is a file, it needs to be split into filename and arguments.
					if (!File.Exists(filename))
					{
						if (runOnClick.StartsWith(SingleQuote, StringComparison.Ordinal))
						{
							// The run argument starts with a quote so split filename and arguments on the second (closing) quote.
							filename = runOnClick.Substring(1, runOnClick.IndexOf(SingleQuote, 1, StringComparison.Ordinal) - 1);
							arguments = runOnClick.Substring(filename.Length + 2);
						}
						else if (runOnClick.Contains(Space))
						{
							// Split into filename and arguments on the first space.
							filename = runOnClick.Substring(0, runOnClick.IndexOf(Space, StringComparison.Ordinal));
							arguments = runOnClick.Substring(filename.Length + 1);
						}
					}

					// For some reason the started process sometimes end up behind all other open windows (Seen on Windows 10). Try to bring it to the front.
					BrintToFront(Process.Start(filename, arguments));
				}
				catch (ArgumentOutOfRangeException)
				{
					// Thrown when the run on click argument starts with a quote but has no second quote to split on.
					MessageBox.Show(Localization.GetString(Strings.ArgumentOutOfRangeExceptionText), Localization.GetString(Strings.ClickedNotificationTitle), MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, 0);
				}
				catch (Win32Exception ex)
				{
					// Thrown by Process.Start if filename isn't found.
					MessageBox.Show(string.Format(CultureInfo.CurrentCulture, Localization.GetString(Strings.Win32ExceptionText), ex.Message), Localization.GetString(Strings.ClickedNotificationTitle), MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, 0);
				}
			}

			Application.Exit();
		}

		private void OnBalloonTipClosed(object sender, EventArgs e)
		{
			Application.Exit();
		}

		/// <summary>
		/// Checks if the input string represents an icon.
		/// </summary>
		private static bool IsIcon(string iconText)
		{
			var icon = iconText.ToUpperInvariant();

			if (Array.IndexOf(NoIconStrings, icon) >= 0)
			{
				return true;
			}
			else if (Array.IndexOf(InfoIconStrings, icon) >= 0)
			{
				return true;
			}
			else if (Array.IndexOf(WarningIconStrings, icon) >= 0)
			{
				return true;
			}
			else if (Array.IndexOf(ErrorIconStrings, icon) >= 0)
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Returns the ToolTipIcon associated with the input string.
		/// The input must be verified to be an icon with IsIcon before GetIcon is called.
		/// </summary>
		private static ToolTipIcon GetIcon(string verifiedIconText)
		{
			var icon = verifiedIconText.ToUpperInvariant();

			if (Array.IndexOf(InfoIconStrings, icon) >= 0)
			{
				return ToolTipIcon.Info;
			}
			else if (Array.IndexOf(WarningIconStrings, icon) >= 0)
			{
				return ToolTipIcon.Warning;
			}
			else if (Array.IndexOf(ErrorIconStrings, icon) >= 0)
			{
				return ToolTipIcon.Error;
			}

			return ToolTipIcon.None;
		}

		/// <summary>
		/// Replaces \n and nnnn in <paramref name="text"/> with a environment specific newline character.
		/// </summary>
		private static string UnescapeNewline(string text)
		{
			return text.Replace(@"\n", Environment.NewLine).Replace("nnnn", Environment.NewLine);
		}

		/// <summary>
		/// Tries to bring a process main window to the top and into focus.
		/// </summary>
		private static void BrintToFront(Process process)
		{
			try
			{
				if (!process.HasExited)
				{
					process.WaitForInputIdle(1000);
					NativeMethods.SetForegroundWindow(process.MainWindowHandle);
				}
			}
			catch (InvalidOperationException)
			{
				// Most likely the process has no GUI.
			}
		}
	}
}
