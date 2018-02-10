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
        private NavigationViewItem selecteViewItem;
        private NavigationViewItem addRtspNavigationViewItem;
        private NavigationViewItem removeNavigationItem;

        public MainPage()
        {
            InitializeComponent();
            AddAddMenuItem();
            AddNavigationViewItemHeader();
            IconPicker.ItemsSource = Enum.GetValues(typeof(Symbol)).Cast<Symbol>();
            IconPicker.SelectedItem = Symbol.Home;
        }

        public NavigationViewItem SelectedItem
        {
            get => selecteViewItem;
            set
            {
                if (value.Equals(addRtspNavigationViewItem) || value.Equals(removeNavigationItem))
                    NavigationView.SelectedItem = selecteViewItem;
                else selecteViewItem = value;
            }
        }

        public ObservableCollection<NavigationViewItemBase> MenuItems { get; set; } =
            new ObservableCollection<NavigationViewItemBase>();

        private static void AddNewStreamOnTapped(object sender, TappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }

        private static void RemoveStreamOnTapped(object sender, TappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }

        private void ButtonOkOnTapped(object sender, TappedRoutedEventArgs e)
        {
            var navItem = new NavigationViewItem
                              {
                                  Content = CameraName.Text,
                                  Icon = new SymbolIcon(
                                      (Symbol)(IconPicker.SelectedItem ?? Symbol.Home))
                              };
            navItem.Tapped += CameraTapped;

            MenuItems.Add(navItem);
            NewRtspStreamFlyout.Hide();
        }

        private void ButtonCancelOnTapped(object sender, TappedRoutedEventArgs e)
        {
            NewRtspStreamFlyout.Hide();
        }

        private void AddAddMenuItem()
        {
            addRtspNavigationViewItem = (NavigationViewItem)XamlReader.Load(
                "<NavigationViewItem "
                + "xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" "
                + "xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" "
                + "x:Uid=\"AddCameraNavItem\" "
                + "Icon=\"Add\" />");

            FlyoutBase.SetAttachedFlyout(addRtspNavigationViewItem, (Flyout)Resources["NewRtspStreamFlyout"]);
            addRtspNavigationViewItem.Tapped += AddNewStreamOnTapped;
            MenuItems.Add(addRtspNavigationViewItem);
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

        private void CameraTapped(object sender, TappedRoutedEventArgs e)
        {
            AddRemoveMenuItem();
        }

        private void AddRemoveMenuItem()
        {
            if (removeNavigationItem == null)
            {
                removeNavigationItem = (NavigationViewItem)XamlReader.Load(
                    "<NavigationViewItem " + "xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" "
                    + "xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" " 
                    + "x:Uid=\"RemoveCameraNavItem\" "
                    + "Icon=\"Remove\" />");

                FlyoutBase.SetAttachedFlyout(removeNavigationItem, (Flyout)Resources["RemoveRtspStreamFlyout"]);
                removeNavigationItem.Tapped += RemoveStreamOnTapped;
            }

            if (!MenuItems.Contains(removeNavigationItem)) MenuItems.Insert(1, removeNavigationItem);
        }

        private void RemoveOkTapped(object sender, TappedRoutedEventArgs e)
        {
            MenuItems.Remove(SelectedItem);

            if (MenuItems.Count == 2) MenuItems.Remove(removeNavigationItem);
            RemoveRtspStreamFlyout.Hide();
        }

        private void RemoveCancelTapped(object sender, TappedRoutedEventArgs e)
        {
            RemoveRtspStreamFlyout.Hide();
        }
    }
}
