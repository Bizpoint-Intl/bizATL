using BizRAD.BizReport;
using BizRAD.BizXml;
using BizRAD.BizDocument;
using BizRAD.DB.Client;
using BizRAD.DB.Interface;
using BizRAD.BizApplication;
using BizRAD.BizControls.OutLookBar;
using BizRAD.BizControls.BizDateTimePicker;
using BizRAD.BizControls.DataGridColumns;
using BizRAD.BizCommon;
using BizRAD.BizAccounts;
using BizRAD.BizVoucher;
using BizRAD.BizBase;

using System;
using System.Data;
using System.Collections;
using System.Windows.Forms;
using System.Configuration;
using System.IO;
using System.Drawing;
using System.Globalization;
using System.Collections.Generic;
using System.Text;
using NodaTime;
using ATL.SortTable;

/// <summary>
/// Created By Jason
/// Used for Manipulating Time
/// </summary>

namespace ATL.TimeUtilites
{
    class TimeTools
    {
       

        public static LocalTime ParseMilitaryTime(string time)
        {

            string hour = time.Substring(0, 2);
            int hourInt = int.Parse(hour);
            if (hourInt >= 24)
            {
                throw new ArgumentOutOfRangeException("Invalid hour");
            }

            string minute = time.Substring(2, 2);
            int minuteInt = int.Parse(minute);
            if (minuteInt >= 60)
            {
                throw new ArgumentOutOfRangeException("Invalid minute");
            }

            return new LocalTime(hourInt, minuteInt, 0);       
        }

        public static string GetSafeMilitaryTimeOnly2(LocalTime time)
        {
            int hh, mm, ss = 0;

            hh = time.HourOfDay;
            mm = time.MinuteOfHour;
            
            string hhStr, mmStr = "";


            if (hh < 10)
            {
                hhStr = "0" + Convert.ToString(hh);
            }
            else
            {
                hhStr = Convert.ToString(hh);
            }

            if (mm < 10)
            {
                mmStr = "0" + Convert.ToString(mm);
            }
            else
            {
                mmStr = Convert.ToString(mm);
            }

         
            return hhStr + mmStr;

        }

       

        public static DateTime GetNextDateForDay(DateTime startDate, DayOfWeek desiredDay)
        {
            return startDate.AddDays(DaysToAdd(startDate.DayOfWeek, desiredDay));
        }
        
        public static int DaysToAdd(DayOfWeek current, DayOfWeek desired)
        {
            int c = (int)current;
            int d = (int)desired;
            int n = (7 - c + d);
         
            return (n > 7) ? n % 7 : n;
        }

     
        public static int GetDayNoOfWeek(string strDate)
        {
            string dateTmp = "";
            int dayNo = 0;
            int dd, mm, yyyy = 0;

            if (!BizFunctions.IsEmpty(strDate) || strDate != "")
            {

                dateTmp = strDate;
                yyyy = Convert.ToInt32(dateTmp.Substring(0, 4));
                mm = Convert.ToInt32(dateTmp.Substring(4, 2));
                dd = Convert.ToInt32(dateTmp.Substring(6, 2));
                LocalDate dt1 = new LocalDate(yyyy, mm, dd);
                dayNo = dt1.DayOfWeek;
                
            }
            return dayNo;
            
        }



        public static DayOfWeek GetDayOfWeek(string day)
        {
            DayOfWeek dt = new DayOfWeek();
            switch (day)
            {
                case "monday":
                    dt = DayOfWeek.Monday;
                    break;
                case "tuesday":
                    dt = DayOfWeek.Tuesday;
                    break;
                case "wednesday":
                    dt = DayOfWeek.Wednesday;
                    break;
                case "thursday":
                    dt = DayOfWeek.Thursday;
                    break;
                case "friday":
                    dt = DayOfWeek.Friday;
                    break;
                case "saturday":
                    dt = DayOfWeek.Saturday;
                    break;
                case "sunday":
                    dt = DayOfWeek.Sunday;
                    break; 
        
                    ////////


                case "Monday":
                    dt = DayOfWeek.Monday;
                    break;
                case "Tuesday":
                    dt = DayOfWeek.Tuesday;
                    break;
                case "Wednesday":
                    dt = DayOfWeek.Wednesday;
                    break;
                case "Thursday":
                    dt = DayOfWeek.Thursday;
                    break;
                case "Friday":
                    dt = DayOfWeek.Friday;
                    break;
                case "Saturday":
                    dt = DayOfWeek.Saturday;
                    break;
                case "Sunday":
                    dt = DayOfWeek.Sunday;
                    break;

            }
            return dt;
        }

