using System.Globalization;

namespace Petr.Notify
{
	internal enum Strings
	{
		BadArgumentsTitle,
		BadArgumentsText,
		ClickedNotificationTitle,
		ArgumentOutOfRangeExceptionText,
		Win32ExceptionText
	}

	internal static class Localization
	{
		private static readonly string[] Default = {
			"Bad arguments",
			"Expected {0} to {1} but got {2}.",
			"Clicked notification",
			"An error occurred while splitting the last argument into filename and argument. Check your quotes.",
			"An error occurred while trying to start the last argument. Error message: {0}"
		};

		private static readonly string[] Swedish = {
			"Felaktiga argument",
			"Förväntade {0} till {1} men fick {2}.",
			"Klickad notifiering",
			"Ett fel inträffade när sista argumentet skulle delas upp i filnamn och argument. Verifiera dina citationstecken.",
			"Ett fel inträffade när sista argumentet skulle startas. Felmeddelande: {0}"
		};

		/// <summary>
		/// Returns a localized string if a translation exists for the current UI culture. Otherwise, an English string is retuned.
		/// </summary>
		internal static string GetString(Strings @string)
		{
			// Quick and dirty multi language support. No resx files and satellite assemblies required.
			switch (CultureInfo.CurrentUICulture.Name)
			{
				case "sv":
				case "sv-AX":
				case "sv-FI":
				case "sv-SE":
					return Swedish[(int)@string];
				default:
					return Default[(int)@string];
			}
		}
	}
}
