using BO;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace PL.Pages
{
    public class DronesConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not ObservableCollection<Drone> drones) return null;
            /*
            bool? boxChecked = ((CheckBox)parameter).IsChecked;
            bool isCollected = boxChecked.HasValue ? boxChecked.Value : false;
            */
            bool isCollected = true;
            if (isCollected)
            {
                return drones.GroupBy(d => d.State);
            }
            else
            {
                return drones;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
