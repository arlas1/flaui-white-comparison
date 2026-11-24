using System.Diagnostics;
using System.Runtime.InteropServices;
using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;
using static flaui_library.Utilities.TestSettings;

namespace flaui_library.Utilities;

/// <summary>
    /// Manages the application lifecycle, window access, and process attachment.
    /// </summary>
public static class AppManager
{
    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll")]
    private static extern bool IsIconic(IntPtr hWnd);

    private const int SW_RESTORE = 9;

    /// <summary>
    /// Retrieves the main window of the configured application process.
    /// </summary>
    /// <returns>The main application window.</returns>
    public static Window GetMainWindow()
    {
        var processName = Application.ProcessName;
        using var automation = new UIA3Automation();

        var processes = Process.GetProcessesByName(processName);
        if (processes.Length == 0)
            throw new InvalidOperationException($"No process found with name '{processName}'.");

        var targetProcess = processes.OrderByDescending(p => p.StartTime).First();
        var app = FlaUI.Core.Application.Attach(targetProcess.Id);

        var retryResult = FlaUI.Core.Tools.Retry.WhileNull(
            () => app.GetMainWindow(automation),
            timeout: Timeouts.WindowAttachTimeout,
            ignoreException: true
        );

        if (retryResult.Result == null)
            throw new InvalidOperationException($"Main window for '{processName}' not found within timeout.");

        return retryResult.Result;
    }

    /// <summary>
    /// Launches the application using the configured launch path and arguments.
    /// </summary>
    /// <returns>The launched application instance.</returns>
    public static FlaUI.Core.Application LaunchApplication()
    {
        var launchPath = Application.LaunchPath;
        if (string.IsNullOrEmpty(launchPath))
            throw new InvalidOperationException("LaunchPath is not configured in appsettings.json.");

        var args = Application.LaunchArguments;
        return string.IsNullOrEmpty(args)
            ? FlaUI.Core.Application.Launch(launchPath)
            : FlaUI.Core.Application.Launch(launchPath, args);
    }

    /// <summary>
    /// Attaches to the most recently started instance of the configured process.
    /// </summary>
    /// <returns>The attached application instance.</returns>
    public static FlaUI.Core.Application AttachToRunning()
    {
        var processName = Application.ProcessName;
        var processes = Process.GetProcessesByName(processName);

        if (processes.Length == 0) throw new InvalidOperationException($"No running process found with name '{processName}'.");

        var targetProcess = processes.OrderByDescending(p => p.StartTime).First();
        return FlaUI.Core.Application.Attach(targetProcess.Id);
    }

    /// <summary>
    /// Brings the application window to the foreground, restoring it if minimized.
    /// </summary>
    /// <returns>True if the window was successfully brought to the foreground.</returns>
    public static bool BringToFront()
    {
        var processName = Application.ProcessName;
        var processes = Process.GetProcessesByName(processName);

        if (processes.Length == 0)
        {
            Logger.Failure($"No process '{processName}' found to bring to front.");
            return false;
        }

        var mainWindowHandle = processes
            .OrderByDescending(p => p.StartTime)
            .First()
            .MainWindowHandle;

        if (mainWindowHandle == IntPtr.Zero)
        {
            Logger.Failure($"Main window handle not found for '{processName}'.");
            return false;
        }

        if (IsIconic(mainWindowHandle))
            ShowWindow(mainWindowHandle, SW_RESTORE);

        return SetForegroundWindow(mainWindowHandle);
    }

    /// <summary>
    /// Checks whether the configured application process is currently running.
    /// </summary>
    /// <returns>True if the application process is running.</returns>
    public static bool IsRunning()
    {
        var processName = Application.ProcessName;
        return Process.GetProcessesByName(processName).Length > 0;
    }
}
