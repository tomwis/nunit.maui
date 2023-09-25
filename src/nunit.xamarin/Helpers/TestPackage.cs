// ***********************************************************************
// Copyright (c) 2017 NUnit Project
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using NUnit.Framework.Api;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using nunit.xamarin.Factories;
using nunit.xamarin.Helpers;

namespace NUnit.Runner.Helpers
{
    /// <summary>
    /// Contains all assemblies for a test run, and controls execution of tests and collection of results
    /// </summary>
    internal class TestPackage
    {
        private readonly List<(Assembly, Dictionary<string,object>)> _testAssemblies = new List<(Assembly, Dictionary<string,object>)>();
        
        public void AddAssembly(Assembly testAssembly, Dictionary<string,object> options = null)
        {
            _testAssemblies.Add((testAssembly, options));
        }

        public async Task<TestRunResult> ExecuteTests()
        {
            var resultPackage = new TestRunResult();

            foreach (var (assembly,options) in _testAssemblies)
            {
                var runner = await TestAssemblyRunnerFactory.CreateTestAssemblyRunner(assembly, options)
                    .ConfigureAwait(false);
                
                var testFilter = GetTestFilters();
                var result = await Task.Run(() => runner.Run(TestListener.NULL, testFilter))
                    .ConfigureAwait(false);
                
                resultPackage.AddResult(result);
            }
            
            resultPackage.CompleteTestRun();
            return resultPackage;
        }

        protected virtual ITestFilter GetTestFilters() => TestFilter.Empty;
    }
}
