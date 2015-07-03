using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;

namespace KKBOX.Utility
{
    public class Palette
    {
        private static readonly int CALCULATE_BITMAP_MIN_DIMENSION = 100;
        private static readonly int DEFAULT_CALCULATE_NUMBER_COLORS = 16;

        private static readonly float TARGET_DARK_LUMA = 0.26f;
        private static readonly float MAX_DARK_LUMA = 0.45f;

        private static readonly float MIN_LIGHT_LUMA = 0.55f;
        private static readonly float TARGET_LIGHT_LUMA = 0.74f;

        private static readonly float MIN_NORMAL_LUMA = 0.3f;
        private static readonly float TARGET_NORMAL_LUMA = 0.5f;
        private static readonly float MAX_NORMAL_LUMA = 0.7f;

        private static readonly float TARGET_MUTED_SATURATION = 0.3f;
        private static readonly float MAX_MUTED_SATURATION = 0.4f;

        private static readonly float TARGET_VIBRANT_SATURATION = 1f;
        private static readonly float MIN_VIBRANT_SATURATION = 0.35f;

        private readonly List<Swatch> mSwatches;
        private readonly int mHighestPopulation;

        private Swatch mVibrantSwatch;
        private Swatch mMutedSwatch;

        private Swatch mDarkVibrantSwatch;
        private Swatch mDarkMutedSwatch;

        private Swatch mLightVibrantSwatch;
        private Swatch mLightMutedColor;

        private Palette(List<Swatch> swatches)
        {
            mSwatches = swatches;
            mHighestPopulation = findMaxPopulation();

            mVibrantSwatch = findColor(TARGET_NORMAL_LUMA, MIN_NORMAL_LUMA, MAX_NORMAL_LUMA,
                    TARGET_VIBRANT_SATURATION, MIN_VIBRANT_SATURATION, 1f);

            mLightVibrantSwatch = findColor(TARGET_LIGHT_LUMA, MIN_LIGHT_LUMA, 1f,
                    TARGET_VIBRANT_SATURATION, MIN_VIBRANT_SATURATION, 1f);

            mDarkVibrantSwatch = findColor(TARGET_DARK_LUMA, 0f, MAX_DARK_LUMA,
                    TARGET_VIBRANT_SATURATION, MIN_VIBRANT_SATURATION, 1f);

            mMutedSwatch = findColor(TARGET_NORMAL_LUMA, MIN_NORMAL_LUMA, MAX_NORMAL_LUMA,
                    TARGET_MUTED_SATURATION, 0f, MAX_MUTED_SATURATION);

            mLightMutedColor = findColor(TARGET_LIGHT_LUMA, MIN_LIGHT_LUMA, 1f,
                    TARGET_MUTED_SATURATION, 0f, MAX_MUTED_SATURATION);

            mDarkMutedSwatch = findColor(TARGET_DARK_LUMA, 0f, MAX_DARK_LUMA,
                    TARGET_MUTED_SATURATION, 0f, MAX_MUTED_SATURATION);

            // Now try and generate any missing colors
            generateEmptySwatches();
        }

        public static async Task<Palette> Generate(WriteableBitmap bitmap)
        {
            return await Generate(bitmap, DEFAULT_CALCULATE_NUMBER_COLORS);
        }

        public static async Task<Palette> Generate(WriteableBitmap bitmap, int numColors)
        {
            checkBitmapParam(bitmap);
            checkNumberColorsParam(numColors);

            // First we'll scale down the bitmap so it's shortest dimension is 100px
            WriteableBitmap scaledBitmap = scaleBitmapDown(bitmap);

            // Now generate a quantizer from the Bitmap
            ColorCutQuantizer quantizer = await ColorCutQuantizer.fromBitmap(scaledBitmap, numColors);

            // If created a new bitmap, recycle it
            if (scaledBitmap != bitmap)
            {
                bitmap = null;
            }

            // Now return a ColorExtractor instance
            return new Palette(quantizer.getQuantizedColors());
        }

