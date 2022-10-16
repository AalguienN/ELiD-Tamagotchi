using System;
using System.Collections.Generic;

public class MonthConstants
{    
    public static int[] monthDays = new int[] { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31, 29 };
    public static string[] monthNames = new string[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };

    public static Month GetMonth(int monthNumber, int yearNumber) {
        Month month = new Month();
        month.Name = monthNames[monthNumber];
        if(DateTime.IsLeapYear(yearNumber) && monthNumber == 1) {
            month.Days = monthDays[12];
        }
        else {
            month.Days = monthDays[monthNumber];
        }
        return month;
    }
}
