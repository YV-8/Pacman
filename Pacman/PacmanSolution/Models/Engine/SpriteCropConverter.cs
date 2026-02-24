using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;

namespace PacmanSolution.Models.Engine;

public class SpriteCropConverter : IMultiValueConverter
{
    /// <summary>
    /// Converts a bitmap and a source rectangle into a cropped portion of that bitmap
    /// Expects exactly two values: a <see cref="Bitmap"/> and an <see cref="Avalonia.Rect"/>
    /// Returns null if inputs are invalid, the full bitmap if no valid rect is provided,
    /// or a <see cref="CroppedBitmap"/> with the specified region
    /// </summary>
    /// <param name="values">A list containing the source bitmap and the crop rectangle.</param>
    /// <param name="targetType">The target type of the binding.</param>
    /// <param name="parameter">Optional converter parameter (not used).</param>
    /// <param name="culture">The culture info for the conversion.</param>
    /// <returns>A cropped bitmap, the original bitmap, or null.</returns>
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count != 2)
        {
            return null;
        }

        if (values[0] is not Bitmap bitmap)
        {
            return null;
        }

        if (values[1] is not Avalonia.Rect sourceRect)
        {
            return bitmap;
        }

        if (sourceRect.Width > 0 && sourceRect.Height > 0)
        {
            return new CroppedBitmap(bitmap, new Avalonia.PixelRect(
                (int)sourceRect.X,
                (int)sourceRect.Y,
                (int)sourceRect.Width,
                (int)sourceRect.Height));
        }

        return bitmap;
    }
}