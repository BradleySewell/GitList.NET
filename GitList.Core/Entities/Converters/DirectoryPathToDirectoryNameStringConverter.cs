using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace GitList.Core.Entities.Converters
{
    public class DirectoryPathToDirectoryNameStringConverter : IValueConverter
    {

        public CharacterCasing Case { get; set; }

        public DirectoryPathToDirectoryNameStringConverter()
        {
            Case = CharacterCasing.Upper;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var str = value as string;
            if (str != null && Directory.Exists(str))
            {
                if (Path.GetPathRoot(str) != str)
                {
                    str = Path.GetFileName(str);
                }

                switch (Case)
                {
                    case CharacterCasing.Lower:
                        return str.ToLower();
                    case CharacterCasing.Normal:
                        return str;
                    case CharacterCasing.Upper:
                        return str.ToUpper();
                    default:
                        return str;
                }
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
