using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace PacmanSolution.Models;

public class SpriteManager
{
    private readonly Dictionary<string, Bitmap> _spriteCache = new();
    /// <summary>
    /// Get the bitmap return and store
    /// </summary>
    /// <param name="imagenPath"></param>
    /// <returns></returns>
    private Bitmap? LoadSprite(string imagenPath)
     {
         if (_spriteCache.TryGetValue(imagenPath, out var sprite))
         {
            return sprite;
         }
         try
         {
             Uri uri = new Uri($"avares://PacmanSolution/Assets/Imagen/SpritesPacman/{imagenPath}");
             var asset = AssetLoader.Open(uri);
             var bitmap = new Bitmap(asset);
 
             _spriteCache[imagenPath] = bitmap;
             return bitmap;
         }
         catch
         {
             Console.WriteLine("Failed to load imagen");
             return null;
         }
     }
 
     public static Rect CreateSpriteRect(int x, int y, int width, int height)
     {
         return new Rect(x, y, width, height);
     }
     /// <summary>
     ///  check si el bitmap update con el path es null o no
     /// si no lo es retoramos un croppedBitmap y la region que need
     /// para heredared
     /// </summary>
     /// <param name="path"></param>
     /// <param name="region"></param>
     /// <returns></returns>
     public CroppedBitmap? GetSpriteSection(string path, PixelRect region)
     {
         var fullBitmap = LoadSprite(path);
         if (fullBitmap == null) return null;
         return new CroppedBitmap(fullBitmap, region);
     }
     
     /// <summary>
     /// clean all
     /// </summary>
     public void ClearCache()
     {
         foreach (Bitmap bitmap in _spriteCache.Values)
         {
             bitmap.Dispose();
         }
         _spriteCache.Clear(); 
     }
}