        private static void checkBitmapParam(WriteableBitmap bitmap)
        {
            if (bitmap == null)
            {
                throw new ArgumentNullException("bitmap can not be null");
            }
        }

        private static void checkNumberColorsParam(int numColors)
        {
            if (numColors < 1)
            {
                throw new ArgumentException("numColors must be 1 of greater");
            }
        }

        private static WriteableBitmap scaleBitmapDown(WriteableBitmap bitmap)
        {
            int minDimension = Math.Min(bitmap.PixelWidth, bitmap.PixelHeight);

            if (minDimension <= CALCULATE_BITMAP_MIN_DIMENSION)
            {
                // If the bitmap is small enough already, just return it
                return bitmap;
            }

            float scaleRatio = CALCULATE_BITMAP_MIN_DIMENSION / (float)minDimension;

            WriteableBitmap resizedBitmap = bitmap.Resize((Int32)(bitmap.PixelWidth * scaleRatio), (Int32)(bitmap.PixelHeight * scaleRatio), WriteableBitmapExtensions.Interpolation.Bilinear);

            return resizedBitmap;
        }

        /**
     * Returns all of the swatches which make up the palette.
     */
        public IList<Swatch> getSwatches()
        {
            return new ReadOnlyCollection<Swatch>(mSwatches);
        }

        /**
         * Returns the most vibrant swatch in the palette. Might be null.
         */
        public Swatch getVibrantSwatch()
        {
            return mVibrantSwatch;
        }

        /**
         * Returns a light and vibrant swatch from the palette. Might be null.
         */
        public Swatch getLightVibrantSwatch()
        {
            return mLightVibrantSwatch;
        }

        /**
         * Returns a dark and vibrant swatch from the palette. Might be null.
         */
        public Swatch getDarkVibrantSwatch()
        {
            return mDarkVibrantSwatch;
        }

        /**
         * Returns a muted swatch from the palette. Might be null.
         */
        public Swatch getMutedSwatch()
        {
            return mMutedSwatch;
        }

        /**
         * Returns a muted and light swatch from the palette. Might be null.
         */
        public Swatch getLightMutedSwatch()
        {
            return mLightMutedColor;
        }

        /**
         * Returns a muted and dark swatch from the palette. Might be null.
         */
        public Swatch getDarkMutedSwatch()
        {
            return mDarkMutedSwatch;
        }

        /**
         * Returns the most vibrant color in the palette as an RGB packed int.
         *
         * @param defaultColor value to return if the swatch isn't available
         */
        public Color getVibrantColor(Color defaultColor)
        {
            return mVibrantSwatch != null ? mVibrantSwatch.GetRgb() : defaultColor;
        }

        /**
         * Returns a light and vibrant color from the palette as an RGB packed int.
         *
         * @param defaultColor value to return if the swatch isn't available
         */
        public Color getLightVibrantColor(Color defaultColor)
        {
            return mLightVibrantSwatch != null ? mLightVibrantSwatch.GetRgb() : defaultColor;
        }

        /**
         * Returns a dark and vibrant color from the palette as an RGB packed int.
         *
         * @param defaultColor value to return if the swatch isn't available
         */
        public Color getDarkVibrantColor(Color defaultColor)
        {
            return mDarkVibrantSwatch != null ? mDarkVibrantSwatch.GetRgb() : defaultColor;
        }

        /**
         * Returns a muted color from the palette as an RGB packed int.
         *
         * @param defaultColor value to return if the swatch isn't available
         */
        public Color getMutedColor(Color defaultColor)
        {
            return mMutedSwatch != null ? mMutedSwatch.GetRgb() : defaultColor;
        }

        /**
         * Returns a muted and light color from the palette as an RGB packed int.
         *
         * @param defaultColor value to return if the swatch isn't available
         */
        public Color getLightMutedColor(Color defaultColor)
        {
            return mLightMutedColor != null ? mLightMutedColor.GetRgb() : defaultColor;
        }

