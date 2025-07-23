using System;
using System.Globalization;
using System.Windows.Data;

namespace HikariApp.Converters
{
    /// <summary>
    /// Converter to convert boolean IsCompleted status to display text
    /// </summary>
    public class BooleanToStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isCompleted)
            {
                return isCompleted ? "Đã hoàn thành" : "Chưa hoàn thành";
            }
            return "N/A";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