        public static DataTable GetWeekFormat(int day)
        {
            DataTable WeekFormat = new DataTable("WeekFormat");
            switch (day)
            {
          
                case 1:
                    WeekFormat = GetWeekFormat1();
                    break;
                case 2:
                    WeekFormat = GetWeekFormat2();
                    break;
                case 3:
                    WeekFormat = GetWeekFormat3();
                    break;
                case 4:
                    WeekFormat = GetWeekFormat4();
                    break;
                case 5:
                    WeekFormat = GetWeekFormat5();
                    break;
                case 6:
                    WeekFormat = GetWeekFormat6();
                    break;
                case 7:
                    WeekFormat = GetWeekFormat7();
                    break;
            }
            return WeekFormat;
        }

        public static string GetDay(int day)
        {
            string dayinfo = "";

            switch (day)
            {
                case 1:
                    dayinfo = "Monday";
                    break;
                case 2:
                    dayinfo = "Tuesday";
                    break;
                case 3:
                    dayinfo = "Wednesday";
                    break;
                case 4:
                    dayinfo = "Thursday";
                    break;
                case 5:
                    dayinfo = "Friday";
                    break;
                case 6:
                    dayinfo = "Saturday";
                    break;
                case 7:
                    dayinfo = "Sunday";
                    break;

            }
            return dayinfo;
        }

        public static int GetDayOfWeekNo(string day)
        {
            int dayNo = 0;
          
            switch (day)
            {
                case "Sunday":
                    dayNo = 7;
                    break; 
                case "Monday":
                    dayNo = 1;
                    break;
                case "Tuesday":
                    dayNo = 2;
                    break;
                case "Wednesday":
                    dayNo = 3;
                    break;
                case "Thursday":
                    dayNo = 4;
                    break;
                case "Friday":
                    dayNo = 5;
                    break;
                case "Saturday":
                    dayNo = 6;
                    break;

                case "sunday":
                    dayNo = 7;
                    break;
                case "monday":
                    dayNo = 1;
                    break;
                case "tuesday":
                    dayNo = 2;
                    break;
                case "wednesday":
                    dayNo = 3;
                    break;
                case "thursday":
                    dayNo = 4;
                    break;
                case "friday":
                    dayNo = 5;
                    break;
                case "saturday":
                    dayNo = 6;
                    break;
                       
            }
            return dayNo;
        }

        public static DayOfWeek GetDayOfWeek2(int day)
        {
            DayOfWeek dt = new DayOfWeek();

            if (day > 7)
            {
                day = 1;
            }

            switch (day)
            {
                case 1:
                    dt = DayOfWeek.Monday;
                    break;
                case 2:
                    dt = DayOfWeek.Tuesday;
                    break;
                case 3:
                    dt = DayOfWeek.Wednesday;
                    break;
                case 4:
                    dt = DayOfWeek.Thursday;
                    break;
                case 5:
                    dt = DayOfWeek.Friday;
                    break;
                case 6:
                    dt = DayOfWeek.Saturday;
                    break;
                case 7:
                    dt = DayOfWeek.Sunday;
                    break;
            }
            return dt;
        }

