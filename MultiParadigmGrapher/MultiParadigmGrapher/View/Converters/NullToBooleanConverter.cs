﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MultiParadigmGrapher.View.Converters
{
    /// <summary>
    /// Converts null to boolean. 
    /// </summary>
    /// <remarks>
    /// If parameter is not specified or specified with null:
    /// if passed value is null then true is returned, otherwise false. 
    /// If parameter is specified then inversion is done:
    /// if passed value is null then false is returned, otherwise true.
    /// </remarks>
    [ValueConversion(typeof(object), typeof(bool))]
    public class NullToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
