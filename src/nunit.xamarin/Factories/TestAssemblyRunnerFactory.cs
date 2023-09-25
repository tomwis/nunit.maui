using System.Reflection;
using NUnit.Framework.Api;

namespace nunit.xamarin.Factories;

public static class TestAssemblyRunnerFactory
{
    public static async Task<NUnitTestAssemblyRunner> CreateTestAssemblyRunner(
        Assembly assembly, 
        Dictionary<string, object> options)
    {
        var runner = new NUnitTestAssemblyRunner(new DefaultTestAssemblyBuilder());
        await Task.Run(() => runner.Load(assembly, options ?? new Dictionary<string, object>()));
        return runner;
    }
}