        public static int daysTaken(string dateFromTmp, string dateToTmp)
        {
            
            int Dtaken = 0;
            int dd1, mm1, yyyy1, dd2, mm2, yyyy2 = 0;
           
                yyyy1 = Convert.ToInt32(dateFromTmp.Substring(0, 4));
                mm1 = Convert.ToInt32(dateFromTmp.Substring(4, 2));
                dd1 = Convert.ToInt32(dateFromTmp.Substring(6, 2));
                //LocalDate dt1 = new LocalDate(yyyy1, mm1, dd1);

                DateTime dt1 = new DateTime(yyyy1, mm1, dd1);

                yyyy2 = Convert.ToInt32(dateToTmp.Substring(0, 4));
                mm2 = Convert.ToInt32(dateToTmp.Substring(4, 2));
                dd2 = Convert.ToInt32(dateToTmp.Substring(6, 2));

                DateTime dt2 = new DateTime(yyyy2, mm2, dd2);
                //LocalDate dt2 = new LocalDate(yyyy2, mm2, dd2);

                TimeSpan tm1 = dt1 - dt2;
                Dtaken = Math.Abs(Convert.ToInt32(tm1.TotalDays));
            
            return Dtaken;

        }

        public static int CalculateAge(DateTime birthDate, DateTime now)
        {
            int age = now.Year - birthDate.Year;
            if (now.Month < birthDate.Month || (now.Month == birthDate.Month && now.Day < birthDate.Day)) age--;
            return age;
        }

        public static int CalculateYears(DateTime fromDate, DateTime now)
        {
            int years = now.Year - fromDate.Year;
            if (now.Month < fromDate.Month || (now.Month == fromDate.Month && now.Day < fromDate.Day)) years--;
            return years;
        }

        public static decimal CalculateExactYears(DateTime fromDate, DateTime now)
        {
            decimal years = now.Year - fromDate.Year;
            if (now.Month < fromDate.Month || (now.Month == fromDate.Month && now.Day < fromDate.Day)) years--;
            return years;
        }

        public static int TotelMonthDifference(DateTime dtThis, DateTime dtOther)
        {
            int intReturn = 0;

            dtThis = dtThis.Date.AddDays(-(dtThis.Day-1));
            dtOther = dtOther.Date.AddDays(-(dtOther.Day-1));

            while (dtOther.Date > dtThis.Date)
            {
                intReturn++;     
                dtThis = dtThis.AddMonths(1);
            }

            return intReturn;
        }



        public static DateTime GetSafeDate(string date)
        {
            int dd, mm, yyyy = 0;

            yyyy = Convert.ToInt32(date.Substring(0, 4));
            mm = Convert.ToInt32(date.Substring(4, 2));
            dd = Convert.ToInt32(date.Substring(6, 2));
         
            DateTime dt = new DateTime(yyyy, mm, dd);

            return dt;

        }

        public static DateTime GetDateOfYear(int days, int year)
        {
            int YY = year;
            int DD = days;

           DateTime d = new DateTime(YY, 1, 1).AddDays(DD - 1);


           return d;
            //MessageBox.Show(d.ToShortDateString() + "\n" + d.DayOfYear.ToString());
        }

        public static string GetSafeDateOnly(DateTime date)
        {
            int dd, mm, yyyy = 0;

            yyyy = date.Year;
            mm = date.Month;
            dd = date.Day;
            string mmStr, ddStr = "";


            if (mm < 10)
            {
                mmStr = "0" + Convert.ToString(mm);
            }
            else
            {
                mmStr = Convert.ToString(mm);
            }

            if (dd < 10)
            {
                ddStr = "0" + Convert.ToString(dd);
            }
            else
            {
                ddStr = Convert.ToString(dd);
            }            
            return Convert.ToString(yyyy) + "-" + mmStr + "-" + ddStr;

        }

