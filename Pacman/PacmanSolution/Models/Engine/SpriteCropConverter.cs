using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia;
using Avalonia.Media.Imaging;

namespace PacmanSolution.Models.Engine;

public class SpriteCropConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        // values[0] = Bitmap? Sprite
        // values[1] = Rect SourceRect
        if (values[0] is not Bitmap bitmap) return null;
        if (values[1] is not Rect sourceRect) return null;
        if (sourceRect == default) return bitmap; // si no hay rect, devuelve el sprite completo

        try
        {
            // Recorta el sprite sheet según el SourceRect
            var pixelSize = new PixelSize((int)sourceRect.Width, (int)sourceRect.Height);
            var cropped   = new RenderTargetBitmap(pixelSize);

            using var ctx = cropped.CreateDrawingContext();
            ctx.DrawImage(bitmap,
                sourceRect,                                          // origen en el sprite sheet
                new Rect(0, 0, sourceRect.Width, sourceRect.Height)); // destino
            
            return cropped;
        }
        catch
        {
            return bitmap;
        }
    }
}