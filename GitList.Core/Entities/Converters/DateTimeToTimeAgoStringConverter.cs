using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace GitList.Core.Entities.Converters
{
    public class DateTimeToTimeAgoStringConverter : IValueConverter
    {
        public CharacterCasing Case { get; set; }

        public DateTimeToTimeAgoStringConverter()
        {
            Case = CharacterCasing.Normal;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var dateTime = value as DateTime? ?? new DateTime();
            switch (Case)
            {
                case CharacterCasing.Lower:
                    return FeedDateTimeMinutesAgo(dateTime).ToLower();
                case CharacterCasing.Normal:
                    return FeedDateTimeMinutesAgo(dateTime);
                case CharacterCasing.Upper:
                    return FeedDateTimeMinutesAgo(dateTime).ToUpper();
                default:
                    return FeedDateTimeMinutesAgo(dateTime);
            }
            return string.Empty;
        }

        private string FeedDateTimeMinutesAgo(DateTime dateTime)
        {
            TimeSpan span = DateTime.Now - dateTime;

            if (span.Days > 365)
            {
                int years = (span.Days/365);
                if (span.Days%365 != 0)
                    years += 1;
                return String.Format("About {0} {1} ago", years, years == 1 ? "year" : "years");
            }

            if (span.Days > 30)
            {
                int months = (span.Days/30);
                if (span.Days%31 != 0)
                    months += 1;
                return String.Format("About {0} {1} ago", months, months == 1 ? "month" : "months");
            }

            if (span.Days > 0) return String.Format("About {0} {1} ago",span.Days, span.Days == 1 ? "day" : "days");
            
            if (span.Hours > 0) return String.Format("About {0} {1} ago", span.Hours, span.Hours == 1 ? "hour" : "hours");
            
            if (span.Minutes > 0) return String.Format("About {0} {1} ago", span.Minutes, span.Minutes == 1 ? "minute" : "minutes");
            
            if (span.Seconds > 5) return String.Format("About {0} seconds ago", span.Seconds);
            
            if (span.Seconds <= 5) return "Just now";
            
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
