using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FUMiniTikiSystem.Converters
{
    public class StringToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string path && !string.IsNullOrWhiteSpace(path))
            {
                try
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    
                    // Check if it's a URL (starts with http/https)
                    if (path.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || 
                        path.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                    {
                        bitmap.UriSource = new Uri(path);
                    }
                    else
                    {
                        // It's a local path, make it relative to the application
                        if (path.StartsWith("/"))
                        {
                            path = path.Substring(1); // Remove leading slash
                        }
                        bitmap.UriSource = new Uri($"pack://application:,,,/{path}");
                    }
                    
                    bitmap.EndInit();
                    return bitmap;
                }
                catch (Exception ex)
                {
                    // Log error for debugging (you can remove this in production)
                    System.Diagnostics.Debug.WriteLine($"Failed to load image from path: {path}, Error: {ex.Message}");
                    return null;
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 