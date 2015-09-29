using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace GitList.Core.Entities.Converters
{
    public class ObservableCollectionToMultiStringConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            if (targetType != typeof(string))
                throw new InvalidOperationException(
                      "The target must be a String");

            return String.Join("\r\n",((ObservableCollection<string>)value[0]).ToArray().Reverse());
        }

        public object[] ConvertBack(object value, Type[] targetType,
                                  object parameter, CultureInfo culture)
        {
            if (targetType[0] != typeof(ObservableCollection<string>))
                throw new InvalidOperationException(
                      "The target must be an ObservableCollection<string>");

            var list =
                     new ObservableCollection<string>();

            foreach (string s in value.ToString().Split('\r'))
            {
                string val = s.Replace('\n', ' ');
                val = val.Trim();
                if (!string.IsNullOrEmpty(val))
                {
                    list.Add(val);
                }
            }

            return list.ToArray();
        }


    }
}
