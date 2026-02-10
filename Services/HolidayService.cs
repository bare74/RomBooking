namespace RomBooking.Services;

public interface IHolidayService
{
    bool IsHoliday(DateTime date);
    List<DateTime> GetHolidays(int year);
}

public class NorwegianHolidayService : IHolidayService
{
    public bool IsHoliday(DateTime date)
    {
        var holidays = GetHolidays(date.Year);
        return holidays.Any(h => h.Date == date.Date);
    }
    
    public List<DateTime> GetHolidays(int year)
    {
        var holidays = new List<DateTime>();
        
        // Faste helligdager
        holidays.Add(new DateTime(year, 1, 1));   // Nyttårsdag
        holidays.Add(new DateTime(year, 5, 1));   // 1. mai
        holidays.Add(new DateTime(year, 5, 17));  // 17. mai
        holidays.Add(new DateTime(year, 12, 25)); // 1. juledag
        holidays.Add(new DateTime(year, 12, 26)); // 2. juledag
        
        // Bevegelige helligdager (basert på påske)
        var easter = CalculateEaster(year);
        holidays.Add(easter.AddDays(-3));  // Skjærtorsdag
        holidays.Add(easter.AddDays(-2));  // Langfredag
        holidays.Add(easter);              // Påskedag
        holidays.Add(easter.AddDays(1));   // 2. påskedag
        holidays.Add(easter.AddDays(39));  // Kristi himmelfartsdag
        holidays.Add(easter.AddDays(49));  // Pinsedag
        holidays.Add(easter.AddDays(50));  // 2. pinsedag
        
        return holidays;
    }
    
    private DateTime CalculateEaster(int year)
    {
        // Meeus/Jones/Butcher algoritme for påskeberegning
        int a = year % 19;
        int b = year / 100;
        int c = year % 100;
        int d = b / 4;
        int e = b % 4;
        int f = (b + 8) / 25;
        int g = (b - f + 1) / 3;
        int h = (19 * a + b - d - g + 15) % 30;
        int i = c / 4;
        int k = c % 4;
        int l = (32 + 2 * e + 2 * i - h - k) % 7;
        int m = (a + 11 * h + 22 * l) / 451;
        int month = (h + l - 7 * m + 114) / 31;
        int day = ((h + l - 7 * m + 114) % 31) + 1;
        
        return new DateTime(year, month, day);
    }
}
