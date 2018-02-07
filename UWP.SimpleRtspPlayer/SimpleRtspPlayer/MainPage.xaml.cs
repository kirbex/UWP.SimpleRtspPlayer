using Windows.UI.Xaml.Input;

namespace SimpleRtspPlayer
{
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls.Primitives;

    public sealed partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void AddNewStreamOnTapped(object sender, TappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }

        private void ButtonOkOnTapped(object sender, TappedRoutedEventArgs e)
        {
            
        }

        private void ButtonCancelOnTapped(object sender, TappedRoutedEventArgs e)
        {
            NewRtspStreamFlyout.Hide();
        }
    }
}
