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

using NUnit.Runner.ViewModel;
using nunit.xamarin.Constants;
using nunit.xamarin.Enums;

namespace NUnit.Runner.View
{
    /// <summary>
    /// Xamarin.Forms view of a list of test results
    /// </summary>
    public partial class ResultsView : ContentPage
	{
        private readonly ResultsViewModel _viewModel;

        internal ResultsView (ResultsViewModel model)
        {
            _viewModel = model;
            model.Navigation = Navigation;
            BindingContext = model;
            InitializeComponent();
		}

        internal async void ViewTest(object sender, SelectedItemChangedEventArgs e)
        {
                var result = e.SelectedItem as ResultViewModel;
                if (result != null)
                {
                    await Navigation.PushAsync(
                        new TestView(
                            new TestViewModel(result.TestResult, result.TestAssemblies)));
                }
        }

        private async void Sort_OnClicked(object sender, EventArgs e)
        {
            const string cancelOptionText = "Cancel";
            var options = new []
            {
                SortingNames.SortOptionTestNameAsc,
                SortingNames.SortOptionTestNameDesc,
                SortingNames.SortOptionParentNameAsc,
                SortingNames.SortOptionParentNameDesc,
                SortingNames.SortOptionDurationAsc,
                SortingNames.SortOptionDurationDesc,
            };
            var result = await DisplayActionSheet("Sort results", cancelOptionText, null, options);
            if (result == cancelOptionText) return;
            
            var (sortOption, sortDirection) = ParseSortingResult(result);
            if (sortOption is null || sortDirection is null) return;
            _viewModel.SortItems(sortOption.Value, sortDirection.Value);
        }

        private static (SortOption?, SortDirection?) ParseSortingResult(string result)
        {
            SortOption? sortOption = SortOption.TestName;
            SortDirection? sortDirection = SortDirection.Ascending;
            switch (result)
            {
                case SortingNames.SortOptionTestNameAsc:
                    sortOption = SortOption.TestName;
                    sortDirection = SortDirection.Ascending;
                    break;
                case SortingNames.SortOptionTestNameDesc:
                    sortOption = SortOption.TestName;
                    sortDirection = SortDirection.Descending;
                    break;
                case SortingNames.SortOptionParentNameAsc:
                    sortOption = SortOption.ParentName;
                    sortDirection = SortDirection.Ascending;
                    break;
                case SortingNames.SortOptionParentNameDesc:
                    sortOption = SortOption.ParentName;
                    sortDirection = SortDirection.Descending;
                    break;
                case SortingNames.SortOptionDurationAsc:
                    sortOption = SortOption.Duration;
                    sortDirection = SortDirection.Ascending;
                    break;
                case SortingNames.SortOptionDurationDesc:
                    sortOption = SortOption.Duration;
                    sortDirection = SortDirection.Descending;
                    break;
            }

            return (sortOption, sortDirection);
        }
    }
}
