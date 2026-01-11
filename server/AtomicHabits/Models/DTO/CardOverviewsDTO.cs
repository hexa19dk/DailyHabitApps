namespace AtomicHabits.Models.DTO
{
    public class CardOverviewsDTO
    {
    }

    public class DashboardOverviewDto
    {
        public TodayCardDto Today { get; set; }
        public StreakCardDto Streak { get; set; }
        public WeeklyCardDto WeeklyConsistency { get; set; }
        public List<MonthlyTrendDto> MonthlyTrend { get; set; }
    }

    public class TodayCardDto
    {
        public int Completed { get; set; }
        public int Total { get; set; }
    }

    public class StreakCardDto
    {
        public int CurrentStreak { get; set; }
        public int LongestStreak { get; set; }
    }

    public class WeeklyCardDto
    {
        public int CompletionRate { get; set; }
    }

    public class MonthlyTrendDto
    {
        public string Month { get; set; } = string.Empty;
        public int CompletionRate { get; set; }
    }


}
