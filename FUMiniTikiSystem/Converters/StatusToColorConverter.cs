using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace FUMiniTikiSystem.Converters
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string status)
            {
                return status.ToLower() switch
                {
                    "pending" => new SolidColorBrush(Color.FromRgb(255, 193, 7)),      // Yellow
                    "processing" => new SolidColorBrush(Color.FromRgb(23, 162, 184)),   // Cyan
                    "shipped" => new SolidColorBrush(Color.FromRgb(40, 167, 69)),       // Green
                    "delivered" => new SolidColorBrush(Color.FromRgb(40, 167, 69)),     // Green
                    "cancelled" => new SolidColorBrush(Color.FromRgb(220, 53, 69)),     // Red
                    _ => new SolidColorBrush(Color.FromRgb(108, 117, 125))             // Gray (default)
                };
            }
            
            return new SolidColorBrush(Color.FromRgb(108, 117, 125)); // Gray (default)
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 