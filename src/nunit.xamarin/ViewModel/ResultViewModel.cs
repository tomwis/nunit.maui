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

using System.Reflection;
using System.Windows.Input;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Runner.Extensions;
using NUnit.Runner.Helpers;

namespace NUnit.Runner.ViewModel
{
    internal class ResultViewModel
    {
        private const double MillisecondsInSecond = 1000.0;
        private readonly List<Assembly> _testAssemblies;

        public ResultViewModel(ITestResult result, List<Assembly> testAssemblies)
        {
            _testAssemblies = testAssemblies;
            TestResult = result;
            Name = result.Name;
            Parent = result.Test.Parent.FullName;
            if (result is TestCaseResult testCaseResult)
            {
                DurationMs = testCaseResult.Duration * MillisecondsInSecond;
            }
            
            Result = result.ResultState.Status.ToString().ToUpperInvariant();
            Message = result.Message;
        }

        public ITestResult TestResult { get; private set; }
        public string Result { get; set; }
        public string Name { get; private set; }
        public string Parent { get; private set; }
        public double DurationMs { get; private set; }
        public string Message { get; private set; }
        public IReadOnlyList<Assembly> TestAssemblies => _testAssemblies.AsReadOnly();

        /// <summary>
        /// Gets the color for this result.
        /// </summary>
        public Color Color
        {
            get { return TestResult.ResultState.Color(); }
        }
    }
}