# Notify
Notify can be used to easily display notifications from scripts, scheduled tasks and other programs running in the background on your Windows desktop.

The notifications are displayed as toasts on Windows 10* and as balloon tips on earlier versions of Windows.

Notifications can be made interactive so that a program or file is opened if the notification is clicked.
On Windows 10 version 1511 and later, it is possible to make persistent notifications that remain in Action Center until manually dismissed.

Precompiled binaries can be downloaded from [Releases](https://github.com/vertecx/notify/releases).

> \* Balloon to toast conversion on Windows 10 can be disabled by the "Disable showing balloon notifications as toasts" group policy.   

# Syntax
`QuickNotification.exe [title] text [icon] [launch]`

> The syntax for PersistentNotification.exe is identical.

# Arguments
**title**: Optional. Enclose in quotes if the title contains spaces.

**text**: Required. Enclose in quotes if the text contains spaces. Use `\n` or `nnnn` to split the text over multiple lines.

**icon**: Optional. To explicitly display no icon, use the value 0, n or none. To display an information icon, use 1, i, info or information. Warning icon: 2, w, warn or warning. Error icon: 3, e, err or error.

**launch**: Optional. Path to launch if the notification is clicked. Enclose in quotes if the text contains spaces. Surround the executable path with single quotes if it contains spaces. Escaping other quotes as `\"` seems to be compatible with most software.

# Examples
````
# A notification displaying only Hello:
QuickNotification.exe Hello

# The title Hello World and the text I'm a notification!:
QuickNotification.exe "Hello World" "I'm a notification!"

# An information icon added to the above example:
QuickNotification.exe "Hello World" "I'm a notification!" info

# A notification that opens the log file of a failed backup when clicked:
QuickNotification.exe Backup "Backup job failed!\n3 files could not be copied." err C:\Logs\Backup.txt

# Launch a program with spaces in the path:
QuickNotification.exe Space! "Click to launch..." "'C:\Program Files\Windows NT\Accessories\wordpad.exe' \"C:\Log Files\Space Log.txt\""
````

# Persistent notifications in Windows 10
Notify consists of two binaries, QuickNotification.exe and PersistentNotification.exe.
On operating systems prior to Windows 10 version 1511 (November Update) they have the same behavior.
On Windows 10 version 1511 and later, PersistentNotification.exe can be configured to show the notifications in Action Center until manually dismissed.
This can be useful for notifications you don't want to miss like backup results.

For this to work, the application must misbehave by not running the Windows message loop.
A side effect of this is that it is no longer possible to detect if the notification was clicked and to launch the last argument.
Another side effect is that the text in Action Center changes from white to gray making it slightly harder to read.

## Configure PersistentNotification.exe
1. Start PersistentNotification.exe at least once. Double clicking it to show the invalid arguments message is enough.
2. Open the **Settings** app and click **System** followed by **Notifications & actions**.
3. Find and click **Persistent Notification** in the list of senders.
4. Toggle **Show notifications in action center** from off to on.

## Why not code it proper?
Implementing PersistentNotification.exe using the Windows 10 Toast API would require a complete rewrite sharing little to no code with the current project.

The Toast API also have some strange requirements [documented at MSDN](https://blogs.msdn.microsoft.com/tiles_and_toasts/2015/10/16/quickstart-handling-toast-activations-from-win32-apps-in-windows-10/) like requiring a shortcut on the start screen. The shortcut must also have special properties that are not easily configurable.

The original author of Notify doesn't feel doing it right is worth the extra complexity.
