using Windows.UI.Xaml.Input;

namespace SimpleRtspPlayer
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;

    using Windows.ApplicationModel.Resources;
    using Windows.Storage;
    using Windows.UI.Popups;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Controls.Primitives;
    using Windows.UI.Xaml.Markup;

    public sealed partial class MainPage
    {
        private readonly ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        private NavigationViewItem prevNavItem;
        private NavigationViewItem addRtspNavigationViewItem;
        private NavigationViewItem removeNavigationItem;

        private ApplicationDataCompositeValue listOfCameras;

        public MainPage()
        {
            InitializeComponent();
            AddAddMenuItem();
            AddRemoveMenuItem();
            IconPicker.ItemsSource = Enum.GetValues(typeof(Symbol)).Cast<Symbol>();
            IconPicker.SelectedItem = Symbol.Home;
            ReadCameras();
        }

        public ObservableCollection<NavigationViewItemBase> MenuItems { get; set; } =
            new ObservableCollection<NavigationViewItemBase>();

        private void AddNewStreamOnTapped(object sender, TappedRoutedEventArgs e)
        {
            removeNavigationItem.IsEnabled = false;
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }

        private void RemoveStreamOnTapped(object sender, TappedRoutedEventArgs e)
        {
            removeNavigationItem.IsEnabled = false;
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }

        private async void ButtonOkOnTapped(object sender, TappedRoutedEventArgs e)
        {
            if (MenuItems.Where(cam => cam is NavigationViewItem)
                .Select(cam => (cam as NavigationViewItem).Content.ToString()).Contains(CameraName.Text))
            {
                var loader = new ResourceLoader();
                MessageDialog dialog = new MessageDialog(loader.GetString("CameraSameNameError"));
                await dialog.ShowAsync();
                return;
            }

            var navItem = AddCameraNavItem();

            listOfCameras[navItem.Content.ToString()] = $"{((SymbolIcon)(navItem.Icon)).Symbol},{navItem.Tag}";
            localSettings.Values["listOfCameras"] = listOfCameras;

            NewRtspStreamFlyout.Hide();
        }

        private NavigationViewItem AddCameraNavItem(string name = null, Symbol? icon = null, string cameraUri = null)
        {
            if (MenuItems.Count == 2) AddNavigationViewItemHeader();

            var navItem = new NavigationViewItem
                              {
                                  Content = name ?? CameraName.Text,
                                  Icon = new SymbolIcon(
                                      icon ?? (Symbol)(IconPicker.SelectedItem ?? Symbol.Home)),
                                  Tag = cameraUri ?? CameraUri.Text
                              };
            navItem.Tapped += CameraTapped;
            navItem.CanDrag = true;

            MenuItems.Add(navItem);
            return navItem;
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
            removeNavigationItem.IsEnabled = true;
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
                removeNavigationItem.IsEnabled = false;

                FlyoutBase.SetAttachedFlyout(removeNavigationItem, (Flyout)Resources["RemoveRtspStreamFlyout"]);
                removeNavigationItem.Tapped += RemoveStreamOnTapped;
            }

            MenuItems.Add(removeNavigationItem);
        }

        private void RemoveOkTapped(object sender, TappedRoutedEventArgs e)
        {
            listOfCameras.Remove(prevNavItem.Content?.ToString());
            localSettings.Values["listOfCameras"] = listOfCameras;

            MenuItems.Remove(prevNavItem);

            if (MenuItems.Count == 3) MenuItems.RemoveAt(2);

            RemoveRtspStreamFlyout.Hide();
        }

        private void RemoveCancelTapped(object sender, TappedRoutedEventArgs e)
        {
            RemoveRtspStreamFlyout.Hide();
        }

        private void NavigationView_OnSelectionChanged(
            NavigationView sender,
            NavigationViewSelectionChangedEventArgs args)
        {
            if (!args.SelectedItem.Equals(addRtspNavigationViewItem) && !args.SelectedItem.Equals(removeNavigationItem))
                prevNavItem = args.SelectedItem as NavigationViewItem;
        }

        private void FrameworkElement_OnLoaded(object sender, RoutedEventArgs e)
        {
            CameraName.Text = string.Empty;
            CameraUri.Text = string.Empty;
            IconPicker.SelectedItem = Symbol.Home;
        }

        private void ReadCameras()
        {
            listOfCameras = (ApplicationDataCompositeValue)localSettings.Values["listOfCameras"];
            if (listOfCameras == null)
            {
                listOfCameras = new ApplicationDataCompositeValue();
                return;
            }

            foreach (var camera in listOfCameras)
            {
                var cameraName = camera.Key;
                var cameraParams = (string)camera.Value;
                var indexOfSemiColon = cameraParams.IndexOf(',');
                var icon = (Symbol)Enum.Parse(typeof(Symbol), cameraParams.Substring(0, indexOfSemiColon));
                var cameraUri = cameraParams.Substring(
                    indexOfSemiColon + 1,
                    cameraParams.Length - indexOfSemiColon - 1);
                AddCameraNavItem(cameraName, icon, cameraUri);
            }
        }
    }
}
