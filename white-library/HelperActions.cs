using System.Diagnostics;
using TestStack.White.UIItems;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems.ListBoxItems;
using TestStack.White.UIItems.WindowItems;
using TestStack.White.WindowsAPI;

namespace white_library;

public static class HelperActions
{
    /// <summary>
    /// Finds a text element by its display text and index.
    /// </summary>
    public static IUIItem FindText(Window mainWindow, string text, int index)
    {
        if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));

        var stopwatch = Stopwatch.StartNew();
        var criteria = SearchCriteria
            .ByText(text)
            .AndIndex(index);

        while (stopwatch.Elapsed.TotalSeconds < 30)
        {
            try
            {
                var element = mainWindow.Get(criteria);
                if (element != null)
                    return element;
            }
            catch
            {
                // Element has not been found yet.
            }

            Thread.Sleep(500);
        }

        return null;
    }

    /// <summary>
    /// Finds a combo box that follows a label with the given text.
    /// </summary>
    public static ComboBox GetComboBoxByPreceding(Window mainWindow, string labelText, int index)
    {
        if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));

        var stopwatch = Stopwatch.StartNew();
        var criteria = SearchCriteria
            .ByText(labelText)
            .AndIndex(index);

        while (stopwatch.Elapsed.TotalSeconds < 30)
        {
            try
            {
                var label = mainWindow.Get<Label>(criteria);
                if (label != null)
                {
                    var comboBox = mainWindow.Items
                        .SkipWhile(item => !item.Equals(label))
                        .Skip(1)
                        .OfType<ComboBox>()
                        .FirstOrDefault();

                    if (comboBox != null)
                        return comboBox;
                }
            }
            catch
            {
                // ComboBox has not been found yet.
            }
            
            Thread.Sleep(500);
        }
        
        return null;
    }

    /// <summary>
    /// Waits until a combo box dropdown is expanded.
    /// </summary>
    public static void WaitForComboBoxToExpand(ComboBox comboBox, int timeoutInSeconds)
    {
        var stopwatch = Stopwatch.StartNew();
        while (stopwatch.Elapsed.TotalSeconds < timeoutInSeconds)
        {
            try
            {
                var items = comboBox.Items;
                if (items != null && items.Count > 0)
                    return;
            }
            catch
            {
                // Dropdown is not ready yet.
            }

            Thread.Sleep(100);
        }

        throw new TimeoutException("ComboBox did not expand within the expected time.");
    }

    /// <summary>
    /// Closes the combo box dropdown using Escape.
    /// </summary>
    public static void CollapseComboBox(ComboBox comboBox)
    {
        comboBox.KeyIn(KeyboardInput.SpecialKeys.ESCAPE);
        Thread.Sleep(100);
    }
}