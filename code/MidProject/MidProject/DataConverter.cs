using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace MidProject
{
    public class DataConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool? completed = value as bool?;
            if (completed == null || completed == false)
                return Visibility.Collapsed;
            else return Visibility.Visible;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}