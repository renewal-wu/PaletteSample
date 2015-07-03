using System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace PaletteSample
{
    public sealed partial class MainPage : Page
    {
        MainPageViewModel viewModel { get; set; }
        public MainPage()
        {
            this.InitializeComponent();
            viewModel = new MainPageViewModel();
            this.DataContext = viewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ApplicationView CurrentApplicationView = ApplicationView.GetForCurrentView();
            if (CurrentApplicationView != null)
            {
                CurrentApplicationView.SetDesiredBoundsMode(ApplicationViewBoundsMode.UseCoreWindow);
            }
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Uri clickedImageUri = e.ClickedItem as Uri;
            if (clickedImageUri != null)
            {
                this.Frame.Navigate(typeof(DemoColorPage), clickedImageUri);
            }
        }
    }
}