        public static string GetStandardSafeDateOnly(DateTime date)
        {
            int dd, mm, yyyy = 0;

            yyyy = date.Year;
            mm = date.Month;
            dd = date.Day;
            string mmStr, ddStr = "";


            if (mm < 10)
            {
                mmStr = "0" + Convert.ToString(mm);
            }
            else
            {
                mmStr = Convert.ToString(mm);
            }

            if (dd < 10)
            {
                ddStr = "0" + Convert.ToString(dd);
            }
            else
            {
                ddStr = Convert.ToString(dd);
            }
            return ddStr + "/" + mmStr + "/" + Convert.ToString(yyyy);

        }

        public static string GetStandardSafeDateOnly2(DateTime date)
        {
            int dd, mm, yyyy = 0;

            yyyy = date.Year;
            mm = date.Month;
            dd = date.Day;
            string mmStr, ddStr = "";


            if (mm < 10)
            {
                mmStr = "0" + Convert.ToString(mm);
            }
            else
            {
                mmStr = Convert.ToString(mm);
            }

            if (dd < 10)
            {
                ddStr = "0" + Convert.ToString(dd);
            }
            else
            {
                ddStr = Convert.ToString(dd);
            }
            return mmStr + "/" + ddStr + "/" + Convert.ToString(yyyy);

        }

        public static string GetStandardSafeDateOnly3(DateTime date)
        {
            int dd, mm, yyyy = 0;

            yyyy = date.Year;
            mm = date.Month;
            dd = date.Day;
            string mmStr, ddStr = "";


            if (mm < 10)
            {
                mmStr = "0" + Convert.ToString(mm);
            }
            else
            {
                mmStr = Convert.ToString(mm);
            }

            if (dd < 10)
            {
                ddStr = "0" + Convert.ToString(dd);
            }
            else
            {
                ddStr = Convert.ToString(dd);
            }
            return mmStr+ddStr+Convert.ToString(yyyy);

        }

        public static string GetSafeTimeOnly(DateTime date)
        {
            int hh, mm, ss = 0;

            hh = date.Hour;
            mm = date.Minute;
            ss = date.Second;
            string hhStr, mmStr, ssStr = "";


            if (hh < 10)
            {
                hhStr = "0" + Convert.ToString(hh);
            }
            else
            {
                hhStr = Convert.ToString(hh);
            }

            if (mm < 10)
            {
                mmStr = "0" + Convert.ToString(mm);
            }
            else
            {
                mmStr = Convert.ToString(mm);
            }

            if (ss < 10)
            {
                ssStr = "0" + Convert.ToString(ss);
            }
            else
            {
                ssStr = Convert.ToString(ss);
            }
            return hhStr + ":" + mmStr + ":" + ss;

        }

        public static string GetSafeMilitaryTimeOnly1(DateTime date)
        {
            int hh, mm, ss = 0;

            hh = date.Hour;
            mm = date.Minute;
            ss = date.Second;
            string hhStr, mmStr, ssStr = "";


            if (hh < 10)
            {
                hhStr = "0" + Convert.ToString(hh);
            }
            else
            {
                hhStr = Convert.ToString(hh);
            }

            if (mm < 10)
            {
                mmStr = "0" + Convert.ToString(mm);
            }
            else
            {
                mmStr = Convert.ToString(mm);
            }

            if (ss < 10)
            {
                ssStr = "0" + Convert.ToString(ss);
            }
            else
            {
                ssStr = Convert.ToString(ss);
            }
            return hhStr +  mmStr;

        }

        public static int[,] GetMonthYear(int mm, int yyyy, int monthno)
        {
            int MM, YYYY = 0;
            int[,] MonthYear;

            string mmStr = "";

            if(mm>=10)
            {
                mmStr = mm.ToString();
            }
            else
            {
                mmStr = "0" + mm.ToString();
            }

            string DateString = Convert.ToString(yyyy) + Convert.ToString(mmStr) + "01";

            DateTime dt = GetSafeDate(DateString).AddMonths(monthno);



            YYYY = dt.Year;
            MM = dt.Month;

            MonthYear = new int[1, 2];

            MonthYear[0, 0] = MM;
            MonthYear[0, 1] = YYYY;
        

            return MonthYear;
        }


