using System.Text;
using NUnit.Framework;

namespace flaui_library.Utilities;

using static Allure.Net.Commons.AllureApi;

public static class Logger
{
    public static void Debug(string message)
    {
        TestContext.Out.WriteLine($"[DEBUG] {message}");
    }

    public static void Info(string message)
    {
        TestContext.Out.WriteLine($"[INFO] {message}");
        Step(message);
    }

    /// <summary>
    /// Logs text whose line breaks and spacing matter, such as a table: <paramref name="title"/> becomes the
    /// Allure step and <paramref name="text"/> its text attachment, since a step name collapses whitespace.
    /// </summary>
    public static void InfoBlock(string title, string text)
    {
        TestContext.Out.WriteLine($"[INFO] {text}");
        Step(title, () => AddAttachment(title, "text/plain", Encoding.UTF8.GetBytes(text), ".txt"));
    }

    public static void Warn(string message)
    {
        TestContext.Out.WriteLine($"[WARN] {message}");
        Step($"[WARN] {message}", () => Assert.Warn(message));
    }

    public static void Failure(string message)
    {
        TestContext.Out.WriteLine($"[FAILURE] {message}");
        Step($"[FAILURE] {message}", () => Assert.Fail(message));
    }

    public static void Success(string message)
    {
        TestContext.Out.WriteLine($"[SUCCESS] {message}");
        Step($"[SUCCESS] {message}", () => Assert.Pass(message));
    }
}
