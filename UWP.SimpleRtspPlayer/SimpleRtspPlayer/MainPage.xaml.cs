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

    public sealed partial class MainPage
    {
        private readonly ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        private ApplicationDataCompositeValue listOfCameras;

        public MainPage()
        {
            InitializeComponent();
            IconPicker.ItemsSource = Enum.GetValues(typeof(Symbol)).Cast<Symbol>();
            IconPicker.SelectedItem = Symbol.Home;
            ReadCameras();
        }

        public NavigationViewItem SelectedNavItem { get; set; }

        public ObservableCollection<NavigationViewItem> MenuItems { get; set; } =
            new ObservableCollection<NavigationViewItem>();

        private void AddNewStreamOnTapped(object sender, TappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }

        private void RemoveStreamOnTapped(object sender, TappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }

        private async void ButtonOkOnTapped(object sender, TappedRoutedEventArgs e)
        {
            if (MenuItems.Where(cam => cam.Content != null).Select(cam => cam.Content.ToString())
                .Contains(CameraName.Text))
            {
                var loader = new ResourceLoader();
                MessageDialog dialog = new MessageDialog(loader.GetString("CameraSameNameError"));
                await dialog.ShowAsync();
                return;
            }

            if (string.IsNullOrEmpty(CameraName.Text))
            {
                var loader = new ResourceLoader();
                MessageDialog dialog = new MessageDialog(loader.GetString("CameraNameIsEmpty"));
                await dialog.ShowAsync();
                return;
            }

            if (string.IsNullOrEmpty(CameraUri.Text))
            {
                var loader = new ResourceLoader();
                MessageDialog dialog = new MessageDialog(loader.GetString("CameraUriIsEmpty"));
                await dialog.ShowAsync();
                return;
            }

            var navItem = AddCameraNavItem();

            if (navItem.Content != null)
                listOfCameras[navItem.Content.ToString()] = $"{((SymbolIcon)(navItem.Icon)).Symbol},{navItem.Tag}";
            localSettings.Values["listOfCameras"] = listOfCameras;

            NewRtspStreamFlyout.Hide();
        }

        private NavigationViewItem AddCameraNavItem(string name = null, Symbol? icon = null, string cameraUri = null)
        {
            var navItem = new NavigationViewItem
                              {
                                  Content = name ?? CameraName.Text,
                                  Icon = new SymbolIcon(
                                      icon ?? (Symbol)(IconPicker.SelectedItem ?? Symbol.Home)),
                                  Tag = cameraUri ?? CameraUri.Text
                              };

            navItem.SetValue(ToolTipService.ToolTipProperty, navItem.Tag);

            MenuItems.Add(navItem);
            return navItem;
        }

        private void ButtonCancelOnTapped(object sender, TappedRoutedEventArgs e)
        {
            NewRtspStreamFlyout.Hide();
        }

        private void RemoveButtonCancelOnTapped(object sender, TappedRoutedEventArgs e)
        {
            RemoveRtspStreamFlyout.Hide();
        }

        private void RemoveOkTapped(object sender, TappedRoutedEventArgs e)
        {
            listOfCameras.Remove(SelectedNavItem.Content?.ToString());
            localSettings.Values["listOfCameras"] = listOfCameras;

            MenuItems.Remove(SelectedNavItem);

            RemoveRtspStreamFlyout.Hide();
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

        private void NavigationViewOnSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            SelectedNavItem = args.SelectedItem as NavigationViewItem;
        }
    }
}
