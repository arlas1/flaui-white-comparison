using System.Diagnostics;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using static flaui_library.Utilities.Logger;
using static flaui_library.Utilities.TestSettings;

namespace flaui_library.Actions;

public static class HelperActions
{
    /// <summary>
    /// Finds a combo box that follows a text block with the given label, by index.
    /// </summary>
    /// <param name="mainWindow">The main application window.</param>
    /// <param name="text">Label text of the preceding text block.</param>
    /// <param name="index">Zero-based text block index.</param>
    /// <returns>The matching combo box element.</returns>
    public static ComboBox GetComboByIndex(AutomationElement mainWindow, string text, int index)
    {
        var timeout = Timeouts.DefaultTimeout;
        var interval = Timeouts.RetryInterval;

        var textBlock = FlaUI.Core.Tools.Retry.WhileNull(() =>
        {
            var textBlocks = mainWindow.FindAllDescendants(cf => cf.ByText(text));
            return textBlocks.Length > index ? textBlocks[index] : null;
        }, timeout, interval).Result;

        if (textBlock == null)
            Failure($"No TextBlock found with text '{text}'.");

        var comboBox = FlaUI.Core.Tools.Retry.WhileNull(() =>
        {
            return textBlock.Parent.FindAllChildren()
                .SkipWhile(child => !child.Equals(textBlock))
                .Skip(1)
                .FirstOrDefault(child => child.ControlType == ControlType.ComboBox)
                ?.AsComboBox();
        }, timeout, interval).Result;

        if (comboBox == null)
            Failure($"No ComboBox found following TextBlock '{text}'.");

        return comboBox;
    }

    /// <summary>
    /// Waits until a combo box reaches the expanded state within a timeout.
    /// </summary>
    /// <param name="comboBox">The combo box to monitor.</param>
    /// <param name="timeoutInSeconds">Maximum wait time in seconds.</param>
    public static void WaitForComboBoxToExpand(ComboBox comboBox, int timeoutInSeconds)
    {
        var stopwatch = Stopwatch.StartNew();
        while (stopwatch.Elapsed.TotalSeconds < timeoutInSeconds)
        {
            var expandCollapsePattern = comboBox.Patterns.ExpandCollapse.PatternOrDefault;
            if (expandCollapsePattern != null &&
                expandCollapsePattern.ExpandCollapseState == ExpandCollapseState.Expanded)
            {
                Thread.Sleep(200);
                return;
            }

            Thread.Sleep(100);
        }

        Failure("ComboBox did not expand within the expected time.");
    }

    /// <summary>
    /// Finds a text box that follows a text block with the given label, by index.
    /// </summary>
    /// <param name="mainWindow">The main application window.</param>
    /// <param name="text">Label text of the preceding text block.</param>
    /// <param name="index">Zero-based text block index.</param>
    /// <returns>The matching text box element, or null if not found.</returns>
    public static TextBox FindTextBoxByIndex(AutomationElement mainWindow, string text, int index)
    {
        var endTime = DateTime.Now.AddSeconds(30);
        AutomationElement label = null;
        while (DateTime.Now < endTime)
        {
            var textBlocks = mainWindow.FindAllDescendants(cf => cf.ByText(text));

            if (textBlocks != null && textBlocks.Length > index)
            {
                label = textBlocks[index];
                break;
            }

            Thread.Sleep(200);
        }

        if (label == null)
            return null;

        while (DateTime.Now < endTime)
        {
            var textBoxElement = label.Parent.FindAllChildren()
                .SkipWhile(child => !child.Equals(label))
                .Skip(1)
                .FirstOrDefault(child => child.ControlType == ControlType.Edit);

            if (textBoxElement != null)
                return textBoxElement.AsTextBox();

            Thread.Sleep(200);
        }

        return null;
    }
    
    /// <summary>
    /// Performs a single, double, or right click on an element.
    /// </summary>
    public static void PerformClick(AutomationElement element, bool isDoubleClick, bool isRightClick)
    {
        if (isDoubleClick)
            element.DoubleClick();
        else if (isRightClick)
            element.RightClick();
        else
        {
            element.Focus();
            element.Click();
        }
    }
    
    /// <summary>
    /// Finds a checkbox that follows a text block with the given label, by index.
    /// </summary>
    /// <param name="mainWindow">The main application window.</param>
    /// <param name="text">Label text of the preceding text block.</param>
    /// <param name="index">Zero-based text block index.</param>
    /// <returns>The matching checkbox element.</returns>
    public static CheckBox GetCheckbox(AutomationElement mainWindow, string text, int index)
    {
        var textBlock = FlaUI.Core.Tools.Retry.WhileNull(() =>
        {
            var textBlocks = mainWindow.FindAllDescendants(cf => cf.ByText(text));
            return textBlocks.Length > index ? textBlocks[index] : null;
        }, Timeouts.DefaultTimeout, Timeouts.RetryInterval).Result;

        if (textBlock == null)
            Failure($"No TextBlock found with text '{text}'.");

        var checkBox = FlaUI.Core.Tools.Retry.WhileNull(() =>
        {
            return textBlock.Parent.FindAllChildren()
                .SkipWhile(child => !child.Equals(textBlock))
                .Skip(1)
                .FirstOrDefault(child => child.ControlType == ControlType.CheckBox)
                ?.AsCheckBox();
        }, Timeouts.DefaultTimeout, Timeouts.RetryInterval).Result;

        if (checkBox == null)
            Failure($"No CheckBox found following TextBlock '{text}'.");

        return checkBox;
    }
}


