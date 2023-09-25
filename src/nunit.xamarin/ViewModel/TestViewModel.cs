// ***********************************************************************
// Copyright (c) 2015 NUnit Project
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

using System;
using System.Reflection;
using System.Text;
using System.Windows.Input;
using NUnit.Framework.Interfaces;
using NUnit.Runner.Extensions;
using NUnit.Runner.Helpers;
using nunit.xamarin.Helpers;

namespace NUnit.Runner.ViewModel
{
    class TestViewModel : BaseViewModel
    {
        private readonly IReadOnlyList<Assembly> _testAssemblies;
        private ITestResult _testResult;
        private string _message;
        private string _output;
        private string _stackTrace;

        public TestViewModel(ITestResult result, IReadOnlyList<Assembly> testAssemblies)
        {
            _testAssemblies = testAssemblies;
            TestResult = result;
            Message = StringOrNone(result.Message);
            Output = StringOrNone(result.Output);
            StackTrace = StringOrNone(result.StackTrace);
            RunTestCommand = new Command(async () => await RunTest());

            var builder = new StringBuilder();
            var props = result.Test.Properties;
            foreach (string key in props.Keys)
            {
                foreach (var value in props[key])
                {
                    builder.AppendFormat("{0} = {1}{2}", key, value, Environment.NewLine);
                }
            }
            Properties = StringOrNone(builder.ToString());
        }

        public ITestResult TestResult
        {
            get => _testResult;
            private set
            {
                _testResult = value;
                OnPropertyChanged();
            }
        }

        public string Message
        {
            get => _message;
            private set
            {
                _message = value;
                OnPropertyChanged();
            }
        }

        public string Output
        {
            get => _output;
            private set
            {
                _output = value;
                OnPropertyChanged();
            }
        }

        public string StackTrace
        {
            get => _stackTrace;
            private set
            {
                _stackTrace = value;
                OnPropertyChanged();
            }
        }

        public string Properties { get; private set; }
        public ICommand RunTestCommand { get; private set; }

        private async Task RunTest()
        {
            var testResultFullName = TestResult.FullName;
            var testPackage = new SingleTestPackage(testResultFullName);
            _testAssemblies.ToList().ForEach(asm => testPackage.AddAssembly(asm));
            var results = await testPackage.ExecuteTests();
            TestResult = GetCurrentTest(results.TestResults, testResultFullName);
            Message = StringOrNone(TestResult.Message);
            Output = StringOrNone(TestResult.Output);
            StackTrace = StringOrNone(TestResult.StackTrace);
        }

        private ITestResult GetCurrentTest(IEnumerable<ITestResult> testResults, string testFullName)
        {
            ITestResult result = null;
            foreach (var testResult in testResults)
            {
                result = GetCurrentTest(testResult, testFullName);
                if (result is not null)
                {
                    break;
                }
            }

            return result;
        }

        private ITestResult GetCurrentTest(ITestResult testResult, string testFullName)
        {
            if (testResult.Test.IsSuite)
            {
                return GetCurrentTest(testResult.Children, testFullName);
            }

            return testResult.FullName == testFullName ? testResult : null;
        }

        /// <summary>
        /// Gets the color for this result.
        /// </summary>
        public Color Color
        {
            get { return TestResult.ResultState.Color(); }
        }

        private string StringOrNone(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return "<none>";
            return str;
        }
    }
}