        /**
         * Returns a muted and dark color from the palette as an RGB packed int.
         *
         * @param defaultColor value to return if the swatch isn't available
         */
        public Color getDarkMutedColor(Color defaultColor)
        {
            return mDarkMutedSwatch != null ? mDarkMutedSwatch.GetRgb() : defaultColor;
        }

        /**
         * @return true if we have already selected {@code swatch}
         */
        private Boolean isAlreadySelected(Swatch swatch)
        {
            return mVibrantSwatch == swatch || mDarkVibrantSwatch == swatch ||
                    mLightVibrantSwatch == swatch || mMutedSwatch == swatch ||
                    mDarkMutedSwatch == swatch || mLightMutedColor == swatch;
        }

        private Swatch findColor(float targetLuma, float minLuma, float maxLuma,
                                 float targetSaturation, float minSaturation, float maxSaturation)
        {
            Swatch max = null;
            float maxValue = 0f;

            foreach (Swatch swatch in mSwatches)
            {
                float sat = swatch.getHsl()[1];
                float luma = swatch.getHsl()[2];

                if (sat >= minSaturation && sat <= maxSaturation &&
                        luma >= minLuma && luma <= maxLuma &&
                        !isAlreadySelected(swatch))
                {
                    float thisValue = createComparisonValue(sat, targetSaturation, luma, targetLuma,
                            swatch.getPopulation(), mHighestPopulation);
                    if (max == null || thisValue > maxValue)
                    {
                        max = swatch;
                        maxValue = thisValue;
                    }
                }
            }

            return max;
        }

        /**
         * Try and generate any missing swatches from the swatches we did find.
         */
        private void generateEmptySwatches()
        {
            if (mVibrantSwatch == null)
            {
                if (mDarkVibrantSwatch != null)
                {
                    float[] newHsl = copyHslValues(mDarkVibrantSwatch);
                    newHsl[2] = TARGET_NORMAL_LUMA;
                    mVibrantSwatch = new Swatch(ColorUtils.HSLtoRGB(newHsl), 0);
                }
                else if (mLightVibrantSwatch != null)
                {
                    float[] newHsl = copyHslValues(mLightVibrantSwatch);
                    newHsl[2] = TARGET_NORMAL_LUMA;
                    mVibrantSwatch = new Swatch(ColorUtils.HSLtoRGB(newHsl), 0);
                }
            }

            if (mDarkVibrantSwatch == null)
            {
                if (mVibrantSwatch != null)
                {
                    float[] newHsl = copyHslValues(mVibrantSwatch);
                    newHsl[2] = TARGET_DARK_LUMA;
                    mDarkVibrantSwatch = new Swatch(ColorUtils.HSLtoRGB(newHsl), 0);
                }
                else if (mLightVibrantSwatch != null)
                {
                    float[] newHsl = copyHslValues(mLightVibrantSwatch);
                    newHsl[2] = TARGET_DARK_LUMA;
                    mDarkVibrantSwatch = new Swatch(ColorUtils.HSLtoRGB(newHsl), 0);
                }
            }

            if (mLightVibrantSwatch == null)
            {
                if (mVibrantSwatch != null)
                {
                    float[] newHsl = copyHslValues(mVibrantSwatch);
                    newHsl[2] = TARGET_LIGHT_LUMA;
                    mLightVibrantSwatch = new Swatch(ColorUtils.HSLtoRGB(newHsl), 0);
                }
                else if (mDarkVibrantSwatch != null)
                {
                    float[] newHsl = copyHslValues(mDarkVibrantSwatch);
                    newHsl[2] = TARGET_LIGHT_LUMA;
                    mLightVibrantSwatch = new Swatch(ColorUtils.HSLtoRGB(newHsl), 0);
                }
            }

            if (mMutedSwatch == null)
            {
                if (mDarkMutedSwatch != null)
                {
                    float[] newHsl = copyHslValues(mDarkMutedSwatch);
                    newHsl[2] = TARGET_NORMAL_LUMA;
                    mMutedSwatch = new Swatch(ColorUtils.HSLtoRGB(newHsl), 0);
                }
                else if (mLightMutedColor != null)
                {
                    float[] newHsl = copyHslValues(mLightMutedColor);
                    newHsl[2] = TARGET_NORMAL_LUMA;
                    mMutedSwatch = new Swatch(ColorUtils.HSLtoRGB(newHsl), 0);
                }
            }

            if (mDarkMutedSwatch == null)
            {
                if (mMutedSwatch != null)
                {
                    float[] newHsl = copyHslValues(mMutedSwatch);
                    newHsl[2] = TARGET_DARK_LUMA;
                    mDarkMutedSwatch = new Swatch(ColorUtils.HSLtoRGB(newHsl), 0);
                }
                else if (mLightMutedColor != null)
                {
                    float[] newHsl = copyHslValues(mLightMutedColor);
                    newHsl[2] = TARGET_DARK_LUMA;
                    mDarkMutedSwatch = new Swatch(ColorUtils.HSLtoRGB(newHsl), 0);
                }
            }

            if (mLightMutedColor == null)
            {
                if (mMutedSwatch != null)
                {
                    float[] newHsl = copyHslValues(mMutedSwatch);
                    newHsl[2] = TARGET_LIGHT_LUMA;
                    mLightMutedColor = new Swatch(ColorUtils.HSLtoRGB(newHsl), 0);
                }
                else if (mDarkMutedSwatch != null)
                {
                    float[] newHsl = copyHslValues(mDarkMutedSwatch);
                    newHsl[2] = TARGET_LIGHT_LUMA;
                    mLightMutedColor = new Swatch(ColorUtils.HSLtoRGB(newHsl), 0);
                }
            }
        }

