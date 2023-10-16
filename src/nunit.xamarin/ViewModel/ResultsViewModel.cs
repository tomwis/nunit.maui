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

using System.Collections.ObjectModel;
using System.Reflection;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Runner.View;
using nunit.xamarin.Enums;

namespace NUnit.Runner.ViewModel
{
    class ResultsViewModel : BaseViewModel
    {
        private readonly List<Assembly> _testAssemblies;
        private ObservableCollection<ResultViewModel> _results;

        /// <summary>
        /// Constructs the view model
        /// </summary>
        /// <param name="results">The package of all results in run</param>
        /// <param name="testAssemblies"></param>
        /// <param name="viewAll">If true, views all tests, otherwise only shows those
        ///     that did not pass</param>
        public ResultsViewModel(IReadOnlyCollection<ITestResult> results, List<Assembly> testAssemblies, bool viewAll)
        {
            _testAssemblies = testAssemblies;
            Results = new ObservableCollection<ResultViewModel>();
            foreach (var result in results)
                AddTestResults(result, viewAll);
        }

        /// <summary>
        /// A list of tests that did not pass
        /// </summary>
        public ObservableCollection<ResultViewModel> Results
        {
            get => _results;
            private set
            {
                if (Equals(value, _results)) return;
                _results = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Add all tests that did not pass to the Results collection
        /// </summary>
        /// <param name="result"></param>
        /// <param name="viewAll"></param>
        private void AddTestResults(ITestResult result, bool viewAll)
        {
            if (result.Test.IsSuite)
            {
                foreach (var childResult in result.Children)
                    AddTestResults(childResult, viewAll);
            }
            else if (viewAll || result.ResultState.Status != TestStatus.Passed)
            {
                Results.Add(new ResultViewModel(result, _testAssemblies));
            }
        }

        public void SortItems(SortOption sortOption, SortDirection sortDirection)
        {
            var sorted = sortOption switch
            {
                SortOption.TestName => SortBy(result => result.Name),
                SortOption.ParentName => SortBy(result => result.Parent),
                SortOption.Duration => SortBy(result => result.DurationMs),
                _ => throw new ArgumentOutOfRangeException(nameof(sortOption), sortOption, null)
            };

            Results = new ObservableCollection<ResultViewModel>(sorted);

            IOrderedEnumerable<ResultViewModel> SortBy<T>(Func<ResultViewModel, T> sortingPropertyFunc)
            {
                return sortDirection == SortDirection.Ascending 
                    ? Results.OrderBy(sortingPropertyFunc) 
                    : Results.OrderByDescending(sortingPropertyFunc);
            }
        }
    }
}
