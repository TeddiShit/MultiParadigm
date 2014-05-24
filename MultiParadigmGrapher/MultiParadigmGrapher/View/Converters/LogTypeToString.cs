using MultiParadigmGrapher.GraphFunctions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MultiParadigmGrapher.View.Converters
{
    [ValueConversion(typeof(LogType), typeof(string))]
    public class LogTypeToString : IValueConverter
    {
        Dictionary<LogType, string> mapping = new Dictionary<LogType, string> 
        { 
            {LogType.StdOut, "Output"},
            {LogType.StdErr, "Error output"},
            {LogType.SyntaxErr, "Syntax error"},
            {LogType.SchemeError, "Scheme error"},
            {LogType.GeneralError, "Error"},
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is LogType)
            {
                return mapping[(LogType)value];
            }

            throw new ArgumentException("Value must be of type LogType.");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