        /**
         * Find the {@link Swatch} with the highest population value and return the population.
         */
        private int findMaxPopulation()
        {
            return mSwatches.Max(s => s.getPopulation());
        }

        private static float createComparisonValue(float saturation, float targetSaturation,
        float luma, float targetLuma,
        int population, int highestPopulation)
        {
            return weightedMean(
                    invertDiff(saturation, targetSaturation), 3f,
                    invertDiff(luma, targetLuma), 6.5f,
                    population / (float)highestPopulation, 0.5f
            );
        }

        /**
         * Copy a {@link Swatch}'s HSL values into a new float[].
         */
        private static float[] copyHslValues(Swatch color)
        {
            float[] newHsl = new float[3];
            Array.Copy(color.getHsl(), 0, newHsl, 0, 3);
            return newHsl;
        }

        /**
         * Returns a value in the range 0-1. 1 is returned when {@code value} equals the
         * {@code targetValue} and then decreases as the absolute difference between {@code value} and
         * {@code targetValue} increases.
         *
         * @param value the item's value
         * @param targetValue the value which we desire
         */
        private static float invertDiff(float value, float targetValue)
        {
            return 1f - Math.Abs(value - targetValue);
        }

        private static float weightedMean(params float[] values)
        {
            float sum = 0f;
            float sumWeight = 0f;

            for (int i = 0; i < values.Length; i += 2)
            {
                float value = values[i];
                float weight = values[i + 1];

                sum += (value * weight);
                sumWeight += weight;
            }

            return sum / sumWeight;
        }
    }

    public static class WriteableBitmapExtension
    {
        public static void GetPixels(this WriteableBitmap bitmap, Color[] pixels, Int32 width, Int32 height)
        {
            if (bitmap != null && pixels != null)
            {
                for (Int32 i = 0; i < width; i++)
                {
                    for (Int32 j = 0; j < height; j++)
                    {
                        Int32 index = (i * height) + j;
                        pixels[index] = bitmap.GetPixel(i, j);
                    }
                }
                //IRandomAccessStream bitmapStream = new InMemoryRandomAccessStream();
                //await bitmap.ToStream(bitmapStream, new Guid());

                //var bitmapDecoder = await BitmapDecoder.CreateAsync(bitmapStream);

                //var pixelProvider = await bitmapDecoder.GetPixelDataAsync();

                //pixelProvider.DetachPixelData().CopyTo(pixels, pixels.Length);
            }
        }
    }
}
