using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PL.Pages
{
    internal class PackageIdConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not int valueAsInt) return null;
            if (valueAsInt < 0) return String.Empty;
            else if (parameter == null) return value;
            return parameter as string switch
            {
                "sender" => BlApi.BlFactory.GetBl().GetPackageById(valueAsInt).Sender.Name,
                "reciver" => BlApi.BlFactory.GetBl().GetPackageById(valueAsInt).Reciver.Name,
                _ => throw new NotImplementedException()
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
