using System;
using Windows.UI;
using Windows.UI.Xaml.Data;

namespace PaletteSample.Converter
{
    public class ColorToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is Color && targetType == typeof(String))
            {
                Color originColor = (Color)value;
                return String.Format("#{0:X}{1:X}{2:X}{3:X}", originColor.A, originColor.R, originColor.G, originColor.B);
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
