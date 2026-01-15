namespace AtomicHabits.Models
{
    public class Streak
    {
        public int Id { get; set; }

        public int HabitId { get; set; }
        public Habit Habit { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        // Current Streak
        public int CurrentStreak { get; set; } = 0;
        public DateTime? CurrentStreakStartDate { get; set; }

        // Best Streak
        public int BestStreak { get; set; } = 0;
        public DateTime? BestStreakStartDate { get; set; }
        public DateTime? BestStreakEndDate { get; set; }

        // Statistics
        public int TotalCompletions { get; set; } = 0;
        public int TotalDaysTracked { get; set; } = 0;
        public float CompletionRate { get; set; }

        public DateTime? LastUpdated { get; set; }
    }

}
