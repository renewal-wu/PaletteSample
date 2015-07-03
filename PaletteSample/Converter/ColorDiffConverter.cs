using KKBOX.Utility;
using System;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace PaletteSample.Converter
{
    public class ColorDiffConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (targetType == typeof(Color))
            {
                Color originColor = (Color)value;
                return ColorUtils.DiffColor(originColor);
            }
            else if (targetType == typeof(Brush))
            {
                SolidColorBrush originBrush = value as SolidColorBrush;
                if (originBrush != null)
                {
                    return new SolidColorBrush(ColorUtils.DiffColor(originBrush.Color));
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
