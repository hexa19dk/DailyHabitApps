namespace AtomicHabits.Models.DTO
{
    public class HabitSummaryDto
    {
        public TodaySummaryDto TodaySummary { get; set; }
        public WeeklySummaryDto WeeklySummary { get; set; }
        public MonthlySummaryDto MonthlySummary { get; set; }
        public int HabitHealthScore { get; set; }
    }


    public class TodaySummaryDto
    {
        public int HabitsToday { get; set; }
        public int CompletedToday { get; set; }
        public int RemainingToday => HabitsToday - CompletedToday;
        public int TodayCompletionRate { get; set; }
    }


    public class WeeklySummaryDto
    {
        public int WeeklyCompletionRate { get; set; }
        public int TotalCompletedThisWeek { get; set; }
    }


    public class MonthlySummaryDto
    {
        public int MonthlyCompletionRate { get; set; }
        public int TotalMonthlySessions { get; set; }
    }
}
