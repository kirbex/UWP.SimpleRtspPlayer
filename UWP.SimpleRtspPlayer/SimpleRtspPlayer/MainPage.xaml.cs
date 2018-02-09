using Windows.UI.Xaml.Input;

namespace SimpleRtspPlayer
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;

    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Controls.Primitives;
    using Windows.UI.Xaml.Markup;

    public sealed partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
            AddAddMenuItem();
            AddNavigationViewItemHeader();
            IconPicker.ItemsSource = Enum.GetValues(typeof(Symbol)).Cast<Symbol>();
            IconPicker.SelectedItem = Symbol.Home;
        }

        public ObservableCollection<NavigationViewItemBase> MenuItems { get; set; } =
            new ObservableCollection<NavigationViewItemBase>();

        private static void AddNewStreamOnTapped(object sender, TappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }

        private void ButtonOkOnTapped(object sender, TappedRoutedEventArgs e)
        {
            MenuItems.Add(
                new NavigationViewItem
                    {
                        Content = CameraName.Text,
                        Icon = new SymbolIcon((Symbol)(IconPicker.SelectedItem ?? Symbol.Home))
                    });
            NewRtspStreamFlyout.Hide();
        }

        private void ButtonCancelOnTapped(object sender, TappedRoutedEventArgs e)
        {
            NewRtspStreamFlyout.Hide();
        }

        private void AddAddMenuItem()
        {
            var navItem = (NavigationViewItem)XamlReader.Load(
                "<NavigationViewItem "
                + "xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" "
                + "xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" "
                + "x:Uid=\"AddCameraNavItem\" "
                + "Icon=\"Add\" />");

            FlyoutBase.SetAttachedFlyout(navItem, (Flyout)Resources["NewRtspStreamFlyout"]);
            navItem.Tapped += AddNewStreamOnTapped;
            MenuItems.Add(navItem);
        }

        private void AddNavigationViewItemHeader()
        {
            var navItem = (NavigationViewItemHeader)XamlReader.Load(
                "<NavigationViewItemHeader "
                + "xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" "
                + "xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" "
                + "x:Uid=\"CamerasNavViewItemHeader\" "
                + "Margin=\"33,0,0,0\" />");

            MenuItems.Add(navItem);
        }
    }
}
