using System.Diagnostics;
using TestStack.White;
using TestStack.White.Factory;
using TestStack.White.UIItems.WindowItems;

namespace white_library;

public static class AppManager
{
    private const string WindowTitle = "TestApp";

    /// <summary>
    /// Attaches White to the currently running application.
    /// </summary>
    /// <returns>The attached application instance.</returns>
    public static Application GetApplication()
    {
        var processName = Path.GetFileNameWithoutExtension("TestProcess.exe");
        var processes = Process.GetProcessesByName(processName);
        if (processes.Length == 0) throw new InvalidOperationException($"No running process found with name '{processName}'.");
        var targetProcess = processes
            .OrderByDescending(p => p.StartTime)
            .First();
        
        return Application.Attach(targetProcess.Id);
    }

    /// <summary>
    /// Retrieves the main window of the application.
    /// </summary>
    /// <returns>The main application window.</returns>
    public static Window GetMainWindow()
    {
        var application = GetApplication();
        var stopwatch = Stopwatch.StartNew();
        while (stopwatch.Elapsed.TotalSeconds < 30)
        {
            try
            {
                var mainWindow = application.GetWindow(WindowTitle, InitializeOption.NoCache);
                if (mainWindow != null) return mainWindow;
            }
            catch
            {
                // Application window may not be ready yet.
            }

            Thread.Sleep(500);
        }

        throw new InvalidOperationException($"Main window '{WindowTitle}' was not found within 30 seconds.");
    }
}
