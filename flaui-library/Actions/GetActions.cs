using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using static flaui_library.Actions.HelperActions;
using static flaui_library.Utilities.AppManager;
using static flaui_library.Utilities.Logger;

namespace flaui_library.Actions;

public static class GetActions
{
    /// <summary>
    /// Gets the text value of a specific grid cell by row and cell index.
    /// </summary>
    /// <param name="automationId">The grid's AutomationId.</param>
    /// <param name="row">Zero-based row index.</param>
    /// <param name="cell">Zero-based cell index.</param>
    /// <param name="listItemName">Name of the parent list item to scope the search.</param>
    /// <returns>The text value of the cell.</returns>
    public static string GetCellValue(string automationId, int row, int cell, string listItemName = "Enter Item Name")
    {
        Thread.Sleep(1000);
        var mainWindow = GetMainWindow();
        if (string.IsNullOrEmpty(automationId))
            automationId = "PART_GridViewVirtualizingPanel";
        var listItems = mainWindow.FindAllDescendants(
            cf => cf.ByControlType(ControlType.ListItem));
        var parentItem = listItems.FirstOrDefault(
            item => item.Name.Contains(listItemName));

        AutomationElement searchScope = parentItem ?? mainWindow;
        var tables = searchScope.FindAllDescendants(
            cf => cf.ByAutomationId(automationId));
        var table = tables.FirstOrDefault();
        if (table == null)
            Failure("Grid not found");

        var grid = table.AsGrid();
        var textElement = grid.Rows[row].Cells[cell]
            .FindFirstDescendant(
                cf => cf.ByControlType(ControlType.Text));

        Info($"Cell '{row},{cell}' value: {textElement.Name}");
        return textElement.Name;
    }

    /// <summary>
    /// Gets a combo box by its header text and index.
    /// </summary>
    /// <param name="text">The combo box header text.</param>
    /// <param name="index">Zero-based combo box index.</param>
    /// <returns>The matching combo box element.</returns>
    public static string GetComboBox(string text, int index)
    {
        var mainWindow = GetMainWindow();
        ComboBox comboBox = null;
        var endTime = DateTime.Now.AddSeconds(30);
        while (DateTime.Now < endTime)
        {
            comboBox = GetComboByIndex(mainWindow, text, index);
            if (comboBox != null)
                break;

            Thread.Sleep(500);
        }
        if (comboBox != null)
            return comboBox.SelectedItem?.Text ?? string.Empty;
        
        Failure($"ComboBox '{text}' not found.");
        return null;
    }

    /// <summary>
    /// Gets the current text value from a text box by its label.
    /// </summary>
    /// <param name="label">The text box label text.</param>
    /// <param name="index">Zero-based label index.</param>
    /// <returns>The current text value, or empty string if not found.</returns>
    public static string GetText(string label, int index)
    {
        var mainWindow = GetMainWindow();
        TextBox textBox = null;
        var endTime = DateTime.Now.AddSeconds(30);
        while (DateTime.Now < endTime)
        {
            textBox = FindTextBoxByIndex(
                mainWindow, label, index);

            if (textBox != null)
                break;

            Thread.Sleep(500);
        }

        if (textBox != null)
            return textBox.Text;
        Failure($"TextBox '{label}' not found.");

        return string.Empty;
    }
}
