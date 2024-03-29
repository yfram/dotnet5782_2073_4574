﻿// File PackageIdConverter.cs created by Yoni Fram and Gil Kovshi
// All rights reserved

using System;
using System.Globalization;
using System.Windows.Data;

namespace PL.Pages
{
    internal class PackageIdConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not int valueAsInt)
                return null;

            if (valueAsInt < 0)
                return String.Empty;
            else if (parameter == null)
                return value;

            return parameter as string switch
            {
                "sender" => BlApi.BlFactory.GetBl().GetPackageById(valueAsInt).Sender.Name,
                "receiver" => BlApi.BlFactory.GetBl().GetPackageById(valueAsInt).Reciver.Name,
                _ => throw new NotImplementedException()
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
