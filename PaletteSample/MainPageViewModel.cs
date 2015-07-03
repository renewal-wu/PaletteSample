using System;
using System.Collections.Generic;

namespace PaletteSample
{
    public class MainPageViewModel
    {
        public List<Uri> ImageSources { get; set; }

        public MainPageViewModel()
        {
            ImageSources = new List<Uri>();

            for (Int32 i = 1; i < 103; i++)
            {
                ImageSources.Add(new Uri("ms-appx:///Assets/Sample/image_" + i.ToString() + ".jpg", UriKind.Absolute));
            }
        }
    }
}
