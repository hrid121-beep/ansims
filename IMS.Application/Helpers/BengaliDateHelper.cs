using System;
using System.Collections.Generic;
using System.Linq;

namespace IMS.Application.Helpers
{
    /// <summary>
    /// Helper class for converting dates to Bengali format
    /// </summary>
    public static class BengaliDateHelper
    {
        // Bengali digits
        private static readonly Dictionary<char, char> EnglishToBengaliDigits = new Dictionary<char, char>
        {
            {'0', '০'}, {'1', '১'}, {'2', '২'}, {'3', '৩'}, {'4', '৪'},
            {'5', '৫'}, {'6', '৬'}, {'7', '৭'}, {'8', '৮'}, {'9', '৯'}
        };

        // Bengali month names (Gregorian calendar)
        private static readonly Dictionary<int, string> BengaliMonthsGregorian = new Dictionary<int, string>
        {
            {1, "জানুয়ারি"}, {2, "ফেব্রুয়ারি"}, {3, "মার্চ"}, {4, "এপ্রিল"},
            {5, "মে"}, {6, "জুন"}, {7, "জুলাই"}, {8, "আগস্ট"},
            {9, "সেপ্টেম্বর"}, {10, "অক্টোবর"}, {11, "নভেম্বর"}, {12, "ডিসেম্বর"}
        };

        // Bengali month names (Bengali calendar)
        private static readonly string[] BengaliMonthsBangla = new[]
        {
            "বৈশাখ", "জ্যৈষ্ঠ", "আষাঢ়", "শ্রাবণ", "ভাদ্র", "আশ্বিন",
            "কার্তিক", "অগ্রহায়ণ", "পৌষ", "মাঘ", "ফাল্গুন", "চৈত্র"
        };

        /// <summary>
        /// Converts English digits to Bengali digits
        /// Example: "2025" -> "২০২৫"
        /// </summary>
        public static string ConvertToBengaliDigits(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return new string(input.Select(c => EnglishToBengaliDigits.ContainsKey(c) ? EnglishToBengaliDigits[c] : c).ToArray());
        }

        /// <summary>
        /// Converts a number to Bengali digits
        /// Example: 2025 -> "২০২৫"
        /// </summary>
        public static string ConvertToBengaliDigits(int number)
        {
            return ConvertToBengaliDigits(number.ToString());
        }

        /// <summary>
        /// Gets Bengali month name for Gregorian calendar
        /// Example: 8 -> "আগস্ট"
        /// </summary>
        public static string GetBengaliMonthNameGregorian(int month)
        {
            return BengaliMonthsGregorian.TryGetValue(month, out var name) ? name : "";
        }

        /// <summary>
        /// Converts Gregorian date to Bengali formatted date
        /// Example: 2025-08-25 -> "২৫ আগস্ট ২০২৫ খ্রিস্টাব্দ"
        /// </summary>
        public static string ConvertToGregorianBengali(DateTime date)
        {
            var day = ConvertToBengaliDigits(date.Day);
            var month = GetBengaliMonthNameGregorian(date.Month);
            var year = ConvertToBengaliDigits(date.Year);

            return $"{day} {month} {year} খ্রিস্টাব্দ";
        }

        /// <summary>
        /// Converts Gregorian date to Bengali calendar date
        /// Example: 2025-08-25 -> "১০ ভাদ্র ১৪৩২ বঙ্গাব্দ"
        ///
        /// Note: This is an approximation. Bengali calendar follows solar calculation
        /// and the actual conversion may vary by a day or two depending on the year.
        /// </summary>
        public static string ConvertToBengaliCalendar(DateTime gregorianDate)
        {
            // Bengali calendar starts on 14 April (Pahela Baishakh)
            // Bengali year is approximately Gregorian year - 593/594 depending on the date

            int bengaliYear;
            int bengaliMonth;
            int bengaliDay;

            // Bengali calendar months have fixed days:
            // First month (Baishakh) starts on April 14/15
            // Months 1-6 (Baishakh to Ashwin): 31 days each
            // Months 7-11 (Kartik to Falgun): 30 days each
            // Month 12 (Chaitra): 30 days (31 in leap year)

            // Determine Bengali year
            if (gregorianDate.Month < 4 || (gregorianDate.Month == 4 && gregorianDate.Day < 14))
            {
                // Before April 14: Previous Bengali year
                bengaliYear = gregorianDate.Year - 594;
            }
            else
            {
                // On or after April 14: Current Bengali year
                bengaliYear = gregorianDate.Year - 593;
            }

            // Calculate days from start of Bengali year
            var bengaliYearStart = new DateTime(
                gregorianDate.Month >= 4 && gregorianDate.Day >= 14 ? gregorianDate.Year : gregorianDate.Year - 1,
                4, 14
            );

            int daysFromYearStart = (int)(gregorianDate - bengaliYearStart).TotalDays;

            // Determine Bengali month and day
            if (daysFromYearStart < 0)
            {
                // Should not happen with correct logic, but handle edge case
                bengaliYearStart = bengaliYearStart.AddYears(-1);
                daysFromYearStart = (int)(gregorianDate - bengaliYearStart).TotalDays;
            }

            // Calculate month and day
            int[] monthDays = { 31, 31, 31, 31, 31, 31, 30, 30, 30, 30, 30, 30 }; // Days in each Bengali month

            bengaliMonth = 0;
            bengaliDay = daysFromYearStart + 1; // +1 because day 0 is day 1 of Baishakh

            for (int i = 0; i < 12; i++)
            {
                if (bengaliDay <= monthDays[i])
                {
                    bengaliMonth = i;
                    break;
                }
                bengaliDay -= monthDays[i];
            }

            // Convert to Bengali digits
            var day = ConvertToBengaliDigits(bengaliDay);
            var month = BengaliMonthsBangla[bengaliMonth];
            var year = ConvertToBengaliDigits(bengaliYear);

            return $"{day} {month} {year} বঙ্গাব্দ";
        }

        /// <summary>
        /// Gets both Bengali calendar and Gregorian dates formatted in Bengali
        /// Returns tuple of (BengaliCalendar, GregorianBengali)
        /// Example: ("১০ ভাদ্র ১৪৩২ বঙ্গাব্দ", "২৫ আগস্ট ২০২৫ খ্রিস্টাব্দ")
        /// </summary>
        public static (string bengaliCalendar, string gregorianBengali) GetBothFormats(DateTime date)
        {
            return (ConvertToBengaliCalendar(date), ConvertToGregorianBengali(date));
        }

        /// <summary>
        /// Gets complete date string with both formats on separate lines
        /// Example:
        /// "১০ ভাদ্র ১৪৩২ বঙ্গাব্দ
        ///  ২৫ আগস্ট ২০২৫ খ্রিস্টাব্দ"
        /// </summary>
        public static string GetCompleteDateString(DateTime date)
        {
            var (bengaliCalendar, gregorianBengali) = GetBothFormats(date);
            return $"{bengaliCalendar}\n{gregorianBengali}";
        }
    }
}
