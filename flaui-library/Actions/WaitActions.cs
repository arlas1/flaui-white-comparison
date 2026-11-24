using static flaui_library.Utilities.AppManager;
using static flaui_library.Utilities.Logger;
using static flaui_library.Utilities.TestSettings;

namespace flaui_library.Actions;

public static class WaitActions
{
    /// <summary>
    /// Waits for the specified text to appear or disappear.
    /// </summary>
    /// <param name="text">The text to search for.</param>
    /// <param name="shouldExist">Whether the text should exist.</param>
    /// <param name="timeoutInSeconds">Maximum wait time in seconds.</param>
    public static void WaitForText(string text, bool shouldExist, int timeoutInSeconds = 90)
    {
        var mainWindow = GetMainWindow();
        int visibleCount = 0;

        var result = FlaUI.Core.Tools.Retry.WhileFalse(() =>
        {
            try
            {
                var elements = mainWindow.FindAllDescendants(cf => cf.ByText(text));
                visibleCount = elements.Count(e =>
                {
                    try { return !e.Properties.IsOffscreen.Value; }
                    catch { return false; }
                });

                return shouldExist ? visibleCount > 0 : visibleCount == 0;
            }
            catch { return false; }
        },
        TimeSpan.FromSeconds(timeoutInSeconds),
        Timeouts.RetryInterval,
        throwOnTimeout: true,
        timeoutMessage: $"WaitForText failed for '{text}' after {timeoutInSeconds}s");

        if (!result.Success)
            Failure($"Wait for text '{text}' failed (shouldExist={shouldExist}), VisibleCount: {visibleCount}");
    }

    /// <summary>
    /// Waits for an element with the given AutomationId to appear or disappear.
    /// </summary>
    /// <param name="automationId">The AutomationId to search for.</param>
    /// <param name="shouldExist">Whether the element should exist.</param>
    /// <param name="timeoutInSeconds">Maximum wait time in seconds.</param>
    /// <param name="index">Zero-based element index, or -1 for any match.</param>
    public static void WaitForAutomationId(string automationId, bool shouldExist, int timeoutInSeconds = 60, int index = -1)
    {
        var mainWindow = GetMainWindow();
        int visibleCount = 0;

        var result = FlaUI.Core.Tools.Retry.While(
            () =>
            {
                var elements = mainWindow
                    .FindAllDescendants(cf => cf.ByAutomationId(automationId))
                    .ToList();

                if (index >= 0)
                {
                    if (index >= elements.Count)
                    {
                        visibleCount = 0;
                        return 0;
                    }

                    bool isVisible = !elements[index].Properties.IsOffscreen.Value;
                    visibleCount = isVisible ? 1 : 0;
                    return visibleCount;
                }

                visibleCount = elements.Count(e => !e.Properties.IsOffscreen.Value);
                return visibleCount;
            },
            count => shouldExist ? count == 0 : count > 0,
            TimeSpan.FromSeconds(timeoutInSeconds),
            Timeouts.RetryInterval);

        if (!result.Success)
            Failure($"Wait for AutomationId '{automationId}'[{index}] failed, VisibleCount: {visibleCount}");
    }
}
