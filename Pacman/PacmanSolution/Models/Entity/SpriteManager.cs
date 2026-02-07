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
    /// Se obtiene el bitmap devolviendolo y lo almacena
    /// </summary>
    /// <param name="imagenPath"></param>
    /// <returns></returns>
    public Bitmap? LoadSprite(string imagenPath)
     {
         if (_spriteCache.ContainsKey(imagenPath))
         {
            return _spriteCache[imagenPath];
         }
         try
         {
             Uri uri = new Uri($"avares://PacmanSolution/Assets/Imagen/SpritesPacman{imagenPath}");
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
     ///  verificamos si el bitmap cargado con el path es nulo o no
     /// si no lo es retoramos un croppedBitmap y la region que necesitamos
     /// </summary>
     /// <param name="path"></param>
     /// <param name="region"></param>
     /// <returns></returns>
     public CroppedBitmap? GetSpriteSection(string path, PixelRect region)
     {
         var fullBitmap = LoadSprite(path);
         if (fullBitmap == null) return null;

         // CroppedBitmap hereda de IImage, que es lo que Image de Avalonia necesita
         return new CroppedBitmap(fullBitmap, region);
     }
     
     /// <summary>
     /// se limpia todo
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