using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Runner.Helpers;
using nunit.xamarin.Factories;

namespace nunit.xamarin.Helpers
{
    /// <summary>
    /// Contains all assemblies for a test run, and controls execution of tests and collection of results
    /// </summary>
    internal class CategoryTestPackage : TestPackage
    {
        private readonly string _categoryName;

        public CategoryTestPackage(string categoryName)
        {
            _categoryName = categoryName;
        }

        protected override ITestFilter GetTestFilters()
        {
            if (string.IsNullOrEmpty(_categoryName))
            {
                return TestFilter.Empty;
            }

            return TestRunnerFilterFactory.CreateCategoryFilter(_categoryName);
        }
    }
}
