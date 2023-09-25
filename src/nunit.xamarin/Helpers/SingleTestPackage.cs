using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Runner.Helpers;
using nunit.xamarin.Factories;

namespace nunit.xamarin.Helpers
{
    /// <summary>
    /// Contains all assemblies for a test run, and controls execution of tests and collection of results
    /// </summary>
    internal class SingleTestPackage : TestPackage
    {
        private readonly string _testNameToRun;
        
        public SingleTestPackage(string testNameToRun)
        {
            _testNameToRun = testNameToRun;
        }

        protected override ITestFilter GetTestFilters()
        {
            if (string.IsNullOrEmpty(_testNameToRun))
            {
                return TestFilter.Empty;
            }

            return TestRunnerFilterFactory.CreateTestNameFilter(_testNameToRun);
        }
    }
}
