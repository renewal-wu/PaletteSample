using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Navigation;

namespace PaletteSample
{
    public sealed partial class DemoColorPage : Page
    {
        DemoColorPageViewModel viewModel { get; set; }
        Uri currentImageUri { get; set; }
        public DemoColorPage()
        {
            this.Loaded += Frame_Loaded;
            this.Unloaded += Frame_Unloaded;
            this.InitializeComponent();
            viewModel = new DemoColorPageViewModel();
            this.DataContext = viewModel;
        }

        void Frame_Loaded(object sender, RoutedEventArgs e)
        {
            registerBackKey();
        }

        void Frame_Unloaded(object sender, RoutedEventArgs e)
        {
            unregisterBackKey();
        }

        private void registerBackKey()
        {
            Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
        }

        private void unregisterBackKey()
        {
            Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            Windows.Phone.UI.Input.HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
        }

        private void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            e.Handled = true;

            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }

        private void hideProgressBar()
        {
            DemoProgressBar.IsIndeterminate = false;
            DemoProgressBar.Visibility = Visibility.Collapsed;
        }

        private void showProgressBar()
        {
            DemoProgressBar.IsIndeterminate = true;
            DemoProgressBar.Visibility = Visibility.Visible;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var parameterUri = e.Parameter as Uri;
            if (parameterUri != null)
            {
                currentImageUri = parameterUri;
                showProgressBar();
                await viewModel.SetDemoItem(currentImageUri);
                hideProgressBar();
            }
            else
            {
                this.Frame.GoBack();
            }
        }

        private void RootBackgroundOpacitySlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (viewModel != null)
            {
                viewModel.UpdateOpacity(e.NewValue);
            }
        }
    }
}
