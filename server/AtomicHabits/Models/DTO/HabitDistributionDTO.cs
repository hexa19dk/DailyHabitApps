namespace AtomicHabits.Models.DTO
{
    public class HabitDistributionDTO
    {
        public List<string> Labels { get; set; } = new();
        public List<int> Values { get; set; } = new();
    }

    public class HabitDistributionRequestDTO
    {
        public int HabitId { get; set; } 
        public int UserId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int StartYear { get; set; }
        public int EndYear { get; set; }
    }

    public class WeeklyDistributionDTO
    {
        public int HabitId { get; set; }
        public int UserId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
    }

    public class MonthlyDistributionDTO
    {
        public int HabitId { get; set; }
        public int UserId { get; set; }
        public int Year { get; set; }
    }

    public class YearlyDistributionDTO
    {
        public int HabitId { get; set; }
        public int UserId { get; set; }
        public int StartYear { get; set; }
        public int EndYear { get; set; }
    }
}
