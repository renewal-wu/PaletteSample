using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Media.Imaging;

namespace PaletteSample
{
    public class GaussianBlurHelper
    {
        public static WriteableBitmap GetMiddleGaussianImage(WriteableBitmap writeableBitmap)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var height = writeableBitmap.PixelHeight;
            var width = writeableBitmap.PixelWidth;
            var cropedWriteableBitmap = writeableBitmap.Crop(new Rect(new Point(0, height / 3), new Point(width, height / 3 * 2)));
            WriteableBitmap resizedWriteableBitmap = cropedWriteableBitmap.Resize(width / 8, height / 8, WriteableBitmapExtensions.Interpolation.Bilinear);
            WriteableBitmap gaussianBackground = resizedWriteableBitmap.Convolute(WriteableBitmapExtensions.KernelGaussianBlur5x5);
            
            stopwatch.Stop();
            Debug.WriteLine("GetMiddleGaussianImage time:" + stopwatch.ElapsedMilliseconds.ToString());
            
            return gaussianBackground;
        }
    }
}
