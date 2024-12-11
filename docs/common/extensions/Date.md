# Date Reference


```csharp
class Date 
{

        public static Date Now;
        public static Date UtcNow;

        public static Date Parse(string s, string culture?!);

        public Date(){}
        public Date(string s){}
        public Date(num ms){}
        public Date(num year, num month, num day){}
        public Date(num year, num month, num day, num hour, num minute, num second){}
        public Date(num year, num month, num day, num hour, num minute, num second, num millisecond){}
        public Date(num year, num month, num day, num hour, num minute, num second, num millisecond, Time offset){}

        public num Year;
        public num Month;
        public num Day;
        public num Hour;
        public num Minute;
        public num Second;
        public num Millisecond;
        public num DayOfWeek;
        public num DayOfYear;
        public Time TimeOfDay;
        public num UnixTimeMilliseconds;
        public num UnixTimeSeconds;
        public Time Offset;
        public num WeekOfYear(string culture?);
        public Date ToUniversalTime();
        public Date ToLocalTime();
        public Date ToOffset(Time offset!);

        public Type GetType();
        public bool IsInstanceOf(any prototype!);

        public string ToString(string format?!, string culture?!);
        public string ToShortTimeString(string timeZone?);
        public string ToShortTimeString(string timeZone?);
        public string ToShortDateString(string timeZone?);
        public string ToLongTimeString(string timeZone?);
        public string ToLongDateString(string timeZone?);
        public string Format(string format!, string timeZone?, string culture?);

        public Date AddYears(num years!);
        public Date AddMonths(num months!);
        public Date AddDays(num days!);
        public Date AddHours(num hours!);
        public Date AddMinutes(num minutes!);
        public Date AddSeconds(num seconds!);
        public Date AddMilliseconds(num ms!);
        public Date AddTicks(num ticks!);
        public Date Add(Time time!);

        public Date Subtract(Time time!);

        public Date op_Add(Time time!);
        public Date op_Subtract(Time time!);
        public bool op_Equal(any right);
        public bool op_NotEqual(any right);
        public bool op_Greater(Date right);
        public bool op_GreaterOrEqual(Date right);
        public bool op_Less(Date right);
        public bool op_LessOrEqual(Date right);
}
```

___

## Links

[Home](../../Readme.md)

[Getting Started](../../GettingStarted.md)