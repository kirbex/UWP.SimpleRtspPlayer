namespace SimpleRtspPlayer
{
    using System;

    using Windows.UI.Xaml.Data;

    public class ReferenceTypeToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language) => value != null;

        public object ConvertBack(object value, Type targetType, object parameter, string language) =>
            throw new NotSupportedException();
    }
}
