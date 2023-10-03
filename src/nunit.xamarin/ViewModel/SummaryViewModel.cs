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
using NUnit.Framework.Api;
using NUnit.Framework.Internal;
using NUnit.Runner.Helpers;
using NUnit.Runner.Services;
using NUnit.Runner.View;
using nunit.xamarin.Constants;
using nunit.xamarin.Factories;
using nunit.xamarin.Helpers;

namespace NUnit.Runner.ViewModel;

internal class SummaryViewModel : BaseViewModel
{
    private readonly List<(Assembly Assembly, Dictionary<string, object> Options)> _testAssemblies = new();
    private TestResultProcessor _resultProcessor;
    private ResultSummary _results;
    private bool _running;

    private TestOptions options;
    private Exception _runException;

    public SummaryViewModel()
    {
        RunTestsCommand = new Command(async o => await ExecuteTestsAsync(), o => !Running);
        ViewAllResultsCommand = new Command(
            async o => await Navigation.PushAsync(
                new ResultsView(new ResultsViewModel(_results.GetTestResults(), _testAssemblies.Select(t => t.Assembly).ToList(), true))),
            o => !HasResults);
        ViewFailedResultsCommand = new Command(
            async o => await Navigation.PushAsync(
                new ResultsView(new ResultsViewModel(_results.GetTestResults(), _testAssemblies.Select(t => t.Assembly).ToList(), false))),
            o => !HasResults);
    }

    /// <summary>
    ///     User options for the test suite.
    /// </summary>
    public TestOptions Options
    {
        get
        {
            if (options == null) options = new TestOptions();
            return options;
        }
        set => options = value;
    }

    /// <summary>
    ///     The overall test results
    /// </summary>
    public ResultSummary Results
    {
        get => _results;
        set
        {
            if (Equals(value, _results)) return;
            _results = value;
            OnPropertyChanged();
            OnPropertyChanged("HasResults");
        }
    }

    /// <summary>
    ///     True if tests are currently running
    /// </summary>
    public bool Running
    {
        get => _running;
        set
        {
            if (value.Equals(_running)) return;
            _running = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    ///     True if we have test results to display
    /// </summary>
    public bool HasResults => Results != null;

    public ICommand RunTestsCommand { set; get; }
    public ICommand ViewAllResultsCommand { set; get; }
    public ICommand ViewFailedResultsCommand { set; get; }

    /// <summary>
    ///     Called from the view when the view is appearing
    /// </summary>
    public void OnAppearing()
    {
        if (Options.AutoRun)
        {
            // Don't rerun if we navigate back
            Options.AutoRun = false;
            RunTestsCommand.Execute(null);
        }
    }

    public static void TerminateWithSuccess()
    {
#if __IOS__
            var selector = new ObjCRuntime.Selector("terminateWithSuccess");
            UIKit.UIApplication.SharedApplication.PerformSelector(selector, UIKit.UIApplication.SharedApplication, 0);
#elif __DROID__
            System.Environment.Exit(0);
#elif WINDOWS_UWP
            Windows.UI.Xaml.Application.Current.Exit();
#endif
    }

    private async Task ExecuteTestsAsync()
    {
        Running = true;
        Results = null;
        await Task.Run(async () =>
        {
            try
            {
                var testPackage = await SelectTestPackage();
                foreach (var tuple in _testAssemblies)
                {
                    testPackage.AddAssembly(tuple.Assembly, tuple.Options);
                }

                var results = await testPackage.ExecuteTests();
                var summary = new ResultSummary(results);

                _resultProcessor = TestResultProcessor.BuildChainOfResponsability(Options);
                await _resultProcessor.Process(summary).ConfigureAwait(false);
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Results = summary;
                    Running = false;

                    if (Options.TerminateAfterExecution)
                        TerminateWithSuccess();
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                await MainThread.InvokeOnMainThreadAsync(() => RunException = e);
            }
            finally
            {
                await MainThread.InvokeOnMainThreadAsync(() => Running = false);
            }
        });
    }

    public Exception RunException
    {
        get => _runException;
        set
        {
            if (Equals(value, _runException)) return;
            _runException = value;
            OnPropertyChanged();
        }
    }

    private async Task<TestPackage> SelectTestPackage()
    {
        var testPackage = new TestPackage();
        const string categoryName = CategoryNames.RunOnlyThis;
        var testFilter = TestRunnerFilterFactory.CreateCategoryFilter(categoryName);
        foreach (var assembly in _testAssemblies)
        {
            var runner = await TestAssemblyRunnerFactory.CreateTestAssemblyRunner(assembly.Assembly, assembly.Options)
                .ConfigureAwait(false);
            
            var count = runner.CountTestCases(testFilter);
            if (count > 0)
            {
                testPackage = new CategoryTestPackage(categoryName);
                break;
            }
        }

        return testPackage;
    }

    /// <summary>
    ///     Adds an assembly to be tested.
    /// </summary>
    /// <param name="testAssembly">The test assembly.</param>
    /// <returns></returns>
    internal void AddTest(Assembly testAssembly, Dictionary<string, object> options = null)
    {
        var assemblyName = testAssembly.ToString();
        var alreadyAdded = _testAssemblies.Any(tuple => tuple.Assembly.ToString() == assemblyName);
        if (alreadyAdded) return;
        
        _testAssemblies.Add((testAssembly, options));
    }
}