        public static string GetSafeDateString(int mm, int yyyy, int dd)
        {

            string DateString = "";

            string mmStr,yyStr,ddStr = "";

            if (dd >= 10)
            {
                ddStr = dd.ToString();
            }
            else
            {
                ddStr = "0" + dd.ToString();
            }

            if(mm>=10)
            {
                mmStr = mm.ToString();
            }
            else
            {
                mmStr = "0" + mm.ToString();
            }

            DateString = Convert.ToString(yyyy) + Convert.ToString(mmStr) + Convert.ToString(ddStr);



            return DateString;
        }

        
   

        public static DataTable WeekTable(string datefrom, string dateto)
        {    
            int startingDayNo = GetDayNoOfWeek(datefrom);
            int endingDayNo = GetDayNoOfWeek(dateto);
            int sequence = 0;
            int count=0;
            DateTime tmpDT = new DateTime();

            DataTable weekTB = new DataTable("weekTB");

            weekTB = GetWeekFormat(startingDayNo); // Most Important

            SortDT sort = new SortDT(weekTB, "SequenceNo");

            DataTable sortedweekTB = sort.SortedTable();


            for (int i = 0; i <= sortedweekTB.Rows.Count - 1; i++)
            {

                int tmpWeekDayNo = (int)sortedweekTB.Rows[i]["WeekOfDayNo"];
                if (startingDayNo == tmpWeekDayNo)
                {
                    sortedweekTB.Rows[i]["Date"] = GetSafeDate(datefrom);
                    tmpDT = GetSafeDate(datefrom);
                }
                else
                {
                    sortedweekTB.Rows[i]["Date"] = GetNextDateForDay(tmpDT, GetDayOfWeek2((GetDayOfWeekNo(sortedweekTB.Rows[i]["Day"].ToString()))));
                    tmpDT = Convert.ToDateTime(sortedweekTB.Rows[i]["Date"]);
                }


            }

            //foreach (DataRow dr1 in sortedweekTB.Rows)
            //{

            //    int tmpWeekDayNo = (int)dr1["WeekOfDayNo"];
            //    if (startingDayNo == tmpWeekDayNo)
            //    {
            //        dr1["Date"] = GetSafeDate(datefrom);
            //        tmpDT = GetSafeDate(datefrom);
            //    }
            //    else
            //    {
            //        dr1["Date"] = GetNextDateForDay(tmpDT, GetDayOfWeek2((GetDayOfWeekNo(dr1["Day"].ToString()))));
            //        tmpDT = Convert.ToDateTime(dr1["Date"]);
            //    }
            //}

            return sortedweekTB;
        }

      
        

        public static DateTime GetLastOccurenceOfDay(DateTime value, DayOfWeek dayOfWeek)
         {
               int daysToAdd = dayOfWeek - value.DayOfWeek;
               //if(daysToAdd < 1) 
               //{
               //      daysToAdd -= 7;
               //} 
               return value.AddDays(daysToAdd);
         }

        public static DateTime GetFirstDayOfWeek(int year, int weekNumber, DayOfWeek dayOfWeek)
         {
               return GetLastOccurenceOfDay(new DateTime(year,1,1).AddDays(7*weekNumber), dayOfWeek);
         }
         
