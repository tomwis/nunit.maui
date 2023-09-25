using NUnit.Framework.Internal;

namespace nunit.xamarin.Factories;

internal static class TestRunnerFilterFactory
{
    public static TestFilter CreateTestNameFilter(string testName) 
        => TestFilter.FromXml($"<filter><test>{testName}</test></filter>");

    public static TestFilter CreateCategoryFilter(string categoryName) 
        => TestFilter.FromXml($"<filter><cat>{categoryName}</cat></filter>");
}