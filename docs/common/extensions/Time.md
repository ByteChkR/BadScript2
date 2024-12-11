# Time Reference


```csharp
class Time
{
        public static Time Zero;
        public static Time Parse(string s, string culture?!);

        public Time(){}
        public Time(string s){}
        public Time(num ms){}
        public Time(num h, num m, num s){}
        public Time(num h, num m, num s, num ms){}

        public num Hours;
        public num Minutes;
        public num Seconds;
        public num Milliseconds;
        public num Ticks;
        public num TotalHours;
        public num TotalMinutes;
        public num TotalSeconds;
        public num TotalMilliseconds;
        public Time Time.Negate();

        public Type GetType();
        public bool IsInstanceOf(any prototype!);
        
        public string ToString(string format?!, string culture?!);

        public Time Add(Time time!);
        public Time Subtract(Time time!);

        public Time op_Add(Time time!);
        public Time op_Subtract(Time time!);
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