using KKBOX.Utility;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;

namespace PaletteSample
{
    public class DemoColorPageViewModel
    {
        public ObservableCollection<DemoItemData> DemoItems { get; set; }
        public DemoColorPageViewModel()
        {
            DemoItems = new ObservableCollection<DemoItemData>();
        }

        public async Task SetDemoItem(Uri imageUri)
        {
            DemoItems.Clear();

            WriteableBitmap writeableBitmap = await BitmapFactory.New(1, 1).FromContent(imageUri);
            Palette palette = await Palette.Generate(writeableBitmap);

            DemoItems.Add(generateDemoItemData(imageUri, palette.getVibrantColor(Colors.Transparent), "Vibrant"));
            DemoItems.Add(generateDemoItemData(imageUri, palette.getLightVibrantColor(Colors.Transparent), "LightVibrant"));
            DemoItems.Add(generateDemoItemData(imageUri, palette.getDarkVibrantColor(Colors.Transparent), "DarkVibrant"));
            DemoItems.Add(generateDemoItemData(imageUri, palette.getMutedColor(Colors.Transparent), "Muted"));
            DemoItems.Add(generateDemoItemData(imageUri, palette.getLightMutedColor(Colors.Transparent), "LightMuted"));
            DemoItems.Add(generateDemoItemData(imageUri, palette.getDarkMutedColor(Colors.Transparent), "DarkMuted"));
        }

        private DemoItemData generateDemoItemData(Uri imageUri, Color color, String colorTypeName)
        {
            return new DemoItemData()
            {
                ImageUri = imageUri,
                BackgroundColor = color,
                ColorTypeName = color == Colors.Transparent ? colorTypeName + ", Failed" : colorTypeName
            };
        }

        public void UpdateOpacity(Double newOpacity)
        {
            if (DemoItems != null)
            {
                newOpacity = newOpacity / 100;
                foreach (var item in DemoItems)
                {
                    item.BackgroundOpacity = newOpacity;
                }
            }
        }
    }

    public class DemoItemData : INotifyPropertyChanged
    {
        private Uri imageUri;
        public Uri ImageUri
        {
            get
            {
                return imageUri;
            }
            set
            {
                imageUri = value;
                NotifyPropertyChanged("ImageUri");
            }
        }

        private Color backgroundColor;
        public Color BackgroundColor
        {
            get
            {
                return backgroundColor;
            }
            set
            {
                backgroundColor = value;
                NotifyPropertyChanged("BackgroundColor");
            }
        }

        private String colorTypeName;
        public String ColorTypeName
        {
            get
            {
                return colorTypeName;
            }
            set
            {
                colorTypeName = value;
                NotifyPropertyChanged("ColorTypeName");
            }
        }

        private Double backgroundOpacity = 1.0;
        public Double BackgroundOpacity
        {
            get
            {
                return backgroundOpacity;
            }
            set
            {
                backgroundOpacity = value;
                NotifyPropertyChanged("BackgroundOpacity");
            }
        }

        #region Property Changed
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
