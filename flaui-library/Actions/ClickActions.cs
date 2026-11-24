using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using static flaui_library.Actions.HelperActions;
using static flaui_library.Utilities.AppManager;
using static flaui_library.Utilities.Logger;
using static flaui_library.Utilities.TestSettings;

namespace flaui_library.Actions;

public static class ClickActions
{
    /// <summary>
    /// Clicks a combo box identified by its label text.
    /// </summary>
    /// <param name="text">The combo box label text.</param>
    public static void ClickCombo(string text)
    {
        var mainWindow = GetMainWindow();
        var comboBox = GetComboByIndex(mainWindow, text, 0);
        if (comboBox != null && comboBox.IsEnabled && !comboBox.IsOffscreen)
            comboBox.Click();
        else
            Failure($"ComboBox '{text}' not found.");
    }

    /// <summary>
    /// Clicks a text element by its display text and index.
    /// </summary>
    /// <param name="text">The display text to match.</param>
    /// <param name="index">Zero-based element index.</param>
    /// <param name="isDoubleClick">Whether to double-click instead of single-click.</param>
    /// <param name="isRightClick">Whether to right-click instead of left-click.</param>
    public static void ClickText(string text, int index, bool isDoubleClick = false, bool isRightClick = false)
    {
        var mainWindow = GetMainWindow();
        var textBlocks = FlaUI.Core.Tools.Retry.WhileNull(
            () =>
            {
                var found = mainWindow.FindAllDescendants(
                    cf => cf.ByControlType(ControlType.Text).And(cf.ByName(text)));

                return (found != null && found.Length > index && index >= 0)
                    ? found
                    : null;
            },
            Timeouts.DefaultTimeout,
            Timeouts.RetryInterval).Result;

        if (textBlocks != null && index >= 0 && index < textBlocks.Length)
        {
            PerformClick(textBlocks[index], isDoubleClick, isRightClick);
        }
        else
        {
            Failure($"Text '{text}'[{index}] not found.");
        }
    }

    /// <summary>
    /// Clicks a button by its display text and index.
    /// </summary>
    /// <param name="text">The button display text.</param>
    /// <param name="index">Zero-based button index.</param>
    /// <param name="isDoubleClick">Whether to double-click instead of single-click.</param>
    /// <param name="isRightClick">Whether to right-click instead of left-click.</param>
    public static void ClickButton(string text, int index, bool isDoubleClick = false, bool isRightClick = false)
    {
        var mainWindow = GetMainWindow();
        var buttons = FlaUI.Core.Tools.Retry.WhileNull(
            () =>
            {
                var matches = mainWindow
                    .FindAllDescendants(cf =>
                        cf.ByControlType(ControlType.Button).And(cf.ByName(text)))
                    .Select(e => e.AsButton())
                    .ToList();

                return matches.Count > index && index >= 0
                    ? matches
                    : null;
            },
            Timeouts.DefaultTimeout,
            Timeouts.RetryInterval).Result;

        if (buttons != null && index < buttons.Count && buttons[index] != null)
        {
            var button = buttons[index];
            button.Focus();
            Thread.Sleep(250);

            PerformClick(button, isDoubleClick, isRightClick);
        }
        else
        {
            Failure($"Button '{text}'[{index}] not found.");
        }
    }

    /// <summary>
    /// Clicks a specific cell in a grid identified by ClassName.
    /// </summary>
    /// <param name="className">The grid parent's ClassName.</param>
    /// <param name="row">Zero-based row index.</param>
    /// <param name="cell">Zero-based cell index within the row.</param>
    /// <param name="isDoubleClick">Whether to double-click the cell.</param>
    /// <param name="isRightClick">Whether to right-click instead of left-click.</param>
    public static void ClickTableCell(string className, int row, int cell, bool isDoubleClick = false, bool isRightClick = false)
    {
        var mainWindow = GetMainWindow();
        if (string.IsNullOrEmpty(className)) throw new ArgumentNullException(nameof(className));

        var listItem = FlaUI.Core.Tools.Retry.WhileNull(
            () => mainWindow.FindFirstDescendant(
                cf => cf.ByClassName(className)),
            Timeouts.DefaultTimeout,
            Timeouts.RetryInterval).Result;

        if (listItem == null) 
            Failure($"Grid not found for class '{className}'.");

        var tables = listItem.FindAllDescendants(
            cf => cf.ByAutomationId("PART_GridViewVirtualizingPanel"));

        var table = tables.FirstOrDefault();

        if (table == null)
            Failure($"Grid not found for class '{className}'.");

        var grid = table.AsGrid();

        if (grid.Rows.Length <= row)
            Failure($"Invalid row index {row}.");

        var gridRow = grid.Rows[row];

        if (gridRow.Cells.Length <= cell)
            Failure($"Invalid cell index {cell}.");

        var cellToClick = gridRow.Cells[cell];

        PerformClick(cellToClick, isDoubleClick, isRightClick);
    }

