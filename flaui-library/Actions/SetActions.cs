using FlaUI.Core.AutomationElements;
using FlaUI.Core.Input;
using FlaUI.Core.WindowsAPI;
using static flaui_library.Actions.ClickActions;
using static flaui_library.Actions.HelperActions;
using static flaui_library.Utilities.AppManager;
using static flaui_library.Utilities.Logger;
using static flaui_library.Utilities.TestSettings;

namespace flaui_library.Actions;

public static class SetActions
{
    /// <summary>
    /// Selects a value in a combo box by header, value, and index.
    /// </summary>
    /// <param name="comboHeader">The combo box header text.</param>
    /// <param name="value">The value to type and select.</param>
    /// <param name="index">Zero-based combo box index.</param>
    /// <param name="isDoubleClick">Whether to double-click the combo box.</param>
    /// <param name="isListItem">Whether to select by clicking a ListItem instead of pressing Enter.</param>
    public static void SetComboBox(string comboHeader, string value, int index, bool isDoubleClick = false, bool isListItem = false)
    {
        var mainWindow = GetMainWindow();
        var comboBox = FlaUI.Core.Tools.Retry.WhileNull(
            () => GetComboByIndex(mainWindow, comboHeader, index),
            Timeouts.DefaultTimeout,
            Timeouts.RetryInterval).Result;

        if (comboBox == null)
            Failure($"ComboBox '{comboHeader}' not found.");

        if (isDoubleClick)
            comboBox.DoubleClick();
        else
            comboBox.Click();

        ClearAndType(value);

        WaitForComboBoxToExpand(
            comboBox,
            (int)Timeouts.DefaultTimeout.TotalSeconds);

        if (isListItem)
            ClickListItem(value);
        else
            Keyboard.Press(VirtualKeyShort.ENTER);

        Thread.Sleep(100);
        comboBox.Collapse();
    }

    /// <summary>
    /// Sets a text box value by its label text.
    /// </summary>
    /// <param name="label">The text box label text.</param>
    /// <param name="value">The value to enter.</param>
    /// <param name="index">Zero-based label index.</param>
    public static void SetText(string label, string value, int index = 0)
    {
        var mainWindow = GetMainWindow();
        TextBox textBox = null;

        var endTime = DateTime.Now.AddSeconds(20);

        while (DateTime.Now < endTime)
        {
            textBox = FindTextBoxByIndex(mainWindow, label, index);

            if (textBox != null)
                break;

            Thread.Sleep(500);
        }

        if (textBox != null)
        {
            textBox.Enter(value);
            Thread.Sleep(700);

            if (!textBox.Text.Equals(value))
                Failure(
                    $"Failed to set TextBox '{label}'. Current value: '{textBox.Text}'");
        }
        else
        {
            Failure($"TextBox with label '{label}' not found.");
        }
    }

    private static void ClearAndType(string value)
    {
        Keyboard.TypeSimultaneously(
            VirtualKeyShort.CONTROL,
            VirtualKeyShort.KEY_A);

        Keyboard.Type(value);
    }
}