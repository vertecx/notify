# Notify
Notify can be used to easily display notifications from scripts, scheduled tasks and other programs running in the background on your Windows desktop.

The notifications are displayed as toasts on Windows 10* and as balloon tips on earlier versions of Windows.

Notifications can be made interactive so that a program or file is opened if the notification is clicked.
On Windows 10 version 1511 and later, it is possible to make persistent notifications that remain in Action Center until manually dismissed.

Precompiled binaries can be downloaded from [Releases](https://github.com/vertecx/notify/releases).

> \* Balloon to toast conversion on Windows 10 can be disabled by the "Disable showing balloon notifications as toasts" group policy.   

# Syntax
```
QuickNotification.exe [title] text [icon] [launch]

PersistentNotification.exe /autostart
PersistentNotification.exe [title] text [icon] [launch]
```

# Arguments
**title**: Optional. Enclose in quotes if the title contains spaces.

**text**: Required. Enclose in quotes if the text contains spaces. Use `\n` or `nnnn` to split the text over multiple lines.

**icon**: Optional. To explicitly display no icon, use the value 0, n or none. To display an information icon, use 1, i, info or information. Warning icon: 2, w, warn or warning. Error icon: 3, e, err or error.

**launch**: Optional. Path to launch if the notification is clicked. Enclose in quotes if the text contains spaces. Surround the executable path with single quotes if it contains spaces. Escaping other quotes as `\"` seems to be compatible with most software.

**/autostart**: Must occur alone. Causes PersistentNotification to start without showing any notification.

# Examples
```
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
```

# Persistent notifications in Windows 10
Notify consists of two binaries, QuickNotification.exe and PersistentNotification.exe.
PersistentNotification.exe differs from QuickNotification.exe in that it is a single instance application that doesn't terminate after showing a notification.
Subsequent calls to PersistentNotification.exe causes notifications to be shown by the already running instance.

PersistentNotification.exe is mainly intended to be used on Windows 10 version 1511 or later, where it can be configured to show notifications that remain in Action Center until manually dismissed.
This can be useful for notifications you don't want to miss like backup results.

Configuring PersistentNotification.exe to show notification in Action Center comes with a few side effects.
Once the toast has closed and the notification is only shown in Action Center, clicking it no longer launches the configured application.
Another side effect is that the text in Action Center changes from white to gray making it slightly harder to read.

## Configure PersistentNotification.exe
1. Start PersistentNotification.exe by double clicking it. The invalid arguments message will be shown.
2. Open the **Settings** app and click **System** followed by **Notifications & actions**.
3. Find and click **Persistent Notification** in the list of senders.
4. Toggle **Show notifications in action center** from off to on.

## Usage in scripts
As the first instance of PersistentNotification.exe does not terminate, it might block the script that started it.
This can be resolved either by modifying the script or make Windows auto start the first instance of PersistentNotification.exe.

### Modify script
In batch files, use the start command to launch PersistentNotification.exe without blocking the script.

Example: `start PersistentNotification.exe Hello World i`

In PowerShell, make sure the -Wait argument is **not** used when calling Start-Process.

### Auto start with Windows
PersistentNotification.exe supports the /autostart argument which will start the program without showing any notification.
Specifying the argument is useful when making Windows auto start the first instance of PersistentNotification.exe, 
either by adding a shortcut to the Startup directory or a string value to one of the registry Run keys.

## Why not code it proper?
Implementing PersistentNotification.exe using the Windows 10 Toast API would require a complete rewrite sharing little to no code with the current project.

The Toast API also have some strange requirements [documented at MSDN](https://blogs.msdn.microsoft.com/tiles_and_toasts/2015/10/16/quickstart-handling-toast-activations-from-win32-apps-in-windows-10/) like requiring a shortcut on the start screen. The shortcut must also have special properties that are not easily configurable.

The original author of Notify doesn't feel doing it right is worth the extra complexity.
