using TestStack.White.WindowsAPI;
using TestStack.White.UIItems;
using TestStack.White.UIItems.Finders;
using static white_library.AppManager;
using static white_library.HelperActions;

namespace white_library;

public static class TestActions
{
    /// <summary>
    /// Clicks a text element by its display text and index.
    /// </summary>
    /// <param name="text">The display text to match.</param>
    /// <param name="index">Zero-based element index.</param>
    /// <param name="isDoubleClick">Whether to double-click instead of single-click.</param>
    /// <param name="isRightClick">Whether to right-click instead of left-click.</param>
    public static void ClickText(string text, int index, bool isDoubleClick = false, bool isRightClick = false)
    {
        if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
        var mainWindow = GetMainWindow();
        var element = FindText(mainWindow, text, index);
        if (element == null) throw new InvalidOperationException($"Text '{text}'[{index}] not found.");
        
        if (isDoubleClick)
            element.DoubleClick();
        else if (isRightClick)
            element.RightClick();
        else
            element.Click();
    }
    
    /// <summary>
    /// Selects a value in a combo box by header, value, and index.
    /// </summary>
    /// <param name="comboHeader">The combo box header text.</param>
    /// <param name="value">The value to type and select.</param>
    /// <param name="index">Zero-based combo box index.</param>
    /// <param name="isDoubleClick">Whether to double-click the combo box.</param>
    /// <param name="isListItem">Whether to select by clicking a ListItem instead of pressing Enter.</param>
    public static void SelectCombo(string comboHeader, string value, int index, bool isDoubleClick = false, bool isListItem = false)
    {
        var mainWindow = GetMainWindow();
        var comboBox = GetComboBoxByPreceding(mainWindow, comboHeader, index);
        
        if (comboBox == null)
            throw new InvalidOperationException(
                $"ComboBox '{comboHeader}' not found.");
        if (!comboBox.Enabled || comboBox.IsOffScreen)
            throw new InvalidOperationException(
                $"ComboBox '{comboHeader}' is disabled or not visible.");

        // Open ComboBox.
        if (isDoubleClick)
            comboBox.DoubleClick();
        else
            comboBox.Click();

        // Type the requested value.
        if (comboBox.IsEditable)
            comboBox.EditableText = value;
        else
            mainWindow.Keyboard.Enter(value);

        // Wait until dropdown expands.
        WaitForComboBoxToExpand(comboBox, 10);

        // Select the requested value.
        if (isListItem)
        {
            var listItem = comboBox.Item(value);
            if (listItem == null) throw new InvalidOperationException($"ListItem '{value}' not found in ComboBox '{comboHeader}'.");
            listItem.Click();
        }
        else
        {
            mainWindow.Keyboard.PressSpecialKey(
                KeyboardInput.SpecialKeys.RETURN);
        }
        CollapseComboBox(comboBox);
    }
    
    /// <summary>
    /// Finds a button by its text and clicks it.
    /// </summary>
    /// <param name="buttonText">The text displayed on the button.</param>
    /// <param name="index">The zero-based index of the matching button.</param>
    public static void ClickButton(string buttonText, int index = 0)
    {
        var mainWindow = GetMainWindow();
        if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
        
        var button = mainWindow.Get<Button>(
            SearchCriteria.ByText(buttonText).AndIndex(index));
        if (button == null) throw new InvalidOperationException($"Button '{buttonText}' with index {index} was not found.");
        
        button.Click();
    }
    
    /// <summary>
    /// Finds a label by its text and returns the displayed text.
    /// </summary>
    /// <param name="elementText">The text used to identify the label.</param>
    /// <param name="index">The zero-based index of the matching label.</param>
    public static string GetText(string elementText, int index = 0)
    {
        var mainWindow = GetMainWindow();
        if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));

        var element = mainWindow.Get<Label>(
            SearchCriteria.ByText(elementText).AndIndex(index));
        if (element == null) throw new InvalidOperationException($"Element '{elementText}' with index {index} was not found.");

        return element.Text;
    }
    
    /// <summary>
    /// Gets a combo box by its header text and index.
    /// </summary>
    /// <param name="labelText">The text of the label preceding the ComboBox.</param>
    /// <param name="index">The zero-based index of the matching label.</param>
    public static string GetComboBox(string labelText, int index = 0)
    {
        var mainWindow = GetMainWindow();
        if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));

        var comboBox = GetComboBoxByPreceding(mainWindow, labelText, index);
        if (comboBox == null) throw new InvalidOperationException($"ComboBox associated with label '{labelText}' was not found.");

        return comboBox.SelectedItem?.Text ?? string.Empty;
    }
}