        public static DateTime GetFirstDayOfWeek(int year,int weekNumber)
         {
               return GetFirstDayOfWeek(year,weekNumber, DayOfWeek.Monday);
         }

        
        public static int GetYearWeekNumber(DateTime dtPassed)
        {
         
            CultureInfo ciCurr = CultureInfo.CurrentCulture;
     
            int weekNum = ciCurr.Calendar.GetWeekOfYear(dtPassed, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
     
            return weekNum;
         
        }

        public static int GetTotalDayMonth(DateTime date)
        {
            int mm, yyyy = 0;

            yyyy = date.Year;
            mm = date.Month;

            return System.DateTime.DaysInMonth(yyyy, mm);
        }

        public static DateTime GetLastDateMonth(DateTime date)
        {
            DateTime lastDate;

            int mm, yyyy, dd = 0;

            yyyy = date.Year;
            mm = date.Month;
            dd = GetTotalDayMonth(date);

            lastDate = Convert.ToDateTime(dd.ToString() + "-" + mm.ToString() + "-" + yyyy.ToString());

            return lastDate;
        }

        public static int GetWeeksInYear(int year)
        {
            DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
            DateTime date1 = new DateTime(year, 12, 31);
            Calendar cal = dfi.Calendar;
            return cal.GetWeekOfYear(date1, dfi.CalendarWeekRule,
                                                dfi.FirstDayOfWeek);
        }

  

        public static DataTable GetWeekFormat1()
        {
            DataTable weekFormat1 = new DataTable("weekTB");

            weekFormat1.Columns.Add("Day", typeof(string));
            weekFormat1.Columns.Add("Date", typeof(DateTime));
            weekFormat1.Columns.Add("WeekOfDayNo", typeof(int));
            weekFormat1.Columns.Add("SequenceNo", typeof(int));


            weekFormat1.Rows.Add("Monday", System.DBNull.Value, System.DBNull.Value, 1);
            weekFormat1.Rows.Add("Tuesday", System.DBNull.Value, System.DBNull.Value, 2);
            weekFormat1.Rows.Add("Wednesday", System.DBNull.Value, System.DBNull.Value, 3);
            weekFormat1.Rows.Add("Thursday", System.DBNull.Value, System.DBNull.Value, 4);
            weekFormat1.Rows.Add("Friday", System.DBNull.Value, System.DBNull.Value, 5);
            weekFormat1.Rows.Add("Saturday", System.DBNull.Value, System.DBNull.Value, 6);
            weekFormat1.Rows.Add("Sunday", System.DBNull.Value, System.DBNull.Value, 7);

            foreach (DataRow dr1 in weekFormat1.Rows)
            {
                dr1["WeekOfDayNo"] = (int)GetDayOfWeekNo(dr1["day"].ToString());

            }

            return weekFormat1;
        }

        public static DataTable GetWeekFormat2()
        {
            DataTable weekFormat2 = new DataTable("weekTB");

            weekFormat2.Columns.Add("Day", typeof(string));
            weekFormat2.Columns.Add("Date", typeof(DateTime));
            weekFormat2.Columns.Add("WeekOfDayNo", typeof(int));
            weekFormat2.Columns.Add("SequenceNo", typeof(int));


            weekFormat2.Rows.Add("Monday", System.DBNull.Value, System.DBNull.Value, 7);
            weekFormat2.Rows.Add("Tuesday", System.DBNull.Value, System.DBNull.Value, 1);
            weekFormat2.Rows.Add("Wednesday", System.DBNull.Value, System.DBNull.Value, 2);
            weekFormat2.Rows.Add("Thursday", System.DBNull.Value, System.DBNull.Value, 3);
            weekFormat2.Rows.Add("Friday", System.DBNull.Value, System.DBNull.Value, 4);
            weekFormat2.Rows.Add("Saturday", System.DBNull.Value, System.DBNull.Value, 5);
            weekFormat2.Rows.Add("Sunday", System.DBNull.Value, System.DBNull.Value, 6);

            foreach (DataRow dr1 in weekFormat2.Rows)
            {
                dr1["WeekOfDayNo"] = (int)GetDayOfWeekNo(dr1["day"].ToString());

            }

            return weekFormat2;
        }

        public static DataTable GetWeekFormat3()
        {
            DataTable weekFormat3 = new DataTable("weekTB");

            weekFormat3.Columns.Add("Day", typeof(string));
            weekFormat3.Columns.Add("Date", typeof(DateTime));
            weekFormat3.Columns.Add("WeekOfDayNo", typeof(int));
            weekFormat3.Columns.Add("SequenceNo", typeof(int));


            weekFormat3.Rows.Add("Monday", System.DBNull.Value, System.DBNull.Value, 6);
            weekFormat3.Rows.Add("Tuesday", System.DBNull.Value, System.DBNull.Value, 7);
            weekFormat3.Rows.Add("Wednesday", System.DBNull.Value, System.DBNull.Value, 1);
            weekFormat3.Rows.Add("Thursday", System.DBNull.Value, System.DBNull.Value, 2);
            weekFormat3.Rows.Add("Friday", System.DBNull.Value, System.DBNull.Value, 3);
            weekFormat3.Rows.Add("Saturday", System.DBNull.Value, System.DBNull.Value, 4);
            weekFormat3.Rows.Add("Sunday", System.DBNull.Value, System.DBNull.Value, 5);

            foreach (DataRow dr1 in weekFormat3.Rows)
            {
                dr1["WeekOfDayNo"] = (int)GetDayOfWeekNo(dr1["day"].ToString());

            }

            return weekFormat3;
        }

        public static DataTable GetWeekFormat4()
        {
            DataTable weekFormat4 = new DataTable("weekTB");

            weekFormat4.Columns.Add("Day", typeof(string));
            weekFormat4.Columns.Add("Date", typeof(DateTime));
            weekFormat4.Columns.Add("WeekOfDayNo", typeof(int));
            weekFormat4.Columns.Add("SequenceNo", typeof(int));


            weekFormat4.Rows.Add("Monday", System.DBNull.Value, System.DBNull.Value, 5);
            weekFormat4.Rows.Add("Tuesday", System.DBNull.Value, System.DBNull.Value, 6);
            weekFormat4.Rows.Add("Wednesday", System.DBNull.Value, System.DBNull.Value, 7);
            weekFormat4.Rows.Add("Thursday", System.DBNull.Value, System.DBNull.Value, 1);
            weekFormat4.Rows.Add("Friday", System.DBNull.Value, System.DBNull.Value, 2);
            weekFormat4.Rows.Add("Saturday", System.DBNull.Value, System.DBNull.Value, 3);
            weekFormat4.Rows.Add("Sunday", System.DBNull.Value, System.DBNull.Value, 4);

            foreach (DataRow dr1 in weekFormat4.Rows)
            {
                dr1["WeekOfDayNo"] = (int)GetDayOfWeekNo(dr1["day"].ToString());

            }

            return weekFormat4;
        }

        public static DataTable GetWeekFormat5()
        {
            DataTable weekFormat5 = new DataTable("weekTB");

            weekFormat5.Columns.Add("Day", typeof(string));
            weekFormat5.Columns.Add("Date", typeof(DateTime));
            weekFormat5.Columns.Add("WeekOfDayNo", typeof(int));
            weekFormat5.Columns.Add("SequenceNo", typeof(int));


            weekFormat5.Rows.Add("Monday", System.DBNull.Value, System.DBNull.Value, 4);
            weekFormat5.Rows.Add("Tuesday", System.DBNull.Value, System.DBNull.Value, 5);
            weekFormat5.Rows.Add("Wednesday", System.DBNull.Value, System.DBNull.Value, 6);
            weekFormat5.Rows.Add("Thursday", System.DBNull.Value, System.DBNull.Value, 7);
            weekFormat5.Rows.Add("Friday", System.DBNull.Value, System.DBNull.Value, 1);
            weekFormat5.Rows.Add("Saturday", System.DBNull.Value, System.DBNull.Value, 2);
            weekFormat5.Rows.Add("Sunday", System.DBNull.Value, System.DBNull.Value, 3);

            foreach (DataRow dr1 in weekFormat5.Rows)
            {
                dr1["WeekOfDayNo"] = (int)GetDayOfWeekNo(dr1["day"].ToString());

            }

            return weekFormat5;
        }

        public static DataTable GetWeekFormat6()
        {
            DataTable weekFormat6 = new DataTable("weekTB");

            weekFormat6.Columns.Add("Day", typeof(string));
            weekFormat6.Columns.Add("Date", typeof(DateTime));
            weekFormat6.Columns.Add("WeekOfDayNo", typeof(int));
            weekFormat6.Columns.Add("SequenceNo", typeof(int));


            weekFormat6.Rows.Add("Monday", System.DBNull.Value, System.DBNull.Value, 3);
            weekFormat6.Rows.Add("Tuesday", System.DBNull.Value, System.DBNull.Value, 4);
            weekFormat6.Rows.Add("Wednesday", System.DBNull.Value, System.DBNull.Value, 5);
            weekFormat6.Rows.Add("Thursday", System.DBNull.Value, System.DBNull.Value, 6);
            weekFormat6.Rows.Add("Friday", System.DBNull.Value, System.DBNull.Value, 7);
            weekFormat6.Rows.Add("Saturday", System.DBNull.Value, System.DBNull.Value, 1);
            weekFormat6.Rows.Add("Sunday", System.DBNull.Value, System.DBNull.Value, 2);

            foreach (DataRow dr1 in weekFormat6.Rows)
            {
                dr1["WeekOfDayNo"] = (int)GetDayOfWeekNo(dr1["day"].ToString());

            }

            return weekFormat6;
        }

        public static DataTable GetWeekFormat7()
        {
            DataTable weekFormat7 = new DataTable("weekTB");

            weekFormat7.Columns.Add("Day", typeof(string));
            weekFormat7.Columns.Add("Date", typeof(DateTime));
            weekFormat7.Columns.Add("WeekOfDayNo", typeof(int));
            weekFormat7.Columns.Add("SequenceNo", typeof(int));


            weekFormat7.Rows.Add("Monday", System.DBNull.Value, System.DBNull.Value, 2);
            weekFormat7.Rows.Add("Tuesday", System.DBNull.Value, System.DBNull.Value, 3);
            weekFormat7.Rows.Add("Wednesday", System.DBNull.Value, System.DBNull.Value, 4);
            weekFormat7.Rows.Add("Thursday", System.DBNull.Value, System.DBNull.Value, 5);
            weekFormat7.Rows.Add("Friday", System.DBNull.Value, System.DBNull.Value, 6);
            weekFormat7.Rows.Add("Saturday", System.DBNull.Value, System.DBNull.Value, 7);
            weekFormat7.Rows.Add("Sunday", System.DBNull.Value, System.DBNull.Value, 1);

            foreach (DataRow dr1 in weekFormat7.Rows)
            {
                dr1["WeekOfDayNo"] = (int)GetDayOfWeekNo(dr1["day"].ToString());

            }

            return weekFormat7;
        }

        //public static int GetMonthDifference(DateTime startDate,DateTime endDate)
        //{
        //    // if dates were passed in wrong order, swap 'em
        //    if (startDate > endDate)
        //    {
        //        DateTime temp = startDate;
        //        startDate = endDate;
        //        endDate = temp;
        //    }

        //    int count = 0;
        //    DateTime tempDate = startDate;

        //    while ((tempDate = GetNextMonth(tempDate)) <= endDate)
        //    {
        //        count++;
        //    }
           
        //    return count;
        //}

        //public static DateTime GetNextMonth(DateTime date)
        //{
        //    int month = date.Month;
        //    int day = date.Day;
        //    int year = date.Year;

        //    int nextDateMonth = month == 12 ? 1 : month + 1;
        //    int nextDateYear = month == 12 ? year + 1 : year;

        //    DateTime nextDate;

        //    while (!DateTime.TryParse(nextDateMonth + "/" + day
        //      + "/" + nextDateYear, out nextDate))
        //    {
        //        // if it didn't parse right, 
        //        // then the month must not have that many days
        //        day--;
        //    }

        //    return nextDate;
        //}


        public static int MonthDiff(DateTime d1, DateTime d2)
        {

            int monthsApart = 12 * (d1.Year - d2.Year) + d1.Month - d2.Month;
            if (d1.Day < d2.Day)
            {

                monthsApart--;

            }

            return System.Math.Abs(monthsApart);
        }




    }
}
