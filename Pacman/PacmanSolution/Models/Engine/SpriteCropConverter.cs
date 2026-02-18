using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;

namespace PacmanSolution.Models.Engine;

public class SpriteCropConverter : IMultiValueConverter
{
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
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}