    /// <summary>
    /// Clicks a ListItem matching the given display text.
    /// </summary>
    /// <param name="text">The display text to search for.</param>
    /// <param name="isDoubleClick">Whether to double-click instead of single-click.</param>
    /// <param name="isRightClick">Whether to right-click instead of left-click.</param>
    public static void ClickListItem(string text, bool isDoubleClick = false, bool isRightClick = false)
    {
        var mainWindow = GetMainWindow();

        var targetListItem = FlaUI.Core.Tools.Retry.WhileNull(
            () =>
            {
                var listItems = mainWindow.FindAllDescendants(
                    cf => cf.ByControlType(ControlType.ListItem));

                foreach (var listItem in listItems)
                {
                    var listItemTexts = listItem.FindAllDescendants(
                        cf => cf.ByControlType(ControlType.Text));

                    foreach (var listItemText in listItemTexts)
                    {
                        if (listItemText.Name.Equals(
                            text,
                            StringComparison.OrdinalIgnoreCase))
                        {
                            return listItem;
                        }
                    }
                }

                return null;
            },
            Timeouts.DefaultTimeout,
            Timeouts.RetryInterval).Result;

        if (targetListItem != null)
            PerformClick(targetListItem, isDoubleClick, isRightClick);
        else
            Failure($"ListItem '{text}' not found.");
    }

    /// <summary>
    /// Clicks a checkbox matching the given text and index.
    /// </summary>
    /// <param name="text">The checkbox label text.</param>
    /// <param name="index">Zero-based checkbox index.</param>
    /// <param name="isDoubleClick">Whether to double-click instead of single-click.</param>
    /// <param name="isRightClick">Whether to right-click instead of left-click.</param>
    public static void ClickCheckBox(string text, int index, bool isDoubleClick = false, bool isRightClick = false)
    {
        var mainWindow = GetMainWindow();
        CheckBox checkbox = FlaUI.Core.Tools.Retry.WhileNull(
            () =>
            {
                var found = GetCheckbox(mainWindow, text, index);

                return (found != null && found.IsEnabled && !found.IsOffscreen)
                    ? found
                    : null;
            },
            Timeouts.DefaultTimeout,
            Timeouts.RetryInterval).Result;

        if (checkbox == null)
            Failure($"Checkbox '{text}'[{index}] not found.");

        Thread.Sleep(250);

        if (isDoubleClick)
            checkbox.DoubleClick();
        else if (isRightClick)
            checkbox.RightClick();
        else
        {
            checkbox.Focus();
            checkbox.Click();
        }
    }

    /// <summary>
    /// Clicks an element found by HelpText and index.
    /// </summary>
    /// <param name="text">The HelpText value to search for.</param>
    /// <param name="index">Zero-based element index.</param>
    /// <param name="isDoubleClick">Whether to double-click instead of single-click.</param>
    /// <param name="isRightClick">Whether to right-click instead of left-click.</param>
    public static void ClickByHelpText(string text, int index, bool isDoubleClick = false, bool isRightClick = false)
    {
        var mainWindow = GetMainWindow();
        var elements = FlaUI.Core.Tools.Retry.WhileNull(
            () =>
            {
                var found = mainWindow.FindAllDescendants(
                    cf => cf.ByHelpText(text));

                return (found != null && found.Length > index && index >= 0)
                    ? found
                    : null;
            },
            Timeouts.DefaultTimeout,
            Timeouts.RetryInterval).Result;

        if (elements != null && index >= 0 && index < elements.Length)
        {
            PerformClick(elements[index], isDoubleClick, isRightClick);
        }
        else
        {
            Failure($"Element with HelpText '{text}'[{index}] not found.");
        }
    }
}
