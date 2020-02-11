using System;
using System.Text.RegularExpressions;
using static System.Int32;
using static System.String;

namespace GovUk.Education.ExploreEducationStatistics.Common.Model
{
    public class PartialDate
    {
        public static readonly Regex YearRegex = new Regex(@"^([0-9]{4})?$");
        public static readonly Regex MonthRegex = new Regex(@"^([0-9]{1,2})?$");
        public static readonly Regex DayRegex = new Regex(@"^([0-9]{1,2})?$");
        private string _year;
        private string _month;
        private string _day;

        public string Year
        {
            get => _year ?? "";
            set => _year = value;
        }
        
        public string Month
        {
            get => _month ?? "";
            set => _month = value;
        }
        
        public string Day
        {
            get => _day ?? "";
            set => _day = value;
        }

        public bool IsValid()
        {
            if (!YearRegex.Match(Year).Success || !MonthRegex.Match(Month).Success || !DayRegex.Match(Day).Success)
            {
                return false; // Failed rudimentary number validation  
            }

            if (!EmptyOrBetween(Month, 1, 12) || !EmptyOrBetween(Day, 1, 31))
            {
                return false; // Failed more precise number validation
            }

            if (!IsNullOrEmpty(Month) && !IsNullOrEmpty(Day))
            {
                // We at least have a month and a day so at the very least we can check that they are acceptable
                // together. If we have a year we can do even more if we do not then we use a leap year as this gives a
                // wider range of acceptable values.
                const int leapYear = 2016;
                var yearToCheckAgainst = !IsNullOrEmpty(Year) ? Parse(Year) : leapYear;
                var intDay = Parse(Day);
                var intMonth = Parse(Month);
                var daysInMonth = DateTime.DaysInMonth(yearToCheckAgainst, intMonth);
                if (intDay > daysInMonth || intDay < 1)
                {
                    return false; // Failed more specific day / month validation
                }
            }

            return true;
        }

        private static bool EmptyOrBetween(string value, int lower, int upper)
        {
            return IsNullOrEmpty(value) || (Parse(value) >= lower && Parse(value) <= upper);
        }
    }
}