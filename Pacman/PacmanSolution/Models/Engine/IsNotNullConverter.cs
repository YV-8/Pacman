using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace PacmanSolution.Models.Engine;

public class IsNotNullConverter : IValueConverter
{
    /// <summary>
    /// Convert values of UI to WPF
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value != null;
    }

    /// <summary>
    /// User binding of UI to XAML if exist or not them
    /// Launch not implement exception because not build  
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}