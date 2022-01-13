// File NotNullConvertor.cs created by Yoni Fram and Gil Kovshi
// All rights reserved

using System;
using System.Globalization;
using System.Windows.Data;

namespace PL.Pages
{
    public class NotNullConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
                return false;

            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
